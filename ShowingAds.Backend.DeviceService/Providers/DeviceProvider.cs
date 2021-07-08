using Newtonsoft.Json;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Backend.Models.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Providers
{
    public class DeviceProvider : IDataProvider<Guid, Device>
    {
        public Uri BaseURI { get; private set; }

        public DeviceProvider(Uri baseURI)
        {
            BaseURI = baseURI ?? throw new ArgumentNullException(nameof(baseURI));
        }

        public async Task<HttpResponseMessage> DeleteModelAsync(Guid modelId)
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

        public async Task<IEnumerable<Device>> GetModelsOrNullAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var responseMessage = await httpClient.GetAsync(BaseURI);
                    if (responseMessage.IsSuccessStatusCode == false)
                        throw new HttpRequestException(nameof(BaseURI));
                    var json = await responseMessage.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<Device>>(json);
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> PostModelAsync(Device model)
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

        public async Task<HttpResponseMessage> PutModelAsync(Device model)
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
    }
}
