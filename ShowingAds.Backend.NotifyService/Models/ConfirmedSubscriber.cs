using ShowingAds.Backend.NotifyService.Visitors;
using ShowingAds.Shared.Backend.Models.NotifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.Backend.NotifyService.Models
{
    public class ConfirmedSubscriber : Subscriber, IDisposable
    {
        private readonly TimeSpan NOTIFY_INTERVAL = TimeSpan.FromSeconds(3);

        public event Action<string> NotifySubscriber;

        public Guid SubscriberId { get; private set; }

        private readonly object _syncTimer = new();
        public Timer _notifyTimer;
        private List<Notification> _notifications;

        public ConfirmedSubscriber(Guid subscriberId, NotConfirmedSubscriber subscriber) : base(subscriber.ConnectionId)
        {
            SubscriberId = subscriberId;
            _notifyTimer = new Timer();
            _notifyTimer.Interval = NOTIFY_INTERVAL.TotalMilliseconds;
            _notifyTimer.AutoReset = true;
            _notifyTimer.Elapsed += (s, e) => NotifySubscriber?.Invoke(ConnectionId);
            _notifications = new List<Notification>();
        }

        public void AddNotification(Notification packet)
        {
            _notifications.Add(packet);
            lock (_syncTimer)
            {
                if (_notifyTimer.Enabled == false)
                {
                    NotifySubscriber?.Invoke(ConnectionId);
                    _notifyTimer.Start();
                }
            }
        }

        public IEnumerable<Notification> GetNotifications()
        {
            lock (_syncTimer)
                _notifyTimer.Stop();
            var notifications = new List<Notification>();
            _notifications.ForEach(x => notifications.Add(x));
            _notifications.Clear();
            return notifications;
        }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);

        public void Dispose()
        {
            lock (_syncTimer)
            {
                _notifyTimer.Stop();
                _notifyTimer.Close();
            }
        }
    }
}
