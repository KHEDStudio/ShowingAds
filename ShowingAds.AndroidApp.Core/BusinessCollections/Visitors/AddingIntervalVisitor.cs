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
    public class AddingIntervalVisitor : BaseVisitor
    {
        public ClientInterval Interval { get; private set; }

        public AddingIntervalVisitor(ClientInterval interval)
        {
            Interval = interval ?? throw new ArgumentNullException(nameof(interval));
        }

        public override void VisitClientInterval(ClientInterval interval) { /* Can visit client interval therefore act nothing */ }

        public override void VisitClientOrder(ClientOrder order) => throw new ArgumentException("Cannot visit advertising order");

        public override void VisitLowCollection<T>(LowLevelCollection<T> collection) => throw new ArgumentException("Cannot visit low level collection");

        public override void VisitTopCollection<T>(TopLevelCollection<T> collection)
        {
            collection.Components.AsParallel().Any(x => x.GetId() == Interval.Id)
                .IfFalse(() => collection.Add(Interval));
        }

        public override void VisitVideo(Video video) => throw new ArgumentException("Cannot visit video");

        public override void VisitVideoDownloadCommand(VideoDownloadCommand command) => throw new ArgumentException("Cannot visit download command");

        public override void VisitLogoDownloadCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot visit download command");
    }
}
