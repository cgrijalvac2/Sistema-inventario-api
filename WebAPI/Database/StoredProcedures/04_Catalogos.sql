/* =====================================================================
   04 - CATÁLOGOS: categorías, unidades de medida, almacenes, motivos de ajuste,
        productos, clientes y proveedores
   Todos los "Eliminar" son borrado lógico (Estado = 0)
   ===================================================================== */

-- ---------- CATEGORÍAS ----------

CREATE OR ALTER PROCEDURE sp_Categoria_Listar
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Categoria, NombreCategoria, DescripcionCategoria, Estado
      FROM Categoria
     WHERE @SoloActivos = 0 OR Estado = 1
     ORDER BY NombreCategoria;
END
GO

CREATE OR ALTER PROCEDURE sp_Categoria_Obtener
    @ID_Categoria INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Categoria, NombreCategoria, DescripcionCategoria, Estado
      FROM Categoria
     WHERE ID_Categoria = @ID_Categoria;
END
GO

CREATE OR ALTER PROCEDURE sp_Categoria_Insertar
    @NombreCategoria      NVARCHAR(100),
    @DescripcionCategoria NVARCHAR(255) = NULL,
    @Estado BIT = 1   -- 1 = activo, 0 = inactivo
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ID INT;

    IF EXISTS (SELECT 1 FROM Categoria WHERE NombreCategoria = @NombreCategoria)
        THROW 50001, N'Ya existe una categoría con ese nombre', 1;

    INSERT INTO Categoria (NombreCategoria, DescripcionCategoria, Estado)
    VALUES (@NombreCategoria, @DescripcionCategoria, @Estado);

    SET @ID = SCOPE_IDENTITY();
    EXEC sp_Categoria_Obtener @ID;
END
GO

CREATE OR ALTER PROCEDURE sp_Categoria_Actualizar
    @ID_Categoria         INT,
    @NombreCategoria      NVARCHAR(100),
    @DescripcionCategoria NVARCHAR(255) = NULL,
    @Estado               BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Categoria WHERE ID_Categoria = @ID_Categoria)
        THROW 50404, N'La categoría no existe', 1;

    IF EXISTS (SELECT 1 FROM Categoria WHERE NombreCategoria = @NombreCategoria AND ID_Categoria <> @ID_Categoria)
        THROW 50001, N'Ya existe una categoría con ese nombre', 1;

    UPDATE Categoria
       SET NombreCategoria = @NombreCategoria, DescripcionCategoria = @DescripcionCategoria, Estado = @Estado
     WHERE ID_Categoria = @ID_Categoria;
END
GO

CREATE OR ALTER PROCEDURE sp_Categoria_Eliminar
    @ID_Categoria INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Categoria WHERE ID_Categoria = @ID_Categoria)
        THROW 50404, N'La categoría no existe', 1;

    UPDATE Categoria SET Estado = 0 WHERE ID_Categoria = @ID_Categoria;
END
GO

-- ---------- UNIDADES DE MEDIDA ----------

CREATE OR ALTER PROCEDURE sp_UnidadMedida_Listar
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_UnidadMedida, Nombre, Abreviatura, Estado
      FROM UnidadMedida
     WHERE @SoloActivos = 0 OR Estado = 1
     ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_UnidadMedida_Obtener
    @ID_UnidadMedida INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_UnidadMedida, Nombre, Abreviatura, Estado
      FROM UnidadMedida
     WHERE ID_UnidadMedida = @ID_UnidadMedida;
END
GO

CREATE OR ALTER PROCEDURE sp_UnidadMedida_Insertar
    @Nombre      NVARCHAR(50),
    @Abreviatura NVARCHAR(10),
    @Estado BIT = 1   -- 1 = activo, 0 = inactivo
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ID INT;

    IF EXISTS (SELECT 1 FROM UnidadMedida WHERE Abreviatura = UPPER(@Abreviatura))
        THROW 50001, N'Ya existe una unidad de medida con esa abreviatura', 1;

    INSERT INTO UnidadMedida (Nombre, Abreviatura, Estado)
    VALUES (@Nombre, UPPER(@Abreviatura), @Estado);

    SET @ID = SCOPE_IDENTITY();
    EXEC sp_UnidadMedida_Obtener @ID;
