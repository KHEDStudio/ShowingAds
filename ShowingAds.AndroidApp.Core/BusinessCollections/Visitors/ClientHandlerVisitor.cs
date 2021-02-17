using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Visitors
{
    public class ClientHandlerVisitor : BaseVisitor
    {
        private readonly uint _timerCount;
        private readonly bool _hasContentVideos;
        private List<ClientInterval> _intervals;
        private List<ClientOrder> _orders;
        private List<(LowLevelCollection<Video>, DateTime)> _clients;

        public ClientHandlerVisitor(uint timerCount, bool hasContentVideos)
        {
            _timerCount = timerCount;
            _hasContentVideos = hasContentVideos;
        }

        public override void VisitClientInterval(ClientInterval interval)
        {
            if (_intervals == null)
                _intervals = new List<ClientInterval>();

            if (interval.Interval.TotalMinutes == 0 || _timerCount % interval.Interval.TotalMinutes == 0)
                _intervals.Add(interval);
        }

        public override void VisitClientOrder(ClientOrder order)
        {
            if (_intervals == null)
                throw new ArgumentNullException("ClientInterval is the first for visit");

            if (_orders == null)
                _orders = new List<ClientOrder>();

            if (_hasContentVideos == false || _intervals.Any(x => x.Id == order.Id))
                _orders.Add(order);
        }

        public override void VisitLogoDownloadCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot visit logo download command");

        public override void VisitLowCollection<T>(LowLevelCollection<T> collection)
        {
            if (_intervals == null)
                throw new ArgumentNullException("ClientInterval is the first for visit");
            if (_orders == null)
                throw new ArgumentNullException("ClientOrder is the second for visit");

            if (_clients == null)
                _clients = new List<(LowLevelCollection<Video>, DateTime)>();

            var order = _orders.FirstOrDefault(x => x.Id == collection.Id);
            if (order != default)
            {
                _clients.Add((collection as LowLevelCollection<Video>, order.OrderField));
                _clients = _clients.OrderBy(x => x.Item2).ToList();
            }
        }

        public override void VisitTopCollection<T>(TopLevelCollection<T> collection) { }

        public override void VisitVideo(Video video) { }

        public override void VisitVideoDownloadCommand(VideoDownloadCommand command) => throw new ArgumentException("Cannot visit video download command");

        public IEnumerable<Video> GetSortedVideos()
        {
            if (_clients == null)
                _clients = new List<(LowLevelCollection<Video>, DateTime)>();
            foreach (var (client, orderField) in _clients)
                if (client.TryGetNext(out var video))
                    yield return video;
        }
    }
}
