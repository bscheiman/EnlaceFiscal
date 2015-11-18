#region
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using EnlaceFiscal.Extensions;
using NodaTime;

#endregion

namespace EnlaceFiscal.Objects {
    public class Invoice {
        internal Currency Currency { get; set; }
        internal string Design { get; set; }
        internal string[] Emails { get; set; }
        public string Id { get; set; }
        internal List<Item> Items { get; set; }
        internal EnlaceFiscalLib Library { get; set; }
        internal PaymentInformation PaymentInformation { get; set; }
        internal bool Production { get; set; }
        internal Receiver Receiver { get; set; }
        internal string Series { get; set; }

        public decimal Subtotal {
            get { return Items.Sum(a => a.Total); }
        }

        internal string Target { get; set; }

        public decimal Total {
            get { return Items.Sum(a => a.TotalWithTax); }
        }

        internal string Username { get; set; }

        internal Invoice(EnlaceFiscalLib library, string username, string target, string series, string id, bool production = true) {
            Library = library;
            Username = username;
            Target = target;
            Series = series;
            Id = id;
            Production = production;
            Items = new List<Item>();
        }

        public void AddItem(params Item[] items) {
            if (items == null)
                return;

            Items.AddRange(items);
        }

        public Invoice Email(params string[] emails) {
            Emails = emails;

            return this;
        }

        public Task<InvoiceResult> Send() {
            return Library.Send(ToXml());
        }

        public XDocument ToXml() {
            var requiredObjects = new object[] { PaymentInformation, Items, Receiver, Series, Id };

            if (requiredObjects.Any(x => x == null))
                throw new ArgumentException("Faltan algunos objetos en la factura");

            if (Items.Count == 0)
                throw new ArgumentException("No hay articulos que facturar");

            var date = SystemClock.Instance.Now.InZone(DateTimeZoneProviders.Tzdb["America/Mexico_City"]) - Duration.FromMinutes(10);

            var root =
                XDocument.Parse(
                                @"<CFDi xsi:schemaLocation=""https://esquemas.enlacefiscal.com/EF/ https://esquemas.enlacefiscal.com/EF/EFv5_0.xsd"" xmlns=""https://esquemas.enlacefiscal.com/EF/EFv5_0.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" />");

            root.AddWithDefaultNamespace("modo", Production ? "produccion" : "debug");
            root.AddWithDefaultNamespace("versionEF", "5.0");
            root.AddWithDefaultNamespace("serie", Series);
            root.AddWithDefaultNamespace("folioInterno", Id);
            root.AddWithDefaultNamespace("fechaEmision", date.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
            root.AddWithDefaultNamespace("subTotal", Subtotal.ToString("0.000000").RemoveTrailingZeroes());
            root.AddWithDefaultNamespace("total", Total.ToString("0.000000").RemoveTrailingZeroes());
            root.AddWithDefaultNamespace("numeroDecimales", 2);
            root.AddWithDefaultNamespace("tipoMoneda", Currency.ToString());

            if (Currency != Currency.MXN)
                root.AddWithDefaultNamespace("tipoCambio", 0.ToString("0.000000"));

            if (!string.IsNullOrEmpty(Design))
                root.AddWithDefaultNamespace("nombreDiseno", Design);

            root.AddWithDefaultNamespace("rfc", Username);
            root.AddWithDefaultNamespace("DatosDePago", PaymentInformation.ToXml());

            root.AddWithDefaultNamespace("Receptor", Receiver.ToXml());

            root.AddWithDefaultNamespace("Partidas", Items.Select(a => a.ToXml()).ToArray());
            root.AddWithDefaultNamespace("Impuestos", new Tax(Items).ToXml());

            if (Emails != null && Emails.Length > 0)
                root.AddWithDefaultNamespace("EnviarCFDi", Emails.Select(a => XmlHelper.CreateElement("correo", a)));

            root.AddWithDefaultNamespace("softwareIntegrador", "EnlaceFiscal.NET");

            return root;
        }

        public Invoice WithCurrency(Currency curr) {
            Currency = curr;

            return this;
        }

        public Invoice WithDesign(string design) {
            if (!string.IsNullOrEmpty(design))
                Design = design;

            return this;
        }

        public Invoice WithItems(params Item[] items) {
            AddItem(items);

            return this;
        }

        public Invoice WithPaymentType(PaymentMethod method, string description = "Pago en una sola exhibición", string account = "") {
            PaymentInformation = new PaymentInformation(method, description, account);

            return this;
        }

        public Invoice WithReceiver(string socialName, string address, string extNumber, string intNumber, string neighborhood,
                                    string locality, string county, string state, int zipCode) {
            Receiver = new Receiver(Target, socialName, address, extNumber, intNumber, neighborhood, locality, county, state, zipCode);

            return this;
        }

        public Invoice WithReceiver(string socialName, string address, string extNumber, string neighborhood, string locality, string county,
                                    string state, int zipCode) {
            Receiver = new Receiver(Target, socialName, address, extNumber, string.Empty, neighborhood, locality, county, state, zipCode);

            return this;
        }
    }
}
