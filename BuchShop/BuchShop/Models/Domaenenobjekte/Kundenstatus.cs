namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public interface Kundenstatus
    {
        decimal Mahnen (Kunde kunde, decimal mahnBetrag);

        void TreuepunkteHinzufuegen (Kunde kunde, decimal rechnungsBetrag);

        void Entsperren(Kunde kunde);

        void VipUpgrade(Kunde kunde);

        string ToString();

    }
}
