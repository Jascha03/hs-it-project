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
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                return View("Login");
            }
            Artikel artikel = _bestellservice.GetArtikelByArtikelnummer(artikelnummer);
            return View(artikel);
        }

        public void ArtikelInWarenkorb(int artikelnummer, int Rabattcode = 0)
        {
            // bekomme von der View nur die Artikelnummer und nicht den gesamten Artikel, 
            // da der Wert "Preis" nicht als decimalstelle übergeben wird.
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                Response.Redirect("Login", true);
            }
            if (Rabattcode == 0)
            {

                Artikel artikel = _bestellservice.GetArtikelByArtikelnummer(artikelnummer);
                _bestellservice.AddArtikelToWarenkorb(artikel.Artikelnummer, 1, int.Parse(value));
            }
            else
            {
                _bestellservice.RabattcodeSpeichern(int.Parse(value), Rabattcode.ToString());
            }

            Response.Redirect("Warenkorbansicht", true);
        }
        
        public ActionResult RabattcodeSpeichern(int Rabattcode)
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            _bestellservice.RabattcodeSpeichern(int.Parse(value), Rabattcode.ToString());

            try
            {
                return Json(new
                {
                    msg = "Successfully added " + Rabattcode
                });
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult Warenkorbansicht()
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                Response.Redirect("Login", true);
            }
            Nutzer nutzer = _nutzerservice.GetNutzerByNutzerId(int.Parse(value));
            Warenkorb warenkorb = _bestellservice.GetWarenkorbByKundenId((int.Parse(value)));
            ViewData["Nutzer"] = nutzer;
            ViewData["Warenkorb"] = warenkorb;
            ViewData["Rabatwert"] = _bestellservice.GetWarenkorbRabattByKundenId(int.Parse(value));
            ViewData["Versandkosten"] = _bestellservice.Versandkosten();
            ViewData["Preiswert"] = _bestellservice.GetWarenkorbGesamtpreisByKundenId(int.Parse(value));
            ViewData["TreuepunktWert"] = _nutzerservice.WertEinesTreuepunktsInEuro();
            return View();
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
            //TODO Besser mit Ajax 
            _bestellservice.Mahnen(id, mahnbetrag);
            return Json("Kunden erfolgreich Ermahnt");
        }
        public ActionResult Entsperren(int id, decimal mahnbetrag)
        {
            //TODO Besser mit Ajax 
            _nutzerservice.Entsperren(id);
            return Json("Kunden erfolgreich Entsperrt");
        }
        public ActionResult VipUgrade(int id, decimal mahnbetrag)
        {
            //TODO Besser mit Ajax 
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
        
    }
}
