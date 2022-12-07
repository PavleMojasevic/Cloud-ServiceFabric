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
         
        public async Task<bool> Registration(User user, string accountNumber)
        {
            var bankService =await ServiceFabricClientHelper.GetBankService(); 
            var userService =await ServiceFabricClientHelper.GetUserService(); 

            bool bankResponse=await bankService.InvokeWithRetryAsync(client => client.Channel.PrepareAdd(accountNumber));
            bool userResponse=await userService.InvokeWithRetryAsync(client => client.Channel.PrepareRegistration(user.Username));

            if(bankResponse && userResponse)
            {
                try
                {
                    await bankService.InvokeWithRetryAsync(client => client.Channel.AddAccount(accountNumber));
                    await userService.InvokeWithRetryAsync(client => client.Channel.Registration(user)); 
                    return true;
                    //TODO dodati recnike
                }
                catch (Exception e)
                {
                    await bankService.InvokeWithRetryAsync(client => client.Channel.RemoveAccount(accountNumber));
                    await userService.InvokeWithRetryAsync(client => client.Channel.RollbackRegistration(user.Username));
                }
            }
            return false;
        }
    }
}
