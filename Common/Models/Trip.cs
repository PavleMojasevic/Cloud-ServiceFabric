using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DataContract]
    public class Trip
    {

        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public TripType Type { get; set; }
        [DataMember]
        public double Price{ get; set; }
        [DataMember]
        public DateTime Depart{ get; set; }
        [DataMember]
        public DateTime? Return{ get; set; }
        [DataMember]
        public int TotalTickets{ get; set; }
        [DataMember]
        public int AvailableTickets{ get; set; }
        [DataMember]
        public string Weather { get; set; }
    }
}
