using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Threading.Tasks;

namespace UserService
{
    public class UserServiceProvider : IUserService
    {
        private IReliableStateManager StateManager;

        public UserServiceProvider(IReliableStateManager stateManager)
        {
            this.StateManager = stateManager;
        }

        public async Task<bool> Login(string username, string password)
        { 
            var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
            using (var tx = this.StateManager.CreateTransaction())
            {
                User user =(await users.TryGetValueAsync(tx, username)).Value;
                if (user == null)
                    return false;
                return user.Password == password;

            }
        }

        public async Task<bool> PrepareRegistration(string username)
        {
            using (var tx = this.StateManager.CreateTransaction())
            {
                var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
                return (await users.TryGetValueAsync(tx, username)).Value == null;
            }
        }

        public async Task Registration(User user)
        {
            var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
            using (var tx = this.StateManager.CreateTransaction())
            {
                bool result = await users.TryAddAsync(tx, user.Username, user);
                await tx.CommitAsync(); 
            }
        }

        public async Task RollbackRegistration(string username)
        {
            using (var tx = this.StateManager.CreateTransaction())
            {
                var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
                await users.TryRemoveAsync(tx, username);
                await tx.CommitAsync();
            }
        }
    }
}