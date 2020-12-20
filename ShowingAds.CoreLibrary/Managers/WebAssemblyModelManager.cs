using Nito.AsyncEx;
using ShowingAds.CoreLibrary.Abstract;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.CoreLibrary.Managers
{
    public abstract class WebAssemblyModelManager<TKey, TValue, TSingleton> : Singleton<TSingleton>, IManager<TKey, TValue>
        where TKey : struct
        where TValue : class, IModel<TKey>, ICloneable
        where TSingleton : Singleton<TSingleton>
    {
        protected IDataProvider<TKey, TValue> _provider { get; set; }
        protected Dictionary<TKey, TValue> _models { get; set; }
        protected Timer _syncTimer { get; set; }

        protected WebAssemblyModelManager(IDataProvider<TKey, TValue> provider)
        {
            _provider = provider;
            _models = new Dictionary<TKey, TValue>();
            _syncTimer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);
            _syncTimer.Elapsed += async (s, e) => await UpdateOrInitializeModels();
            _syncTimer.AutoReset = false;
        }

        public async virtual Task UpdateOrInitializeModels()
        {
            var models = await _provider.GetModels();
            _models.Clear();
            foreach (var model in models)
                _models.Add(model.GetKey(), model);
        }

        public async Task<bool> TryAddOrUpdate(TKey key, TValue newValue)
        {
            newValue = (TValue)newValue.Clone();
            if (_models.ContainsKey(key))
            {
                var updateResult = await _provider.PutModel(newValue);
                if (updateResult.Item1)
                {
                    _models.Remove(key);
                    _models.Add(key, newValue);
                    return true;
                }
                else return false;
            }
            else
            {
                var createResult = await _provider.PostModel(newValue);
                if (createResult.Item1)
                {
                    _models.Add(key, newValue);
                    return true;
                }
                else return false;
            }
        }

        private async Task<(bool, TValue)> TryDeleteModel(KeyValuePair<TKey, TValue> model)
        {
            if (model.Equals(default(KeyValuePair<TKey, TValue>)))
                return (true, default);
            var deleteResult = await _provider.DeleteModel(model.Key);
            if (deleteResult)
            {
                _models.Remove(model.Key);
                return (true, model.Value);
            }
            else return (false, model.Value);
        }

        public async Task<(bool, TValue)> TryDelete(TKey key)
        {
            var model = _models.FirstOrDefault(x => x.Key.Equals(key));
            return await TryDeleteModel(model);
        }

        public async Task<(bool, TValue)> TryDelete(Func<TValue, bool> filter)
        {
            var model = _models.AsParallel().FirstOrDefault(x => filter(x.Value));
            return await TryDeleteModel(model);
        }

        public Task<(bool, TValue)> TryGet(TKey key)
        {
            var isExists = _models.TryGetValue(key, out var value);
            return Task.FromResult((isExists, (TValue)value.Clone()));
        }

        public Task<(bool, TValue)> TryGet(Func<TValue, bool> filter)
        {
            var model = _models.AsParallel().FirstOrDefault(x => filter(x.Value));
            if (model.Equals(default(KeyValuePair<TKey, TValue>)))
                return Task.FromResult((false, (TValue)default));
            else
                return Task.FromResult((true, (TValue)model.Value.Clone()));
        }

        public Task<IEnumerable<TValue>> GetCollection(Func<TValue, bool> filter)
        {
            return Task.FromResult(_models.Where(x => filter(x.Value))
                .Select(x => (TValue)x.Value.Clone()));
        }
    }
}
