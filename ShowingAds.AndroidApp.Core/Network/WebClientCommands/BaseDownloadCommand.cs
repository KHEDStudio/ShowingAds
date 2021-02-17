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
        private WebClient _webClient;
        private CancellationTokenSource _cancellationToken;
        protected readonly object _syncRoot = new object();

        private readonly Uri _address;
        private readonly string _filePath;

        public abstract event Action<EventArgs> Completed;
        public abstract event Action<DownloadProgressChangedEventArgs> ProgressChanged;

        protected BaseDownloadCommand(Uri address, string filePath)
        {
            _cancellationToken = new CancellationTokenSource();
            _address = address ?? throw new ArgumentNullException(nameof(address));
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public abstract bool IsValid(BaseFilter filter);

        public abstract void Accept(BaseVisitor visitor);

        protected abstract void FileProgressChanged(object sender, DownloadProgressChangedEventArgs e);

        protected abstract void FileDownloaded(object sender, AsyncCompletedEventArgs e);

        public async Task Execute()
        {
            try
            {
                Task downloading = null;
                lock (_syncRoot)
                {
                    if (_cancellationToken.IsCancellationRequested == false)
                    {
                        _webClient = _webClient.IfNull(() => new WebClient());
                        _webClient.DownloadFileCompleted += FileDownloaded;
                        //_webClient.DownloadProgressChanged += FileProgressChanged;
                        downloading = _webClient.DownloadFileTaskAsync(_address, _filePath);
                    }
                }
                await downloading.IfNotNull(async () => await downloading, Task.FromResult(default(object)));
                lock (_syncRoot)
                    if (_cancellationToken.IsCancellationRequested && File.Exists(_filePath))
                        File.Delete(_filePath);
            }
            catch
            {
                File.Delete(_filePath);
                throw;
            }
            finally
            {
                lock (_syncRoot)
                {
                    _webClient.Dispose();
                    _webClient = null;
                }
            }
        }

        public void Undo()
        {
            lock (_syncRoot)
            {
                _cancellationToken.Cancel();
                _webClient.IfNotNull(() => _webClient.CancelAsync());
            }
        }

        ~BaseDownloadCommand()
        {
            _cancellationToken.Dispose();
        }
    }
}
