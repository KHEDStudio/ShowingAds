using Newtonsoft.Json;
using ShowingAds.Shared.Backend.Enums;
using ShowingAds.Shared.Backend.Models.NotifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Models
{
    public class Notification
    {
        [JsonProperty("operation")]
        public Operation Operation { get; private set; }
        [JsonProperty("type")]
        public string ModelType { get; private set; }
        [JsonProperty("model")]
        public object Model { get; private set; }

        public Notification(NotifyPacket notifyPacket)
        {
            Operation = notifyPacket.Operation;
            ModelType = notifyPacket.ModelType ?? throw new ArgumentNullException(nameof(ModelType));
            Model = notifyPacket.Model ?? throw new ArgumentNullException(nameof(Model));
        }
    }
}
