using Nito.AsyncEx;
using NLog;
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

        private UserManager() : base(new WebProvider<int, User>(Settings.UsersPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            UpdateOrInitializeModels(default, default);
        }

        protected override void UpdateOrInitializeModels(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Initialize Users...");
            base.UpdateOrInitializeModels(sender, e);
            _syncTimer.Start();
        }

        public async Task<IEnumerable<User>> GetPermittedModels(List<int> users) =>
            await GetCollection(x => users.Contains(x.Id));

        public async Task<bool> TryAddOrUpdate(User model) =>
            await TryAddOrUpdate(model.Id, model);

        public async IAsyncEnumerable<int> GetEmployerUsers(int user)
        {
            int lastUser;
            do
            {
                lastUser = user;
                yield return user;
                var (isSuccess, _user) = await TryGet(user);
                if (isSuccess)
                    user = _user.OwnerId;
            } while (lastUser != user);
        }

        public async IAsyncEnumerable<int> GetEmployeeUsers(int user)
        {
            var queue = new Queue<int>();
            queue.Enqueue(user);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var users = (await GetCollection(x => x.OwnerId == current && x.Id != current))
                    .Select(x => x.Id).ToList();
                users.ForEach(x => queue.Enqueue(x));
                yield return current;
            }
        }
    }
}
