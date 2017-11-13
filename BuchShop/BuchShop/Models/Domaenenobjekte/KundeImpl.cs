namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public partial class Kunde : Nutzer
	{
        public void SetKundenstatus(string kundenstatus)
        {
            Kundenstatus normal = new NormalerKunde();
            Kundenstatus premium = new PremiumKunde();
            Kundenstatus gesperrt = new GesperrterKunde();
            Kundenstatus vip = new VipKunde();

            if (kundenstatus == normal.ToString())
            {
                Status = normal;
            }
            else if (kundenstatus == premium.ToString())
            {
                Status = premium;
            }
            else if (kundenstatus == gesperrt.ToString())
            {
                Status = gesperrt;
            }
            else if (kundenstatus == vip.ToString())
            {
                Status = vip;
            }
        }

        public decimal Mahnen (decimal mahnBetrag)
        {
            return Status.Mahnen(this, mahnBetrag);
        }

        public void TreuepunkteHinzufuegen(decimal rechnungsBetrag)
        {
            Status.TreuepunkteHinzufuegen(this, rechnungsBetrag);
        }

        public void VipUpgrade()
        {
            Status.VipUpgrade(this);
        }

        public void Entsperren()
        {
            Status.Entsperren(this);
        }

	}
}

