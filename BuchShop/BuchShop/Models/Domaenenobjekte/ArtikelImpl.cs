namespace BuchShop.Geschaeftslogik.Domaenenobjekte
{
    public partial class Artikel
	{
		public virtual string Typ
		{
			get
            {
                if (this is Buch)
                {
                    return "Buch";
                }
                else if (this is BluRay)
                {
                    return "BluRay";
                }
                else
                {
                    return "";
                }
            }
        }

	}
}

