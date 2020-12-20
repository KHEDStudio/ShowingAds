using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.CoreLibrary.DataProviders
{
    public class EmptyProvider<T, U> : IDataProvider<T, U>
        where T : struct
        where U : class
    {
        public Task<bool> DeleteModel(T modelId)
        {
            return Task.FromResult(true);
        }

        public Task<IEnumerable<U>> GetModels()
        {
            return Task.FromResult((IEnumerable<U>)new List<U>());
        }

        public Task<(bool, string)> PostModel(U model)
        {
            return Task.FromResult((true, string.Empty));
        }

        public Task<(bool, string)> PutModel(U model)
        {
            return Task.FromResult((true, string.Empty));
        }
    }
}
