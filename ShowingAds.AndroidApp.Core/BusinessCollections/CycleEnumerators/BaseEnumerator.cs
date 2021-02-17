using ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators.Models;
using ShowingAds.AndroidApp.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators
{
    public abstract class BaseEnumerator<T> : IEnumerator
    {
        protected Node<T> _node;
        protected List<Node<T>> _nodes = new List<Node<T>>();

        public object Current => _node.IfNotNull(() => _node.Current, default);

        protected BaseEnumerator(IEnumerable<T> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));
            foreach (var model in models)
                AddNode(model);
        }

        public abstract bool MoveNext();
        public abstract void Reset();

        public void AddNode(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            Node<T> node;
            if (_nodes.Count == 0)
            {
                node = new Node<T>(model, model);
            }
            else
            {
                var nextNode = _nodes[0];
                var previousNode = _nodes[_nodes.Count - 1];
                previousNode.SetNext(model);
                node = new Node<T>(nextNode.Current, model);
            }
            _nodes.Add(node);
        }

        public void RemoveNode(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (_nodes.Count == 0)
                return;
            if (_nodes.Count == 1)
            {
                if (_nodes.Any((node) => node.Current.Equals(model)))
                {
                    _nodes.RemoveAt(0);
                    _node = null;
                }
            }
            if (_nodes.Count >= 2)
            {
                var currentNode = _nodes.Find((node) => node.Current.Equals(model));
                var previousNode = _nodes.Find((node) => node.Next.Equals(model));
                previousNode.SetNext(currentNode.Next);
                if (_node != null && currentNode.Current.Equals(_node.Current))
                    _node = previousNode;
                _nodes.Remove(currentNode);
            }
        }
    }
}
