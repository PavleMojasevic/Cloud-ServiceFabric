using Common;
using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionCoordinator
{
    public class TransactionCoordinatorProvider : ITransactionCoordinator
    {
        private IReliableStateManager stateManager;

        public TransactionCoordinatorProvider(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        public async Task<List<Trip>> GetTrips(FilterDto filter)
        {
            return new List<Trip>();
        }
    }
}
