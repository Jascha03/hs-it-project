using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace BuchShop.Datenzugriff
{
    public sealed class LogistiksystemZugriffFake : ILogistiksystemZugriff, IDisposable
    {
        private DataTable artikelTabelle = new DataTable("Artikel");
        private Collection<Bestellung> bestellungsListe = new Collection<Bestellung>();

        public LogistiksystemZugriffFake()
        {
            artikelTabelle.Columns.Add(new DataColumn("Artikelnummer", typeof(int)));
            artikelTabelle.Columns.Add(new DataColumn("Titel", typeof(string)));
            artikelTabelle.Columns.Add(new DataColumn("Preis", typeof(decimal)));
            artikelTabelle.Columns.Add(new DataColumn("Lieferbar", typeof(bool)));
            artikelTabelle.Columns.Add(new DataColumn("Artikeltyp", typeof(string)));
            artikelTabelle.Columns.Add(new DataColumn("Autor", typeof(string)));
            artikelTabelle.Columns.Add(new DataColumn("Zusammenfassung", typeof(string)));
            artikelTabelle.Columns.Add(new DataColumn("Regisseur", typeof(string)));
            artikelTabelle.Columns.Add(new DataColumn("TrailerLink", typeof(string)));

            artikelTabelle.Rows.Add(1000, "Software Engineering", 59.95m , true, Artikeltyp.Buch , "Ian Sommerville", "Die 9. Auflage des Klassikers Software Engineering von Ian Sommerville wurde aktualisiert und um zahlreiche neue Inhalte erweitert, wie z.B. agile Softwareentwicklung, eingebettete Systeme, modelgetriebene Entwicklung, Open-Source-Entwicklung, testgetriebene Entwicklung, serviceorientierte Entwicklung und vieles mehr.", "","");
            artikelTabelle.Rows.Add(1001, "Softwaretechnik", 49.95m, true, Artikeltyp.Buch, "Thomas Grechenig", "Das vorliegende Buch ist eine realitätsnahe Einführung in die Softwaretechnik. Es erläutert, wie man Softwareprojekte methodisch ohne größere Risiken zum Erfolg führt. Es vermittelt das Grundgerüst für den Bau solider Softwaresysteme, liefert eine ausgewogene Gesamtprojektsicht und stellt eine Guideline dar, wie man moderne Software-Entwicklungsprojekte mit den Aufwandsgrößen von 12 Personenmonaten (z.B. ein Web-Shop für regionale Produkte) über 120 (z.B. die betriebliche Verwaltung eines Mittel­ständers) bis 1200 (z.B. Sparbuch- und Anlageverwaltung einer großen Bank) nach dem technischen State-of-the-Art angemessen ausführt.", "", "");
            artikelTabelle.Rows.Add(1002, "Der Pate", 9.99m, true, Artikeltyp.BluRay, "", "", "Francis Ford Coppola", @"http://www.imdb.com/video/imdb/vi1348706585");
            artikelTabelle.Rows.Add(1003, "Der Pate 2", 9.99m, true, Artikeltyp.BluRay, "", "", "Francis Ford Coppola", @"http://www.imdb.com/video/imdb/vi696162841");
            artikelTabelle.Rows.Add(1004, "Die Verurteilten", 14.99m, true, Artikeltyp.BluRay, "", "", "Frank Darabont", @"http://www.imdb.com/video/imdb/vi3877612057");
            artikelTabelle.Rows.Add(1005, "Der Pate", 59.95m, true, Artikeltyp.Buch, "Mario Puzo", "Der kleine Vito entkommt als einziger einem Massaker in seinem Heimatort auf Sizilien. Er flieht nach New York und wird als Erwachsener zum gefürchteten Paten der amerikanischen Mafia. Aber ihn beherrscht nur ein Gedanke: Er will den Mord an seiner Familie rächen.", "", "");
            artikelTabelle.Rows.Add(1006, "The Dark Knight", 7.99m, false, Artikeltyp.BluRay, "", "", "Christopher Nolan", @"http://www.imdb.com/video/imdb/vi324468761");
        }

        public int AnzahlBestellungen()
        {
            return bestellungsListe.Count;
        }

        public int AnzahlUnterschiedlicheArtikel()
        {
            return artikelTabelle.Rows.Count;
        }

        public int AnzahlVerkaufteArtikel()
        {
            int anzahl = 0;
            foreach(Bestellung bestellung in bestellungsListe)
            {
                foreach (int artikelAnzahl in bestellung.ArtikelnummernMitAnzahl.Values)
                { 
                    anzahl = anzahl + artikelAnzahl;
                }
            }

            return anzahl;
        }

        public void BestellungVersenden(Dictionary<int, int> artikelnummernMitAnzahl, string name, int postleitzahl, string strasse, int hausnummer)
        {
            Bestellung bestellung = new Bestellung(artikelnummernMitAnzahl, name, postleitzahl, strasse, hausnummer);
            bestellungsListe.Add(bestellung);
            Console.WriteLine("Bestellung: " + bestellung.ToString());
        }

        public void Dispose()
        {
            artikelTabelle.Dispose();
        }

        public DataRow GetArtikelDatenByArtikelnummer(int artikelnummer)
        {
            DataRow[] artikelDaten = artikelTabelle.Select("Artikelnummer = '" + artikelnummer + "'");
            DataRow artikel = artikelTabelle.NewRow();
            artikel.ItemArray = artikelDaten[0].ItemArray.Clone() as object[];

            return artikel;
        }

        public Collection<int> SucheArtikelnummernByTitel(string titel)
        {
            string selectQuery = "Titel LIKE '%" + titel + "%'";

            return getArtikelnummernByQuery(selectQuery);
        }

        public Collection<int> SucheBluRayArtikelnummernByTitel(string titel)
        {
            string selectQuery = "Artikeltyp = '" + Artikeltyp.BluRay + "' AND Titel LIKE '%" + titel + "%'";

            return getArtikelnummernByQuery(selectQuery);
        }

        public Collection<int> SucheBuchArtikelnummernByTitel(string titel)
        {
            string selectQuery = "Artikeltyp = '" + Artikeltyp.Buch + "' AND Titel LIKE '%" + titel + "%'";

            return getArtikelnummernByQuery(selectQuery);
        }

        private Collection<int> getArtikelnummernByQuery(string selectQuery)
        {
            Collection<int> listeMitArtikelnummern = new Collection<int>();
            DataRow[] artikelDaten = artikelTabelle.Select(selectQuery+ " AND Lieferbar = 'true'");

            foreach (DataRow artikel in artikelDaten)
            {
                listeMitArtikelnummern.Add((int)artikel["Artikelnummer"]);
            }

            return listeMitArtikelnummern;
        }

        class Bestellung
        {

            public Bestellung(Dictionary<int, int> artikelnummernMitAnzahl, string name, int postleitzahl, string strasse, int hausnummer)
            {
                ArtikelnummernMitAnzahl = artikelnummernMitAnzahl;
                Name = name;
                Postleitzahl = postleitzahl;
                Strasse = strasse;
                Hausnummer = hausnummer;
            }

            public Dictionary<int, int> ArtikelnummernMitAnzahl
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
            public int Postleitzahl
            {
                get;
                set;
            }

            public string Strasse
            {
                get;
                set;
            }

            public int Hausnummer
            {
                get;
                set;
            }

            public override string ToString()
            {
                string ergebnis = "";
                foreach (KeyValuePair<int, int> kvp in ArtikelnummernMitAnzahl)
                {
                    ergebnis = ergebnis + kvp.Key + " " + kvp.Value + " ";
                }
                return ergebnis + Name + " " + Postleitzahl + " " + Strasse + " " + Hausnummer;
            }

        }
    }
}