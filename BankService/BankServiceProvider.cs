using Common.Interfaces;
using Common.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankService
{
    public class BankServiceProvider : IBankService
    {
        private IReliableStateManager StateManager;
        private Random random=new Random();

        public BankServiceProvider(IReliableStateManager stateManager)
        {
            this.StateManager = stateManager;
        }

        public async Task AddAccount(string accountNumber)
        {
            var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BankAccount>>("accounts");
            using (var tx = this.StateManager.CreateTransaction())
            {
                BankAccount account = new BankAccount
                {
                    AccountNumber = accountNumber,
                    AvailableFunds = 1000 + random.Next(10000)
                };
                bool result = await users.TryAddAsync(tx, accountNumber, account);
                await tx.CommitAsync();

            }
        }

        public async Task<bool> Prepare(string accountNumber)
        {
            using (var tx = this.StateManager.CreateTransaction())
            {
                var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("accounts");
                return (await users.TryGetValueAsync(tx, accountNumber)).Value == null;
            }
        }
    }
}
