# Sistema de Inventarios – Web API

ASP.NET Core Web API (.NET 10) + Entity Framework Core (**Code First**) + SQL Server.
Sigue la misma estructura del proyecto de clase `APIRestSolution`.
**Toda la comunicación con la base de datos se hace con procedimientos almacenados (SP).**
El frontend está en `../SistemaInventarioFrontend` (Angular).

```
SistemaInventarioSolution/
├── SistemaInventarioSolution.slnx
├── DiagramaER.md                 Diagrama entidad-relación mejorado y lista de cambios
└── WebAPI/
    ├── Models/                   24 clases = 24 tablas (Code First)
    ├── Contexts/                 ApplicationDbContext: DbSet de cada tabla + datos iniciales
    ├── Database/StoredProcedures 102 procedimientos almacenados, un archivo .sql por módulo
    ├── Migrations/               InitialSetupDb (tablas) + StoredProcedures, ProductosPorAlmacen, RolPermisoSP, ExistenciaInicial, EstadoEnFormularios y CostoDeCompra (SP)
    ├── DTOs/                     RequestDTO (lo que recibe la API) y ResponseDTO (lo que devuelve)
    ├── Helpers/                  SpHelper (llamar SP), ManejadorErroresSql, HashUsuario, ScriptsSql
    ├── Controllers/              21 controladores; solo llaman SP
    ├── WebAPI.http               Peticiones de prueba para ejecutar desde Visual Studio
    └── Program.cs
```

## Cómo crear / actualizar la base de datos

1. Abrir `SistemaInventarioSolution.slnx` en Visual Studio y revisar la cadena de conexión en `WebAPI/appsettings.json`.
2. **Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes**:
   ```
   Update-Database
   ```
   Aplica las migraciones que ya vienen en el proyecto:
   - `InitialSetupDb`: crea las 24 tablas y los datos iniciales.
   - `StoredProcedures`, `ProductosPorAlmacen`, `RolPermisoSP`, `ExistenciaInicial`, `EstadoEnFormularios` y `CostoDeCompra`: crean y actualizan los 102 SP leyendo los archivos de `Database/StoredProcedures`.

   > No hace falta `Add-Migration`: las migraciones ya están creadas. Si la base ya tenía `InitialSetupDb`,
   > `Update-Database` solo aplica las que faltan.
3. Ejecutar con F5 y probar con `WebAPI.http` o con el frontend.

### Si después se modifica un SP

Los SP usan `CREATE OR ALTER`, así que se pueden volver a ejecutar sin problema. Hay dos opciones:
- Abrir el `.sql` en SQL Server Management Studio y ejecutarlo, o
- `Add-Migration ActualizarSP`, copiar dentro de `Up()` el mismo `foreach` de la migración `StoredProcedures`, y `Update-Database`.

## Cómo se llama a un SP desde C#

Todos los controladores usan `Helpers/SpHelper.cs`:

```csharp
// SP que devuelve filas -> lista de DTO (una propiedad por cada columna del SELECT)
var productos = await _context.Database.ConsultarAsync<ProductoResponseDTO>("sp_Producto_Listar",
    Parametro("@Buscar", buscar), Parametro("@SoloActivos", true));

// SP que devuelve filas de una tabla del DbContext
var categorias = await _context.Categoria.ConsultarAsync("sp_Categoria_Listar", Parametro("@SoloActivos", false));

// SP que crea un registro y termina con "SELECT @ID AS Value"
var id = await _context.Database.ObtenerIdAsync("sp_Producto_Insertar", ...);

// SP que no devuelve filas (actualizar, eliminar, anular)
await _context.Database.EjecutarAsync("sp_Producto_CambiarEstado", Parametro("@ID_Producto", id), Parametro("@Estado", false));
```

Los detalles de los documentos (líneas de una venta, almacenes de un usuario, permisos de un rol) viajan como
**JSON** en un parámetro `NVARCHAR(MAX)` y el SP los lee con `OPENJSON`.

### Errores

Las reglas de negocio están dentro de los SP. Cuando algo no se cumple el SP hace `THROW`:

| THROW | Respuesta de la API |
|---|---|
| `THROW 50001, N'Stock insuficiente de ...', 1` | **400** `{ "mensaje": "Stock insuficiente de ..." }` |
| `THROW 50404, N'El producto no existe', 1` | **404** `{ "mensaje": "El producto no existe" }` |

