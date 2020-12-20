using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Client
{
    public class AuthenticationUserProvider : AuthenticationStateProvider
    {
        private HttpClient _httpClient { get; }
        private ILocalStorageService _localStorage { get; }

        public AuthenticationUserProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedCookie = await _localStorage.GetItemAsync<string>("cookie");
            if (string.IsNullOrEmpty(savedCookie))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Cookie", savedCookie);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity("ApplicationCookie")));
        }

        public void MarkUserAsAuthenticated(string username, UserRole role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            var authenticatedUser = new ClaimsPrincipal(claimsIdentity);
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