END
GO

CREATE OR ALTER PROCEDURE sp_UnidadMedida_Actualizar
    @ID_UnidadMedida INT,
    @Nombre          NVARCHAR(50),
    @Abreviatura     NVARCHAR(10),
    @Estado          BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM UnidadMedida WHERE ID_UnidadMedida = @ID_UnidadMedida)
        THROW 50404, N'La unidad de medida no existe', 1;

    IF EXISTS (SELECT 1 FROM UnidadMedida WHERE Abreviatura = UPPER(@Abreviatura) AND ID_UnidadMedida <> @ID_UnidadMedida)
        THROW 50001, N'Ya existe una unidad de medida con esa abreviatura', 1;

    UPDATE UnidadMedida
       SET Nombre = @Nombre, Abreviatura = UPPER(@Abreviatura), Estado = @Estado
     WHERE ID_UnidadMedida = @ID_UnidadMedida;
END
GO

CREATE OR ALTER PROCEDURE sp_UnidadMedida_Eliminar
    @ID_UnidadMedida INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM UnidadMedida WHERE ID_UnidadMedida = @ID_UnidadMedida)
        THROW 50404, N'La unidad de medida no existe', 1;

    UPDATE UnidadMedida SET Estado = 0 WHERE ID_UnidadMedida = @ID_UnidadMedida;
END
GO

-- ---------- ALMACENES ----------

CREATE OR ALTER PROCEDURE sp_Almacen_Listar
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Almacen, Codigo, Nombre, Direccion, Descripcion, Estado
      FROM Almacen
     WHERE @SoloActivos = 0 OR Estado = 1
     ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_Almacen_Obtener
    @ID_Almacen INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Almacen, Codigo, Nombre, Direccion, Descripcion, Estado
      FROM Almacen
     WHERE ID_Almacen = @ID_Almacen;
END
GO

CREATE OR ALTER PROCEDURE sp_Almacen_Insertar
    @Codigo      NVARCHAR(20),
    @Nombre      NVARCHAR(100),
    @Direccion   NVARCHAR(255) = NULL,
    @Descripcion NVARCHAR(255) = NULL,
    @Estado BIT = 1   -- 1 = activo, 0 = inactivo
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ID INT;

    IF EXISTS (SELECT 1 FROM Almacen WHERE Codigo = UPPER(@Codigo))
        THROW 50001, N'Ya existe un almacén con ese código', 1;

    INSERT INTO Almacen (Codigo, Nombre, Direccion, Descripcion, Estado)
    VALUES (UPPER(@Codigo), @Nombre, @Direccion, @Descripcion, @Estado);

    SET @ID = SCOPE_IDENTITY();
    EXEC sp_Almacen_Obtener @ID;
END
GO

CREATE OR ALTER PROCEDURE sp_Almacen_Actualizar
    @ID_Almacen  INT,
    @Codigo      NVARCHAR(20),
    @Nombre      NVARCHAR(100),
    @Direccion   NVARCHAR(255) = NULL,
    @Descripcion NVARCHAR(255) = NULL,
    @Estado      BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Almacen WHERE ID_Almacen = @ID_Almacen)
        THROW 50404, N'El almacén no existe', 1;

    IF EXISTS (SELECT 1 FROM Almacen WHERE Codigo = UPPER(@Codigo) AND ID_Almacen <> @ID_Almacen)
        THROW 50001, N'Ya existe un almacén con ese código', 1;

    IF @Estado = 0 AND EXISTS (SELECT 1 FROM InventarioAlmacen WHERE ID_Almacen = @ID_Almacen AND StockActual > 0)
        THROW 50001, N'No se puede desactivar un almacén que todavía tiene existencias', 1;

    UPDATE Almacen
       SET Codigo = UPPER(@Codigo), Nombre = @Nombre, Direccion = @Direccion, Descripcion = @Descripcion, Estado = @Estado
     WHERE ID_Almacen = @ID_Almacen;
