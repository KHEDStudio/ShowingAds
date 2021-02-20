using NUnit.Framework;
using ShowingAds.AndroidApp.Core.BusinessCollections;
using ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators;
using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.DataAccess.Interfaces;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class ContentsEnumeratorTest
    {
        private TopLevelCollection<LowLevelCollection<Video>> _videos;

        [SetUp]
        public void Setup()
        {
            var fakeStore = new FakeStore<List<LowLevelCollection<Video>>>();
            _videos = new TopLevelCollection<LowLevelCollection<Video>>(fakeStore);
            for (int i = 0; i < 100; i++)
            {
                var ownerId = Guid.NewGuid();
                for (int j = 0; j < 100; j++)
                {
                    var visitor = new AddingVideoVisitor(new VideoEventArgs(ownerId, Guid.NewGuid(), string.Empty));
                    _videos.Accept(visitor);
                }
            }
        }

        [Test]
        public void EnumeratorsTest()
        {
            Assert.IsTrue(_videos.TryGetNext(out var collection));
            Assert.IsTrue(collection.TryGetRandom(out var first));
            Assert.IsTrue(collection.TryGetNext(out var last));
            while (first.Id != last.Id && collection.TryGetNext(out last)) { }
            Assert.Pass();
        }

        internal class FakeStore<T> : IDataStore<T>
        {
            public void Dispose() { }

            public T Load() => throw new NotImplementedException();

            public void Save(T obj) => Thread.Sleep(1);
        }
    }
}
