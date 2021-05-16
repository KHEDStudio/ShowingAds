using ShowingAds.Backend.NotifyService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Visitors
{
    public class RemoveSubscriberVisitor : IVisitor
    {
        public Guid SubscriberId { get; private set; }
        public string ConnectionId { get; private set; }

        private List<Subscriber> _subscribersToRemove;

        public RemoveSubscriberVisitor(Guid subscriberId, string connectionId)
        {
            SubscriberId = subscriberId;
            ConnectionId = connectionId ?? string.Empty;

            _subscribersToRemove = new List<Subscriber>();
        }

        public void Visit(ConfirmedSubscriber subscriber)
        {
            if (SubscriberId == subscriber.SubscriberId || subscriber.ConnectionId == ConnectionId)
            {
                subscriber.Dispose();
                _subscribersToRemove.Add(subscriber);
            }
        }

        public void Visit(NotConfirmedSubscriber subscriber)
        {
            if (subscriber.ConnectionId == ConnectionId || subscriber.ConnectionExpire < DateTime.Now)
                _subscribersToRemove.Add(subscriber);
        }

        public IEnumerable<Subscriber> GetSubscribersToRemove() => _subscribersToRemove;
    }
}
