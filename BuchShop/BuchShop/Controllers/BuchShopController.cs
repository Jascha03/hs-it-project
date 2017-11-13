using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BuchShop.Models;
using BuchShop.Geschaeftslogik.Domaenenobjekte;
using BuchShop.Datenzugriff;
using BuchShop.Geschaeftslogik.Geschaeftsservices;
using BuchShop.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BuchShop.Controllers
{
    public class BuchShopController : Controller
    {
        // private readonly INutzerservice _nutzerservice;
        private IBestellservice _bestellservice;
        private INutzerservice _nutzerservice;
        public BuchShopController(
            IBestellservice bestellservice, INutzerservice nutzerservice)
        {
            _bestellservice = bestellservice;
            _nutzerservice = nutzerservice;
        }

        public IActionResult Index()
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                Response.Redirect("BuchShop/Login", true);
            }
           
            return View();
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
            
            if(nutzer.Identifikationsnummer != 0)
            {
            Response.Redirect("Startseite", true);
            HttpContext.Session.SetString(
                "Identifikationsnummer", nutzer.Identifikationsnummer.ToString());
            }
            ModelState.AddModelError("Ungültig", "Email und/oder Passwort falsch");
            return View();
        }

        public IActionResult Startseite()
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                Response.Redirect("Login", true);
            }
            Nutzer nutzer = _nutzerservice.GetNutzerByNutzerId(int.Parse(value));
            if (nutzer is Kunde)
            {
                ViewData["Nutzer"] = "Kunde";
                ViewData["Treuepunkte"] = ((Kunde)nutzer).Treuepunkte;
                ViewData["Kundenstatus"] = ((Kunde)nutzer).Status.ToString();
                ViewData["Artikel"] = _bestellservice.GetWarenkorbArtikelanzahlByKundenId(int.Parse(value));
            }
            else if(nutzer is Mitarbeiter)
            {
                ViewData["Telefonnummer"] = ((Mitarbeiter)nutzer).Telefonnummer;
                string telefon = ((Mitarbeiter)nutzer).Telefonnummer;
            }

            return View("Startseite", nutzer);
        }

        public IActionResult Artikelsuche()
        {
            return View();
        }
    }
}