END
GO

CREATE OR ALTER PROCEDURE sp_Almacen_Eliminar
    @ID_Almacen INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Almacen WHERE ID_Almacen = @ID_Almacen)
        THROW 50404, N'El almacén no existe', 1;

    IF EXISTS (SELECT 1 FROM InventarioAlmacen WHERE ID_Almacen = @ID_Almacen AND StockActual > 0)
        THROW 50001, N'No se puede desactivar un almacén que todavía tiene existencias', 1;

    UPDATE Almacen SET Estado = 0 WHERE ID_Almacen = @ID_Almacen;
END
GO

-- ---------- MOTIVOS DE AJUSTE ----------

CREATE OR ALTER PROCEDURE sp_MotivoAjuste_Listar
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Motivo, Descripcion, Estado
      FROM MotivoAjuste
     WHERE @SoloActivos = 0 OR Estado = 1
     ORDER BY Descripcion;
END
GO

CREATE OR ALTER PROCEDURE sp_MotivoAjuste_Obtener
    @ID_Motivo INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID_Motivo, Descripcion, Estado FROM MotivoAjuste WHERE ID_Motivo = @ID_Motivo;
END
GO

CREATE OR ALTER PROCEDURE sp_MotivoAjuste_Insertar
    @Descripcion NVARCHAR(100),
    @Estado BIT = 1   -- 1 = activo, 0 = inactivo
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ID INT;

    INSERT INTO MotivoAjuste (Descripcion, Estado) VALUES (@Descripcion, @Estado);

    SET @ID = SCOPE_IDENTITY();
    EXEC sp_MotivoAjuste_Obtener @ID;
END
GO

CREATE OR ALTER PROCEDURE sp_MotivoAjuste_Actualizar
    @ID_Motivo   INT,
    @Descripcion NVARCHAR(100),
    @Estado      BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM MotivoAjuste WHERE ID_Motivo = @ID_Motivo)
        THROW 50404, N'El motivo no existe', 1;

    UPDATE MotivoAjuste SET Descripcion = @Descripcion, Estado = @Estado WHERE ID_Motivo = @ID_Motivo;
END
GO

CREATE OR ALTER PROCEDURE sp_MotivoAjuste_Eliminar
    @ID_Motivo INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM MotivoAjuste WHERE ID_Motivo = @ID_Motivo)
        THROW 50404, N'El motivo no existe', 1;

    UPDATE MotivoAjuste SET Estado = 0 WHERE ID_Motivo = @ID_Motivo;
END
GO

-- ---------- PRODUCTOS ----------

CREATE OR ALTER PROCEDURE sp_Producto_Listar
    @Buscar       NVARCHAR(100) = NULL,
    @ID_Categoria INT = NULL,
    @SoloActivos  BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.ID_Producto, p.CodigoProducto, p.NombreProducto, p.Descripcion,
           p.ID_Categoria, c.NombreCategoria, p.ID_UnidadMedida, u.Abreviatura AS UnidadMedida,
           p.Costo, p.Precio,
           ISNULL((SELECT SUM(i.StockActual) FROM InventarioAlmacen i WHERE i.ID_Producto = p.ID_Producto), 0) AS StockTotal,
           p.Estado
      FROM Producto p
      JOIN Categoria c ON c.ID_Categoria = p.ID_Categoria
      JOIN UnidadMedida u ON u.ID_UnidadMedida = p.ID_UnidadMedida
     WHERE (@Buscar IS NULL OR p.CodigoProducto LIKE CONCAT(N'%', @Buscar, N'%') OR p.NombreProducto LIKE CONCAT(N'%', @Buscar, N'%'))
       AND (@ID_Categoria IS NULL OR p.ID_Categoria = @ID_Categoria)
       AND (@SoloActivos = 0 OR p.Estado = 1)
     ORDER BY p.NombreProducto;
