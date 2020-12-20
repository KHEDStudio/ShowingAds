using ShowingAds.NotifyService.BusinessLayer.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.NotifyService.BusinessLayer.Interfaces
{
    public interface INotifier
    {
        public void AddNotifyTask(NotifySender task);
        public Task<(bool, IEnumerable<string>)> TryGetConnections(Guid clientId);
    }
}
