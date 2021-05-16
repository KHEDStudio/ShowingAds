using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.Shared.Core.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Models
{
    public class Video : Component
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("path")]
        public string VideoPath { get; private set; }

        [JsonConstructor]
        public Video(Guid id, string videoPath)
        {
            Id = id;
            VideoPath = videoPath ?? throw new ArgumentNullException(nameof(videoPath));
        }

        public override void Add(Component component) => throw new ArgumentException("Video doesn't have subcomponents");

        public override void Remove(Component component) => throw new ArgumentException("Video doesn't have subcomponents");

        public override void Accept(BaseVisitor visitor) => visitor.VisitVideo(this);

        public override bool IsValid(BaseFilter filter)
        {
            var isValid = filter.FilterVideo(this);
            return isValid;
        }

        public void DeleteVideoFile()
        {
            try
            {
                ServerLog.Debug("Next request", "video file delete");
                if (VideoPath != string.Empty)
                    File.Delete(VideoPath);
            }
            catch (Exception ex)
            {
                ServerLog.Error("VideoFile", ex.Message);
            }
        }

        public override Guid GetId() => Id;

        ~Video()
        {
            DeleteVideoFile();
        }
    }
}
