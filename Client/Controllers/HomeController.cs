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
        public async Task<IActionResult> Index(FilterDto filter, string message)
        {
            var proxy = WcfHelper.GetStationService();
            Dictionary<long, Trip> list = (await proxy.GetTrips(filter)).ToDictionary(x => x.Id);
            ViewBag.List = list;
            Purchase purchase = HttpContext.Session.GetObjectFromSession<Purchase>("purchase");
            ViewData["Purchase"] = purchase;
            ViewBag.Message = message;
            ViewData["Title"] = "Početna strana";
            return View();
        }
        public async Task<IActionResult> Purchases(string message)
        {
            User user = HttpContext.Session.GetObjectFromSession<User>("user");
            if (user == null)
                return RedirectToAction("Index", "Login");
            var proxy = WcfHelper.GetUserService();
            List<Purchase> list = await proxy.GetPurchases(user.Username);
            ViewBag.List = list;
            ViewBag.Message = message;
            ViewData["Title"] = "Istorija kupovina";
            return View();
        }
        public async Task<IActionResult> CancelPurchase(string purchaseId)
        {
            User user = HttpContext.Session.GetObjectFromSession<User>("user");
            if (user == null)
                return RedirectToAction("Index", "Login");
            var proxy = WcfHelper.GetTransactionCoordinator();

            await proxy.CancelPurchase(user, Guid.Parse(purchaseId));
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> AddNewTrip(Trip trip)
        {

            var proxy = WcfHelper.GetStationService();
            await proxy.AddTrip(trip);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> AddTrip(long tripId, decimal price)
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
            await proxy.MakePurchase(HttpContext.Session.GetObjectFromSession<User>("user"), purchase);

            HttpContext.Session.Remove("purchase");

            return RedirectToAction("Index", "Home");

        }



    }
}
