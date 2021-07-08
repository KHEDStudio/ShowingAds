using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShowingAds.AndroidApp.Core.Extensions;
using ShowingAds.AndroidApp.Core.Network.Interfaces;
using ShowingAds.AndroidApp.Core.Network.Parsers.Interfaces;
using ShowingAds.Shared.Core.Converters;
using ShowingAds.Shared.Core.Enums;
using ShowingAds.Shared.Core.Models.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.AndroidApp.Core.Network
{
    public class NetworkClient : IClient
    {
        private TaskFactory _taskFactory;

        private readonly object _syncTasks = new object();
        private DeviceTasks _tasks = new DeviceTasks(Settings.DeviceId);

        private readonly AutoResetEvent _syncInfo = new AutoResetEvent(true);
        private DiagnosticInfo _info = new DiagnosticInfo(Settings.DeviceId);

        private readonly object _syncRoot = new object();
        private CookieContainer _cookieContainer;

        private IParser _parser;
        private System.Timers.Timer _timerPeriodicRequest;

        private DateTime _nextRequestTime;
        private DateTime _nextDiagnosticTime;

        public event Action TakeScreenshotChanged;
        public event Action<Guid> PriorityClientChanged;

        public NetworkClient(IParser parser, CookieContainer cookieContainer)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _cookieContainer = cookieContainer ?? throw new ArgumentNullException(nameof(cookieContainer));
            _taskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                TaskContinuationOptions.None, TaskScheduler.Current);
            _timerPeriodicRequest = new System.Timers.Timer();
            _timerPeriodicRequest.Elapsed += TimerRequestCallback;
            _timerPeriodicRequest.AutoReset = false;
            _nextRequestTime = DateTime.Now;
            _nextDiagnosticTime = DateTime.Now;
        }

        private void TimerRequestCallback(object sender, ElapsedEventArgs e)
        {
            _taskFactory.StartNew(async () =>
            {
                try
                {
                    if (_nextDiagnosticTime < DateTime.Now)
                        await SendDiagnosticInfoAsync().ConfigureAwait(false);
                    if (_nextRequestTime < DateTime.Now || _tasks.IsUpdated)
                        await SendRequestAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    ServerLog.Error("TimerRequestCallback", ex.Message);
                }
                finally
                {
                    _timerPeriodicRequest.Start();
                }
            }).ConfigureAwait(false);
        }

        private async Task TakeScreenshotAsync()
        {
            try
            {
                TakeScreenshotChanged?.Invoke();
                var screenshotPath = Settings.GetLogotypeFilesPath("screenshot.png");
                if (File.Exists(screenshotPath))
                {
                    var fileInfo = new FileInfo(screenshotPath);
                    using (var fileStream = new FileStream(screenshotPath, FileMode.Open))
                    {
                        using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(10) })
                        {
                            var content = new MultipartFormDataContent();
                            content.Add(new StreamContent(fileStream), "file", fileInfo.Name);
                            var response = await httpClient.PostAsync(new Uri(Settings.FilesServicePath, "screenshot"), content).ConfigureAwait(false);
                            if (response.IsSuccessStatusCode)
                            {
                                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                                var fileResponse = JsonConvert.DeserializeObject<FileUploadResponse>(json);
                                _info.Screenshot = fileResponse.FileGuid.ToString();

                                using (var handler = new HttpClientHandler())
                                {
                                    handler.CookieContainer = _cookieContainer;
                                    using (var client = new HttpClient(handler))
                                        await client.PostAsync(new Uri(Settings.DeviceControllerPath, "screenshot"), default).ConfigureAwait(false);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServerLog.Error("TakeScreenshotAsync", ex.Message);
            }
        }

        private class FileUploadResponse
        {
            [JsonProperty("file"), JsonConverter(typeof(GuidConverter))]
            public Guid FileGuid { get; private set; }
            [JsonProperty("duration"), JsonConverter(typeof(TimeSpanConverter))]
            public TimeSpan Duration { get; private set; }
        }

        private async Task SendDiagnosticInfoAsync()
        {
            try
            {
                _syncInfo.WaitOne();
                using (var handler = new HttpClientHandler())
                {
                    handler.CookieContainer = _cookieContainer;
                    using (var client = new HttpClient(handler))
                    {
                        var json = JsonConvert.SerializeObject(_info);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(new Uri(Settings.DeviceControllerPath, $"info"), content).ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            json = response.Content.ReadAsStringAsync().Result;
                            _tasks = JsonConvert.DeserializeObject<DeviceTasks>(json);
                            if (_tasks.TakeScreenshot)
                                await TakeScreenshotAsync().ConfigureAwait(false);
                            PriorityClientChanged?.Invoke(_tasks.PriorityAdvertisingClient);
                        }
                    }
                }
                _nextDiagnosticTime = DateTime.Now.AddSeconds((5, 10).RandomNumber());

                _info = new DiagnosticInfo(_info.Id, _info.DeviceStatus, _info.Version, _info.ReadyVideosCount, _info.ClientVideos, new Dictionary<string, int>(),
                    _info.ClientVideosWillShow, _info.DownloadType, _info.DownloadProgress, _info.DownloadSpeed,
                    _info.CurrentVideo, _info.Screenshot, _info.FreeSpaceDisk, new List<string>());
            }
            catch (Exception ex)
            {
                ServerLog.Error("SendDiagnosticInfo", ex.Message);
            }
            finally
            {
                _syncInfo.Set();
            }
        }

        public async Task SendRequestAsync()
        {
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    handler.CookieContainer = _cookieContainer;
                    using (var client = new HttpClient(handler))
                    {
                        var responseMessage = await client.GetAsync(Settings.ChannelPath).ConfigureAwait(false);
                        if (responseMessage.StatusCode == HttpStatusCode.OK)
                        {
                            var jsonResponseMessage = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                            lock (_syncRoot)
                                if (string.IsNullOrEmpty(jsonResponseMessage) == false)
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
            catch (Exception ex)
            {
                ServerLog.Error("SendRequest", ex.Message);
            }
        }

        public void StartPeriodicTimerRequest(TimeSpan interval)
        {
            ServerLog.Debug("Timer", $"Request every {interval} seconds");
            _timerPeriodicRequest.Interval = interval.TotalMilliseconds;
            _timerPeriodicRequest.Start();
        }

        public async Task DisposeAsync()
        {
            await Task.Yield();
            _timerPeriodicRequest.Dispose();
        }

        public void SetDiagnosticInfo(DiagnosticInfo info)
        {
            try
            {
                _syncInfo.WaitOne();
                var showeds = _info.ClientVideosShowed;
                foreach (var showed in info.ClientVideosShowed)
                {
                    if (showeds.ContainsKey(showed.Key))
                        showeds[showed.Key] += showed.Value;
                    else
                        showeds.Add(showed.Key, showed.Value);
                }

                var logs = new List<string>(_info.Logs);
                logs.AddRange(info.Logs);

                _info = new DiagnosticInfo(info.Id, info.DeviceStatus, info.Version, info.ReadyVideosCount, info.ClientVideos, showeds,
                    info.ClientVideosWillShow, info.DownloadType, info.DownloadProgress, info.DownloadSpeed,
                    info.CurrentVideo, _info.Screenshot, info.FreeSpaceDisk, logs);
            }
            catch (Exception ex)
            {
                ServerLog.Error("SetDiagnosticInfo", ex.Message);
            }
            finally
            {
                _syncInfo.Set();
            }
        }
    }
}