END
GO

CREATE OR ALTER PROCEDURE sp_Producto_Obtener
    @ID_Producto INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.ID_Producto, p.CodigoProducto, p.NombreProducto, p.Descripcion,
           p.ID_Categoria, c.NombreCategoria, p.ID_UnidadMedida, u.Abreviatura AS UnidadMedida,
           p.Costo, p.Precio,
           ISNULL((SELECT SUM(i.StockActual) FROM InventarioAlmacen i WHERE i.ID_Producto = p.ID_Producto), 0) AS StockTotal,
           p.Estado
      FROM Producto p
      JOIN Categoria c ON c.ID_Categoria = p.ID_Categoria
      JOIN UnidadMedida u ON u.ID_UnidadMedida = p.ID_UnidadMedida
     WHERE p.ID_Producto = @ID_Producto;
END
GO

CREATE OR ALTER PROCEDURE sp_Producto_Validar
    @ID_Producto     INT,
    @CodigoProducto  NVARCHAR(50),
    @ID_Categoria    INT,
    @ID_UnidadMedida INT,
    @Precio          DECIMAL(18,2),
    @Costo           DECIMAL(18,2) = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Producto WHERE CodigoProducto = UPPER(@CodigoProducto) AND ID_Producto <> @ID_Producto)
        THROW 50001, N'Ya existe un producto con ese código', 1;

    IF NOT EXISTS (SELECT 1 FROM Categoria WHERE ID_Categoria = @ID_Categoria AND Estado = 1)
        THROW 50001, N'La categoría no existe o está inactiva', 1;

    IF NOT EXISTS (SELECT 1 FROM UnidadMedida WHERE ID_UnidadMedida = @ID_UnidadMedida AND Estado = 1)
        THROW 50001, N'La unidad de medida no existe o está inactiva', 1;

    IF @Precio < 0
        THROW 50001, N'El precio de venta no puede ser negativo', 1;

    IF @Costo < 0
        THROW 50001, N'El costo de compra no puede ser negativo', 1;
END
GO

CREATE OR ALTER PROCEDURE sp_Producto_Insertar
    @CodigoProducto  NVARCHAR(50),
    @NombreProducto  NVARCHAR(150),
    @Descripcion     NVARCHAR(255) = NULL,
    @ID_Categoria    INT,
    @ID_UnidadMedida INT,
    @Costo           DECIMAL(18,2) = 0,
    @Precio          DECIMAL(18,2) = 0,
    @Estado BIT = 1   -- 1 = activo, 0 = inactivo
AS
BEGIN
    SET NOCOUNT ON;

    EXEC sp_Producto_Validar 0, @CodigoProducto, @ID_Categoria, @ID_UnidadMedida, @Precio, @Costo;

    INSERT INTO Producto (CodigoProducto, NombreProducto, Descripcion, ID_Categoria, ID_UnidadMedida, Costo, Precio, Estado, FechaCreacion)
    VALUES (UPPER(@CodigoProducto), @NombreProducto, @Descripcion, @ID_Categoria, @ID_UnidadMedida, @Costo, @Precio, @Estado, GETDATE());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Value;
END
GO

-- Costo = costo de compra (también lo actualiza cada compra). Precio = precio de venta
CREATE OR ALTER PROCEDURE sp_Producto_Actualizar
    @ID_Producto     INT,
    @CodigoProducto  NVARCHAR(50),
    @NombreProducto  NVARCHAR(150),
    @Descripcion     NVARCHAR(255) = NULL,
    @ID_Categoria    INT,
    @ID_UnidadMedida INT,
    @Precio          DECIMAL(18,2),
    @Estado BIT = NULL,   -- NULL = no cambia el estado
    @Costo  DECIMAL(18,2) = NULL   -- NULL = no cambia el costo de compra
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Producto WHERE ID_Producto = @ID_Producto)
        THROW 50404, N'El producto no existe', 1;

    DECLARE @CostoValidar DECIMAL(18,2) = ISNULL(@Costo, 0);
    EXEC sp_Producto_Validar @ID_Producto, @CodigoProducto, @ID_Categoria, @ID_UnidadMedida, @Precio, @CostoValidar;

    UPDATE Producto
       SET CodigoProducto = UPPER(@CodigoProducto), NombreProducto = @NombreProducto, Descripcion = @Descripcion,
           ID_Categoria = @ID_Categoria, ID_UnidadMedida = @ID_UnidadMedida, Precio = @Precio,
           Costo = ISNULL(@Costo, Costo),
           Estado = ISNULL(@Estado, Estado)
     WHERE ID_Producto = @ID_Producto;
