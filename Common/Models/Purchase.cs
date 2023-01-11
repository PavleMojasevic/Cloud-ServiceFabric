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
        public List<string> TripIds { get; set; } = new List<string>();
        [DataMember]
        public List<int> Quantities { get; set; } = new List<int>();
        [DataMember]
        public List<double> Amounts { get; set; } = new List<double>();
    }
}
