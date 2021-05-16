using ShowingAds.Backend.NotifyService.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Models
{
    public abstract class Subscriber : IVisitable
    {
        public string ConnectionId { get; private set; }

        public Subscriber(string connectionId) => ConnectionId = connectionId;

        public abstract void Accept(IVisitor visitor);
    }
}
