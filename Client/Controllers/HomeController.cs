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
        public async Task<IActionResult> Index(FilterDto filter)
        {
            /*IEnumerable<string> x = HttpContext.Session.Keys;
            if (!HttpContext.Session.Keys.Contains("user"))
            {
                return RedirectToAction("Index", "Login");
            }*/
            var binding = new NetTcpBinding(SecurityMode.None);
            var endpoint = new EndpointAddress("net.tcp://localhost:53851/StationService");
            ChannelFactory<IStationService> channelFactory = new ChannelFactory<IStationService>(binding, endpoint);
            var proxy = channelFactory.CreateChannel();

            List<Trip> list = await proxy.GetTrips(filter);
            ViewBag.List=(list);
            ViewData["Title"] = "Početna strana";
            return View();
        }

       
    }
}
