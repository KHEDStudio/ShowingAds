using Newtonsoft.Json;
using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Backend.Models.NotifyService;
using ShowingAds.Shared.Backend.Models.States;
using ShowingAds.Shared.Core.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Managers
{
    public sealed class Loginer : Singleton<Loginer>
    {
        private DeviceManager _manager => DeviceManager.GetInstance();
        private IDataProvider<int, LoginUser> _provider { get; }

        private Loginer() => _provider = new WebProvider<int, LoginUser>(Settings.SessionPath);

        public async Task<bool> TryLogin(LoginDevice login)
        {
            var response = await _provider.PostModelAsync(login);
            if (response.IsSuccessStatusCode)
            {
                var device = await _manager.GetOrDefaultAsync(login.UUID);
                if (device == default)
                {
                    var userJson = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(userJson);
                    device = new DeviceState(login, user.Id);
                    return await _manager.TryAddOrUpdateAsync(device.Id, device);
                }
                else return true;
            }
            return false;
        }
    }
}
