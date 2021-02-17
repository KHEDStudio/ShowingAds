using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.Extensions;
using ShowingAds.AndroidApp.Core.Network.Interfaces;
using ShowingAds.AndroidApp.Core.Network.Models;
using ShowingAds.AndroidApp.Core.Network.Parsers.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.AndroidApp.Core.Network
{
    public class NetworkClient : IClient
    {
        private readonly object _syncDiagnosticInfo = new object();
        private DiagnosticInfo _info = new DiagnosticInfo(0);

        private readonly object _syncRoot = new object();
        private CookieContainer _cookieContainer;

        private IParser _parser;
        private HubConnection _notifyHub;
        private System.Timers.Timer _timerPeriodicRequest;
        private DateTime _nextRequestTime;
        private Guid _lastMessageUUID;

        public HubConnectionState ConnectionState => _notifyHub.IfNotNull(() => _notifyHub.State);

        public NetworkClient(IParser parser, CookieContainer cookieContainer, TimeSpan hubReconnectionInterval)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _cookieContainer = cookieContainer ?? throw new ArgumentNullException(nameof(cookieContainer));
            Task.Factory.StartNew(async () => await InitializeHubConnection(hubReconnectionInterval),
                TaskCreationOptions.LongRunning);
            _timerPeriodicRequest = new System.Timers.Timer();
            _timerPeriodicRequest.Elapsed += TimerRequestCallback;
            _timerPeriodicRequest.AutoReset = false;
            _nextRequestTime = DateTime.Now;
            _lastMessageUUID = Guid.Empty;
        }

        private async void TimerRequestCallback(object sender, ElapsedEventArgs e)
        {
            try
            {
                await SendDiagnosticInfo();
                if (_nextRequestTime.CompareTo(DateTime.Now) < 0)
                    await SendRequest();
            }
            catch (Exception ex)
            {
                await ServerLog.Error("ChannelUpdated", ex.Message);
            }
            finally
            {
                _timerPeriodicRequest.Start();
            }
        }

        private async Task SendDiagnosticInfo()
        {
            try
            {
                DiagnosticInfo info;
                lock (_syncDiagnosticInfo)
                    info = _info.Clone() as DiagnosticInfo;
                using (var handler = new HttpClientHandler())
                {
                    handler.CookieContainer = _cookieContainer;
                    using (var client = new HttpClient(handler))
                        await client.PostAsync(new Uri(Settings.DeviceControllerPath, $"content?count={info.ReadyVideosCount}"), default);
                }
            }
            catch (Exception ex)
            {
                await ServerLog.Error("SendDiagnosticInfo", ex.Message);
            }
        }
        
        private async Task InitializeHubConnection(TimeSpan hubReconnectionInterval)
        {
            while (_notifyHub == null)
            {
                try
                {
                    _notifyHub = new HubConnectionBuilder()
                    .WithUrl(Settings.NotifyPath)
                    .Build();
                    _notifyHub.Closed += async (error) =>
                    {
                        await ServerLog.Error("NotifyHubClosed", error.Message);
                        await StartNotifyConnection(hubReconnectionInterval);
                    };
                    _notifyHub.On<Guid>("ChannelUpdated", async (messageUUID) =>
                    {
                        await _notifyHub.InvokeAsync("MessageUUID", messageUUID);
                        if (_lastMessageUUID != messageUUID)
                        {
                            _lastMessageUUID = messageUUID;
                            try
                            {
                                await SendRequest();
                            } 
                            catch (Exception ex)
                            {
                                await ServerLog.Error("ChannelUpdated", ex.Message);
                            }
                        }
                    });
                    await StartNotifyConnection(hubReconnectionInterval);
                }
                catch (Exception ex)
                {
                    await _notifyHub.IfNotNull(() => _notifyHub.DisposeAsync());
                    _notifyHub = null;
                    await ServerLog.Error("NotifyConnection", ex.Message);
                }
                finally
                {
                    await Task.Delay(hubReconnectionInterval);
                }
            }
        }

        private async Task StartNotifyConnection(TimeSpan reconnectionInterval)
        {
            while (_notifyHub.State != HubConnectionState.Connected)
            {
                try
                {
                    await _notifyHub.StartAsync();
                    if (Settings.DeviceId != Guid.Empty)
                        await _notifyHub.SendAsync("ClientConnectedAsync", Settings.DeviceId);
                }
                catch (Exception ex)
                {
                    await ServerLog.Error("NotifyConnection", ex.Message);
                }
                finally
                {
                    await Task.Delay(reconnectionInterval);
                }
            }
        }

        public async Task SendRequest()
        {
            using (var handler = new HttpClientHandler())
            {
                handler.CookieContainer = _cookieContainer;
                using (var client = new HttpClient(handler))
                {
                    var responseMessage = await client.GetAsync(Settings.ChannelPath);
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        var jsonResponseMessage = await responseMessage.Content.ReadAsStringAsync();
                        lock (_syncRoot)
                            _parser.Parse(jsonResponseMessage);
                    }
                    if (responseMessage.StatusCode == HttpStatusCode.NoContent)
                        lock (_syncRoot)
                            _parser.Parse(string.Empty);
                }
            }
            _nextRequestTime = DateTime.Now.AddMinutes((5, 10).RandomNumber());
            ServerLog.Debug("NetworkClient", $"Next request {_nextRequestTime}");
        }

        public void StartPeriodicTimerRequest(TimeSpan interval)
        {
            ServerLog.Debug("Timer", $"Request every {interval} seconds");
            _timerPeriodicRequest.Interval = interval.TotalMilliseconds;
            _timerPeriodicRequest.Start();
        }

        public async Task DisposeAsync()
        {
            await _notifyHub.IfNotNull(() => _notifyHub.DisposeAsync());
            _timerPeriodicRequest.Dispose();
        }

        public void SetDiagnosticInfo(DiagnosticInfo info)
        {
            lock (_syncDiagnosticInfo)
                _info = info;
        }
    }
}
