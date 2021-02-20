using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.Extensions;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Visitors
{
    public class AddingVideoVisitor : BaseVisitor
    {
        public VideoEventArgs VideoArgs { get; private set; }

        public AddingVideoVisitor(VideoEventArgs videoArgs)
        {
            VideoArgs = videoArgs ?? throw new ArgumentNullException(nameof(videoArgs));
        }

        public override void VisitLowCollection<T>(LowLevelCollection<T> collection)
        {
            if (collection.GetId() == VideoArgs.OwnerId) 
                collection.Components.Any(x => x.GetId() == VideoArgs.Id)
                    .IfFalse(() =>
                    {
                        var video = GetVideo(VideoArgs);
                        collection.Add(video);
                    });
        }

        public override void VisitTopCollection<T>(TopLevelCollection<T> collection)
        {
            collection.Components.Any(x => x.GetId() == VideoArgs.OwnerId)
                .IfFalse(() =>
                {
                    var videos = new List<Video> { GetVideo(VideoArgs) };
                    var lowCollection = new LowLevelCollection<Video>(VideoArgs.OwnerId, videos);
                    collection.Add(lowCollection);
                });
        }

        private Video GetVideo(VideoEventArgs args) => new Video(args.Id, args.VideoPath);

        public override void VisitVideo(Video video) { /* Can visit video therefore act nothing */ }

        public override void VisitClientInterval(ClientInterval interval) => throw new ArgumentException("Cannot visit client interval");

        public override void VisitClientOrder(ClientOrder order) => throw new ArgumentException("Cannot visit advertising order");

        public override void VisitVideoDownloadCommand(VideoDownloadCommand command) => throw new ArgumentException("Cannot visit download command");

        public override void VisitLogoDownloadCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot visit download command");
    }
}
