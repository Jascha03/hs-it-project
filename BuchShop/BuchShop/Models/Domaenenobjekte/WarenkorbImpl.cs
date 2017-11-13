using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public partial class Warenkorb
	{
        private RabattStrategie rabattStrategie;
		public void Hinzufuegen (Artikel artikel)
        {
            Collection<Artikel> artikelListe = (Collection<Artikel>)this.Artikel;
            Artikel listenArtikel = artikelListe.ToList().Find(x => x.Artikelnummer == artikel.Artikelnummer);

            if (listenArtikel == null)
            {
                if(artikel.Anzahl > 0)
                { 
                    artikelListe.Add(artikel);
                }
            }
            else
            {
                listenArtikel.Anzahl = listenArtikel.Anzahl + artikel.Anzahl;
                if (listenArtikel.Anzahl <= 0)
                { 
                    artikelListe.Remove(listenArtikel);
                }
            }
        }

        public void Leeren()
        {
            Collection<Artikel> artikelListe = (Collection<Artikel>)this.Artikel;
            artikelListe.Clear();
        }

        public void AnzahlAktualisieren(int artikelnummer, int anzahl)
        {
            Collection<Artikel> artikelListe = (Collection<Artikel>)this.Artikel;
            Artikel listenArtikel = artikelListe.ToList().Find(x => x.Artikelnummer == artikelnummer);

            if (listenArtikel != null)
            {
                if (anzahl > 0)
                { 
                    listenArtikel.Anzahl = anzahl;
                }
                else
                { 
                    artikelListe.Remove(listenArtikel);
                }
            }
        }

        private decimal SummeArtikelPreise()
        {
            decimal preis = 0;

            foreach (Artikel artikel in Artikel)
            {
                preis += artikel.Anzahl * artikel.Preis;
            }

            return Math.Round(preis, 2);
        }

        public int Artikelanzahl()
        {
            int anzahl = 0;

            foreach (Artikel artikel in Artikel)
            {
                anzahl += artikel.Anzahl;
            }

            return anzahl;
        }

        public decimal Rabatt()
        {
            if (rabattStrategie != null)
            {
                return rabattStrategie.RabattBerechnen(Rabattcode, SummeArtikelPreise(), Versandkosten());
            }
            else
            {
                return 0;
            }
        }

        public void SetRabattStrategie(RabattStrategie strategie)
        {
            rabattStrategie = strategie;
        }

        public decimal GesamtPreis()
        {
              return Math.Round(SummeArtikelPreise() - Rabatt() + Versandkosten(), 2);
        }

        public static decimal Versandkosten()
        {
            return Warenkorb.versandkosten;
        }

    }
}

