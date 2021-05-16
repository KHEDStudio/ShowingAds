using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Models.Json;
using ShowingAds.WebAssembly.Server.BusinessLayer.Interfaces;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server.BusinessLayer
{
    public class ChannelJsonConstructor : IConstructor<Guid, ChannelJson>
    {
        private ChannelManager _channelManager => ChannelManager.GetInstance();
        private ContentManager _contentManager => ContentManager.GetInstance();
        private ContentVideoManager _videoManager => ContentVideoManager.GetInstance();
        private ClientChannelManager _clientManager => ClientChannelManager.GetInstance();
        private AdvertisingVideoManager _adsManager => AdvertisingVideoManager.GetInstance();
        private OrderManager _orderManager => OrderManager.GetInstance();

        public async Task<ChannelJson> Construct(Guid key)
        {
            var (isExists, channel) = await _channelManager.TryGetAsync(key);
            if (isExists)
            {
                var contents = GetContents(channel.ContentsId);
                var clients = GetClients(channel.Id);
                var orders = GetOrders(channel.Id);
                return new ChannelJson(channel, contents, clients, orders);
            }
            return default;
        }

        private List<OrderJson> GetOrders(Guid channelId)
        {
            var orders = new List<OrderJson>();
            _orderManager.GetCollectionAsync(x => x.ChannelId == channelId).Result
                .ToList().ForEach(x => orders.Add(new OrderJson(x.ClientChannelConnectionId, x.OrderField)));
            return orders;
        }

        private List<ClientChannelJson> GetClients(Guid channelId)
        {
            var clients = new List<ClientChannelJson>();
            var clientsCollection = _clientManager.GetCollectionAsync(x => x.ChannelId == channelId).Result.ToList();
            clientsCollection.ForEach(x =>
                {
                    var videos = _adsManager.GetCollectionAsync(y => x.AdvertisingVideosId.Contains(y.Id)).Result
                        .Select(x => new VideoJson(x.Id)).ToList();
                    clients.Add(new ClientChannelJson(x.Id, x.Interval, videos));
                });
            return clients;
        }

        private List<ContentJson> GetContents(List<Guid> contentsId)
        {
            var contents = new List<ContentJson>();
            contentsId.ForEach(x =>
            {
                var (isExists, content) = _contentManager.TryGetAsync(x).Result;
                if (isExists)
                {
                    var videos = _videoManager.GetCollectionAsync(x => x.ContentId == content.Id).Result
                        .Select(x => new VideoJson(x.Id)).ToList();
                    contents.Add(new ContentJson(x, videos));
                }
            });
            return contents;
        }

        public async Task<string> ConstructAsJson(Guid key)
        {
            var channelJson = await Construct(key);
            return JsonConvert.SerializeObject(channelJson);
        }

        public Task<string> ConstructAsJson(ChannelJson value)
        {
            return Task.FromResult(JsonConvert.SerializeObject(value));
        }
    }
}
