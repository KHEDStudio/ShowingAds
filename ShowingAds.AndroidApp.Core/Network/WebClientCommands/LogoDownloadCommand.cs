using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.ModelEventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands
{
    public class LogoDownloadCommand : BaseDownloadCommand
    {
        public Guid Id { get; private set; }
        public string FilePath { get; private set; }
        public bool IsLeft { get; private set; }

        public override event Action<EventArgs> Completed;
        public override event Action<DownloadProgressChangedEventArgs> ProgressChanged;

        public LogoDownloadCommand(Uri address, string filePath, Guid id, bool isLeft)
            : base(address, filePath)
        {
            Id = id;
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            IsLeft = isLeft;
        }

        /* Invoke Completed event with base._syncRoot lock for safe thread cancel executing */
        protected override void FileDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            lock (_syncRoot)
            {
                if (e.Cancelled == false && e.Error == null)
                {
                    var eventArgs = new LogoEventArgs(Id, FilePath, IsLeft);
                    Completed?.Invoke(eventArgs);
                }
            }
        }

        protected override void FileProgressChanged(object sender, DownloadProgressChangedEventArgs e) => ProgressChanged?.Invoke(e);

        public override bool IsValid(BaseFilter filter) => filter.FilterLogoCommand(this);

        public override void Accept(BaseVisitor visitor) => visitor.VisitLogoDownloadCommand(this);
    }
}
