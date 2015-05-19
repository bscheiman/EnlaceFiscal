#region
using System.Xml.Linq;
using EnlaceFiscal.Interfaces;

#endregion

namespace EnlaceFiscal.Extensions {
    public static class XmlExtensions {
        public static void AddWithDefaultNamespace(this XDocument root, string name, object obj) {
            root.Root.Add(XmlHelper.CreateElement(name, obj));
        }

        public static void AddWithDefaultNamespace(this XDocument root, string name, IConvertsToXml obj) {
            root.Root.Add(XmlHelper.CreateElement(name, obj.ToXml()));
        }
    }

    public static class XmlHelper {
        private static readonly XNamespace Namespace = "https://esquemas.enlacefiscal.com/EF/EFv5_0.xsd";

        public static XElement CreateElement(string name, params object[] val) {
            return new XElement(Namespace + name, val);
        }
    }
}