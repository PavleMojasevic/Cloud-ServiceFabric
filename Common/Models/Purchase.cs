using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DataContract]
    public class Purchase
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Username{ get; set; }
        [DataMember]
        public DateTime Date{ get; set; }
        [DataMember]
        public List<long> TripIds { get; set; } = new List<long>();
        [DataMember]
        public List<int> Quantities { get; set; } = new List<int>();
        [DataMember]
        public List<decimal> Amounts { get; set; } = new List<decimal>();
    }
}
