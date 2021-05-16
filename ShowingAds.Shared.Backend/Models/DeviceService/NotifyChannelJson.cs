using ShowingAds.Shared.Backend.Enums;
using ShowingAds.Shared.Core.Models.Json;
using System;

namespace ShowingAds.Shared.Backend.Models.DeviceService
{
    public class NotifyChannelJson
    {
        public Operation Operation { get; private set; }
        public ChannelJson Channel { get; private set; }

        public NotifyChannelJson(Operation operation, ChannelJson channel)
        {
            Operation = operation;
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }
    }
}
