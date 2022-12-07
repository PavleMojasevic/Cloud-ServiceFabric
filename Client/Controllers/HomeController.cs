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
            /*IEnumerable<string> x = HttpContext.Session.Keys;
            if (!HttpContext.Session.Keys.Contains("user"))
            {
                return RedirectToAction("Index", "Login");
            }*/ 
            var proxy = WcfHelper.GetStationService();

            Dictionary<long,Trip> list = (await proxy.GetTrips(filter)).ToDictionary(x=>x.Id);
            ViewBag.List=list;
            ViewData["Purchase"] = purchase;
            ViewBag.Message=message;
            ViewData["Title"] = "Početna strana";
            return View();
        }
        private static Purchase purchase = new Purchase();//TODO: u sesiju
        public async Task<IActionResult> AddTrip(long tripId, decimal price)
        {
            if(purchase.TripIds.Contains(tripId))
            {
                purchase.Quantities[purchase.TripIds.IndexOf(tripId)]++;
                purchase.Amounts[purchase.TripIds.IndexOf(tripId)]+= price;
            }
            else
            {
                purchase.TripIds.Add(tripId);
                purchase.Quantities.Add(1);
                purchase.Amounts.Add(price);
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> MakePurchase()
        {
            return RedirectToAction("Index", "Home");

        }



    }
}
