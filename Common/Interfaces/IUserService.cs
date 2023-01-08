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
        Task<User> Login(string username, string password);
        [OperationContract]
        Task<bool> PrepareRegistration(string username);
        [OperationContract]
        Task Registration(User user);
        [OperationContract]
        Task<User> AddPurchase(string username, Purchase purchase);
        [OperationContract]
        Task RollbackRegistration(string username);
        [OperationContract]
        Task<List<Purchase>> GetPurchases(string username);
        [OperationContract]
        Task<User> RemovePurchase(string username, Purchase purchase);
        [OperationContract]
        Task<User> GetUserByEmail(string username); 
    }
}
