using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters
{
    public class ClientIntervalFilter : BaseFilter
    {
        /* 
         * Parameters:
         * Guid is id of advertising client
         * bool is flag shows ClientInterval already found
         * TimeSpan is interval of advertising client
         */
        private readonly Dictionary<Guid, (bool, TimeSpan)> _intervals;

        /* (Guid, TimeSpan) where Guid is id of advertising client */
        public ClientIntervalFilter(IEnumerable<(Guid, TimeSpan)> intervals)
        {
            _intervals = new Dictionary<Guid, (bool, TimeSpan)>();
            foreach (var (id, interval) in intervals)
                _intervals.Add(id, (false, interval));
        }

        public override bool FilterClientInterval(ClientInterval interval)
        {
            if (_intervals.ContainsKey(interval.Id)
                && _intervals[interval.Id].Item2 == interval.Interval)
            {
                _intervals[interval.Id] = (true, _intervals[interval.Id].Item2);
                return true;
            }
            return false;
        }

        public override bool FilterClientOrder(ClientOrder order) => throw new ArgumentException("Cannot filter client order");

        public override bool FilterLogoCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot filter logo download command");

        public override bool FilterLogotype(Logotype logotype) => throw new ArgumentException("Cannot filter logo");

        public override bool FilterVideo(Video video) => throw new ArgumentException("Cannot filter video");

        public override bool FilterVideoCommand(VideoDownloadCommand command) => throw new ArgumentException("Cannot filter video download command");

        public override IEnumerable<BaseDownloadCommand> GetDownloadCommands() => throw new ArgumentException("Have not downloading entities");

        public override IEnumerable<BaseVisitor> GetVisitors()
        {
            foreach (var (id, (isFound, interval)) in _intervals)
                if (isFound == false)
                    yield return new AddingIntervalVisitor(new ClientInterval(id, interval));
        }
    }
}
