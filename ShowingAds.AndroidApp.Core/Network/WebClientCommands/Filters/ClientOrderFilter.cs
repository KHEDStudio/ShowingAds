using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters
{
    public class ClientOrderFilter : BaseFilter
    {
        /* 
         * Parameters:
         * Guid is id of advertising client
         * bool is flag shows client order already found
         * DateTime is order field for sorting
         */
        private readonly Dictionary<Guid, (bool, DateTime)> _orders;

        /* (Guid, DateTime) where Guid is advertising client id */
        public ClientOrderFilter(IEnumerable<(Guid, DateTime)> orders)
        {
            _orders = new Dictionary<Guid, (bool, DateTime)>();
            foreach (var (clientId, orderField) in orders)
                _orders.Add(clientId, (false, orderField));
        }

        public override bool FilterClientInterval(ClientInterval interval) => throw new ArgumentException("Cannot filter client interval");

        public override bool FilterClientOrder(ClientOrder order)
        {
            if (_orders.ContainsKey(order.Id)
                && _orders[order.Id].Item2 == order.OrderField)
            {
                _orders[order.Id] = (true, _orders[order.Id].Item2);
                return true;
            }
            return false;
        }

        public override bool FilterLogoCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot filter logo download command");

        public override bool FilterLogotype(Logotype logotype) => throw new ArgumentException("Cannot filter logo");

        public override bool FilterVideo(Video video) => throw new ArgumentException("Cannot filter video");

        public override bool FilterVideoCommand(VideoDownloadCommand command) => throw new ArgumentException("Cannot filter video download command");

        public override IEnumerable<BaseDownloadCommand> GetDownloadCommands() => throw new ArgumentException("Have not downloading entities");

        public override IEnumerable<BaseVisitor> GetVisitors()
        {
            foreach (var (id, (isFound, orderField)) in _orders)
                if (isFound == false)
                    yield return new AddingOrderVisitor(new ClientOrder(id, orderField));
        }
    }
}
