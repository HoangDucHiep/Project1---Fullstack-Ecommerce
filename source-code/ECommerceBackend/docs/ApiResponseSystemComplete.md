# API Response System - Complete Implementation Guide

## 📋 Mục lục
1. [Vấn đề ban đầu](#vấn-đề-ban-đầu)
2. [Yêu cầu refactoring](#yêu-cầu-refactoring)
3. [Solution Architecture](#solution-architecture)
4. [Implementation Details](#implementation-details)
5. [TraceId Integration](#traceid-integration)
6. [Validation Errors Flow](#validation-errors-flow)
7. [Complete Request Flow](#complete-request-flow)
8. [Testing & Examples](#testing--examples)

---

## 🎯 Vấn đề ban đầu

### Before Refactoring

**Response không đồng nhất:**

```csharp
// Controller 1
[HttpGet]
public async Task<IActionResult> GetUser(Guid id)
{
    var user = await _userService.GetById(id);
    if (user == null)
    {
        return NotFound("User not found");  // ← String
    }
    return Ok(user);  // ← Direct object
}

// Controller 2
[HttpGet]
public async Task<IActionResult> GetProducts()
{
    var products = await _productService.GetAll();
    return Ok(new { data = products, count = products.Count });  // ← Anonymous object
}

// Controller 3
[HttpPost]
public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
{
    try
    {
        var order = await _orderService.Create(request);
        return Ok(new { success = true, order });  // ← Another format
    }
    catch (ValidationException ex)
    {
        return BadRequest(ex.Errors);  // ← Raw errors
    }
}
```

**Vấn đề:**
- ❌ Mỗi endpoint có format khác nhau
- ❌ Client phải handle nhiều format
- ❌ Không có TraceId để trace lỗi
- ❌ Validation errors không standardized
- ❌ Không có pagination metadata
- ❌ Không thể mở rộng (HATEOAS, etc.)

---

## 📝 Yêu cầu refactoring

### User Requirements

**Format mẫu - Success Response:**
```json
{
  "success": true,
  "message": "Lấy thông tin người dùng thành công",
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "user@example.com",
    "phone": "+84123456789"
  },
  "traceId": "0HNGAS34COM1A:00000001"
}
```

**Format mẫu - Error Response:**
```json
{
  "success": false,
  "message": "Không tìm thấy người dùng",
  "errorCode": "User.NotFound",
  "errors": null,
  "traceId": "0HNGAS34COM1A:00000002"
}
```

**Format mẫu - Validation Error:**
```json
{
  "success": false,
  "message": "One or more validation errors occurred",
  "errorCode": "General.Validation",
  "errors": [
    {
      "code": "Name.Invalid",
      "message": "Bắt buộc phải nhập họ tên.",
      "field": "name"
    },
    {
      "code": "Email.Invalid",
      "message": "Email không hợp lệ.",
      "field": "email"
    }
  ],
  "traceId": "0HNGAS34COM1A:00000003"
}
```

**Format mẫu - Paginated Response:**
```json
{
  "success": true,
  "message": "Lấy danh sách thành công",
  "data": [...],
  "metadata": {
    "pagination": {
      "currentPage": 1,
      "pageSize": 10,
      "totalCount": 50,
      "totalPages": 5,
      "hasPreviousPage": false,
      "hasNextPage": true
    },
    "links": null
  },
  "traceId": "0HNGAS34COM1A:00000004"
}
```

**Yêu cầu kỹ thuật:**
- ✅ Abstract, dễ mở rộng
- ✅ Đơn giản, ít code
- ✅ Không cần base controller
- ✅ Auto validation error mapping
- ✅ TraceId khớp với logs (Seq)
- ✅ Null fields tự động ẩn
- ✅ Support HATEOAS (prepare for future)

---

## 🏗️ Solution Architecture

### High-level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Client Request                          │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                  ASP.NET Core Pipeline                      │
│                                                             │
│  TraceIdFilter.OnActionExecuting()                          │
│    ├─ Enrich logs với RequestId                            │
│    └─ LogContext.PushProperty("RequestId", TraceId)         │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                      Controller                             │
│                                                             │
│  var result = await _sender.Send(command);                  │
│  var response = result.ToResponse("Success");               │
│  return Ok(response);                                       │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                  MediatR Pipeline                           │
│                                                             │
│  1. ValidationPipelineBehavior                              │
│     ├─ FluentValidation validates                          │
│     ├─ Generate error codes (Name.Invalid)                 │
│     └─ Return ValidationError if failed                     │
│                                                             │
│  2. Handler executes                                        │
│     └─ Return Result<T> or Result                          │
│                                                             │
│  3. ExceptionHandlingPipelineBehavior                       │
│     └─ Catch & wrap exceptions                             │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                  ResultExtensions                           │
│                                                             │
│  result.ToResponse() or result.ToPaginatedResponse()        │
│    ├─ If success: Create ApiResponse<T>                    │
│    └─ If failure: Create ApiErrorResponse                  │
│         └─ If ValidationError: Extract field names         │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                  TraceIdFilter.OnActionExecuted()           │
│                                                             │
│  Inject TraceId vào response                                │
│    ├─ ApiResponse<T>.TraceId = TraceIdentifier             │
│    └─ ApiErrorResponse.TraceId = TraceIdentifier           │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│              JSON Serialization & Response                  │
│                                                             │
│  System.Text.Json serializes                                │
│    └─ [JsonIgnore(WhenWritingNull)] hides null fields      │
└─────────────────────────────────────────────────────────────┘
```

### Core Components

#### 1. **ApiResponse.cs** - Response Models

```csharp
// Success response
ApiResponse<T>
  ├─ Success: bool
  ├─ Message: string
  ├─ Data: T?
  ├─ Metadata: ResponseMetadata? (pagination, links)
  └─ TraceId: string

// Error response
ApiErrorResponse
  ├─ Success: bool
  ├─ Message: string
  ├─ ErrorCode: string
  ├─ Errors: List<ErrorDetail>?
  └─ TraceId: string

// Metadata
ResponseMetadata
  ├─ Pagination: PaginationMeta?
  └─ Links: List<LinkDto>?

// Error detail
ErrorDetail
  ├─ Code: string
  ├─ Message: string
  ├─ Field: string?
  └─ InnerErrors: List<ErrorDetail>?
```

#### 2. **ResultExtensions.cs** - Conversion Logic

```csharp
Result<T>.ToResponse()
  → ApiResponse<T> | ApiErrorResponse

Result.ToResponse()
  → ApiResponse<object?> | ApiErrorResponse

Result<PaginationResult<T>>.ToPaginatedResponse()
  → ApiResponse<List<T>> | ApiErrorResponse
```

#### 3. **TraceIdFilter.cs** - TraceId Injection

```csharp
OnActionExecuting()
  → Enrich logs với RequestId

OnActionExecuted()
  → Inject TraceId vào response
```

#### 4. **GlobalExceptionHandler.cs** - Exception Handling

```csharp
Catch exceptions
  → Create ApiErrorResponse
  → Set TraceId
  → Return error response
```

---

## 💻 Implementation Details

### 1. ApiResponse Models

**File:** `src/ECommerceBackend.Application/Contracts/Commons/ApiResponse.cs`

```csharp
using System.Text.Json.Serialization;

namespace ECommerceBackend.Application.Contracts.Commons;

/// <summary>
/// Standard API response for SUCCESS
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseMetadata? Metadata { get; set; }

    public string TraceId { get; set; } = string.Empty;

    // Factory method: Simple success
    public static ApiResponse<T> Ok(T data, string message = "Thành công") => new()
    {
        Success = true,
        Message = message,
        Data = data
    };

    // Factory method: Paginated success
    public static ApiResponse<List<TItem>> Paginated<TItem>(
        PaginationResult<TItem> result,
        string message = "Thành công") => new()
    {
        Success = true,
        Message = message,
        Data = result.Items as dynamic,
        Metadata = new ResponseMetadata
        {
            Pagination = new PaginationMeta
            {
                CurrentPage = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            }
        }
    };
}

/// <summary>
/// Standard API response for ERRORS
/// </summary>
public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ErrorDetail>? Errors { get; set; }

    public string TraceId { get; set; } = string.Empty;

    // Factory method: Create error
    public static ApiErrorResponse Error(
        string errorCode,
        string message,
        List<ErrorDetail>? errors = null) => new()
    {
        Success = false,
        Message = message,
        ErrorCode = errorCode,
        Errors = errors
    };
}

/// <summary>
/// Metadata for pagination and links (HATEOAS)
/// </summary>
public class ResponseMetadata
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PaginationMeta? Pagination { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<LinkDto>? Links { get; set; }
}

/// <summary>
/// Pagination metadata
/// </summary>
public class PaginationMeta
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

/// <summary>
/// Error detail with support for nested errors
/// </summary>
public class ErrorDetail
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Field { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ErrorDetail>? InnerErrors { get; set; }
}

/// <summary>
/// HATEOAS link (for future use)
/// </summary>
public class LinkDto
{
    public string Href { get; set; } = string.Empty;
    public string Rel { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
}
```

**Thiết kế chi tiết:**

1. **Separate Success & Error responses**
   - `ApiResponse<T>` cho success
   - `ApiErrorResponse` cho errors
   - Không mix fields → Clean JSON

2. **JsonIgnore(WhenWritingNull)**
   - Null fields tự động ẩn
   - Response gọn hơn
   - Client không cần handle null

3. **Generic support**
   - `ApiResponse<UserDto>`
   - `ApiResponse<List<ProductDto>>`
   - Type-safe

4. **Extensible**
   - `ResponseMetadata` cho pagination/links
   - `ErrorDetail` có `InnerErrors` (nested)
   - HATEOAS ready

---

### 2. ResultExtensions - Conversion Logic

**File:** `src/ECommerceBackend.Application/Contracts/Commons/ResultExtensions.cs`

```csharp
using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Application.Contracts.Commons;

public static class ResultExtensions
{
    /// <summary>
    /// Convert Result<T> to ApiResponse
    /// </summary>
    public static object ToResponse<T>(
        this Result<T> result,
        string successMessage = "Thành công")
    {
        if (result.IsSuccess)
        {
            return ApiResponse<T>.Ok(result.Value, successMessage);
        }

        return CreateErrorResponse(result.Error);
    }

    /// <summary>
    /// Convert Result (non-generic) to ApiResponse
    /// </summary>
    public static object ToResponse(
        this Result result,
        string successMessage = "Thành công")
    {
        if (result.IsSuccess)
        {
            return ApiResponse<object?>.Ok(null, successMessage);
        }

        return CreateErrorResponse(result.Error);
    }

    /// <summary>
    /// Convert PaginationResult to ApiResponse with metadata
    /// </summary>
    public static object ToPaginatedResponse<T>(
        this Result<PaginationResult<T>> result,
        string successMessage = "Thành công")
    {
        if (result.IsSuccess)
        {
            return ApiResponse<List<T>>.Paginated(result.Value, successMessage);
        }

        return CreateErrorResponse(result.Error);
    }

    /// <summary>
    /// Get HTTP status code from Error
    /// </summary>
    public static int GetStatusCode(this Error error)
    {
        return error.Type.StatusCode;
    }

    /// <summary>
    /// Create error response, handling ValidationError specially
    /// </summary>
    private static ApiErrorResponse CreateErrorResponse(Error error)
    {
        // Special handling for ValidationError
        if (error is ValidationError validationError)
        {
            var errorDetails = validationError.Errors
                .Select(e => new ErrorDetail
                {
                    Code = e.Code,
                    Message = e.Description,
                    Field = ExtractFieldFromErrorCode(e.Code)
                })
                .ToList();

            return ApiErrorResponse.Error(
                validationError.Code,
                validationError.Description,
                errorDetails
            );
        }

        // Standard error
        return ApiErrorResponse.Error(
            error.Code,
            error.Description
        );
    }

    /// <summary>
    /// Extract field name from error code
    /// "Name.Invalid" → "name"
    /// "Email.Duplicate" → "email"
    /// </summary>
    private static string? ExtractFieldFromErrorCode(string errorCode)
    {
        string[] parts = errorCode.Split('.');
        if (parts.Length >= 1)
        {
            string fieldName = parts[0];
            if (fieldName.Length > 0)
            {
                // Convert PascalCase → camelCase
                return char.ToLowerInvariant(fieldName[0]) +
                       (fieldName.Length > 1 ? fieldName[1..] : "");
            }
        }
        return null;
    }
}
```

**Flow chi tiết:**

```csharp
// ========== Success Flow ==========
Result<User>.Success(user)
    ↓
result.ToResponse("Lấy user thành công")
    ↓
result.IsSuccess = true
    ↓
ApiResponse<User>.Ok(user, "Lấy user thành công")
    ↓
new ApiResponse<User>
{
    Success = true,
    Message = "Lấy user thành công",
    Data = user,
    TraceId = "" // Will be set by filter
}

// ========== Error Flow ==========
Result<User>.Failure(Error.NotFound("User.NotFound", "User not found"))
    ↓
result.ToResponse("...")
    ↓
result.IsFailure = true
    ↓
CreateErrorResponse(result.Error)
    ↓
error is NOT ValidationError
    ↓
ApiErrorResponse.Error("User.NotFound", "User not found")
    ↓
new ApiErrorResponse
{
    Success = false,
    Message = "User not found",
    ErrorCode = "User.NotFound",
    Errors = null,
    TraceId = "" // Will be set by filter
}

// ========== ValidationError Flow ==========
Result.Failure(ValidationError)
    ↓
result.ToResponse("...")
    ↓
CreateErrorResponse(validationError)
    ↓
error IS ValidationError
    ↓
Extract errorDetails from validationError.Errors:
  - Code: "Name.Invalid"
  - Message: "Bắt buộc phải nhập họ tên."
  - Field: ExtractFieldFromErrorCode("Name.Invalid") → "name"
    ↓
ApiErrorResponse.Error(
    "General.Validation",
    "One or more validation errors occurred",
    errorDetails
)
    ↓
new ApiErrorResponse
{
    Success = false,
    Message = "One or more validation errors occurred",
    ErrorCode = "General.Validation",
    Errors = [
        {
            Code = "Name.Invalid",
            Message = "Bắt buộc phải nhập họ tên.",
            Field = "name"
        }
    ],
    TraceId = "" // Will be set by filter
}
```

---

### 3. Controller Usage

**File:** `src/ECommerceBackend.Api/Controllers/Addresses/AddressController.cs`

**Before:**
```csharp
[HttpGet("/me/addresses")]
public async Task<IActionResult> GetAddresses([FromQuery] int page, int pageSize)
{
    var query = new GetAddressesQuery(page, pageSize);
    Result<PaginationResult<AddressDto>> result = await _sender.Send(query);

    if (result.IsFailure)
    {
        return StatusCode(result.Error.Type.StatusCode, result.Error);
    }

    return Ok(result.Value);
}
```

**After:**
```csharp
[HttpGet("/me/addresses")]
public async Task<IActionResult> GetAddresses([FromQuery] int page, int pageSize)
{
    var result = await _sender.Send(new GetAddressesQuery(page, pageSize));
    return Ok(result.ToPaginatedResponse("Lấy danh sách địa chỉ thành công"));
}
```

**Giảm từ 9 dòng → 3 dòng (66% ít code hơn)!**

**More examples:**

```csharp
// GET single item
[HttpGet("{id}")]
public async Task<IActionResult> GetAddress(Guid id)
{
    var result = await _sender.Send(new GetAddressQuery(id));
    return Ok(result.ToResponse("Lấy địa chỉ thành công"));
}

// POST with error handling
[HttpPost]
public async Task<IActionResult> CreateAddress([FromBody] CreateAddressRequest request)
{
    var result = await _sender.Send(new CreateAddressCommand(request));
    var response = result.ToResponse("Tạo địa chỉ thành công");

    return result.IsSuccess
        ? Ok(response)
        : StatusCode(result.Error.GetStatusCode(), response);
}

// PUT
[HttpPut("{id}")]
public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressRequest request)
{
    var result = await _sender.Send(new UpdateAddressCommand(id, request));
    return Ok(result.ToResponse("Cập nhật địa chỉ thành công"));
}

// DELETE
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteAddress(Guid id)
{
    var result = await _sender.Send(new DeleteAddressCommand(id));

    return result.IsSuccess
        ? NoContent()
        : StatusCode(result.Error.GetStatusCode(), result.ToResponse());
}
```

---

## 🔍 TraceId Integration

### TraceIdFilter Implementation

**File:** `src/ECommerceBackend.Api/Filters/TraceIdFilter.cs`

```csharp
using System.Reflection;
using ECommerceBackend.Application.Contracts.Commons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace ECommerceBackend.Api.Filters;

public class TraceIdFilter : IActionFilter
{
    // ========== BEFORE action executes ==========
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Push RequestId into Serilog LogContext
        // All logs in this request will have this property
        LogContext.PushProperty("RequestId", context.HttpContext.TraceIdentifier);
    }

    // ========== AFTER action executes ==========
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value != null)
        {
            string traceId = context.HttpContext.TraceIdentifier;

            switch (objectResult.Value)
            {
                case ApiErrorResponse errorResponse:
                    errorResponse.TraceId = traceId;
                    break;

                default:
                    // Generic ApiResponse<T> - use reflection
                    PropertyInfo? prop = objectResult.Value.GetType().GetProperty("TraceId");
                    prop?.SetValue(objectResult.Value, traceId);
                    break;
            }
        }
    }
}
```

### Registration

**File:** `src/ECommerceBackend.Api/Program.cs`

```csharp
builder.Services.AddControllers(options =>
{
    options.Filters.Add<TraceIdFilter>();
});
```

### GlobalExceptionHandler Integration

**File:** `src/ECommerceBackend.Api/Middlewares/GlobalExceptionHandler.cs`

```csharp
public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
{
    _logger.LogError(exception, "Unhandled exception occurred");

    (int statusCode, ApiErrorResponse response) = exception switch
    {
        ApplicationException appEx when appEx.Error is ValidationError validationError =>
            (validationError.Type.StatusCode, CreateValidationErrorResponse(validationError)),
        ApplicationException appEx when appEx.Error is not null =>
            (appEx.Error.Type.StatusCode, ApiErrorResponse.Error(appEx.Error.Code, appEx.Error.Description)),
        _ =>
            (StatusCodes.Status500InternalServerError, ApiErrorResponse.Error("Server.InternalError", "Đã xảy ra lỗi hệ thống"))
    };

    // ✅ Set TraceId from HttpContext
    response.TraceId = httpContext.TraceIdentifier;

    httpContext.Response.StatusCode = statusCode;
    await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

    return true;
}
```

---

## ✅ Validation Errors Flow

### Complete Flow

```
1. Request with invalid data
   POST /me/addresses
   { "name": "", "phone": "invalid" }
        ↓
2. FluentValidation validates
   AddNewAddressCommandValidator.Validate()
        ↓
3. ValidationPipelineBehavior catches failures
   ValidationFailure[] = [
     { PropertyName="Name", ErrorCode="NotEmptyValidator", Message="..." },
     { PropertyName="Phone", ErrorCode="RegularExpressionValidator", Message="..." }
   ]
        ↓
4. Generate error codes
   "NotEmptyValidator" + "Name" → "Name.Invalid"
   "RegularExpressionValidator" + "Phone" → "Phone.Invalid"
        ↓
5. Create ValidationError
   ValidationError {
     Code = "General.Validation",
     Description = "One or more validation errors occurred",
     Errors = [
       Error.Validation("Name.Invalid", "Bắt buộc phải nhập họ tên."),
       Error.Validation("Phone.Invalid", "Số điện thoại không hợp lệ.")
     ]
   }
        ↓
6. Return Result.Failure(ValidationError)
        ↓
7. Controller calls result.ToResponse()
        ↓
8. ResultExtensions.CreateErrorResponse()
   - Detect ValidationError
   - Extract error details
   - Extract field names from codes:
     "Name.Invalid" → "name"
     "Phone.Invalid" → "phone"
        ↓
9. Create ApiErrorResponse
   {
     Success = false,
     Message = "One or more validation errors occurred",
     ErrorCode = "General.Validation",
     Errors = [
       { Code="Name.Invalid", Message="...", Field="name" },
       { Code="Phone.Invalid", Message="...", Field="phone" }
     ]
   }
        ↓
10. TraceIdFilter.OnActionExecuted()
    response.TraceId = "0HNGAS34COM1A:00000001"
        ↓
11. JSON Response
    {
      "success": false,
      "message": "One or more validation errors occurred",
      "errorCode": "General.Validation",
      "errors": [
        {
          "code": "Name.Invalid",
          "message": "Bắt buộc phải nhập họ tên.",
          "field": "name"
        },
        {
          "code": "Phone.Invalid",
          "message": "Số điện thoại không hợp lệ.",
          "field": "phone"
        }
      ],
      "traceId": "0HNGAS34COM1A:00000001"
    }
```

### ValidationPipelineBehavior

**File:** `src/ECommerceBackend.Application/Abstracts/Behaviors/ValidationPipelineBehavior.cs`

```csharp
private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
    new(validationFailures.Select(f =>
    {
        // Convert FluentValidation error codes to standardized format
        // "NotEmptyValidator" → "PropertyName.Invalid"
        // "MinimumLengthValidator" → "PropertyName.Invalid"
        // "RegularExpressionValidator" → "PropertyName.Invalid"
        // Custom codes → Keep as is

        string errorCode = string.IsNullOrEmpty(f.ErrorCode) || f.ErrorCode.EndsWith("Validator")
            ? $"{f.PropertyName}.Invalid"
            : f.ErrorCode;

        return Error.Validation(errorCode, f.ErrorMessage);
    }).ToArray());
```

**Examples:**

```csharp
// Input from FluentValidation
ValidationFailure {
    PropertyName = "Name",
    ErrorCode = "NotEmptyValidator",
    ErrorMessage = "Bắt buộc phải nhập họ tên."
}
// Output
Error.Validation("Name.Invalid", "Bắt buộc phải nhập họ tên.")

// ==========================================

// Input with custom error code
ValidationFailure {
    PropertyName = "Email",
    ErrorCode = "Email.Duplicate",
    ErrorMessage = "Email đã tồn tại."
}
// Output
Error.Validation("Email.Duplicate", "Email đã tồn tại.")

// ==========================================

// Input with MinimumLengthValidator
ValidationFailure {
    PropertyName = "Password",
    ErrorCode = "MinimumLengthValidator",
    ErrorMessage = "Password phải dài ít nhất 8 ký tự."
}
// Output
Error.Validation("Password.Invalid", "Password phải dài ít nhất 8 ký tự.")
```

---

## 🔄 Complete Request Flow

### Scenario: Create Address with Validation Errors

#### Request
```http
POST /me/addresses HTTP/1.1
Content-Type: application/json

{
  "name": "",
  "phone": "abc",
  "province": "",
  "district": ""
}
```

#### Step-by-step Flow

**1. ASP.NET Core receives request**
```
HttpContext.TraceIdentifier = "0HNGAS34COM1A:00000005"
```

**2. TraceIdFilter.OnActionExecuting()**
```csharp
LogContext.PushProperty("RequestId", "0HNGAS34COM1A:00000005");
```

**3. Controller receives request**
```csharp
[HttpPost]
public async Task<IActionResult> AddAddress([FromBody] AddNewAddressRequest request)
{
    var result = await _sender.Send(new AddNewAddressCommand(
        request.Name,    // ""
        request.Phone,   // "abc"
        request.Province, // ""
        request.District  // ""
    ));

    var response = result.ToResponse("Thêm địa chỉ thành công");
    return result.IsSuccess
        ? Ok(response)
        : StatusCode(result.Error.GetStatusCode(), response);
}
```

**4. MediatR Pipeline - ValidationPipelineBehavior**
```csharp
// FluentValidation validates
var validationFailures = await ValidateAsync(request);

// Failures detected:
[
    { PropertyName="Name", ErrorCode="NotEmptyValidator", Message="Bắt buộc phải nhập họ tên." },
    { PropertyName="Phone", ErrorCode="RegularExpressionValidator", Message="Số điện thoại không hợp lệ." },
    { PropertyName="Province", ErrorCode="NotEmptyValidator", Message="Bắt buộc phải nhập tỉnh/thành phố." },
    { PropertyName="District", ErrorCode="NotEmptyValidator", Message="Bắt buộc phải nhập quận/huyện." }
]

// Generate error codes
CreateValidationError(validationFailures)
  → ValidationError {
      Code = "General.Validation",
      Errors = [
        Error.Validation("Name.Invalid", "..."),
        Error.Validation("Phone.Invalid", "..."),
        Error.Validation("Province.Invalid", "..."),
        Error.Validation("District.Invalid", "...")
      ]
  }

// Return failure
return Result.Failure(validationError);
```

**5. Handler NOT executed (validation failed)**

**6. Controller receives Result**
```csharp
result.IsFailure = true
result.Error = ValidationError
```

**7. ResultExtensions.ToResponse()**
```csharp
result.ToResponse("Thêm địa chỉ thành công")
  ↓
result.IsFailure = true
  ↓
CreateErrorResponse(result.Error)
  ↓
error is ValidationError = true
  ↓
Extract errorDetails:
  Code="Name.Invalid" → Field="name"
  Code="Phone.Invalid" → Field="phone"
  Code="Province.Invalid" → Field="province"
  Code="District.Invalid" → Field="district"
  ↓
ApiErrorResponse.Error(
    "General.Validation",
    "One or more validation errors occurred",
    errorDetails
)
```

**8. Controller returns StatusCode**
```csharp
result.Error.GetStatusCode() = 400
return StatusCode(400, response);
```

**9. TraceIdFilter.OnActionExecuted()**
```csharp
response.TraceId = "0HNGAS34COM1A:00000005";
```

**10. JSON Response**
```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "success": false,
  "message": "One or more validation errors occurred",
  "errorCode": "General.Validation",
  "errors": [
    {
      "code": "Name.Invalid",
      "message": "Bắt buộc phải nhập họ tên.",
      "field": "name"
    },
    {
      "code": "Phone.Invalid",
      "message": "Số điện thoại không hợp lệ.",
      "field": "phone"
    },
    {
      "code": "Province.Invalid",
      "message": "Bắt buộc phải nhập tỉnh/thành phố.",
      "field": "province"
    },
    {
      "code": "District.Invalid",
      "message": "Bắt buộc phải nhập quận/huyện.",
      "field": "district"
    }
  ],
  "traceId": "0HNGAS34COM1A:00000005"
}
```

**11. Seq Logs**
```
[14:30:15.123] HTTP POST /me/addresses started
               RequestId: 0HNGAS34COM1A:00000005

[14:30:15.234] Validation failed for AddNewAddressCommand
               RequestId: 0HNGAS34COM1A:00000005
               Errors: ["Name.Invalid", "Phone.Invalid", "Province.Invalid", "District.Invalid"]

[14:30:15.345] HTTP POST /me/addresses completed
               RequestId: 0HNGAS34COM1A:00000005
               StatusCode: 400
               Duration: 222ms
```

**12. Search in Seq**
```
RequestId = "0HNGAS34COM1A:00000005"
```
→ Thấy tất cả logs của request này!

---

## 🧪 Testing & Examples

### Test Case 1: Success Response

**Request:**
```bash
curl -X GET https://localhost:5001/me/addresses \
  -H "Authorization: Bearer {token}"
```

**Response:**
```json
{
  "success": true,
  "message": "Lấy danh sách địa chỉ thành công",
  "data": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "name": "Nguyễn Văn A",
      "phone": "+84123456789",
      "province": "Hà Nội",
      "district": "Ba Đình"
    }
  ],
  "metadata": {
    "pagination": {
      "currentPage": 1,
      "pageSize": 10,
      "totalCount": 1,
      "totalPages": 1,
      "hasPreviousPage": false,
      "hasNextPage": false
    }
  },
  "traceId": "0HNGAS34COM1A:00000001"
}
```

---

### Test Case 2: Validation Error

**Request:**
```bash
curl -X POST https://localhost:5001/me/addresses \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "name": "",
    "phone": "invalid",
    "province": "",
    "district": ""
  }'
