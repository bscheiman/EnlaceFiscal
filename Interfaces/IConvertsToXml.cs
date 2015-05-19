#region
using System.Xml.Linq;

#endregion

namespace EnlaceFiscal.Interfaces {
    public interface IConvertsToXml {
        XElement[] ToXml();
    }
}