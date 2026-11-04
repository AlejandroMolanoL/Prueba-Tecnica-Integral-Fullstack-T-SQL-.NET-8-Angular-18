-- ============================================================================
-- PROYECTO: PRUEBA TECNICA FULLSTACK - OL SOFTWARE
-- FASE 1: BASE DE DATOS (T-SQL / SQL SERVER)
-- SCRIPT 02: TRIGGERS DE AUDITORIA (RETO 02)
-- ============================================================================

USE ComerciantesDB;
GO

-- ============================================================================
-- TRIGGER: TR_Comerciante_Audit
-- Actualiza los campos de auditoria (FechaActualizacion y UsuarioActualizacion)
-- en operaciones de INSERCION (INSERT) y ACTUALIZACION (UPDATE).
-- Obtiene el usuario del contexto de sesion (SESSION_CONTEXT) o de SYSTEM_USER.
-- ============================================================================
IF OBJECT_ID('dbo.TR_Comerciante_Audit', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_Comerciante_Audit;
GO

CREATE TRIGGER dbo.TR_Comerciante_Audit
ON dbo.Comerciantes
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Evitar recursividad infinita si el trigger modifica la misma tabla
    IF TRIGGER_NESTLEVEL() > 1
        RETURN;

    -- Obtener usuario actual del contexto de la aplicacion o del sistema operativo / login
    DECLARE @UsuarioAuditoria VARCHAR(100);
    SET @UsuarioAuditoria = COALESCE(
        CAST(SESSION_CONTEXT(N'Usuario') AS VARCHAR(100)),
        SYSTEM_USER,
        'Sistema'
    );

    -- Actualizacion basada en conjuntos (Set-based) para soportar inserciones y actualizaciones masivas
    UPDATE c
    SET 
        c.FechaActualizacion = GETDATE(),
        c.UsuarioActualizacion = @UsuarioAuditoria
    FROM dbo.Comerciantes c
    INNER JOIN inserted i ON c.IdComerciante = i.IdComerciante;
END;
GO

-- ============================================================================
-- TRIGGER: TR_Establecimiento_Audit
-- Actualiza los campos de auditoria (FechaActualizacion y UsuarioActualizacion)
-- en operaciones de INSERCION (INSERT) y ACTUALIZACION (UPDATE).
-- ============================================================================
IF OBJECT_ID('dbo.TR_Establecimiento_Audit', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_Establecimiento_Audit;
GO

CREATE TRIGGER dbo.TR_Establecimiento_Audit
ON dbo.Establecimientos
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Evitar recursividad infinita
    IF TRIGGER_NESTLEVEL() > 1
        RETURN;

    DECLARE @UsuarioAuditoria VARCHAR(100);
    SET @UsuarioAuditoria = COALESCE(
        CAST(SESSION_CONTEXT(N'Usuario') AS VARCHAR(100)),
        SYSTEM_USER,
        'Sistema'
    );

    -- Actualizacion basada en conjuntos (Set-based)
    UPDATE e
    SET 
        e.FechaActualizacion = GETDATE(),
        e.UsuarioActualizacion = @UsuarioAuditoria
    FROM dbo.Establecimientos e
    INNER JOIN inserted i ON e.IdEstablecimiento = i.IdEstablecimiento;
END;
GO

PRINT 'Script 02 ejecutado exitosamente: Triggers de auditoria creados.';
GO

