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
            {
                if (nutzer.Identifikationsnummer != 0)
                {
                    HttpContext.Session.SetString(
                        "Identifikationsnummer", nutzer.Identifikationsnummer.ToString());
                    return RedirectToAction("Startseite", true);
                }
            }
            ModelState.AddModelError("Ungültig", "Email und/oder Passwort falsch");
            return View();
        }

        public IActionResult Startseite()
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                return RedirectToAction("Login", true);
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
                RedirectToAction("Login", true);
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
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                return RedirectToAction("Login", true);
            }
            Artikel artikel = _bestellservice.GetArtikelByArtikelnummer(artikelnummer);
            return View(artikel);
        }

        public IActionResult ArtikelInWarenkorb(int artikelnummer)
        {
            // bekomme von der View nur die Artikelnummer und nicht den gesamten Artikel, 
            // da der Wert "Preis" nicht als decimalstelle übergeben wird.
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                return RedirectToAction("Login", true);
            }
            Artikel artikel = _bestellservice.GetArtikelByArtikelnummer(artikelnummer);
            _bestellservice.AddArtikelToWarenkorb(artikel.Artikelnummer, 1, int.Parse(value));

            return RedirectToAction("Warenkorbansicht", true);
        }
        
        public IActionResult RabattcodeSpeichern(int Rabattcode)
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            _bestellservice.RabattcodeSpeichern(int.Parse(value), Rabattcode.ToString());
            return RedirectToAction("Warenkorbansicht", new { Rabattcode = true });
        }

        public IActionResult Warenkorbansicht(bool Rabattcode = false, bool NutzerValid = true)
        {
            ViewData["Rabattcode"] = Rabattcode;
            
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                Response.Redirect("Login", true);
            }
            Nutzer nutzer = _nutzerservice.GetNutzerByNutzerId(int.Parse(value));
            Warenkorb warenkorb = _bestellservice.GetWarenkorbByKundenId((int.Parse(value)));
            if(!NutzerValid)
            {
                ModelState.AddModelError("Gesperrt", "Gesperrter Account - Bestellen nicht möglicht");
            }
            ViewData["Nutzer"] = nutzer;
            ViewData["Warenkorb"] = warenkorb;
            ViewData["Rabatwert"] = _bestellservice.GetWarenkorbRabattByKundenId(int.Parse(value));
            ViewData["Versandkosten"] = _bestellservice.Versandkosten();
            ViewData["Preiswert"] = _bestellservice.GetWarenkorbGesamtpreisByKundenId(int.Parse(value));
            ViewData["TreuepunktWert"] = _nutzerservice.WertEinesTreuepunktsInEuro();
            ViewBag.A = "B";
            return View();      
        }

        [HttpPost]
        public IActionResult Warenkorbansicht(int treuepunkte)
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            bool erfolgreich = _bestellservice.Bestellen(int.Parse(value), treuepunkte);

            if (erfolgreich)
            {
                return RedirectToAction("Startseite", true);
            }
            else
            {
                return RedirectToAction("Warenkorbansicht", new { NutzerValid = false });
            }
        }

        public void Bestellen(int treuepunkte)
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            bool erfolgreich = _bestellservice.Bestellen(int.Parse(value), treuepunkte);

            if (erfolgreich)
            {
                Response.Redirect("Startseite", true);
            }
            else
            {
                ModelState.AddModelError("Gesperrt", "Gesperrter Account - Bestellen nicht möglicht");
                Response.Redirect("Warenkorbansicht", true);
                //TODO: error message
            }
        }
         // Mitarbeiter Loggin

        public IActionResult Kundensuche()
        { 
            return View();
        }

        [HttpPost]
        public IActionResult Kundensuche(string kundenName)
        {
            System.Collections.ObjectModel.Collection<Kunde> kundenListe = _nutzerservice.SucheKundenByName(kundenName);
            ViewData["Kunden"] = kundenListe;
            return View();
        }

        public IActionResult Kundendetails(int id)
        {
            Kunde kunde = (Kunde)_nutzerservice.GetNutzerByNutzerId(id);
            return View(kunde);
        }
        public ActionResult Mahnen(int id, decimal mahnbetrag)
        {
            _bestellservice.Mahnen(id, mahnbetrag);
            return Json("Kunden erfolgreich Ermahnt");
        }
        public ActionResult Entsperren(int id, decimal mahnbetrag)
        {
            _nutzerservice.Entsperren(id);
            return Json("Kunden erfolgreich Entsperrt");
        }
        public ActionResult VipUgrade(int id, decimal mahnbetrag)
        {
            _nutzerservice.VipUpgrade(id);
            return Json("Kunden erfolgreich zum Vip befördert");
        }

        public IActionResult Bestellstatistik()
        {
            ViewData["Artikelanzahl"] = _bestellservice.AnzahlUnterschiedlicheArtikelInLogistiksystem();
            ViewData["Bestellungsanzahl"] = _bestellservice.AnzahlBestellungen();
            ViewData["ArtikelanzahlProBestellung"] = _bestellservice.DurchschnittlicheAnzahlArtikelProBestellung();
            ViewData["GesamteMahngebuehren"] = _bestellservice.GesamtSummeMahnGebuehrenPlusBetraege();

            return View();
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.SetString(
                "Identifikationsnummer", "0");
            return RedirectToAction("Login", true);
        }
    }
}
