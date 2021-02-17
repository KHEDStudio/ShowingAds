using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ShowingAds.AndroidApp
{
    public class AutoRebooter
    {
        [JsonIgnore]
        private readonly object _syncRoot = new object();

        [JsonIgnore]
        private Timer _rebootTimer = new Timer();

        [JsonIgnore]
        private TimeSpan _rebootTime;

        [JsonProperty("reboot_time")]
        public TimeSpan RebootTime
        {
            get
            {
                lock (_syncRoot)
                    return _rebootTime;
            }
            set
            {
                lock (_syncRoot)
                {
                    _rebootTime = value;
                    InitializeTimer();
                }
            }
        }

        [JsonConstructor]
        public AutoRebooter()
        {
            lock (_syncRoot)
                InitializeTimer();
        }

        private void InitializeTimer()
        {
            _rebootTimer.Stop();
            _rebootTimer = new Timer();
            _rebootTimer.AutoReset = false;
            _rebootTimer.Elapsed += RebootDeviceCallback;
            _rebootTimer.Interval = CalculateInterval();
            _rebootTimer.Start();
        }

        private double CalculateInterval()
        {
            var nowDate = DateTime.Now;
            var nowTime = nowDate.TimeOfDay;
            var a = RebootTime;
            if (TimeSpan.Compare(nowTime, RebootTime) > 0)
            {
                var interval = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day,
                    RebootTime.Hours, RebootTime.Minutes, RebootTime.Seconds, RebootTime.Milliseconds);
                return interval.AddDays(1).Subtract(nowDate).TotalMilliseconds;
            }
            else return RebootTime.Subtract(nowTime).TotalMilliseconds;
        }

        private void RebootDeviceCallback(object sender, ElapsedEventArgs e) => RebootDevice();

        public void RebootDevice()
        {
            try
            {
                Java.Lang.Runtime.GetRuntime().Exec(new[] { "su", "-c", "reboot" });
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, $"Error reboot: {ex.Message}", ToastLength.Long).Show();
                lock (_syncRoot)
                    InitializeTimer();
            }
        }
    }
}