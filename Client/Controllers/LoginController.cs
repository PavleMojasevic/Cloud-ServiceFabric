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
            if (HttpContext.Session.GetObjectFromSession<User>("user") != null)
                return RedirectToAction("Index", "Home");

            ViewData["Title"] = "Početna strana";
            ViewBag.Message = message;
            return View();
        }
        public async Task<IActionResult> Registracija(string message)
        {
            if (HttpContext.Session.GetObjectFromSession<User>("user") != null)
                return RedirectToAction("Index", "Home");
            ViewData["Title"] = "Početna strana";
            ViewBag.Message = message;
            return View();
        }

        #region Actions
        //-----------ACTIONS-----------
        public async Task<IActionResult> Login(string username, string password)
        {

            var proxy = WcfHelper.GetUserService();

            User result = await proxy.Login(username, password);
            if (result != null)
            {
                HttpContext.Session.SetObjectInSession("user", result); 
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login", new { message = "Ne postoji korisnik sa unetim podacima" });
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
        public async Task<IActionResult> Register(User user, string accountNumber)
        {
            ITransactionCoordinator proxy = WcfHelper.GetTransactionCoordinator();

            bool result = await proxy.Registration(user, accountNumber);
            if (result)
            {
                return RedirectToAction("Index", "Login");
            }
            return RedirectToAction("Registracija", "Login", new { message = "Ne postoji korisnik sa unetim podacima" });

        }
        #endregion
    }
}
