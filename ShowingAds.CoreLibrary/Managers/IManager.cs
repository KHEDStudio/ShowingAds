using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowingAds.CoreLibrary.Managers
{
    public interface IManager<T, U>
        where T : struct
        where U : class
    {
        public Task<(bool, U)> TryGet(T key);
        public Task<IEnumerable<U>> GetCollection(Func<U, bool> filter);
        public Task<(bool, U)> TryGet(Func<U, bool> filter);
        public Task<bool> TryAddOrUpdate(T key, U newValue);
        public Task<(bool, U)> TryDelete(T key);
        public Task<(bool, U)> TryDelete(Func<U, bool> filter);
    }
}
