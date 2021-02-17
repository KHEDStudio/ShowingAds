using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.Parsers.Interfaces
{
    public interface IParser
    {
        void Parse(string json);
    }
}
