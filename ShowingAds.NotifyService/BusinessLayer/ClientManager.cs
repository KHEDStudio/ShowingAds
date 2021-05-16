using Nito.AsyncEx;
using NLog;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Enums;
using ShowingAds.CoreLibrary.Managers;
using ShowingAds.CoreLibrary.Models.NotifyService;
using ShowingAds.NotifyService.BusinessLayer.Interfaces;
using ShowingAds.NotifyService.BusinessLayer.Models.Abstract;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.NotifyService.BusinessLayer
{
    public sealed class ClientManager : SaveModelManager<Guid, ClientConnections, ClientManager>, IClientManager, INotifier
    {
        private Logger _logger { get; }

        public event Action<NotifySender> Notify;

        private ClientManager() : base(new EmptyProvider<Guid, ClientConnections>())
        {
            _logger = LogManager.GetCurrentClassLogger();
            UpdateOrInitializeModelsAsync().Wait();
        }

        protected async override Task UpdateOrInitializeModelsAsync()
        {
            await Task.Yield();
            _logger.Info("Initialize client manager...");
        }

        public async Task AddClient(Guid clientId, string connectionId)
        {
            (var isExists, var client) = await TryGetAsync(clientId);
            if (isExists)
            {
                client.Connections.Add(connectionId);
                await TryAddOrUpdateAsync(clientId, client);
            }
            else
            {
                var connections = new List<string>();
                connections.Add(connectionId);
                client = new ClientConnections(clientId, connections);
                await TryAddOrUpdateAsync(clientId, client);
            }
            await NotifyConnectionStatus(clientId, DeviceStatus.Online);
        }

        public async Task RemoveClient(string connectionId)
        {
            (var isExists, var client) = await TryGetAsync(x => x.Connections.Contains(connectionId));
            if (isExists)
            {
                client.Connections.Remove(connectionId);
                if (client.Connections.Count > 0)
                    await TryAddOrUpdateAsync(client.Client, client);
                else await TryDeleteAsync(client.Client);
            }
            await NotifyConnectionStatus(client.Client, DeviceStatus.Offline);
        }

        private async Task NotifyConnectionStatus(Guid deviceId, DeviceStatus status)
        {
            using (var httpClient = new HttpClient())
            {
                var requestUri = new Uri(Settings.StatusPath, $"?status={status}&device={deviceId}");
                await httpClient.PostAsync(requestUri, default);
            }
        }

        public void AddNotifyTask(NotifySender task) => Notify?.Invoke(task);

        public async Task<(bool, IEnumerable<string>)> TryGetConnectionsAsync(Guid clientId)
        {
            (var isExists, var client) = await TryGetAsync(clientId);
            if (isExists)
                return (true, client.Connections);
            else return (false, default);
        }
    }
}
