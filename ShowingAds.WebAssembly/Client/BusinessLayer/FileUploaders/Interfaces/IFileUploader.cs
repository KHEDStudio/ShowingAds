using ShowingAds.CoreLibrary.Models.FilesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;

namespace ShowingAds.WebAssembly.Client.BusinessLayer.FileUploaders.Interfaces
{
    public interface IFileUploader
    {
        public Task<(bool, FileUploadResponse)> TryUploadAsync(IFileReference file, int retryAttempts);
    }
}
