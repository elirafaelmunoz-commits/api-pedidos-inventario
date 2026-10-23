# API de procesamiento de pedidos

API desarrollada con .NET 8, Entity Framework Core y SQL Server.

## Arquitectura

- `Controllers`: recibe las solicitudes HTTP y devuelve las respuestas.
- `Dtos`: define los datos que puede enviar el cliente.
- `Services`: contiene la lógica para crear pedidos y reservar stock.
- `Data`: contiene el `DbContext` para acceder a SQL Server.
- `Models`: representa las tablas de la base de datos.
- `Exceptions`: contiene la excepción para stock insuficiente.

## Configuración

1. Ejecutar `database.sql` en SQL Server para crear `InventarioDb`, las tablas y los datos de prueba.
2. Configurar `ConnectionStrings:InventarioDb` en `Inventario.Api/appsettings.Development.json`.
3. Desde la carpeta `Inventario.Api`, ejecutar `dotnet restore` y `dotnet run`.

La configuración local utiliza autenticación de Windows con `Server=localhost`.

## Endpoints

- `GET /api/productos`: consulta los productos y su stock.
- `POST /api/pedidos`: crea un pedido con uno o varios productos.

Los ejemplos de solicitudes están en `Inventario.Api/Inventario.Api.http`.

## Consistencia del inventario

La creación del pedido ocurre dentro de una transacción. El stock se descuenta
solo si la cantidad disponible sigue siendo suficiente. Si falla un producto,
se revierten todos los cambios y la API devuelve 400 con los IDs rechazados.
El total y los precios se calculan en el servidor usando los valores de la base de datos.
