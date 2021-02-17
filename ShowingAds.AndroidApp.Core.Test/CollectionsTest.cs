using NUnit.Framework;
using ShowingAds.AndroidApp.Core.BusinessCollections.Factory;
using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs;
using ShowingAds.AndroidApp.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.BusinessCollections;
using System.IO;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class CollectionsTest
    {
        private List<Guid> _owners;
        private List<VideoEventArgs> _videos;

        [SetUp]
        public void Setup()
        {
            _owners = new List<Guid>();
            for (int i = 0; i < 3; i++)
                _owners.Add(Guid.NewGuid());

            _videos = new List<VideoEventArgs>();
            for (int i = 0; i < 100; i++)
                _videos.Add(new VideoEventArgs(_owners[(0, 3).RandomNumber()], Guid.NewGuid(), $"{i}.mp4"));
        }

        [Test]
        public void FillingCollectionTest()
        {
            var developer = new ContentsDeveloper();
            var contents = developer.Create();

            foreach (var video in _videos)
            {
                var visitor = new AddingVideoVisitor(video);
                contents.Accept(visitor);
            }
            Assert.AreEqual(_owners.Count, contents.Components.Count);
            int count = 0;
            foreach (var component in contents.Components)
                if (component is LowLevelCollection<Video> category)
                    count += category.Components.Count;
            Assert.AreEqual(_videos.Count, count);
        }

        [Test]
        public async Task SaveAndLoadCollectionTest()
        {
            var developer = new ContentsDeveloper();
            var contents = developer.Create();
            foreach (var video in _videos)
            {
                var visitor = new AddingVideoVisitor(video);
                contents.Accept(visitor);
            }

            await contents.SaveComponents();
            var loadedContents = developer.Create();
            await Task.Delay(TimeSpan.FromSeconds(1));

            for (int i = 0; i < contents.Components.Count; i++)
            {
                var components = contents.Components[i];
                var loadedComponents = loadedContents.Components[i];
                for (int j = 0; j < components.Components.Count; j++)
                {
                    var video = components.Components[j];
                    var loadedVideo = loadedComponents.Components[j];
                    Assert.AreEqual(video.Id, loadedVideo.Id);
                }
            }
        }

        [Test]
        public void FilterTest()
        {
            var developer = new ContentsDeveloper();
            var contents = developer.Create();
            foreach (var video in _videos)
            {
                var visitor = new AddingVideoVisitor(video);
                contents.Accept(visitor);
            }
            var filter = new GuidFilter(new[] { _videos[0].Id });
            contents.IsValid(filter);
            Assert.AreEqual(1, contents.Components.Count);
        }

        [TearDown]
        public void Down()
        {
            File.Delete(Settings.GetConfigFilePath("contents.config"));
        }
    }
}
