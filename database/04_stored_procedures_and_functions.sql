-- ============================================================================
-- PROYECTO: PRUEBA TECNICA FULLSTACK - OL SOFTWARE
-- FASE 1: BASE DE DATOS (T-SQL / SQL SERVER)
-- SCRIPT 04: PROCEDIMIENTOS ALMACENADOS Y FUNCIONES (RETO 04)
-- ============================================================================

USE ComerciantesDB;
GO

-- ============================================================================
-- 1. FUNCION DE TABLA EN LINEA: fn_ObtenerReporteComerciantesActivos
-- Retorna la coleccion de comerciantes activos calculando:
--   - Cantidad de Establecimientos
--   - Total de Ingresos (con 2 decimales)
--   - Cantidad de Empleados
-- Ordenados descendentemente por la cantidad de establecimientos asociados.
-- Esta funcion sera consumida tanto directamente como por el Backend (.NET / Reto 08).
-- ============================================================================
IF OBJECT_ID('dbo.fn_ObtenerReporteComerciantesActivos', 'IF') IS NOT NULL
    DROP FUNCTION dbo.fn_ObtenerReporteComerciantesActivos;
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

-- ============================================================================
-- 2. PROCEDIMIENTO ALMACENADO: sp_ObtenerReporteComerciantesActivos
-- Permite invocar el reporte mediante EXEC sp_ObtenerReporteComerciantesActivos
-- garantizando compatibilidad con ambas especificaciones de la consigna.
-- ============================================================================
IF OBJECT_ID('dbo.sp_ObtenerReporteComerciantesActivos', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ObtenerReporteComerciantesActivos;
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

PRINT 'Script 04 ejecutado exitosamente: Funcion y Procedimiento de reporte creados.';
GO