La conversión la hace `Helpers/ManejadorErroresSql.cs`, registrado en `Program.cs`.

## Procedimientos almacenados por módulo

| Archivo | SP |
|---|---|
| `01_Internos.sql` | `sp_Correlativo_Siguiente`, `sp_Parametro_ObtenerIva`, `sp_Usuario_ValidarAlmacen`, **`sp_Inventario_RegistrarMovimiento`** (único que modifica el stock y escribe el kardex) |
| `02_Seguridad.sql` | Login (`sp_Auth_*`), Usuarios, Roles y Permisos: `_Listar`, `_Obtener`, `_Insertar`, `_Actualizar`, `_CambiarEstado` / `_Eliminar` |
| `03_Configuracion.sql` | ParametroSistema, Correlativo, TipoTransaccion |
| `04_Catalogos.sql` | Categoria, UnidadMedida, Almacen, MotivoAjuste, Producto, Cliente, Proveedor |
| `05_OrdenesCompra.sql` | `sp_OrdenCompra_Listar`, `_Obtener`, `_Detalle`, `_Guardar` (crear y editar), `_Cancelar` |
| `06_Transacciones.sql` | `sp_Transaccion_Listar`, `_Obtener`, `_Detalle`, **`_Registrar`** (compras, ventas, NC, ND, ajustes), **`_Anular`** |
| `07_Transferencias.sql` | `sp_Transferencia_Listar`, `_Obtener`, `_Detalle`, `_Registrar`, `_Finalizar` (recibir / rechazar) |
| `09_ProductosPorAlmacen.sql` | `sp_InventarioAlmacen_Asignar` (varios productos con su existencia actual y límites), `sp_InventarioAlmacen_Actualizar` (límites y conteo físico), `sp_InventarioAlmacen_Quitar` (solo con existencia 0), `sp_MotivoAjuste_Buscar` |
| `10_RolPermiso.sql` | `sp_RolPermiso_Asignar`, `sp_RolPermiso_Quitar` (el Administrador nunca pierde SEGURIDAD) |
| `08_Reportes.sql` | `sp_Inventario_Existencias`, `_Movimientos` (kardex y entradas/salidas), `_Resumen`, `_ActualizarLimites`, `sp_Dashboard_*` |

## Datos iniciales

| Tabla | Datos |
|---|---|
| TipoTransaccion | COMPRA (+1), VENTA (−1), NC_VENTA (+1), NC_COMPRA (−1), ND_VENTA (0), ND_COMPRA (0), AJUSTE_ENTRADA (+1), AJUSTE_SALIDA (−1) |
| Correlativo | Uno por tipo (COM-, VEN-, NCV-…) + `ORDEN_COMPRA` (OC-) y `TRANSFERENCIA` (TRF-) |
| Rol / Permiso | Administrador (todo), Bodeguero, Vendedor · 8 permisos que el frontend usa para armar el menú |
| Usuario | **admin@inventario.com / Admin123!** (cambiar la contraseña después) |
| Almacen | CENTRAL – Bodega central |
| ParametroSistema | IVA_PORCENTAJE = 12, MONEDA = GTQ, NOMBRE_EMPRESA |
| UnidadMedida / MotivoAjuste | UND, CJA, LB · Inventario inicial, Vencido, Dañado, Robo, Conteo |

## Endpoints

