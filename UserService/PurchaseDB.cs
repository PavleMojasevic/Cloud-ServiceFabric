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
    public class PurchaseDB:TableEntity
    {
        public PurchaseDB() { }
        public PurchaseDB(Purchase purchase)
        {
            Id=purchase.Id;
            RowKey=purchase.Id.ToString();
            PartitionKey = "Purchase";
            Username = purchase.Username;
            Date = purchase.Date;

            TripIds = "";
            purchase.TripIds.ForEach(x => TripIds += x + " ");
            TripIds=TripIds.Trim();

            Quantities = "";
            purchase.Quantities.ForEach(x => Quantities += x + " ");
            Quantities = Quantities.Trim();

            Amounts = "";
            purchase.Amounts.ForEach(x => Amounts += x + " ");
            Amounts = Amounts.Trim();


        }
        public Purchase GetPurchase()
        {
            Purchase purchase = new Purchase();
            purchase.Id = Id;
            purchase.Username = Username;
            purchase.Date = Date;

            purchase.TripIds = new List<string>();
            TripIds.Split(' ').ToList().ForEach(x => purchase.TripIds.Add(x));
            Quantities.Split(' ').ToList().ForEach(x => purchase.Quantities.Add(Convert.ToInt32(x)));
            Amounts.Split(' ').ToList().ForEach(x => purchase.Amounts.Add(Convert.ToDouble(x)));
            return purchase;
        }
        public Guid Id { get; set; } 
        public string Username { get; set; } 
        public DateTime Date { get; set; } 
        public string TripIds { get; set; }
        public string Quantities { get; set; } 
        public string Amounts { get; set; } 
    }
}
