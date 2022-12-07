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
    public interface IUserService
    {
        [OperationContract]
        Task<bool> Login(string username, string password);
        [OperationContract]
        Task<bool> Prepare(string username);
        [OperationContract]
        Task Registration(User user);
    }
}
