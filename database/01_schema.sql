-- ============================================================================
-- PROYECTO: PRUEBA TECNICA FULLSTACK - OL SOFTWARE
-- FASE 1: BASE DE DATOS (T-SQL / SQL SERVER)
-- SCRIPT 01: CREACION DE BASE DE DATOS, TABLAS E INDICES (RETO 01 & RETO 02)
-- ============================================================================

-- 1. Crear Base de Datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ComerciantesDB')
BEGIN
    CREATE DATABASE ComerciantesDB;
END
GO

USE ComerciantesDB;
GO

-- 2. Eliminar tablas previas en orden inverso de dependencias si existen
IF OBJECT_ID('dbo.Establecimientos', 'U') IS NOT NULL DROP TABLE dbo.Establecimientos;
IF OBJECT_ID('dbo.Comerciantes', 'U') IS NOT NULL DROP TABLE dbo.Comerciantes;
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID('dbo.Municipios', 'U') IS NOT NULL DROP TABLE dbo.Municipios;
GO

-- ============================================================================
-- TABLA: Roles (Normalizacion de roles de usuario)
-- ============================================================================
CREATE TABLE dbo.Roles (
    IdRol INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(200) NULL,
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (IdRol),
    CONSTRAINT UQ_Roles_Nombre UNIQUE (Nombre)
);
GO

-- ============================================================================
-- TABLA: Usuarios (Reto 01)
-- Identificador unico IDENTITY (Reto 02)
-- Roles requeridos: Administrador o Auxiliar de Registro
-- ============================================================================
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

-- ============================================================================
-- TABLA: Municipios (Normalizacion de catalogo de municipios - Reto 01 y Reto 06)
-- ============================================================================
CREATE TABLE dbo.Municipios (
    IdMunicipio INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Departamento VARCHAR(100) NOT NULL,
    CodigoDANE VARCHAR(10) NULL,
    CONSTRAINT PK_Municipios PRIMARY KEY CLUSTERED (IdMunicipio)
);
GO

-- ============================================================================
-- TABLA: Comerciantes (Reto 01 & Reto 02)
-- ============================================================================
CREATE TABLE dbo.Comerciantes (
    IdComerciante INT IDENTITY(1,1) NOT NULL,
    NombreRazonSocial VARCHAR(150) NOT NULL,
    IdMunicipio INT NOT NULL,
    Telefono VARCHAR(30) NULL,
    CorreoElectronico VARCHAR(150) NULL,
    FechaRegistro DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    Estado VARCHAR(10) NOT NULL DEFAULT 'Activo',
    -- Campos de auditoria obligatorios (Reto 01 & Reto 02)
    FechaActualizacion DATETIME2(7) NULL,
    UsuarioActualizacion VARCHAR(100) NULL,
    CONSTRAINT PK_Comerciantes PRIMARY KEY CLUSTERED (IdComerciante),
    CONSTRAINT FK_Comerciantes_Municipios FOREIGN KEY (IdMunicipio) REFERENCES dbo.Municipios(IdMunicipio),
    CONSTRAINT CK_Comerciantes_Estado CHECK (Estado IN ('Activo', 'Inactivo'))
);
GO

-- ============================================================================
-- TABLA: Establecimientos (Reto 01 & Reto 02)
-- Ingresos con dos decimales DECIMAL(18,2)
-- ============================================================================
CREATE TABLE dbo.Establecimientos (
    IdEstablecimiento INT IDENTITY(1,1) NOT NULL,
    Nombre VARCHAR(150) NOT NULL,
    Ingresos DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    NumeroEmpleados INT NOT NULL DEFAULT 0,
    IdComerciante INT NOT NULL,
    -- Campos de auditoria obligatorios (Reto 01 & Reto 02)
    FechaActualizacion DATETIME2(7) NULL,
    UsuarioActualizacion VARCHAR(100) NULL,
    CONSTRAINT PK_Establecimientos PRIMARY KEY CLUSTERED (IdEstablecimiento),
    CONSTRAINT FK_Establecimientos_Comerciantes FOREIGN KEY (IdComerciante) 
        REFERENCES dbo.Comerciantes(IdComerciante) ON DELETE CASCADE,
    CONSTRAINT CK_Establecimientos_Ingresos CHECK (Ingresos >= 0),
    CONSTRAINT CK_Establecimientos_NumeroEmpleados CHECK (NumeroEmpleados >= 0)
);
GO

-- ============================================================================
-- DEFINICION DE INDICES PARA OPTIMIZACION (REGLAS GENERALES)
-- ============================================================================

-- Indice para busqueda rapida por correo de usuario
CREATE NONCLUSTERED INDEX IX_Usuarios_CorreoElectronico 
ON dbo.Usuarios (CorreoElectronico);
GO

-- Indice para relacion de establecimientos con comerciantes
CREATE NONCLUSTERED INDEX IX_Establecimientos_IdComerciante 
ON dbo.Establecimientos (IdComerciante)
INCLUDE (Ingresos, NumeroEmpleados);
GO

-- Indice compuesto para consulta paginada y reportes de comerciantes
CREATE NONCLUSTERED INDEX IX_Comerciantes_Estado_FechaRegistro 
ON dbo.Comerciantes (Estado, FechaRegistro)
INCLUDE (NombreRazonSocial, IdMunicipio, Telefono, CorreoElectronico);
GO

-- Indice para busqueda por Nombre o Razon Social
CREATE NONCLUSTERED INDEX IX_Comerciantes_NombreRazonSocial 
ON dbo.Comerciantes (NombreRazonSocial);
GO

-- Indice para filtro de Municipios por Nombre y Departamento
CREATE NONCLUSTERED INDEX IX_Municipios_Nombre 
ON dbo.Municipios (Nombre, Departamento);
GO

PRINT 'Script 01 ejecutado exitosamente: Base de datos, tablas e indices creados.';
GO
