using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Trip
    {
        public long Id { get; set; }
        public TripType Type { get; set; }
        public double Price{ get; set; }
        public DateTime Depart{ get; set; }
        public DateTime? Return{ get; set; }
        public int TotalTickets{ get; set; }
        public int AvailableTickets{ get; set; }
    }
}
