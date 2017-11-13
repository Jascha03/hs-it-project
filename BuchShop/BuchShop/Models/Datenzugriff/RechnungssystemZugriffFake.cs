using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;

namespace BuchShop.Datenzugriff
{
    public class RechnungssystemZugriffFake
    {
        private Collection<Rechnung> rechnungsListe = new Collection<Rechnung>();
        private Collection<Mahnung> mahnungsListe = new Collection<Mahnung>();

       public  void RechnungSenden(int preisInCentOhneMwst, string name, int postleitzahl, string strasseUndHausnummer, string rechnungsdatum)
        {
            Rechnung rechnung = new Rechnung(preisInCentOhneMwst, name, postleitzahl, strasseUndHausnummer, rechnungsdatum);
            rechnungsListe.Add(rechnung);
            Console.WriteLine("Rechnung: " + rechnung.ToString());
        }

        public void MahnungSenden(int betragPlusGebuehrInCent, string name, int postleitzahl, string strasseUndHausnummer)
        {
            Mahnung mahnung = new Mahnung(betragPlusGebuehrInCent, name, postleitzahl, strasseUndHausnummer);
            mahnungsListe.Add(mahnung);
            Console.WriteLine("Mahnung: " + mahnung.ToString());
        }

        public int GesamtSummeMahnGebuehrenPlusBetraegeInCent()
        {
            int summe = 0;

            foreach(Mahnung mahnung in mahnungsListe)
            {
                summe = summe + mahnung.BetragPlusGebuehr;
            }

            return summe;
        }

        class Rechnung
        {
            public Rechnung(int preisInCentOhneMwst, string name, int postleitzahl, string strasseUndHausnummer, string rechnungsdatum)
            {
                Preis = preisInCentOhneMwst;
                Name = name;
                Postleitzahl = postleitzahl;
                Strasse = strasseUndHausnummer;
                Rechnungsdatum = rechnungsdatum;
            }

            public int Preis
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
            public string Rechnungsdatum
            {
                get;
                set;
            }
            public override string ToString()
            {
                return Preis + " " + Name + " " + Postleitzahl + " " + Strasse + " " + Rechnungsdatum;
            }

        }

        class Mahnung
        {
            public Mahnung(int betragPlusGebuehrInCent, string name, int postleitzahl, string strasseUndHausnummer)
            {
                BetragPlusGebuehr = betragPlusGebuehrInCent;
                Name = name;
                Postleitzahl = postleitzahl;
                Strasse = strasseUndHausnummer;
            }

            public int BetragPlusGebuehr
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

            public override string ToString()
            {
                return BetragPlusGebuehr + " " + Name + " " + Postleitzahl + " " + Strasse;
            }
        }
    }


}