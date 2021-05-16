using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShowingAds.CoreLibrary;
using ShowingAds.CoreLibrary.Abstract;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Interfaces;
using ShowingAds.CoreLibrary.Managers;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Abstract
{
    public abstract class NotifyModelManager<TKey, TValue, TManager> : NoSaveModelManager<TKey, TValue, TManager>
        where TKey : struct
        where TValue : class, IModel<TKey>, ICloneable
        where TManager : Singleton<TManager>
    {
        protected Uri _notifyPath { get; }

        protected NotifyModelManager(IDataProvider<TKey, TValue> provider, Uri notifyPath) : base(provider)
        {
            _notifyPath = notifyPath;
        }

        public async override Task<bool> TryAddOrUpdateAsync(TKey key, TValue newValue)
        {
            var isSuccess = await base.TryAddOrUpdateAsync(key, newValue);
            if (isSuccess)
                await NotifySubscribersAsync(newValue);
            return isSuccess;
        }

        public async override Task<(bool, TValue)> TryDeleteAsync(TKey key)
        {
            var (isSuccess, model) = await base.TryDeleteAsync(key);
            if (isSuccess)
                await NotifySubscribersAsync(model);
            return (isSuccess, model);
        }

        public async override Task<IEnumerable<TValue>> TryDeleteAsync(Func<TValue, bool> filter)
        {
            var models = await base.TryDeleteAsync(filter);
            foreach (var model in models)
                await NotifySubscribersAsync(model);
            return models;
        }

        public abstract Task<IEnumerable<Guid>> GetSubscribersAsync(TValue model);

        protected async Task NotifySubscribersAsync(TValue model)
        {
            var subscribers = await GetSubscribersAsync(model);
            foreach (var subscriber in subscribers)
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var requestUri = new Uri(_notifyPath, $"?client={subscriber}");
                    await httpClient.PostAsync(requestUri, content);
                }
            }
        }
    }
}
