using System.Collections.ObjectModel;
using System.Data;

namespace BuchShop.Datenzugriff
{
    public interface IDatenbankZugriff
    {

        int GetNutzerIdentifikationsnummerByEmail(string email);

        DataRow GetNutzerDatenByNutzerId(int identifikationsnummer);

        void SetKundenTreuepunkte(int identifikationsnummer, int treuepunkte);

        void SetKundenStatus(int identifikationsnummer, string status);

        Collection<int> SucheKundenIdentifikationsnummernByName(string name);

    }

    public enum Nutzertyp { Kunde, Mitarbeiter }

}
