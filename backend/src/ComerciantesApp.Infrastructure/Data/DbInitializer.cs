using Microsoft.EntityFrameworkCore;
using ComerciantesApp.Application.Interfaces;
using ComerciantesApp.Domain.Constants;
using ComerciantesApp.Domain.Entities;

namespace ComerciantesApp.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        // Si ya hay usuarios registrados, no duplicamos
        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        // 1. Roles
        var rolAdmin = new Rol
        {
            Nombre = RolesConstants.Administrador,
            Descripcion = "Acceso total a la plataforma"
        };

        var rolAuxiliar = new Rol
        {
            Nombre = RolesConstants.AuxiliarDeRegistro,
            Descripcion = "Acceso operativo a la plataforma"
        };

        await context.Roles.AddRangeAsync(rolAdmin, rolAuxiliar);
        await context.SaveChangesAsync();

        // 2. Municipios
        var municipios = new List<Municipio>
        {
            new() { Nombre = "Bogotá D.C.", Departamento = "Cundinamarca", CodigoDANE = "11001" },
            new() { Nombre = "Medellín", Departamento = "Antioquia", CodigoDANE = "05001" },
            new() { Nombre = "Cali", Departamento = "Valle del Cauca", CodigoDANE = "76001" },
            new() { Nombre = "Barranquilla", Departamento = "Atlántico", CodigoDANE = "08001" },
            new() { Nombre = "Bucaramanga", Departamento = "Santander", CodigoDANE = "68001" },
            new() { Nombre = "Cartagena", Departamento = "Bolívar", CodigoDANE = "13001" },
            new() { Nombre = "Pereira", Departamento = "Risaralda", CodigoDANE = "66001" },
            new() { Nombre = "Manizales", Departamento = "Caldas", CodigoDANE = "17001" },
            new() { Nombre = "Cúcuta", Departamento = "Norte de Santander", CodigoDANE = "54001" },
            new() { Nombre = "Ibagué", Departamento = "Tolima", CodigoDANE = "73001" }
        };

        await context.Municipios.AddRangeAsync(municipios);
        await context.SaveChangesAsync();

        // 3. Usuarios (Reto 03: 2 registros, uno por cada rol)
        var adminUser = new Usuario
        {
            Nombre = "John Doe Administrador",
            CorreoElectronico = "admin@olsoftware.com",
            ContrasenaHash = passwordHasher.HashPassword("Admin123*"),
            IdRol = rolAdmin.IdRol,
            FechaCreacion = DateTime.UtcNow
        };

        var auxiliarUser = new Usuario
        {
            Nombre = "Jane Smith Auxiliar",
            CorreoElectronico = "auxiliar@olsoftware.com",
            ContrasenaHash = passwordHasher.HashPassword("Auxiliar123*"),
            IdRol = rolAuxiliar.IdRol,
            FechaCreacion = DateTime.UtcNow
        };

        await context.Usuarios.AddRangeAsync(adminUser, auxiliarUser);
        await context.SaveChangesAsync();

        // 4. Comerciantes (Reto 03: 5 registros aleatorios y diferentes)
        var comerciantes = new List<Comerciante>
        {
            new()
            {
                NombreRazonSocial = "Distribuidora Los Andes S.A.S.",
                IdMunicipio = municipios[0].IdMunicipio,
                Telefono = "+57 1 234 5678",
                CorreoElectronico = "contacto@losandes.com.co",
                FechaRegistro = new DateTime(2024, 1, 15),
                Estado = "Activo",
                FechaActualizacion = DateTime.UtcNow,
                UsuarioActualizacion = "SeedData_Admin"
            },
            new()
            {
                NombreRazonSocial = "Tecnología e Innovación del Caribe Ltda.",
                IdMunicipio = municipios[3].IdMunicipio,
                Telefono = "+57 5 369 8521",
                CorreoElectronico = "info@caribeinnovacion.com",
                FechaRegistro = new DateTime(2024, 2, 20),
                Estado = "Activo",
                FechaActualizacion = DateTime.UtcNow,
                UsuarioActualizacion = "SeedData_Admin"
            },
            new()
            {
                NombreRazonSocial = "Comercializadora Cafetera del Café S.A.",
                IdMunicipio = municipios[6].IdMunicipio,
                Telefono = "+57 6 312 9845",
                CorreoElectronico = "servicio@cafeteracafe.com",
                FechaRegistro = new DateTime(2024, 3, 10),
                Estado = "Activo",
                FechaActualizacion = DateTime.UtcNow,
                UsuarioActualizacion = "SeedData_Admin"
            },
            new()
            {
                NombreRazonSocial = "Inversiones y Alimentos Antioquia E.U.",
                IdMunicipio = municipios[1].IdMunicipio,
                Telefono = "+57 4 444 1234",
                CorreoElectronico = "ventas@alimentosantioquia.com",
                FechaRegistro = new DateTime(2024, 4, 5),
                Estado = "Activo",
                FechaActualizacion = DateTime.UtcNow,
                UsuarioActualizacion = "SeedData_Admin"
            },
            new()
            {
                NombreRazonSocial = "Textiles & Confecciones del Valle S.A.S.",
                IdMunicipio = municipios[2].IdMunicipio,
                Telefono = null,
                CorreoElectronico = null,
                FechaRegistro = new DateTime(2024, 5, 18),
                Estado = "Inactivo",
                FechaActualizacion = DateTime.UtcNow,
                UsuarioActualizacion = "SeedData_Admin"
            }
        };

        await context.Comerciantes.AddRangeAsync(comerciantes);
        await context.SaveChangesAsync();

        // 5. Establecimientos (Reto 03: 10 registros distribuidos aleatoriamente: 3, 1, 4, 2, 0)
        var establecimientos = new List<Establecimiento>
        {
            // Comerciante 1 (3 establecimientos)
            new() { Nombre = "Los Andes - Sede Norte Chapinero", Ingresos = 125000000.50m, NumeroEmpleados = 18, IdComerciante = comerciantes[0].IdComerciante, FechaActualizacion = DateTime.UtcNow, UsuarioActualizacion = "SeedData_Admin" },
            new() { Nombre = "Los Andes - Bodega Central Fontibón", Ingresos = 285400500.00m, NumeroEmpleados = 35, IdComerciante = comerciantes[0].IdComerciante, FechaActualizacion = DateTime.UtcNow, UsuarioActualizacion = "SeedData_Admin" },
            new() { Nombre = "Los Andes - Punto Express Suba", Ingresos = 45200350.25m, NumeroEmpleados = 6, IdComerciante = comerciantes[0].IdComerciante, FechaActualizacion = DateTime.UtcNow, UsuarioActualizacion = "SeedData_Admin" },

            // Comerciante 2 (1 establecimiento)
            new() { Nombre = "Caribe Innovación - Portal Prado", Ingresos = 89300000.00m, NumeroEmpleados = 12, IdComerciante = comerciantes[1].IdComerciante, FechaActualizacion = DateTime.UtcNow, UsuarioActualizacion = "SeedData_Admin" },

            // Comerciante 3 (4 establecimientos)
            new() { Nombre = "Café Gourmet - Plaza Bolívar Pereira", Ingresos = 68450120.75m, NumeroEmpleados = 10, IdComerciante = comerciantes[2].IdComerciante, FechaActualizacion = DateTime.UtcNow, UsuarioActualizacion = "SeedData_Admin" },
            new() { Nombre = "Café Tostado - Zona Rosa Dosquebradas", Ingresos = 54200800.50m, NumeroEmpleados = 8, IdComerciante = comerciantes[2].IdComerciante, FechaActualizacion = DateTime.UtcNow, UsuarioActualizacion = "SeedData_Admin" },
            new() { Nombre = "Exportadora de Café - Bodega Armenia", Ingresos = 340150900.00m, NumeroEmpleados = 42, IdComerciante = comerciantes[2].IdComerciante, FechaActualizacion = DateTime.UtcNow, UsuarioActualizacion = "SeedData_Admin" },
            new() { Nombre = "Café Origen - Centro Comercial Arboleda", Ingresos = 76800400.00m, NumeroEmpleados = 11, IdComerciante = comerciantes[2].IdComerciante, FechaActualizacion = DateTime.UtcNow, UsuarioActualizacion = "SeedData_Admin" },

            // Comerciante 4 (2 establecimientos)
            new() { Nombre = "Alimentos Antioquia - Planta Envigado", Ingresos = 412000000.00m, NumeroEmpleados = 55, IdComerciante = comerciantes[3].IdComerciante, FechaActualizacion = DateTime.UtcNow, UsuarioActualizacion = "SeedData_Admin" },
            new() { Nombre = "Alimentos Antioquia - Punto Laureles", Ingresos = 98500250.80m, NumeroEmpleados = 14, IdComerciante = comerciantes[3].IdComerciante, FechaActualizacion = DateTime.UtcNow, UsuarioActualizacion = "SeedData_Admin" }
        };

        await context.Establecimientos.AddRangeAsync(establecimientos);
        await context.SaveChangesAsync();
    }
}

