using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using System;

namespace ShowingAds.CoreLibrary.Models.FilesService
{
    public class FileUploadResponse
    {
        [JsonProperty("file"), JsonConverter(typeof(GuidConverter))]
        public Guid FileGuid { get; private set; }
        [JsonProperty("duration"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Duration { get; private set; }

        [JsonConstructor]
        public FileUploadResponse(Guid file, long duration)
        {
            FileGuid = file;
            Duration = TimeSpan.FromTicks(duration);
        }

        public FileUploadResponse(Guid file)
        {
            FileGuid = file;
            Duration = TimeSpan.Zero;
        }
    }
}
