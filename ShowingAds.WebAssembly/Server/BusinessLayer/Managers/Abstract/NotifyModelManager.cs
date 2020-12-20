using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ShowingAds.CoreLibrary;
using ShowingAds.CoreLibrary.Abstract;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Interfaces;
using ShowingAds.CoreLibrary.Managers;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Abstract
{
    public abstract class NotifyModelManager<TKey, TValue, TManager> : ModelManager<TKey, TValue, TManager>
        where TKey : struct
        where TValue : class, IModel<TKey>, ICloneable
        where TManager : Singleton<TManager>
    {
        protected ModelOwnerParser _parser { get; set; }

        protected NotifyModelManager(IDataProvider<TKey, TValue> provider) : base(provider) =>
            _parser = ModelOwnerParser.GetInstance();

        public async override Task<bool> TryAddOrUpdate(TKey key, TValue newValue)
        {
            var isSuccess = await base.TryAddOrUpdate(key, newValue);
            if (isSuccess)
            {
                var _ = StartNotifying(newValue);
            }
            return isSuccess;
        }

        public async override Task<(bool, TValue)> TryDelete(TKey key)
        {
            var (isSuccess, model) = await base.TryDelete(key);
            if (isSuccess)
            {
                var _ = StartNotifying(model);
            }
            return (isSuccess, model);
        }

        public async override Task<(bool, TValue)> TryDelete(Func<TValue, bool> filter)
        {
            var (isSuccess, model) = await base.TryDelete(filter);
            if (isSuccess)
            {
                var _ = StartNotifying(model);
            }
            return (isSuccess, model);
        }

        private async Task StartNotifying(TValue model)
        {
            var subscribers = await _parser.GetSubscribers(model);
            Notify(subscribers.ToList());
        }

        public async Task Notify(List<Guid> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                using (var httpClient = new HttpClient())
                {
                    var requestUri = new Uri(Settings.NotifyChannelPath, $"?client={subscriber}");
                    await httpClient.PostAsync(requestUri, default);
                }
            }
        }
    }
}
