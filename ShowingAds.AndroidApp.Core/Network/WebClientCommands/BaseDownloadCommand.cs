using ShowingAds.AndroidApp.Core.BusinessCollections.Interfaces;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Extensions;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands
{
    public abstract class BaseDownloadCommand : IWebClientCommand
    {
        private WebClient _downloader;
        private CancellationTokenSource _cancellationToken;
        protected readonly object _syncRoot = new object();

        private readonly Uri _address;
        private readonly string _filePath;

        public abstract event Action<EventArgs> Completed;
        public abstract event Action<ProgressChangedEventArgs> ProgressChanged;

        protected BaseDownloadCommand(Uri address, string filePath)
        {
            _downloader = new WebClient();
            _downloader.DownloadFileCompleted += FileDownloaded;
            _downloader.DownloadProgressChanged += FileProgressChanged;
            _cancellationToken = new CancellationTokenSource();
            _address = address ?? throw new ArgumentNullException(nameof(address));
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public abstract bool IsValid(BaseFilter filter);

        public abstract void Accept(BaseVisitor visitor);

        protected abstract void FileProgressChanged(object sender, ProgressChangedEventArgs e);

        protected abstract void FileDownloaded(object sender, AsyncCompletedEventArgs e);

        public void Execute()
        {
            try
            {
                if (_cancellationToken.IsCancellationRequested == false)
                    _downloader.DownloadFile(_address, _filePath);
                lock (_syncRoot)
                    if (_cancellationToken.IsCancellationRequested && File.Exists(_filePath))
                        File.Delete(_filePath);
                FileDownloaded(this, new AsyncCompletedEventArgs(default, _cancellationToken.Token.IsCancellationRequested, default));
            }
            catch (Exception ex)
            {
                File.Delete(_filePath);
                FileDownloaded(this, new AsyncCompletedEventArgs(ex, _cancellationToken.Token.IsCancellationRequested, default));
                throw;
            }
        }

        public void Undo()
        {
            lock (_syncRoot)
            {
                _cancellationToken.Cancel();
                _downloader.CancelAsync();
            }
        }

        ~BaseDownloadCommand()
        {
            _cancellationToken.Dispose();
            _downloader.Dispose();
        }
    }
}
