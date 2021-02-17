using NUnit.Framework;
using ShowingAds.AndroidApp.Core.Network;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Interfaces;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs;
using ShowingAds.AndroidApp.Core.Extensions;
using ShowingAds.CoreLibrary.Models.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class WebClientExecutorTest
    {
        private Dictionary<Guid, string> _videos;

        [SetUp]
        public void Setup()
        {
            _videos = new Dictionary<Guid, string>();
            for (int i = 0; i < 10; i++)
            {
                _videos.Add(Guid.NewGuid(), "f0ec8138-490f-4d67-b34b-540bc32515bc");
                _videos.Add(Guid.NewGuid(), "e70bdd98-68eb-4e60-9b90-9bec35a348ce");
                _videos.Add(Guid.NewGuid(), "50e76af0-b65c-40bb-80ad-cc898bf7e115");
            }
        }

        [Test]
        public async Task ExecuteTest()
        {
            var executor = new WebClientExecutor<VideoEventArgs>();
            executor.CommandExecuted += VideoDownloaded;
            foreach (var video in _videos)
            {
                var address = Settings.GetVideoDownloadUri(video.Value);
                executor.AddCommandToQueue(new VideoDownloadCommand(address, video.Key.ToString(),
                    Guid.Empty, new VideoJson(video.Key)));
            }
            await Task.Delay(TimeSpan.FromMinutes(1));
            executor.Dispose();
            await Task.Delay(TimeSpan.FromSeconds(10));
            Assert.Pass();
        }

        [Test]
        public async Task UndoTest()
        {
            var executor = new WebClientExecutor<VideoEventArgs>();
            executor.CommandExecuted += VideoDownloaded;
            foreach (var video in _videos)
            {
                var address = Settings.GetVideoDownloadUri(video.Value);
                executor.AddCommandToQueue(new VideoDownloadCommand(address, video.Key.ToString(),
                    Guid.Empty, new VideoJson(video.Key)));
            }
            await Task.Delay(TimeSpan.FromSeconds(5));
            foreach (var video in _videos)
            {
                var filter = new GuidFilter(new[] { video.Key });
                executor.Filter(filter);
            }
            executor.Dispose();
            await Task.Delay(TimeSpan.FromSeconds(10));
            Assert.Pass();
        }

        private void VideoDownloaded(VideoEventArgs obj)
        {
            ServerLog.Debug(obj.Id.ToString(), obj.VideoPath);
        }

        [TearDown]
        public void Down()
        {
            foreach (var video in _videos)
                File.Delete(video.Key.ToString());
        }
    }
}
