// HDHiep
/**
 * Custom application error class to standardize error handling across the application.
 * Includes properties for status code, operational flag, and optional details.
 * Extends the built-in Error class.
 */
export class AppError extends Error {
  public readonly statusCode: number;
  public readonly isOperational: boolean;
  public readonly details?: any;

  constructor(message: string, statusCode = 500, isOperational = true, details?: any) {
    super(message);
    this.statusCode = statusCode;
    this.isOperational = isOperational;
    this.details = details;
    Error.captureStackTrace(this);  // Captures stack trace for where the error was thrown
  }
}

/**
 * Specific error class for 404 Not Found errors.
 * Extends the AppError class with a default message and status code.
 * Used to indicate that a requested resource could not be found.
 * @example
 * throw new NotFoundError('User not found', { userId: 123 });
 */
export class NotFoundError extends AppError {
  constructor(message = 'Resource not found', details?: any) {
    super(message, 404, true, details);
  }
}

/**
 * Specific error class for 400 Bad Request errors.
 * Extends the AppError class with a default message and status code.
 * Used to indicate that the request data is invalid or malformed.
 * @example
 * throw new ValidationError('Invalid input data', { field: 'email' });
 */
export class ValidationError extends AppError {
  constructor(message = "Invalid request data", details?: any) {
    super(message, 400, true, details);
  }
}

/**
 * Specific error class for 401 Unauthorized errors.
 * Extends the AppError class with a default message and status code.
 * Used to indicate that authentication is required and has failed or has not yet been provided.
 * @example
 * throw new AuthError('Authentication required');
 */
export class AuthError extends AppError {
  constructor(message = "Unauthories", details?: any) {
    super(message, 401, true, details);
  }
}

/**
 * Specific error class for 403 Forbidden errors.
 * Extends the AppError class with a default message and status code.
 * Used to indicate that the server understands the request but refuses to authorize it.
 * @example
 * throw new ForbiddenError('Access denied to this resource');
 */
export class ForbiddenError extends AppError {
  constructor(message = "Forbidden", details?: any) {
    super(message, 403, true, details);
  }
}

/**
 * Specific error class for 500 Internal Server errors.
 * Extends the AppError class with a default message and status code.
 * Used to indicate that an unexpected condition was encountered on the server.
 * @example
 * throw new DatabaseError('Database connection failed', { dbHost: 'localhost' });
 */
export class DatabaseError extends AppError {
  constructor(message = "Database error", details?: any) {
    super(message, 500, true, details);
  }
}

/**
 * Specific error class for 429 Too Many Requests errors.
 * Extends the AppError class with a default message and status code.
 * Used to indicate that the user has sent too many requests in a given amount of time.
 * @example
 * throw new RateLimitError('Too many requests, please try again later.');
 * */
export class RateLimitError extends AppError {
  constructor(message = "Too many requests, please try again later.", details?: any) {
    super(message, 429, true, details);
  }
}
