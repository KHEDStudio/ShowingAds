using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Extensions
{
    public static class NumberRangeExtensions
    {
        public static int RandomNumber(this (int, int) range)
        {
            if (range.Item2 < range.Item1)
                return new Random().Next(range.Item2, range.Item1);
            if (range.Item1 < range.Item2)
                return new Random().Next(range.Item1, range.Item2);
            return range.Item1;
        }
    }
}