END
GO

CREATE OR ALTER PROCEDURE sp_Producto_CambiarEstado
    @ID_Producto INT,
    @Estado      BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Producto WHERE ID_Producto = @ID_Producto)
        THROW 50404, N'El producto no existe', 1;

    UPDATE Producto SET Estado = @Estado WHERE ID_Producto = @ID_Producto;
END
GO

-- ---------- CLIENTES ----------
-- Los datos personales se guardan en DirectorioContacto. Si ya existe un contacto con el mismo NIT
-- (por ejemplo porque es proveedor), se reutiliza en lugar de duplicarlo

CREATE OR ALTER PROCEDURE sp_Cliente_Listar
    @Buscar      NVARCHAR(100) = NULL,
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.ID_Cliente, c.ID_Contacto, d.Nombre, d.NIT, d.Direccion, d.Telefono, d.CorreoElectronico, c.Estado
      FROM Cliente c
      JOIN DirectorioContacto d ON d.ID_Contacto = c.ID_Contacto
     WHERE (@Buscar IS NULL OR d.Nombre LIKE CONCAT(N'%', @Buscar, N'%') OR d.NIT LIKE CONCAT(N'%', @Buscar, N'%'))
       AND (@SoloActivos = 0 OR c.Estado = 1)
     ORDER BY d.Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_Cliente_Obtener
    @ID_Cliente INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.ID_Cliente, c.ID_Contacto, d.Nombre, d.NIT, d.Direccion, d.Telefono, d.CorreoElectronico, c.Estado
      FROM Cliente c
      JOIN DirectorioContacto d ON d.ID_Contacto = c.ID_Contacto
     WHERE c.ID_Cliente = @ID_Cliente;
END
GO

CREATE OR ALTER PROCEDURE sp_Cliente_Insertar
    @Nombre            NVARCHAR(150),
    @NIT               NVARCHAR(20),
    @Direccion         NVARCHAR(255) = NULL,
    @Telefono          NVARCHAR(50) = NULL,
    @CorreoElectronico NVARCHAR(100) = NULL,
    @Estado BIT = 1   -- 1 = activo, 0 = inactivo
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @ID_Contacto INT, @ID_Cliente INT;
    SET @NIT = UPPER(LTRIM(RTRIM(@NIT)));

    IF @NIT <> N'CF'
        SELECT @ID_Contacto = ID_Contacto FROM DirectorioContacto WHERE NIT = @NIT;

    IF @ID_Contacto IS NOT NULL AND EXISTS (SELECT 1 FROM Cliente WHERE ID_Contacto = @ID_Contacto)
        THROW 50001, N'Ya existe un cliente con ese NIT', 1;

    BEGIN TRANSACTION;

    IF @ID_Contacto IS NULL
    BEGIN
        INSERT INTO DirectorioContacto (Nombre, NIT, Direccion, Telefono, CorreoElectronico, Estado)
        VALUES (@Nombre, @NIT, @Direccion, @Telefono, @CorreoElectronico, 1);
        SET @ID_Contacto = SCOPE_IDENTITY();
    END
    ELSE
        UPDATE DirectorioContacto
           SET Nombre = @Nombre, Direccion = @Direccion, Telefono = @Telefono, CorreoElectronico = @CorreoElectronico
         WHERE ID_Contacto = @ID_Contacto;

    INSERT INTO Cliente (ID_Contacto, Estado) VALUES (@ID_Contacto, @Estado);
    SET @ID_Cliente = SCOPE_IDENTITY();

    COMMIT;

    SELECT @ID_Cliente AS Value;
