using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DataContract]
    public enum TripType
    {
        [EnumMember]
        Avion,
        [EnumMember]
        Autobus,
        [EnumMember]
        Voz,
        [EnumMember]
        Train

    }
}
