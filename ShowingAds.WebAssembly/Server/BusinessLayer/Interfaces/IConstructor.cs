using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Interfaces
{
    public interface IConstructor<TKey, TValue>
        where TKey : struct
        where TValue : class
    {
        public Task<TValue> Construct(TKey key);
        public Task<string> ConstructAsJson(TKey key);
        public Task<string> ConstructAsJson(TValue value);
    }
}
