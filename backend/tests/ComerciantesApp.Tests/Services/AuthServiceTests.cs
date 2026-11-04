using Moq;
using FluentAssertions;
using FluentValidation;
using ComerciantesApp.Application.DTOs.Auth;
using ComerciantesApp.Application.Exceptions;
using ComerciantesApp.Application.Interfaces;
using ComerciantesApp.Application.Services;
using ComerciantesApp.Application.Validators;
using ComerciantesApp.Domain.Constants;
using ComerciantesApp.Domain.Entities;
using ComerciantesApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ComerciantesApp.Tests.Services;

public class AuthServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;
    private readonly IValidator<LoginRequestDto> _validator;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();
        _validator = new LoginRequestValidator();

        _authService = new AuthService(
            _context,
            _passwordHasherMock.Object,
            _jwtGeneratorMock.Object,
            _validator);

        // Seed basic rol and usuario
        var rol = new Rol { IdRol = 1, Nombre = RolesConstants.Administrador };
        _context.Roles.Add(rol);

        var usuario = new Usuario
        {
            IdUsuario = 1,
            Nombre = "Admin Test",
            CorreoElectronico = "admin@test.com",
            ContrasenaHash = "hashed_secret",
            IdRol = 1,
            Rol = rol
        };
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }

    [Fact]
    public async Task LoginAsync_CredencialesValidas_RetornaTokenYUsuario()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            CorreoElectronico = "admin@test.com",
            Contrasena = "Password123*"
        };

        _passwordHasherMock.Setup(p => p.VerifyPassword("Password123*", "hashed_secret"))
            .Returns(true);

        _jwtGeneratorMock.Setup(j => j.GenerateToken(1, "Admin Test", "admin@test.com", RolesConstants.Administrador))
            .Returns(("token_jwt_valido", DateTime.UtcNow.AddHours(1)));

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("token_jwt_valido");
        result.Usuario.Should().NotBeNull();
        result.Usuario.CorreoElectronico.Should().Be("admin@test.com");
        result.Usuario.Rol.Should().Be(RolesConstants.Administrador);
    }

    [Fact]
    public async Task LoginAsync_ContrasenaInvalida_LanzaUnauthorizedAppException()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            CorreoElectronico = "admin@test.com",
            Contrasena = "PasswordIncorrecta"
        };

        _passwordHasherMock.Setup(p => p.VerifyPassword("PasswordIncorrecta", "hashed_secret"))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAppException>(() => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_UsuarioNoExiste_LanzaUnauthorizedAppException()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            CorreoElectronico = "noexiste@test.com",
            Contrasena = "Password123*"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAppException>(() => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_CorreoInvalido_LanzaValidationAppException()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            CorreoElectronico = "correo-invalido",
            Contrasena = "123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationAppException>(() => _authService.LoginAsync(request));
    }
}

