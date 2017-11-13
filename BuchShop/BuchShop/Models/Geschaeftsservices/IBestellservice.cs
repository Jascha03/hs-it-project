using BuchShop.Geschaeftslogik.Domaenenobjekte;
using BuchShop.Datenzugriff;
using System.Collections.ObjectModel;

namespace BuchShop.Geschaeftslogik.Geschaeftsservices
{
    public interface IBestellservice
    {
        Artikel GetArtikelByArtikelnummer(int artikelnummer);
        Collection<Artikel> SucheArtikelByTitel(string titel, bool buecher, bool bluRays);
        Warenkorb GetWarenkorbByKundenId(int kundenIdentifikationsnummer);
        void AddArtikelToWarenkorb(int artikelnummer, int anzahl, int kundenIdentifikationsnummer);
        void UpdateArtikelanzahlInWarenkorb(int kundenIdentifikationsnummer, int artikelnummer, int anzahl);
        bool Bestellen(int kundenIdentifikationsnummer, int treuepunkte);
        decimal Versandkosten();
        decimal GetWarenkorbRabattByKundenId(int kundenIdentifikationsnummer);
        decimal GetWarenkorbGesamtpreisByKundenId(int kundenIdentifikationsnummer);
        int GetWarenkorbArtikelanzahlByKundenId(int kundenIdentifikationsnummer);
        void RabattcodeSpeichern(int kundenIdentifikationsnummer, string rabattcode);
        int AnzahlUnterschiedlicheArtikelInLogistiksystem();
        int AnzahlBestellungen();
        decimal DurchschnittlicheAnzahlArtikelProBestellung();
        void Mahnen(int kundenIdentifikationsnummer, decimal mahnBetrag);
        decimal GesamtSummeMahnGebuehrenPlusBetraege();
        void SetLogistiksystemZugriff(ILogistiksystemZugriff logistiksystemZugriff);
        void SetRechnungssystemZugriff(IRechnungssystemZugriff rechnungssystemZugriff);
        void SetNutzerservice(INutzerservice service);
    }
}
