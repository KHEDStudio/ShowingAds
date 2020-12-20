using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Interfaces
{
    public interface IModel<TKey> where TKey : struct
    {
        public TKey GetKey();
    }
}
