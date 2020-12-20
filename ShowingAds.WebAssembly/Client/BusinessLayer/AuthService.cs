using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.Login;
using ShowingAds.WebAssembly.Client.BusinessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Client.BusinessLayer
{
    public class AuthService : IAuthService
    {
        private HttpClient _httpClient { get; }
        private AuthenticationStateProvider _authenticationStateProvider { get; }
        private ILocalStorageService _localStorage { get; }

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<(bool, User)> Login(LoginUser login)
        {
            var json = JsonConvert.SerializeObject(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("session/", content);
            json = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var user = JsonConvert.DeserializeObject<User>(json);
                ((AuthenticationUserProvider)_authenticationStateProvider).MarkUserAsAuthenticated(user.Name, user.Role);
                return (true, user);
            }
            return (false, default);
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("cookie");
            ((AuthenticationUserProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
