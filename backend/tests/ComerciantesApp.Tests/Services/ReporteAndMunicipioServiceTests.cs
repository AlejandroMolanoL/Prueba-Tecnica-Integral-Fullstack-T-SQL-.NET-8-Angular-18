using System.Text;
using FluentAssertions;
using ComerciantesApp.Application.Interfaces;
using ComerciantesApp.Application.Services;
using ComerciantesApp.Domain.Entities;
using ComerciantesApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ComerciantesApp.Tests.Services;

public class ReporteServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly ReporteService _reporteService;

    public ReporteServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _reporteService = new ReporteService(_context);

        // Seed municipio y comerciante activo
        var municipio = new Municipio { IdMunicipio = 1, Nombre = "Medellín", Departamento = "Antioquia" };
        _context.Municipios.Add(municipio);

        var comerciante = new Comerciante
        {
            IdComerciante = 1,
            NombreRazonSocial = "Distribuciones Antioquia",
            IdMunicipio = 1,
            Telefono = "+57 4 1234567",
            CorreoElectronico = "info@antioquia.com",
            FechaRegistro = new DateTime(2024, 1, 1),
            Estado = "Activo"
        };
        _context.Comerciantes.Add(comerciante);

        _context.Establecimientos.Add(new Establecimiento
        {
            IdEstablecimiento = 1,
            Nombre = "Sede Poblado",
            Ingresos = 100000.50m,
            NumeroEmpleados = 10,
            IdComerciante = 1
        });

        _context.SaveChanges();
    }

    [Fact]
    public async Task GenerarReporteCsvPipeAsync_RetornaArchivoConDelimitadorPipeYEncabezados()
    {
        // Act
        var csvBytes = await _reporteService.GenerarReporteCsvPipeAsync();

        // Assert
        csvBytes.Should().NotBeNull();
        csvBytes.Length.Should().BeGreaterThan(0);

        var csvText = Encoding.UTF8.GetString(csvBytes);
        csvText.Should().Contain("Nombre o razon social|Municipio|Telefono|Correo Electronico|Fecha de Registro|Estado|Cantidad de Establecimientos|Total Ingresos|Cantidad de Empleados");
        csvText.Should().Contain("Distribuciones Antioquia|Medellín|+57 4 1234567|info@antioquia.com|2024-01-01|Activo|1|100000.50|10");
    }
}

public class MunicipioServiceTests
{
    [Fact]
    public async Task ObtenerMunicipiosAsync_UtilizaCacheEnMemoria()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Municipios.Add(new Municipio { IdMunicipio = 1, Nombre = "Cali", Departamento = "Valle del Cauca" });
        await context.SaveChangesAsync();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var service = new MunicipioService(context, memoryCache);

        // Act 1: Primer llamado carga desde base de datos y almacena en cache
        var resultado1 = await service.ObtenerMunicipiosAsync();

        // Modificamos directamente la base de datos (agregamos otro municipio)
        context.Municipios.Add(new Municipio { IdMunicipio = 2, Nombre = "Palmira", Departamento = "Valle del Cauca" });
        await context.SaveChangesAsync();

        // Act 2: Segundo llamado debe retornar del cache (aun tendra 1 municipio)
        var resultado2 = await service.ObtenerMunicipiosAsync();

        // Assert
        resultado1.Should().HaveCount(1);
        resultado2.Should().HaveCount(1);
        resultado2.First().Nombre.Should().Be("Cali");
    }
}
