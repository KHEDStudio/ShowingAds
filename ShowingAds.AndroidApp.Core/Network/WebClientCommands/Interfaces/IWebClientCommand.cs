using ShowingAds.AndroidApp.Core.BusinessCollections.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Network.WebClientCommands.Interfaces
{
    public interface IWebClientCommand : IFilterVisitable, IVisitable
    {
        event Action<EventArgs> Completed;
        event Action<DownloadProgressChangedEventArgs> ProgressChanged;

        Task Execute();
        void Undo();
    }
}
