using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Core.Models.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShowingAds.Shared.Core;

namespace ShowingAds.Backend.DeviceService.Managers
{
    public class DeviceTasksManager : NotifyModelManager<Guid, DeviceTasks, DeviceTasksManager>
    {
        private readonly UserManager _userManager = UserManager.GetInstance();
        private readonly DeviceManager _deviceManager = DeviceManager.GetInstance();

        public DeviceTasksManager() : base(new EmptyProvider<Guid, DeviceTasks>()) { }

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(Guid key, DeviceTasks model)
        {
            var device = await _deviceManager.GetOrDefaultAsync(key);
            var employerUsers = await _userManager.GetEmployerUsers(device.OwnerId);
            return employerUsers.Select(x => x.ToGuid());
        }
    }
}
