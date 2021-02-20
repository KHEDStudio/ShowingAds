using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShowingAds.AndroidApp.Core.Extensions;

namespace ShowingAds.AndroidApp.Core.Network
{
    public class WebDownloader
    {
        private CancellationTokenSource _tokenSource; 

        private byte[] _buffer = new byte[1024 * 1024];

        public event Action<ProgressChangedEventArgs> ProgressChanged;
        public event Action<AsyncCompletedEventArgs> DownloadFileCompleted;

        public WebDownloader() => _tokenSource = new CancellationTokenSource();

        public void DownloadFile(Uri address, string filePath)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = httpClient.GetAsync(address, HttpCompletionOption.ResponseHeadersRead).Result;
                        using (var downloadStream = httpClient.GetStreamAsync(address).Result)
                        {
                            long readBytes = 0;
                            long? totalBytes = response.Content.Headers.ContentLength;
                            while (downloadStream.ReadBytes(ref _buffer, 0) > 0)
                            {
                                readBytes += _buffer.LongLength;
                                fileStream.Write(_buffer, 0, _buffer.Length);
                                var progressPercentage = (int)(readBytes * 100 / totalBytes);
                                Task.Run(() => ProgressChanged?.Invoke(new ProgressChangedEventArgs(progressPercentage, default)));
                                if (_tokenSource.Token.IsCancellationRequested)
                                {
                                    Task.Run(() => DownloadFileCompleted?.Invoke(new AsyncCompletedEventArgs(default, _tokenSource.Token.IsCancellationRequested, default)));
                                    return;
                                }
                            }
                            Task.Run(() => DownloadFileCompleted?.Invoke(new AsyncCompletedEventArgs(default, _tokenSource.Token.IsCancellationRequested, default)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => DownloadFileCompleted?.Invoke(new AsyncCompletedEventArgs(ex, _tokenSource.Token.IsCancellationRequested, default)));
            }
        }

        public void Cancel() => _tokenSource.Cancel();

        ~WebDownloader()
        {
            _tokenSource.Dispose();
        }
    }
}
