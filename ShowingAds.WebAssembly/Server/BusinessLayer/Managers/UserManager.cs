using Nito.AsyncEx;
using NLog;
using ShowingAds.CoreLibrary;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Managers;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Abstract;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Managers
{
    public sealed class UserManager : NotifyModelManager<int, User, UserManager>, IModelManager<int, User>
    {
        private Logger _logger { get; }

        private UserManager() : base(new WebProvider<int, User>(Settings.UsersPath), Settings.NotifyChannelPath)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task<IEnumerable<User>> GetPermittedModelsAsync(IEnumerable<int> users) =>
            await GetCollectionAsync(x => users.Contains(x.Id));

        public async Task<bool> TryAddOrUpdateAsync(User model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public async Task<IEnumerable<int>> GetEmployerUsers(int user)
        {
            var employerUsers = new List<int>();
            int lastUser;
            do
            {
                lastUser = user;
                employerUsers.Add(user);
                var (isSuccess, _user) = await TryGetAsync(user);
                if (isSuccess)
                    user = _user.OwnerId;
            } while (lastUser != user);
            return employerUsers;
        }

        public async Task<IEnumerable<int>> GetEmployeeUsers(int user)
        {
            var employeeUsers = new List<int>();
            var queue = new Queue<int>();
            queue.Enqueue(user);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var users = (await GetCollectionAsync(x => x.OwnerId == current && x.Id != current))
                    .Select(x => x.Id).ToList();
                users.ForEach(x => queue.Enqueue(x));
                employeeUsers.Add(current);
            }
            return employeeUsers;
        }

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(User model)
        {
            var employerUsers = await GetEmployerUsers(model.Id);
            return employerUsers.Select(x => x.ToGuid());
        }
    }
}
