using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp
{
    public class TickerView : WebViewClient
    {
        private ManualResetEvent _syncLoading = new ManualResetEvent(false);
        private readonly Activity _activity;
        private readonly WebView _webView;

        public TickerView(WebView webView, Activity activity)
        {
            _webView = webView ?? throw new ArgumentNullException(nameof(webView));
            _activity = activity ?? throw new ArgumentNullException(nameof(activity));
        }

        public void InitWebView()
        {
            _webView.Settings.JavaScriptEnabled = true;
            _webView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            _webView.LoadUrl("file:///android_asset/ticker.html");
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            _syncLoading.Set();
        }

        public async Task SetTickerAsync(string ticker, TimeSpan delay)
        {
            if (ticker == null)
                throw new ArgumentNullException(nameof(ticker));
            await Task.Run(() =>
            {
                _syncLoading.WaitOne();
                _activity.RunOnUiThread(() =>
                       _webView.EvaluateJavascript($"javascript:initTicker('{ticker}', {delay.TotalMilliseconds});", null));
            });
        }

        ~TickerView()
        {
            _syncLoading.Close();
        }
    }
}