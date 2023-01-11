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
        Task AddAccount(string accountNumber, string username);
        [OperationContract]
        Task RemoveAccount(string accountNumber);
        [OperationContract]
        Task<double> GetAvailableFunds(string accountNumber);
        [OperationContract]
        Task AddFunds(string accountNumber, double amount);
        [OperationContract]
        Task<bool> Pay(string accountNumber, double amount);
        [OperationContract]
        Task Refund(string bankAccountNumber, double amount);
    }
}
