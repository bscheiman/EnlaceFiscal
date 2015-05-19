#region
using System.Collections.Generic;
using System.Xml.Linq;
using bscheiman.Common.Extensions;
using EnlaceFiscal.Extensions;
using EnlaceFiscal.Interfaces;

#endregion

namespace EnlaceFiscal.Objects {
    public class PaymentInformation : IConvertsToXml {
        public string PaymentAccount { get; set; }
        public string PaymentDescription { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public PaymentInformation(PaymentMethod paymentMethod, string paymentDescription, string paymentAccount) {
            PaymentMethod = paymentMethod;
            PaymentDescription = paymentDescription;
            PaymentAccount = paymentAccount;
        }

        public XElement[] ToXml() {
            var list = new List<XElement> {
                XmlHelper.CreateElement("metodoDePago", PaymentMethod.GetDescription()), 
                XmlHelper.CreateElement("numeroCuentaPago", PaymentAccount),
                XmlHelper.CreateElement("formaDePago", PaymentDescription)
            };

            if (string.IsNullOrEmpty(PaymentAccount))
                list.RemoveAt(1);

            return list.ToArray();
        }
    }
}