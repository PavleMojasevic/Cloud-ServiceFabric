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
    public interface IStationService
    {
        [OperationContract]
        Task<List<Trip>> GetTrips(FilterDto filter);
        [OperationContract]
        Task<bool> IsAvailable(Purchase purchase);
        [OperationContract]
        Task BuyTickets(Purchase purchase);
        [OperationContract]
        Task<bool> ReturnTickets(Purchase purchase, bool skipValidation=false);
        [OperationContract]
        Task AddTrip(Trip trip);
        [OperationContract]
        Task<double> GetTripPrice(string id);
        [OperationContract]
        Task<List<Trip>> GetTripsHistory(SortType type);
    }
}
