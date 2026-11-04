# Fase 3: Frontend (Angular 18+ / Signals / Jest)

Aplicación frontend moderna desarrollada en **Angular 18.2** con componentes independientes (*standalone components*), gestión de estado global reactiva con **Angular Signals**, pruebas unitarias con **Jest**, y adherencia a las mejores prácticas de seguridad de **OWASP**.

---

## Estructura del Proyecto

```text
frontend/
├── dist/                                  # Compilado de producción
│   ├── comerciantes-frontend/             # Archivos estáticos optimizados
│   └── comerciantes-frontend-prod.zip     # Compilado comprimido para entrega (Reglas PDF)
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── guards/                    # authGuard, adminGuard, publicGuard
│   │   │   ├── interceptors/              # jwtInterceptor (inyección de Bearer y manejo de 401)
│   │   │   ├── models/                    # user, comerciante, municipio, api-response
│   │   │   ├── services/                  # auth, comerciantes, municipios, notification
│   │   │   └── state/                     # AuthStore y ComerciantesStore (Angular Signals)
│   │   ├── shared/
│   │   │   ├── components/                # Header (con prototipo fiel), Toast (notificaciones)
│   │   │   └── pipes/                     # CurrencyCopPipe (formato monetario colombiano)
│   │   └── features/
│   │       ├── auth/                      # Reto 09: Página de Login
│   │       └── comerciantes/
│   │           ├── list/                  # Reto 10: Home / Lista de Comerciantes (Paginación 5, 10, 15)
│   │           └── form/                  # Reto 11: Formulario Creación/Edición con métricas en Footer
│   ├── environments/                      # Variables de entorno (desarrollo y producción)
│   ├── index.html
│   ├── main.ts
│   └── styles.scss
├── angular.json
├── jest.config.js                         # Configuración Jest
├── setup-jest.ts
└── package.json
```

---

## Implementación de los Retos

### RETO 09: Página de Login
- Formulario reactivo con campos:
  - Correo electrónico (usuario) con validación de obligatoriedad y formato email.
  - Contraseña con validación de obligatoriedad y longitud mínima.
  - **Checkbox obligatorio de aceptación de términos y condiciones** antes de permitir el inicio de sesión.
- **Seguridad OWASP / Anti-debug**: La contraseña no se transmite en texto plano en la capa de transporte/red del navegador (codificación Base64 segura).
- Consumo del endpoint `POST /api/auth/login` (Reto 05).
- Encabezado persistente que muestra `Nombre del Usuario` y `Rol` (`Administrador` o `Auxiliar de Registro`).
- Botones de acceso rápido para pruebas con un solo clic (Admin y Auxiliar).

### RETO 10: Página Home / Lista de Comerciantes
- Tabla interactiva con columnas:
  - Razón Social
  - Teléfono
  - Correo Electrónico
  - Fecha Registro
  - No. Establecimientos
  - Estado (Badge interactivo que permite activar/inactivar mediante `PATCH /api/comerciantes/{id}/estado`)
  - Acciones:
    - **Editar**: Redirige al formulario de edición (Reto 11).
    - **Activar/Inactivar**: Alterna estado inmediatamente.
    - **Eliminar**: Botón visible y ejecutable **únicamente para usuarios con rol `Administrador`**. Abre modal de confirmación antes de eliminar.
- **Paginación configurable**: Selector de ítems por página con opciones exactas **5, 10 y 15**.
- **Filtros en tiempo real**: Por Nombre/Razón Social, Estado y Fecha de Registro.
- **Botón "Descargar Reporte en CSV"**: Visible **únicamente para `Administrador`**. Consume el endpoint del Reto 08 y descarga el archivo `.csv` delimitado por pipe (`|`).
- **Botón "Crear Formulario Nuevo"**: Lleva al formulario del Reto 11.
- Opción de **Cerrar Sesión** en el header superior.

### RETO 11: Formulario (Creación y Actualización)
- Campos del formulario con validaciones reactivas estrictas:
  - Razón Social (Requerido, máx. 150 caracteres).
  - Departamento (Selector que filtra municipios).
  - Ciudad / Municipio (Selector poblado dinámicamente desde `GET /api/municipios` - Reto 06).
  - Teléfono (Opcional, validación de caracteres numéricos/formato).
  - Correo Electrónico (Opcional, formato de email).
  - Fecha de Registro (Datepicker obligatorio).
  - Estado (Selector: Activo / Inactivo).
  - ¿Posee establecimientos? (Checkbox).
- **Footer inferior de Totales (Prototipo Página 10)**:
  - **Exclusivo en modo edición**: Muestra la barra inferior con:
    - `Total Ingresos Formulario: $...` (formateado con separador de miles en pesos colombianos).
    - `Cantidad de empleados: ...` (sumatoria acumulada de los establecimientos cargados en base de datos).
  - Botón "Enviar Formulario" que consume `POST /api/comerciantes` o `PUT /api/comerciantes/{id}`.

---

## Seguridad (Recomendaciones OWASP Implementadas)

1. **Protección de Rutas (Route Guards)**:
   - `authGuard`: Bloquea navegación a rutas privadas si no existe token válido.
   - `adminGuard`: Restringe acciones críticas a administradores.
   - `publicGuard`: Redirige usuarios ya autenticados hacia el dashboard.
2. **Intercepción HTTP Segura**:
   - `jwtInterceptor`: Inyecta automáticamente `Authorization: Bearer <token>` solo en peticiones a la API.
   - Manejo automático de respuestas 401 Unauthorized (limpieza de sesión y redirección).
3. **Almacenamiento Seguro**:
   - Manejo en `sessionStorage` para evitar persistencia indebida tras cerrar la ventana del navegador.
4. **Prevención XSS**:
   - Uso exclusivo de interpolación segura de Angular y data binding reactivo (sin `innerHTML` riesgoso).

---

## Comandos para Ejecución y Pruebas

### 1. Instalar dependencias
```bash
npm install
```

### 2. Iniciar servidor de desarrollo
```bash
npm start
```
La aplicación correrá en `http://localhost:4200/`.

### 3. Ejecutar pruebas unitarias (Jest)
```bash
npm test
```

### 4. Compilación para producción
```bash
npm run build
```
Genera los archivos optimizados en `dist/comerciantes-frontend/` y el archivo comprimido `dist/comerciantes-frontend-prod.zip`.
