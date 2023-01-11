using Common.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System; 

namespace StationService
{
    public class TripDB:TableEntity
    {
        public TripDB()
        {

        }

        public TripDB(Trip trip)
        {
            Id= trip.Id;
            Destination = trip.Destination;
            Type = trip.Type;
            Price = trip.Price;
            Depart = trip.Depart;
            Return = trip.Depart;
            TotalTickets = trip.TotalTickets;
            AvailableTickets = trip.AvailableTickets;
            Weather = trip.Weather;
        }

        public long Id { get; set; } 
        public string Destination { get; set; } 
        public TripType Type { get; set; }
        public double Price { get; set; }
        public DateTime Depart { get; set; }
        public DateTime? Return { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }
        public string Weather { get; set; }
    }
}
