using Common.Models;
using Common;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationService
{
    public class StationRepository : Repository<TripDB>
    {
        private IReliableStateManager StateManager;
        public StationRepository(IReliableStateManager stateManager) : base("Trip")
        {
            this.StateManager = stateManager;
        }
        public override List<TripDB> RetrieveAll()
        {
            IQueryable<TripDB> results = from g in _table.CreateQuery<TripDB>()
                                                where g.PartitionKey == _tableName
                                                select g; 
            return results.ToList();
        }

        public override async Task SyncTable()
        {
            var accountsDisc = await this.StateManager.GetOrAddAsync<IReliableDictionary<long, Trip>>("trips");

            using (var tx = this.StateManager.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<long, Trip>> enumerable = await accountsDisc.CreateEnumerableAsync(tx);
                using (Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<long, Trip>> e = enumerable.GetAsyncEnumerator())
                {
                    while (await e.MoveNextAsync(System.Threading.CancellationToken.None).ConfigureAwait(false))
                    {
                        AddOrUpdate(new TripDB(e.Current.Value));
                    }
                }
            }
        }
    }
}
