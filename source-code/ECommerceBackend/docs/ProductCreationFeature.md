# Tài liệu tính năng Tạo Sản phẩm

## Tổng quan

Tính năng tạo sản phẩm đã được triển khai hoàn chỉnh với các chức năng:
- Upload media (9 ảnh + 1 video)
- Quản lý variants với options (tương tự Shopee)
- Thông tin cân nặng, kích thước
- Background job tự động dọn dẹp orphan files

## Các thành phần đã triển khai

### 1. File Storage Service

**Files:**
- `src/ECommerceBackend.Application/Abstracts/FileStorage/IFileStorageService.cs`
- `src/ECommerceBackend.Application/Abstracts/FileStorage/FileStorageError.cs`
- `src/ECommerceBackend.Infrastructure/FileStorage/LocalFileStorageService.cs`
- `src/ECommerceBackend.Infrastructure/FileStorage/FileStorageOptions.cs`

**Chức năng:**
- Lưu file vào `wwwroot/uploads/{folder}/{guid}_{filename}`
- Validate file type: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp` (ảnh), `.mp4`, `.webm` (video)
- Validate file size: 5MB (ảnh), 50MB (video)
- Trả về relative path để lưu vào database

**Configuration (appsettings.json):**
```json
"FileStorage": {
  "LocalStoragePath": "wwwroot/uploads",
  "BaseUrl": "http://localhost:5000",
  "MaxImageSizeMB": 5,
  "MaxVideoSizeMB": 50,
  "AllowedImageExtensions": [".jpg", ".jpeg", ".png", ".gif", ".webp"],
  "AllowedVideoExtensions": [".mp4", ".webm"]
}
```

### 2. Media Upload API

**Endpoints:**

#### Upload single file
```
POST /api/v1/media/upload
Content-Type: multipart/form-data

Parameters:
- file: IFormFile (required)
- folder: string (default: "products")
```

**Response:**
```json
{
  "success": true,
  "message": "Upload media thành công",
  "data": {
    "mediaUrl": "/uploads/products/abc123_image.jpg",
    "mediaType": "Image",
    "fileName": "image.jpg",
    "fileSize": 1024567
  }
}
```

#### Upload multiple files
```
POST /api/v1/media/upload-multiple
Content-Type: multipart/form-data

