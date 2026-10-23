CREATE DATABASE InventarioDb;
GO

USE InventarioDb;
GO

CREATE TABLE Productos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL,
    CONSTRAINT CK_Productos_Precio CHECK (Precio >= 0),
    CONSTRAINT CK_Productos_Stock CHECK (Stock >= 0)
);


CREATE TABLE Pedidos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FechaPedido DATETIME2 NOT NULL,
    Total DECIMAL(18,2) NOT NULL, check (Total >= 0),
    Estado NVARCHAR(30) NOT NULL
);

ALTER TABLE dbo.Pedidos
ADD CONSTRAINT DF_Pedidos_FechaPedido
DEFAULT SYSUTCDATETIME() FOR FechaPedido;


USE InventarioDb;
GO

CREATE TABLE dbo.PedidoDetalles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PedidoId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,

    CONSTRAINT CK_PedidoDetalles_Cantidad
        CHECK (Cantidad > 0),

    CONSTRAINT CK_PedidoDetalles_PrecioUnitario
        CHECK (PrecioUnitario >= 0),

    CONSTRAINT FK_PedidoDetalles_Pedidos
        FOREIGN KEY (PedidoId) REFERENCES dbo.Pedidos(Id),

    CONSTRAINT FK_PedidoDetalles_Productos
        FOREIGN KEY (ProductoId) REFERENCES dbo.Productos(Id)
);

USE InventarioDb;
GO

BEGIN TRANSACTION;

DECLARE @ProductosNuevos TABLE (
    Id INT,
    Nombre NVARCHAR(150)
);

DECLARE @PedidosNuevos TABLE (
    Id INT,
    FechaPedido DATETIME2
);

-- 10 productos diferentes
INSERT INTO dbo.Productos (Nombre, Precio, Stock)
OUTPUT inserted.Id, inserted.Nombre
INTO @ProductosNuevos (Id, Nombre)
VALUES
    (N'Teclado mecánico', 10.00, 19),
    (N'Mouse inalámbrico', 20.00, 19),
    (N'Monitor de 24 pulgadas', 30.00, 19),
    (N'Auriculares Bluetooth', 40.00, 19),
    (N'Cámara web Full HD', 50.00, 19),
    (N'Disco SSD de 1 TB', 60.00, 19),
    (N'Memoria RAM de 16 GB', 70.00, 19),
    (N'Impresora láser', 80.00, 19),
    (N'Router Wi-Fi 6', 90.00, 19),
    (N'Silla ergonómica', 100.00, 19);

-- 10 pedidos
INSERT INTO dbo.Pedidos (FechaPedido, Total, Estado)
OUTPUT inserted.Id, inserted.FechaPedido
INTO @PedidosNuevos (Id, FechaPedido)
VALUES
    ('2026-10-01T12:00:00', 10.00, N'Completado'),
    ('2026-10-02T12:00:00', 20.00, N'Completado'),
    ('2026-10-03T12:00:00', 30.00, N'Completado'),
    ('2026-10-04T12:00:00', 40.00, N'Completado'),
    ('2026-10-05T12:00:00', 50.00, N'Completado'),
    ('2026-10-06T12:00:00', 60.00, N'Completado'),
    ('2026-10-07T12:00:00', 70.00, N'Completado'),
    ('2026-10-08T12:00:00', 80.00, N'Completado'),
    ('2026-10-09T12:00:00', 90.00, N'Completado'),
    ('2026-10-10T12:00:00', 100.00, N'Completado');

-- Un detalle por cada pedido
INSERT INTO dbo.PedidoDetalles
    (PedidoId, ProductoId, Cantidad, PrecioUnitario)
SELECT
    pedido.Id,
    producto.Id,
    1,
    productoTabla.Precio
FROM (VALUES
    ('2026-10-01T12:00:00', N'Teclado mecánico'),
    ('2026-10-02T12:00:00', N'Mouse inalámbrico'),
    ('2026-10-03T12:00:00', N'Monitor de 24 pulgadas'),
    ('2026-10-04T12:00:00', N'Auriculares Bluetooth'),
    ('2026-10-05T12:00:00', N'Cámara web Full HD'),
    ('2026-10-06T12:00:00', N'Disco SSD de 1 TB'),
    ('2026-10-07T12:00:00', N'Memoria RAM de 16 GB'),
    ('2026-10-08T12:00:00', N'Impresora láser'),
    ('2026-10-09T12:00:00', N'Router Wi-Fi 6'),
    ('2026-10-10T12:00:00', N'Silla ergonómica')
) AS datos(FechaPedido, NombreProducto)
JOIN @PedidosNuevos AS pedido
    ON pedido.FechaPedido = datos.FechaPedido
JOIN @ProductosNuevos AS producto
    ON producto.Nombre = datos.NombreProducto
JOIN dbo.Productos AS productoTabla
    ON productoTabla.Id = producto.Id;

COMMIT TRANSACTION;

SELECT 'Productos' AS Tabla, COUNT(*) AS Total FROM dbo.Productos
UNION ALL
SELECT 'Pedidos', COUNT(*) FROM dbo.Pedidos
UNION ALL
SELECT 'PedidoDetalles', COUNT(*) FROM dbo.PedidoDetalles;