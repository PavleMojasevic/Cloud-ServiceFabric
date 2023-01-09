using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Add(Purchase purchase)
        {
            TempStorage.AddPurchase(purchase);   
            for (int i = 0; i < purchase.TripIds.Count; i++)
            {
                long id = purchase.TripIds[i];
                Trip trip = TempStorage.Trips.FirstOrDefault(x => x.Id == id);
                if(trip!=null)
                {
                    trip.AvailableTickets -= purchase.Quantities[i];
                }

            }  
            return Ok();
        }
    }
}
