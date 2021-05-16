using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.Shared.Core.Models
{
    public interface IModel<TKey> where TKey : struct
    {
        public TKey GetKey();
    }
}
