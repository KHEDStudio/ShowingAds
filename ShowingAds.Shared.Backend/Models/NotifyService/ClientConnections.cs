using System;
using System.Collections.Generic;

namespace ShowingAds.Shared.Core.Models.NotifyService
{
    public class ClientConnections : ICloneable, IModel<Guid>
    {
        public Guid Client { get; }
        public List<string> Connections { get; }

        public ClientConnections(Guid client, List<string> connections)
        {
            Client = client;
            Connections = connections ?? new List<string>();
        }

        public object Clone() => MemberwiseClone();

        public Guid GetKey() => Client;
    }
}
