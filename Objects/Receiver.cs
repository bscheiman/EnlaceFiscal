#region
using System.Collections.Generic;
using System.Xml.Linq;
using EnlaceFiscal.Extensions;
using EnlaceFiscal.Interfaces;

#endregion

namespace EnlaceFiscal.Objects {
    public class Receiver : IConvertsToXml {
        internal string Address { get; set; }
        internal string County { get; set; }
        internal string ExtNumber { get; set; }
        internal string IntNumber { get; set; }
        internal string Locality { get; set; }
        internal string Neighborhood { get; set; }
        internal string SocialName { get; set; }
        internal string State { get; set; }
        internal string Target { get; set; }
        internal int ZipCode { get; set; }

        public Receiver(string target, string socialName, string address, string extNumber, string intNumber, string neighborhood, string locality,
                        string county, string state, int zipCode) {
            Target = target;
            SocialName = socialName;
            Address = address;
            ExtNumber = extNumber;
            IntNumber = intNumber;
            Neighborhood = neighborhood;
            Locality = locality;
            County = county;
            State = state;
            ZipCode = zipCode;
        }

        public XElement[] ToXml() {
            var list = new List<XElement> {
                XmlHelper.CreateElement("calle", Address),
                XmlHelper.CreateElement("noExterior", ExtNumber),
                XmlHelper.CreateElement("noInterior", IntNumber),
                XmlHelper.CreateElement("colonia", Neighborhood),
                XmlHelper.CreateElement("localidad", Locality),
                XmlHelper.CreateElement("municipio", County),
                XmlHelper.CreateElement("estado", State),
                XmlHelper.CreateElement("pais", "México"),
                XmlHelper.CreateElement("cp", ZipCode.ToString("00000"))
            };

            if (string.IsNullOrEmpty(IntNumber))
                list.RemoveAt(2);

            return new[] {
                XmlHelper.CreateElement("rfc", Target), XmlHelper.CreateElement("nombre", SocialName),
                XmlHelper.CreateElement("DomicilioFiscal", list)
            };
        }
    }
}