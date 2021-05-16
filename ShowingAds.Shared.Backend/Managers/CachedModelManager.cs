using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.Shared.Backend.Managers
{
    public abstract class CachedModelManager<TKey, TModel, TSingleton> : Singleton<TSingleton>, IManager<TKey, TModel>
        where TKey : struct
        where TModel : class, IModel<TKey>, ICloneable
        where TSingleton : Singleton<TSingleton>
    {
        protected AutoResetEvent _mutex { get; }
        protected IDataProvider<TKey, TModel> _provider { get; set; }
        protected Dictionary<TKey, TModel> _models { get; set; }

        protected CachedModelManager(IDataProvider<TKey, TModel> provider)
        {
            _mutex = new AutoResetEvent(true);
            _provider = provider;
            _models = new Dictionary<TKey, TModel>();
        }

        protected async virtual Task UpdateOrInitializeModelsAsync()
        {
            try
            {
                _mutex.WaitOne();
                var models = await _provider.GetModelsOrNullAsync();
                _models.Clear();
                foreach (var model in models)
                    _models.Add(model.GetKey(), model);
            }
            catch
            {
                throw;
            }
            finally
            {
                _mutex.Set();
            }
        }

        public async virtual Task<bool> TryAddOrUpdateAsync(TKey key, TModel newValue)
        {
            try
            {
                _mutex.WaitOne();
                newValue = (TModel)newValue.Clone();
                if (_models.ContainsKey(key))
                {
                    var response = await _provider.PutModelAsync(newValue);
                    if (response.IsSuccessStatusCode)
                    {
                        _models.Remove(key);
                        _models.Add(key, newValue);
                        return true;
                    }
                    else return false;
                }
                else
                {
                    var response = await _provider.PostModelAsync(newValue);
                    if (response.IsSuccessStatusCode)
                    {
                        _models.Add(key, newValue);
                        return true;
                    }
                    else return false;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                _mutex.Set();
            }
        }

        private async Task<bool> TryDeleteRecord(KeyValuePair<TKey, TModel> record)
        {
            if (record.Equals(default(KeyValuePair<TKey, TModel>)))
                return true;
            var response = await _provider.DeleteModelAsync(record.Key);
            if (response.IsSuccessStatusCode)
            {
                _models.Remove(record.Key);
                return true;
            }
            else return false;
        }

        public async virtual Task<bool> TryDeleteAsync(TKey key, TModel model)
        {
            try
            {
                _mutex.WaitOne();
                var _model = _models.AsParallel().FirstOrDefault(x => x.Key.Equals(key));
                return await TryDeleteRecord(_model);
            }
            catch
            {
                throw;
            }
            finally
            {
                _mutex.Set();
            }
        }

        public async virtual Task<IEnumerable<TModel>> TryDeleteAsync(Func<TModel, bool> filter)
        {
            try
            {
                _mutex.WaitOne();
                var models = _models.AsParallel().Where(x => filter(x.Value));
                foreach (var model in models)
                    await TryDeleteRecord(model);
                return models.Select(x => x.Value);
            }
            catch
            {
                throw;
            }
            finally
            {
                _mutex.Set();
            }
        }

        public async Task<TModel> GetOrDefaultAsync(TKey key)
        {
            try
            {
                await Task.Yield();
                _mutex.WaitOne();
                var isExists = _models.TryGetValue(key, out var value);
                return value?.Clone() as TModel;
            }
            catch
            {
                throw;
            }
            finally
            {
                _mutex.Set();
            }
        }

        public async Task<TModel> GetOrDefaultAsync(Func<TModel, bool> filter)
        {
            try
            {
                await Task.Yield();
                _mutex.WaitOne();
                var model = _models.AsParallel().FirstOrDefault(x => filter(x.Value));
                return model.Value?.Clone() as TModel;
            }
            catch
            {
                throw;
            }
            finally
            {
                _mutex.Set();
            }
        }

        public async Task<IEnumerable<TModel>> GetCollectionOrNullAsync(Func<TModel, bool> filter)
        {
            try
            {
                await Task.Yield();
                _mutex.WaitOne();
                return _models.AsParallel().Where(x => filter(x.Value))
                    .Select(x => (TModel)x.Value.Clone());
            }
            catch
            {
                throw;
            }
            finally
            {
                _mutex.Set();
            }
        }
    }
}
