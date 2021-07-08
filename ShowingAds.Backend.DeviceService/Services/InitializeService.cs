using MassTransit;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using ShowingAds.Backend.DeviceService.Managers;
using ShowingAds.Shared.Backend.Models.DeviceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Services
{
    public class InitializeService : BackgroundService
    {
        private readonly IBus _rabbitMq;

        public InitializeService(IBus rabbitMq)
        {
            _rabbitMq = rabbitMq ?? throw new ArgumentNullException(nameof(rabbitMq));
            Settings.RabbitMq = _rabbitMq;

            Settings.MongoClient = new MongoClient(Settings.MongoConnectionString);
            var database = Settings.MongoClient.GetDatabase(Settings.MongoDatabaseName);
            Settings.DeviceCollection = database.GetCollection<BsonDocument>("Devices");
            Settings.ChannelCollection = database.GetCollection<BsonDocument>("Channels");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var endpoint = await _rabbitMq.GetSendEndpoint(new Uri("queue:update_channels"));
            await endpoint.Send(new UpdateChannelJson());
            DeviceManager.GetInstance();
        }
    }
}
