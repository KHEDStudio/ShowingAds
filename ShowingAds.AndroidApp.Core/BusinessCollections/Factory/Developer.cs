using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Factory
{
    public abstract class Developer<T> where T : Component
    {
        public abstract TopLevelCollection<T> Create();
    }
}
