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
    public abstract class ModelManager<TKey, TValue, TSingleton> : Singleton<TSingleton>, IManager<TKey, TValue>
        where TKey : struct
        where TValue : class, IModel<TKey>, ICloneable
        where TSingleton : Singleton<TSingleton>
    {
        protected AsyncLock _mutex { get; }
        protected IDataProvider<TKey, TValue> _provider { get; set; }
        protected Dictionary<TKey, TValue> _models { get; set; }
        protected Timer _syncTimer { get; set; }

        protected ModelManager(IDataProvider<TKey, TValue> provider)
        {
            _mutex = new AsyncLock();
            _provider = provider;
            _models = new Dictionary<TKey, TValue>();
            _syncTimer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);
            _syncTimer.Elapsed += UpdateOrInitializeModels;
            _syncTimer.AutoReset = false;
        }

        protected async virtual void UpdateOrInitializeModels(object sender, ElapsedEventArgs e)
        {
            using (await _mutex.LockAsync())
            {
                var models = await _provider.GetModels();
                _models.Clear();
                foreach (var model in models)
                    _models.Add(model.GetKey(), model);
            }
        }

        public async virtual Task<bool> TryAddOrUpdate(TKey key, TValue newValue)
        {
            using (await _mutex.LockAsync())
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

        public async virtual Task<(bool, TValue)> TryDelete(TKey key)
        {
            using (await _mutex.LockAsync())
            {
                var model = _models.AsParallel().FirstOrDefault(x => x.Key.Equals(key));
                return await TryDeleteModel(model);
            }
        }

        public async virtual Task<(bool, TValue)> TryDelete(Func<TValue, bool> filter)
        {
            using (await _mutex.LockAsync())
            {
                var model = _models.AsParallel().FirstOrDefault(x => filter(x.Value));
                return await TryDeleteModel(model);
            }
        }

        public async Task<(bool, TValue)> TryGet(TKey key)
        {
            using (await _mutex.LockAsync())
            {
                var isExists = _models.TryGetValue(key, out var value);
                return (isExists, (TValue)value?.Clone());
            }
        }

        public async Task<(bool, TValue)> TryGet(Func<TValue, bool> filter)
        {
            using (await _mutex.LockAsync())
            {
                var model = _models.AsParallel().FirstOrDefault(x => filter(x.Value));
                if (model.Equals(default(KeyValuePair<TKey, TValue>)))
                    return (false, default);
                else
                    return (true, (TValue)model.Value.Clone());
            }
        }

        public async Task<IEnumerable<TValue>> GetCollection(Func<TValue, bool> filter)
        {
            using (await _mutex.LockAsync())
            {
                return _models.AsParallel().Where(x => filter(x.Value))
                    .Select(x => (TValue)x.Value.Clone());
            }
        }
    }
}
