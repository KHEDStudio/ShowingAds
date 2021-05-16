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

        private readonly object _syncUUID = new object();
        private Dictionary<Guid, bool> _confirmationReceived = new Dictionary<Guid, bool>();

        public NotifyHub(ILogger<NotifyHub> logger)
        {
            _logger = logger;
            _manager = ClientManager.GetInstance();
            _manager.Notify += SendNotify;
        }

        public void ClientConnectedAsync(Guid clientId)
        {
            _logger.LogInformation($"Client {clientId} connected -> {Context.ConnectionId}");
            _manager.AddClient(clientId, Context.ConnectionId);
        }

        private async void SendNotify(NotifySender sender)
        {
            try
            {
                var random = new Random();
                lock (_syncUUID)
                    _confirmationReceived.Add(sender.MessageUUID, false);
                for (int i = 0; i < 3; i++)
                {
                    _logger.LogInformation($"Sending notification... Client {sender.ClientUUID} MessageUUID {sender.MessageUUID}");
                    await sender.SendAsync(Clients);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    lock (_syncUUID)
                        if (_confirmationReceived[sender.MessageUUID])
                            break;
                }
                lock (_syncUUID)
                {
                    if (_confirmationReceived[sender.MessageUUID] == false)
                        _logger.LogInformation($"Not response. Client {sender.ClientUUID} MessageUUID {sender.MessageUUID}");
                    else _logger.LogInformation($"Notification sent. Client {sender.ClientUUID} MessageUUID {sender.MessageUUID}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _logger.LogInformation($"Notification didn't send. Client {sender.ClientUUID} MessageUUID {sender.MessageUUID}");
            }
            finally
            {
                lock (_syncUUID)
                {
                    _confirmationReceived.Remove(sender.MessageUUID);
                    _logger.LogInformation($"Remaining notifications {_confirmationReceived.Count}");
                }
            }
        }

        public void MessageUUID(Guid uuid)
        {
            _logger.LogInformation($"Ack message uuid {uuid}");
            lock (_syncUUID)
                if (_confirmationReceived.ContainsKey(uuid))
                    _confirmationReceived[uuid] = true;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Client disconnected -> {Context.ConnectionId}");
            _manager.RemoveClient(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
