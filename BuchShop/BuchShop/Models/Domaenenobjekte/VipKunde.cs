namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public class VipKunde : Kundenstatus
    {
        private const decimal mahnBetragsGrenze = 500;
        public void Entsperren(Kunde kunde)
        {
        }

        public decimal Mahnen(Kunde kunde, decimal mahnBetrag)
        {
            if (mahnBetrag > mahnBetragsGrenze)
            { 
                kunde.Status = new PremiumKunde();
            }

            return 0;
        }

        public void TreuepunkteHinzufuegen(Kunde kunde, decimal rechnungsBetrag)
        {
            kunde.Treuepunkte += 3* (int) rechnungsBetrag;
        }

        public void VipUpgrade(Kunde kunde)
        {
        }

        public override string ToString()
        {
            return "Vip";
        }
    }
}