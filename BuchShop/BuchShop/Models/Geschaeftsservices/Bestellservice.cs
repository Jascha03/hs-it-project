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
        private readonly INutzerservice nutzerservice;

        

        private ILogistiksystemZugriff logistiksystem;
        private IRechnungssystemZugriff rechnungssystem;
        
        private Dictionary<int, Warenkorb> warenkorbDictionary = new Dictionary<int, Warenkorb>();
        
        public Bestellservice(INutzerservice _nutzerservice)
        {
            nutzerservice = _nutzerservice;
            Collection<Kunde> alleKunden = nutzerservice.SucheKundenByName("");

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
            return logistiksystem.AnzahlBestellungen();
        }

        public int AnzahlUnterschiedlicheArtikelInLogistiksystem()
        {
            return logistiksystem.AnzahlUnterschiedlicheArtikel();
        }

        public decimal DurchschnittlicheAnzahlArtikelProBestellung()
        {
            int anzahlBestellungen = logistiksystem.AnzahlBestellungen();

            if (anzahlBestellungen != 0)
            {
                return Math.Round((decimal)logistiksystem.AnzahlVerkaufteArtikel() / anzahlBestellungen, 2);
            }
            else
            { 
                return 0;
            }
        }

        public bool Bestellen(int kundenIdentifikationsnummer, int treuepunkte)
        {
            Kunde kunde = (Kunde) nutzerservice.GetNutzerByNutzerId(kundenIdentifikationsnummer);
            Warenkorb warenkorb = GetWarenkorbByKundenId(kundenIdentifikationsnummer);

            if (!(kunde.Status is GesperrterKunde) && ((Collection<Artikel>) warenkorb.Artikel).Count  > 0)
            {                
                Dictionary<int, int> artikelnummernMitAnzahl = new Dictionary<int, int>();

                foreach (Artikel artikel in warenkorb.Artikel)
                {
                    artikelnummernMitAnzahl.Add(artikel.Artikelnummer, artikel.Anzahl);
                }

                logistiksystem.BestellungVersenden(artikelnummernMitAnzahl, kunde.Name, kunde.Rechnungsadresse.Postleitzahl, kunde.Rechnungsadresse.Strasse, kunde.Rechnungsadresse.Hausnummer);

                if (treuepunkte < 0)
                {
                    treuepunkte = 0;
                }

                treuepunkte = Math.Min(treuepunkte, Math.Min(kunde.Treuepunkte, (int) (warenkorb.GesamtPreis() / nutzerservice.WertEinesTreuepunktsInEuro())));

                kunde.Treuepunkte -= treuepunkte;
                decimal rechnungsBetrag = warenkorb.GesamtPreis() - nutzerservice.WertEinesTreuepunktsInEuro() * treuepunkte;
                rechnungssystem.RechnungSenden(rechnungsBetrag, kunde.Name, kunde.Rechnungsadresse.Postleitzahl, kunde.Rechnungsadresse.Strasse, kunde.Rechnungsadresse.Hausnummer, DateTime.Now);
                kunde.TreuepunkteHinzufuegen(rechnungsBetrag);
                nutzerservice.KundenDatenSpeichern(kunde);
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
            DataRow artikelDaten = logistiksystem.GetArtikelDatenByArtikelnummer(artikelnummer);
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
                artikelnummern = logistiksystem.SucheArtikelnummernByTitel(titel);
            }
            else if (buecher)
            { 
                artikelnummern = logistiksystem.SucheBuchArtikelnummernByTitel(titel);
            }
            else if (bluRays)
            { 
                artikelnummern = logistiksystem.SucheBluRayArtikelnummernByTitel(titel);
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
            Kunde kunde = (Kunde)nutzerservice.GetNutzerByNutzerId(kundenIdentifikationsnummer);
            decimal mahnGebuehr = kunde.Mahnen(mahnBetrag);
            rechnungssystem.MahnungSenden(mahnBetrag, mahnGebuehr, kunde.Name, kunde.Rechnungsadresse.Postleitzahl, kunde.Rechnungsadresse.Strasse, kunde.Rechnungsadresse.Hausnummer);
            nutzerservice.KundenDatenSpeichern(kunde);
        }

        public decimal GesamtSummeMahnGebuehrenPlusBetraege()
        {
            return Math.Round(rechnungssystem.GesamtSummeMahnGebuehrenPlusBetraegeInEuro(), 2);
        }

        public void SetLogistiksystemZugriff(ILogistiksystemZugriff logistiksystemZugriff)
        {
            logistiksystem = logistiksystemZugriff;
        }

        public void SetRechnungssystemZugriff(IRechnungssystemZugriff rechnungssystemZugriff)
        {
            rechnungssystem = rechnungssystemZugriff;
        }

        public void SetNutzerservice(INutzerservice service)
        {
            //nutzerservice = service;
        }

    }
}