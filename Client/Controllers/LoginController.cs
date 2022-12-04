using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class LoginController : Controller
    { 
        public async Task<IActionResult> Index(string message)
        {
            
            ViewData["Title"] = "Početna strana";
            ViewBag.Message = message;
            return View();
        }
        public async Task<IActionResult> Registracija(string message)
        {
            ViewData["Title"] = "Početna strana";
            ViewBag.Message = message;
            return View();
        }

        #region Actions
        //-----------ACTIONS-----------
        public async Task<IActionResult> Login(string username, string password)
        {
            var binding = new NetTcpBinding(SecurityMode.None);
            var endpoint = new EndpointAddress("net.tcp://localhost:53852/LoginService");
            ChannelFactory<ILoginService> channelFactory = new ChannelFactory<ILoginService>(binding, endpoint);
            var proxy = channelFactory.CreateChannel();

            bool result = await proxy.Login(username, password);
            if(result)
            {
                HttpContext.Session.SetString("user",username);
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login", new { message = "Ne postoji korisnik sa unetim podacima" });
        } 
        public async Task<IActionResult> Register(User user)
        {



            return RedirectToAction("");
        }
        #endregion
    }
}
