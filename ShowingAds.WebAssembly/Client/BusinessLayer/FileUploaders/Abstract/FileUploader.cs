using Newtonsoft.Json;
using Polly;
using ShowingAds.CoreLibrary.Models.FilesService;
using ShowingAds.WebAssembly.Client.BusinessLayer.FileUploaders.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;

namespace ShowingAds.WebAssembly.Client.BusinessLayer.FileUploaders.Abstract
{
    public abstract class FileUploader : IFileUploader
    {
        public event Action<Exception> UploadFailed;

        protected abstract Uri _uploadUri { get; }

        public async Task<(bool, FileUploadResponse)> TryUploadAsync(IFileReference file, int retryAttempts)
        {
            return await Policy<(bool, FileUploadResponse)>
                .Handle<Exception>()
                .FallbackAsync((false, default))
                .WrapAsync(GetWrapPolicy(retryAttempts))
                .ExecuteAsync(async () =>
                {
                    await using (var fileStream = await file.OpenReadAsync())
                    {
                        var fileInfo = await file.ReadFileInfoAsync();
                        var fileResponse = await StartUploadAsync(fileStream, fileInfo);
                        if (fileResponse != null)
                            return (true, fileResponse);
                        return (false, default);
                    }
                });
        }

        private IAsyncPolicy GetWrapPolicy(int retryAttempts)
        {
            return Policy
                .Handle<Exception>()
                .RetryAsync(retryAttempts, async (e, i) =>
                {
                    UploadFailed?.Invoke(e);
                    await Task.Delay(TimeSpan.FromSeconds(i));
                });
        }

        private async Task<FileUploadResponse> StartUploadAsync(Stream stream, IFileInfo fileInfo)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(stream), "file", fileInfo.Name);
                var response = await httpClient.PostAsync(_uploadUri, content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var fileResponse = JsonConvert.DeserializeObject<FileUploadResponse>(json);
                    return fileResponse;
                }
                return null;
            }
        }
    }
}
