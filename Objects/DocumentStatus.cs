#region
using bscheiman.Common.Attributes;

#endregion

namespace EnlaceFiscal.Objects {
    public enum DocumentStatus {
        [Description("rechazado")] Rejected,
        [Description("aceptado")] Accepted
    }
}