using ShowingAds.WebAssembly.Client.BusinessLayer.FileUploaders.Abstract;
using ShowingAds.WebAssembly.Client.BusinessLayer.FileUploaders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;

namespace ShowingAds.WebAssembly.Client.BusinessLayer.FileUploaders
{
    public class LogoUploader : FileUploader
    {
        protected override Uri _uploadUri => Settings.LogoPath;
    }
}
