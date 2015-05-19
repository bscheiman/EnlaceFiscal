#region
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using bscheiman.Common.Extensions;

#endregion

namespace EnlaceFiscal.Objects {
    public class InvoiceResult {
        public string Certificate { get; internal set; }
        public DateTime Date { get; internal set; }
        public string DigitalSeal { get; internal set; }
        public DocumentStatus DocumentStatus { get; internal set; }
        public int ErrorCode { get; internal set; }
        public string ErrorMessage { get; internal set; }
        public string Id { get; internal set; }
        public string InvoiceSeal { get; internal set; }
        public InvoiceStatus InvoiceStatus { get; internal set; }
        public string InvoiceUrl { get; internal set; }
        public string OriginalString { get; internal set; }
        public byte[] QrCode { get; internal set; }
        public string QrUrl { get; internal set; }
        public int ReferenceNumber { get; internal set; }
        public string SatCertificate { get; internal set; }
        public string Series { get; internal set; }
        public string Uuid { get; internal set; }
        public string Xml { get; internal set; }

        internal InvoiceResult(XElement xml) {
            ReferenceNumber = Convert.ToInt32(xml.Attribute("numeroReferencia").Value);
            DocumentStatus =
                Enum.GetNames(typeof (DocumentStatus))
                    .Select(a => (DocumentStatus) Enum.Parse(typeof (DocumentStatus), a))
                    .First(a => a.GetDescription() == xml.Attribute("estatusDocumento").Value);
            Date = DateTime.ParseExact(xml.Attribute("fechaMensaje").Value, "yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"));

            if (xml.Element("mensajeError") != null) {
                var error = xml.Element("mensajeError");

                ErrorCode = Convert.ToInt32(error.Element("codigoError").Value);
                ErrorMessage = error.Element("descripcionError").Element("texto").Value;
            } else {
                var xmlBytes = Convert.FromBase64String(xml.Element("xmlCFDi").Value);

                Id = xml.Element("folioInterno").Value;
                Uuid = xml.Element("folioFiscalUUID").Value;
                Series = xml.Element("serie").Value;
                Certificate = xml.Element("noSerieCertificado").Value;
                SatCertificate = xml.Element("noSerieCertificadoSAT").Value;
                InvoiceSeal = xml.Element("selloCFDi").Value;
                DigitalSeal = xml.Element("selloSAT").Value;
                OriginalString = xml.Element("cadenaTFD").Value;
                InvoiceStatus =
                    Enum.GetNames(typeof (InvoiceStatus))
                        .Select(a => (InvoiceStatus) Enum.Parse(typeof (InvoiceStatus), a))
                        .First(a => a.GetDescription() == xml.Element("estadoCFDi").Value);
                QrCode = Convert.FromBase64String(xml.Element("archivoQR").Value);
                QrUrl = xml.Element("descargaArchivoQR").Value;
                Xml = Encoding.UTF8.GetString(xmlBytes, 0, xmlBytes.Length);
                InvoiceUrl = xml.Element("descargaXmlCFDi").Value;
            }
        }
    }
}