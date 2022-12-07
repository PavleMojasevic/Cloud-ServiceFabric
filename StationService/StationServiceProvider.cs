using Common;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StationService
{
    public class StationServiceProvider : IStationService
    {
        private IReliableStateManager StateManager;

        public StationServiceProvider(IReliableStateManager stateManager)
        {
            this.StateManager = stateManager;
        }

        public async Task<List<Trip>> GetTrips(FilterDto filter)
        {
            List<Trip> result = new List<Trip>();
            using (var tx = this.StateManager.CreateTransaction())
            {
                var trips = await this.StateManager.GetOrAddAsync<IReliableDictionary<long, Trip>>("trips");
                IAsyncEnumerable<KeyValuePair<long, Trip>> enumerable = await trips.CreateEnumerableAsync(tx);
                using (IAsyncEnumerator<KeyValuePair<long, Trip>> e = enumerable.GetAsyncEnumerator())
                {
                    while (await e.MoveNextAsync(System.Threading.CancellationToken.None).ConfigureAwait(false))
                    {
                        result.Add(e.Current.Value);
                    }
                }
            }
            if(filter.Depart!=null)
                result=result.Where(x=>x.Depart==filter.Depart).ToList();
            if(filter.Quantity!=null)
                result=result.Where(x=>x.AvailableTickets == filter.Quantity).ToList();
            if(filter.Type != null)
                result=result.Where(x=>x.Type ==filter.Type).ToList();
            return result;
        }
    }
}