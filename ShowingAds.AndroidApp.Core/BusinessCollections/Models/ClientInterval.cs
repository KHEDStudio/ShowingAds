using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.Shared.Core.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Models
{
    public class ClientInterval : Component
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("interval"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Interval { get; private set; }

        [JsonConstructor]
        public ClientInterval(Guid id, TimeSpan interval)
        {
            Id = id;
            Interval = interval;
        }

        public override void Accept(BaseVisitor visitor) => visitor.VisitClientInterval(this);

        public override void Add(Component component) => throw new ArgumentException("ClientInterval doesn't have subcomponents");

        public override Guid GetId() => Id;

        public override bool IsValid(BaseFilter filter) => filter.FilterClientInterval(this);

        public override void Remove(Component component) => throw new ArgumentException("ClientInterval doesn't have subcomponents");
    }
}
