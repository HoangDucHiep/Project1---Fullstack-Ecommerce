using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Abstracts.Authentication;

/// <summary>
/// Authentication-related errors
/// </summary>
public static class AuthenticationErrors
{
    public static readonly Error InvalidRefreshToken = Error.Unauthorized(
        "Authentication.InvalidRefreshToken",
        "The provided refresh token is invalid or expired.");

    public static readonly Error RefreshTokenExpired = Error.Unauthorized(
        "Authentication.RefreshTokenExpired",
        "The refresh token has expired.");

    public static readonly Error RefreshTokenRevoked = Error.Unauthorized(
        "Authentication.RefreshTokenRevoked",
        "The refresh token has been revoked.");

    public static readonly Error RefreshTokenAlreadyUsed = Error.Unauthorized(
        "Authentication.RefreshTokenAlreadyUsed",
        "The refresh token has already been used.");

    public static readonly Error UserNotFound = Error.NotFound(
        "Authentication.UserNotFound",
        "The user associated with the refresh token was not found.");

    public static readonly Error TokenReuseDetected = Error.Unauthorized(
        "Authentication.TokenReuseDetected",
        "Token reuse detected. All refresh tokens have been revoked for security.");
}
