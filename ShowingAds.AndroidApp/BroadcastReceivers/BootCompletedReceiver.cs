using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShowingAds.AndroidApp.BroadcastReceivers
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootCompletedReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                var activityIntent = new Intent(context, typeof(MainActivity));
                activityIntent.AddFlags(ActivityFlags.NewTask);
                activityIntent.AddFlags(ActivityFlags.ClearTop);
                context.StartActivity(activityIntent);
            }
            catch (Exception e)
            {
                Toast.MakeText(context, e.Message, ToastLength.Long).Show();
            }
        }
    }
}