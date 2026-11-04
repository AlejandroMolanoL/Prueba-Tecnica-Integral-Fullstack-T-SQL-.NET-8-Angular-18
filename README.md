# Solución Técnica Integral Fullstack - OL Software

Repositorio que contiene la solución completa de la prueba técnica para el gremio nacional de comercio, estructurada en tres fases independientes conforme a las especificaciones solicitadas:

- **Fase 1: Base de Datos (T-SQL / SQL Server)** &rarr; Carpeta [`database/`](./database/)
- **Fase 2: Backend (.NET 8 / Clean Architecture)** &rarr; Carpeta [`backend/`](./backend/)
- **Fase 3: Frontend (Angular 18 / Signals / Jest)** &rarr; Carpeta [`frontend/`](./frontend/)

---

## Matriz de Cumplimiento de Retos

| Reto | Área | Componente | Descripción y Evidencia de Cumplimiento |
|:---:|:---:|:---|:---|
| **RETO 01** | Database | Modelo de Datos | Esquema normalizado en SQL Server con tablas `Roles`, `Usuarios`, `Municipios`, `Comerciantes` y `Establecimientos`. Campos de auditoría incluidos. |
| **RETO 02** | Database | Identificadores y Auditoría | Llaves primarias con `IDENTITY(1,1)`. Triggers DML `AFTER INSERT, UPDATE` que auditan automáticamente `FechaActualizacion` y `UsuarioActualizacion` mediante `SESSION_CONTEXT`. |
| **RETO 03** | Database | Datos Semilla | Inserción de 2 usuarios (uno por rol), 5 comerciantes y 10 establecimientos distribuidos aleatoriamente (3, 1, 4, 2 y 0). |
| **RETO 04** | Database | Reporte de Comerciantes | Función de tabla en línea `fn_ObtenerReporteComerciantesActivos` y Procedimiento `sp_ObtenerReporteComerciantesActivos` ordenados descendentemente por cantidad de establecimientos. |
| **RETO 05** | Backend | Web API - Seguridad | Endpoint público `POST /api/auth/login`. Autenticación con JWT (expiración de 1 hora), autorización por roles (`Administrador` y `Auxiliar de Registro`) y política de CORS. |
| **RETO 06** | Backend | Listas de Valores | Endpoint privado `GET /api/municipios` con caché en memoria (`IMemoryCache`) y respuesta estandarizada `ApiResponse<T>`. |
| **RETO 07** | Backend | CRUD Comerciantes | Endpoints paginados (5 registros por defecto, filtros por nombre, estado y fecha), consulta por Id, creación y edición con auditoría automática, borrado restringido a Admin y método HTTP `PATCH` para estado. |
| **RETO 08** | Backend | Reporte Comerciantes | Endpoint `GET /api/comerciantes/reporte-csv` exclusivo para `Administrador`. Genera archivo plano `.csv` delimitado por pipe (`\|`) consumiendo la función del Reto 04. |
| **RETO 09** | Frontend | Página Login | Interfaz fiel al prototipo con campos requeridos, checkbox obligatorio de términos y condiciones, transporte seguro anti-debug, cabecera persistente con Nombre y Rol, y botones de prueba rápida. |
| **RETO 10** | Frontend | Página Home / Lista | Tabla interactiva con columnas solicitadas, paginación con selector de 5, 10 y 15 ítems, activación/inactivación por PATCH, eliminación exclusiva para Admin con modal de confirmación, descarga de CSV y logout. |
| **RETO 11** | Frontend | Formulario Creación/Edición | Formulario reactivo validado con selectores en cascada (Departamento y Ciudad). **Footer inferior exclusivo en edición** con sumatoria de ingresos y empleados de establecimientos asociados. |

---

## Credenciales de Acceso

| Rol | Correo Electrónico | Contraseña | Permisos |
|---|---|---|---|
| **Administrador** | `admin@olsoftware.com` | `Admin123*` | Total (CRUD, activación/inactivación, borrado físico y descarga de reportes CSV) |
| **Auxiliar de Registro** | `auxiliar@olsoftware.com` | `Auxiliar123*` | Operativo (Consulta, creación, edición y activación/inactivación de comerciantes) |

---

## Guía Rápida de Ejecución

### 1. Base de Datos
Ejecutar el script unificado `00_init_database.sql` o los scripts ordenados (`01` al `04`) ubicados en `database/` en una instancia de SQL Server.

### 2. Backend (.NET 8)
```bash
cd backend
dotnet test                  # Ejecuta las 11 pruebas unitarias xUnit
dotnet run --project src/ComerciantesApp.WebApi
```
Swagger UI disponible en: `http://localhost:5000/`

### 3. Frontend (Angular 18)
```bash
cd frontend
npm install
npm test                     # Ejecuta las 11 pruebas unitarias Jest
npm start                    # Inicia la aplicación en http://localhost:4200/
npm run build                # Compila para producción (disponible también en dist/comerciantes-frontend-prod.zip)
```

---

## Pruebas Automatizadas

- **Backend (xUnit + Moq + FluentAssertions)**: 11 pruebas unitarias pasando exitosamente en servicios de autenticación, comerciantes, municipios, validaciones FluentValidation y generación de archivos CSV.
- **Frontend (Jest + Testing Utilities)**: 11 pruebas unitarias pasando exitosamente en tiendas de estado reactivas (Signals), pipes de formato monetario, componentes y validaciones de formulario.
