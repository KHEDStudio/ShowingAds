using Newtonsoft.Json;
using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Enums;
using ShowingAds.Shared.Backend.Managers;
using ShowingAds.Shared.Backend.Models.NotifyService;
using ShowingAds.Shared.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Managers
{
    public abstract class NotifyModelManager<TKey, TModel, TManager> : NoCachedModelManager<TKey, TModel, TManager>
        where TKey : struct
        where TModel : class, IModel<TKey>, ICloneable
        where TManager : Singleton<TManager>
    {
        public event Action<TModel> ModelUpdated;
        public event Action<TModel> ModelDeleted;

        public NotifyModelManager(IDataProvider<TKey, TModel> provider) : base(provider) { }

        public async override Task<bool> TryAddOrUpdateAsync(TKey key, TModel newValue)
        {
            var isAdded = await base.TryAddOrUpdateAsync(key, newValue);
            if (isAdded)
            {
                ModelUpdated?.Invoke(newValue);
                await NotifySubscribersAsync(Operation.CreateOrUpdate, newValue);
            }
            return isAdded;
        }

        public async override Task<bool> TryDeleteAsync(TKey key, TModel model)
        {
            var isDeleted = await base.TryDeleteAsync(key, model);
            if (isDeleted)
            {
                ModelDeleted?.Invoke(model);
                await NotifySubscribersAsync(Operation.Delete, model);
            }
            return isDeleted;
        }

        public abstract Task<IEnumerable<Guid>> GetSubscribersAsync(TModel model);

        protected async Task NotifySubscribersAsync(Operation operation, TModel model)
        {
            var subscribers = await GetSubscribersAsync(model);
            await Settings.RabbitMq.Publish(
                new NotifyPacket(subscribers, operation, typeof(TModel).Name, JsonConvert.SerializeObject(model)));
        }
    }
}
