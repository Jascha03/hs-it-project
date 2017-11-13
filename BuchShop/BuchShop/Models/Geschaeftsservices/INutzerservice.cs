using BuchShop.Geschaeftslogik.Domaenenobjekte;
using BuchShop.Datenzugriff;
using System.Collections.ObjectModel;

namespace BuchShop.Geschaeftslogik.Geschaeftsservices
{
    public interface INutzerservice
    {
        int GetNutzerIdentifikationsnummerByEmail(string email);
        Nutzer GetNutzerByNutzerId(int identifikationsnummer);
        bool PruefePasswort(int identifikationsnummer, string passwort);
        Collection<Kunde> SucheKundenByName(string name);
        void Entsperren(int kundenIdentifikationsnummer);
        void VipUpgrade(int kundenIdentifikationsnummer);
        void KundenDatenSpeichern(Kunde kunde);
        decimal WertEinesTreuepunktsInEuro();
        void SetDatenbankZugriff(IDatenbankZugriff datenbankZugriff);
    }
}
