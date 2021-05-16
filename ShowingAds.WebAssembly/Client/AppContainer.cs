using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using ShowingAds.CoreLibrary;
using ShowingAds.CoreLibrary.Abstract;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.States;
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
        public event Action<DeviceState> DeviceUpdated;

        public User User { get; set; }
        private HubConnection _notifyHub { get; }

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
            ChannelManager = ChannelManager.GetInstance();
            _notifyHub.On<Guid>("ChannelUpdated", async (messageUUID) => await ChannelUpdate());
            UserManager = UserManager.GetInstance();
            ContentManager = ContentManager.GetInstance();
            ContentVideoManager = ContentVideoManager.GetInstance();
            AdvertisingClientManager = AdvertisingClientManager.GetInstance();
            ClientChannelManager = ClientChannelManager.GetInstance();
            AdvertisingVideoManager = AdvertisingVideoManager.GetInstance();
            OrderManager = OrderManager.GetInstance();
            DeviceManager = DeviceManager.GetInstance();
            _notifyHub.On<Guid, string>("DeviceUpdated", (messageUUID, json) =>
            {
                Console.WriteLine("Device updated!");
                _notifyHub.SendAsync("MessageUUID", messageUUID);
                if (json.Contains("VSTest"))
                    Console.WriteLine(json);
                var device = JsonConvert.DeserializeObject<DeviceState>(json);
                DeviceManager.UpdateModel(device.Id, device);
                DeviceUpdated?.Invoke(device);
            });
            Task.Run(ChannelUpdate);
        }

        private async Task ChannelUpdate()
        {
            Console.WriteLine("1");
            await ChannelManager.UpdateOrInitializeModels();
            Console.WriteLine("2");
            await UserManager.UpdateOrInitializeModels();
            Console.WriteLine("3");
            await ContentManager.UpdateOrInitializeModels();
            Console.WriteLine("4");
            await ContentVideoManager.UpdateOrInitializeModels();
            Console.WriteLine("5");
            await AdvertisingClientManager.UpdateOrInitializeModels();
            Console.WriteLine("6");
            await ClientChannelManager.UpdateOrInitializeModels();
            Console.WriteLine("7");
            await AdvertisingVideoManager.UpdateOrInitializeModels();
            Console.WriteLine("8");
            await OrderManager.UpdateOrInitializeModels();
            Console.WriteLine("9");
            await DeviceManager.UpdateOrInitializeModels();
            Console.WriteLine("10");
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
