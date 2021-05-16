using Newtonsoft.Json;
using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Managers;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Core.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.AuthService.Managers
{
    public sealed class LoginManager : Singleton<LoginManager>
    {
        private WebProvider<int, LoginUser> _webProvider = new(Settings.SessionPath);

        public async Task<User> GetUserOrNullAsync(LoginUser user)
        {
            var response = await _webProvider.PostModelAsync(user);
            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<User>(userJson);
            }
            return null;
        }
    }
}
