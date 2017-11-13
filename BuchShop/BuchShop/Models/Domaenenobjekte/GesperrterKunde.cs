namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public class GesperrterKunde : Kundenstatus
    {
        public void Entsperren(Kunde kunde)
        {
            kunde.Treuepunkte = 0;
            kunde.Status = new NormalerKunde();
        }

        public decimal Mahnen(Kunde kunde, decimal mahnBetrag)
        {
            return mahnBetrag/10;
        }

        public void TreuepunkteHinzufuegen(Kunde kunde, decimal rechnungsBetrag)
        {
        }

        public void VipUpgrade(Kunde kunde)
        {
        }

        public override string ToString()
        {
            return "Gesperrt";
        }
    }
}