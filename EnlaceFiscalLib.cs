#region
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EnlaceFiscal.Objects;

#endregion

namespace EnlaceFiscal {
    public class EnlaceFiscalLib {
        internal string Password { get; set; }
        internal bool Production { get; set; }
        internal string Username { get; set; }

        public EnlaceFiscalLib(string username, string password, bool production = true) {
            Username = username;
            Password = password;
            Production = production;
        }

        public void CancelInvoice(string id, string reason) {
            string tempStr =
                $@"<?xml version=""1.0"" encoding=""UTF-8""?><Solicitud xsi:schemaLocation=""https://esquemas.enlacefiscal.com/EF/API_CFDi/Solicitudes https://esquemas.enlacefiscal.com/EF/API_CFDi/Solicitud.xsd""  xmlns=""https://esquemas.enlacefiscal.com/EF/API_CFDi/Solicitudes"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><rfc>SERB860630BF2</rfc><accion>cancelarCFDi</accion><modo>produccion</modo><CFDi><serie>A</serie><folio>{
                    id}</folio><justificacion>{reason}</justificacion></CFDi></Solicitud>";

            // TODO
        }

        public Invoice Invoice(string target, string series, string id) {
            return Invoice(target, series, id, DateTime.UtcNow);
        }

        public Invoice Invoice(string target, string series, string id, DateTime date) {
            return new Invoice(this, Username, target, series, id, Production);
        }

        internal async Task<InvoiceResult> Send(XDocument invoice) {
            using (var client = new HttpClient()) {
                string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));
                string xml = invoice.ToString(SaveOptions.DisableFormatting);

                client.DefaultRequestHeaders.Add("SOAPAction", "/generarCFDi");
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + auth);

                string str =
                    $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?><SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:wsdl=""http://schemas.xmlsoap.org/wsdl/"" xmlns:soap=""http://schemas.xmlsoap.org/wsdl/soap/"" xmlns:http=""http://schemas.xmlsoap.org/wsdl/http/"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenc=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:mime=""http://schemas.xmlsoap.org/wsdl/mime/"" xmlns:tns=""http://api.enlacefiscal.namespace"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" ><SOAP-ENV:Body><generarFolio xmlns=""http://api.enlacefiscal.namespace"">{
                        WebUtility.HtmlEncode(xml)}</generarFolio></SOAP-ENV:Body></SOAP-ENV:Envelope>";
                var content = new StringContent(str, Encoding.UTF8, "text/xml");

                using (var response = await client.PostAsync("https://api.enlacefiscal.com/soap/serviceCFDi.php", content)) {
                    string soapResponse = await response.Content.ReadAsStringAsync();
                    var soap = XDocument.Parse(soapResponse);
                    XNamespace ns = "http://schemas.xmlsoap.org/soap/envelope/";

                    return new InvoiceResult(XDocument.Parse(soap.Root.Element(ns + "Body").Element("generarResult").Value).Root);
                }
            }
        }
    }
}
