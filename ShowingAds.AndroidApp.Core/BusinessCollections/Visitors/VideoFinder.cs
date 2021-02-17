using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.Extensions;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Visitors
{
    public class VideoFinder : BaseVisitor
    {
        public bool IsFound { get; private set; }
        public Guid VideoId { get; private set; }

        public VideoFinder(Guid videoId)
        {
            IsFound = false;
            VideoId = videoId;
        }

        public override void VisitClientInterval(ClientInterval interval) => throw new ArgumentException("Cannot visit client interval");

        public override void VisitClientOrder(ClientOrder order) => throw new ArgumentException("Cannot visit advertising order");

        public override void VisitLowCollection<T>(LowLevelCollection<T> collection) { /* Can low level collection video therefore act nothing */ }

        public override void VisitTopCollection<T>(TopLevelCollection<T> collection) { /* Can visit top level collection therefore act nothing */ }

        public override void VisitVideo(Video video) => (video.Id == VideoId).IfTrue(() => IsFound = true);

        public override void VisitVideoDownloadCommand(VideoDownloadCommand command) => (command.Video.Id == VideoId).IfTrue(() => IsFound = true);

        public override void VisitLogoDownloadCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot visit download command");
    }
}
