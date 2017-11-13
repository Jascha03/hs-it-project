using System;
using System.Collections.ObjectModel;
using System.Data;

namespace BuchShop.Datenzugriff
{
    public sealed class DatenbankZugriffFake : IDatenbankZugriff, IDisposable
    {
        private DataTable nutzerTabelle = new DataTable("Nutzer");
        public DatenbankZugriffFake ()
        {         
            nutzerTabelle.Columns.Add(new DataColumn("Identifikationsnummer", typeof(int)));
            nutzerTabelle.Columns.Add(new DataColumn("Email", typeof(string)));
            nutzerTabelle.Columns.Add(new DataColumn("Name", typeof(string)));
            nutzerTabelle.Columns.Add(new DataColumn("Passwort", typeof(string)));
            nutzerTabelle.Columns.Add(new DataColumn("Nutzertyp", typeof(string)));
            nutzerTabelle.Columns.Add(new DataColumn("Treuepunkte", typeof(int)));
            nutzerTabelle.Columns.Add(new DataColumn("Kundenstatus", typeof(string)));
            nutzerTabelle.Columns.Add(new DataColumn("Postleitzahl", typeof(int)));
            nutzerTabelle.Columns.Add(new DataColumn("Strasse", typeof(string)));
            nutzerTabelle.Columns.Add(new DataColumn("Hausnummer", typeof(int)));
            nutzerTabelle.Columns.Add(new DataColumn("Telefonnummer", typeof(string)));

            nutzerTabelle.Rows.Add(1234, "kunde1@hs-weingarten.de", "Karl Kundenmann", "kunde1", Nutzertyp.Kunde, 100, "Normal", 88239, "Kundenstrasse",1, "");
            nutzerTabelle.Rows.Add(2345, "kunde2@hs-weingarten.de", "Kira Kundenfrau", "kunde2", Nutzertyp.Kunde, 1500, "Gesperrt", 88250, "Kundenweg", 5, "");
            nutzerTabelle.Rows.Add(3456, "kunde3@hs-weingarten.de", "Vera Vipkundenfrau", "kunde3", Nutzertyp.Kunde, 10000, "Vip", 88250, "Kundenweg", 6, "");
            nutzerTabelle.Rows.Add(4567, "mitarbeiter1@hs-weingarten.de", "Maria Mitarbeiterfrau", "mitarbeiter1", Nutzertyp.Mitarbeiter, 0, "", 0, "", 0, "0751/123456");
            nutzerTabelle.Rows.Add(6789, "kunde4@hs-weingarten.de", "Karla Kundenmann", "kunde4", Nutzertyp.Kunde, 10000, "Premium", 88239, "Kundenstrasse", 1, "");
        }

        public void Dispose()
        {
            nutzerTabelle.Dispose();
        }

        public DataRow GetNutzerDatenByNutzerId(int identifikationsnummer)
        {
            DataRow [] nutzerDaten = nutzerTabelle.Select("Identifikationsnummer = '" + identifikationsnummer + "'");
            DataRow nutzer = nutzerTabelle.NewRow();
            nutzer.ItemArray = nutzerDaten[0].ItemArray.Clone() as object[];

            return nutzer;
        }

        public int GetNutzerIdentifikationsnummerByEmail(string email)
        {
            DataRow[] nutzerDaten = nutzerTabelle.Select("Email = '" + email + "'");

            if (nutzerDaten.Length == 0)
            { 
                return 0;
            }
            else
            { 
                return (int) nutzerDaten[0]["Identifikationsnummer"];
            }
        }

        public void SetKundenStatus(int identifikationsnummer, string status)
        {
            DataRow[] nutzerDaten = nutzerTabelle.Select("Identifikationsnummer = '" + identifikationsnummer + "'");
            nutzerDaten[0]["Kundenstatus"] = status;
        }

        public void SetKundenTreuepunkte(int identifikationsnummer, int treuepunkte)
        {
            DataRow[] nutzerDaten = nutzerTabelle.Select("Identifikationsnummer = '" + identifikationsnummer + "'");
            nutzerDaten[0]["Treuepunkte"] = treuepunkte;
        }

        public Collection<int> SucheKundenIdentifikationsnummernByName(string name)
        {
            Collection<int> listeMitIdentifikationsnummern = new Collection<int>();
            DataRow[] nutzerDaten = nutzerTabelle.Select("Nutzertyp = '" + Nutzertyp.Kunde + "' AND Name LIKE '%" + name + "%'");

            foreach (DataRow nutzer in nutzerDaten)
            {
                listeMitIdentifikationsnummern.Add((int) nutzer["Identifikationsnummer"]);
            }

            return listeMitIdentifikationsnummern;
        }
    }

}