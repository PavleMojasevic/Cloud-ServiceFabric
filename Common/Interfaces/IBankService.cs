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
        Task<bool> Prepare(string accountNumber);
        [OperationContract]
        Task AddAccount(string accountNumber);
    }
}
