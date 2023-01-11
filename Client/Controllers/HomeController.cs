using Client.Models;
using Common;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(FilterDto filter, string message, bool localData = false)
        {
            if (localData)
            {

                ViewBag.List = TempStorage.Trips.Where(x => x.Depart > DateTime.Now).ToDictionary(x => x.Id);
            }
            else
            {
                var proxy = WcfHelper.GetStationService();
                Dictionary<string, Trip> list = (await proxy.GetTrips(filter)).ToDictionary(x => x.Id);
                ViewBag.List = list;
                TempStorage.Trips = list.Values.ToList();
            }
            Purchase purchase = HttpContext.Session.GetObjectFromSession<Purchase>("purchase");
            ViewBag.Purchase = purchase;
            ViewBag.Message = message;
            ViewBag.Quantity = filter.Quantity;
            ViewBag.Depart = filter.Depart;
            ViewBag.Type = filter.Type;
            ViewData["Title"] = "Početna strana";
            ViewBag.User = HttpContext.Session.GetObjectFromSession<User>("user");
            return View();
        }
        public async Task<IActionResult> RefreshTrips()
        {
            return RedirectToAction("Index", "Home", new { localData = true });
        }
        public async Task<IActionResult> Purchases(string message, bool localData = false)
        {
            User user = HttpContext.Session.GetObjectFromSession<User>("user");
            if (user == null)
                return RedirectToAction("Index", "Login");
            User u = HttpContext.Session.GetObjectFromSession<User>("user");
            if (localData)
            {
                ViewBag.List = TempStorage.Purchases[user.Username];

                u.Purchases = TempStorage.Purchases[user.Username];
                HttpContext.Session.SetObjectInSession("user", u);
            }
            else
            {
                var proxy = WcfHelper.GetUserService();
                List<Purchase> list = await proxy.GetPurchases(user.Username);
                TempStorage.Purchases[user.Username] = list;
                ViewBag.List = list; 
                u.Purchases = list;
                HttpContext.Session.SetObjectInSession("user", u);
            }
            ViewBag.Message = message;
            ViewData["Title"] = "Moje kupovine";  

            ViewBag.User = u;
            return View();
        }
        public async Task<IActionResult> RefreshPurchases()
        {
            return RedirectToAction("Purchases", "Home", new { localData = true });
        }

        public async Task<IActionResult> AddTrip(string message)
        {
            ViewBag.Message = message;
            ViewData["Title"] = "Dodavanje putovanja";
            ViewBag.User = HttpContext.Session.GetObjectFromSession<User>("user");
            return View();
        }
        public async Task<IActionResult> CancelPurchase(string purchaseId)
        {
            User user = HttpContext.Session.GetObjectFromSession<User>("user");
            if (user == null)
                return RedirectToAction("Index", "Login");
            var proxy = WcfHelper.GetTransactionCoordinator();
            string message = "Kupovina se ne može otkazati";
            if (!await proxy.CancelPurchase(user, Guid.Parse(purchaseId)))
            {
                return RedirectToAction("Purchases", "Home", new { message = message });
            }
            return RedirectToAction("Purchases", "Home");
        }
        public async Task<IActionResult> AddNewTrip(Trip trip)
        {

            var proxy = WcfHelper.GetStationService();
            await proxy.AddTrip(trip);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> AddTripPurchase(string tripId, double price)
        {
            Purchase purchase = HttpContext.Session.GetObjectFromSession<Purchase>("purchase");
            if (purchase == null)
                purchase = new Purchase();
            if (purchase.TripIds.Contains(tripId))
            {
                purchase.Quantities[purchase.TripIds.IndexOf(tripId)]++;
                purchase.Amounts[purchase.TripIds.IndexOf(tripId)] += price;
            }
            else
            {
                purchase.TripIds.Add(tripId);
                purchase.Quantities.Add(1);
                purchase.Amounts.Add(price);
            }
            HttpContext.Session.SetObjectInSession("purchase", purchase);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> RemoveTrip(int index)
        {
            Purchase purchase = HttpContext.Session.GetObjectFromSession<Purchase>("purchase");
            if (purchase.Quantities.Count == 1)
            {
                HttpContext.Session.Remove("purchase");
                return RedirectToAction("Index", "Home");
            }
            purchase.TripIds.RemoveAt(index);
            purchase.Quantities.RemoveAt(index);
            purchase.Amounts.RemoveAt(index);
            HttpContext.Session.SetObjectInSession("purchase", purchase);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> MakePurchase()
        {
            if (HttpContext.Session.GetObjectFromSession<User>("user") == null)
                return RedirectToAction("Index", "Login");

            var proxy = WcfHelper.GetTransactionCoordinator();
            Purchase purchase = HttpContext.Session.GetObjectFromSession<Purchase>("purchase");
            purchase.Id = Guid.NewGuid();
            if(!await proxy.MakePurchase(HttpContext.Session.GetObjectFromSession<User>("user"), purchase))
                return RedirectToAction("Index", "Home",new { message="Nije moguće izvršiti kupovinu"});


            HttpContext.Session.Remove("purchase");

            return RedirectToAction("Index", "Home");

        }


        public async Task<IActionResult> History(string message, SortType sortType = SortType.Depart)
        {
            var proxy = WcfHelper.GetStationService();
            Dictionary<string, Trip> list = (await proxy.GetTripsHistory(sortType)).ToDictionary(x => x.Id);
            ViewBag.List = list;

            ViewBag.sortType = sortType;
            ViewData["Title"] = "Istorija putovanja";
            ViewBag.User = HttpContext.Session.GetObjectFromSession<User>("user");
            return View();

        }
    }
}
