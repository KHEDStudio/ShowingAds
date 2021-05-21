using Newtonsoft.Json;
using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Enums;
using ShowingAds.Shared.Backend.Managers;
using ShowingAds.Shared.Backend.Models.NotifyService;
using ShowingAds.Shared.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Managers
{
    public abstract class NotifyModelManager<TKey, TModel, TManager> : CachedModelManager<TKey, TModel, TManager>
        where TKey : struct
        where TModel : class, IModel<TKey>, ICloneable
        where TManager : Singleton<TManager>
    {
        public NotifyModelManager(IDataProvider<TKey, TModel> provider) : base(provider) { }

        public async new Task<bool> TryAddOrUpdateAsync(TKey key, TModel newValue)
        {
            var isAdded = await base.TryAddOrUpdateAsync(key, newValue);
            if (isAdded)
                await NotifySubscribersAsync(Operation.CreateOrUpdate, newValue);
            return isAdded;
        }

        public async new Task<bool> TryDeleteAsync(TKey key, TModel model)
        {
            var isDeleted = await base.TryDeleteAsync(key, model);
            if (isDeleted)
                await NotifySubscribersAsync(Operation.Delete, model);
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
