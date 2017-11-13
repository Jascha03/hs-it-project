using System;

namespace BuchShop.Datenzugriff
{
    public interface IRechnungssystemZugriff
    {
        void RechnungSenden(decimal preisInEuroMitMwst, string name, int postleitzahl, string strasse, int hausnummer, DateTime rechnungsdatum);

        void MahnungSenden(decimal betragInEuro, decimal gebuehrInEuro, string name, int postleitzahl, string strasse, int hausnummer);

        decimal GesamtSummeMahnGebuehrenPlusBetraegeInEuro();
    }
}
