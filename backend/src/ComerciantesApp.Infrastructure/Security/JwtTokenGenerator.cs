using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ComerciantesApp.Application.Interfaces;

namespace ComerciantesApp.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime Expiration) GenerateToken(int userId, string nombre, string email, string rol)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? "ClaveSecretaSuperSeguraParaEvaluacionTecnicaOLSoftware2026_JWT_Key!";
        var issuer = _configuration["Jwt:Issuer"] ?? "ComerciantesAppApi";
        var audience = _configuration["Jwt:Audience"] ?? "ComerciantesAppClient";
        
        // Expiracion de exactamente 1 hora segun Reto 05
        var expiration = DateTime.UtcNow.AddHours(1);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, nombre),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, rol),
            new Claim("role", rol)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(securityToken);

        return (tokenString, expiration);
    }
}

