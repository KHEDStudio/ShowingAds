using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using ShowingAds.CoreLibrary.Abstract;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.Login;
using ShowingAds.CoreLibrary.Models.States;
using ShowingAds.DevicesService.BusinessLayer.Interfaces;
using System;
using System.Threading.Tasks;

namespace ShowingAds.DevicesService.BusinessLayer
{
    public sealed class Loginer : Singleton<Loginer>, ILoginer
    {
        private DeviceManager _manager => DeviceManager.GetInstance();
        private IDataProvider<int, LoginUser> _provider { get; }

        private Loginer() => _provider = new WebProvider<int, LoginUser>(Settings.SessionPath);

        public async Task<bool> TryLogin(LoginDevice login)
        {
            var loginResult = await _provider.PostModel(login);
            if (loginResult.Item1)
            {
                (var isExists, var device) = await _manager.TryGet(login.UUID);
                if (isExists == false)
                {
                    var user = JsonConvert.DeserializeObject<User>(loginResult.Item2);
                    device = new DeviceState(login, user.Id);
                    return await _manager.TryAddOrUpdate(device.Id, device);
                } else return true;
            }
            return false;
        }
    }
}
