using Common.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UserService
{
    public class UserDB:TableEntity
    {
        public UserDB() { }
        public UserDB(User user) 
        {
            Username= user.Username;
            Password= user.Password;
            Email= user.Email;
            BankAccountNumber= user.BankAccountNumber;
            Purchases = user.Purchases.Select(x=>new PurchaseDB(x)).ToList();
            PartitionKey = "User";
            RowKey = user.Username;
        }
        public string Username { get; set; }
        public string Password { get; set; } 
        public string Email { get; set; } 
        public string BankAccountNumber { get; set; } 
        public List<PurchaseDB> Purchases { get; set; } = new List<PurchaseDB>();

    }
}
