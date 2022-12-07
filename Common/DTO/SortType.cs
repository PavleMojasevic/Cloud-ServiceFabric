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
    public enum SortType
    {
        [EnumMember]
        Depart,
        [EnumMember]
        DepartDESC,
        [EnumMember]
        COUNTAVAILABLE,
        [EnumMember]
        COUNTAVAILABLEDESC

    }
}
