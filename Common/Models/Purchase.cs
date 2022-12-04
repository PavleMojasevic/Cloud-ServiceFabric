using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Weather{ get; set; }
    }
}
