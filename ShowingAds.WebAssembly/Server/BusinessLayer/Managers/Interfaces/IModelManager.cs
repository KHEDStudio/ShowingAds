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
        public Task<IEnumerable<TModel>> GetPermittedModels(List<int> users);
        public Task<bool> TryAddOrUpdate(TModel model);
        public Task<(bool, TModel)> TryDelete(TKey id);
    }
}
