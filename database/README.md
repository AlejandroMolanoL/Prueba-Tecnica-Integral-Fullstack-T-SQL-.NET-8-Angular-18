# Fase 1: Base de Datos (T-SQL / SQL Server)

Este directorio contiene todos los scripts DDL y DML requeridos para la evaluación de Base de Datos según la especificación de la prueba técnica de OL Software.

## Estructura de Scripts y Orden de Ejecución

Los scripts están organizados y numerados secuencialmente para garantizar la correcta replicación en el ambiente de calificación:

| Script | Nombre | Descripción | Reto Asociado |
|---|---|---|---|
| **00** | `00_init_database.sql` | Script unificado que ejecuta la totalidad de objetos, triggers, datos semilla y procedimientos en un solo paso. | Retos 01 al 04 |
| **01** | `01_schema.sql` | Creación de base de datos (`ComerciantesDB`), tablas (`Roles`, `Usuarios`, `Municipios`, `Comerciantes`, `Establecimientos`), llaves foráneas e índices de optimización. | **Reto 01 & 02** |
| **02** | `02_triggers.sql` | Triggers de auditoría DML (`AFTER INSERT, UPDATE`) para `Comerciantes` y `Establecimientos`. | **Reto 02** |
| **03** | `03_seed.sql` | Inserción de datos semilla requeridos: 2 usuarios (uno por rol), 5 comerciantes y 10 establecimientos aleatorios. | **Reto 03** |
| **04** | `04_stored_procedures_and_functions.sql` | Función de tabla en línea `fn_ObtenerReporteComerciantesActivos` y Procedimiento Almacenado `sp_ObtenerReporteComerciantesActivos`. | **Reto 04** |

---

## Detalle Técnico de los Retos

### RETO 01: Modelo de Datos
- **Entidades implementadas**:
  - `Roles`: Normalización de roles (`Administrador`, `Auxiliar de Registro`).
  - `Usuarios`: `IdUsuario` (IDENTITY), `Nombre`, `CorreoElectronico` (UNIQUE), `ContrasenaHash`, `IdRol`, `FechaCreacion`.
  - `Municipios`: Normalización de municipios para soporte del Reto 06 (`IdMunicipio`, `Nombre`, `Departamento`, `CodigoDANE`).
  - `Comerciantes`: `IdComerciante` (IDENTITY), `NombreRazonSocial`, `IdMunicipio` (FK), `Telefono`, `CorreoElectronico`, `FechaRegistro`, `Estado` ('Activo'/'Inactivo'), `FechaActualizacion`, `UsuarioActualizacion`.
  - `Establecimientos`: `IdEstablecimiento` (IDENTITY), `Nombre`, `Ingresos` (`DECIMAL(18,2)`), `NumeroEmpleados` (`INT`), `IdComerciante` (FK con `ON DELETE CASCADE`), `FechaActualizacion`, `UsuarioActualizacion`.

### RETO 02: Identificadores y Auditoría
- Todos los identificadores son de tipo `IDENTITY(1,1)` a nivel de base de datos.
- Triggers `TR_Comerciante_Audit` y `TR_Establecimiento_Audit`:
  - Se ejecutan en operaciones `AFTER INSERT, UPDATE`.
  - Control de recursión con `TRIGGER_NESTLEVEL() > 1`.
  - Capturan el usuario actual mediante `SESSION_CONTEXT(N'Usuario')`, con respaldo en `SYSTEM_USER`.
  - Totalmente compatibles con operaciones por conjunto (*set-based operations*).

### RETO 03: Datos Semilla
- **Usuarios**: 2 registros (1 Administrador y 1 Auxiliar de Registro):
  - `admin@olsoftware.com` (Rol: Administrador, Password: `Admin123*`)
  - `auxiliar@olsoftware.com` (Rol: Auxiliar de Registro, Password: `Auxiliar123*`)
- **Comerciantes**: 5 registros con datos aleatorios y diferentes.
- **Establecimientos**: 10 registros distribuidos aleatoriamente entre los comerciantes (3, 1, 4, 2 y 0 respectivamente).

### RETO 04: Reporte de Comerciantes Activos
- Función `dbo.fn_ObtenerReporteComerciantesActivos()`:
  - Filtra comerciantes con `Estado = 'Activo'`.
  - Calcula `CantidadEstablecimientos` mediante `COUNT(e.IdEstablecimiento)`.
  - Calcula `TotalIngresos` mediante `SUM(e.Ingresos)` formateado a dos decimales.
  - Calcula `CantidadEmpleados` mediante `SUM(e.NumeroEmpleados)`.
  - Ordena de forma descendente por `CantidadEstablecimientos DESC` y `TotalIngresos DESC`.
- Procedimiento Almacenado `dbo.sp_ObtenerReporteComerciantesActivos`:
  - Permite invocar el reporte mediante ejecución directa `EXEC sp_ObtenerReporteComerciantesActivos`.

---

## Instrucciones de Ejecución

1. Abrir **SQL Server Management Studio (SSMS)** o **Azure Data Studio**.
2. Conectarse a la instancia de SQL Server deseada.
3. Ejecutar los scripts en orden (`01_schema.sql` -> `02_triggers.sql` -> `03_seed.sql` -> `04_stored_procedures_and_functions.sql`) o ejecutar directamente el script unificado `00_init_database.sql`.

