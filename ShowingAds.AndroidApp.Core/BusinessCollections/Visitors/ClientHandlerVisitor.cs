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
        private readonly Guid _priorityClient;
        private readonly bool _hasContentVideos;
        private List<ClientInterval> _intervals;
        private List<ClientOrder> _orders;
        private List<(LowLevelCollection<Video>, DateTime)> _clients;

        public ClientHandlerVisitor(uint timerCount, Guid priorityClient, bool hasContentVideos)
        {
            _timerCount = timerCount;
            _priorityClient = priorityClient;
            _hasContentVideos = hasContentVideos;
            _intervals = new List<ClientInterval>();
            _orders = new List<ClientOrder>();
            _clients = new List<(LowLevelCollection<Video>, DateTime)>();
        }

        public override void VisitClientInterval(ClientInterval interval)
        {
            if (interval.Interval.TotalMinutes == 0 || _timerCount % interval.Interval.TotalMinutes == 0)
                _intervals.Add(interval);
        }

        public override void VisitClientOrder(ClientOrder order)
        {
            if (_hasContentVideos == false || _intervals.Any(x => x.Id == order.Id) || (_priorityClient != Guid.Empty && _priorityClient == order.Id))
                _orders.Add(order);
        }

        public override void VisitLogoDownloadCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot visit logo download command");

        public override void VisitLowCollection<T>(LowLevelCollection<T> collection)
        {
            if (collection is LowLevelCollection<Video> client)
            {
                if (_priorityClient != Guid.Empty && client.Id != _priorityClient)
                    return;
                var order = _orders.FirstOrDefault(x => x.Id == client.Id);
                if (order != default)
                {
                    _clients.Add((client, order.OrderField));
                    _clients = _clients.OrderBy(x => x.Item2).ToList();
                }
            }
        }

        public override void VisitTopCollection<T>(TopLevelCollection<T> collection) { }

        public override void VisitVideo(Video video) { }

        public override void VisitVideoDownloadCommand(VideoDownloadCommand command) => throw new ArgumentException("Cannot visit video download command");

        public IEnumerable<Video> GetSortedVideos()
        {
            foreach (var (client, orderField) in _clients)
                if (client.TryGetNext(out var video))
                    yield return video;
        }
    }
}
