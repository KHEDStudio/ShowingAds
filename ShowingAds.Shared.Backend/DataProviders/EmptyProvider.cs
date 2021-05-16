using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.Shared.Backend.DataProviders
{
    public class EmptyProvider<TKey, TModel> : IDataProvider<TKey, TModel>
        where TKey : struct
        where TModel : class
    {
        public Task<HttpResponseMessage> DeleteModelAsync(TKey modelId)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }

        public Task<IEnumerable<TModel>> GetModelsOrNullAsync()
        {
            return Task.FromResult(new List<TModel>() as IEnumerable<TModel>);
        }

        public Task<HttpResponseMessage> PostModelAsync(TModel model)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }

        public Task<HttpResponseMessage> PutModelAsync(TModel model)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
