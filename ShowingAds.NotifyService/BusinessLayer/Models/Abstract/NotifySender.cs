using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.NotifyService.BusinessLayer.Models.Abstract
{
    public abstract class NotifySender
    {
        public abstract Guid MessageUUID { get; }
        public abstract Guid ClientUUID { get; }
        protected abstract string ConnectionId { get; }
        protected abstract string MethodName { get; }
        protected abstract string Json { get; }

        public async virtual Task SendAsync(IHubCallerClients hubCaller)
        {
            if (Json == string.Empty)
                await hubCaller.Client(ConnectionId).SendAsync(MethodName, MessageUUID);
            await hubCaller.Client(ConnectionId).SendAsync(MethodName, MessageUUID, Json);
        }
    }
}
