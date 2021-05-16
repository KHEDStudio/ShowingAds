using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShowingAds.Shared.Backend.DataProviders
{
    public interface IDataProvider<TKey, TModel>
        where TKey : struct
        where TModel : class
    {
        public Task<IEnumerable<TModel>> GetModelsOrNullAsync();
        public Task<HttpResponseMessage> PostModelAsync(TModel model);
        public Task<HttpResponseMessage> PutModelAsync(TModel model);
        public Task<HttpResponseMessage> DeleteModelAsync(TKey modelId);
    }
}
