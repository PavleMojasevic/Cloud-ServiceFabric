using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<User> Login(string username, string password)
        { 
            var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
            using (var tx = this.StateManager.CreateTransaction())
            {
                User user =(await users.TryGetValueAsync(tx, username)).Value;
                if (user == null)
                    return null;
                if (user.Password == password)
                    return user;
                return null;

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
        public async Task<User> AddPurchase(string username, Purchase purchase)
        {
            using (var tx = this.StateManager.CreateTransaction())
            {
                purchase.Date = System.DateTime.Now;
                var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
                User user = (await users.TryGetValueAsync(tx, username)).Value;
                user.Purchases.Add(purchase);
                await users.AddOrUpdateAsync(tx, username, user, (k, v) => v);
                await tx.CommitAsync();
                return user;
            }
        }
        public async Task<User> RemovePurchase(string username, Purchase purchase)
        {
            using (var tx = this.StateManager.CreateTransaction())
            {
                
                var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
                User user = (await users.TryGetValueAsync(tx, username)).Value;
                user.Purchases.Remove(purchase);
                await users.AddOrUpdateAsync(tx, username, user, (k, v) => v);
                await tx.CommitAsync();
                return user;
            }
        }
        public async Task<List<Purchase>>GetPurchases(string username)
        { 
            using (var tx = this.StateManager.CreateTransaction())
            {
                var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
                User user = (await users.TryGetValueAsync(tx, username)).Value;
                return user.Purchases;
            } 
        }

        public async Task<User> GetUserByEmail(string username)
        {

            List<User> result = new List<User>();
            using (var tx = this.StateManager.CreateTransaction())
            {
                var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
                IAsyncEnumerable<KeyValuePair<string, User>> enumerable = await users.CreateEnumerableAsync(tx);
                using (IAsyncEnumerator<KeyValuePair<string, User>> e = enumerable.GetAsyncEnumerator())
                {
                    while (await e.MoveNextAsync(System.Threading.CancellationToken.None).ConfigureAwait(false))
                    {
                        result.Add(e.Current.Value);
                    }
                }
            }
            return result.FirstOrDefault(x=>x.Email== username); 
        }

    }
}