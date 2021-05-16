using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowingAds.Shared.Backend.Managers
{
    public interface IManager<TKey, TModel>
        where TKey : struct
        where TModel : class
    {
        public Task<TModel> GetOrDefaultAsync(TKey key);
        public Task<TModel> GetOrDefaultAsync(Func<TModel, bool> filter);
        public Task<IEnumerable<TModel>> GetCollectionOrNullAsync(Func<TModel, bool> filter);
        public Task<bool> TryAddOrUpdateAsync(TKey key, TModel newValue);
        public Task<bool> TryDeleteAsync(TKey key, TModel model);
    }
}
