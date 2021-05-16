using ShowingAds.Backend.NotifyService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Visitors
{
    public interface IVisitor
    {
        public void Visit(ConfirmedSubscriber subscriber);
        public void Visit(NotConfirmedSubscriber subscriber);
    }
}
