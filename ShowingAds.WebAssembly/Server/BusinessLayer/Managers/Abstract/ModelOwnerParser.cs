using ShowingAds.CoreLibrary;
using ShowingAds.CoreLibrary.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Abstract
{
    public class ModelOwnerParser : Singleton<ModelOwnerParser>
    {
        private ModelOwnerParser() { }

        public async Task<IEnumerable<Guid>> GetUsers(object model)
        {
            var manager = UserManager.GetInstance();
            var users = await manager.GetCollectionAsync(x => true);
            return users.Select(x => x.Id.ToGuid());
        }

        public async Task<IEnumerable<Guid>> GetDevices(object model)
        {
            var manager = DeviceManager.GetInstance();
            var devices = await manager.GetCollectionAsync(x => true);
            return devices.Select(x => x.Id);
        }
    }
}
