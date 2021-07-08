using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using ShowingAds.Shared.Backend.Abstract;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Managers;
using ShowingAds.Shared.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Managers
{
    public abstract class MongoCachedModelManager<TKey, TModel, TSingleton> : Singleton<TSingleton>, IManager<TKey, TModel>
        where TKey : struct
        where TModel : class, IModel<TKey>, ICloneable
        where TSingleton : Singleton<TSingleton>
    {
        protected IDataProvider<TKey, TModel> _provider;
        private readonly IMongoCollection<BsonDocument> _collection;

        protected MongoCachedModelManager(IDataProvider<TKey, TModel> provider, IMongoCollection<BsonDocument> collection)
        {
            _provider = provider;
            _collection = collection;
        }

        protected async virtual Task UpdateOrInitializeModelsAsync()
        {
            var models = await _provider.GetModelsOrNullAsync();
            await _collection.DeleteManyAsync(new BsonDocument());
            foreach (var model in models)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("key", model.GetKey().ToString());
                var update = Builders<BsonDocument>.Update.Set(typeof(TModel).Name, JsonConvert.SerializeObject(model));
                await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
            }
        }

        public async virtual Task<bool> TryAddOrUpdateAsync(TKey key, TModel newValue)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("key", key.ToString());
            var update = Builders<BsonDocument>.Update.Set(typeof(TModel).Name, JsonConvert.SerializeObject(newValue));
            using var cursor = await _collection.FindAsync(filter);
            await cursor.MoveNextAsync();
            if (cursor.Current.Any())
            {
                var response = await _provider.PutModelAsync(newValue);
                if (response.IsSuccessStatusCode)
                {
                    await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
                    return true;
                }
                else return false;
            }
            else
            {
                var response = await _provider.PostModelAsync(newValue);
                if (response.IsSuccessStatusCode)
                {
                    await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
                    return true;
                }
                else return false;
            }
        }

        public async Task<bool> TryAddOrUpdateWithoutProviderAsync(TKey key, TModel newValue)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("key", key.ToString());
            var update = Builders<BsonDocument>.Update.Set(typeof(TModel).Name, JsonConvert.SerializeObject(newValue));
            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
            return true;
        }

        private async Task<bool> TryDeleteRecord(KeyValuePair<TKey, TModel> record)
        {
            if (record.Equals(default(KeyValuePair<TKey, TModel>)))
                return true;
            var response = await _provider.DeleteModelAsync(record.Key);
            if (response.IsSuccessStatusCode)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("key", record.Key.ToString());
                await _collection.DeleteManyAsync(filter);
                return true;
            }
            else return false;
        }

        public async virtual Task<bool> TryDeleteAsync(TKey key, TModel model) => await TryDeleteRecord(new KeyValuePair<TKey, TModel>(key, model));

        public async Task<bool> TryDeleteWithoutProviderAsync(TKey key, TModel model) => await TryDeleteRecord(new KeyValuePair<TKey, TModel>(key, model));

        public async virtual Task<IEnumerable<TModel>> TryDeleteAsync(Func<TModel, bool> filter) => throw new NotImplementedException();

        public async Task<TModel> GetOrDefaultAsync(TKey key)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("key", key.ToString());
            using var cursor = await _collection.FindAsync(filter);
            await cursor.MoveNextAsync();
            if (cursor.Current.Any())
            {
                var element = cursor.Current.ElementAt(0);
                if (element.Contains(typeof(TModel).Name))
                    return JsonConvert.DeserializeObject<TModel>(element[typeof(TModel).Name].AsString);
            }
            return default;
        }

        public async Task<TModel> GetOrDefaultAsync(Func<TModel, bool> filter)
        {
            await Task.Yield();
            foreach (var item in _collection.AsQueryable())
            {
                if (item.Contains(typeof(TModel).Name))
                {
                    var json = item[typeof(TModel).Name];
                    var model = JsonConvert.DeserializeObject<TModel>(json.AsString);
                    if (filter(model))
                        return model;
                }
            }
            return default;
        }

        public async Task<IEnumerable<TModel>> GetCollectionOrNullAsync(Func<TModel, bool> filter)
        {
            await Task.Yield();
            var result = new List<TModel>();
            foreach (var item in _collection.AsQueryable())
            {

                if (item.Contains(typeof(TModel).Name))
                {
                    var json = item[typeof(TModel).Name];
                    var model = JsonConvert.DeserializeObject<TModel>(json.AsString);
                    if (filter(model))
                        result.Add(model);
                }
            }
            return result;
        }
    }
}
