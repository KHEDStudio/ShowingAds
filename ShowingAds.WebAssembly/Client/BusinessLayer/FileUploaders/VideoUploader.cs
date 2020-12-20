using ShowingAds.WebAssembly.Client.BusinessLayer.FileUploaders.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Client.BusinessLayer.FileUploaders
{
    public class VideoUploader : FileUploader
    {
        protected override Uri _uploadUri => Settings.VideoPath;
    }
}
