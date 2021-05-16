using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.Shared.Backend.DataProviders
{
    public class WebProvider<TKey, TModel> : IDataProvider<TKey, TModel>
        where TKey : struct
        where TModel : class
    {
        public Uri BaseURI { get; private set; }

        public WebProvider(Uri baseURI) => BaseURI = baseURI;

        public async Task<IEnumerable<TModel>> GetModelsOrNullAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var responseMessage = await httpClient.GetAsync(BaseURI);
                    if (responseMessage.IsSuccessStatusCode == false)
                        throw new HttpRequestException(nameof(BaseURI));
                    var json = await responseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<TModel>>(json);
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PostModelAsync(TModel model)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    return await httpClient.PostAsync(BaseURI, httpContent);
                }
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<HttpResponseMessage> PutModelAsync(TModel model)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    return await httpClient.PutAsync(BaseURI, httpContent);
                }
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<HttpResponseMessage> DeleteModelAsync(TKey modelId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Content = new StringContent($"{{\"id\": \"{modelId}\"}}", Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Delete,
                        RequestUri = BaseURI
                    };
                    return await httpClient.SendAsync(request);
                }
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
