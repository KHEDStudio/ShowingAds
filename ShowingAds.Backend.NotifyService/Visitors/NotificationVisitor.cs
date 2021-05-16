using ShowingAds.Backend.NotifyService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Visitors
{
    public class NotificationVisitor : IVisitor
    {
        public Guid SubscriberId { get; private set; }
        public string ConnectionId { get; private set; }
        public IEnumerable<Notification> Notifications { get; private set; }

        public NotificationVisitor(Guid subscriberId, string connectionId)
        {
            SubscriberId = subscriberId;
            ConnectionId = connectionId ?? throw new ArgumentNullException(nameof(connectionId));
            Notifications = new List<Notification>();
        }

        public void Visit(ConfirmedSubscriber subscriber)
        {
            if (ConnectionId == subscriber.ConnectionId && subscriber.SubscriberId == SubscriberId)
                Notifications = subscriber.GetNotifications();
        }

        public void Visit(NotConfirmedSubscriber subscriber)
        {
            // empty...
        }
    }
}
