using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.DataAccess
{
    public class ConfigFileStore<T> : IDataStore<T>
    {
        private readonly string _path;
        private readonly AutoResetEvent _resetEvent;

        public ConfigFileStore(string path)
        {
            _resetEvent = new AutoResetEvent(true);
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public T Load()
        {
            try
            {
                _resetEvent.WaitOne();
                var json = File.ReadAllText(_path, Encoding.UTF8);
                _resetEvent.Set();
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                _resetEvent.Set();
                throw;
            }
        }

        public void Save(T obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                _resetEvent.WaitOne();
                File.WriteAllText(_path, json, Encoding.UTF8);
                _resetEvent.Set();
            }
            catch
            {
                _resetEvent.Set();
                throw;
            }
        }

        public void Dispose()
        {
            _resetEvent.Close();
        }
    }
}
