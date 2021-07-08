using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.Shared.Core.Models.States
{
    public class DeviceTasks : IModel<Guid>, ICloneable
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("priority"), JsonConverter(typeof(GuidConverter))]
        public Guid PriorityAdvertisingClient { get; private set; }
        [JsonProperty("is_updated")]
        public bool IsUpdated { get; private set; }
        [JsonProperty("take_screenshot")]
        public bool TakeScreenshot { get; private set; }
        [JsonProperty("reboot")]
        public bool Reboot { get; private set; }

        [JsonConstructor]
        public DeviceTasks(Guid id, Guid priority, bool is_updated, bool take_screenshot, bool reboot)
        {
            Id = id;
            PriorityAdvertisingClient = priority;
            IsUpdated = is_updated;
            TakeScreenshot = take_screenshot;
            Reboot = reboot;
        }

        public DeviceTasks(Guid id)
        {
            Id = id;
            PriorityAdvertisingClient = Guid.Empty;
            IsUpdated = true;
            TakeScreenshot = true;
            Reboot = false;
        }

        public Guid GetKey() => Id;

        public object Clone() => MemberwiseClone();
    }
}
