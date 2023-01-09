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

        public async Task<bool> CancelPurchase(User user, Guid purchaseId)
        {
            var bankService = await ServiceFabricClientHelper.GetBankService();
            var stationService = await ServiceFabricClientHelper.GetStationService();
            var userService = await ServiceFabricClientHelper.GetUserService();

            Purchase purchase = user.Purchases.FirstOrDefault(x => x.Id == purchaseId);
            if (purchase == null)
                return false;
            if (await stationService.InvokeWithRetryAsync(client => client.Channel.ReturnTickets(purchase)))
            { 
                await userService.InvokeWithRetryAsync(client => client.Channel.RemovePurchase(user.Username, purchase));
                await bankService.InvokeWithRetryAsync(client => client.Channel.Refund(user.BankAccountNumber, purchase.Amounts.Sum()));
                return true;
            }
            return false;
        }

        public async Task<bool> MakePurchase(User user, Purchase purchase)
        {
            var bankService = await ServiceFabricClientHelper.GetBankService();
            var stationService = await ServiceFabricClientHelper.GetStationService();
            var userService = await ServiceFabricClientHelper.GetUserService();
            purchase.Id = Guid.NewGuid();
            decimal bankResponse = await bankService.InvokeWithRetryAsync(client => client.Channel.GetAvailableFunds(user.BankAccountNumber));
            bool stationResponse = await stationService.InvokeWithRetryAsync(client => client.Channel.IsAvailable(purchase));

            if (stationResponse && bankResponse >= purchase.Amounts.Sum())
            {
                try
                {
                    await bankService.InvokeWithRetryAsync(client => client.Channel.Pay(user.BankAccountNumber, purchase.Amounts.Sum()));
                    await stationService.InvokeWithRetryAsync(client => client.Channel.BuyTickets(purchase));
                    await userService.InvokeWithRetryAsync(client => client.Channel.AddPurchase(user.Username, purchase));
                    return true;
                }
                catch (Exception e)
                {
                    await bankService.InvokeWithRetryAsync(client => client.Channel.Refund(user.BankAccountNumber, purchase.Amounts.Sum()));
                    await stationService.InvokeWithRetryAsync(client => client.Channel.ReturnTickets(purchase, true));
                }
            }
            return false;
        }

        public async Task<bool> Registration(User user, string accountNumber)
        {
            var bankService = await ServiceFabricClientHelper.GetBankService();
            var userService = await ServiceFabricClientHelper.GetUserService();
            user.BankAccountNumber = accountNumber;
            bool bankResponse = await bankService.InvokeWithRetryAsync(client => client.Channel.PrepareAdd(accountNumber));
            bool userResponse = await userService.InvokeWithRetryAsync(client => client.Channel.PrepareRegistration(user.Username));

            if (bankResponse && userResponse)
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

        public async Task<bool> MakePurchaseFromMail(Purchase purchase)
        {

            var userService = await ServiceFabricClientHelper.GetUserService();
            var stationService = await ServiceFabricClientHelper.GetStationService();
            User user = await userService.InvokeWithRetryAsync(client => client.Channel.GetUserByEmail(purchase.Username));
            if (user == null)
                return false;
            purchase.Username = user.Username;
            for (int i = 0; i < purchase.TripIds.Count; i++)
            {
                long id = purchase.TripIds[i];
                double price = await stationService.InvokeWithRetryAsync(client => client.Channel.GetTripPrice(id));
                purchase.Amounts.Add((decimal)price * purchase.Quantities[i]);
            }
            return await MakePurchase(user, purchase);
        }
    }
}
