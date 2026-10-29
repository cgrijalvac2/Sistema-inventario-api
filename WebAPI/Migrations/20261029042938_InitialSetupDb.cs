using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetupDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Almacen",
                columns: table => new
                {
                    ID_Almacen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Almacen", x => x.ID_Almacen);
                });

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    ID_Categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCategoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescripcionCategoria = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.ID_Categoria);
                });

            migrationBuilder.CreateTable(
                name: "DirectorioContacto",
                columns: table => new
                {
                    ID_Contacto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NIT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectorioContacto", x => x.ID_Contacto);
                });

            migrationBuilder.CreateTable(
                name: "MotivoAjuste",
                columns: table => new
                {
                    ID_Motivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivoAjuste", x => x.ID_Motivo);
                });

            migrationBuilder.CreateTable(
                name: "Permiso",
                columns: table => new
                {
                    ID_Permiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permiso", x => x.ID_Permiso);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    ID_Rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.ID_Rol);
                });

            migrationBuilder.CreateTable(
                name: "TipoTransaccion",
                columns: table => new
                {
                    ID_TipoTransaccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Factor_Kardex = table.Column<int>(type: "int", nullable: false),
                    TipoTercero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RequiereDocumentoOrigen = table.Column<bool>(type: "bit", nullable: false),
                    RequiereMotivo = table.Column<bool>(type: "bit", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoTransaccion", x => x.ID_TipoTransaccion);
                });

            migrationBuilder.CreateTable(
                name: "UnidadMedida",
                columns: table => new
                {
                    ID_UnidadMedida = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Abreviatura = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadMedida", x => x.ID_UnidadMedida);
                });

            migrationBuilder.CreateTable(
                name: "ParametroSistema",
                columns: table => new
                {
                    ID_Parametro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Almacen = table.Column<int>(type: "int", nullable: true),
                    Clave = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametroSistema", x => x.ID_Parametro);
                    table.ForeignKey(
                        name: "FK_ParametroSistema_Almacen_ID_Almacen",
                        column: x => x.ID_Almacen,
                        principalTable: "Almacen",
                        principalColumn: "ID_Almacen",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    ID_Cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Contacto = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.ID_Cliente);
                    table.ForeignKey(
                        name: "FK_Cliente_DirectorioContacto_ID_Contacto",
                        column: x => x.ID_Contacto,
                        principalTable: "DirectorioContacto",
                        principalColumn: "ID_Contacto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    ID_Proveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Contacto = table.Column<int>(type: "int", nullable: false),
                    DiasCredito = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.ID_Proveedor);
                    table.ForeignKey(
                        name: "FK_Proveedor_DirectorioContacto_ID_Contacto",
                        column: x => x.ID_Contacto,
                        principalTable: "DirectorioContacto",
                        principalColumn: "ID_Contacto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolPermiso",
                columns: table => new
                {
                    ID_Rol = table.Column<int>(type: "int", nullable: false),
                    ID_Permiso = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolPermiso", x => new { x.ID_Rol, x.ID_Permiso });
                    table.ForeignKey(
                        name: "FK_RolPermiso_Permiso_ID_Permiso",
                        column: x => x.ID_Permiso,
                        principalTable: "Permiso",
                        principalColumn: "ID_Permiso",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolPermiso_Rol_ID_Rol",
                        column: x => x.ID_Rol,
                        principalTable: "Rol",
                        principalColumn: "ID_Rol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    ID_Usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ID_Rol = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.ID_Usuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_ID_Rol",
                        column: x => x.ID_Rol,
                        principalTable: "Rol",
                        principalColumn: "ID_Rol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Correlativo",
                columns: table => new
                {
                    ID_Correlativo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_TipoTransaccion = table.Column<int>(type: "int", nullable: true),
                    Clave = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Prefijo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UltimoNumero = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Correlativo", x => x.ID_Correlativo);
                    table.ForeignKey(
                        name: "FK_Correlativo_TipoTransaccion_ID_TipoTransaccion",
                        column: x => x.ID_TipoTransaccion,
                        principalTable: "TipoTransaccion",
                        principalColumn: "ID_TipoTransaccion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    ID_Producto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Categoria = table.Column<int>(type: "int", nullable: false),
                    ID_UnidadMedida = table.Column<int>(type: "int", nullable: false),
                    CodigoProducto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NombreProducto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.ID_Producto);
                    table.ForeignKey(
                        name: "FK_Producto_Categoria_ID_Categoria",
                        column: x => x.ID_Categoria,
                        principalTable: "Categoria",
                        principalColumn: "ID_Categoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Producto_UnidadMedida_ID_UnidadMedida",
                        column: x => x.ID_UnidadMedida,
                        principalTable: "UnidadMedida",
                        principalColumn: "ID_UnidadMedida",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompra",
                columns: table => new
                {
                    ID_OrdenCompra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroOrden = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ID_Proveedor = table.Column<int>(type: "int", nullable: false),
                    ID_Almacen = table.Column<int>(type: "int", nullable: false),
                    ID_Usuario = table.Column<int>(type: "int", nullable: false),
                    FechaOrden = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEsperada = table.Column<DateTime>(type: "date", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompra", x => x.ID_OrdenCompra);
                    table.ForeignKey(
                        name: "FK_OrdenCompra_Almacen_ID_Almacen",
                        column: x => x.ID_Almacen,
                        principalTable: "Almacen",
                        principalColumn: "ID_Almacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenCompra_Proveedor_ID_Proveedor",
                        column: x => x.ID_Proveedor,
                        principalTable: "Proveedor",
                        principalColumn: "ID_Proveedor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenCompra_Usuario_ID_Usuario",
                        column: x => x.ID_Usuario,
                        principalTable: "Usuario",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transferencia",
                columns: table => new
                {
                    ID_Transferencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroTransferencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ID_AlmacenOrigen = table.Column<int>(type: "int", nullable: false),
                    ID_AlmacenDestino = table.Column<int>(type: "int", nullable: false),
                    ID_UsuarioEnvia = table.Column<int>(type: "int", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ID_UsuarioRecibe = table.Column<int>(type: "int", nullable: true),
                    FechaRecepcion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencia", x => x.ID_Transferencia);
                    table.ForeignKey(
                        name: "FK_Transferencia_Almacen_ID_AlmacenDestino",
                        column: x => x.ID_AlmacenDestino,
                        principalTable: "Almacen",
                        principalColumn: "ID_Almacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencia_Almacen_ID_AlmacenOrigen",
                        column: x => x.ID_AlmacenOrigen,
                        principalTable: "Almacen",
                        principalColumn: "ID_Almacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencia_Usuario_ID_UsuarioEnvia",
                        column: x => x.ID_UsuarioEnvia,
                        principalTable: "Usuario",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transferencia_Usuario_ID_UsuarioRecibe",
                        column: x => x.ID_UsuarioRecibe,
                        principalTable: "Usuario",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioAlmacen",
                columns: table => new
                {
                    ID_Usuario = table.Column<int>(type: "int", nullable: false),
                    ID_Almacen = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioAlmacen", x => new { x.ID_Usuario, x.ID_Almacen });
                    table.ForeignKey(
                        name: "FK_UsuarioAlmacen_Almacen_ID_Almacen",
                        column: x => x.ID_Almacen,
                        principalTable: "Almacen",
                        principalColumn: "ID_Almacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioAlmacen_Usuario_ID_Usuario",
                        column: x => x.ID_Usuario,
                        principalTable: "Usuario",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventarioAlmacen",
                columns: table => new
                {
                    ID_Almacen = table.Column<int>(type: "int", nullable: false),
                    ID_Producto = table.Column<int>(type: "int", nullable: false),
                    StockActual = table.Column<int>(type: "int", nullable: false),
                    StockMinimo = table.Column<int>(type: "int", nullable: false),
                    StockMaximo = table.Column<int>(type: "int", nullable: false),
                    FechaUltimoMovimiento = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioAlmacen", x => new { x.ID_Almacen, x.ID_Producto });
                    table.ForeignKey(
                        name: "FK_InventarioAlmacen_Almacen_ID_Almacen",
                        column: x => x.ID_Almacen,
                        principalTable: "Almacen",
                        principalColumn: "ID_Almacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventarioAlmacen_Producto_ID_Producto",
                        column: x => x.ID_Producto,
                        principalTable: "Producto",
                        principalColumn: "ID_Producto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompraDetalle",
                columns: table => new
                {
                    ID_DetalleOC = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_OrdenCompra = table.Column<int>(type: "int", nullable: false),
                    ID_Producto = table.Column<int>(type: "int", nullable: false),
                    CantidadPedida = table.Column<int>(type: "int", nullable: false),
                    CantidadRecibida = table.Column<int>(type: "int", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompraDetalle", x => x.ID_DetalleOC);
                    table.ForeignKey(
                        name: "FK_OrdenCompraDetalle_OrdenCompra_ID_OrdenCompra",
                        column: x => x.ID_OrdenCompra,
                        principalTable: "OrdenCompra",
                        principalColumn: "ID_OrdenCompra",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenCompraDetalle_Producto_ID_Producto",
                        column: x => x.ID_Producto,
                        principalTable: "Producto",
                        principalColumn: "ID_Producto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transaccion",
                columns: table => new
                {
                    ID_Transaccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ID_TipoTransaccion = table.Column<int>(type: "int", nullable: false),
                    ID_Usuario = table.Column<int>(type: "int", nullable: false),
                    ID_Almacen = table.Column<int>(type: "int", nullable: false),
                    ID_Cliente = table.Column<int>(type: "int", nullable: true),
                    ID_Proveedor = table.Column<int>(type: "int", nullable: true),
                    ID_OrdenCompra = table.Column<int>(type: "int", nullable: true),
                    ID_TransaccionOrigen = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ID_UsuarioAnula = table.Column<int>(type: "int", nullable: true),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaccion", x => x.ID_Transaccion);
                    table.ForeignKey(
                        name: "FK_Transaccion_Almacen_ID_Almacen",
                        column: x => x.ID_Almacen,
                        principalTable: "Almacen",
                        principalColumn: "ID_Almacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaccion_Cliente_ID_Cliente",
                        column: x => x.ID_Cliente,
                        principalTable: "Cliente",
                        principalColumn: "ID_Cliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaccion_OrdenCompra_ID_OrdenCompra",
                        column: x => x.ID_OrdenCompra,
                        principalTable: "OrdenCompra",
                        principalColumn: "ID_OrdenCompra",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaccion_Proveedor_ID_Proveedor",
                        column: x => x.ID_Proveedor,
                        principalTable: "Proveedor",
                        principalColumn: "ID_Proveedor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaccion_TipoTransaccion_ID_TipoTransaccion",
                        column: x => x.ID_TipoTransaccion,
                        principalTable: "TipoTransaccion",
                        principalColumn: "ID_TipoTransaccion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaccion_Transaccion_ID_TransaccionOrigen",
                        column: x => x.ID_TransaccionOrigen,
                        principalTable: "Transaccion",
                        principalColumn: "ID_Transaccion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaccion_Usuario_ID_Usuario",
                        column: x => x.ID_Usuario,
                        principalTable: "Usuario",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaccion_Usuario_ID_UsuarioAnula",
                        column: x => x.ID_UsuarioAnula,
                        principalTable: "Usuario",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransferenciaDetalle",
                columns: table => new
                {
                    ID_DetalleTransferencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Transferencia = table.Column<int>(type: "int", nullable: false),
                    ID_Producto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferenciaDetalle", x => x.ID_DetalleTransferencia);
                    table.ForeignKey(
                        name: "FK_TransferenciaDetalle_Producto_ID_Producto",
                        column: x => x.ID_Producto,
                        principalTable: "Producto",
                        principalColumn: "ID_Producto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferenciaDetalle_Transferencia_ID_Transferencia",
                        column: x => x.ID_Transferencia,
                        principalTable: "Transferencia",
                        principalColumn: "ID_Transferencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransaccionDetalle",
                columns: table => new
                {
                    ID_Detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Transaccion = table.Column<int>(type: "int", nullable: false),
                    ID_Producto = table.Column<int>(type: "int", nullable: false),
                    ID_Motivo = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransaccionDetalle", x => x.ID_Detalle);
                    table.ForeignKey(
                        name: "FK_TransaccionDetalle_MotivoAjuste_ID_Motivo",
                        column: x => x.ID_Motivo,
                        principalTable: "MotivoAjuste",
                        principalColumn: "ID_Motivo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransaccionDetalle_Producto_ID_Producto",
                        column: x => x.ID_Producto,
                        principalTable: "Producto",
                        principalColumn: "ID_Producto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransaccionDetalle_Transaccion_ID_Transaccion",
                        column: x => x.ID_Transaccion,
                        principalTable: "Transaccion",
                        principalColumn: "ID_Transaccion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialMovimientos",
                columns: table => new
                {
                    ID_Movimiento = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Producto = table.Column<int>(type: "int", nullable: false),
                    ID_Almacen = table.Column<int>(type: "int", nullable: false),
                    ID_Usuario = table.Column<int>(type: "int", nullable: false),
                    ID_Detalle = table.Column<int>(type: "int", nullable: true),
                    ID_DetalleTransferencia = table.Column<int>(type: "int", nullable: true),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoAnterior_Almacen = table.Column<int>(type: "int", nullable: false),
                    SaldoActual_Almacen = table.Column<int>(type: "int", nullable: false),
                    SaldoActual_Producto = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialMovimientos", x => x.ID_Movimiento);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Almacen_ID_Almacen",
                        column: x => x.ID_Almacen,
                        principalTable: "Almacen",
                        principalColumn: "ID_Almacen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Producto_ID_Producto",
                        column: x => x.ID_Producto,
                        principalTable: "Producto",
                        principalColumn: "ID_Producto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_TransaccionDetalle_ID_Detalle",
                        column: x => x.ID_Detalle,
                        principalTable: "TransaccionDetalle",
                        principalColumn: "ID_Detalle",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_TransferenciaDetalle_ID_DetalleTransferencia",
                        column: x => x.ID_DetalleTransferencia,
                        principalTable: "TransferenciaDetalle",
                        principalColumn: "ID_DetalleTransferencia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialMovimientos_Usuario_ID_Usuario",
                        column: x => x.ID_Usuario,
                        principalTable: "Usuario",
                        principalColumn: "ID_Usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Almacen",
                columns: new[] { "ID_Almacen", "Codigo", "Descripcion", "Direccion", "Estado", "Nombre" },
                values: new object[] { 1, "CENTRAL", null, null, true, "Bodega central" });

            migrationBuilder.InsertData(
                table: "Correlativo",
                columns: new[] { "ID_Correlativo", "Clave", "Estado", "ID_TipoTransaccion", "Prefijo", "UltimoNumero" },
                values: new object[,]
                {
                    { 9, "ORDEN_COMPRA", true, null, "OC", 0 },
                    { 10, "TRANSFERENCIA", true, null, "TRF", 0 }
                });

            migrationBuilder.InsertData(
                table: "MotivoAjuste",
                columns: new[] { "ID_Motivo", "Descripcion", "Estado" },
                values: new object[,]
                {
                    { 1, "Inventario inicial", true },
                    { 2, "Producto vencido", true },
                    { 3, "Producto dañado", true },
                    { 4, "Robo o extravío", true },
                    { 5, "Diferencia en conteo físico", true }
                });

            migrationBuilder.InsertData(
                table: "ParametroSistema",
                columns: new[] { "ID_Parametro", "Clave", "Descripcion", "Estado", "FechaModificacion", "ID_Almacen", "Valor" },
                values: new object[,]
                {
                    { 1, "IVA_PORCENTAJE", "Porcentaje de IVA que se aplica a compras y ventas", true, null, null, "12" },
                    { 2, "MONEDA", "Moneda del sistema", true, null, null, "GTQ" },
                    { 3, "NOMBRE_EMPRESA", "Nombre que aparece en reportes", true, null, null, "Mi Empresa, S.A." }
                });

            migrationBuilder.InsertData(
                table: "Permiso",
                columns: new[] { "ID_Permiso", "Codigo", "Descripcion", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1, "SEGURIDAD", null, true, "Usuarios, roles y permisos" },
                    { 2, "CONFIGURACION", null, true, "Parámetros del sistema y correlativos" },
                    { 3, "CATALOGOS", null, true, "Productos, categorías, almacenes, clientes y proveedores" },
                    { 4, "COMPRAS", null, true, "Órdenes de compra, compras y notas de proveedor" },
                    { 5, "VENTAS", null, true, "Ventas y notas a clientes" },
                    { 6, "AJUSTES", null, true, "Ajustes de inventario" },
                    { 7, "TRANSFERENCIAS", null, true, "Transferencias entre almacenes" },
                    { 8, "REPORTES", null, true, "Existencias, kardex y reportes" }
                });

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "ID_Rol", "Descripcion", "Estado", "FechaCreacion", "FechaModificacion", "NombreRol" },
                values: new object[,]
                {
                    { 1, "Acceso total", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Administrador" },
                    { 2, "Compras, ajustes y transferencias", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Bodeguero" },
                    { 3, "Ventas y consultas", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Vendedor" }
                });

            migrationBuilder.InsertData(
                table: "TipoTransaccion",
                columns: new[] { "ID_TipoTransaccion", "Codigo", "Descripcion", "Estado", "Factor_Kardex", "Nombre", "RequiereDocumentoOrigen", "RequiereMotivo", "TipoTercero" },
                values: new object[,]
                {
                    { 1, "COMPRA", "Compra de mercadería a un proveedor", true, 1, "Compra", false, false, "PROVEEDOR" },
                    { 2, "VENTA", "Venta de mercadería a un cliente", true, -1, "Venta", false, false, "CLIENTE" },
                    { 3, "NC_VENTA", "Devolución del cliente: el producto regresa al inventario", true, 1, "Nota de crédito a cliente", true, false, "CLIENTE" },
                    { 4, "NC_COMPRA", "Devolución al proveedor: el producto sale del inventario", true, -1, "Nota de crédito de proveedor", true, false, "PROVEEDOR" },
                    { 5, "ND_VENTA", "Cargo adicional al cliente (no mueve inventario)", true, 0, "Nota de débito a cliente", true, false, "CLIENTE" },
                    { 6, "ND_COMPRA", "Cargo adicional del proveedor (no mueve inventario)", true, 0, "Nota de débito de proveedor", true, false, "PROVEEDOR" },
                    { 7, "AJUSTE_ENTRADA", "Ingreso por inventario inicial, sobrante o corrección", true, 1, "Ajuste de entrada", false, true, "NINGUNO" },
                    { 8, "AJUSTE_SALIDA", "Salida por merma, daño, vencimiento o faltante", true, -1, "Ajuste de salida", false, true, "NINGUNO" }
                });

            migrationBuilder.InsertData(
                table: "UnidadMedida",
                columns: new[] { "ID_UnidadMedida", "Abreviatura", "Estado", "Nombre" },
                values: new object[,]
                {
                    { 1, "UND", true, "Unidad" },
                    { 2, "CJA", true, "Caja" },
                    { 3, "LB", true, "Libra" }
                });

            migrationBuilder.InsertData(
                table: "Correlativo",
                columns: new[] { "ID_Correlativo", "Clave", "Estado", "ID_TipoTransaccion", "Prefijo", "UltimoNumero" },
                values: new object[,]
                {
                    { 1, "COMPRA", true, 1, "COM", 0 },
                    { 2, "VENTA", true, 2, "VEN", 0 },
                    { 3, "NC_VENTA", true, 3, "NCV", 0 },
                    { 4, "NC_COMPRA", true, 4, "NCC", 0 },
                    { 5, "ND_VENTA", true, 5, "NDV", 0 },
                    { 6, "ND_COMPRA", true, 6, "NDC", 0 },
                    { 7, "AJUSTE_ENTRADA", true, 7, "AJE", 0 },
                    { 8, "AJUSTE_SALIDA", true, 8, "AJS", 0 }
                });

            migrationBuilder.InsertData(
                table: "RolPermiso",
                columns: new[] { "ID_Permiso", "ID_Rol" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 3, 2 },
                    { 4, 2 },
                    { 6, 2 },
                    { 7, 2 },
                    { 8, 2 },
                    { 5, 3 },
                    { 8, 3 }
                });

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "ID_Usuario", "CorreoElectronico", "Estado", "FechaCreacion", "ID_Rol", "NombreCompleto", "PasswordHash", "UltimoAcceso" },
                values: new object[] { 1, "admin@inventario.com", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Administrador del sistema", "AQAAAAIAAYagAAAAEIdZlUACITcho0loWvulAcozN5+OdY1kTuGR4oRX0U7QkgBoeNvBZrsQoVTd3K3blQ==", null });

            migrationBuilder.InsertData(
                table: "UsuarioAlmacen",
                columns: new[] { "ID_Almacen", "ID_Usuario" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Almacen_Codigo",
                table: "Almacen",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_NombreCategoria",
                table: "Categoria",
                column: "NombreCategoria",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_ID_Contacto",
                table: "Cliente",
                column: "ID_Contacto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Correlativo_Clave",
                table: "Correlativo",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Correlativo_ID_TipoTransaccion",
                table: "Correlativo",
                column: "ID_TipoTransaccion");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_ID_Almacen",
                table: "HistorialMovimientos",
                column: "ID_Almacen");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_ID_Detalle",
                table: "HistorialMovimientos",
                column: "ID_Detalle");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_ID_DetalleTransferencia",
                table: "HistorialMovimientos",
                column: "ID_DetalleTransferencia");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_ID_Producto_ID_Almacen_FechaRegistro",
                table: "HistorialMovimientos",
                columns: new[] { "ID_Producto", "ID_Almacen", "FechaRegistro" });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMovimientos_ID_Usuario",
                table: "HistorialMovimientos",
                column: "ID_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioAlmacen_ID_Producto",
                table: "InventarioAlmacen",
                column: "ID_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_ID_Almacen",
                table: "OrdenCompra",
                column: "ID_Almacen");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_ID_Proveedor",
                table: "OrdenCompra",
                column: "ID_Proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_ID_Usuario",
                table: "OrdenCompra",
                column: "ID_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompra_NumeroOrden",
                table: "OrdenCompra",
                column: "NumeroOrden",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraDetalle_ID_OrdenCompra",
                table: "OrdenCompraDetalle",
                column: "ID_OrdenCompra");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompraDetalle_ID_Producto",
                table: "OrdenCompraDetalle",
                column: "ID_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_ParametroSistema_Clave_ID_Almacen",
                table: "ParametroSistema",
                columns: new[] { "Clave", "ID_Almacen" },
                unique: true,
                filter: "[ID_Almacen] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ParametroSistema_ID_Almacen",
                table: "ParametroSistema",
                column: "ID_Almacen");

            migrationBuilder.CreateIndex(
                name: "IX_Permiso_Codigo",
                table: "Permiso",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Producto_CodigoProducto",
                table: "Producto",
                column: "CodigoProducto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Producto_ID_Categoria",
                table: "Producto",
                column: "ID_Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_ID_UnidadMedida",
                table: "Producto",
                column: "ID_UnidadMedida");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedor_ID_Contacto",
                table: "Proveedor",
                column: "ID_Contacto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rol_NombreRol",
                table: "Rol",
                column: "NombreRol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolPermiso_ID_Permiso",
                table: "RolPermiso",
                column: "ID_Permiso");

            migrationBuilder.CreateIndex(
                name: "IX_TipoTransaccion_Codigo",
                table: "TipoTransaccion",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_ID_Almacen",
                table: "Transaccion",
                column: "ID_Almacen");

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_ID_Cliente",
                table: "Transaccion",
                column: "ID_Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_ID_OrdenCompra",
                table: "Transaccion",
                column: "ID_OrdenCompra");

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_ID_Proveedor",
                table: "Transaccion",
                column: "ID_Proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_ID_TipoTransaccion",
                table: "Transaccion",
                column: "ID_TipoTransaccion");

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_ID_TransaccionOrigen",
                table: "Transaccion",
                column: "ID_TransaccionOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_ID_Usuario",
                table: "Transaccion",
                column: "ID_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_ID_UsuarioAnula",
                table: "Transaccion",
                column: "ID_UsuarioAnula");

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_NumeroDocumento",
                table: "Transaccion",
                column: "NumeroDocumento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionDetalle_ID_Motivo",
                table: "TransaccionDetalle",
                column: "ID_Motivo");

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionDetalle_ID_Producto",
                table: "TransaccionDetalle",
                column: "ID_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionDetalle_ID_Transaccion",
                table: "TransaccionDetalle",
                column: "ID_Transaccion");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_ID_AlmacenDestino",
                table: "Transferencia",
                column: "ID_AlmacenDestino");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_ID_AlmacenOrigen",
                table: "Transferencia",
                column: "ID_AlmacenOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_ID_UsuarioEnvia",
                table: "Transferencia",
                column: "ID_UsuarioEnvia");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_ID_UsuarioRecibe",
                table: "Transferencia",
                column: "ID_UsuarioRecibe");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_NumeroTransferencia",
                table: "Transferencia",
                column: "NumeroTransferencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaDetalle_ID_Producto",
                table: "TransferenciaDetalle",
                column: "ID_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_TransferenciaDetalle_ID_Transferencia",
                table: "TransferenciaDetalle",
                column: "ID_Transferencia");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadMedida_Abreviatura",
                table: "UnidadMedida",
                column: "Abreviatura",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_CorreoElectronico",
                table: "Usuario",
                column: "CorreoElectronico",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_ID_Rol",
                table: "Usuario",
                column: "ID_Rol");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAlmacen_ID_Almacen",
                table: "UsuarioAlmacen",
                column: "ID_Almacen");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Correlativo");

            migrationBuilder.DropTable(
                name: "HistorialMovimientos");

            migrationBuilder.DropTable(
                name: "InventarioAlmacen");

            migrationBuilder.DropTable(
                name: "OrdenCompraDetalle");

            migrationBuilder.DropTable(
                name: "ParametroSistema");

            migrationBuilder.DropTable(
                name: "RolPermiso");

            migrationBuilder.DropTable(
                name: "UsuarioAlmacen");

            migrationBuilder.DropTable(
                name: "TransaccionDetalle");

            migrationBuilder.DropTable(
                name: "TransferenciaDetalle");

            migrationBuilder.DropTable(
                name: "Permiso");

            migrationBuilder.DropTable(
                name: "MotivoAjuste");

            migrationBuilder.DropTable(
                name: "Transaccion");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Transferencia");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "OrdenCompra");

            migrationBuilder.DropTable(
                name: "TipoTransaccion");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "UnidadMedida");

            migrationBuilder.DropTable(
                name: "Almacen");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "DirectorioContacto");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
