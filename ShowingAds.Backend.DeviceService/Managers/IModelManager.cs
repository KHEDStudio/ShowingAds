using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Managers
{
    public interface IModelManager<TKey, TModel>
        where TKey : struct
        where TModel : class, ICloneable
    {
        public Task<IEnumerable<TModel>> GetPermittedModelsAsync(IEnumerable<int> users);
    }
}
