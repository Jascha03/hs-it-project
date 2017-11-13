using System;

namespace BuchShop.Datenzugriff
{
    public class RechnungssystemAdapter : IRechnungssystemZugriff
    {
        private RechnungssystemZugriffFake rechnungssystem = new RechnungssystemZugriffFake();
        public void MahnungSenden(decimal betragInEuro, decimal gebuehrInEuro, string name, int postleitzahl, string strasse, int hausnummer)
        {
            rechnungssystem.MahnungSenden((int)((betragInEuro+gebuehrInEuro) * 100), name, postleitzahl, strasse + " " + hausnummer);
        }

        public void RechnungSenden(decimal preisInEuroMitMwst, string name, int postleitzahl, string strasse, int hausnummer, DateTime rechnungsdatum)
        {
            rechnungssystem.RechnungSenden((int)((preisInEuroMitMwst * 100) / 1.19m), name, postleitzahl, strasse + " " + hausnummer, rechnungsdatum.ToShortDateString());
        }

        public decimal GesamtSummeMahnGebuehrenPlusBetraegeInEuro()
        {
            return ((decimal)rechnungssystem.GesamtSummeMahnGebuehrenPlusBetraegeInCent()) / 100;
        }
    }
}