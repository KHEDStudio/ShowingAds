using ShowingAds.Backend.UserService.Managers;
using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Backend.Enums;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Backend.Models.DeviceService;
using ShowingAds.Shared.Core.Enums;
using ShowingAds.Shared.Core.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.ChannelJsonHandlers
{
    public class ChannelJsonPublisher : Singleton<ChannelJsonPublisher>
    {
        private readonly ChannelManager _channelManager = ChannelManager.GetInstance();
        private readonly ContentManager _contentManager = ContentManager.GetInstance();
        private readonly ContentVideoManager _videoManager = ContentVideoManager.GetInstance();
        private readonly ClientChannelManager _clientChannelManager = ClientChannelManager.GetInstance();
        private readonly AdvertisingClientManager _clientManager = AdvertisingClientManager.GetInstance();
        private readonly AdvertisingVideoManager _adsManager = AdvertisingVideoManager.GetInstance();
        private readonly OrderManager _orderManager = OrderManager.GetInstance();

        private ChannelJsonPublisher()
        {
            _channelManager.ModelUpdated += ChannelUpdated;
            _channelManager.ModelDeleted += ChannelDeleted;

            _contentManager.ModelUpdated += ContentUpdated;
            _contentManager.ModelDeleted += ContentUpdated;

            _videoManager.ModelUpdated += ContentVideoUpdated;
            _videoManager.ModelDeleted += ContentVideoUpdated;

            _clientChannelManager.ModelUpdated += ClientChannelUpdated;
            _clientChannelManager.ModelDeleted += ClientChannelUpdated;

            _clientManager.ModelUpdated += ClientUpdated;
            _clientManager.ModelDeleted += ClientUpdated;

            _adsManager.ModelUpdated += AdsUpdated;
            _adsManager.ModelDeleted += AdsUpdated;

            _orderManager.ModelUpdated += OrderUpdated;
            _orderManager.ModelDeleted += OrderUpdated;
        }

        private async void OrderUpdated(Order order) => 
            await PublishChannelJsonAsync(Operation.CreateOrUpdate, order.ChannelId);

        private async void AdsUpdated(AdvertisingVideo video)
        {
            var clientChannels = await _clientChannelManager.GetCollectionOrNullAsync(x => x.AdvertisingClientId == video.AdvertisingClientId);
            if (clientChannels != null)
                foreach (var clientChannel in clientChannels)
                    await PublishChannelJsonAsync(Operation.CreateOrUpdate, clientChannel.ChannelId);
        }

        private async void ClientUpdated(AdvertisingClient client)
        {
            var clientChannels = await _clientChannelManager.GetCollectionOrNullAsync(x => x.AdvertisingClientId == client.Id);
            if (clientChannels != null)
                foreach (var clientChannel in clientChannels)
                    await PublishChannelJsonAsync(Operation.CreateOrUpdate, clientChannel.ChannelId);
        }

        private async void ClientChannelUpdated(ClientChannel clientChannel) =>
            await PublishChannelJsonAsync(Operation.CreateOrUpdate, clientChannel.ChannelId);

        private async void ContentVideoUpdated(ContentVideo video)
        {
            var channels = await _channelManager.GetCollectionOrNullAsync(x => x.ContentIds.Contains(video.ContentId));
            if (channels != null)
                foreach (var channel in channels)
                    await PublishChannelJsonAsync(Operation.CreateOrUpdate, channel.Id);
        }

        private async void ContentUpdated(Content content)
        {
            var channels = await _channelManager.GetCollectionOrNullAsync(x => x.ContentIds.Contains(content.Id));
            if (channels != null)
                foreach (var channel in channels)
                    await PublishChannelJsonAsync(Operation.CreateOrUpdate, channel.Id);
        }

        private async void ChannelUpdated(Channel channel) => 
            await PublishChannelJsonAsync(Operation.CreateOrUpdate, channel.Id);

        private async void ChannelDeleted(Channel channel) =>
            await PublishChannelJsonAsync(Operation.Delete, channel.Id);

        private async Task<ChannelJson> GetChannelJsonAsync(Guid channelId)
        {
            var constructor = ChannelJsonConstructor.GetInstance();
            return await constructor.ConstructAsync(channelId);
        }

        private async Task PublishChannelJsonAsync(Operation operation, Guid channelId)
        {
            ChannelJson channel;
            if (operation == Operation.Delete)
                channel = new ChannelJson(channelId, Guid.Empty, Guid.Empty, string.Empty, TimeSpan.Zero, TimeSpan.Zero,
                    DisplayOrientation.Horizontal, new List<ContentJson>(), new List<ClientChannelJson>(), new List<OrderJson>());
            else
                channel = await GetChannelJsonAsync(channelId);
            await Settings.RabbitMq.Publish(
                new NotifyChannelJson(operation, channel));
        }
    }
}
