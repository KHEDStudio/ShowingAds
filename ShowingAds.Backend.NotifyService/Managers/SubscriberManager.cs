using ShowingAds.Backend.NotifyService.Models;
using ShowingAds.Backend.NotifyService.Visitors;
using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Backend.Models.NotifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Managers
{
    public class SubscriberManager : Singleton<SubscriberManager>
    {
        private readonly TimeSpan HANDLER_INTERVAL = TimeSpan.FromSeconds(10);

        public event Action<string> NotifySubscriber;

        private List<Subscriber> _subscribers;
        private readonly object _syncSubscribers = new();

        private SubscriberManager()
        {
            _subscribers = new List<Subscriber>();
            Task.Factory.StartNew(HandlerLoop, TaskCreationOptions.LongRunning);
        }

        public void AddSubscriber(string connectionId)
        {
            var notConfirmedSubscriber = new NotConfirmedSubscriber(connectionId);
            lock (_syncSubscribers)
                _subscribers.Add(notConfirmedSubscriber);
        }

        public void SetSubscriberId(string connectionId, Guid subscriberId)
        {
            var visitor = new SubscriberIdVisitor(connectionId, subscriberId);
            lock (_syncSubscribers)
            {
                foreach (var subscriber in _subscribers)
                    subscriber.Accept(visitor);
                if (visitor.SubscriberToAdd != null && visitor.SubscriberToRemove != null)
                {
                    visitor.SubscriberToAdd.NotifySubscriber += NotifySubscriber;
                    _subscribers.Add(visitor.SubscriberToAdd);
                    _subscribers.Remove(visitor.SubscriberToRemove);
                }
            }
        }

        public void RemoveSubscriber(string connectionId) => RemoveSubscribers(new RemoveSubscriberVisitor(Guid.Empty, connectionId));

        public void NotifyPacketReceived(NotifyPacket notifyPacket)
        {
            var visitor = new NotifyPacketVisitor(notifyPacket);
            lock (_syncSubscribers)
                foreach (var subscriber in _subscribers)
                    subscriber.Accept(visitor);
        }

        public IEnumerable<Notification> GetNotifications(string connectionId, Guid subscriberId)
        {
            var visitor = new NotificationVisitor(subscriberId, connectionId);
            lock (_syncSubscribers)
                foreach (var subscriber in _subscribers)
                    subscriber.Accept(visitor);
            return visitor.Notifications;
        }

        private void RemoveSubscribers(RemoveSubscriberVisitor visitor)
        {
            lock (_syncSubscribers)
            {
                foreach (var subscriber in _subscribers)
                    subscriber.Accept(visitor);
                foreach (var subscriber in visitor.GetSubscribersToRemove())
                    _subscribers.Remove(subscriber);
            }
        }

        private async Task HandlerLoop()
        {
            while (true)
            {
                try
                {
                    RemoveSubscribers(new RemoveSubscriberVisitor(Guid.Empty, string.Empty));
                }
                catch
                {
                    // empty...
                }
                finally
                {
                    await Task.Delay(HANDLER_INTERVAL);
                }
            }
        }
    }
}
