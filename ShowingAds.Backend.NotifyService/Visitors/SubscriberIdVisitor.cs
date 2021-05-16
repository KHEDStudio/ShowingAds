using ShowingAds.Backend.NotifyService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Visitors
{
    public class SubscriberIdVisitor : IVisitor
    {
        public string ConnectionId { get; private set; }
        public Guid SubscriberId { get; private set; }

        public ConfirmedSubscriber SubscriberToAdd { get; private set; }
        public NotConfirmedSubscriber SubscriberToRemove { get; private set; }

        public SubscriberIdVisitor(string connectionId, Guid subscriberId)
        {
            ConnectionId = connectionId ?? throw new ArgumentNullException(nameof(connectionId));
            SubscriberId = subscriberId;
        }

        public void Visit(ConfirmedSubscriber subscriber)
        {
            // empty...
        }

        public void Visit(NotConfirmedSubscriber subscriber)
        {
            if (subscriber.ConnectionId == ConnectionId)
            {
                SubscriberToAdd = new ConfirmedSubscriber(SubscriberId, subscriber);
                SubscriberToRemove = subscriber;
            }
        }
    }
}
