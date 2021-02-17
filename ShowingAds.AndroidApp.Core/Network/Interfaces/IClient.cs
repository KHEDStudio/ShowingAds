using ShowingAds.AndroidApp.Core.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Network.Interfaces
{
    public interface IClient
    {
        Task SendRequest();
        void StartPeriodicTimerRequest(TimeSpan interval);
        void SetDiagnosticInfo(DiagnosticInfo info);
        Task DisposeAsync();
    }
}
