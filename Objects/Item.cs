#region
using System;
using System.Linq;
using System.Xml.Linq;
using EnlaceFiscal.Extensions;
using EnlaceFiscal.Interfaces;

#endregion

namespace EnlaceFiscal.Objects {
    public class Item : IConvertsToXml {
        internal string Description { get; set; }
        internal decimal PerUnit { get; set; }
        internal decimal Quantity { get; set; }
        internal string Sku { get; set; }
        internal decimal Tax { get; set; }
        internal string TaxType { get; set; }

        internal decimal Total {
            get { return Quantity * PerUnit; }
        }

        internal decimal TotalTax {
            get { return Total * Tax; }
        }

        internal decimal TotalWithTax {
            get { return Total + TotalTax; }
        }

        internal string Unit { get; set; }

        internal Item() {
        }

        /*<Descuento>
            <porcentajeDescuento>5</porcentajeDescuento>
            <montoDescuento>52.50</montoDescuento>
            <importeConDescuento>997.50</importeConDescuento>
        </Descuento>*/

        public XElement[] ToXml() {
            var required = new[] {
                Description, Sku, Unit
            };
            var requiredDecimals = new[] {
                PerUnit, Quantity, Total
            };

            if (required.Any(a => a == null) || requiredDecimals.Any(a => a <= 0M))
                throw new ArgumentException("Artículo con valores inválidos");

            return new[] {
                XmlHelper.CreateElement("Partida", XmlHelper.CreateElement("cantidad", Quantity.ToString("0.00").RemoveTrailingZeroes()),
                    XmlHelper.CreateElement("unidad", Unit), XmlHelper.CreateElement("noIdentificacion", Sku),
                    XmlHelper.CreateElement("descripcion", Description),
                    XmlHelper.CreateElement("valorUnitario", PerUnit.ToString("0.000000").RemoveTrailingZeroes()),
                    XmlHelper.CreateElement("importe", Total.ToString("0.000000").RemoveTrailingZeroes()))
            };
        }

        public Item TaxExempt() {
            Tax = 0M;
            TaxType = "";

            return this;
        }

        public Item WithCost(decimal cost, bool taxIncluded = false) {
            if (taxIncluded)
                PerUnit = cost / (1 + Tax);
            else
                PerUnit = cost;

            return this;
        }

        public static Item WithDescription(string description, decimal tax = 0.16M, string taxType = "IVA") {
            if (tax < 0M || tax >= 1M)
                throw new ArgumentOutOfRangeException("tax", tax, "Impuesto inválido");

            return new Item {
                Description = description,
                Tax = tax,
                TaxType = taxType.ToUpper()
            };
        }

        public Item WithQuantity(decimal quantity) {
            Quantity = quantity;

            return this;
        }

        public Item WithSku(string sku) {
            Sku = sku;

            return this;
        }

        public Item WithUnit(string unit) {
            Unit = unit;

            return this;
        }
    }
}