using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Core.Models.States;
using System;
using System.Collections.Generic;
using ShowingAds.Shared.Core;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Managers
{
    public class DiagnosticInfoManager : NotifyModelManager<Guid, DiagnosticInfo, DiagnosticInfoManager>
    {
        private readonly UserManager _userManager = UserManager.GetInstance();
        private readonly DeviceManager _deviceManager = DeviceManager.GetInstance();

        public DiagnosticInfoManager() : base(new EmptyProvider<Guid, DiagnosticInfo>()) { }

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(Guid key, DiagnosticInfo model)
        {
            var device = await _deviceManager.GetOrDefaultAsync(key);
            if (device != default)
            {
                var employerUsers = await _userManager.GetEmployerUsers(device.OwnerId);
                return employerUsers.Select(x => x.ToGuid());
            }
            return new List<Guid>();
        }
    }
}
