using ShowingAds.AndroidApp.Core.BusinessCollections.Visitors;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Network.Interfaces
{
    public interface IExecutor<T> : IDisposable
    {
        event Action<T> CommandExecuted;
        event Action<DownloadProgressChangedEventArgs> ProgressChanged;

        void AddCommandToQueue(IWebClientCommand command);
        Task<bool> TryExecuteCommandAsync();
        void Filter(BaseFilter filter);
        void Accept(BaseVisitor visitor);
    }
}
