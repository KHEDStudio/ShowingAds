using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Visitors
{
    public class VideosCounterVisitor : BaseVisitor
    {
        public int TotalVideos { get; private set; } = 0;

        public override void VisitClientInterval(ClientInterval interval) => throw new ArgumentException("Cannot visit client interval");

        public override void VisitClientOrder(ClientOrder order) => throw new ArgumentException("Cannot visit advertising order");

        public override void VisitLogoDownloadCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot visit download command");

        public override void VisitLowCollection<T>(LowLevelCollection<T> collection) { /* Can low level collection video therefore act nothing */ }

        public override void VisitTopCollection<T>(TopLevelCollection<T> collection) { /* Can visit top level collection therefore act nothing */ }

        public override void VisitVideo(Video video) => TotalVideos++;

        public override void VisitVideoDownloadCommand(VideoDownloadCommand command) => TotalVideos++;
    }
}
