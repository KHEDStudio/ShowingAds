using ShowingAds.AndroidApp.Core.BusinessCollections.Interfaces;
using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections
{
    public abstract class Component : IFilterVisitable, IVisitable
    {
        public abstract void Add(Component component);

        public abstract void Remove(Component component);

        public abstract void Accept(BaseVisitor visitor);

        public abstract bool IsValid(BaseFilter filter);

        public abstract Guid GetId();
    }
}
