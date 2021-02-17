using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Extensions
{
    public static class StringExtensions
    {
        public static string WhitePlus(this string first, string second) => $"{first} {second}";
        public static string WithTAG(this string str, string tag) => $"[{tag}] {str}";
    }
}