END
GO

CREATE OR ALTER PROCEDURE sp_Cliente_Actualizar
    @ID_Cliente        INT,
    @Nombre            NVARCHAR(150),
    @NIT               NVARCHAR(20),
    @Direccion         NVARCHAR(255) = NULL,
    @Telefono          NVARCHAR(50) = NULL,
    @CorreoElectronico NVARCHAR(100) = NULL,
    @Estado BIT = NULL   -- NULL = no cambia el estado
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ID_Contacto INT;
    SET @NIT = UPPER(LTRIM(RTRIM(@NIT)));

    SELECT @ID_Contacto = ID_Contacto FROM Cliente WHERE ID_Cliente = @ID_Cliente;

    IF @ID_Contacto IS NULL
        THROW 50404, N'El cliente no existe', 1;

    IF @NIT <> N'CF' AND EXISTS (SELECT 1 FROM Cliente c JOIN DirectorioContacto d ON d.ID_Contacto = c.ID_Contacto
                                  WHERE d.NIT = @NIT AND c.ID_Cliente <> @ID_Cliente)
        THROW 50001, N'Ya existe otro cliente con ese NIT', 1;

    UPDATE DirectorioContacto
       SET Nombre = @Nombre, NIT = @NIT, Direccion = @Direccion, Telefono = @Telefono, CorreoElectronico = @CorreoElectronico
     WHERE ID_Contacto = @ID_Contacto;

    UPDATE Cliente SET Estado = ISNULL(@Estado, Estado) WHERE ID_Cliente = @ID_Cliente;
END
GO

CREATE OR ALTER PROCEDURE sp_Cliente_CambiarEstado
    @ID_Cliente INT,
    @Estado     BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Cliente WHERE ID_Cliente = @ID_Cliente)
        THROW 50404, N'El cliente no existe', 1;

    UPDATE Cliente SET Estado = @Estado WHERE ID_Cliente = @ID_Cliente;
END
GO

-- ---------- PROVEEDORES ----------

CREATE OR ALTER PROCEDURE sp_Proveedor_Listar
    @Buscar      NVARCHAR(100) = NULL,
    @SoloActivos BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.ID_Proveedor, p.ID_Contacto, d.Nombre, d.NIT, d.Direccion, d.Telefono, d.CorreoElectronico,
           p.DiasCredito, p.Estado
      FROM Proveedor p
      JOIN DirectorioContacto d ON d.ID_Contacto = p.ID_Contacto
     WHERE (@Buscar IS NULL OR d.Nombre LIKE CONCAT(N'%', @Buscar, N'%') OR d.NIT LIKE CONCAT(N'%', @Buscar, N'%'))
       AND (@SoloActivos = 0 OR p.Estado = 1)
     ORDER BY d.Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_Proveedor_Obtener
    @ID_Proveedor INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.ID_Proveedor, p.ID_Contacto, d.Nombre, d.NIT, d.Direccion, d.Telefono, d.CorreoElectronico,
           p.DiasCredito, p.Estado
      FROM Proveedor p
      JOIN DirectorioContacto d ON d.ID_Contacto = p.ID_Contacto
     WHERE p.ID_Proveedor = @ID_Proveedor;
END
GO

