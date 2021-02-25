using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.Interfaces
{
    public interface IExecutor<T> : IDisposable
    {
        event Action<T> CommandExecuted;
        event Action<ProgressChangedEventArgs> ProgressChanged;

        void AddCommandToQueue(IWebClientCommand command);
        bool TryExecuteCommand();
        void Filter(BaseFilter filter);
        void Accept(BaseVisitor visitor);
    }
}
