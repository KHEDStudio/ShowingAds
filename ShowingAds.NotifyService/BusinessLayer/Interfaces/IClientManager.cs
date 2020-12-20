using ShowingAds.NotifyService.BusinessLayer.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.NotifyService.BusinessLayer.Interfaces
{
    public interface IClientManager
    {
        public event Action<NotifySender> Notify;
        public Task AddClient(Guid clientId, string connectionId);
        public Task RemoveClient(string connectionId);
    }
}
