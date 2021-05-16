using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content.PM;
using Xamarin.Essentials;
using System.Collections.Generic;
using Java.Lang;
using ShowingAds.AndroidApp.BroadcastReceivers;
using Android.Content;
using Android.Views;
using System.Threading.Tasks;
using ShowingAds.AndroidApp.Core.DataAccess;
using ShowingAds.AndroidApp.Core.Network;
using ShowingAds.AndroidApp.Core.Network.Interfaces;
using ShowingAds.AndroidApp.Core;
using System;
using ShowingAds.Shared.Core.Models.Login;

namespace ShowingAds.AndroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : AppCompatActivity
    {
        public static readonly int REQUEST_CODE = 63380;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;

            var listPermissions = new List<string>()
            {
                "android.permission.ACCESS_NETWORK_STATE",
                "android.permission.REBOOT",
                "android.permission.READ_EXTERNAL_STORAGE",
                "android.permission.WRITE_EXTERNAL_STORAGE",
                "android.permission.INTERNET",
                "android.permission.ACCESS_WIFI_STATE",
                "android.permission.DEVICE_POWER",
                "android.permission.DIAGNOSTIC",
                "android.permission.HARDWARE_TEST",
                "android.permission.INJECT_EVENTS",
                "android.permission.MEDIA_CONTENT_CONTROL",
                "android.permission.MODIFY_AUDIO_SETTINGS",
                "android.permission.MODIFY_PHONE_STATE",
                "android.permission.READ_FRAME_BUFFER",
                "android.permission.READ_INPUT_STATE",
                "android.permission.READ_LOGS",
                "android.permission.READ_PHONE_STATE",
                "android.permission.RECEIVE_BOOT_COMPLETED",
                "android.permission.SET_ORIENTATION",
                "android.permission.UPDATE_DEVICE_STATS",
                "android.permission.WRITE_SECURE_SETTINGS",
                "android.permission.WRITE_SETTINGS",
                "android.permission.BIND_DEVICE_ADMIN",
            };

            RequestPermissions(listPermissions.ToArray(), REQUEST_CODE);
            RegisterReceiver(new BootCompletedReceiver(), new IntentFilter(Intent.ActionBootCompleted));
            new Thread(() =>
            {
                //await Task.Delay(TimeSpan.FromSeconds(10));
                LoadApp();
            }).Start();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void LoadApp()
        {
            var loginStore = new ConfigFileStore<LoginDevice>(Settings.GetConfigFilePath("login.config"));
            try
            {
                loginStore.Load();
                StartActivity(typeof(VideoActivity));
            }
            catch
            {
                StartActivity(typeof(LoginActivity));
            }
            finally
            {
                loginStore.Dispose();
            }
        }
    }
}