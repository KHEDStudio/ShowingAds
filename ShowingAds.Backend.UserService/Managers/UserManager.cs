using MassTransit;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Managers
{
    public sealed class UserManager : NotifyModelManager<int, User, UserManager>, IModelManager<int, User>
    {
        private UserManager() : base(new WebProvider<int, User>(Settings.UsersPath)) { }

        public async Task<IEnumerable<User>> GetPermittedModelsAsync(IEnumerable<int> users) =>
               await GetCollectionOrNullAsync(x => users.Contains(x.Id));

        public async Task<bool> TryAddOrUpdateAsync(User model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public async Task<bool> TryDeleteAsync(User model) =>
            await TryDeleteAsync(model.Id, model);

        public async Task<IEnumerable<int>> GetEmployerUsers(int userId)
        {
            var employerUsers = new List<int>();
            int lastUser;
            do
            {
                lastUser = userId;
                employerUsers.Add(userId);
                var user = await GetOrDefaultAsync(userId);
                if (user != default)
                    userId = user.OwnerId;
            } while (lastUser != userId);
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
                var users = (await GetCollectionOrNullAsync(x => x.OwnerId == current && x.Id != current))
                    .Select(x => x.Id).ToList();
                users.ForEach(x => queue.Enqueue(x));
                employeeUsers.Add(current);
            }
            return employeeUsers;
        }

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(User model)
        {
            var employerUsers = new List<int>() { 1, 2, 3 }; //await GetEmployerUsers(model.Id);
            return employerUsers.Select(x => x.ToGuid());
        }
    }
}
