using Common;
using Common.Models;
using Microsoft.Azure;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankService
{
   /* public class BankRepository:Repository<BankAccount>
    {
        private IReliableStateManager StateManager;
        public BankRepository(IReliableStateManager stateManager) :base("BankAccount")
        {
            this.StateManager = stateManager;
        }
        public override List<BankAccount> RetrieveAll()
        {
            IQueryable<BankAccount> results = from g in _table.CreateQuery<BankAccount>()
                          where g.PartitionKey == _tableName
                          select g;
            List<BankAccount> list = results.Select(s => 
            new BankAccount 
            { 
                Username = s.Username, 
                AccountNumber = s.AccountNumber, 
                AvailableFunds = s.AvailableFunds, 
                RowKey=s.RowKey 
            }).ToList(); 
            return list;
        }

        public override async Task SyncService()
        {
                var accountsDisc = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BankAccount>>("accounts"); 

                using (var tx = this.StateManager.CreateTransaction())
                {
                    Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<string, BankAccount>> enumerable = await accountsDisc.CreateEnumerableAsync(tx);
                    using (Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<string, BankAccount>> e = enumerable.GetAsyncEnumerator())
                    {
                        while (await e.MoveNextAsync(System.Threading.CancellationToken.None).ConfigureAwait(false))
                        { 
                            AddOrUpdate(e.Current.Value);
                        }
                    }
                }
                
        }
    }*/
}
