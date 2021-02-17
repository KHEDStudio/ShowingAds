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
using ShowingAds.AndroidApp.Core.Network.Models;
using ShowingAds.AndroidApp.Core.Network.Parsers;
using ShowingAds.AndroidApp.Core.Network.Parsers.Interfaces;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs;
using ShowingAds.CoreLibrary.Models.Login;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class VideoActivity : AppCompatActivity
    {
        private VideoView _videoView;
        private TickerView _tickerView;
        private ImageView _leftLogo;
        private ImageView _rightLogo;

        private Timer _diagnosticTimer;

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

            var tickerView = FindViewById<WebView>(Resource.Id.ticker_display);
            _tickerView = new TickerView(tickerView, this);
            tickerView.SetWebViewClient(_tickerView);
            _tickerView.InitWebView();

            _videoView.Completion += VideoShowed;
            _videoView.Error += VideoViewError;

            _readyExecutor = new WebClientExecutor<VideoEventArgs>();
            _readyExecutor.CommandExecuted += async (e) => await ContentVideoDownloaded(e);
            _clientsExecutor = new WebClientExecutor<VideoEventArgs>();
            _clientsExecutor.CommandExecuted += async (e) => await ClientVideoDownloaded(e);
            _logotypesExecutor = new WebClientExecutor<LogoEventArgs>();
            _logotypesExecutor.CommandExecuted += async (e) => await LogotypeDownloaded(e);

            _loginStore = new ConfigFileStore<LoginDevice>(Settings.GetConfigFilePath("login.config"));
            _logotypesStore = new ConfigFileStore<(Logotype, Logotype)>(Settings.GetConfigFilePath("logotypes.config"));
            _tickerStore = new ConfigFileStore<(string, TimeSpan)>(Settings.GetConfigFilePath("ticker.config"));
            _rebootTimeStore = new ConfigFileStore<AutoRebooter>(Settings.GetConfigFilePath("reload.config"));

            _advertisingTimer = new System.Timers.Timer();
            _advertisingTimer.Elapsed += async (s, e) => await AdvertisingTimerCallback(s, e);
            _advertisingTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
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

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    _logotypes = (new Logotype(Guid.Empty, true, string.Empty),
                        new Logotype(Guid.Empty, false, string.Empty));
                    _logotypes = await _logotypesStore.Load();
                    UpdateLogotypes();
                }
                catch (Exception ex)
                {
                    await ServerLog.Error("Logotypes", ex.Message);
                }
                try
                {
                    _ticker = (string.Empty, TimeSpan.Zero);
                    _ticker = await _tickerStore.Load();
                    await _tickerView.SetTickerAsync(_ticker.Item1, _ticker.Item2);
                }
                catch (Exception ex)
                {
                    await ServerLog.Error("Ticker", ex.Message);
                }
                try
                {
                    _rebootTime = new AutoRebooter();
                    _rebootTime = await _rebootTimeStore.Load();
                }
                catch (Exception ex)
                {
                    await ServerLog.Error("Rebooter", ex.Message);
                }
                var seconds = (5, 10).RandomNumber();
                var interval = TimeSpan.FromSeconds(seconds);
                await StartNetworkClient(interval);
                _advertisingTimer.Start();
                _diagnosticTimer = new Timer(DiagnosticCallback, default, TimeSpan.Zero, TimeSpan.FromSeconds(5));
                await LoadVideo();
            });
        }

        private void DiagnosticCallback(object state)
        {
            _syncContents.WaitOne();
            var readyVisitor = new VideosCounterVisitor();
            _readyVideos.Accept(readyVisitor);
            _syncContents.Set();

            var info = new DiagnosticInfo(readyVisitor.TotalVideos);
            _networkClient.SetDiagnosticInfo(info);
        }

        private async Task LogotypeDownloaded(LogoEventArgs obj)
        {
            _syncLogotypes.WaitOne();
            obj.IsLeft.IfTrue(() => _logotypes.Item1 = new Logotype(obj.Id, true, obj.LogoPath));
            obj.IsLeft.IfFalse(() => _logotypes.Item2 = new Logotype(obj.Id, false, obj.LogoPath));
            await _logotypesStore.Save(_logotypes);
            UpdateLogotypes();
            _syncLogotypes.Set();
        }

        private async Task ClientVideoDownloaded(VideoEventArgs obj)
        {
            _syncClients.WaitOne();
            var visitor = new AddingVideoVisitor(obj);
            _clients.Accept(visitor);
            await _clients.SaveComponents();
            _syncClients.Set();
        }

        private async Task ContentVideoDownloaded(VideoEventArgs obj)
        {
            _syncContents.WaitOne();
            _readyVideos.Add(new Video(obj.Id, obj.VideoPath));
            await _readyVideos.SaveComponents();
            _syncContents.Set();
        }

        private async Task StartNetworkClient(TimeSpan interval)
        {
            var loginer = new NetworkLoginer();
            while (await IsLogined(loginer) == false)
                await Task.Delay(interval);
            _networkClient = loginer.GetClient(GetJsonParser(), interval);
            _networkClient.StartPeriodicTimerRequest(interval);
        }

        private IParser GetJsonParser()
        {
            var parser = new JsonParser();
            parser.ContentsParsed += async e => await ContentsParsed(e);
            parser.AdvertisingParsed += async e => await ClientsParsed(e);
            parser.AdvertisingIntervalsParsed += async e => await ClientIntervalsParsed(e);
            parser.AdvertisingOrdersParsed += async e => await OrdersParsed(e);
            parser.LogotypesParsed += async e => await LogotypesParsed(e);
            parser.TickerParsed += async (e, a) => await TickerParsed(e, a);
            parser.RebootTimeParsed += async e => await RebootTimeParsed(e);
            return parser;
        }

        private async Task RebootTimeParsed(TimeSpan obj)
        {
            _syncRebootTime.WaitOne();
            _rebootTime.RebootTime = obj;
            await _rebootTimeStore.Save(_rebootTime);
            _syncRebootTime.Set();
        }

        private async Task TickerParsed(string arg1, TimeSpan arg2)
        {
            _syncTicker.WaitOne();
            if (arg1 != _ticker.Item1 || arg2 != _ticker.Item2)
            {
                _ticker = (arg1, arg2);
                await _tickerView.SetTickerAsync(arg1, arg2);
                await _tickerStore.Save(_ticker);
            }
            _syncTicker.Set();
        }

        private async Task LogotypesParsed(Core.Network.WebClientCommands.Filters.BaseFilter obj)
        {
            _logotypesExecutor.Filter(obj);
            _syncLogotypes.WaitOne();
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
            await _logotypesStore.Save(_logotypes);
            _syncLogotypes.Set();
            foreach (var command in obj.GetDownloadCommands())
                _logotypesExecutor.AddCommandToQueue(command);
        }

        private async Task OrdersParsed(Core.Network.WebClientCommands.Filters.BaseFilter obj)
        {
            _syncOrders.WaitOne();
            _orders.IsValid(obj);
            foreach (var visitor in obj.GetVisitors())
                _orders.Accept(visitor);
            await _orders.SaveComponents();
            _syncOrders.Set();
        }

        private async Task ClientIntervalsParsed(Core.Network.WebClientCommands.Filters.BaseFilter obj)
        {
            _syncIntervals.WaitOne();
            _intervals.IsValid(obj);
            foreach (var visitor in obj.GetVisitors())
                _intervals.Accept(visitor);
            await _intervals.SaveComponents();
            _syncIntervals.Set();
        }

        private async Task ClientsParsed(Core.Network.WebClientCommands.Filters.BaseFilter obj)
        {
            _clientsExecutor.Filter(obj);
            _syncClients.WaitOne();
            _clients.IsValid(obj);
            await _clients.SaveComponents();
            _syncClients.Set();
            foreach (var command in obj.GetDownloadCommands())
                _clientsExecutor.AddCommandToQueue(command);
        }

        private async Task ContentsParsed(Core.Network.WebClientCommands.Filters.BaseFilter obj)
        {
            _syncContents.WaitOne();
            _contents.IsValid(obj);
            foreach (var visitor in obj.GetVisitors())
                _contents.Accept(visitor);
            await _contents.SaveComponents();

            _readyExecutor.Filter(obj);

            _readyVideos.IsValid(obj);
            await _readyVideos.SaveComponents();
            _syncContents.Set();
        }

        private async Task<bool> IsLogined(ILoginer loginer)
        {
            try
            {
                var loginData = await _loginStore.Load();
                Settings.DeviceId = loginData.UUID;
                var status = await loginer.TryLoginAsync(loginData);
                return status == Core.Network.Enums.LoginStatus.SuccessLogin;
            }
            catch (Exception ex)
            {
                await ServerLog.Error("VideoActivity", ex.Message);
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

        private async Task AdvertisingTimerCallback(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timerCounter++;
            do
            {
                _syncIntervals.WaitOne(); _syncOrders.WaitOne(); _syncClients.WaitOne();
                var visitor = new ClientHandlerVisitor(_timerCounter, HasContentVideos());
                _intervals.Accept(visitor);
                _orders.Accept(visitor);
                _clients.Accept(visitor);
                var videos = visitor.GetSortedVideos();
                foreach (var video in videos)
                    _advertisingVideos.Add(video);
                var hasAdsVideos = Convert.ToBoolean(videos.ToArray().Length);
                _syncIntervals.Set(); _syncOrders.Set(); _syncClients.Set();
                if (hasAdsVideos)
                {
                    InterruptContentVideo();
                    _lastAdsShowed.WaitOne();
                } else await Task.Delay(TimeSpan.FromSeconds(1));
            } while (HasContentVideos() == false);
            _advertisingTimer.Start();
        }

        private void InterruptContentVideo()
        {
            lock (_syncCurrentReservedVideos)
            {
                if (_currentVideo.Item2 != null && _currentVideo.Item1)
                {
                    RunOnUiThread(_videoView.StopPlayback);
                    _reservedVideo = (_currentVideo.Item1, _currentVideo.Item2, _videoView.CurrentPosition);
                    _currentVideo = (false, null);
                    _videoShowed.Set();
                }
            }
        }

        private bool HasContentVideos()
        {
            _syncContents.WaitOne();
            var hasContentVideos = Convert.ToBoolean(_contents.Components.Count);
            _syncContents.Set();
            return hasContentVideos;
        }

        private bool TryGetReadyVideo(out Video video)
        {
            _syncContents.WaitOne();
            var visitor = new VideosCounterVisitor();
            _contents.Accept(visitor);

            var readyVisitor = new VideosCounterVisitor();
            _readyExecutor.Accept(readyVisitor);
            _readyVideos.Accept(readyVisitor);

            while (readyVisitor.TotalVideos < Settings.MaxReadyVideos + 5
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
                                Settings.GetVideoFilesPath(secondVideo.Id), category.Id, new CoreLibrary.Models.Json.VideoJson(secondVideo.Id)));
                            break;
                        }
                    } while (category.TryGetNext(out secondVideo) && firstVideo.Id != secondVideo.Id);
                }

                readyVisitor = new VideosCounterVisitor();
                _readyExecutor.Accept(readyVisitor);
                _readyVideos.Accept(readyVisitor);
            }
            _syncContents.Set();
            return _readyVideos.TryGetNext(out video);
        }

        private async Task LoadVideo()
        {
            while (_advertisingVideos.IsAddingCompleted == false)
            {
                _videoShowed.WaitOne();
                Video video;
                int duration = 0;
                bool isContentVideo = false;
                while (_advertisingVideos.TryTake(out video) == false
                    && (isContentVideo = TryGetReservedVideo(out video, out duration)) == false
                    && (isContentVideo = TryGetReadyVideo(out video)) == false)
                    await Task.Delay(TimeSpan.FromSeconds(1));
                lock (_syncCurrentReservedVideos)
                {
                    _currentVideo = (isContentVideo, video);
                    StartVideo(video.VideoPath, duration);
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

        private async void VideoViewError(object sender, global::Android.Media.MediaPlayer.ErrorEventArgs e)
        {
            await ServerLog.Error("VideoViewError", e.What.ToString());
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
                if (_currentVideo.Item2 != null)
                {
                    if (_currentVideo.Item1)
                    {
                        _syncContents.WaitOne();
                        var readyVisitor = new VideosCounterVisitor();
                        _readyVideos.Accept(readyVisitor);
                        if (readyVisitor.TotalVideos >= Settings.MaxReadyVideos)
                            _readyVideos.Remove(_readyVideos.Components.First());
                        _syncContents.Set();
                    }
                    else
                    {
                        if (_advertisingVideos.Count == 0)
                            _lastAdsShowed.Set();
                    }
                    _currentVideo = (false, null);
                    _videoShowed.Set();
                }
            }
        }

        private void StartVideo(string videoPath, int duration)
        {
            RunOnUiThread(async () =>
            {
                while (true)
                {
                    try
                    {
                        _videoView.SetVideoPath(videoPath);
                        _videoView.SeekTo(duration);
                        _videoView.Start();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(BaseContext, ex.Message, ToastLength.Long).Show();
                        await Task.Delay(TimeSpan.FromSeconds(3));
                    }
                }
            });
        }
    }
}