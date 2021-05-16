using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using ShowingAds.CoreLibrary.Abstract;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.Login;
using ShowingAds.CoreLibrary.Models.States;
using ShowingAds.WebAssembly.Server;
using ShowingAds.WebAssembly.Server.BusinessLayer.Interfaces;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers;
using System;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server.BusinessLayer
{
    public sealed class Loginer : Singleton<Loginer>, ILoginer
    {
        private IDataProvider<int, LoginUser> _provider { get; }

        private Loginer() => _provider = new WebProvider<int, LoginUser>(Settings.SessionPath);

        public async Task<(bool, string)> TryLogin(LoginUser login)
        {
            var loginResult = await _provider.PostModelAsync(login);
            return loginResult;
        }
    }
}
