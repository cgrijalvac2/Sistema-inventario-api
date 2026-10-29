using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        // Seguridad
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Permiso> Permiso { get; set; }
        public DbSet<RolPermiso> RolPermiso { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<UsuarioAlmacen> UsuarioAlmacen { get; set; }

        // Configuración
        public DbSet<ParametroSistema> ParametroSistema { get; set; }
        public DbSet<Correlativo> Correlativo { get; set; }
        public DbSet<TipoTransaccion> TipoTransaccion { get; set; }

        // Catálogos
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<UnidadMedida> UnidadMedida { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Almacen> Almacen { get; set; }
        public DbSet<InventarioAlmacen> InventarioAlmacen { get; set; }
        public DbSet<DirectorioContacto> DirectorioContacto { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<MotivoAjuste> MotivoAjuste { get; set; }

        // Movimientos
        public DbSet<OrdenCompra> OrdenCompra { get; set; }
        public DbSet<OrdenCompraDetalle> OrdenCompraDetalle { get; set; }
        public DbSet<Transaccion> Transaccion { get; set; }
        public DbSet<TransaccionDetalle> TransaccionDetalle { get; set; }
        public DbSet<Transferencia> Transferencia { get; set; }
        public DbSet<TransferenciaDetalle> TransferenciaDetalle { get; set; }
        public DbSet<HistorialMovimiento> HistorialMovimientos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        //add-migration "InitialSetupDb"
        //update-database

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SQL Server no permite borrar en cascada cuando hay varios caminos hacia la misma tabla
            // (ej: Transferencia tiene 2 llaves hacia Almacen y 2 hacia Usuario).
            // Además, en un inventario nunca se debe borrar historial: se desactiva con Estado = false.
            foreach (var relacion in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relacion.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Los detalles sí se borran junto con su encabezado
            modelBuilder.Entity<OrdenCompraDetalle>()
                .HasOne(d => d.OrdenCompra).WithMany(o => o.Detalles)
                .HasForeignKey(d => d.ID_OrdenCompra).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransaccionDetalle>()
                .HasOne(d => d.Transaccion).WithMany(t => t.Detalles)
                .HasForeignKey(d => d.ID_Transaccion).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransferenciaDetalle>()
                .HasOne(d => d.Transferencia).WithMany(t => t.Detalles)
                .HasForeignKey(d => d.ID_Transferencia).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolPermiso>()
                .HasOne(rp => rp.Rol).WithMany(r => r.RolPermisos)
                .HasForeignKey(rp => rp.ID_Rol).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioAlmacen>()
                .HasOne(ua => ua.Usuario).WithMany(u => u.UsuarioAlmacenes)
                .HasForeignKey(ua => ua.ID_Usuario).OnDelete(DeleteBehavior.Cascade);

            // Transaccion tiene dos llaves hacia Usuario: se indica cuál navegación usa cada una
            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.Usuario).WithMany().HasForeignKey(t => t.ID_Usuario);
            modelBuilder.Entity<Transaccion>()
                .HasOne(t => t.UsuarioAnula).WithMany().HasForeignKey(t => t.ID_UsuarioAnula);

            DatosIniciales(modelBuilder);
        }

        // Datos que se insertan automáticamente con update-database
        private static void DatosIniciales(ModelBuilder modelBuilder)
        {
            var fecha = new DateTime(2026, 1, 1);

            modelBuilder.Entity<TipoTransaccion>().HasData(
                new TipoTransaccion { ID_TipoTransaccion = 1, Codigo = "COMPRA", Nombre = "Compra", Descripcion = "Compra de mercadería a un proveedor", Factor_Kardex = 1, TipoTercero = "PROVEEDOR" },
                new TipoTransaccion { ID_TipoTransaccion = 2, Codigo = "VENTA", Nombre = "Venta", Descripcion = "Venta de mercadería a un cliente", Factor_Kardex = -1, TipoTercero = "CLIENTE" },
                new TipoTransaccion { ID_TipoTransaccion = 3, Codigo = "NC_VENTA", Nombre = "Nota de crédito a cliente", Descripcion = "Devolución del cliente: el producto regresa al inventario", Factor_Kardex = 1, TipoTercero = "CLIENTE", RequiereDocumentoOrigen = true },
                new TipoTransaccion { ID_TipoTransaccion = 4, Codigo = "NC_COMPRA", Nombre = "Nota de crédito de proveedor", Descripcion = "Devolución al proveedor: el producto sale del inventario", Factor_Kardex = -1, TipoTercero = "PROVEEDOR", RequiereDocumentoOrigen = true },
                new TipoTransaccion { ID_TipoTransaccion = 5, Codigo = "ND_VENTA", Nombre = "Nota de débito a cliente", Descripcion = "Cargo adicional al cliente (no mueve inventario)", Factor_Kardex = 0, TipoTercero = "CLIENTE", RequiereDocumentoOrigen = true },
                new TipoTransaccion { ID_TipoTransaccion = 6, Codigo = "ND_COMPRA", Nombre = "Nota de débito de proveedor", Descripcion = "Cargo adicional del proveedor (no mueve inventario)", Factor_Kardex = 0, TipoTercero = "PROVEEDOR", RequiereDocumentoOrigen = true },
                new TipoTransaccion { ID_TipoTransaccion = 7, Codigo = "AJUSTE_ENTRADA", Nombre = "Ajuste de entrada", Descripcion = "Ingreso por inventario inicial, sobrante o corrección", Factor_Kardex = 1, TipoTercero = "NINGUNO", RequiereMotivo = true },
                new TipoTransaccion { ID_TipoTransaccion = 8, Codigo = "AJUSTE_SALIDA", Nombre = "Ajuste de salida", Descripcion = "Salida por merma, daño, vencimiento o faltante", Factor_Kardex = -1, TipoTercero = "NINGUNO", RequiereMotivo = true }
            );

            modelBuilder.Entity<Correlativo>().HasData(
                new Correlativo { ID_Correlativo = 1, ID_TipoTransaccion = 1, Clave = "COMPRA", Prefijo = "COM" },
                new Correlativo { ID_Correlativo = 2, ID_TipoTransaccion = 2, Clave = "VENTA", Prefijo = "VEN" },
                new Correlativo { ID_Correlativo = 3, ID_TipoTransaccion = 3, Clave = "NC_VENTA", Prefijo = "NCV" },
                new Correlativo { ID_Correlativo = 4, ID_TipoTransaccion = 4, Clave = "NC_COMPRA", Prefijo = "NCC" },
                new Correlativo { ID_Correlativo = 5, ID_TipoTransaccion = 5, Clave = "ND_VENTA", Prefijo = "NDV" },
                new Correlativo { ID_Correlativo = 6, ID_TipoTransaccion = 6, Clave = "ND_COMPRA", Prefijo = "NDC" },
                new Correlativo { ID_Correlativo = 7, ID_TipoTransaccion = 7, Clave = "AJUSTE_ENTRADA", Prefijo = "AJE" },
                new Correlativo { ID_Correlativo = 8, ID_TipoTransaccion = 8, Clave = "AJUSTE_SALIDA", Prefijo = "AJS" },
                new Correlativo { ID_Correlativo = 9, Clave = "ORDEN_COMPRA", Prefijo = "OC" },
                new Correlativo { ID_Correlativo = 10, Clave = "TRANSFERENCIA", Prefijo = "TRF" }
            );

            modelBuilder.Entity<Permiso>().HasData(
                new Permiso { ID_Permiso = 1, Codigo = "SEGURIDAD", Nombre = "Usuarios, roles y permisos" },
                new Permiso { ID_Permiso = 2, Codigo = "CONFIGURACION", Nombre = "Parámetros del sistema y correlativos" },
                new Permiso { ID_Permiso = 3, Codigo = "CATALOGOS", Nombre = "Productos, categorías, almacenes, clientes y proveedores" },
                new Permiso { ID_Permiso = 4, Codigo = "COMPRAS", Nombre = "Órdenes de compra, compras y notas de proveedor" },
                new Permiso { ID_Permiso = 5, Codigo = "VENTAS", Nombre = "Ventas y notas a clientes" },
                new Permiso { ID_Permiso = 6, Codigo = "AJUSTES", Nombre = "Ajustes de inventario" },
                new Permiso { ID_Permiso = 7, Codigo = "TRANSFERENCIAS", Nombre = "Transferencias entre almacenes" },
                new Permiso { ID_Permiso = 8, Codigo = "REPORTES", Nombre = "Existencias, kardex y reportes" }
            );

            modelBuilder.Entity<Rol>().HasData(
                new Rol { ID_Rol = 1, NombreRol = "Administrador", Descripcion = "Acceso total", FechaCreacion = fecha },
                new Rol { ID_Rol = 2, NombreRol = "Bodeguero", Descripcion = "Compras, ajustes y transferencias", FechaCreacion = fecha },
                new Rol { ID_Rol = 3, NombreRol = "Vendedor", Descripcion = "Ventas y consultas", FechaCreacion = fecha }
            );

            modelBuilder.Entity<RolPermiso>().HasData(
                new RolPermiso { ID_Rol = 1, ID_Permiso = 1 },
                new RolPermiso { ID_Rol = 1, ID_Permiso = 2 },
                new RolPermiso { ID_Rol = 1, ID_Permiso = 3 },
                new RolPermiso { ID_Rol = 1, ID_Permiso = 4 },
                new RolPermiso { ID_Rol = 1, ID_Permiso = 5 },
                new RolPermiso { ID_Rol = 1, ID_Permiso = 6 },
                new RolPermiso { ID_Rol = 1, ID_Permiso = 7 },
                new RolPermiso { ID_Rol = 1, ID_Permiso = 8 },
                new RolPermiso { ID_Rol = 2, ID_Permiso = 3 },
                new RolPermiso { ID_Rol = 2, ID_Permiso = 4 },
                new RolPermiso { ID_Rol = 2, ID_Permiso = 6 },
                new RolPermiso { ID_Rol = 2, ID_Permiso = 7 },
                new RolPermiso { ID_Rol = 2, ID_Permiso = 8 },
                new RolPermiso { ID_Rol = 3, ID_Permiso = 5 },
                new RolPermiso { ID_Rol = 3, ID_Permiso = 8 }
            );

            modelBuilder.Entity<Almacen>().HasData(
                new Almacen { ID_Almacen = 1, Codigo = "CENTRAL", Nombre = "Bodega central" }
            );

            // Usuario inicial para poder entrar al sistema: admin@inventario.com / Admin123!
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    ID_Usuario = 1,
                    NombreCompleto = "Administrador del sistema",
                    CorreoElectronico = "admin@inventario.com",
                    // Hash generado con HashUsuario.HashPassword (nunca se guarda la contraseña en texto plano)
                    PasswordHash = "AQAAAAIAAYagAAAAEIdZlUACITcho0loWvulAcozN5+OdY1kTuGR4oRX0U7QkgBoeNvBZrsQoVTd3K3blQ==",
                    ID_Rol = 1,
                    FechaCreacion = fecha
                }
            );

            modelBuilder.Entity<UsuarioAlmacen>().HasData(
                new UsuarioAlmacen { ID_Usuario = 1, ID_Almacen = 1 }
            );

            modelBuilder.Entity<ParametroSistema>().HasData(
                new ParametroSistema { ID_Parametro = 1, Clave = "IVA_PORCENTAJE", Valor = "12", Descripcion = "Porcentaje de IVA que se aplica a compras y ventas" },
                new ParametroSistema { ID_Parametro = 2, Clave = "MONEDA", Valor = "GTQ", Descripcion = "Moneda del sistema" },
                new ParametroSistema { ID_Parametro = 3, Clave = "NOMBRE_EMPRESA", Valor = "Mi Empresa, S.A.", Descripcion = "Nombre que aparece en reportes" }
            );

            modelBuilder.Entity<UnidadMedida>().HasData(
                new UnidadMedida { ID_UnidadMedida = 1, Nombre = "Unidad", Abreviatura = "UND" },
                new UnidadMedida { ID_UnidadMedida = 2, Nombre = "Caja", Abreviatura = "CJA" },
                new UnidadMedida { ID_UnidadMedida = 3, Nombre = "Libra", Abreviatura = "LB" }
            );

            modelBuilder.Entity<MotivoAjuste>().HasData(
                new MotivoAjuste { ID_Motivo = 1, Descripcion = "Inventario inicial" },
                new MotivoAjuste { ID_Motivo = 2, Descripcion = "Producto vencido" },
                new MotivoAjuste { ID_Motivo = 3, Descripcion = "Producto dañado" },
                new MotivoAjuste { ID_Motivo = 4, Descripcion = "Robo o extravío" },
                new MotivoAjuste { ID_Motivo = 5, Descripcion = "Diferencia en conteo físico" }
            );
        }
    }
}
