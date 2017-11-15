using System;
using System.Collections.Generic;
using BuchShop.Geschaeftslogik.Domaenenobjekte;
using BuchShop.Datenzugriff;
using System.Data;
using System.Collections.ObjectModel;

namespace BuchShop.Geschaeftslogik.Geschaeftsservices
{
    public sealed class Bestellservice : IBestellservice
    {
        private readonly INutzerservice _nutzerservice;

        

        private readonly ILogistiksystemZugriff _logistiksystem;
        private readonly IRechnungssystemZugriff _rechnungssystem;
        
        private Dictionary<int, Warenkorb> warenkorbDictionary = new Dictionary<int, Warenkorb>();
        
        public Bestellservice(
            INutzerservice nutzerservice, ILogistiksystemZugriff logistiksystem, 
            IRechnungssystemZugriff rechnungssystem)
        {
            _nutzerservice = nutzerservice;
            _logistiksystem = logistiksystem;
            _rechnungssystem = rechnungssystem;

            Collection<Kunde> alleKunden = _nutzerservice.SucheKundenByName("");

            foreach(Kunde kunde in alleKunden)
            {
                Warenkorb warenkorb = new Warenkorb();
                warenkorb.Rabattcode = null;
                warenkorb.Artikel = new Collection<Artikel>();
                warenkorb.Kunde = kunde;

                if (warenkorb.Kunde.Rechnungsadresse.Postleitzahl == 88250)
                {
                    warenkorb.SetRabattStrategie(new WeingartenRabattBerechnung());
                }
                else
                {
                    warenkorb.SetRabattStrategie(new NormaleRabattBerechnung());
                }

                warenkorbDictionary.Add(kunde.Identifikationsnummer, warenkorb);
            }

        }
        

        public void AddArtikelToWarenkorb(int artikelnummer, int anzahl, int kundenIdentifikationsnummer)
        {
            Warenkorb warenkorb = GetWarenkorbByKundenId(kundenIdentifikationsnummer);
            Artikel artikel = GetArtikelByArtikelnummer(artikelnummer);
            artikel.Anzahl = anzahl;
            warenkorb.Hinzufuegen(artikel);
        }

        public int AnzahlBestellungen()
        {
            return _logistiksystem.AnzahlBestellungen();
        }

        public int AnzahlUnterschiedlicheArtikelInLogistiksystem()
        {
            return _logistiksystem.AnzahlUnterschiedlicheArtikel();
        }

        public decimal DurchschnittlicheAnzahlArtikelProBestellung()
        {
            int anzahlBestellungen = _logistiksystem.AnzahlBestellungen();

            if (anzahlBestellungen != 0)
            {
                return Math.Round((decimal)_logistiksystem.AnzahlVerkaufteArtikel() / anzahlBestellungen, 2);
            }
            else
            { 
                return 0;
            }
        }

        public bool Bestellen(int kundenIdentifikationsnummer, int treuepunkte)
        {
            Kunde kunde = (Kunde) _nutzerservice.GetNutzerByNutzerId(kundenIdentifikationsnummer);
            Warenkorb warenkorb = GetWarenkorbByKundenId(kundenIdentifikationsnummer);

            if (!(kunde.Status is GesperrterKunde) && ((Collection<Artikel>) warenkorb.Artikel).Count  > 0)
            {                
                Dictionary<int, int> artikelnummernMitAnzahl = new Dictionary<int, int>();

                foreach (Artikel artikel in warenkorb.Artikel)
                {
                    artikelnummernMitAnzahl.Add(artikel.Artikelnummer, artikel.Anzahl);
                }

                _logistiksystem.BestellungVersenden(artikelnummernMitAnzahl, kunde.Name, kunde.Rechnungsadresse.Postleitzahl, kunde.Rechnungsadresse.Strasse, kunde.Rechnungsadresse.Hausnummer);

                if (treuepunkte < 0)
                {
                    treuepunkte = 0;
                }

                treuepunkte = Math.Min(treuepunkte, Math.Min(kunde.Treuepunkte, (int) (warenkorb.GesamtPreis() / _nutzerservice.WertEinesTreuepunktsInEuro())));

                kunde.Treuepunkte -= treuepunkte;
                decimal rechnungsBetrag = warenkorb.GesamtPreis() - _nutzerservice.WertEinesTreuepunktsInEuro() * treuepunkte;
                _rechnungssystem.RechnungSenden(rechnungsBetrag, kunde.Name, kunde.Rechnungsadresse.Postleitzahl, kunde.Rechnungsadresse.Strasse, kunde.Rechnungsadresse.Hausnummer, DateTime.Now);
                kunde.TreuepunkteHinzufuegen(rechnungsBetrag);
                _nutzerservice.KundenDatenSpeichern(kunde);
                warenkorb.Leeren();

                return true;
            }
            else
            { 
                return false;
            }
        }

