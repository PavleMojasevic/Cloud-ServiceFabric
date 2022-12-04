using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface ILoginService
    {
        [OperationContract]
        Task<bool> Login(string username, string password); 

    }
}
