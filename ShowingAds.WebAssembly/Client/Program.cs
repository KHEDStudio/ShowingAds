using Blazored.LocalStorage;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShowingAds.WebAssembly.Client.BusinessLayer;
using ShowingAds.WebAssembly.Client.BusinessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;

namespace ShowingAds.WebAssembly.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBlazoredModal();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationUserProvider>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddFileReaderService();

            Settings.ServerPath = new Uri("http://192.168.1.66:63880/");
            Settings.NotifyPath = new Uri("http://192.168.1.66:3669/notify/");
            Settings.FileServicePath = new Uri("http://192.168.1.66:3666/");

            await builder.Build().RunAsync();
        }
    }
}
