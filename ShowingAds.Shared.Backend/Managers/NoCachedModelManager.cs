using Newtonsoft.Json;
using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Shared.Backend.Managers
{
    public abstract class NoCachedModelManager<TKey, TModel, TSingleton> : Singleton<TSingleton>, IManager<TKey, TModel>
        where TKey : struct
        where TModel : class, IModel<TKey>, ICloneable
        where TSingleton : Singleton<TSingleton>
    {
        private readonly IDataProvider<TKey, TModel> _provider;

        public NoCachedModelManager(IDataProvider<TKey, TModel> provider) => _provider = provider;

        public async virtual Task<bool> TryAddOrUpdateAsync(TKey key, TModel newValue)
        {
            var response = await _provider.PutModelAsync(newValue);
            if (response.IsSuccessStatusCode == false)
                response = await _provider.PostModelAsync(newValue);
            return response.IsSuccessStatusCode;
        }

        public async virtual Task<bool> TryDeleteAsync(TKey key, TModel model)
        {
            var response = await _provider.DeleteModelAsync(key);
            return response.IsSuccessStatusCode;
        }

        public async Task<TModel> GetOrDefaultAsync(TKey key)
        {
            var models = await _provider.GetModelsOrNullAsync();
            if (models != null)
            {
                var model = models.AsParallel().FirstOrDefault(x => x.GetKey().Equals(key));
                return model;
            }
            return default;
        }

        public async Task<TModel> GetOrDefaultAsync(Func<TModel, bool> filter)
        {
            var models = await _provider.GetModelsOrNullAsync();
            if (models != null)
            {
                var model = models.AsParallel().FirstOrDefault(x => filter(x));
                return model;
            }
            return default;
        }

        public async Task<IEnumerable<TModel>> GetCollectionOrNullAsync(Func<TModel, bool> filter)
        {
            var models = await _provider.GetModelsOrNullAsync();
            if (models != null)
                return models.AsParallel().Where(x => filter(x));
            return null;
        }
    }
}
