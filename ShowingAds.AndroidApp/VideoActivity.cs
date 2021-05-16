using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.IO;
using ShowingAds.AndroidApp.Core;
using ShowingAds.AndroidApp.Core.BusinessCollections;
using ShowingAds.AndroidApp.Core.BusinessCollections.Factory;
using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.DataAccess;
using ShowingAds.AndroidApp.Core.DataAccess.Interfaces;
using ShowingAds.AndroidApp.Core.Extensions;
using ShowingAds.AndroidApp.Core.Network;
using ShowingAds.AndroidApp.Core.Network.Interfaces;
using ShowingAds.AndroidApp.Core.Network.Parsers;
using ShowingAds.AndroidApp.Core.Network.Parsers.Interfaces;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octane.Xamarin.Forms.VideoPlayer;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using ShowingAds.Shared.Core.Models.Login;
using ShowingAds.Shared.Core.Models.States;
using ShowingAds.Shared.Core.Models.Json;

namespace ShowingAds.AndroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class VideoActivity : AppCompatActivity
    {
        private Thread _loadVideoThread;
        private Thread _downloadThread;

        private VideoView _videoView;
        private TickerView _tickerView;
        private ImageView _leftLogo;
        private ImageView _rightLogo;

        private TextView _logInfo1;
        private TextView _logInfo2;
        private TextView _logInfo3;

        private System.Timers.Timer _diagnosticTimer;

        private IClient _networkClient;

        private IExecutor<VideoEventArgs> _readyExecutor;
        private IExecutor<VideoEventArgs> _clientsExecutor;
        private IExecutor<LogoEventArgs> _logotypesExecutor;

        private IDataStore<LoginDevice> _loginStore;

        private readonly AutoResetEvent _videoShowed = new AutoResetEvent(true);
        private readonly AutoResetEvent _lastAdsShowed = new AutoResetEvent(false);

        private uint _timerCounter = 0;
        private System.Timers.Timer _advertisingTimer;
        private BlockingCollection<Video> _advertisingVideos = new BlockingCollection<Video>();

        private readonly object _syncCurrentReservedVideos = new object();
        private (bool, Video) _currentVideo = (false, null);
        private (bool, Video, int) _reservedVideo = (false, null, 0);

        private TopLevelCollection<Video> _readyVideos;
        private readonly AutoResetEvent _syncContents = new AutoResetEvent(true);
        private TopLevelCollection<LowLevelCollection<Video>> _contents;
        private readonly AutoResetEvent _syncClients = new AutoResetEvent(true);
        private TopLevelCollection<LowLevelCollection<Video>> _clients;
        private readonly AutoResetEvent _syncIntervals = new AutoResetEvent(true);
        private TopLevelCollection<ClientInterval> _intervals;
        private readonly AutoResetEvent _syncOrders = new AutoResetEvent(true);
        private TopLevelCollection<ClientOrder> _orders;

        private readonly AutoResetEvent _syncLogotypes = new AutoResetEvent(true);
        private ConfigFileStore<(Logotype, Logotype)> _logotypesStore;
        private (Logotype, Logotype) _logotypes;

        private readonly AutoResetEvent _syncTicker = new AutoResetEvent(true);
        private ConfigFileStore<(string, TimeSpan)> _tickerStore;
        private (string, TimeSpan) _ticker;

        private readonly AutoResetEvent _syncRebootTime = new AutoResetEvent(true);
        private ConfigFileStore<AutoRebooter> _rebootTimeStore;
        private AutoRebooter _rebootTime;

        private int _downloadType = 0;
        private int _downloadProgress = 0;
        private double _downloadSpeed = 0;
        private long _lastDownloadBytes = 0;
        private DateTime _lastDownloadUpdate;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_video);
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;

            _videoView = FindViewById<VideoView>(Resource.Id.video_display);
            _leftLogo = FindViewById<ImageView>(Resource.Id.left_logo_image);
            _rightLogo = FindViewById<ImageView>(Resource.Id.right_logo_image);

            _logInfo1 = FindViewById<TextView>(Resource.Id.log_info1);
            _logInfo2 = FindViewById<TextView>(Resource.Id.log_info2);
            _logInfo3 = FindViewById<TextView>(Resource.Id.log_info3);
            _logInfo1.Visibility = _logInfo2.Visibility = _logInfo3.Visibility = ViewStates.Invisible;
            _videoView.SetMediaController(default);

            var tickerView = FindViewById<WebView>(Resource.Id.ticker_display);
            _tickerView = new TickerView(tickerView, this);
            tickerView.SetWebViewClient(_tickerView);
            _tickerView.InitWebView();

            _videoView.Completion += VideoShowed;
            _videoView.Error += VideoViewError;

            _downloadThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (_logotypesExecutor.TryExecuteCommand() == false)
                            if (_clientsExecutor.TryExecuteCommand() == false)
                                if (_readyExecutor.TryExecuteCommand() == false)
                                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                    catch (Exception ex)
                    {
                        ServerLog.Error("DownloadThread", ex.Message);
                    }
                }
            });

            _loadVideoThread = new Thread(() =>
            {
                _readyExecutor = new WebClientExecutor<VideoEventArgs>();
                _readyExecutor.CommandExecuted += ContentVideoDownloaded;
                _readyExecutor.ProgressChanged += e =>
                {
                    _downloadType = 2;
                    _downloadProgress = e.ProgressPercentage;
                    _downloadSpeed = GetDownloadSpeed(e.BytesReceived);
                };
                _clientsExecutor = new WebClientExecutor<VideoEventArgs>();
                _clientsExecutor.CommandExecuted += ClientVideoDownloaded;
                _clientsExecutor.ProgressChanged += e =>
                {
                    _downloadType = 1;
                    _downloadProgress = e.ProgressPercentage;
                    _downloadSpeed = GetDownloadSpeed(e.BytesReceived);
                };
                _logotypesExecutor = new WebClientExecutor<LogoEventArgs>();
                _logotypesExecutor.CommandExecuted += LogotypeDownloaded;
                _logotypesExecutor.ProgressChanged += e =>
                {
                    _downloadType = 0;
                    _downloadProgress = e.ProgressPercentage;
                    _downloadSpeed = GetDownloadSpeed(e.BytesReceived);
                };

                _loginStore = new ConfigFileStore<LoginDevice>(Settings.GetConfigFilePath("login.config"));
                _logotypesStore = new ConfigFileStore<(Logotype, Logotype)>(Settings.GetConfigFilePath("logotypes.config"));
                _tickerStore = new ConfigFileStore<(string, TimeSpan)>(Settings.GetConfigFilePath("ticker.config"));
                _rebootTimeStore = new ConfigFileStore<AutoRebooter>(Settings.GetConfigFilePath("reload.config"));

                _diagnosticTimer = new System.Timers.Timer();
                _diagnosticTimer.Elapsed += DiagnosticCallback;
                _diagnosticTimer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
                _diagnosticTimer.AutoReset = true;

                _advertisingTimer = new System.Timers.Timer();
                _advertisingTimer.Elapsed += AdvertisingTimerCallback;
                _advertisingTimer.Interval = TimeSpan.FromMilliseconds(1).TotalMilliseconds;
                _advertisingTimer.AutoReset = false;

                var readyVideosDeveloper = new ReadyVideosDeveloper();
                _readyVideos = readyVideosDeveloper.Create();
                var contentsDeveloper = new ContentsDeveloper();
                _contents = contentsDeveloper.Create();
                var clientsDeveloper = new AdvertisingClientsDeveloper();
                _clients = clientsDeveloper.Create();
                var intervalsDeveloper = new AdvertisingIntervalsDeveloper();
                _intervals = intervalsDeveloper.Create();
                var ordersDeveloper = new AdvertisingOrdersDeveloper();
                _orders = ordersDeveloper.Create();

                try
                {
                    _logotypes = (new Logotype(Guid.Empty, true, string.Empty),
                        new Logotype(Guid.Empty, false, string.Empty));
                    _logotypes = _logotypesStore.Load();
                    UpdateLogotypes();
                }
                catch (Exception ex)
                {
                    ServerLog.Error("Logotypes", ex.Message);
                }
                try
                {
                    _ticker = (string.Empty, TimeSpan.Zero);
                    _ticker = _tickerStore.Load();
                    _tickerView.SetTickerAsync(_ticker.Item1, _ticker.Item2);
                }
                catch (Exception ex)
                {
                    ServerLog.Error("Ticker", ex.Message);
                }
                try
                {
                    _rebootTime = new AutoRebooter();
                    _rebootTime = _rebootTimeStore.Load();
                }
                catch (Exception ex)
                {
                    ServerLog.Error("Rebooter", ex.Message);
                }
                var seconds = (5, 10).RandomNumber();
                var interval = TimeSpan.FromSeconds(seconds);
                ServerLog.Debug("VideoActivity", "Start network client");
                new Thread(() => StartNetworkClient(interval)).Start();
                _advertisingTimer.Start();
                _diagnosticTimer.Start();
                _downloadThread.Start();
                ServerLog.Debug("VideoActivity", "Start cycle video");
                LoadVideo();
            });

            _loadVideoThread.Start();
        }

        private double GetDownloadSpeed(long bytes)
        {
            if (_lastDownloadBytes == 0)
            {
                _lastDownloadUpdate = DateTime.Now;
                _lastDownloadBytes = bytes;
                return 0;
            }

            var now = DateTime.Now;
            var deltaTime = now - _lastDownloadUpdate;
            var deltaBytes = bytes - _lastDownloadBytes;
            var downloadSpeed = deltaBytes / (deltaTime.TotalMilliseconds / 1000);

            _lastDownloadBytes = bytes;
            _lastDownloadUpdate = now;

            return downloadSpeed;
        }

        private string GetAppVersionName()
        {
            try
            {
                var appContext = Application.Context;
                var packageManager = appContext.PackageManager;
                var versionName = packageManager.GetPackageInfo(
                    appContext.PackageName, 0).VersionName;
                return versionName;
            }
            catch
            {
                return "1.0";
            }
        }

        private void DiagnosticCallback(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _syncContents.WaitOne();
                var readyVisitor = new VideosCounterVisitor();
                _readyVideos.Accept(readyVisitor);

                var logs = new List<string>();
                while (ServerLog.ErrorLogs.TryTake(out var log))
                    logs.Add(log);

                var info = new DiagnosticInfo(GetAppVersionName(), readyVisitor.TotalVideos, _downloadType, _downloadProgress, _downloadSpeed);
                if (_networkClient != null)
                    _networkClient.SetDiagnosticInfo(info);
            }
            catch (Exception ex)
            {
                ServerLog.Error("DiagnosticCallback", ex.Message);
            }
            finally
            {
                _syncContents.Set();
            }
        }

        private void LogotypeDownloaded(LogoEventArgs obj)
        {
            try
            {
                _syncLogotypes.WaitOne();
                _lastDownloadBytes = 0;
                obj.IsLeft.IfTrue(() => _logotypes.Item1 = new Logotype(obj.Id, true, obj.LogoPath));
                obj.IsLeft.IfFalse(() => _logotypes.Item2 = new Logotype(obj.Id, false, obj.LogoPath));
                _logotypesStore.Save(_logotypes);
                UpdateLogotypes();
            }
            catch (Exception ex)
            {
                ServerLog.Error("LogotypeDownloaded", ex.Message);
            }
            finally
            {
                _syncLogotypes.Set();
            }
        }

        private void ClientVideoDownloaded(VideoEventArgs obj)
        {
            try
            {
                _syncClients.WaitOne();
                _lastDownloadBytes = 0;
                var visitor = new AddingVideoVisitor(obj);
                _clients.Accept(visitor);
                _clients.SaveComponents();
            }
            catch (Exception ex)
            {
                ServerLog.Error("ClientVideoDownloaded", ex.Message);
            }
            finally
            {
                _syncClients.Set();
            }
        }

        private void ContentVideoDownloaded(VideoEventArgs obj)
        {
            try
            {
                _syncContents.WaitOne();
                _lastDownloadBytes = 0;
                _readyVideos.Add(new Video(obj.Id, obj.VideoPath));
                _readyVideos.SaveComponents();
            }
            catch (Exception ex)
            {
                ServerLog.Error("ContentVideoDownloaded", ex.Message);
            }
            finally
            {
                _syncContents.Set();
            }
        }

        private void StartNetworkClient(TimeSpan interval)
        {
            var loginer = new NetworkLoginer();
            while (IsLogined(loginer) == false)
                Thread.Sleep(interval);
            _networkClient = loginer.GetClient(GetJsonParser(), interval);
            _networkClient.StartPeriodicTimerRequest(interval);
        }

        private IParser GetJsonParser()
        {
            var parser = new JsonParser();
            parser.ContentsParsed += ContentsParsed;
            parser.AdvertisingParsed += ClientsParsed;
            parser.AdvertisingIntervalsParsed += ClientIntervalsParsed;
            parser.AdvertisingOrdersParsed += OrdersParsed;
            parser.LogotypesParsed += LogotypesParsed;
            parser.TickerParsed += TickerParsed;
            parser.RebootTimeParsed += RebootTimeParsed;
            return parser;
        }

        private void RebootTimeParsed(TimeSpan obj)
        {
            try
            {
                _syncRebootTime.WaitOne();
                _rebootTime.RebootTime = obj;
                _rebootTimeStore.Save(_rebootTime);
            }
            catch (Exception ex)
            {
                ServerLog.Error("RebootTimeParsed", ex.Message);
            }
            finally
            {
                _syncRebootTime.Set();
            }
        }

        private void TickerParsed(string arg1, TimeSpan arg2)
        {
            try
            {
                _syncTicker.WaitOne();
                if (arg1 != _ticker.Item1 || arg2 != _ticker.Item2)
                {
                    _ticker = (arg1, arg2);
                    _tickerView.SetTickerAsync(arg1, arg2);
                    _tickerStore.Save(_ticker);
                }
            }
            catch (Exception ex)
            {
                ServerLog.Error("TickerParsed", ex.Message);
            }
            finally
            {
                _syncTicker.Set();
            }
        }

        private void LogotypesParsed(Core.Network.WebClientCommands.Filters.BaseFilter obj)
        {
            try
            {
                _syncLogotypes.WaitOne();
                _logotypesExecutor.Filter(obj);
                _logotypes.Item1.IsValid(obj).IfFalse(() =>
                {
                    _logotypes.Item1 = new Logotype(Guid.Empty, true, string.Empty);
                    UpdateLogotypes();
                });
                _logotypes.Item2.IsValid(obj).IfFalse(() =>
                {
                    _logotypes.Item2 = new Logotype(Guid.Empty, false, string.Empty);
                    UpdateLogotypes();
                });
                _logotypesStore.Save(_logotypes);
                foreach (var command in obj.GetDownloadCommands())
                    _logotypesExecutor.AddCommandToQueue(command);
            }
            catch (Exception ex)
            {
                ServerLog.Error("LogotypesParsed", ex.Message);
            }
            finally
            {
                _syncLogotypes.Set();
            }
        }

        private void OrdersParsed(Core.Network.WebClientCommands.Filters.BaseFilter obj)
        {
            try
            {
                _syncOrders.WaitOne();
                _orders.IsValid(obj);
                foreach (var visitor in obj.GetVisitors())
                    _orders.Accept(visitor);
                _orders.SaveComponents();
            }
            catch (Exception ex)
            {
                ServerLog.Error("OrdersParsed", ex.Message);
            }
            finally
            {
                _syncOrders.Set();
            }
        }

        private void ClientIntervalsParsed(Core.Network.WebClientCommands.Filters.BaseFilter obj)
        {
            try
            {
                _syncIntervals.WaitOne();
                _intervals.IsValid(obj);
                foreach (var visitor in obj.GetVisitors())
                    _intervals.Accept(visitor);
                _intervals.SaveComponents();
            }
            catch (Exception ex)
            {
                ServerLog.Error("ClientIntervalsParsed", ex.Message);
            }
            finally
            {
                _syncIntervals.Set();
            }
        }

        private void ClientsParsed(Core.Network.WebClientCommands.Filters.BaseFilter obj)
        {
            try
            {
                _syncClients.WaitOne();
                _clientsExecutor.Filter(obj);
                _clients.IsValid(obj);
                _clients.SaveComponents();
                foreach (var command in obj.GetDownloadCommands())
                    _clientsExecutor.AddCommandToQueue(command);
            }
            catch (Exception ex)
            {
                ServerLog.Error("ClientsParsed", ex.Message);
            }
            finally
            {
                _syncClients.Set();
            }
        }

        private void ContentsParsed(Core.Network.WebClientCommands.Filters.BaseFilter obj)
        {
            try
            {
                _syncContents.WaitOne();
                _contents.IsValid(obj);
                foreach (var visitor in obj.GetVisitors())
                    _contents.Accept(visitor);
                _contents.SaveComponents();

                _readyExecutor.Filter(obj);

                _readyVideos.IsValid(obj);
                _readyVideos.SaveComponents();
            }
            catch (Exception ex)
            {
                ServerLog.Error("ContentsParsed", ex.Message);
            }
            finally
            {
                _syncContents.Set();
            }
        }

        private bool IsLogined(ILoginer loginer)
        {
            try
            {
                var loginData = _loginStore.Load();
                Settings.DeviceId = loginData.UUID;
                var status = loginer.TryLogin(loginData);
                return status == Core.Network.Enums.LoginStatus.SuccessLogin;
            }
            catch (Exception ex)
            {
                ServerLog.Error("VideoActivity", ex.Message);
                return false;
            }
        }

        private void UpdateLogotypes()
        {
            RunOnUiThread(() =>
            {
                if (_logotypes.Item1.Id != Guid.Empty)
                    _leftLogo.SetImageURI(Android.Net.Uri.FromFile(new File(_logotypes.Item1.LogotypePath)));
                else _leftLogo.SetImageResource(Resource.Color.mtrl_btn_transparent_bg_color);
                if (_logotypes.Item2.Id != Guid.Empty)
                    _rightLogo.SetImageURI(Android.Net.Uri.FromFile(new File(_logotypes.Item2.LogotypePath)));
                else _rightLogo.SetImageResource(Resource.Color.mtrl_btn_transparent_bg_color);
            });
        }

        private void AdvertisingTimerCallback(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _timerCounter++;
                do
                {
                    bool hasAdsVideos = false;
                    try
                    {
                        ServerLog.Debug("VideoActivity", "Client cycle!");
                        _syncIntervals.WaitOne(); _syncOrders.WaitOne(); _syncClients.WaitOne();
                        var visitor = new ClientHandlerVisitor(_timerCounter, HasContentVideos());
                        _intervals.Accept(visitor);
                        _orders.Accept(visitor);
                        _clients.Accept(visitor);
                        var videos = visitor.GetSortedVideos();
                        foreach (var video in videos)
                            _advertisingVideos.Add(video);
                        hasAdsVideos = Convert.ToBoolean(videos.ToArray().Length);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        _syncIntervals.Set(); _syncOrders.Set(); _syncClients.Set();
                    }
                    
                    if (hasAdsVideos)
                    {
                        InterruptContentVideo();
                        _lastAdsShowed.WaitOne();
                    }
                    else Thread.Sleep(TimeSpan.FromSeconds(1));
                } while (HasContentVideos() == false);
            }
            catch (Exception ex)
            {
                ServerLog.Error("AdvertisingTimerCallback", ex.Message);
            }
            finally
            {
                _advertisingTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
                _advertisingTimer.Start();
            }
        }

        private void InterruptContentVideo()
        {
            RunOnUiThread(() =>
            {
                lock (_syncCurrentReservedVideos)
                {
                    if (_currentVideo.Item2 != null && _currentVideo.Item1)
                    {
                        _reservedVideo = (_currentVideo.Item1, _currentVideo.Item2, _videoView.CurrentPosition);
                        _videoView.StopPlayback();
                        _currentVideo = (false, null);
                    }
                    _videoShowed.Set();
                }
            });
        }

        private bool HasContentVideos()
        {
            try
            {
                _syncContents.WaitOne();
                var hasContentVideos = Convert.ToBoolean(_contents.Components.Count);
                return hasContentVideos;
            }
            catch (Exception ex)
            {
                ServerLog.Error("HasContentVideos", ex.Message);
            }
            finally
            {
                _syncContents.Set();
            }
            return false;
        }

        private bool TryGetReadyVideo(out Video video)
        {
            try
            {
                _syncContents.WaitOne();
                var visitor = new VideosCounterVisitor();
                _contents.Accept(visitor);

                var readyVisitor = new VideosCounterVisitor();
                _readyExecutor.Accept(readyVisitor);
                _readyVideos.Accept(readyVisitor);

                ServerLog.Debug("TryGetReadyVideo", $"TotalVideos {visitor.TotalVideos} downloaded videos {_readyVideos.Components.Count}");

                while (readyVisitor.TotalVideos < Settings.MaxReadyVideos
                    && visitor.TotalVideos > readyVisitor.TotalVideos
                    && _contents.TryGetNext(out var category))
                {
                    var isSuccess = category.TryGetRandom(out var firstVideo);
                    if (isSuccess)
                    {
                        var secondVideo = firstVideo;
                        do
                        {
                            var finderVisitor = new VideoFinder(secondVideo.Id);
                            _readyExecutor.Accept(finderVisitor);
                            _readyVideos.Accept(finderVisitor);
                            if (finderVisitor.IsFound == false)
                            {
                                _readyExecutor.AddCommandToQueue(new VideoDownloadCommand(Settings.GetVideoDownloadUri(secondVideo.Id),
                                    Settings.GetVideoFilesPath(secondVideo.Id), category.Id, new VideoJson(secondVideo.Id)));
                                break;
                            }
                        } while (category.TryGetNext(out secondVideo) && firstVideo.Id != secondVideo.Id);
                    }

                    readyVisitor = new VideosCounterVisitor();
                    _readyExecutor.Accept(readyVisitor);
                    _readyVideos.Accept(readyVisitor);
                }
                return _readyVideos.TryGetNext(out video);
            }
            catch (Exception ex)
            {
                ServerLog.Error("TryGetReadyVideo", ex.Message);
            }
            finally
            {
                _syncContents.Set();
            }
            video = null;
            return false;
        }

        private void LoadVideo()
        {
            while (_advertisingVideos.IsAddingCompleted == false)
            {
                try
                {
                    ServerLog.Debug("LoadVideo", "Start");
                    _videoShowed.WaitOne();
                    Video video;
                    int duration = 0;
                    bool isContentVideo = false;
                    while (_advertisingVideos.TryTake(out video) == false
                        && (isContentVideo = TryGetReservedVideo(out video, out duration)) == false
                        && (isContentVideo = TryGetReadyVideo(out video)) == false)
                    {
                        ServerLog.Debug("LoadVideo", "No videos");
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                    lock (_syncCurrentReservedVideos)
                    {
                        _currentVideo = (isContentVideo, video);
                        StartVideo(video.VideoPath, duration);
                    }
                }
                catch (Exception ex)
                {
                    ServerLog.Error("LoadVideo", ex.Message);
                }
            }
        }

        private bool TryGetReservedVideo(out Video video, out int duration)
        {
            video = null;
            duration = 0;
            lock (_syncCurrentReservedVideos)
            {
                if (_reservedVideo.Item2 != null)
                {
                    video = _reservedVideo.Item2;
                    duration = _reservedVideo.Item3;
                    _reservedVideo = (false, null, 0);
                    return true;
                }
            }
            return false;
        }

        private void VideoViewError(object sender, global::Android.Media.MediaPlayer.ErrorEventArgs e)
        {
            ServerLog.Error("VideoViewError", e.What.ToString());
            VideoHandler();
        }

        private void VideoShowed(object sender, EventArgs e)
        {
            VideoHandler();
        }

        private void VideoHandler()
        {
            lock (_syncCurrentReservedVideos)
            {
                try
                {
                    if (_currentVideo.Item2 != null)
                    {
                        if (_currentVideo.Item1)
                        {
                            try
                            {
                                _syncContents.WaitOne();
                                var readyVisitor = new VideosCounterVisitor();
                                _readyVideos.Accept(readyVisitor);
                                while (readyVisitor.TotalVideos >= Settings.MaxReadyVideos)
                                {
                                    var video = _readyVideos.Components.First();
                                    video.DeleteVideoFile();
                                    _readyVideos.Remove(video);
                                    _readyVideos.SaveComponents();

                                    readyVisitor = new VideosCounterVisitor();
                                    _readyVideos.Accept(readyVisitor);
                                }
                            }
                            catch
                            {
                                throw;
                            }
                            finally
                            {
                                _syncContents.Set();
                            }
                        }
                        else
                        {
                            if (_advertisingVideos.Count == 0)
                                _lastAdsShowed.Set();
                        }
                        _currentVideo = (false, null);
                    }
                }
                catch (Exception ex)
                {
                    ServerLog.Error("VideoHandler", ex.Message);
                }
                finally
                {
                    _videoShowed.Set();
                }
            }
        }

        private void StartVideo(string videoPath, int duration)
        {
            ServerLog.Debug("StartVideo", "Start");
            RunOnUiThread(() =>
            {
                try
                {
                    _videoView.Suspend();
                    _videoView.SetVideoPath(videoPath);
                    _videoView.SeekTo(duration);
                    _videoView.Start();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(BaseContext, ex.Message, ToastLength.Long).Show();
                    _videoShowed.Set();
                }
            });
        }
    }
}