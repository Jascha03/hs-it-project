using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BuchShop.Geschaeftslogik.Domaenenobjekte;
using BuchShop.Geschaeftslogik.Geschaeftsservices;

namespace BuchShop.Controllers
{
    public class KundenController : Controller
    {
        // private readonly INutzerservice _nutzerservice;
        private IBestellservice _bestellservice;
        private INutzerservice _nutzerservice;
        public KundenController(
            IBestellservice bestellservice, INutzerservice nutzerservice)
        {
            _bestellservice = bestellservice;
            _nutzerservice = nutzerservice;
        }

        public IActionResult Artikelsuche(string artikelName, bool checkboxBuch, bool checkboxBlueRay)
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                return RedirectToAction("Login", "Benutzer");
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
                return RedirectToAction("Login", "Benutzer");
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
                return RedirectToAction("Login", "Benutzer");
            }
            Artikel artikel = _bestellservice.GetArtikelByArtikelnummer(artikelnummer);
            _bestellservice.AddArtikelToWarenkorb(artikel.Artikelnummer, 1, int.Parse(value));

            return RedirectToAction("Warenkorbansicht", "Kunden");
        }


        public IActionResult Warenkorbansicht(bool Rabattcode = false, bool NutzerValid = true)
        {
            ViewData["Rabattcode"] = Rabattcode;

            var value = HttpContext.Session.GetString("Identifikationsnummer");
            if (string.IsNullOrEmpty(value))
            {
                return RedirectToAction("Login", "Benutzer");
            }
            Nutzer nutzer = _nutzerservice.GetNutzerByNutzerId(int.Parse(value));
            Warenkorb warenkorb = _bestellservice.GetWarenkorbByKundenId((int.Parse(value)));
            if (!NutzerValid)
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
                return RedirectToAction("Startseite", "BuchShop");
            }
            else
            {
                return RedirectToAction("Warenkorbansicht", "Kunden", new { NutzerValid = false });
            }
        }
        public IActionResult RabattcodeSpeichern(int Rabattcode)
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            _bestellservice.RabattcodeSpeichern(int.Parse(value), Rabattcode.ToString());
            return RedirectToAction("Warenkorbansicht", "Kunden", new { Rabattcode = true });
        }


        public IActionResult Bestellen(int treuepunkte)
        {
            var value = HttpContext.Session.GetString("Identifikationsnummer");
            bool erfolgreich = _bestellservice.Bestellen(int.Parse(value), treuepunkte);

            if (erfolgreich)
            {
                return RedirectToAction("Startseite", "BuchShop");
            }
            else
            {
                ModelState.AddModelError("Gesperrt", "Gesperrter Account - Bestellen nicht möglicht");
                return RedirectToAction("Warenkorbansicht", "Kunden");
            }
        }
    }
}