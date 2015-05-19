#region
using bscheiman.Common.Attributes;

#endregion

namespace EnlaceFiscal.Objects {
    public enum PaymentMethod {
        [Description("No identificado")] Unidentified,
        [Description("Efectivo")] Cash,
        [Description("Transferencia Electrónica")] ElectronicTransfer,
        [Description("Cheque")] Cheque,
        [Description("Tarjeta de Crédito")] CreditCard,
        [Description("Tarjeta de Débito")] DebitCard,
        [Description("Tarjeta de Servicios")] ServicesCard
    }
}