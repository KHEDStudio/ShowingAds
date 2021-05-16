using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.Shared.Backend.Abstract
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static readonly Lazy<T> _instance =
            new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);

        public static T GetInstance() => _instance.Value;
    }
}
