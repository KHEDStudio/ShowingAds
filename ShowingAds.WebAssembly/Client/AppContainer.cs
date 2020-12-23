using Microsoft.AspNetCore.SignalR.Client;
using ShowingAds.CoreLibrary;
using ShowingAds.CoreLibrary.Abstract;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.WebAssembly.Client.BusinessLayer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Client
{
    public class AppContainer : Singleton<AppContainer>
    {
        public event Action ChannelUpdated;

        public User User { get; set; }
        private HubConnection _notifyHub { get; }
        private Guid _lastMessageUUID { get; set; }

        public ChannelManager ChannelManager { get; private set; }
        public UserManager UserManager { get; private set; }
        public ContentManager ContentManager { get; private set; }
        public ContentVideoManager ContentVideoManager { get; private set; }
        public AdvertisingClientManager AdvertisingClientManager { get; private set; }
        public ClientChannelManager ClientChannelManager { get; private set; }
        public AdvertisingVideoManager AdvertisingVideoManager { get; private set; }
        public OrderManager OrderManager { get; private set; }
        public DeviceManager DeviceManager { get; private set; }

        private AppContainer()
        {
            _notifyHub = new HubConnectionBuilder()
                .WithUrl(Settings.NotifyPath)
                .Build();
            _notifyHub.Closed += HubConnectionStartAsync;
            _notifyHub.On<Guid>("ChannelUpdated", async (messageUUID) =>
            {
                await _notifyHub.InvokeAsync("MessageUUID", messageUUID);
                if (_lastMessageUUID != messageUUID)
                {
                    _lastMessageUUID = messageUUID;
                    await ChannelUpdate();
                }
            });
            ChannelManager = ChannelManager.GetInstance();
            UserManager = UserManager.GetInstance();
            ContentManager = ContentManager.GetInstance();
            ContentVideoManager = ContentVideoManager.GetInstance();
            AdvertisingClientManager = AdvertisingClientManager.GetInstance();
            ClientChannelManager = ClientChannelManager.GetInstance();
            AdvertisingVideoManager = AdvertisingVideoManager.GetInstance();
            OrderManager = OrderManager.GetInstance();
            DeviceManager = DeviceManager.GetInstance();
            Task.Run(ChannelUpdate);
        }

        private async Task ChannelUpdate()
        {
            await ChannelManager.UpdateOrInitializeModels();
            await UserManager.UpdateOrInitializeModels();
            await ContentManager.UpdateOrInitializeModels();
            await ContentVideoManager.UpdateOrInitializeModels();
            await AdvertisingClientManager.UpdateOrInitializeModels();
            await ClientChannelManager.UpdateOrInitializeModels();
            await AdvertisingVideoManager.UpdateOrInitializeModels();
            await OrderManager.UpdateOrInitializeModels();
            await DeviceManager.UpdateOrInitializeModels();
            ChannelUpdated?.Invoke();
        }

        public async Task HubConnectionStartAsync(Exception arg)
        {
            Console.WriteLine(arg?.Message);
            while (_notifyHub.State != HubConnectionState.Connected)
            {
                await _notifyHub.StartAsync();
                var guid = User.Id.ToGuid();
                await _notifyHub.SendAsync("ClientConnectedAsync", guid);
            }
        }
    }
}
