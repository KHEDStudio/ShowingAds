using ShowingAds.Shared.Core.Models.States;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Network.Interfaces
{
    public interface IClient
    {
        event Action TakeScreenshotChanged;
        event Action<Guid> PriorityClientChanged;

        Task SendRequestAsync();
        void StartPeriodicTimerRequest(TimeSpan interval);
        void SetDiagnosticInfo(DiagnosticInfo info);
        Task DisposeAsync();
    }
}
