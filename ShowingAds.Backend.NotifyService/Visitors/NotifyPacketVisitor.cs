using ShowingAds.Backend.NotifyService.Models;
using ShowingAds.Shared.Backend.Models.NotifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Visitors
{
    public class NotifyPacketVisitor : IVisitor
    {
        public NotifyPacket NotifyPacket { get; private set; }

        public NotifyPacketVisitor(NotifyPacket notifyPacket)
        {
            NotifyPacket = notifyPacket ?? throw new ArgumentNullException(nameof(notifyPacket));
        }

        public void Visit(ConfirmedSubscriber subscriber)
        {
            if (NotifyPacket.Recipients.Contains(subscriber.SubscriberId))
                subscriber.AddNotification(new Notification(NotifyPacket));
        }

        public void Visit(NotConfirmedSubscriber subscriber)
        {
            // empty...
        }
    }
}
