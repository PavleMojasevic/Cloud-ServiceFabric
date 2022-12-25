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

        public async Task CancelPurchase(User user, Guid purchaseId)
        {
            var bankService = await ServiceFabricClientHelper.GetBankService();
            var stationService = await ServiceFabricClientHelper.GetStationService();
            var userService = await ServiceFabricClientHelper.GetUserService();

            Purchase purchase = user.Purchases.First(x => x.Id == purchaseId);

            await bankService.InvokeWithRetryAsync(client => client.Channel.Refund(user.BankAccountNumber, purchase.Amounts.Sum()));
            await stationService.InvokeWithRetryAsync(client => client.Channel.RollbackBuyTickets(purchase));
            await userService.InvokeWithRetryAsync(client => client.Channel.RemovePurchase(user.Username, purchase));
        }

        public async Task<bool> MakePurchase(User user, Purchase purchase)
        { 
            var bankService = await ServiceFabricClientHelper.GetBankService();
            var stationService = await ServiceFabricClientHelper.GetStationService();
            var userService = await ServiceFabricClientHelper.GetUserService();
            purchase.Id=Guid.NewGuid();
            decimal bankResponse = await bankService.InvokeWithRetryAsync(client => client.Channel.GetAvailableFunds(user.BankAccountNumber));
            bool stationResponse = await stationService.InvokeWithRetryAsync(client => client.Channel.IsAvailable(purchase));
            
            if(stationResponse && bankResponse>=purchase.Amounts.Sum())
            {
                try
                {
                    await bankService.InvokeWithRetryAsync(client => client.Channel.Pay(user.BankAccountNumber, purchase.Amounts.Sum()));
                    await stationService.InvokeWithRetryAsync(client => client.Channel.BuyTickets(purchase));
                    await userService.InvokeWithRetryAsync(client => client.Channel.AddPurchase(user.Username, purchase));
                    return true;
                    //TODO dodati recnike
                }
                catch (Exception e)
                {
                    await bankService.InvokeWithRetryAsync(client => client.Channel.Refund(user.BankAccountNumber, purchase.Amounts.Sum()));
                    await stationService.InvokeWithRetryAsync(client => client.Channel.RollbackBuyTickets(purchase));
                }
            }
            return false;
        }

        public async Task<bool> Registration(User user, string accountNumber)
        {
            var bankService =await ServiceFabricClientHelper.GetBankService(); 
            var userService =await ServiceFabricClientHelper.GetUserService();
            user.BankAccountNumber = accountNumber;
            bool bankResponse=await bankService.InvokeWithRetryAsync(client => client.Channel.PrepareAdd(accountNumber));
            bool userResponse=await userService.InvokeWithRetryAsync(client => client.Channel.PrepareRegistration(user.Username));

            if(bankResponse && userResponse)
            {
                try
                {
                    await bankService.InvokeWithRetryAsync(client => client.Channel.AddAccount(accountNumber, user.Username));
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
