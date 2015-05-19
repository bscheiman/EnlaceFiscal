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
            return new[] {
                XmlHelper.CreateElement("totalImpuestosRetenidos",
                    Items.Where(a => a.TaxType == "ISR").Sum(a => a.TotalTax).ToString("0.000000").RemoveTrailingZeroes()),
                XmlHelper.CreateElement("totalImpuestosTrasladados",
                    Items.Where(a => a.TaxType == "IVA").Sum(a => a.TotalTax).ToString("0.000000").RemoveTrailingZeroes()),
                XmlHelper.CreateElement("Retenciones", GetTaxes("Retencion", "ISR")), XmlHelper.CreateElement("Traslados", GetTaxes("Traslado", "IVA"))
            };
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