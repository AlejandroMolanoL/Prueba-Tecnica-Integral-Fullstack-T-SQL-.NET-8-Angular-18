using ComerciantesApp.Application.DTOs.Auth;

namespace ComerciantesApp.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}

public interface IJwtTokenGenerator
{
    (string Token, DateTime Expiration) GenerateToken(int userId, string nombre, string email, string rol);
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? UserName { get; }
    string? Role { get; }
}

