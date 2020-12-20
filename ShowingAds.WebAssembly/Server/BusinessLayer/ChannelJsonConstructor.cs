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
            var (isExists, channel) = await _channelManager.TryGet(key);
            if (isExists)
            {
                var contents = await GetContents(channel.ContentsId);
                var clients = await GetClients(channel.Id);
                var orders = await GetOrders(channel.Id);
                return new ChannelJson(channel, contents, clients, orders);
            }
            return default;
        }

        private async Task<List<OrderJson>> GetOrders(Guid channelId)
        {
            var orders = new List<OrderJson>();
            (await _orderManager.GetCollection(x => x.ChannelId == channelId))
                .AsParallel().ForAll(x => orders.Add(new OrderJson(x.ClientChannelConnectionId, x.OrderField)));
            return orders;
        }

        private async Task<List<ClientChannelJson>> GetClients(Guid channelId)
        {
            var clients = new List<ClientChannelJson>();
            (await _clientManager.GetCollection(x => x.ChannelId == channelId))
                .AsParallel().ForAll(async x =>
                {
                    var videos = (await _adsManager.GetCollection(y => x.AdvertisingVideosId.Contains(y.Id)))
                        .Select(x => new VideoJson(x.Id)).ToList();
                    clients.Add(new ClientChannelJson(x.Id, x.Interval, videos));
                });
            return clients;
        }

        private Task<List<ContentJson>> GetContents(List<Guid> contentsId)
        {
            var contents = new List<ContentJson>();
            contentsId.AsParallel().ForAll(async x =>
            {
                var (isExists, content) = await _contentManager.TryGet(x);
                if (isExists)
                {
                    var videos = (await _videoManager.GetCollection(x => x.ContentId == content.Id))
                        .Select(x => new VideoJson(x.Id)).ToList();
                    contents.Add(new ContentJson(x, videos));
                }
            });
            return Task.FromResult(contents);
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
