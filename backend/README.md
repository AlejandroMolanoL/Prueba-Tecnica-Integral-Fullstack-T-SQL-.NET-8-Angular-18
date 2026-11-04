# Fase 2: Backend (.NET 8 / Clean Architecture)

Solución backend construida bajo los principios de **Clean Architecture**, **SOLID**, **Entity Framework Core**, **JWT Bearer**, **Swagger**, y pruebas unitarias con **xUnit**.

## Estructura del Proyecto

```text
backend/
├── ComerciantesApp.sln
├── Dockerfile
├── .dockerignore
├── Comerciantes_API.postman_collection.json
├── src/
│   ├── ComerciantesApp.Domain/            # Entidades, constantes y lógica de dominio pura
│   │   ├── Constants/RolesConstants.cs
│   │   └── Entities/ (Rol, Usuario, Municipio, Comerciante, Establecimiento, Reporte)
│   ├── ComerciantesApp.Application/       # Casos de uso, DTOs, validaciones, interfaces y servicios
│   │   ├── Common/ (ApiResponse, PagedResult)
│   │   ├── DTOs/ (Auth, Municipios, Comerciantes, Reportes)
│   │   ├── Exceptions/ (AppException, NotFound, Unauthorized, Validation)
│   │   ├── Interfaces/
│   │   ├── Services/ (AuthService, ComercianteService, MunicipioService, ReporteService)
│   │   └── Validators/ (FluentValidation para Login y Comerciantes)
│   ├── ComerciantesApp.Infrastructure/    # Persistencia EF Core, JWT, Seguridad, Cache y Seeder
│   │   ├── Data/ (ApplicationDbContext, DbInitializer)
│   │   └── Security/ (JwtTokenGenerator, PasswordHasher, CurrentUserService)
│   └── ComerciantesApp.WebApi/            # API REST, controladores, middleware y configuración
│       ├── Controllers/ (AuthController, MunicipiosController, ComerciantesController)
│       ├── Middleware/ExceptionHandlingMiddleware.cs
│       ├── Program.cs
│       └── appsettings.json
└── tests/
    └── ComerciantesApp.Tests/             # Pruebas unitarias xUnit, Moq y FluentAssertions
        └── Services/ (AuthServiceTests, ComercianteServiceTests, ReporteAndMunicipioServiceTests)
```

---

## Implementación de los Retos

### RETO 05: Web API - Seguridad
- **Endpoint `POST /api/auth/login`**:
  - Público (`[AllowAnonymous]`).
  - Recibe `correoElectronico` y `contrasena`.
  - Genera token JWT con **1 hora de expiración**.
  - Asigna claims de identidad, nombre, correo y rol (`Administrador` o `Auxiliar de Registro`).
  - Política de CORS habilitada para `http://localhost:4200`.

### RETO 06: Web API - Listas de Valores
- **Endpoint `GET /api/municipios`**:
  - Privado (requiere cabecera `Authorization: Bearer <token>`).
  - Implementa **Caché en Memoria (`IMemoryCache`)** para evitar accesos redundantes a la base de datos.
  - Respuesta HTTP estandarizada (`ApiResponse<T>`).

### RETO 07: Web API - CRUD Comerciante
- **Endpoints**:
  - `GET /api/comerciantes`: Consulta paginada (5 registros por página por defecto), con filtros por `nombreRazonSocial`, `fechaRegistro` y `estado`.
  - `GET /api/comerciantes/{id}`: Consulta por Id (incluye totales de ingresos y empleados calculados).
  - `POST /api/comerciantes`: Creación con validaciones FluentValidation. Modifica campos de auditoría automáticamente a partir del token JWT.
  - `PUT /api/comerciantes/{id}`: Actualización completa con campos de auditoría automáticos.
  - `DELETE /api/comerciantes/{id}`: Eliminación física/cascada. **Protegido exclusivamente para el rol `Administrador`**.
  - `PATCH /api/comerciantes/{id}/estado`: Modificación de estado (`Activo` / `Inactivo`) mediante método HTTP PATCH.

### RETO 08: Web API - Reporte Comerciantes
- **Endpoint `GET /api/comerciantes/reporte-csv`**:
  - Protegido exclusivamente para el rol `Administrador`.
  - Genera un archivo plano `.csv` delimitado por **pipe (`|`)**.
  - Consume la función de tabla en línea `dbo.fn_ObtenerReporteComerciantesActivos()` creada en el Reto 04.

---

## Ejecución y Pruebas

### 1. Compilación
```bash
dotnet build
```

### 2. Ejecutar Pruebas Unitarias (xUnit)
```bash
dotnet test
```

### 3. Ejecutar la Web API localmente
```bash
cd src/ComerciantesApp.WebApi
dotnet run
```
La API estará disponible en `http://localhost:5000` con Swagger cargado en la raíz (`http://localhost:5000/`).

### 4. Credenciales de Prueba
| Rol | Correo Electrónico | Contraseña |
|---|---|---|
| **Administrador** | `admin@olsoftware.com` | `Admin123*` |
| **Auxiliar de Registro** | `auxiliar@olsoftware.com` | `Auxiliar123*` |

### 5. Colección de Postman
Se incluye el archivo `Comerciantes_API.postman_collection.json` listo para importar en Postman. El endpoint de login guarda automáticamente el token JWT en las variables de entorno de la colección.

### 6. Contenedor Docker
```bash
docker build -t comerciantes-api .
docker run -d -p 8080:8080 --name comerciantes-container comerciantes-api
```
