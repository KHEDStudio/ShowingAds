using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.Extensions;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Visitors
{
    public class AddingOrderVisitor : BaseVisitor
    {
        public ClientOrder Order { get; private set; }

        public AddingOrderVisitor(ClientOrder clientOrder)
        {
            Order = clientOrder ?? throw new ArgumentNullException(nameof(clientOrder));
        }

        public override void VisitClientInterval(ClientInterval interval) => throw new ArgumentException("Cannot visit client interval");

        public override void VisitClientOrder(ClientOrder order) { /* Can visit client order therefore act nothing */ }

        public override void VisitLowCollection<T>(LowLevelCollection<T> collection) => throw new ArgumentException("Cannot visit low level collection");

        public override void VisitTopCollection<T>(TopLevelCollection<T> collection)
        {
            collection.Add(Order);
        }

        public override void VisitVideo(Video video) => throw new ArgumentException("Cannot visit video");

        public override void VisitVideoDownloadCommand(VideoDownloadCommand command) => throw new ArgumentException("Cannot visit download command");

        public override void VisitLogoDownloadCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot visit download command");
    }
}
