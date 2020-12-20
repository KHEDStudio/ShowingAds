using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Interfaces
{
    public interface ILoginer
    {
        public Task<(bool, string)> TryLogin(LoginUser login);
    }
}
