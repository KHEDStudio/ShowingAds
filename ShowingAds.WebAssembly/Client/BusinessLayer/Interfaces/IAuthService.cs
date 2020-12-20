using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Client.BusinessLayer.Interfaces
{
    public interface IAuthService
    {
        public Task<(bool, User)> Login(LoginUser login);
        public Task Logout();
    }
}
