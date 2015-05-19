# EnlaceFiscal


## Instalación

Install-Package EnlaceFiscal

## Uso

```cs
var lib = new EnlaceFiscalLib("<RFC emisor>", "<API password>", [true/false]); // Producción = true, Debug = false

var invoice = lib.Invoice("<RFC receptor>", "<serie>", "<folio>")
					.WithPaymentType(PaymentMethod.Cash)
					.WithCurrency(Currency.MXN)
					.WithReceiver("<razon social>", "<calle>", "<num exterior>", "<num interior>", "<colonia>", "<localidad>", "<municipio>", "<estado>", <codigo postal>)
					.WithItems(
						Item.WithDescription("Articulo 1")
						.WithCost(80, true) // true = Incluye impuestos
						.WithQuantity(2)
						.WithSku("ALA")
						.WithUnit("unidad"),
						
						Item.WithDescription("Arreglo floral")
						.WithCost(100) 	// false = No incluye impuestos
						.WithQuantity(1)
						.WithSku("ALA")
						.WithUnit("app"),
						
						Item.WithDescription("Arreglo floral", 0.09M, "ISR") // Impuestos extras
						.WithCost(100)
						.WithQuantity(1)
						.WithSku("ALA")
						.WithUnit("app")
					).Email("<email1>", "<email2>");

await invoice.Send();
```