using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.Network.Enums;
using ShowingAds.AndroidApp.Core.Network.Interfaces;
using ShowingAds.AndroidApp.Core.Network.Parsers.Interfaces;
using ShowingAds.CoreLibrary.Models.Login;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Network
{
    public class NetworkLoginer : ILoginer
    {
        private CookieContainer _cookieContainer = new CookieContainer();

        public IClient GetClient(IParser parser, TimeSpan reconnetInterval) => new NetworkClient(parser, _cookieContainer, reconnetInterval);

        public async Task<LoginStatus> TryLoginAsync(LoginDevice login)
        {
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    handler.CookieContainer = _cookieContainer;
                    using (var client = new HttpClient(handler))
                    {
                        var responseMessage = await SendLoginDataAsync(client, login);
                        if (responseMessage.StatusCode == HttpStatusCode.OK)
                            return LoginStatus.SuccessLogin;
                        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                            return LoginStatus.NotCorrectLoginData;
                        return LoginStatus.RequestError;
                    }
                }
            }
            catch (Exception ex)
            {
                await ServerLog.Error("NetworkLogin", ex.Message);
            }

            return LoginStatus.RequestError;
        }

        private async Task<HttpResponseMessage> SendLoginDataAsync(HttpClient client, LoginDevice login)
        {
            var jsonLoginData = JsonConvert.SerializeObject(login);
            var loginDataContent = new StringContent(jsonLoginData, Encoding.UTF8, "application/json");
            return await client.PostAsync(Settings.LoginPath, loginDataContent);
        }
    }
}
