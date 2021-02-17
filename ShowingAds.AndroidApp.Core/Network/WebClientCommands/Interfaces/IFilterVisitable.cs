using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands.Interfaces
{
    public interface IFilterVisitable
    {
        bool IsValid(BaseFilter filter);
    }
}
