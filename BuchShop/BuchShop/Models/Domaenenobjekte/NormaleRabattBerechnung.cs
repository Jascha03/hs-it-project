using System;

namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public class NormaleRabattBerechnung : RabattStrategie
    {
        public decimal RabattBerechnen(string rabattcode, decimal summeArtikelPreise, decimal versandkosten)
        {
            if (rabattcode == "rabatt10")
            {
                return Math.Round(0.1m * summeArtikelPreise, 2);
            }
            else if (rabattcode == "rabattV")
            {
                return versandkosten;
            }
            else
            {
                return 0;
            }
        }
    }
}