        public Artikel GetArtikelByArtikelnummer(int artikelnummer)
        {
            Artikel artikel;
            DataRow artikelDaten = _logistiksystem.GetArtikelDatenByArtikelnummer(artikelnummer);
            if ((string)artikelDaten["Artikeltyp"] == Artikeltyp.Buch.ToString())
            {
                Buch buch = new Buch();
                buch.Autor = (string)artikelDaten["Autor"];
                buch.Zusammenfassung = (string)artikelDaten["Zusammenfassung"];

                artikel = buch;
            }
            else
            {
                BluRay bluRay = new BluRay();
                bluRay.Regisseur = (string)artikelDaten["Regisseur"];
                bluRay.TrailerLink = (string)artikelDaten["TrailerLink"];

                artikel = bluRay;
            }

            artikel.Artikelnummer = artikelnummer;
            artikel.Titel = (string)artikelDaten["Titel"];
            artikel.Preis = (decimal)artikelDaten["Preis"];
            artikel.Lieferbar = (bool)artikelDaten["Lieferbar"];

            return artikel;
        }

        public decimal GetWarenkorbGesamtpreisByKundenId(int kundenIdentifikationsnummer)
        {
            return GetWarenkorbByKundenId(kundenIdentifikationsnummer).GesamtPreis();
        }

        public decimal GetWarenkorbRabattByKundenId(int kundenIdentifikationsnummer)
        {
            return GetWarenkorbByKundenId(kundenIdentifikationsnummer).Rabatt();
        }

        public int GetWarenkorbArtikelanzahlByKundenId(int kundenIdentifikationsnummer)
        {
            return GetWarenkorbByKundenId(kundenIdentifikationsnummer).Artikelanzahl();
        }

        public decimal Versandkosten()
        {
            return Warenkorb.Versandkosten();
        }

        public Warenkorb GetWarenkorbByKundenId(int kundenIdentifikationsnummer)
        {
            return warenkorbDictionary[kundenIdentifikationsnummer];
        }

        public void RabattcodeSpeichern(int kundenIdentifikationsnummer, string rabattcode)
        {
            GetWarenkorbByKundenId(kundenIdentifikationsnummer).Rabattcode = rabattcode;
        }

        public Collection<Artikel> SucheArtikelByTitel(string titel, bool buecher, bool bluRays)
        {
            Collection<Artikel> artikelListe = new Collection<Artikel>();
            Collection<int> artikelnummern = new Collection<int>();

            if (buecher && bluRays)
            { 
                artikelnummern = _logistiksystem.SucheArtikelnummernByTitel(titel);
            }
            else if (buecher)
            { 
                artikelnummern = _logistiksystem.SucheBuchArtikelnummernByTitel(titel);
            }
            else if (bluRays)
            { 
                artikelnummern = _logistiksystem.SucheBluRayArtikelnummernByTitel(titel);
            }

            foreach (int id in artikelnummern)
            {
                artikelListe.Add(GetArtikelByArtikelnummer(id));
            }

            return artikelListe;
        }

        public void UpdateArtikelanzahlInWarenkorb(int kundenIdentifikationsnummer, int artikelnummer, int anzahl)
        {
            Warenkorb warenkorb = GetWarenkorbByKundenId(kundenIdentifikationsnummer);
            warenkorb.AnzahlAktualisieren(artikelnummer, anzahl);
        }

        public void Mahnen(int kundenIdentifikationsnummer, decimal mahnBetrag)
        {
            Kunde kunde = (Kunde)_nutzerservice.GetNutzerByNutzerId(kundenIdentifikationsnummer);
            decimal mahnGebuehr = kunde.Mahnen(mahnBetrag);
            _rechnungssystem.MahnungSenden(mahnBetrag, mahnGebuehr, kunde.Name, kunde.Rechnungsadresse.Postleitzahl, kunde.Rechnungsadresse.Strasse, kunde.Rechnungsadresse.Hausnummer);
            _nutzerservice.KundenDatenSpeichern(kunde);
        }

        public decimal GesamtSummeMahnGebuehrenPlusBetraege()
        {
            return Math.Round(_rechnungssystem.GesamtSummeMahnGebuehrenPlusBetraegeInEuro(), 2);
        }

        public void SetLogistiksystemZugriff(ILogistiksystemZugriff logistiksystemZugriff)
        {
            //_logistiksystem = logistiksystemZugriff;
        }

        public void SetRechnungssystemZugriff(IRechnungssystemZugriff rechnungssystemZugriff)
        {
           // _rechnungssystem = rechnungssystemZugriff;
        }

        public void SetNutzerservice(INutzerservice service)
        {
            //nutzerservice = service;
        }

    }
}