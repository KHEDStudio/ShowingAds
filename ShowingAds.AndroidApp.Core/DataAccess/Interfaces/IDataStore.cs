using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.DataAccess.Interfaces
{
    public interface IDataStore<T> : IDisposable
    {
        void Save(T obj);
        T Load();
    }
}
