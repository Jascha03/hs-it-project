using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace BuchShop.Datenzugriff
{
    public interface ILogistiksystemZugriff
    {
        DataRow GetArtikelDatenByArtikelnummer(int artikelnummer);

        Collection<int> SucheArtikelnummernByTitel(string titel);

        Collection<int> SucheBuchArtikelnummernByTitel(string titel);

        Collection<int> SucheBluRayArtikelnummernByTitel(string titel);

        void BestellungVersenden(Dictionary<int,int> artikelnummernMitAnzahl, string name, int postleitzahl, string strasse, int hausnummer);

        int AnzahlUnterschiedlicheArtikel();

        int AnzahlBestellungen();

        int AnzahlVerkaufteArtikel();
    }

    public enum Artikeltyp { Buch, BluRay }
}
