using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs;
using ShowingAds.Shared.Core.Models.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters
{
    public class VideoFilter : BaseFilter
    {
        /* 
         * Parameters:
         * Guid is id of VideoJson
         * bool is flag shows VideoJson already found
         * Guid is owners id of VideoJson
         * VideoJson is VideoJson)
         */
        private readonly Dictionary<Guid, (bool, Guid, VideoJson)> _videos;

        /* (Guid, VideoJson) where Guid is owners id */
        public VideoFilter(IEnumerable<(Guid, VideoJson)> videos)
        {
            _videos = new Dictionary<Guid, (bool, Guid, VideoJson)>();
            foreach (var (ownerId, video) in videos)
                _videos.Add(video.Id, (false, ownerId, video));
        }

        public override bool FilterLogoCommand(LogoDownloadCommand command) => throw new ArgumentException("Cannot filter logo download command");

        public override bool FilterVideo(Video video) => Filter(video.Id);

        public override bool FilterVideoCommand(VideoDownloadCommand command) => Filter(command.Video.Id);

        private bool Filter(Guid id)
        {
            if (_videos.ContainsKey(id))
            {
                _videos[id] = (true, _videos[id].Item2, _videos[id].Item3);
                return true;
            }
            return false;
        }

        public override bool FilterClientInterval(ClientInterval interval) => throw new ArgumentException("Cannot filter client interval");

        public override bool FilterClientOrder(ClientOrder order) => throw new ArgumentException("Cannot filter client order");

        public override IEnumerable<BaseDownloadCommand> GetDownloadCommands()
        {
            foreach (var (isFound, ownerId, video) in _videos.Values)
                if (isFound == false)
                    yield return new VideoDownloadCommand(Settings.GetVideoDownloadUri(video.Id),
                        Settings.GetVideoFilesPath(video.Id), ownerId, video);
        }

        public override IEnumerable<BaseVisitor> GetVisitors()
        {
            foreach (var (isFound, ownerId, video) in _videos.Values)
                if (isFound == false)
                    yield return new AddingVideoVisitor(
                        new VideoEventArgs(ownerId, video.Id, string.Empty));
        }

        public override bool FilterLogotype(Logotype logotype) => throw new ArgumentException("Cannot filter logo");
    }
}