CREATE OR ALTER PROCEDURE sp_Proveedor_Insertar
    @Nombre            NVARCHAR(150),
    @NIT               NVARCHAR(20),
    @Direccion         NVARCHAR(255) = NULL,
    @Telefono          NVARCHAR(50) = NULL,
    @CorreoElectronico NVARCHAR(100) = NULL,
    @DiasCredito       INT = 0,
    @Estado BIT = 1   -- 1 = activo, 0 = inactivo
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @ID_Contacto INT, @ID_Proveedor INT;
    SET @NIT = UPPER(LTRIM(RTRIM(@NIT)));

    IF @NIT = N'CF'
        THROW 50001, N'Un proveedor debe tener NIT', 1;

    IF @DiasCredito < 0 OR @DiasCredito > 365
        THROW 50001, N'Los días de crédito deben estar entre 0 y 365', 1;

    SELECT @ID_Contacto = ID_Contacto FROM DirectorioContacto WHERE NIT = @NIT;

    IF @ID_Contacto IS NOT NULL AND EXISTS (SELECT 1 FROM Proveedor WHERE ID_Contacto = @ID_Contacto)
        THROW 50001, N'Ya existe un proveedor con ese NIT', 1;

    BEGIN TRANSACTION;

    IF @ID_Contacto IS NULL
    BEGIN
        INSERT INTO DirectorioContacto (Nombre, NIT, Direccion, Telefono, CorreoElectronico, Estado)
        VALUES (@Nombre, @NIT, @Direccion, @Telefono, @CorreoElectronico, 1);
        SET @ID_Contacto = SCOPE_IDENTITY();
    END
    ELSE
        UPDATE DirectorioContacto
           SET Nombre = @Nombre, Direccion = @Direccion, Telefono = @Telefono, CorreoElectronico = @CorreoElectronico
         WHERE ID_Contacto = @ID_Contacto;

    INSERT INTO Proveedor (ID_Contacto, DiasCredito, Estado) VALUES (@ID_Contacto, @DiasCredito, @Estado);
    SET @ID_Proveedor = SCOPE_IDENTITY();

    COMMIT;

    SELECT @ID_Proveedor AS Value;
END
GO

CREATE OR ALTER PROCEDURE sp_Proveedor_Actualizar
    @ID_Proveedor      INT,
    @Nombre            NVARCHAR(150),
    @NIT               NVARCHAR(20),
    @Direccion         NVARCHAR(255) = NULL,
    @Telefono          NVARCHAR(50) = NULL,
    @CorreoElectronico NVARCHAR(100) = NULL,
    @DiasCredito       INT = 0,
    @Estado BIT = NULL   -- NULL = no cambia el estado
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @ID_Contacto INT;
    SET @NIT = UPPER(LTRIM(RTRIM(@NIT)));

    SELECT @ID_Contacto = ID_Contacto FROM Proveedor WHERE ID_Proveedor = @ID_Proveedor;

    IF @ID_Contacto IS NULL
        THROW 50404, N'El proveedor no existe', 1;

    IF @NIT = N'CF'
        THROW 50001, N'Un proveedor debe tener NIT', 1;

    IF @DiasCredito < 0 OR @DiasCredito > 365
        THROW 50001, N'Los días de crédito deben estar entre 0 y 365', 1;

    IF EXISTS (SELECT 1 FROM Proveedor p JOIN DirectorioContacto d ON d.ID_Contacto = p.ID_Contacto
                WHERE d.NIT = @NIT AND p.ID_Proveedor <> @ID_Proveedor)
        THROW 50001, N'Ya existe otro proveedor con ese NIT', 1;

    BEGIN TRANSACTION;

    UPDATE Proveedor SET DiasCredito = @DiasCredito, Estado = ISNULL(@Estado, Estado) WHERE ID_Proveedor = @ID_Proveedor;

    UPDATE DirectorioContacto
       SET Nombre = @Nombre, NIT = @NIT, Direccion = @Direccion, Telefono = @Telefono, CorreoElectronico = @CorreoElectronico
     WHERE ID_Contacto = @ID_Contacto;

    COMMIT;
END
GO

CREATE OR ALTER PROCEDURE sp_Proveedor_CambiarEstado
    @ID_Proveedor INT,
    @Estado       BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Proveedor WHERE ID_Proveedor = @ID_Proveedor)
        THROW 50404, N'El proveedor no existe', 1;

    UPDATE Proveedor SET Estado = @Estado WHERE ID_Proveedor = @ID_Proveedor;
END
GO
