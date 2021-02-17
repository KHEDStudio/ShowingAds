using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.CycleEnumerators.Models
{
    public class Node<T>
    {
        public T Next { get; private set; }
        public T Current { get; private set; }

        public Node(T next, T current)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
            Current = current ?? throw new ArgumentNullException(nameof(current));
        }

        public void SetNext(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            Next = model;
        }
    }
}
