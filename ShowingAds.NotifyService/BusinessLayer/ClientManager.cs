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
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.NotifyService.BusinessLayer
{
    public sealed class ClientManager : ModelManager<Guid, ClientConnections, ClientManager>, IClientManager, INotifier
    {
        private Logger _logger { get; }
        private AsyncLock _syncMutex { get; }

        public event Action<NotifySender> Notify;
        private BlockingCollection<NotifySender> _senders { get; set; }

        private ClientManager() : base(new EmptyProvider<Guid, ClientConnections>())
        {
            _logger = LogManager.GetCurrentClassLogger();
            _syncMutex = new AsyncLock();
            UpdateOrInitializeModels(default, default);
        }

        protected override void UpdateOrInitializeModels(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Initialize manager...");
            _senders = new BlockingCollection<NotifySender>();
            Task.Run(async () => await Sender());
        }

        public async Task AddClient(Guid clientId, string connectionId)
        {
            using (await _syncMutex.LockAsync())
            {
                (var isExists, var client) = await TryGet(clientId);
                if (isExists)
                {
                    client.Connections.Add(connectionId);
                    await TryAddOrUpdate(clientId, client);
                }
                else
                {
                    var connections = new List<string>();
                    connections.Add(connectionId);
                    client = new ClientConnections(clientId, connections);
                    await TryAddOrUpdate(clientId, client);
                }
                NotifyConnectionStatus(clientId, DeviceStatus.Online);
            }
        }

        public async Task RemoveClient(string connectionId)
        {
            using (await _syncMutex.LockAsync())
            {
                (var isExists, var client) = await TryGet(x => x.Connections.Contains(connectionId));
                if (isExists)
                {
                    client.Connections.Remove(connectionId);
                    if (client.Connections.Count > 0)
                        await TryAddOrUpdate(client.Client, client);
                    else await TryDelete(client.Client);
                }
                NotifyConnectionStatus(client.Client, DeviceStatus.Offline);
            }
        }

        private async Task NotifyConnectionStatus(Guid deviceId, DeviceStatus status)
        {
            using (var httpClient = new HttpClient())
            {
                var requestUri = new Uri(Settings.StatusPath, $"?status={status}&device={deviceId}");
                await httpClient.PostAsync(requestUri, default);
            }
        }

        public void AddNotifyTask(NotifySender task) => _senders.Add(task);

        public async Task<(bool, IEnumerable<string>)> TryGetConnections(Guid clientId)
        {
            using (await _syncMutex.LockAsync())
            {
                (var isExists, var client) = await TryGet(clientId);
                if (isExists)
                    return (true, client.Connections);
                else return (false, default);
            }
        }

        private async Task Sender()
        {
            while (_senders.IsCompleted == false)
            {
                try
                {
                    var sender = _senders.Take();
                    Notify?.Invoke(sender);
                }
                catch(Exception ex)
                {
                    _logger.Error(ex);
                }
                await Task.Delay(200);
            }
        }
    }
}
