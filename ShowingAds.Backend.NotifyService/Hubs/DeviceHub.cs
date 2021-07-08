using Microsoft.AspNetCore.SignalR;
using ShowingAds.Shared.Core.Models.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Hubs
{
    public class DeviceHub : Hub
    {
        public async Task SetListenDevice(Guid deviceId) =>
            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId.ToString());

        public async Task SendDiagnosticInfo(Guid deviceId, string info) =>
            await Clients.Group(deviceId.ToString()).SendAsync("SetDiagnostic", info);
    }
}
