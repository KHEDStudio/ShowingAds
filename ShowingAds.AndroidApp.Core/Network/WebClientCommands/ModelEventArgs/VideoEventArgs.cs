using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs
{
    public class VideoEventArgs : EventArgs
    {
        public Guid OwnerId { get; private set; }
        public Guid Id { get; private set; }
        public string VideoPath { get; private set; }

        public VideoEventArgs(Guid ownerId, Guid id, string videoPath)
        {
            OwnerId = ownerId;
            Id = id;
            VideoPath = videoPath ?? throw new ArgumentNullException(nameof(videoPath));
        }
    }
}
