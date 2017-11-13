namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public class NormalerKunde : Kundenstatus
    {
        private const int premiumTreuepunkte = 1000;
        public void Entsperren(Kunde kunde)
        {
        }

        public decimal Mahnen(Kunde kunde, decimal mahnBetrag)
        {
            kunde.Status = new GesperrterKunde();

            return mahnBetrag/10;
        }

        public void TreuepunkteHinzufuegen(Kunde kunde, decimal rechnungsBetrag)
        {
            kunde.Treuepunkte += (int) rechnungsBetrag;
            if (kunde.Treuepunkte > premiumTreuepunkte)
            { 
                kunde.Status = new PremiumKunde();
            }
        }

        public void VipUpgrade(Kunde kunde)
        {
        }

        public override string ToString()
        {
            return "Normal";
        }
    }
}