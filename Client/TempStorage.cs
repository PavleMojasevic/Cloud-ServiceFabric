using Common.Models;
using System.Collections.Generic;

namespace Client
{
    public static class TempStorage
    {
        public static List<Trip> Trips { get; set; }=new List<Trip>();  
        public static Dictionary<string, List<Purchase>> Purchases{ get; set; }=new Dictionary<string, List<Purchase>>();

        public static void AddPurchase(Purchase purchase)
        {
            if(!Purchases.ContainsKey(purchase.Username))
            {
                Purchases[purchase.Username]=new List<Purchase>();  
            }
            Purchases[purchase.Username].Add(purchase);
        }
    }
}
