using NUnit.Framework;
using ShowingAds.AndroidApp.Core.BusinessCollections;
using ShowingAds.AndroidApp.Core.BusinessCollections.Factory;
using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class ClientHandlerTest
    {
        private TopLevelCollection<LowLevelCollection<Video>> _clients;
        private TopLevelCollection<ClientInterval> _intervals;
        private TopLevelCollection<ClientOrder> _orders;

        [SetUp]
        public void Setup()
        {
            var clientsDeveloper = new AdvertisingClientsDeveloper();
            _clients = clientsDeveloper.Create();
            var intervalsDeveloper = new AdvertisingIntervalsDeveloper();
            _intervals = intervalsDeveloper.Create();
            var ordersDeveloper = new AdvertisingOrdersDeveloper();
            _orders = ordersDeveloper.Create();

            var firstClient = Guid.NewGuid();
            var secondClient = Guid.NewGuid();
            var thirdClient = Guid.NewGuid();

            _intervals.Add(new ClientInterval(firstClient, TimeSpan.FromMinutes(2)));
            _intervals.Add(new ClientInterval(secondClient, TimeSpan.FromMinutes(3)));
            _intervals.Add(new ClientInterval(thirdClient, TimeSpan.FromMinutes(1)));

            _orders.Add(new ClientOrder(secondClient, DateTime.Now));
            _orders.Add(new ClientOrder(thirdClient, DateTime.Now));

            for (int i = 0; i < 4; i++)
            {
                var visitor = new AddingVideoVisitor(new VideoEventArgs(firstClient, Guid.Parse("11111111-1111-1111-1111-11111111111" + i), string.Empty));
                _clients.Accept(visitor);
            }

            for (int i = 4; i < 7; i++)
            {
                var visitor = new AddingVideoVisitor(new VideoEventArgs(secondClient, Guid.Parse("11111111-1111-1111-1111-11111111111" + i), string.Empty));
                _clients.Accept(visitor);
            }

            for (int i = 7; i < 10; i++)
            {
                var visitor = new AddingVideoVisitor(new VideoEventArgs(thirdClient, Guid.Parse("11111111-1111-1111-1111-11111111111" + i), string.Empty));
                _clients.Accept(visitor);
            }
        }

        [Test]
        public void GetVideosTest()
        {
            for (uint i = 1; i < 100; i++)
            {
                var visitor = new ClientHandlerVisitor(i, false);
                _intervals.Accept(visitor);
                _orders.Accept(visitor);
                _clients.Accept(visitor);
                var videos = visitor.GetSortedVideos().ToList();
            }
            Assert.Pass();
        }
    }
}
