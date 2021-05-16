using ShowingAds.NotifyService.BusinessLayer.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.NotifyService.BusinessLayer.Models
{
    public class ChannelUpdated : NotifySender
    {
        public override Guid MessageUUID { get; }
        public override Guid ClientUUID { get; }
        protected override string ConnectionId { get; }

        protected override string MethodName => GetType().Name;

        protected override string Json { get; }

        public ChannelUpdated(Guid client, string connectionId, string json = "")
        {
            MessageUUID = Guid.NewGuid();
            ClientUUID = client;
            ConnectionId = connectionId;
            Json = json;
        }
    }
}
