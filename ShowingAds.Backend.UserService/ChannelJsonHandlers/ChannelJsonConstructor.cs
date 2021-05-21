using ShowingAds.Backend.UserService.Managers;
using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Core.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.ChannelJsonHandlers
{
    public class ChannelJsonConstructor : Singleton<ChannelJsonConstructor>
    {
        private readonly ChannelManager _channelManager = ChannelManager.GetInstance();
        private readonly ContentManager _contentManager = ContentManager.GetInstance();
        private readonly ContentVideoManager _videoManager = ContentVideoManager.GetInstance();
        private readonly ClientChannelManager _clientManager = ClientChannelManager.GetInstance();
        private readonly AdvertisingVideoManager _adsManager = AdvertisingVideoManager.GetInstance();
        private readonly OrderManager _orderManager = OrderManager.GetInstance();

        private ChannelJsonConstructor() { }

        public async Task<ChannelJson> ConstructAsync(Guid key)
        {
            var channel = await _channelManager.GetOrDefaultAsync(key);
            if (channel != default)
            {
                var contents = await GetContentsAsync(channel.ContentIds);
                var clients = await GetClientsAsync(channel.Id);
                var orders = await GetOrdersAsync(channel.Id);
                return new ChannelJson(channel.Id, channel.LogoLeft, channel.LogoRight, channel.Ticker, channel.TickerInterval,
                    channel.ReloadTime, channel.DisplayOrientation, contents, clients, orders);
            }
            return default;
        }

        private async Task<IEnumerable<OrderJson>> GetOrdersAsync(Guid channelId)
        {
            var orders = await _orderManager.GetCollectionOrNullAsync(x => x.ChannelId == channelId);
            return orders.Select(x => new OrderJson(x.ClientChannelConnectionId, x.OrderField));
        }

        private async Task<IEnumerable<ClientChannelJson>> GetClientsAsync(Guid channelId)
        {
            var clientChannels = new List<ClientChannelJson>();
            var clientsCollection = await _clientManager.GetCollectionOrNullAsync(x => x.ChannelId == channelId);
            foreach (var client in clientsCollection)
            {
                var videos = await _adsManager.GetCollectionOrNullAsync(y => client.AdvertisingVideosId.Contains(y.Id));
                clientChannels.Add(new ClientChannelJson(client.Id, client.Interval, videos.Select(x => new VideoJson(x.Id))));
            }
            return clientChannels;
        }

        private async Task<IEnumerable<ContentJson>> GetContentsAsync(IEnumerable<Guid> contentIds)
        {
            var contents = new List<ContentJson>();
            foreach (var contentId in contentIds)
            {
                var content = await _contentManager.GetOrDefaultAsync(contentId);
                if (content != default)
                {
                    var videos = await _videoManager.GetCollectionOrNullAsync(x => x.ContentId == content.Id);
                    contents.Add(new ContentJson(contentId, videos.Select(x => new VideoJson(x.Id))));
                }
            }
            return contents;
        }
    }
}
