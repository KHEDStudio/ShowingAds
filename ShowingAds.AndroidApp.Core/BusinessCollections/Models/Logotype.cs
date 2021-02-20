using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.BusinessCollections.Interfaces;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Interfaces;
using ShowingAds.CoreLibrary.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Models
{
    public class Logotype : IFilterVisitable
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("left")]
        public bool IsLeft { get; private set; }
        [JsonProperty("path")]
        public string LogotypePath { get; private set; }

        [JsonConstructor]
        public Logotype(Guid id, bool isLeft, string logotypePath)
        {
            Id = id;
            IsLeft = isLeft;
            LogotypePath = logotypePath ?? throw new ArgumentNullException(nameof(logotypePath));
        }

        public bool IsValid(BaseFilter filter)
        {
            var isValid = filter.FilterLogotype(this);
            if (isValid == false)
                Task.Run(DeleteVideoFile);
            return isValid;
        }

        private void DeleteVideoFile()
        {
            try
            {
                File.Delete(LogotypePath);
            }
            catch (Exception ex)
            {
                ServerLog.Error("VideoFile", ex.Message);
            }
        }
    }
}
