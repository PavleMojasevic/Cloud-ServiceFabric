using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DataContract]
    public class BankAccount
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public decimal AvailableFunds { get; set; }
    }
}