```

**Response:**
```json
{
  "success": false,
  "message": "One or more validation errors occurred",
  "errorCode": "General.Validation",
  "errors": [
    {
      "code": "Name.Invalid",
      "message": "Bắt buộc phải nhập họ tên.",
      "field": "name"
    },
    {
      "code": "Phone.Invalid",
      "message": "Số điện thoại không hợp lệ.",
      "field": "phone"
    },
    {
      "code": "Province.Invalid",
      "message": "Bắt buộc phải nhập tỉnh/thành phố.",
      "field": "province"
    },
    {
      "code": "District.Invalid",
      "message": "Bắt buộc phải nhập quận/huyện.",
      "field": "district"
    }
  ],
  "traceId": "0HNGAS34COM1A:00000002"
}
```

---

### Test Case 3: Not Found Error

**Request:**
```bash
curl -X GET https://localhost:5001/me/addresses/123e4567-e89b-12d3-a456-000000000000 \
  -H "Authorization: Bearer {token}"
```

**Response:**
```json
{
  "success": false,
  "message": "Không tìm thấy địa chỉ",
  "errorCode": "Address.NotFound",
  "traceId": "0HNGAS34COM1A:00000003"
}
```

Note: `errors` field không xuất hiện vì null (JsonIgnore)

---

### Test Case 4: Server Error (Exception)

**Scenario:** Database connection fails

**Response:**
```json
{
  "success": false,
  "message": "Đã xảy ra lỗi hệ thống",
  "errorCode": "Server.InternalError",
  "traceId": "0HNGAS34COM1A:00000004"
}
```

**Seq Logs:**
```
[14:30:15] Unhandled exception occurred
           RequestId: 0HNGAS34COM1A:00000004
           Level: Error
           Exception: Npgsql.NpgsqlException
           Message: Connection refused
           StackTrace: ...
