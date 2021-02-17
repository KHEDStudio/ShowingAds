using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Extensions
{
    public static class BooleanExtensions
    {
        public static void IfTrue(this bool obj, Action action)
        {
            if (obj)
                action();
        }
        public static void IfFalse(this bool obj, Action action)
        {
            if (obj == false)
                action();
        }
    }
}
