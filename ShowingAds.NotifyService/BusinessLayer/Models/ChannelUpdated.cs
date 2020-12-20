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
        protected override string ConnectionId { get; }

        protected override string MethodName => GetType().Name;

        protected override string Json { get; }

        public ChannelUpdated(string connectionId)
        {
            MessageUUID = Guid.NewGuid();
            ConnectionId = connectionId;
            Json = string.Empty;
        }

        public ChannelUpdated(string connectionId, string json)
        {
            ConnectionId = connectionId;
            Json = json;
        }
    }
}
