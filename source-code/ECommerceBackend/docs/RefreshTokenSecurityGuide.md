# Refresh Token Security Implementation Guide

## Overview
This document describes the security measures implemented in the refresh token system to protect against token theft and misuse.

## Security Features

### 1. Token Rotation
- Every time a refresh token is used, a new pair of access and refresh tokens is generated
- The old refresh token is marked as `IsUsed = true`
- This prevents token replay attacks

### 2. Token Reuse Detection
When a refresh token that has already been used is presented again:
- **Security Violation Detected**: This indicates potential token theft
- **Immediate Response**: All refresh tokens for that user are revoked
- **Error Returned**: `TokenReuseDetected` error with HTTP 401

### 3. Revoked Token Protection
When a revoked token is used:
- **Additional Security**: All user tokens are revoked as a precaution
- **Assumption**: If someone has a revoked token, there might be a security breach

### 4. Token Expiration
- Access tokens: Short-lived (configurable, default 60 minutes)
- Refresh tokens: Longer-lived (configurable, default 7 days)
- Expired tokens are automatically rejected

## API Endpoints

### Refresh Token
```http
POST /api/authentication/refresh-token
Content-Type: application/json

{
  "refreshToken": "your_refresh_token_here"
}
```

**Success Response (200):**
```json
{
  "accessToken": "new_access_token",
  "refreshToken": "new_refresh_token",
  "accessTokenExpiration": "2025-10-22T10:30:00Z",
  "refreshTokenExpiration": "2025-10-29T10:00:00Z",
  "identityUserId": "user_id"
}
```

**Error Responses:**
- `401 InvalidRefreshToken`: Token not found or invalid
- `401 RefreshTokenExpired`: Token has expired
- `401 RefreshTokenRevoked`: Token has been revoked
- `401 TokenReuseDetected`: **Security violation - all tokens revoked**
- `404 UserNotFound`: Associated user not found

### Logout
```http
POST /api/authentication/logout
Content-Type: application/json

{
  "userId": "user_id_here"
}
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "Logout successful"
}
```

## Security Flow Diagram

```
Client Request with Refresh Token
           ↓
    Token exists in DB?
           ↓ No → Return InvalidRefreshToken
         Yes
           ↓
    Token expired?
           ↓ Yes → Return RefreshTokenExpired
          No
           ↓
    Token revoked?
           ↓ Yes → Revoke ALL tokens → Return RefreshTokenRevoked
          No
           ↓
    Token already used?
           ↓ Yes → 🚨 SECURITY VIOLATION 🚨
          No       ↓
           ↓       Revoke ALL user tokens
    Generate new   ↓
    token pair     Return TokenReuseDetected
           ↓
    Mark old token as used
           ↓
    Save new token to DB
           ↓
    Return new tokens
```

## Best Practices for Frontend

### 1. Token Storage
- Store refresh tokens securely (HttpOnly cookies recommended)
- Never store refresh tokens in localStorage or sessionStorage
- Use secure, sameSite cookies for web applications

### 2. Error Handling
```javascript
// Example error handling
try {
  const response = await refreshToken(currentRefreshToken);
  // Update stored tokens
  updateTokens(response.accessToken, response.refreshToken);
} catch (error) {
  if (error.code === 'TokenReuseDetected') {
    // Security violation - redirect to login immediately
    redirectToLogin();
    showSecurityAlert('Security violation detected. Please log in again.');
  } else if (error.code === 'RefreshTokenExpired') {
    // Normal expiration - redirect to login
    redirectToLogin();
  }
}
```

### 3. Automatic Token Refresh
- Implement automatic token refresh before access token expires
- Handle refresh failures gracefully
- Implement exponential backoff for retry logic

## Database Schema

### RefreshTokens Table
```sql
CREATE TABLE RefreshTokens (
    Id UUID PRIMARY KEY,
    Token TEXT NOT NULL,
    JwtId TEXT NOT NULL,
    CreatedAtUtc TIMESTAMP NOT NULL,
    ExpiresAtUtc TIMESTAMP NOT NULL,
    IsUsed BOOLEAN NOT NULL DEFAULT FALSE,
    IsRevoked BOOLEAN NOT NULL DEFAULT FALSE,
    ReplacedByToken TEXT NULL,
    IdentityUserId TEXT NOT NULL,
    FOREIGN KEY (IdentityUserId) REFERENCES Users(Id)
);
```

## Security Considerations

### 1. Token Theft Scenarios
- **Scenario**: Attacker steals refresh token
- **Detection**: When legitimate user tries to refresh, old token is already used
- **Response**: All tokens revoked, both attacker and user must re-authenticate

### 2. Network Interception
- Always use HTTPS in production
- Implement certificate pinning for mobile apps
- Consider additional encryption for highly sensitive applications

### 3. XSS Protection
- Sanitize all user inputs
- Use Content Security Policy (CSP)
- Store tokens in HttpOnly cookies when possible

### 4. CSRF Protection
- Implement CSRF tokens for state-changing operations
- Use SameSite cookie attributes
- Validate origin headers

## Monitoring and Alerting

### Security Events to Monitor
- Multiple `TokenReuseDetected` errors from same user
- High frequency of token refresh requests
- Refresh attempts with expired tokens
- Geographic anomalies in token usage

### Recommended Alerts
- Alert on `TokenReuseDetected` errors
- Monitor for unusual token refresh patterns
- Track failed authentication attempts

## Configuration

### Environment Variables
```env
JWT_ACCESS_TOKEN_EXPIRATION_MINUTES=60
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7
JWT_ACCESS_TOKEN_SECRET_KEY=your_secret_key
JWT_REFRESH_TOKEN_SECRET_KEY=your_refresh_secret_key
```

### Security Recommendations
- Use different secret keys for access and refresh tokens
- Rotate secret keys periodically
- Use strong, randomly generated keys (minimum 256 bits)
- Store keys securely (Azure Key Vault, AWS Secrets Manager, etc.)
