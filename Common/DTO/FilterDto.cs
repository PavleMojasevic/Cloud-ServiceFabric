using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class FilterDto
    {
        [DataMember]
        public TripType? Type{ get; set; }
        [DataMember]
        public DateTime? Depart { get; set; }
        [DataMember]
        public int? Quantity { get; set; }
    }
}