Parameters:
- files: List<IFormFile> (required, max 10)
- folder: string (default: "products")
```

**Response:**
```json
{
  "success": true,
  "message": "Upload thành công 3/3 files",
  "data": [
    {
      "mediaUrl": "/uploads/products/abc123_image1.jpg",
      "mediaType": "Image",
      "fileName": "image1.jpg",
      "fileSize": 1024567
    },
    {
      "mediaUrl": "/uploads/products/def456_image2.jpg",
      "mediaType": "Image",
      "fileName": "image2.jpg",
      "fileSize": 987654
    }
  ]
}
```

### 3. Create Product API

**Endpoint:**
```
POST /api/v1/products/create
Content-Type: application/json
```

**Request Body:**
```json
{
  "shopId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "name": "iPhone 15 Pro Max",
  "description": "Điện thoại cao cấp từ Apple",
  "media": [
    {
      "mediaUrl": "/uploads/products/abc_cover.jpg",
      "mediaType": "Image",
      "sortOrder": 0
    },
    {
      "mediaUrl": "/uploads/products/def_img2.jpg",
      "mediaType": "Image",
      "sortOrder": 1
    },
    {
      "mediaUrl": "/uploads/products/ghi_video.mp4",
      "mediaType": "Video",
      "sortOrder": 2
    }
  ],
  "options": [
    {
      "name": "Màu sắc",
      "values": ["Titan Tự Nhiên", "Titan Đen", "Titan Xanh"]
    },
    {
      "name": "Dung lượng",
      "values": ["256GB", "512GB", "1TB"]
    }
  ],
  "variants": [
    {
      "optionValues": ["Titan Tự Nhiên", "256GB"],
      "price": 29990000,
      "stock": 100,
      "sku": "IP15PM-TN-256",
      "weight": 0.221,
      "height": 16.0,
      "width": 7.7,
      "length": 15.9
    },
    {
      "optionValues": ["Titan Tự Nhiên", "512GB"],
      "price": 34990000,
      "stock": 50,
      "sku": "IP15PM-TN-512",
      "weight": 0.221,
      "height": 16.0,
      "width": 7.7,
      "length": 15.9
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "message": "Tạo sản phẩm thành công",
  "data": "3fa85f64-5717-4562-b3fc-2c963f66afa8"
}
```

### 4. Validation Rules

**Product:**
- Tên sản phẩm: 5-200 ký tự
- Mô tả: tối đa 1000 ký tự
- Media: ít nhất 1 ảnh, tối đa 10 files (9 ảnh + 1 video)
- ShopId và CategoryId phải tồn tại trong hệ thống

**Options:**
- Tối đa 2 option types
- Mỗi option phải có ít nhất 1 value
- Tên option không được trùng nhau

**Variants:**
- Phải có ít nhất 1 variant
- SKU: 3-100 ký tự, unique trong toàn hệ thống
- Price > 0
- Stock >= 0
- Weight, Height, Width, Length > 0
- Số lượng variants phải = tích của số values các options
- Option values trong variant phải tồn tại trong options đã định nghĩa

### 5. Background Job - Cleanup Orphan Files

**File:** `src/ECommerceBackend.Infrastructure/BackgroundJobs/CleanupOrphanFilesJob.cs`

**Chức năng:**
- Chạy tự động mỗi ngày lúc 2:00 AM
- Quét tất cả files trong `wwwroot/uploads/`
- So sánh với MediaUrl trong database
- Xóa files:
  - Không tồn tại trong database
  - Được tạo hơn 24 giờ trước
- Log số lượng files đã xóa
- Tự động xóa empty directories

**Hangfire Dashboard:**
- Truy cập: `http://localhost:5000/hangfire`
- Quản lý và monitor background jobs
- Xem lịch sử chạy job

### 6. Database Schema

**Tables liên quan:**
- `products`: Thông tin sản phẩm chính
- `product_media`: Ảnh/video của sản phẩm
- `product_option_types`: Loại option (Màu sắc, Dung lượng)
- `product_option_values`: Giá trị option (Đỏ, 256GB)
- `product_variants`: Biến thể sản phẩm với giá, SKU, số lượng
- `product_variant_option_values`: Liên kết variant với option values

**Relationships:**
- Product 1-N ProductMedia
- Product 1-N ProductOptionType
- ProductOptionType 1-N ProductOptionValue
- Product 1-N ProductVariant
- ProductVariant N-N ProductOptionValue (through ProductVariantOptionValue)

## Workflow

### 1. Upload Media Flow
```
Frontend -> POST /api/v1/media/upload
         -> LocalFileStorageService validates & saves file
         -> Returns relative path: /uploads/products/{guid}_{filename}
         -> Frontend stores these URLs
```

### 2. Create Product Flow
```
Frontend -> POST /api/v1/products/create (with media URLs)
         -> CreateProductCommandValidator validates input
         -> CreateProductCommandHandler:
            1. Validates Shop & Category exist
            2. Generates unique slug
            3. Validates media files exist
            4. Validates SKUs are unique
            5. Creates Product entity
            6. Creates ProductMedia entities
            7. Creates ProductOptionType entities
            8. Creates ProductOptionValue entities
            9. Creates ProductVariant entities
            10. Creates ProductVariantOptionValue entities
            11. Saves all in one transaction (UnitOfWork)
         -> Returns Product ID
```

### 3. Background Cleanup Flow
```
Every day at 2:00 AM:
  CleanupOrphanFilesJob runs
  -> Gets all files in wwwroot/uploads/
  -> Gets all MediaUrls from product_media table
  -> Identifies orphan files (not in DB, > 24h old)
  -> Deletes orphan files
  -> Removes empty directories
  -> Logs summary
```

## Packages đã cài đặt

```xml
<PackageReference Include="Slugify.Core" Version="5.1.1" /> <!-- Application layer -->
<PackageReference Include="Hangfire.AspNetCore" Version="1.8.21" /> <!-- Infrastructure layer -->
<PackageReference Include="Hangfire.PostgreSql" Version="1.20.12" /> <!-- Infrastructure layer -->
```

## Lưu ý kỹ thuật

1. **Transaction Safety**: Tất cả operations trong CreateProductCommandHandler được thực hiện trong một transaction duy nhất thông qua UnitOfWork.

2. **Slug Generation**: Slug tự động generate từ tên sản phẩm và đảm bảo unique bằng cách append GUID nếu trùng.

3. **Media SortOrder**: SortOrder = 0 là ảnh cover, các ảnh khác có SortOrder tăng dần, video có thể có SortOrder bất kỳ.

4. **Orphan Files**: Files chỉ được xóa sau 24h để tránh xóa nhầm files đang trong quá trình upload/sử dụng.

5. **Extensibility**: LocalFileStorageService implement IFileStorageService, dễ dàng thay thế bằng cloud storage (AWS S3, Azure Blob, etc.) mà không cần thay đổi business logic.

6. **Error Handling**: Sử dụng Result pattern và custom errors (ProductErrors) để xử lý lỗi một cách nhất quán.

## Kiểm thử

### Test Upload Media
```bash
curl -X POST "http://localhost:5000/api/v1/media/upload" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@path/to/image.jpg" \
  -F "folder=products"
```

### Test Create Product
```bash
curl -X POST "http://localhost:5000/api/v1/products/create" \
  -H "Content-Type: application/json" \
  -d '{
    "shopId": "your-shop-id",
    "categoryId": "your-category-id",
    "name": "Test Product",
    "description": "Test Description",
    "media": [...],
    "options": [...],
    "variants": [...]
  }'
```

### Monitor Background Job
Truy cập: `http://localhost:5000/hangfire`

## TODO - Tính năng mở rộng

- [ ] Implement Cloud Storage (AWS S3/Azure Blob)
- [ ] Thêm Product Specification field
- [ ] Thêm Shipping Services integration
- [ ] Implement Update Product endpoint
- [ ] Implement Delete Product endpoint
- [ ] Implement Get Product details endpoint
- [ ] Image optimization (resize, compress)
- [ ] Video transcoding
- [ ] Bulk product import
- [ ] Product duplication

