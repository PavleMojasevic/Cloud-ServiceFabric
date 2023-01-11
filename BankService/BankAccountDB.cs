using Common.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System.Runtime.Serialization;

namespace BankService
{
    public class BankAccountDB: TableEntity
    {
        public string Username { get; set; } 
        public string AccountNumber { get; set; } 
        public double AvailableFunds { get; set; }
        public BankAccountDB(BankAccount bankAccount)
        {
            Username=bankAccount.Username;
            AccountNumber=bankAccount.AccountNumber;
            AvailableFunds=bankAccount.AvailableFunds;
            PartitionKey = "BankAccount";
            RowKey = bankAccount.AccountNumber;
        }

        public BankAccountDB()
        {
        }
    }
}