using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public class WeingartenRabattBerechnung : RabattStrategie
    {
        public decimal RabattBerechnen(string rabattcode, decimal summeArtikelPreise, decimal versandkosten)
        {
            if (rabattcode == "rabatt10")
            {
                return Math.Round(0.1m * summeArtikelPreise + versandkosten, 2);
            }
            else if (rabattcode == "rabatt20")
            {
                return Math.Round(0.2m * summeArtikelPreise + versandkosten, 2);
            }
            else
            {
                return versandkosten;
            }
        }
    }
}