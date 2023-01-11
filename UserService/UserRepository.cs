using Common;
using Common.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService
{
    public class UserRepository:Repository<UserDB>
    {

        private IReliableStateManager StateManager;
        public UserRepository(IReliableStateManager stateManager) : base("User")
        {
            this.StateManager = stateManager;
        }

        public override List<UserDB> RetrieveAll()
        {
            {

                IQueryable<UserDB> results = from g in _table.CreateQuery<UserDB>()
                                        where g.PartitionKey == _tableName
                                        select g;
                var list = results.ToList();
                foreach (var item in list)
                {
                    IQueryable<PurchaseDB> resultsPur = from g in _table.CreateQuery<PurchaseDB>()
                                                 where g.PartitionKey == "Purchase" && g.Username==item.Username
                                                        select g;
                    item.Purchases.AddRange(resultsPur.ToList());
                }
                return results.ToList();
            }
        }

        public override async Task SyncTable()
        {
            var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");
            using (var tx = this.StateManager.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<string, User>> enumerable = await users.CreateEnumerableAsync(tx);
                using (Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<string, User>> e = enumerable.GetAsyncEnumerator())
                {
                    while (await e.MoveNextAsync(System.Threading.CancellationToken.None).ConfigureAwait(false))
                    {
                        AddOrUpdate(new UserDB(e.Current.Value));
                        foreach (var item in e.Current.Value.Purchases)
                        {
                            AddOrUpdatePurchase(new PurchaseDB(item));
                        }
                    }
                }
            }
        }

        private void AddOrUpdatePurchase(PurchaseDB purchaseDB)
        {
            TableOperation insertOperation = TableOperation.InsertOrReplace(purchaseDB);
            var result = _table.Execute(insertOperation);
        }
    }
}
