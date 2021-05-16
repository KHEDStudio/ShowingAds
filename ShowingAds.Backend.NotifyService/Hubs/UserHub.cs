using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShowingAds.Backend.NotifyService.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Hubs
{
    public class UserHub : Hub
    {
        public UserHub()
        {
            var manager = SubscriberManager.GetInstance();
            manager.NotifySubscriber += NotifySubscriberAsync;
        }

        private async void NotifySubscriberAsync(string connectionId)
        {
            try
            {
                await Clients.Client(connectionId).SendAsync("Notify");
            }
            catch
            {
                // empty...
            }
        }

        public override Task OnConnectedAsync()
        {
            var manager = SubscriberManager.GetInstance();
            manager.AddSubscriber(Context.ConnectionId);
            return Task.CompletedTask;
        }

        public Task<string> SetSubscriberIdAsync(Guid id)
        {
            var manager = SubscriberManager.GetInstance();
            manager.SetSubscriberId(Context.ConnectionId, id);
            return Task.FromResult(Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var manager = SubscriberManager.GetInstance();
            manager.RemoveSubscriber(Context.ConnectionId);
            return Task.CompletedTask;
        }
    }
}
