using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.Models
{
    public class DiagnosticInfo : ICloneable
    {
        public int ReadyVideosCount { get; private set; }

        public DiagnosticInfo(int readyVideosCount)
        {
            ReadyVideosCount = readyVideosCount;
        }

        public object Clone() => MemberwiseClone();
    }
}
