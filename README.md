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
    ├── Database/StoredProcedures 96 procedimientos almacenados, un archivo .sql por módulo
    ├── Migrations/               InitialSetupDb (tablas) + StoredProcedures (crea los SP)
    ├── DTOs/                     RequestDTO (lo que recibe la API) y ResponseDTO (lo que devuelve)
    ├── Helpers/                  SpHelper (llamar SP), ManejadorErroresSql, HashUsuario, ScriptsSql
    ├── Controllers/              19 controladores; solo llaman SP
    ├── WebAPI.http               Peticiones de prueba para ejecutar desde Visual Studio
    └── Program.cs
```

## Cómo crear / actualizar la base de datos

1. Abrir `SistemaInventarioSolution.slnx` en Visual Studio y revisar la cadena de conexión en `WebAPI/appsettings.json`.
2. **Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes**:
   ```
   Update-Database
   ```
   Aplica las dos migraciones que ya vienen en el proyecto:
   - `InitialSetupDb`: crea las 24 tablas y los datos iniciales.
   - `StoredProcedures`: crea los 96 SP leyendo los archivos de `Database/StoredProcedures`.

   > No hace falta `Add-Migration`: las migraciones ya están creadas. Si la base ya tenía `InitialSetupDb`,
   > `Update-Database` solo agrega los SP.
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
| Menú principal | `GET /api/Dashboard/indicadores` · `/ventas-por-dia` · `/top-productos` |
| Parámetros del sistema | `/api/ParametrosSistema` · `/api/Correlativos` · `/api/TiposTransaccion` |
| Proveedores | `/api/Proveedores` |
| Productos | `/api/Productos` · `/api/Categorias` · `/api/UnidadesMedida` |
| Catálogos | `/api/Clientes` · `/api/Almacenes` · `/api/MotivosAjuste` |
| Transacciones (compras, ventas, NC, ND, ajustes) | `GET/POST /api/Transacciones` · `POST /api/Transacciones/{id}/anular` |
| Órdenes de compra | `/api/OrdenesCompra` · `POST /{id}/cancelar` |
| Transferencias | `/api/Transferencias` · `POST /{id}/recibir` · `POST /{id}/rechazar` |
| Registro de movimientos de entrada y salida | `GET /api/Inventario/movimientos` · `GET /api/Inventario/kardex?idProducto=` |
| Listado de existencias | `GET /api/Inventario/existencias` · `/stock-bajo` · `/resumen` · `PUT /limites/{idAlmacen}/{idProducto}` |

## Reglas de negocio (todas dentro de los SP)

- **Borrado lógico**: los DELETE solo ponen `Estado = 0`. Se reactivan con PUT (`Estado = true`) o `PATCH /{id}/activar`.
- **El stock solo cambia por documentos**: `sp_Inventario_RegistrarMovimiento` es el único que modifica `InventarioAlmacen`,
  y siempre deja una fila en `HistorialMovimientos` con el saldo anterior y el actual. Nunca permite stock negativo.
- **Factor_Kardex** decide el movimiento: +1 entra, −1 sale, 0 no mueve (notas de débito).
- **Costo promedio ponderado**: compras y ajustes de entrada recalculan `Producto.Costo`.
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
