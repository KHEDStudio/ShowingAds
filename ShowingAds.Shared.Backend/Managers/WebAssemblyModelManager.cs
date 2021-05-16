using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.Shared.Backend.Managers
{
    public abstract class WebAssemblyModelManager<TKey, TModel, TSingleton> : Singleton<TSingleton>, IManager<TKey, TModel>
        where TKey : struct
        where TModel : class, IModel<TKey>, ICloneable
        where TSingleton : Singleton<TSingleton>
    {
        protected IDataProvider<TKey, TModel> _provider { get; set; }
        protected Dictionary<TKey, TModel> _models { get; set; }
        protected Timer _syncTimer { get; set; }

        protected WebAssemblyModelManager(IDataProvider<TKey, TModel> provider)
        {
            _provider = provider;
            _models = new Dictionary<TKey, TModel>();
            _syncTimer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);
            _syncTimer.Elapsed += async (s, e) => await UpdateOrInitializeModels();
            _syncTimer.AutoReset = false;
        }

        public void UpdateModel(TKey key, TModel newValue)
        {
            if (_models.ContainsKey(key))
            {
                newValue = (TModel)newValue.Clone();
                _models.Remove(key);
                _models.Add(key, newValue);
            }
        }

        public async virtual Task UpdateOrInitializeModels()
        {
            var models = await _provider.GetModelsOrNullAsync();
            _models.Clear();
            foreach (var model in models)
                _models.Add(model.GetKey(), model);
        }

        public async Task<bool> TryAddOrUpdateAsync(TKey key, TModel newValue)
        {
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

        private async Task<bool> TryDeleteModel(KeyValuePair<TKey, TModel> model)
        {
            if (model.Equals(default(KeyValuePair<TKey, TModel>)))
                return true;
            var response = await _provider.DeleteModelAsync(model.Key);
            if (response.IsSuccessStatusCode)
            {
                _models.Remove(model.Key);
                return true;
            }
            else return false;
        }

        public async Task<bool> TryDeleteAsync(TKey key, TModel model)
        {
            var _model = _models.FirstOrDefault(x => x.Key.Equals(key));
            return await TryDeleteModel(_model);
        }

        public Task<IEnumerable<TModel>> TryDeleteAsync(Func<TModel, bool> filter)
        {
            var models = _models.AsParallel().Where(x => filter(x.Value));
            models.ForAll(async x => await TryDeleteModel(x));
            return Task.FromResult(models.AsEnumerable().Select(x => x.Value));
        }

        public Task<TModel> GetOrDefaultAsync(TKey key)
        {
            _models.TryGetValue(key, out var value);
            return Task.FromResult(value?.Clone() as TModel);
        }

        public Task<TModel> GetOrDefaultAsync(Func<TModel, bool> filter)
        {
            var record = _models.AsParallel().FirstOrDefault(x => filter(x.Value));
            return Task.FromResult(record.Value?.Clone() as TModel);
        }

        public Task<IEnumerable<TModel>> GetCollectionOrNullAsync(Func<TModel, bool> filter)
        {
            return Task.FromResult(_models.Where(x => filter(x.Value))
                .Select(x => (TModel)x.Value.Clone()));
        }
    }
}
