using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters
{
    public abstract class BaseFilter
    {
        public abstract bool FilterVideoCommand(VideoDownloadCommand command);
        public abstract bool FilterLogoCommand(LogoDownloadCommand command);

        public abstract bool FilterVideo(Video video);
        public abstract bool FilterClientInterval(ClientInterval interval);
        public abstract bool FilterClientOrder(ClientOrder order);
        public abstract bool FilterLogotype(Logotype logotype);

        public abstract IEnumerable<BaseDownloadCommand> GetDownloadCommands();
        public abstract IEnumerable<BaseVisitor> GetVisitors();
    }
}
