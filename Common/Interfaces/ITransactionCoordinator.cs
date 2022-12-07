﻿using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface ITransactionCoordinator
    { 
        [OperationContract]
        Task<bool> Registration(User user, string accountNumber);
    }
}