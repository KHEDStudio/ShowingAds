using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.Shared.Core.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Models
{
    public class ClientOrder : Component
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("order")]
        public DateTime OrderField { get; private set; }

        [JsonConstructor]
        public ClientOrder(Guid id, DateTime orderField)
        {
            Id = id;
            OrderField = orderField;
        }

        public override void Accept(BaseVisitor visitor) => visitor.VisitClientOrder(this);

        public override void Add(Component component) => throw new ArgumentException("ClientOrder doesn't have subcomponents");

        public override Guid GetId() => Id;

        public override bool IsValid(BaseFilter filter) => filter.FilterClientOrder(this);

        public override void Remove(Component component) => throw new ArgumentException("ClientOrder doesn't have subcomponents");
    }
}
