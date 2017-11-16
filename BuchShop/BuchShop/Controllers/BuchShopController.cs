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
                return View("Login");
            }
            Nutzer nutzer = _nutzerservice.GetNutzerByNutzerId(int.Parse(value));
            if (nutzer is Kunde)
            {
                ViewData["Artikel"] = _bestellservice.GetWarenkorbArtikelanzahlByKundenId(int.Parse(value));
            }

            return View("Startseite", nutzer);
        }

        public IActionResult Artikelsuche(string artikelName, bool checkboxBuch, bool checkboxBlueRay)
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                Response.Redirect("Login", true);
            }
            if (!string.IsNullOrEmpty(artikelName))
            {
                System.Collections.ObjectModel.Collection<Artikel> artikelListe = _bestellservice.SucheArtikelByTitel(artikelName, checkboxBuch, checkboxBlueRay);
                ViewData["artikel"] = artikelListe;
            }
            return View();
        }

        public IActionResult Artikeldetails(int artikelnummer)
        {
            Artikel artikel = _bestellservice.GetArtikelByArtikelnummer(artikelnummer);
            artikel.Anzahl = 1;
            return View(artikel);
        }

        public IActionResult ArtikelInWarenkorb(int artikelnummer)
        {
            // bekomme von der View nur die Artikelnummer und nicht den gesamten Artikel, 
            // da der Wert "Preis" nicht als decimalstelle übergeben wird.
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            Artikel artikel = _bestellservice.GetArtikelByArtikelnummer(artikelnummer);
            _bestellservice.AddArtikelToWarenkorb(artikel.Artikelnummer, artikel.Anzahl, int.Parse(value));
            return View("Warenkorbansicht", artikel);
        }

        public IActionResult Warenkorbansicht(Artikel artikel)
        {
            
            return View();
        }

        public IActionResult Kundensuche()
        {
            return View();
        }

        public IActionResult Bestellstatistik()
        {
            return View();
        }
    }
}
