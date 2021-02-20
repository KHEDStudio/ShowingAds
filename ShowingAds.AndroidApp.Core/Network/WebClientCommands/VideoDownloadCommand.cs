using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs;
using ShowingAds.CoreLibrary.Models.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands
{
    public class VideoDownloadCommand : BaseDownloadCommand
    {
        public Guid OwnerId { get; private set; }
        public VideoJson Video { get; private set; }
        public string FilePath { get; private set; }

        public override event Action<EventArgs> Completed;
        public override event Action<ProgressChangedEventArgs> ProgressChanged;

        public VideoDownloadCommand(Uri address, string filePath, Guid ownerId, VideoJson video)
            : base(address, filePath)
        {
            OwnerId = ownerId;
            FilePath = filePath;
            Video = video ?? throw new ArgumentNullException(nameof(video));
        }

        /* Invoke Completed event with base._syncRoot lock for safe thread cancel executing */
        protected override void FileDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            lock (_syncRoot)
            {
                if (e.Cancelled == false && e.Error == null)
                {
                    var eventArgs = new VideoEventArgs(OwnerId, Video.Id, FilePath);
                    Completed?.Invoke(eventArgs);
                }
            }
        }

        protected override void FileProgressChanged(object sender, ProgressChangedEventArgs e) => ProgressChanged?.Invoke(e);

        public override bool IsValid(BaseFilter filter) => filter.FilterVideoCommand(this);

        public override void Accept(BaseVisitor visitor) => visitor.VisitVideoDownloadCommand(this);
    }
}
