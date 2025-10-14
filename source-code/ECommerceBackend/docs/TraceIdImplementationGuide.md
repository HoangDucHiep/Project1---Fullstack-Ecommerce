# TraceId Implementation - Complete Flow Explanation

## 📋 Mục lục
1. [Vấn đề cần giải quyết](#vấn-đề-cần-giải-quyết)
2. [Solution Overview](#solution-overview)
3. [Request → Response Flow](#request--response-flow)
4. [Code Deep Dive](#code-deep-dive)
5. [Seq Integration](#seq-integration)
6. [Testing & Verification](#testing--verification)

---

## 🎯 Vấn đề cần giải quyết

### Ban đầu:
```json
// API Response
{
  "success": false,
  "message": "Validation failed",
  "traceId": "???"  ← Không có hoặc không khớp với logs
}

// Seq Logs
[14:30:15] Request started | RequestId: "0HNGAS34COM1A:00000001"
[14:30:16] Validation error | RequestId: "0HNGAS34COM1A:00000001"
```

**Vấn đề:**
- ❌ TraceId trong response khác với RequestId trong logs
- ❌ Không thể trace từ response sang logs
- ❌ Debug khó khăn khi user báo lỗi

### Mục tiêu:
```json
// API Response
{
  "success": false,
  "message": "Validation failed",
  "traceId": "0HNGAS34COM1A:00000001"  ← Khớp với Seq
}

// Seq Logs
[14:30:15] Request started | RequestId: "0HNGAS34COM1A:00000001"  ← Giống nhau!
[14:30:16] Validation error | RequestId: "0HNGAS34COM1A:00000001"
```

**Giải pháp:**
- ✅ Dùng `HttpContext.TraceIdentifier` cho cả response và logs
- ✅ Search trong Seq: `RequestId = "0HNGAS34COM1A:00000001"`
- ✅ Perfect tracing từ response → logs

---

## 🏗️ Solution Overview

### Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         Request                             │
│                            ↓                                │
│            ASP.NET Core creates HttpContext                 │
│         HttpContext.TraceIdentifier = "0HNG...001"          │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                    TraceIdFilter                            │
│              OnActionExecuting() chạy                       │
│  LogContext.PushProperty("RequestId", TraceIdentifier)      │
│         → Serilog logs sẽ có RequestId field                │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                    Controller Action                        │
│                 result.ToResponse(...)                      │
│        → Tạo ApiResponse hoặc ApiErrorResponse              │
│              TraceId = "" (chưa set)                        │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                    TraceIdFilter                            │
│               OnActionExecuted() chạy                       │
│       response.TraceId = TraceIdentifier                    │
│         → Response giờ có TraceId field                     │
└─────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────┐
│                      JSON Response                          │
│         { "traceId": "0HNG...001" }                        │
└─────────────────────────────────────────────────────────────┘
```

### Components

1. **`TraceIdFilter.cs`** - Action filter handle cả logs và response
2. **`ApiResponse.cs`** - Response models với TraceId property
3. **`GlobalExceptionHandler.cs`** - Exception handling với TraceId
4. **`HttpContext.TraceIdentifier`** - Source of truth cho TraceId

---

## 🔄 Request → Response Flow

### Step-by-step với ví dụ thực tế

#### 1. Request đến server

```http
POST /me/addresses HTTP/1.1
Host: localhost:5001
Content-Type: application/json

{
  "name": "",
  "phone": "",
  "province": ""
}
```

ASP.NET Core tạo `HttpContext`:
```csharp
HttpContext.TraceIdentifier = "0HNGAS34COM1A:00000001"
```

**Format:** `{ConnectionId}:{RequestNumber}`
- `0HNGAS34COM1A` - Connection ID (unique per TCP connection)
- `00000001` - Request number trong connection này

---

#### 2. TraceIdFilter.OnActionExecuting() chạy

**File:** `src/ECommerceBackend.Api/Filters/TraceIdFilter.cs`

```csharp
public void OnActionExecuting(ActionExecutingContext context)
{
    // Push RequestId vào Serilog LogContext
    LogContext.PushProperty("RequestId", context.HttpContext.TraceIdentifier);
    //                        ↑                ↑
    //                   Field name      Value = "0HNGAS34COM1A:00000001"
}
```

**Điều gì xảy ra:**
- Serilog's `LogContext` là thread-local storage
- Mọi log trong request này sẽ tự động có property `RequestId`
- Không cần manually pass TraceId vào mỗi log statement

**Example logs từ giờ:**
```csharp
_logger.LogInformation("Processing request");
// → Seq sẽ thấy: 
// {
//   "Message": "Processing request",
//   "RequestId": "0HNGAS34COM1A:00000001",  ← Tự động!
//   "Timestamp": "..."
// }
```

---

#### 3. Controller Action execute

**File:** `src/ECommerceBackend.Api/Controllers/Addresses/AddressController.cs`

```csharp
[HttpPost("/me/addresses")]
public async Task<IActionResult> AddNewAddress([FromBody] AddNewAddressRequest request)
{
    var result = await _sender.Send(new AddNewAddressCommand(
        request.Name, 
        request.Phone, 
        ...
    ));
    
    // Nếu validation failed:
    // result.IsFailure = true
    // result.Error = ValidationError với các inner errors
    
    var response = result.ToResponse("Thêm địa chỉ thành công");
    //              ↑
    //    Extension method tạo ApiResponse hoặc ApiErrorResponse
    
    return result.IsSuccess 
        ? Ok(response) 
        : StatusCode(result.Error.GetStatusCode(), response);
}
```

---

#### 4. ResultExtensions.ToResponse() tạo response object

**File:** `src/ECommerceBackend.Application/Contracts/Commons/ResultExtensions.cs`

```csharp
public static object ToResponse(this Result result, string successMessage = "Thành công")
{
    if (result.IsSuccess)
    {
        return ApiResponse<object?>.Ok(null, successMessage);
    }
    
    return CreateErrorResponse(result.Error);
}

private static ApiErrorResponse CreateErrorResponse(Error error)
{
    if (error is ValidationError validationError)
    {
        var errorDetails = validationError.Errors
            .Select(e => new ErrorDetail
            {
                Code = e.Code,              // "Name.Invalid"
                Message = e.Description,     // "Bắt buộc phải nhập họ tên."
                Field = ExtractFieldFromErrorCode(e.Code)  // "name"
            })
            .ToList();

        return ApiErrorResponse.Error(
            validationError.Code,
            validationError.Description,
            errorDetails
        );
    }
    
    return ApiErrorResponse.Error(error.Code, error.Description);
}
```

**Tạo ra object:**
```csharp
new ApiErrorResponse
{
    Success = false,
    Message = "One or more validation errors occurred",
    ErrorCode = "General.Validation",
    Errors = [
        { Code = "Name.Invalid", Message = "...", Field = "name" },
        { Code = "Phone.Invalid", Message = "...", Field = "phone" }
    ],
    TraceId = ""  ← CHƯA CÓ GIÁ TRỊ!
}
```

---

#### 5. Controller return Ok(response)

```csharp
return StatusCode(400, response);
//                     ↑
//          ApiErrorResponse object (TraceId vẫn empty)
```

ASP.NET Core wrap vào `ObjectResult`:
```csharp
new ObjectResult(response)
{
    StatusCode = 400,
    Value = response  ← ApiErrorResponse object
}
```

---

#### 6. TraceIdFilter.OnActionExecuted() chạy

**File:** `src/ECommerceBackend.Api/Filters/TraceIdFilter.cs`

```csharp
public void OnActionExecuted(ActionExecutedContext context)
{
    // Check nếu result là ObjectResult (Ok, BadRequest, StatusCode, etc.)
    if (context.Result is ObjectResult objectResult && objectResult.Value != null)
    {
        string traceId = context.HttpContext.TraceIdentifier;
        //                       ↑
        //          "0HNGAS34COM1A:00000001"

        // Pattern match để set TraceId
        switch (objectResult.Value)
        {
            case ApiErrorResponse errorResponse:
                errorResponse.TraceId = traceId;
                // ✅ SET TraceId!
                break;
                
            default:
                // For generic ApiResponse<T>, dùng reflection
                PropertyInfo? prop = objectResult.Value.GetType().GetProperty("TraceId");
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(objectResult.Value, traceId);
                    // ✅ SET TraceId via reflection!
                }
                break;
        }
    }
}
```

**Sau khi filter chạy:**
```csharp
response.TraceId = "0HNGAS34COM1A:00000001"  ← ĐÃ CÓ!
```

---

#### 7. JSON Serialization

ASP.NET Core serialize response object thành JSON:

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
      "message": "Bắt buộc phải nhập số điện thoại.",
      "field": "phone"
    }
  ],
  "traceId": "0HNGAS34COM1A:00000001"  ← ✅ Có trong response!
}
```

---

#### 8. Response gửi về client

```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

{
  "success": false,
  ...
  "traceId": "0HNGAS34COM1A:00000001"
}
```

---

## 💻 Code Deep Dive

### 1. TraceIdFilter.cs - Core Logic

**File:** `src/ECommerceBackend.Api/Filters/TraceIdFilter.cs`

```csharp
using System.Reflection;
using ECommerceBackend.Application.Contracts.Commons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace ECommerceBackend.Api.Filters;

/// <summary>
/// Action filter that injects TraceId into responses and enriches logs
/// </summary>
public class TraceIdFilter : IActionFilter
{
    // ========== PHASE 1: Trước khi action execute ==========
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Push RequestId vào Serilog LogContext
        // Tất cả logs trong request này sẽ có RequestId property
        LogContext.PushProperty("RequestId", context.HttpContext.TraceIdentifier);
        
        // LogContext sử dụng AsyncLocal<T> internally
        // → Thread-safe, work với async/await
        // → Tự động cleanup khi request kết thúc
    }

    // ========== PHASE 2: Sau khi action execute ==========
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Chỉ process ObjectResult (Ok, BadRequest, StatusCode, etc.)
        // Không process NoContent, RedirectResult, FileResult, etc.
        if (context.Result is ObjectResult objectResult && objectResult.Value != null)
        {
            string traceId = context.HttpContext.TraceIdentifier;

            // Pattern match based on response type
            switch (objectResult.Value)
            {
                // Case 1: ApiErrorResponse - trực tiếp set property
                case ApiErrorResponse errorResponse:
                    errorResponse.TraceId = traceId;
                    break;

                // Case 2: ApiResponse<T> - dùng reflection vì generic
                default:
                    PropertyInfo? prop = objectResult.Value.GetType().GetProperty("TraceId");
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(objectResult.Value, traceId);
                    }
                    break;
            }
        }
    }
}
```

**Tại sao cần reflection cho ApiResponse<T>?**

```csharp
// Case 1: ApiErrorResponse (non-generic)
case ApiErrorResponse errorResponse:
    errorResponse.TraceId = traceId;  // ✅ Compile-time type check
    break;

// Case 2: ApiResponse<T> (generic)
// Không thể viết:
// case ApiResponse<???> response:  // ❌ Không biết T là gì runtime
//     response.TraceId = traceId;

// Phải dùng reflection:
default:
    PropertyInfo? prop = objectResult.Value.GetType().GetProperty("TraceId");
    //                   ↑ Runtime type: ApiResponse<UserDto>, ApiResponse<List<AddressDto>>, etc.
    prop?.SetValue(objectResult.Value, traceId);
    break;
```

---

### 2. ApiResponse.cs - Response Models

**File:** `src/ECommerceBackend.Application/Contracts/Commons/ApiResponse.cs`

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseMetadata? Metadata { get; set; }
    
    public string TraceId { get; set; } = string.Empty;
    //                                    ↑
    //          Default empty, sẽ được set bởi TraceIdFilter

    public static ApiResponse<T> Ok(T data, string message = "Thành công") => new()
    {
        Success = true,
        Message = message,
        Data = data
        // TraceId sẽ được set sau bởi filter
    };

    public static ApiResponse<List<TItem>> Paginated<TItem>(
        PaginationResult<TItem> result, 
        string message = "Thành công") => new()
    {
        Success = true,
        Message = message,
        Data = result.Items as dynamic,
        Metadata = new ResponseMetadata
        {
            Pagination = new PaginationMeta { ... }
        }
    };
}

public class ApiErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ErrorDetail>? Errors { get; set; }
    
    public string TraceId { get; set; } = string.Empty;
    //                                    ↑
    //          Default empty, sẽ được set bởi TraceIdFilter

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
```

**Tại sao `TraceId = string.Empty` thay vì `Guid.NewGuid()`?**

```csharp
// ❌ BAD: Tạo random GUID
public string TraceId { get; set; } = Guid.NewGuid().ToString();
// → TraceId khác với logs
// → Không thể trace

// ✅ GOOD: Empty, sẽ set từ HttpContext.TraceIdentifier
public string TraceId { get; set; } = string.Empty;
// → TraceId khớp với logs
// → Perfect tracing
```

---

### 3. GlobalExceptionHandler.cs - Exception Handling

**File:** `src/ECommerceBackend.Api/Middlewares/GlobalExceptionHandler.cs`

```csharp
public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext, 
    Exception exception, 
    CancellationToken cancellationToken)
{
    _logger.LogError(exception, "Unhandled exception occurred");

    // Create error response based on exception type
    (int statusCode, ApiErrorResponse response) = exception switch
    {
        ApplicationException appEx when appEx.Error is ValidationError validationError =>
            (validationError.Type.StatusCode, CreateValidationErrorResponse(validationError)),
        ApplicationException appEx when appEx.Error is not null =>
            (appEx.Error.Type.StatusCode, ApiErrorResponse.Error(appEx.Error.Code, appEx.Error.Description)),
        ApplicationException appEx =>
            (StatusCodes.Status400BadRequest, ApiErrorResponse.Error("Application.Error", $"Lỗi xử lý yêu cầu: {appEx.RequestName}")),
        _ =>
            (StatusCodes.Status500InternalServerError, ApiErrorResponse.Error("Server.InternalError", "Đã xảy ra lỗi hệ thống"))
    };

    // ✅ Set TraceId từ HttpContext.TraceIdentifier
    response.TraceId = httpContext.TraceIdentifier;
    
    httpContext.Response.StatusCode = statusCode;
    
    // Write response trực tiếp, KHÔNG qua controller
    // → TraceIdFilter KHÔNG chạy cho exceptions!
    // → Phải set TraceId manually ở đây
    await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

    return true;
}
```

**Tại sao GlobalExceptionHandler phải set TraceId manually?**

```
Normal Request Flow:
Request → Filter.OnActionExecuting() → Controller → Filter.OnActionExecuted() → Response
                ↑                                           ↑
          Enrich logs                              Set TraceId in response

Exception Flow:
Request → Filter.OnActionExecuting() → Controller → EXCEPTION!
                ↑                                        ↓
          Enrich logs                        GlobalExceptionHandler
                                                         ↓
                                            Set TraceId manually
                                                         ↓
                                             Write response directly
                                                         ↓
                                       Filter.OnActionExecuted() KHÔNG chạy!
```

---

### 4. ValidationPipelineBehavior.cs - Error Code Generation

**File:** `src/ECommerceBackend.Application/Abstracts/Behaviors/ValidationPipelineBehavior.cs`

```csharp
private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
    new(validationFailures.Select(f =>
    {
        // Convert FluentValidation error codes thành format chuẩn
        // "MinimumLengthValidator" → "Name.Invalid"
        // "RegularExpressionValidator" → "Phone.Invalid"
        // Custom error code → giữ nguyên
        
        string errorCode = string.IsNullOrEmpty(f.ErrorCode) || f.ErrorCode.EndsWith("Validator")
            ? $"{f.PropertyName}.Invalid"
            //   ↑               ↑
            //  "Name"      + ".Invalid" = "Name.Invalid"
            : f.ErrorCode;
            
        return Error.Validation(errorCode, f.ErrorMessage);
    }).ToArray());
```

**Examples:**

```csharp
// Input from FluentValidation:
ValidationFailure {
    PropertyName = "Name",
    ErrorCode = "NotEmptyValidator",
    ErrorMessage = "Bắt buộc phải nhập họ tên."
}

// Output:
Error.Validation("Name.Invalid", "Bắt buộc phải nhập họ tên.")
//                ↑
//          PropertyName + ".Invalid"

// ========================================

// Input with custom error code:
ValidationFailure {
    PropertyName = "Email",
    ErrorCode = "Email.Duplicate",  // Custom
    ErrorMessage = "Email đã tồn tại."
}

// Output:
Error.Validation("Email.Duplicate", "Email đã tồn tại.")
//                ↑
//          Giữ nguyên custom code
```

---

### 5. ResultExtensions.ExtractFieldFromErrorCode()

**File:** `src/ECommerceBackend.Application/Contracts/Commons/ResultExtensions.cs`

```csharp
private static string? ExtractFieldFromErrorCode(string errorCode)
{
    // Extract field name from error code
    // "Name.Invalid" → "name"
    // "Email.Duplicate" → "email"
    
    string[] parts = errorCode.Split('.');
    //                                  ↑
    //                          ["Name", "Invalid"]
    
    if (parts.Length >= 1)
    {
        string fieldName = parts[0];  // "Name"
        
        if (fieldName.Length > 0)
        {
            // Convert PascalCase → camelCase
            return char.ToLowerInvariant(fieldName[0]) + 
                   (fieldName.Length > 1 ? fieldName[1..] : "");
            //     ↑                           ↑
            //    "n"              +          "ame"  = "name"
        }
    }
    
    return null;
}
```

**Examples:**

```csharp
ExtractFieldFromErrorCode("Name.Invalid")
// → Split: ["Name", "Invalid"]
// → fieldName: "Name"
// → ToLower first char: "n"
// → Append rest: "name"
// ✅ Result: "name"

ExtractFieldFromErrorCode("Email.Duplicate")
// ✅ Result: "email"

ExtractFieldFromErrorCode("PhoneNumber.Invalid")
// ✅ Result: "phoneNumber"

ExtractFieldFromErrorCode("Province.Required")
// ✅ Result: "province"
```

---

## 🔍 Seq Integration

### Serilog → Seq Flow

```
┌──────────────────────────────────────────────────────────────┐
│  1. Filter.OnActionExecuting()                               │
│     LogContext.PushProperty("RequestId", TraceIdentifier)    │
│                                                              │
│     ┌─────────────────────────────────────────┐             │
│     │  AsyncLocal<LogContext>                 │             │
│     │  RequestId: "0HNGAS34COM1A:00000001"    │             │
│     └─────────────────────────────────────────┘             │
└──────────────────────────────────────────────────────────────┘
                             ↓
┌──────────────────────────────────────────────────────────────┐
│  2. Application logs                                         │
│     _logger.LogInformation("Processing...");                 │
│     _logger.LogError(ex, "Failed");                          │
│                                                              │
│     Serilog automatically attaches LogContext properties:    │
│     {                                                        │
│       "Message": "Processing...",                            │
│       "RequestId": "0HNGAS34COM1A:00000001",  ← Auto!        │
│       "Timestamp": "2025-10-14T14:30:15.123Z"                │
│     }                                                        │
└──────────────────────────────────────────────────────────────┘
                             ↓
┌──────────────────────────────────────────────────────────────┐
│  3. Serilog sends to Seq                                     │
│     WriteTo.Seq("http://seq:5341")                           │
│                                                              │
│     POST http://seq:5341/api/events/raw                      │
│     {                                                        │
│       "Events": [                                            │
│         {                                                    │
│           "Timestamp": "...",                                │
│           "Level": "Information",                            │
│           "MessageTemplate": "Processing...",                │
│           "Properties": {                                    │
│             "RequestId": "0HNGAS34COM1A:00000001",           │
│             "Application": "Ecommerce.Api",                  │
│             "MachineName": "...",                            │
│             "ThreadId": 42                                   │
│           }                                                  │
│         }                                                    │
│       ]                                                      │
│     }                                                        │
└──────────────────────────────────────────────────────────────┘
                             ↓
┌──────────────────────────────────────────────────────────────┐
│  4. Seq indexes events                                       │
│     Events searchable by:                                    │
│     - RequestId                                              │
│     - Application                                            │
│     - Level                                                  │
│     - Timestamp                                              │
│     - MessageTemplate                                        │
│     - Any custom properties                                  │
└──────────────────────────────────────────────────────────────┘
```

### Search trong Seq

**Search query:**
```
RequestId = "0HNGAS34COM1A:00000001"
```

**Kết quả:**
```
[14:30:15.123] HTTP POST /me/addresses started
               RequestId: 0HNGAS34COM1A:00000001
               
[14:30:15.234] Processing AddNewAddressCommand
               RequestId: 0HNGAS34COM1A:00000001
               
[14:30:15.345] Validation failed
               RequestId: 0HNGAS34COM1A:00000001
               ValidationErrors: ["Name.Invalid", "Phone.Invalid"]
               
[14:30:15.456] HTTP POST /me/addresses completed
               RequestId: 0HNGAS34COM1A:00000001
               StatusCode: 400
               Duration: 333ms
```

**Advanced search:**
```
// Tất cả requests từ cùng connection
RequestId like "0HNGAS34COM1A:%"

// Validation errors
RequestId = "0HNGAS34COM1A:00000001" and Level = "Error"

// Slow requests
RequestId = "0HNGAS34COM1A:00000001" and Duration > 1000
```

---

## ✅ Testing & Verification

### Test Case 1: Validation Error

**Request:**
```bash
curl -X POST https://localhost:5001/me/addresses \
  -H "Content-Type: application/json" \
  -d '{
    "name": "",
    "phone": "invalid",
    "province": ""
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
    }
  ],
  "traceId": "0HNGAS34COM1A:00000001"  ← Copy this
}
```

**Verify in Seq:**
```
1. Copy traceId: "0HNGAS34COM1A:00000001"
2. Go to Seq → Search: RequestId = "0HNGAS34COM1A:00000001"
3. Should see ALL logs for this request
```

---

### Test Case 2: Success Response

**Request:**
```bash
curl -X GET https://localhost:5001/me/addresses
```

**Response:**
```json
{
  "success": true,
  "message": "Lấy danh sách địa chỉ thành công",
  "data": [...],
  "metadata": {
    "pagination": {
      "currentPage": 1,
      "pageSize": 10,
      "totalCount": 25,
      "totalPages": 3
    }
  },
  "traceId": "0HNGAS34COM1A:00000002"  ← Copy this
}
```

**Verify in Seq:**
```
RequestId = "0HNGAS34COM1A:00000002"
```

---

### Test Case 3: Unhandled Exception

**Scenario:** Database connection fails

**Response (từ GlobalExceptionHandler):**
```json
{
  "success": false,
  "message": "Đã xảy ra lỗi hệ thống",
  "errorCode": "Server.InternalError",
  "traceId": "0HNGAS34COM1A:00000003"
}
```

**Seq logs:**
```
[14:30:15] Request started
           RequestId: 0HNGAS34COM1A:00000003
           
