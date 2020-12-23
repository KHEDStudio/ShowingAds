using ShowingAds.CoreLibrary.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.CoreLibrary
{
    public class EventBus : Singleton<EventBus>
    {
        public event Action StartManagersUpdate;
        public event Action ManagersUpdated;
        private Timer _timer { get; set; }

        private EventBus()
        {
            _timer = new Timer();
            _timer.Interval = TimeSpan.FromHours(1).TotalMilliseconds;
            _timer.Elapsed += (s, e) => Task.Run(UpdateManagers);
            _timer.AutoReset = true;
            _timer.Start();
        }

        public void UpdateManagers()
        {
            StartManagersUpdate?.Invoke();
            ManagersUpdated?.Invoke();
        }
    }
}
