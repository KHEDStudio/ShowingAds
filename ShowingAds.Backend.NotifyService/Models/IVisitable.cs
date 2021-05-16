using ShowingAds.Backend.NotifyService.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Models
{
    public interface IVisitable
    {
        public void Accept(IVisitor visitor);
    }
}
