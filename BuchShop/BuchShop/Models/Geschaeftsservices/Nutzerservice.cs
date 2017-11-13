using BuchShop.Geschaeftslogik.Domaenenobjekte;
using BuchShop.Datenzugriff;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using System;
using System.Collections.ObjectModel;

namespace BuchShop.Geschaeftslogik.Geschaeftsservices
{
    public sealed class Nutzerservice : INutzerservice
    {

        private IDatenbankZugriff datenbank;

        public Nutzerservice(IDatenbankZugriff _datenbank)
        {
            datenbank = _datenbank;
        }
        

        public bool PruefePasswort(int identifikationsnummer, string passwort)
        {
            /*
            byte[] data = Encoding.ASCII.GetBytes(passwort);
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] sha1data = sha1.ComputeHash(data);
            ASCIIEncoding ascienc = new ASCIIEncoding();
            string passwortHash = ascienc.GetString(sha1data);
            */

            return passwort == (string) datenbank.GetNutzerDatenByNutzerId(identifikationsnummer)["Passwort"];
        }

        public Nutzer GetNutzerByNutzerId(int identifikationsnummer)
        {
            Nutzer nutzer;
            DataRow nutzerDaten = datenbank.GetNutzerDatenByNutzerId(identifikationsnummer);

            if ((string) nutzerDaten["Nutzertyp"] == Nutzertyp.Kunde.ToString())
            {
                Kunde kunde = new Kunde();
                kunde.Treuepunkte = (int) nutzerDaten["Treuepunkte"];
                string kundenstatus = (string) nutzerDaten["Kundenstatus"];

                kunde.SetKundenstatus(kundenstatus);

                Adresse adresse = new Adresse();
                adresse.Postleitzahl = (int) nutzerDaten["Postleitzahl"];
                adresse.Strasse = (string) nutzerDaten["Strasse"];
                adresse.Hausnummer = (int) nutzerDaten["Hausnummer"];
                kunde.Rechnungsadresse = adresse;

                nutzer = kunde;
            }
            else
            {
                Mitarbeiter mitarbeiter = new Mitarbeiter();
                mitarbeiter.Telefonnummer = (string) nutzerDaten["Telefonnummer"];

                nutzer = mitarbeiter;
            }

            nutzer.Identifikationsnummer = identifikationsnummer;
            nutzer.Email = (string) nutzerDaten["Email"];
            nutzer.Name = (string) nutzerDaten["Name"];
            nutzer.Passwort = (string) nutzerDaten["Passwort"];

            return nutzer;
        }

        public int GetNutzerIdentifikationsnummerByEmail(string email)
        {
            return datenbank.GetNutzerIdentifikationsnummerByEmail(email);
        }

        public Collection<Kunde> SucheKundenByName(string name)
        {
            Collection<Kunde> kundenListe = new Collection<Kunde>();
            Collection<int> kundenIdentifikationsnummern = datenbank.SucheKundenIdentifikationsnummernByName(name);

            foreach (int id in kundenIdentifikationsnummern)
            {
                kundenListe.Add((Kunde)GetNutzerByNutzerId(id));
            }

            return kundenListe;
        }



        public void Entsperren(int kundenIdentifikationsnummer)
        {
            Kunde kunde = (Kunde)GetNutzerByNutzerId(kundenIdentifikationsnummer);
            kunde.Entsperren();
            KundenDatenSpeichern(kunde);
        }

        public void KundenDatenSpeichern(Kunde kunde)
        {
            datenbank.SetKundenStatus(kunde.Identifikationsnummer, kunde.Status.ToString());
            datenbank.SetKundenTreuepunkte(kunde.Identifikationsnummer, kunde.Treuepunkte);
        }

        public decimal WertEinesTreuepunktsInEuro()
        {
            return 0.01m;
        }

        public void VipUpgrade(int kundenIdentifikationsnummer)
        {
            Kunde kunde = (Kunde)GetNutzerByNutzerId(kundenIdentifikationsnummer);
            kunde.VipUpgrade();
            KundenDatenSpeichern(kunde);
        }

        public void SetDatenbankZugriff(IDatenbankZugriff datenbankZugriff)
        {
            datenbank = datenbankZugriff;
        }
    }
}