using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Extensions
{
    public static class NullableExtensions
    {
        public static void IfNull<T>(this T obj, Action action)
        {
            if (obj == null)
                action();
        }
        public static void IfNotNull<T>(this T obj, Action action)
        {
            if (obj != null)
                action();
        }
        public static TRes IfNull<T, TRes>(this T obj, Func<TRes> func, TRes defResult = default)
        {
            if (obj == null)
                return func();
            return defResult;
        }
        public static TRes IfNotNull<T, TRes>(this T obj, Func<TRes> func, TRes defResult = default)
        {
            if (obj != null)
                return func();
            return defResult;
        }
    }
}
