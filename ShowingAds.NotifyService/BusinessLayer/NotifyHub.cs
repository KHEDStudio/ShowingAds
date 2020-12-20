using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ShowingAds.NotifyService.BusinessLayer.Interfaces;
using ShowingAds.NotifyService.BusinessLayer.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.NotifyService.BusinessLayer
{
    public class NotifyHub : Hub
    {
        private ILogger<NotifyHub> _logger { get; }
        private IClientManager _manager { get; }

        private Guid LastMessageUUID { get; set; }
        private readonly object _syncUUID = new object();

        public NotifyHub(ILogger<NotifyHub> logger)
        {
            _logger = logger;
            _manager = ClientManager.GetInstance();
            _manager.Notify += SendNotify;
            LastMessageUUID = Guid.Empty;
        }

        public void ClientConnectedAsync(Guid clientId)
        {
            _logger.LogInformation($"Client {clientId} connected -> {Context.ConnectionId}");
            _manager.AddClient(clientId, Context.ConnectionId);
        }

        private async void SendNotify(NotifySender sender)
        {
            _logger.LogInformation("Sending notification...");
            try
            {
                var uuid = Guid.Empty;
                do
                {
                    await sender.SendAsync(Clients);
                    await Task.Delay(200);
                    lock (_syncUUID)
                        uuid = LastMessageUUID;
                } while (sender.MessageUUID != uuid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            _logger.LogInformation("Notification sended");
        }

        public void MessageUUID(Guid uuid)
        {
            _logger.LogInformation("Set last message uuid");
            lock (_syncUUID)
                LastMessageUUID = uuid;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Client disconnected -> {Context.ConnectionId}");
            _manager.RemoveClient(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
