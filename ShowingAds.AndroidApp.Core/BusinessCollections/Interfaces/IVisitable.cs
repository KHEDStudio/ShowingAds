using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Interfaces
{
    public interface IVisitable
    {
        void Accept(BaseVisitor visitor);
    }
}
