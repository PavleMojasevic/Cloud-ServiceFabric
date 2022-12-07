using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface IBankService
    {
        [OperationContract]
        Task<bool> PrepareAdd(string accountNumber);
        [OperationContract]
        Task AddAccount(string accountNumber);
        [OperationContract]
        Task RemoveAccount(string accountNumber);
        [OperationContract]
        Task<decimal> GetAvailableFunds(string accountNumber);
        [OperationContract]
        Task AddFunds(string accountNumber, decimal amount);
        [OperationContract]
        Task<bool> Pay(string accountNumber, decimal amount);
    }
}
