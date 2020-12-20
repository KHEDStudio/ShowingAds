using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowingAds.CoreLibrary.DataProviders
{
    public interface IDataProvider<T, U>
        where T : struct
        where U : class
    {
        public Task<IEnumerable<U>> GetModels();
        public Task<(bool, string)> PostModel(U model);
        public Task<(bool, string)> PutModel(U model);
        public Task<bool> DeleteModel(T modelId);
    }
}
