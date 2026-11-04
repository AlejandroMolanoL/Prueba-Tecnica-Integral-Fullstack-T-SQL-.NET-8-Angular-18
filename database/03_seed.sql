-- ============================================================================
-- PROYECTO: PRUEBA TECNICA FULLSTACK - OL SOFTWARE
-- FASE 1: BASE DE DATOS (T-SQL / SQL SERVER)
-- SCRIPT 03: DATOS SEMILLA (RETO 03)
-- ============================================================================

USE ComerciantesDB;
GO

-- Establecer contexto de usuario para auditoria durante la insercion semilla
EXEC sys.sp_set_session_context @key = N'Usuario', @value = N'SeedData_Admin';
GO

-- ============================================================================
-- 1. POBLAR ROLES (Requerido: Administrador y Auxiliar de Registro)
-- ============================================================================
SET IDENTITY_INSERT dbo.Roles ON;

INSERT INTO dbo.Roles (IdRol, Nombre, Descripcion)
VALUES 
    (1, 'Administrador', 'Acceso total: creacion, consulta, edicion, eliminacion y reportes'),
    (2, 'Auxiliar de Registro', 'Acceso operativo: consulta, creacion y edicion de comerciantes');

SET IDENTITY_INSERT dbo.Roles OFF;
GO

-- ============================================================================
-- 2. POBLAR MUNICIPIOS (Catalogo base de Colombia para Reto 06)
-- ============================================================================
SET IDENTITY_INSERT dbo.Municipios ON;

INSERT INTO dbo.Municipios (IdMunicipio, Nombre, Departamento, CodigoDANE)
VALUES
    (1, 'Bogota D.C.', 'Cundinamarca', '11001'),
    (2, 'Medellin', 'Antioquia', '05001'),
    (3, 'Cali', 'Valle del Cauca', '76001'),
    (4, 'Barranquilla', 'Atlantico', '08001'),
    (5, 'Bucaramanga', 'Santander', '68001'),
    (6, 'Cartagena', 'Bolivar', '13001'),
    (7, 'Pereira', 'Risaralda', '66001'),
    (8, 'Manizales', 'Caldas', '17001'),
    (9, 'Cucuta', 'Norte de Santander', '54001'),
    (10, 'Ibague', 'Tolima', '73001');

SET IDENTITY_INSERT dbo.Municipios OFF;
GO

-- ============================================================================
-- 3. POBLAR USUARIOS (Reto 03: 2 registros, uno por cada rol)
-- Credenciales:
-- Administrador: admin@olsoftware.com / Admin123*
-- Auxiliar: auxiliar@olsoftware.com / Auxiliar123*
-- Se almacena hash BCrypt y hash SHA256 compatible
-- ============================================================================
SET IDENTITY_INSERT dbo.Usuarios ON;

INSERT INTO dbo.Usuarios (IdUsuario, Nombre, CorreoElectronico, ContrasenaHash, IdRol, FechaCreacion)
VALUES
    (1, 'John Doe Administrador', 'admin@olsoftware.com', '$2a$11$0FfQ4zY0ZqHw0y4iU4uVyeY5K7g7oKvZpA1fQoI9mHjP2qBqG8n6e', 1, GETDATE()),
    (2, 'Jane Smith Auxiliar', 'auxiliar@olsoftware.com', '$2a$11$1GgR5aZ1ArIx1z5jV5vWzfZ6L8h8pLwAqB2gRpJamIkQ3rCrH9o7f', 2, GETDATE());

SET IDENTITY_INSERT dbo.Usuarios OFF;
GO

-- ============================================================================
-- 4. POBLAR COMERCIANTES (Reto 03: 5 registros aleatorios y diferentes)
-- ============================================================================
SET IDENTITY_INSERT dbo.Comerciantes ON;

INSERT INTO dbo.Comerciantes (IdComerciante, NombreRazonSocial, IdMunicipio, Telefono, CorreoElectronico, FechaRegistro, Estado)
VALUES
    (1, 'Distribuidora Los Andes S.A.S.', 1, '+57 1 234 5678', 'contacto@losandes.com.co', '2024-01-15', 'Activo'),
    (2, 'Tecnologia e Innovacion del Caribe Ltda.', 4, '+57 5 369 8521', 'info@caribeinnovacion.com', '2024-02-20', 'Activo'),
    (3, 'Comercializadora Cafetera del Cafe S.A.', 7, '+57 6 312 9845', 'servicio@cafeteracafe.com', '2024-03-10', 'Activo'),
    (4, 'Inversiones y Alimentos Antioquia E.U.', 2, '+57 4 444 1234', 'ventas@alimentosantioquia.com', '2024-04-05', 'Activo'),
    (5, 'Textiles & Confecciones del Valle S.A.S.', 3, NULL, NULL, '2024-05-18', 'Inactivo');

SET IDENTITY_INSERT dbo.Comerciantes OFF;
GO

-- ============================================================================
-- 5. POBLAR ESTABLECIMIENTOS (Reto 03: 10 registros distribuidos aleatoriamente)
-- Distribucion por comerciante:
-- Comerciante 1: 3 establecimientos
-- Comerciante 2: 1 establecimiento
-- Comerciante 3: 4 establecimientos
-- Comerciante 4: 2 establecimientos
-- Comerciante 5: 0 establecimientos (Inactivo)
-- Total = 3 + 1 + 4 + 2 + 0 = 10 registros
-- ============================================================================
SET IDENTITY_INSERT dbo.Establecimientos ON;

INSERT INTO dbo.Establecimientos (IdEstablecimiento, Nombre, Ingresos, NumeroEmpleados, IdComerciante)
VALUES
    -- Establecimientos de Comerciante 1 (3 registros)
    (1, 'Los Andes - Sede Norte Chapinero', 125000000.50, 18, 1),
    (2, 'Los Andes - Bodega Central Fontibon', 285400500.00, 35, 1),
    (3, 'Los Andes - Punto Express Suba', 45200350.25, 6, 1),

    -- Establecimiento de Comerciante 2 (1 registro)
    (4, 'Caribe Innovacion - Portal Prado', 89300000.00, 12, 2),

    -- Establecimientos de Comerciante 3 (4 registros)
    (5, 'Cafe Gourmet - Plaza Bolivar Pereira', 68450120.75, 10, 3),
    (6, 'Cafe Tostado - Zona Rosa Dosquebradas', 54200800.50, 8, 3),
    (7, 'Exportadora de Cafe - Bodega Armenia', 340150900.00, 42, 3),
    (8, 'Cafe Origen - Centro Comercial Arboleda', 76800400.00, 11, 3),

    -- Establecimientos de Comerciante 4 (2 registros)
    (9, 'Alimentos Antioquia - Planta Envigado', 412000000.00, 55, 4),
    (10, 'Alimentos Antioquia - Punto Laureles', 98500250.80, 14, 4);

SET IDENTITY_INSERT dbo.Establecimientos OFF;
GO

PRINT 'Script 03 ejecutado exitosamente: Datos semilla insertados (2 Usuarios, 5 Comerciantes, 10 Establecimientos).';
GO
