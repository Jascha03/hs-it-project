using System;

namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public class PremiumKunde : Kundenstatus
    {
        private const decimal mahnBetragsGrenze = 100;
        public void Entsperren(Kunde kunde)
        {
        }

        public decimal Mahnen(Kunde kunde, decimal mahnBetrag)
        {
            if (mahnBetrag > mahnBetragsGrenze)
            { 
                kunde.Status = new GesperrterKunde();
            }

            return Math.Min(mahnBetrag/10, 5);
        }

        public void TreuepunkteHinzufuegen(Kunde kunde, decimal rechnungsBetrag)
        {
            kunde.Treuepunkte += 2* (int) rechnungsBetrag;
        }

        public void VipUpgrade(Kunde kunde)
        {
            kunde.Status = new VipKunde();
        }

        public override string ToString()
        {
            return "Premium";
        }
    }
}