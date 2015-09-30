#region
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EnlaceFiscal.Extensions;
using EnlaceFiscal.Interfaces;

#endregion

namespace EnlaceFiscal.Objects {
    public class Tax : IConvertsToXml {
        internal List<Item> Items { get; set; }

        internal Tax(List<Item> items) {
            Items = items;
        }

        public XElement[] ToXml() {
            var list = new List<XElement>();

            decimal totalIsr = Items.Where(a => a.TaxType == "ISR").Sum(a => a.TotalTax);
            decimal totalIva = Items.Where(a => a.TaxType == "IVA").Sum(a => a.TotalTax);

            if (totalIsr > 0)
                list.Add(XmlHelper.CreateElement("totalImpuestosRetenidos", totalIsr.ToString("0.000000").RemoveTrailingZeroes()));
            if (totalIva > 0)
                list.Add(XmlHelper.CreateElement("totalImpuestosTrasladados", totalIva.ToString("0.000000").RemoveTrailingZeroes()));

            var isr = GetTaxes("Retencion", "ISR");
            var iva = GetTaxes("Traslado", "IVA");

            if (isr != null && isr.Any())
                list.Add(XmlHelper.CreateElement("Retenciones", isr));

            if (iva != null && iva.Any())
                list.Add(XmlHelper.CreateElement("Traslados", iva));

            return list.ToArray();
        }

        private IEnumerable<XElement[]> GetTaxes(string name, string tax) {
            return Items.Where(a => a.TaxType == tax).GroupBy(a => a.TaxType + ":" + a.Tax, a => a, (key, g) => new {
                Tax = key,
                Items = g.ToList()
            }).Select(n => {
                var first = n.Items.First();

                return new[] {
                    XmlHelper.CreateElement(name, XmlHelper.CreateElement("impuesto", first.TaxType),
                        XmlHelper.CreateElement("tasa", (first.Tax * 100).ToString("0.000000").RemoveTrailingZeroes()),
                        XmlHelper.CreateElement("importe", n.Items.Sum(a => a.TotalTax).ToString("0.000").RemoveTrailingZeroes()))
                };
            });
        }
    }
}
