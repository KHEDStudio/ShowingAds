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
        private Thread _networkThread;

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
            _networkThread = new Thread(() => InitializeHubConnection(hubReconnectionInterval));
            _networkThread.Start();
            _timerPeriodicRequest = new System.Timers.Timer();
            _timerPeriodicRequest.Elapsed += TimerRequestCallback;
            _timerPeriodicRequest.AutoReset = false;
            _nextRequestTime = DateTime.Now;
            _lastMessageUUID = Guid.Empty;
        }

        private void TimerRequestCallback(object sender, ElapsedEventArgs e)
        {
            try
            {
                SendDiagnosticInfo();
                if (_nextRequestTime.CompareTo(DateTime.Now) < 0)
                    SendRequest();
            }
            catch (Exception ex)
            {
                ServerLog.Error("ChannelUpdated", ex.Message);
            }
            finally
            {
                _timerPeriodicRequest.Start();
            }
        }

        private void SendDiagnosticInfo()
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
                        client.PostAsync(new Uri(Settings.DeviceControllerPath, $"content?count={info.ReadyVideosCount}"), default).Wait();
                }
            }
            catch (Exception ex)
            {
                ServerLog.Error("SendDiagnosticInfo", ex.Message);
            }
        }
        
        private void InitializeHubConnection(TimeSpan hubReconnectionInterval)
        {
            while (_notifyHub == null)
            {
                try
                {
                    _notifyHub = new HubConnectionBuilder()
                    .WithUrl(Settings.NotifyPath)
                    .Build();
                    _notifyHub.Closed += (error) =>
                    {
                        ServerLog.Error("NotifyHubClosed", error.Message);
                        StartNotifyConnection(hubReconnectionInterval);
                        return Task.CompletedTask;
                    };
                    _notifyHub.On<Guid>("ChannelUpdated", async (messageUUID) =>
                    {
                        await _notifyHub.InvokeAsync("MessageUUID", messageUUID);
                        if (_lastMessageUUID != messageUUID)
                        {
                            _lastMessageUUID = messageUUID;
                            try
                            {
                                SendRequest();
                            } 
                            catch (Exception ex)
                            {
                                ServerLog.Error("ChannelUpdated", ex.Message);
                            }
                        }
                    });
                    StartNotifyConnection(hubReconnectionInterval);
                }
                catch (Exception ex)
                {
                    _notifyHub.IfNotNull(() => _notifyHub.DisposeAsync());
                    _notifyHub = null;
                    ServerLog.Error("NotifyConnection", ex.Message);
                }
                finally
                {
                    Thread.Sleep(hubReconnectionInterval);
                }
            }
        }

        private void StartNotifyConnection(TimeSpan reconnectionInterval)
        {
            while (_notifyHub.State != HubConnectionState.Connected)
            {
                try
                {
                    _notifyHub.StartAsync().Wait();
                    if (Settings.DeviceId != Guid.Empty)
                        _notifyHub.SendAsync("ClientConnectedAsync", Settings.DeviceId).Wait();
                }
                catch (Exception ex)
                {
                    ServerLog.Error("NotifyConnection", ex.Message);
                }
                finally
                {
                    Thread.Sleep(reconnectionInterval);
                }
            }
        }

        public void SendRequest()
        {
            using (var handler = new HttpClientHandler())
            {
                handler.CookieContainer = _cookieContainer;
                using (var client = new HttpClient(handler))
                {
                    var responseMessage = client.GetAsync(Settings.ChannelPath).Result;
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        var jsonResponseMessage = responseMessage.Content.ReadAsStringAsync().Result;
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