| Módulo (requisito del proyecto) | Ruta |
|---|---|
| Acceso de seguridad | `POST /api/Auth/login` · `/api/Usuarios` · `/api/Roles` · `/api/Permisos` |
| Roles y permisos (tabla RolPermiso) | `GET /api/RolPermiso?idRol=` · `POST` · `DELETE /{idRol}/{idPermiso}` |
| Menú principal | `GET /api/Dashboard/indicadores` · `/ventas-por-dia` · `/top-productos` |
| Parámetros del sistema | `/api/ParametrosSistema` · `/api/Correlativos` · `/api/TiposTransaccion` |
| Proveedores | `/api/Proveedores` |
| Productos | `/api/Productos` · `/api/Categorias` · `/api/UnidadesMedida` |
| Productos por almacén (tabla InventarioAlmacen) | `GET /api/InventarioAlmacen?idAlmacen=` · `POST` · `PUT /{idAlmacen}/{idProducto}` · `DELETE /{idAlmacen}/{idProducto}` |
| Catálogos | `/api/Clientes` · `/api/Almacenes` · `/api/MotivosAjuste` |
| Transacciones (compras, ventas, NC, ND, ajustes) | `GET/POST /api/Transacciones` · `POST /api/Transacciones/{id}/anular` |
| Órdenes de compra | `/api/OrdenesCompra` · `POST /{id}/cancelar` |
| Transferencias | `/api/Transferencias` · `POST /{id}/recibir` · `POST /{id}/rechazar` |
| Registro de movimientos de entrada y salida | `GET /api/Inventario/movimientos` · `GET /api/Inventario/kardex?idProducto=` |
| Listado de existencias | `GET /api/Inventario/existencias` · `/stock-bajo` · `/resumen` · `PUT /limites/{idAlmacen}/{idProducto}` |

## Reglas de negocio (todas dentro de los SP)

- **Estado**: al crear y al editar se puede elegir Activo / Inactivo (el DTO trae `Estado`; al crear, si no viene, queda activo;
  al editar, si no viene, no cambia). También se puede cambiar con DELETE (desactivar) y PUT o `PATCH /{id}/activar`.
  Nunca se borran registros. No se puede desactivar un almacén con existencias ni un rol con usuarios activos.
- **El stock solo cambia por documentos**: `sp_Inventario_RegistrarMovimiento` es el único que modifica `InventarioAlmacen.StockActual`,
  y siempre deja una fila en `HistorialMovimientos` con el saldo anterior y el actual. Nunca permite stock negativo.
- **Productos por almacén**: al asignar un producto se indica su existencia actual, costo y límites. La existencia nunca se
  escribe directo: el SP llama a `sp_Transaccion_Registrar` y genera un ajuste de entrada (motivo "Inventario inicial").
  Al editar, si la existencia contada es distinta se genera un ajuste de entrada o salida por la diferencia
  (motivo "Diferencia en conteo físico"). Todo queda en el kardex y en Documentos. Solo se puede quitar un producto si
  su existencia es 0. Si un documento mueve un producto que no estaba asignado, la fila se crea sola.
- **Roles y permisos**: cada registro de `RolPermiso` da acceso a una parte del menú. El rol Administrador no puede
  perder el permiso SEGURIDAD (si lo perdiera, nadie podría volver a asignar permisos). Los cambios se ven en el menú
  la próxima vez que el usuario inicia sesión.
- **Factor_Kardex** decide el movimiento: +1 entra, −1 sale, 0 no mueve (notas de débito).
- **Costo de compra y precio de venta**: `Producto.Costo` es el costo de compra (se captura y edita en el producto) y
  `Producto.Precio` es el precio de venta. Cada compra (y ajuste de entrada con costo) actualiza el costo de compra con el
  último costo pagado.
- **IVA**: se toma de `ParametroSistema` (IVA_PORCENTAJE del almacén o el global). Los ajustes no llevan IVA.
- **Notas de crédito / débito**: exigen la venta o compra de origen, del mismo cliente/proveedor y almacén.
  Una NC no puede devolver más de lo facturado (descontando NC anteriores).
- **Orden de compra**: no mueve inventario. La COMPRA que la referencia valida lo pendiente y la deja PARCIAL o COMPLETADA.
- **Anulación**: los documentos no se editan ni se borran; se anulan y se generan los movimientos contrarios.
  No se puede anular una venta/compra que tiene notas vigentes.
- **Transferencias**: al crearla sale del origen; al recibirla entra al destino; si se rechaza regresa al origen.
- **Acceso por almacén**: el usuario debe tener asignado el almacén (`UsuarioAlmacen`), excepto el Administrador.
- **Todo o nada**: los SP de movimientos usan `BEGIN TRANSACTION` + `TRY/CATCH`; si algo falla se revierte todo.
- **Contraseñas**: se convierten a hash en C# con `PasswordHasher` (igual que el proyecto de clase); al SP solo llega el hash.

## Pendiente / siguientes pasos

- **JWT**: hoy el frontend envía `ID_Usuario` en cada movimiento. Para producción conviene emitir un token en el
  login y tomar el usuario del token.
