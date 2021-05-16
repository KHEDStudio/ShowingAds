using ShowingAds.Backend.NotifyService.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Models
{
    public class NotConfirmedSubscriber : Subscriber
    {
        private const int VALIDITY_PERIOD_MINUTES = 1;

        public DateTime ConnectionExpire { get; private set; }

        public NotConfirmedSubscriber(string connectionId) : base(connectionId) => ConnectionExpire = DateTime.Now.AddMinutes(VALIDITY_PERIOD_MINUTES);

        public override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
