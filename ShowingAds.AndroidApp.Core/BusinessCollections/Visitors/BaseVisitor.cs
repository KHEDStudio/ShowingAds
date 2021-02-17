using ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators;
using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Visitors
{
    public abstract class BaseVisitor
    {
        public abstract void VisitTopCollection<T>(TopLevelCollection<T> collection) where T : Component;

        public abstract void VisitLowCollection<T>(LowLevelCollection<T> collection) where T : Component;

        public abstract void VisitVideo(Video video);

        public abstract void VisitClientInterval(ClientInterval interval);

        public abstract void VisitClientOrder(ClientOrder order);

        public abstract void VisitVideoDownloadCommand(VideoDownloadCommand command);

        public abstract void VisitLogoDownloadCommand(LogoDownloadCommand command);
    }
}
