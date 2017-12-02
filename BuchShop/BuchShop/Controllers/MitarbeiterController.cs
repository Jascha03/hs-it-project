using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BuchShop.Geschaeftslogik.Domaenenobjekte;
using BuchShop.Geschaeftslogik.Geschaeftsservices;

namespace BuchShop.Controllers
{
    public class MitarbeiterController : Controller
    {

    private IBestellservice _bestellservice;
    private INutzerservice _nutzerservice;
    public MitarbeiterController(
        IBestellservice bestellservice, INutzerservice nutzerservice)
    {
        _bestellservice = bestellservice;
        _nutzerservice = nutzerservice;
    }

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
    }
}