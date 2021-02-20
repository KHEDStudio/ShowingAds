using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
        private List<(Guid, bool, DateTime)> _orders;

        /* (Guid, DateTime) where Guid is advertising client id */
        public ClientOrderFilter(IEnumerable<(Guid, DateTime)> orders)
        {
            _orders = new List<(Guid, bool, DateTime)>();
            foreach (var (clientId, orderField) in orders)
                _orders.Add((clientId, false, orderField));
        }

        public override bool FilterClientInterval(ClientInterval interval) => throw new ArgumentException("Cannot filter client interval");

        public override bool FilterClientOrder(ClientOrder order)
        {
            bool isFiltered = false;
            _orders = _orders.Select(x =>
            {
                if (x.Item1 == order.Id && x.Item3 == order.OrderField)
                {
                    isFiltered = true;
                    return (x.Item1, true, x.Item3);
                }
                return x;
            }).ToList();
            return isFiltered;
        }

        public override bool FilterLogoCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot filter logo download command");

        public override bool FilterLogotype(Logotype logotype) => throw new ArgumentException("Cannot filter logo");

        public override bool FilterVideo(Video video) => throw new ArgumentException("Cannot filter video");

        public override bool FilterVideoCommand(VideoDownloadCommand command) => throw new ArgumentException("Cannot filter video download command");

        public override IEnumerable<BaseDownloadCommand> GetDownloadCommands() => throw new ArgumentException("Have not downloading entities");

        public override IEnumerable<BaseVisitor> GetVisitors()
        {
            foreach (var (id, isFound, orderField) in _orders)
                if (isFound == false)
                    yield return new AddingOrderVisitor(new ClientOrder(id, orderField));
        }
    }
}
