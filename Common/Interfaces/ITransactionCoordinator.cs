using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface ITransactionCoordinator
    {
        [OperationContract]
        Task CancelPurchase(User user, Guid purchaseId);
        [OperationContract]
        Task<bool> MakePurchase(User user, Purchase purchase);
        [OperationContract]
        Task<bool> MakePurchaseFromMail( Purchase purchase);
        [OperationContract]
        Task<bool> Registration(User user, string accountNumber);
    }
}