```

---

## 📊 Summary

### Architecture Overview

```
Components:
├── ApiResponse.cs (2 classes)
│   ├── ApiResponse<T>
│   └── ApiErrorResponse
├── ResultExtensions.cs
│   ├── ToResponse()
│   ├── ToPaginatedResponse()
│   └── CreateErrorResponse()
├── TraceIdFilter.cs
│   ├── OnActionExecuting() → Enrich logs
│   └── OnActionExecuted() → Inject TraceId
├── GlobalExceptionHandler.cs
│   └── Handle exceptions → Set TraceId
└── ValidationPipelineBehavior.cs
    └── Generate error codes
```

### Key Features

✅ **Standardized Responses**
- Consistent format across all endpoints
- Clean JSON (null fields hidden)

✅ **Validation Errors**
- Auto mapping from FluentValidation
- Field names extracted automatically
- Standardized error codes

✅ **TraceId Integration**
- Response TraceId = Logs RequestId
- Perfect tracing from client to Seq
- Single source of truth: HttpContext.TraceIdentifier

✅ **Simple Usage**
- Just call `.ToResponse()` or `.ToPaginatedResponse()`
- No base controller needed
- Minimal code in controllers

✅ **Extensible**
- Support pagination metadata
- HATEOAS ready (links)
- Nested errors support

### Code Reduction

**Before:** 9+ lines per endpoint  
**After:** 3 lines per endpoint  
**Reduction:** ~66%

### Files Modified

1. ✅ `ApiResponse.cs` - Response models
2. ✅ `ResultExtensions.cs` - Conversion logic
3. ✅ `TraceIdFilter.cs` - TraceId & logging
4. ✅ `GlobalExceptionHandler.cs` - Exception handling
5. ✅ `ValidationPipelineBehavior.cs` - Error codes
6. ✅ Controllers - Use `.ToResponse()`

**Total: ~300 lines of code cho complete API response system! 🎉**

---

## 🎯 Conclusion

Hệ thống API Response đã được refactor hoàn toàn:

1. **Standardized** - Format đồng nhất
2. **Simple** - Dễ sử dụng, ít code
3. **Traceable** - TraceId khớp với logs
4. **Extensible** - Dễ mở rộng
5. **Clean** - Null fields tự động ẩn
6. **Type-safe** - Generic support

**Ready for production! 🚀**

