using ShowingAds.AndroidApp.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators
{
    public class RandomEnumerator<T> : LinearEnumerator<T>
    {
        public RandomEnumerator(IEnumerable<T> models) : base(models) { }

        public new bool MoveNext()
        {
            if (_nodes.Count == 0)
                return false;
            _node = _nodes[(0, _nodes.Count).RandomNumber()];
            return true;
        }
    }
}
