using Moq;
using FluentAssertions;
using ComerciantesApp.Application.DTOs.Comerciantes;
using ComerciantesApp.Application.Exceptions;
using ComerciantesApp.Application.Interfaces;
using ComerciantesApp.Application.Services;
using ComerciantesApp.Application.Validators;
using ComerciantesApp.Domain.Entities;
using ComerciantesApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ComerciantesApp.Tests.Services;

public class ComercianteServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly ComercianteService _service;

    public ComercianteServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _currentUserServiceMock.Setup(u => u.UserName).Returns("UsuarioTest");
        _currentUserServiceMock.Setup(u => u.Email).Returns("test@olsoftware.com");

        var crearValidator = new CrearComercianteValidator();
        var actualizarValidator = new ActualizarComercianteValidator();
        var estadoValidator = new ActualizarEstadoValidator();

        _service = new ComercianteService(
            _context,
            _currentUserServiceMock.Object,
            crearValidator,
            actualizarValidator,
            estadoValidator);

        // Seed Municipio
        _context.Municipios.Add(new Municipio
        {
            IdMunicipio = 1,
            Nombre = "Bogotá D.C.",
            Departamento = "Cundinamarca"
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task CrearAsync_DatosValidos_CreaComercianteConAuditoria()
    {
        // Arrange
        var dto = new CrearComercianteDto
        {
            NombreRazonSocial = "Comercializadora Prueba S.A.S.",
            IdMunicipio = 1,
            Telefono = "+57 300 123 4567",
            CorreoElectronico = "contacto@prueba.com",
            FechaRegistro = DateTime.UtcNow.Date,
            Estado = "Activo"
        };

        // Act
        var result = await _service.CrearAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.IdComerciante.Should().BeGreaterThan(0);
        result.NombreRazonSocial.Should().Be("Comercializadora Prueba S.A.S.");
        result.UsuarioActualizacion.Should().Be("UsuarioTest");
        result.FechaActualizacion.Should().NotBeNull();
    }

    [Fact]
    public async Task CrearAsync_MunicipioInexistente_LanzaValidationAppException()
    {
        // Arrange
        var dto = new CrearComercianteDto
        {
            NombreRazonSocial = "Empresa Ficticia",
            IdMunicipio = 999, // Inexistente
            FechaRegistro = DateTime.UtcNow.Date,
            Estado = "Activo"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationAppException>(() => _service.CrearAsync(dto));
    }

    [Fact]
    public async Task ObtenerPaginadoAsync_RetornaPaginacionYCalculosDeEstablecimientos()
    {
        // Arrange
        var comerciante = new Comerciante
        {
            IdComerciante = 10,
            NombreRazonSocial = "Supermercados El Ahorro",
            IdMunicipio = 1,
            FechaRegistro = DateTime.UtcNow.Date,
            Estado = "Activo"
        };
        _context.Comerciantes.Add(comerciante);

        // Asociar 2 establecimientos
        _context.Establecimientos.AddRange(
            new Establecimiento { IdEstablecimiento = 101, Nombre = "Sede 1", Ingresos = 50000.00m, NumeroEmpleados = 5, IdComerciante = 10 },
            new Establecimiento { IdEstablecimiento = 102, Nombre = "Sede 2", Ingresos = 30000.00m, NumeroEmpleados = 3, IdComerciante = 10 }
        );
        await _context.SaveChangesAsync();

        var filtros = new ComercianteFiltrosDto { PageNumber = 1, PageSize = 5 };

        // Act
        var resultado = await _service.ObtenerPaginadoAsync(filtros);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Items.Should().NotBeEmpty();
        var item = resultado.Items.First(x => x.IdComerciante == 10);
        item.CantidadEstablecimientos.Should().Be(2);
        item.TotalIngresos.Should().Be(80000.00m);
        item.CantidadEmpleados.Should().Be(8);
    }

    [Fact]
    public async Task ModificarEstadoAsync_CambiaEstadoCorrectamente()
    {
        // Arrange
        var comerciante = new Comerciante
        {
            IdComerciante = 20,
            NombreRazonSocial = "Inversiones Beta",
            IdMunicipio = 1,
            FechaRegistro = DateTime.UtcNow.Date,
            Estado = "Activo"
        };
        _context.Comerciantes.Add(comerciante);
        await _context.SaveChangesAsync();

        // Act
        var actualizado = await _service.ModificarEstadoAsync(20, "Inactivo");

        // Assert
        actualizado.Estado.Should().Be("Inactivo");
        actualizado.UsuarioActualizacion.Should().Be("UsuarioTest");
    }

    [Fact]
    public async Task EliminarAsync_ComercianteExistente_EliminaCorrectamente()
    {
        // Arrange
        var comerciante = new Comerciante
        {
            IdComerciante = 30,
            NombreRazonSocial = "Para Eliminar",
            IdMunicipio = 1,
            FechaRegistro = DateTime.UtcNow.Date,
            Estado = "Activo"
        };
        _context.Comerciantes.Add(comerciante);
        await _context.SaveChangesAsync();

        // Act
        var eliminado = await _service.EliminarAsync(30);

        // Assert
        eliminado.Should().BeTrue();
        (await _context.Comerciantes.FindAsync(30)).Should().BeNull();
    }
}

