-- ============================================================================
-- PROYECTO: PRUEBA TECNICA FULLSTACK - OL SOFTWARE
-- FASE 1: BASE DE DATOS (T-SQL / SQL SERVER)
-- SCRIPT 00: EJECUCION INTEGRAL UNIFICADA (SCRIPTS 01 AL 04)
-- ============================================================================

-- 1. Creacion de Base de Datos
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ComerciantesDB')
BEGIN
    CREATE DATABASE ComerciantesDB;
END
GO

USE ComerciantesDB;
GO

-- 2. Limpieza de tablas previas si existen
IF OBJECT_ID('dbo.Establecimientos', 'U') IS NOT NULL DROP TABLE dbo.Establecimientos;
IF OBJECT_ID('dbo.Comerciantes', 'U') IS NOT NULL DROP TABLE dbo.Comerciantes;
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID('dbo.Municipios', 'U') IS NOT NULL DROP TABLE dbo.Municipios;
GO

-- 3. Tablas
CREATE TABLE dbo.Roles (
    IdRol INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(200) NULL,
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (IdRol),
    CONSTRAINT UQ_Roles_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE dbo.Usuarios (
    IdUsuario INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    CorreoElectronico VARCHAR(150) NOT NULL,
    ContrasenaHash VARCHAR(255) NOT NULL,
    IdRol INT NOT NULL,
    FechaCreacion DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Usuarios PRIMARY KEY CLUSTERED (IdUsuario),
    CONSTRAINT UQ_Usuarios_CorreoElectronico UNIQUE (CorreoElectronico),
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (IdRol) REFERENCES dbo.Roles(IdRol)
);
GO

CREATE TABLE dbo.Municipios (
    IdMunicipio INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Departamento VARCHAR(100) NOT NULL,
    CodigoDANE VARCHAR(10) NULL,
    CONSTRAINT PK_Municipios PRIMARY KEY CLUSTERED (IdMunicipio)
);
GO

CREATE TABLE dbo.Comerciantes (
    IdComerciante INT IDENTITY(1,1) NOT NULL,
    NombreRazonSocial VARCHAR(150) NOT NULL,
    IdMunicipio INT NOT NULL,
    Telefono VARCHAR(30) NULL,
    CorreoElectronico VARCHAR(150) NULL,
    FechaRegistro DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    Estado VARCHAR(10) NOT NULL DEFAULT 'Activo',
    FechaActualizacion DATETIME2(7) NULL,
    UsuarioActualizacion VARCHAR(100) NULL,
    CONSTRAINT PK_Comerciantes PRIMARY KEY CLUSTERED (IdComerciante),
    CONSTRAINT FK_Comerciantes_Municipios FOREIGN KEY (IdMunicipio) REFERENCES dbo.Municipios(IdMunicipio),
    CONSTRAINT CK_Comerciantes_Estado CHECK (Estado IN ('Activo', 'Inactivo'))
);
GO

CREATE TABLE dbo.Establecimientos (
    IdEstablecimiento INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(150) NOT NULL,
    Ingresos DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    NumeroEmpleados INT NOT NULL DEFAULT 0,
    IdComerciante INT NOT NULL,
    FechaActualizacion DATETIME2(7) NULL,
    UsuarioActualizacion VARCHAR(100) NULL,
    CONSTRAINT PK_Establecimientos PRIMARY KEY CLUSTERED (IdEstablecimiento),
    CONSTRAINT FK_Establecimientos_Comerciantes FOREIGN KEY (IdComerciante) 
        REFERENCES dbo.Comerciantes(IdComerciante) ON DELETE CASCADE,
    CONSTRAINT CK_Establecimientos_Ingresos CHECK (Ingresos >= 0),
    CONSTRAINT CK_Establecimientos_NumeroEmpleados CHECK (NumeroEmpleados >= 0)
);
GO

-- Indices
CREATE NONCLUSTERED INDEX IX_Usuarios_CorreoElectronico ON dbo.Usuarios (CorreoElectronico);
CREATE NONCLUSTERED INDEX IX_Establecimientos_IdComerciante ON dbo.Establecimientos (IdComerciante) INCLUDE (Ingresos, NumeroEmpleados);
CREATE NONCLUSTERED INDEX IX_Comerciantes_Estado_FechaRegistro ON dbo.Comerciantes (Estado, FechaRegistro) INCLUDE (NombreRazonSocial, IdMunicipio, Telefono, CorreoElectronico);
CREATE NONCLUSTERED INDEX IX_Comerciantes_NombreRazonSocial ON dbo.Comerciantes (NombreRazonSocial);
CREATE NONCLUSTERED INDEX IX_Municipios_Nombre ON dbo.Municipios (Nombre, Departamento);
GO

-- 4. Triggers de Auditoria (Reto 02)
IF OBJECT_ID('dbo.TR_Comerciante_Audit', 'TR') IS NOT NULL DROP TRIGGER dbo.TR_Comerciante_Audit;
GO
CREATE TRIGGER dbo.TR_Comerciante_Audit
ON dbo.Comerciantes
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    DECLARE @UsuarioAuditoria VARCHAR(100);
    SET @UsuarioAuditoria = COALESCE(CAST(SESSION_CONTEXT(N'Usuario') AS VARCHAR(100)), SYSTEM_USER, 'Sistema');

    UPDATE c
    SET 
        c.FechaActualizacion = GETDATE(),
        c.UsuarioActualizacion = @UsuarioAuditoria
    FROM dbo.Comerciantes c
    INNER JOIN inserted i ON c.IdComerciante = i.IdComerciante;
END;
GO

IF OBJECT_ID('dbo.TR_Establecimiento_Audit', 'TR') IS NOT NULL DROP TRIGGER dbo.TR_Establecimiento_Audit;
GO
CREATE TRIGGER dbo.TR_Establecimiento_Audit
ON dbo.Establecimientos
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF TRIGGER_NESTLEVEL() > 1 RETURN;

    DECLARE @UsuarioAuditoria VARCHAR(100);
    SET @UsuarioAuditoria = COALESCE(CAST(SESSION_CONTEXT(N'Usuario') AS VARCHAR(100)), SYSTEM_USER, 'Sistema');

    UPDATE e
    SET 
        e.FechaActualizacion = GETDATE(),
        e.UsuarioActualizacion = @UsuarioAuditoria
    FROM dbo.Establecimientos e
    INNER JOIN inserted i ON e.IdEstablecimiento = i.IdEstablecimiento;
END;
GO

-- 5. Datos Semilla (Reto 03)
EXEC sys.sp_set_session_context @key = N'Usuario', @value = N'SeedData_Admin';
GO

SET IDENTITY_INSERT dbo.Roles ON;
INSERT INTO dbo.Roles (IdRol, Nombre, Descripcion)
VALUES 
    (1, 'Administrador', 'Acceso total a la plataforma'),
    (2, 'Auxiliar de Registro', 'Acceso operativo a la plataforma');
SET IDENTITY_INSERT dbo.Roles OFF;
GO

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

SET IDENTITY_INSERT dbo.Usuarios ON;
INSERT INTO dbo.Usuarios (IdUsuario, Nombre, CorreoElectronico, ContrasenaHash, IdRol, FechaCreacion)
VALUES
    (1, 'John Doe Administrador', 'admin@olsoftware.com', '$2a$11$0FfQ4zY0ZqHw0y4iU4uVyeY5K7g7oKvZpA1fQoI9mHjP2qBqG8n6e', 1, GETDATE()),
    (2, 'Jane Smith Auxiliar', 'auxiliar@olsoftware.com', '$2a$11$1GgR5aZ1ArIx1z5jV5vWzfZ6L8h8pLwAqB2gRpJamIkQ3rCrH9o7f', 2, GETDATE());
SET IDENTITY_INSERT dbo.Usuarios OFF;
GO

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

SET IDENTITY_INSERT dbo.Establecimientos ON;
INSERT INTO dbo.Establecimientos (IdEstablecimiento, Nombre, Ingresos, NumeroEmpleados, IdComerciante)
VALUES
    (1, 'Los Andes - Sede Norte Chapinero', 125000000.50, 18, 1),
    (2, 'Los Andes - Bodega Central Fontibon', 285400500.00, 35, 1),
    (3, 'Los Andes - Punto Express Suba', 45200350.25, 6, 1),
    (4, 'Caribe Innovacion - Portal Prado', 89300000.00, 12, 2),
    (5, 'Cafe Gourmet - Plaza Bolivar Pereira', 68450120.75, 10, 3),
    (6, 'Cafe Tostado - Zona Rosa Dosquebradas', 54200800.50, 8, 3),
    (7, 'Exportadora de Cafe - Bodega Armenia', 340150900.00, 42, 3),
    (8, 'Cafe Origen - Centro Comercial Arboleda', 76800400.00, 11, 3),
    (9, 'Alimentos Antioquia - Planta Envigado', 412000000.00, 55, 4),
    (10, 'Alimentos Antioquia - Punto Laureles', 98500250.80, 14, 4);
SET IDENTITY_INSERT dbo.Establecimientos OFF;
GO

-- 6. Funciones y Procedimientos (Reto 04)
IF OBJECT_ID('dbo.fn_ObtenerReporteComerciantesActivos', 'IF') IS NOT NULL DROP FUNCTION dbo.fn_ObtenerReporteComerciantesActivos;
GO
CREATE FUNCTION dbo.fn_ObtenerReporteComerciantesActivos()
RETURNS TABLE
AS
RETURN
(
    SELECT TOP 100 PERCENT
        c.NombreRazonSocial,
        m.Nombre AS Municipio,
        COALESCE(c.Telefono, '') AS Telefono,
        COALESCE(c.CorreoElectronico, '') AS CorreoElectronico,
        c.FechaRegistro,
        c.Estado,
        COUNT(e.IdEstablecimiento) AS CantidadEstablecimientos,
        CAST(COALESCE(SUM(e.Ingresos), 0.00) AS DECIMAL(18,2)) AS TotalIngresos,
        COALESCE(SUM(e.NumeroEmpleados), 0) AS CantidadEmpleados
    FROM dbo.Comerciantes c
    INNER JOIN dbo.Municipios m ON c.IdMunicipio = m.IdMunicipio
    LEFT JOIN dbo.Establecimientos e ON c.IdComerciante = e.IdComerciante
    WHERE c.Estado = 'Activo'
    GROUP BY 
        c.IdComerciante,
        c.NombreRazonSocial,
        m.Nombre,
        c.Telefono,
        c.CorreoElectronico,
        c.FechaRegistro,
        c.Estado
    ORDER BY 
        COUNT(e.IdEstablecimiento) DESC,
        TotalIngresos DESC
);
GO

IF OBJECT_ID('dbo.sp_ObtenerReporteComerciantesActivos', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ObtenerReporteComerciantesActivos;
GO
CREATE PROCEDURE dbo.sp_ObtenerReporteComerciantesActivos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        NombreRazonSocial,
        Municipio,
        Telefono,
        CorreoElectronico,
        FechaRegistro,
        Estado,
        CantidadEstablecimientos,
        TotalIngresos,
        CantidadEmpleados
    FROM dbo.fn_ObtenerReporteComerciantesActivos()
    ORDER BY CantidadEstablecimientos DESC, TotalIngresos DESC;
END;
GO

PRINT 'Inicializacion completa de ComerciantesDB ejecutada con exito.';
GO

