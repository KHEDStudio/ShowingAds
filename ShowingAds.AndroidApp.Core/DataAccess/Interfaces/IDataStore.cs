using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.DataAccess.Interfaces
{
    public interface IDataStore<T> : IDisposable
    {
        Task Save(T obj);
        Task<T> Load();
    }
}
