using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BuchShop.Geschaeftslogik.Geschaeftsservices;
using BuchShop.Geschaeftslogik.Domaenenobjekte;
using Microsoft.AspNetCore.Http;

namespace BuchShop.Controllers
{
    public class BenutzerController : Controller
    {
        private IBestellservice _bestellservice;
        private INutzerservice _nutzerservice;
        public BenutzerController(
            IBestellservice bestellservice, INutzerservice nutzerservice)
        {
            _bestellservice = bestellservice;
            _nutzerservice = nutzerservice;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Nutzer nutzer)
        {
            string email = nutzer.Email;
            nutzer.Identifikationsnummer =
                _nutzerservice.GetNutzerIdentifikationsnummerByEmail(email);
            {
                if (nutzer.Identifikationsnummer != 0)
                {
                    HttpContext.Session.SetString(
                        "Identifikationsnummer", nutzer.Identifikationsnummer.ToString());
                    return RedirectToAction("Startseite", "BuchShop");
                }
            }
            ModelState.AddModelError("Ungültig", "Email und/oder Passwort falsch");
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.SetString(
                "Identifikationsnummer", "0");
            return RedirectToAction("Login", "Benutzer");
        }
    }
}