using ShowingAds.AndroidApp.Core.Network.Enums;
using ShowingAds.AndroidApp.Core.Network.Parsers.Interfaces;
using ShowingAds.CoreLibrary.Models.Login;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Network.Interfaces
{
    public interface ILoginer
    {
        Task<LoginStatus> TryLoginAsync(LoginDevice login);
        IClient GetClient(IParser parser, TimeSpan reconnetInterval);
    }
}
