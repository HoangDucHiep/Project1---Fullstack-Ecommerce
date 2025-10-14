using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Clock;
using ECommerceBackend.Domain.Abstracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ArgumentNullException = ECommerceBackend.Application.Abstracts.Exceptions.ArgumentNullException;

namespace ECommerceBackend.Infrastructure.Authentication;

// HDHiep - 10/07/2025
/// <summary>
/// JWT Service for generating and validating JWT tokens
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly string _accessTokenSecretKey;
    private readonly string _refreshTokenSecretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;
    private readonly IDateTimeProvider _dateTimeProvider;



    public JwtService(IConfiguration configuration, IDateTimeProvider dateTimeProvider)
    {
        _configuration = configuration;
        _accessTokenSecretKey = _configuration["Jwt:AccessTokenSecretKey"] ?? throw new ArgumentNullException(new Error("env.missing", "Missing Jwt:AccessTokenSecretKey", ErrorType.Failure));
        _refreshTokenSecretKey = _configuration["Jwt:RefreshTokenSecretKey"] ?? throw new ArgumentNullException(new Error("env.missing", "Missing Jwt:RefreshTokenSecretKey", ErrorType.Failure));
        _issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(new Error("env.missing", "Missing Jwt:Issuer", ErrorType.Failure));
        _audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException(new Error("env.missing", "Missing Jwt:Audience", ErrorType.Failure));
        _accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60", System.Globalization.CultureInfo.InvariantCulture);
        _refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7", System.Globalization.CultureInfo.InvariantCulture);
        _dateTimeProvider = dateTimeProvider;
    }

    public string GenerateAccessToken(string userId, string? email = null, string? phoneNumber = null)
    {
        // create key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenSecretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email ?? string.Empty),
            new(ClaimTypes.MobilePhone, phoneNumber ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, _dateTimeProvider.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
            ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: _dateTimeProvider.UtcNow.AddMinutes(_accessTokenExpirationMinutes).UtcDateTime,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshTokenSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            expires: _dateTimeProvider.UtcNow.AddDays(_refreshTokenExpirationDays).UtcDateTime,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int GetAccessTokenExpirationInMinutes()
    {
        return _accessTokenExpirationMinutes;
    }

    public int GetRefreshTokenExpirationInDays()
    {
        return _refreshTokenExpirationDays;
    }

    public string? GetUserIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);
            Claim? userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new ArgumentNullException(new Error("token.invalid", "User ID claim not found in token", ErrorType.Failure));
            }
            return userIdClaim.Value;
        }
        catch
        {
            return null;
        }
    }

    public bool ValidateAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.UTF8.GetBytes(_accessTokenSecretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

}
