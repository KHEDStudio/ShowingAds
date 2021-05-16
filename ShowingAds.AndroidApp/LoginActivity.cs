using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ShowingAds.AndroidApp.Core.DataAccess;
using ShowingAds.AndroidApp.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowingAds.AndroidApp.Core;
using System.Threading;
using ShowingAds.Shared.Core.Models.Login;

namespace ShowingAds.AndroidApp
{
    [Activity(Label = "@string/app_name", ScreenOrientation = ScreenOrientation.Landscape)]
    public class LoginActivity : AppCompatActivity
    {
        private EditText _deviceName;
        private EditText _login;
        private EditText _password;
        private Button _loginButton;

        private NetworkLoginer _loginer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_login);
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;

            _loginer = new NetworkLoginer();
            _deviceName = FindViewById<EditText>(Resource.Id.device_name);
            _login = FindViewById<EditText>(Resource.Id.login);
            _password = FindViewById<EditText>(Resource.Id.password);
            _loginButton = FindViewById<Button>(Resource.Id.btn_login);

            _loginButton.Click += (s, e) => new Thread(() => TryLoginSession()).Start();
        }

        private void TryLoginSession()
        {
            SetEnabledElements(false);
#if DEBUG
            var loginData = new LoginDevice(_deviceName.Text, Guid.Parse("11111111-1111-1111-1111-111111111111"), _login.Text, _password.Text);
#else
            var loginData = new LoginDevice(_deviceName.Text, Guid.NewGuid(), _login.Text, _password.Text);
#endif
            var status = _loginer.TryLogin(loginData);
            switch (status)
            {
                case Core.Network.Enums.LoginStatus.SuccessLogin:
                    var loginStore = new ConfigFileStore<LoginDevice>(Settings.GetConfigFilePath("login.config"));
                    loginStore.Save(loginData);
                    loginStore.Dispose();
                    StartActivity(typeof(VideoActivity));
                    break;
                case Core.Network.Enums.LoginStatus.NotCorrectLoginData:
                    Toast.MakeText(BaseContext, Resource.String.no_correct_login_data, ToastLength.Long).Show();
                    break;
                case Core.Network.Enums.LoginStatus.RequestError:
                    Toast.MakeText(BaseContext, Resource.String.no_connection, ToastLength.Long).Show();
                    break;
                default:
                    Toast.MakeText(BaseContext, Resource.String.something_wrong, ToastLength.Long).Show();
                    break;
            }
            SetEnabledElements(true);
        }

        private void SetEnabledElements(bool isEnabled)
        {
            RunOnUiThread(() =>
            {
                _deviceName.Enabled = isEnabled;
                _login.Enabled = isEnabled;
                _password.Enabled = isEnabled;
                _loginButton.Enabled = isEnabled;
            });
        }
    }
}