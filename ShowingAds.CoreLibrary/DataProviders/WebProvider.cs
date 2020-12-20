using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.CoreLibrary.DataProviders
{
    public class WebProvider<T, U> : IDataProvider<T, U>
        where T : struct
        where U : class
    {
        private Uri _requestPath { get; }

        public WebProvider(Uri requestPath) => _requestPath = requestPath;

        public async Task<IEnumerable<U>> GetModels()
        {
            using (var httpClient = new HttpClient())
            {
                var responseMessage = await httpClient.GetAsync(_requestPath);
                if (responseMessage.IsSuccessStatusCode == false)
                    throw new HttpRequestException(nameof(_requestPath));
                var json = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<U>>(json);
            }
        }

        public async Task<(bool, string)> PostModel(U model)
        {
            using (var httpClient = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(model);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var responseMessage = await httpClient.PostAsync(_requestPath, httpContent);
                json = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.IsSuccessStatusCode)
                    return (true, json);
                return (false, json);
            }
        }

        public async Task<(bool, string)> PutModel(U model)
        {
            using (var httpClient = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(model);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var responseMessage = await httpClient.PutAsync(_requestPath, httpContent);
                json = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.IsSuccessStatusCode)
                    return (true, json);
                return (false, json);
            }
        }

        public async Task<bool> DeleteModel(T modelId)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Content = new StringContent($"{{\"id\": \"{modelId}\"}}", Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Delete,
                    RequestUri = _requestPath
                };
                var responseMessage = await httpClient.SendAsync(request);
                if (responseMessage.IsSuccessStatusCode)
                    return true;
                return false;
            }
        }
    }
}
