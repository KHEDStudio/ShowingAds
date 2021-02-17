using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters
{
    public class GuidFilter : BaseFilter
    {
        private readonly HashSet<Guid> _validGuids;

        public GuidFilter(IEnumerable<Guid> validGuids)
        {
            _validGuids = new HashSet<Guid>(validGuids);
        }

        public override bool FilterLogoCommand(LogoDownloadCommand command) => _validGuids.Contains(command.Id);

        public override bool FilterVideoCommand(VideoDownloadCommand command) => _validGuids.Contains(command.Video.Id);

        public override bool FilterVideo(Video video) => _validGuids.Contains(video.Id);

        public override bool FilterClientInterval(ClientInterval interval) => _validGuids.Contains(interval.Id);

        public override bool FilterClientOrder(ClientOrder order) => _validGuids.Contains(order.Id);

        public override IEnumerable<BaseDownloadCommand> GetDownloadCommands() => throw new ArgumentException("Have not downloading entities");

        public override IEnumerable<BaseVisitor> GetVisitors() => throw new ArgumentException("Have not visitables entities");

        public override bool FilterLogotype(Logotype logotype) => _validGuids.Contains(logotype.Id);
    }
}
