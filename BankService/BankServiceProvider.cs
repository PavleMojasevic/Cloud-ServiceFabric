using Common.Interfaces;
using Common.Models;
using Microsoft.Azure;
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

        public async Task AddAccount(string accountNumber, string username)
        {
            var users = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BankAccount>>("accounts");
            using (var tx = this.StateManager.CreateTransaction())
            {
                BankAccount account = new BankAccount
                {
                    AccountNumber = accountNumber,
                    Username=username,
                    AvailableFunds = 3000 + random.Next(10000)
                };
                bool result = await users.TryAddAsync(tx, accountNumber, account);
                await tx.CommitAsync();

            }
        }

        public async Task<bool> PrepareAdd(string accountNumber)
        {
            using (var tx = this.StateManager.CreateTransaction())
            {
                var accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BankAccount>>("accounts");
                return (await accounts.TryGetValueAsync(tx, accountNumber)).Value == null;
            }
        }

        public async Task RemoveAccount(string accountNumber)
        {
            var accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BankAccount>>("accounts");
            using (var tx = this.StateManager.CreateTransaction())
            { 
                await accounts.TryRemoveAsync(tx, accountNumber);
                await tx.CommitAsync();

            }
        }

        public async Task<bool> Pay(string accountNumber, decimal amount)
        {
            IReliableDictionary<string, BankAccount> accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BankAccount>>("accounts");
            using (var tx = this.StateManager.CreateTransaction())
            {
                BankAccount acc = (await accounts.TryGetValueAsync(tx, accountNumber, LockMode.Update)).Value;
                acc.AvailableFunds -= amount;
                if (acc.AvailableFunds < 0)
                    return false;
                await accounts.AddOrUpdateAsync(tx, accountNumber, acc, (k, v) => v);
                await tx.CommitAsync();

            }
            return true;
        }  
        public async Task Refund(string accountNumber, decimal amount)
        {
            IReliableDictionary<string, BankAccount> accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BankAccount>>("accounts");
            using (var tx = this.StateManager.CreateTransaction())
            {
                BankAccount acc =(await accounts.TryGetValueAsync(tx, accountNumber, LockMode.Update)).Value;
                acc.AvailableFunds += amount; 
                await accounts.AddOrUpdateAsync(tx, accountNumber, acc, (k,v)=>v);
                await tx.CommitAsync();

            } 
        }
        public async Task AddFunds(string accountNumber, decimal amount)
        {
            IReliableDictionary<string, BankAccount> accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BankAccount>>("accounts");
            using (var tx = this.StateManager.CreateTransaction())
            {
                BankAccount acc = (await accounts.TryGetValueAsync(tx, accountNumber, LockMode.Update)).Value;
                acc.AvailableFunds += amount; 
                await accounts.AddOrUpdateAsync(tx, accountNumber, acc, (k, v) => v);
                await tx.CommitAsync();

            } 
        }

        public async Task<decimal> GetAvailableFunds(string accountNumber)
        {
            using (var tx = this.StateManager.CreateTransaction())
            {
                var accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BankAccount>>("accounts");
                return (await accounts.TryGetValueAsync(tx, accountNumber)).Value.AvailableFunds;
            }
        }
    }
}
