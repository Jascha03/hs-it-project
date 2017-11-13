namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public interface RabattStrategie
    {
        decimal RabattBerechnen(string rabattcode, decimal summeArtikelPreise, decimal versandkosten);
    }
}
