#region
using bscheiman.Common.Attributes;

#endregion

namespace EnlaceFiscal.Objects {
    public enum InvoiceStatus {
        Unknown,
        [Description("vigente")] Ok,
        [Description("cancelado")] Cancelled
    }
}