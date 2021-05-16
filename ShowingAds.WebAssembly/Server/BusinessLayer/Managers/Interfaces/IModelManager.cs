using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Interfaces
{
    public interface IModelManager<TKey, TModel>
        where TKey : struct
        where TModel : class, ICloneable
    {
        public Task<IEnumerable<TModel>> GetPermittedModelsAsync(IEnumerable<int> users);
        public Task<bool> TryAddOrUpdateAsync(TModel model);
        public Task<(bool, TModel)> TryDeleteAsync(TKey id);
    }
}
