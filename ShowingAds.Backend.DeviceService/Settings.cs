using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService
{
    public class Settings
    {
        public static string MongoConnectionString { get; } = "mongodb://showingads:2000muse@84.38.188.128";
        public static string MongoDatabaseName { get; } = "ShowingAds";
        public static MongoClient MongoClient { get; set; }

        public static IMongoCollection<BsonDocument> ChannelCollection { get; set; }
        public static IMongoCollection<BsonDocument> DeviceCollection { get; set; }
        public static IBus RabbitMq { get; set; }
        public static Uri RabbitMqPath { get; set; }
        public static string RabbitMqUsername { get; set; }
        public static string RabbitMqPassword { get; set; }
        public static Uri DjangoPath { get; set; }
        public static Uri SessionPath => new Uri(DjangoPath, "session/");
        public static Uri UsersPath => new Uri(DjangoPath, "users/");
        public static Uri DevicesPath => new Uri(DjangoPath, "devices/");
    }
}
