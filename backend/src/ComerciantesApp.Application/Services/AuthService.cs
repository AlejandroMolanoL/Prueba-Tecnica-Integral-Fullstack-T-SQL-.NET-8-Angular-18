using Microsoft.EntityFrameworkCore;
using ComerciantesApp.Application.DTOs.Auth;
using ComerciantesApp.Application.Exceptions;
using ComerciantesApp.Application.Interfaces;
using FluentValidation;

namespace ComerciantesApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IValidator<LoginRequestDto> _validator;

    public AuthService(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IValidator<LoginRequestDto> validator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _validator = validator;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ValidationAppException(errors);
        }

        var normalizedEmail = request.CorreoElectronico.Trim().ToLowerInvariant();
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.CorreoElectronico.ToLower() == normalizedEmail, cancellationToken);

        if (usuario == null)
        {
            throw new UnauthorizedAppException("Credenciales inválidas. Correo o contraseña incorrectos.");
        }

        bool isPasswordValid = _passwordHasher.VerifyPassword(request.Contrasena, usuario.ContrasenaHash);
        
        // Soporte para contraseñas transmitidas codificadas desde el frontend (Reto 09: sin enviar datos en bruto)
        if (!isPasswordValid)
        {
            try
            {
                var decodedBytes = Convert.FromBase64String(request.Contrasena);
                var decodedPassword = System.Text.Encoding.UTF8.GetString(decodedBytes);
                if (_passwordHasher.VerifyPassword(decodedPassword, usuario.ContrasenaHash))
                {
                    isPasswordValid = true;
                }
            }
            catch
            {
                // Ignorar error de conversion en caso de no ser Base64
            }
        }

        if (!isPasswordValid)
        {
            throw new UnauthorizedAppException("Credenciales inválidas. Correo o contraseña incorrectos.");
        }

        var (token, expiration) = _jwtTokenGenerator.GenerateToken(
            usuario.IdUsuario,
            usuario.Nombre,
            usuario.CorreoElectronico,
            usuario.Rol.Nombre);

        return new LoginResponseDto
        {
            Token = token,
            Expiracion = expiration,
            Usuario = new UsuarioDto
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                CorreoElectronico = usuario.CorreoElectronico,
                Rol = usuario.Rol.Nombre
            }
        };
    }
}