[14:30:16] Database connection failed
           RequestId: 0HNGAS34COM1A:00000003
           Exception: Npgsql.NpgsqlException
           StackTrace: ...
           
[14:30:16] Unhandled exception occurred
           RequestId: 0HNGAS34COM1A:00000003
           Level: Error
```

---

## 📊 Summary

### Components & Responsibilities

| Component | Responsibility | When |
|-----------|---------------|------|
| `TraceIdFilter.OnActionExecuting()` | Enrich logs với RequestId | Before action |
| `TraceIdFilter.OnActionExecuted()` | Inject TraceId vào response | After action (normal flow) |
| `GlobalExceptionHandler` | Set TraceId cho exceptions | Exception flow |
| `ResultExtensions` | Create response objects | In controller |
| `ValidationPipelineBehavior` | Generate error codes | Before handler |

### TraceId Flow

```
HttpContext.TraceIdentifier
        ↓
┌───────┴────────┐
│                │
↓                ↓
Logs         Response
(RequestId)  (traceId)
│                │
↓                ↓
Seq          Client
```

### Key Points

✅ **Single source of truth:** `HttpContext.TraceIdentifier`  
✅ **Format:** `{ConnectionId}:{RequestNumber}`  
✅ **Example:** `0HNGAS34COM1A:00000001`  
✅ **Seq field:** `RequestId`  
✅ **Response field:** `traceId`  
✅ **Search:** `RequestId = "..."`  

### Files Modified

1. ✅ `TraceIdFilter.cs` - Core logic (1 file)
2. ✅ `ApiResponse.cs` - Response models
3. ✅ `GlobalExceptionHandler.cs` - Exception handling
4. ✅ `ValidationPipelineBehavior.cs` - Error code generation
5. ✅ `ResultExtensions.cs` - Field name extraction
6. ✅ `Program.cs` - Filter registration

**Total: 6 files, ~100 lines of code for complete TraceId integration! 🎉**

