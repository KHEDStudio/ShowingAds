using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators
{
    public class LinearEnumerator<T> : BaseEnumerator<T>
    {
        public LinearEnumerator(IEnumerable<T> collection) : base(collection) { }

        public override bool MoveNext()
        {
            if (_nodes.Count == 0)
                return false;
            if (_node == null)
                Reset();
            _node = _nodes.Find(node => node.Current.Equals(_node.Next));
            return true;
        }

        public override void Reset()
        {
            if (_nodes.Count != 0)
                _node = _nodes[_nodes.Count - 1];
        }
    }
}
