# Product Media Separation - Images & Videos

## 📋 Tổng quan

Đã tách riêng **Images** và **Videos** trong product creation để có thể validate số lượng chính xác hơn và đảm bảo tính nhất quán về kiểu media.

---

## 🎯 Quy tắc Media mới

### **1. Product Level**
- **Images**: Tối đa **10 ảnh**
- **Video**: Tối đa **1 video** (optional)
- Validation: Phải có ít nhất 1 ảnh, chỉ 1 ảnh được đánh dấu `IsCover`

### **2. Variant Level**
- **Images**: Tối đa **3 ảnh** (optional)
- **Video**: Không hỗ trợ video cho variant
- Validation: Nếu có ảnh, chỉ 1 ảnh được đánh dấu `IsCover`

---

## 📝 Cấu trúc Request mới

### **CreateNewProductRequest**

```csharp
public record CreateNewProductRequest(
    Guid ShopId,
    Guid CategoryId,
    string Name,
    string Description,
    string Sku,
    List<CreateProductImageRequest> Images,      // Tối đa 10 ảnh
    CreateProductVideoRequest? Video,            // Tối đa 1 video (optional)
    List<CreateProductOptionRequest> Options,
    List<CreateProductVariantRequest> Variants,
    // ... Simple product fields ...
);

public record CreateProductImageRequest(
    string ImageUrl,
    bool IsCover = false,
    int SortOrder = 0
);

public record CreateProductVideoRequest(
    string VideoUrl,
    int SortOrder = 0
);
```

### **CreateProductVariantRequest**

```csharp
public record CreateProductVariantRequest(
    List<string> OptionValues,
    decimal Price,
    int Stock,
    string? Sku = null,
    decimal? Weight = null,
    decimal? Height = null,
    decimal? Width = null,
    decimal? Length = null,
    List<CreateProductImageRequest>? Images = null  // Tối đa 3 ảnh
);
```

---

## 🔍 Validation Rules

### **Product Images**
```csharp
RuleFor(x => x.Images)
    .NotNull()
    .WithMessage("Danh sách ảnh không được null")
    .Must(x => x.Count > 0)
    .WithMessage("Sản phẩm phải có ít nhất 1 hình ảnh")
    .Must(x => x.Count <= 10)
    .WithMessage("Sản phẩm không được có quá 10 hình ảnh")
    .Must(HaveOnlyOneCoverImage)
    .WithMessage("Sản phẩm chỉ được có 1 ảnh cover")
    .Must(HaveUniqueImageSortOrders)
    .WithMessage("Thứ tự sắp xếp ảnh không được trùng lặp");
```

### **Product Video (Optional)**
```csharp
RuleFor(x => x.Video)
    .SetValidator(new CreateProductVideoDtoValidator()!)
    .When(x => x.Video != null);
```

### **Variant Images (Optional)**
```csharp
RuleFor(x => x.Images)
    .Must(x => x == null || x.Count <= 3)
    .WithMessage("Variant không được có quá 3 hình ảnh")
    .Must(x => x == null || x.Count(m => m.IsCover) <= 1)
    .WithMessage("Variant chỉ được có 1 ảnh cover")
    .Must(x => x == null || HaveUniqueVariantImageSortOrders(x))
    .WithMessage("Thứ tự sắp xếp ảnh trong variant không được trùng lặp");
```

---

## 🔧 Handler Changes

### **ResolveImagesAsync**
- Validate media type phải là `MediaType.Image`
- Throw error nếu không tìm thấy hoặc sai type

```csharp
private async Task<List<Media>> ResolveImagesAsync(
    List<CreateProductImageDto> imageDtos, 
    CancellationToken cancellationToken)
{
    // Validate each image exists and is of type Image
    foreach (CreateProductImageDto imageDto in imageDtos)
    {
        Media? media = await _mediaRepository.GetByFileUrlAsync(
            imageDto.ImageUrl, cancellationToken);
        
        if (media == null)
        {
            missingMediaInfo.Add(imageDto.ImageUrl);
        }
        else if (media.MediaType != MediaType.Image)
        {
            throw new ApplicationInvalidOperationException(
                Error.Validation("Media.InvalidType", 
                    $"Media {imageDto.ImageUrl} không phải là ảnh")
            );
        }
    }
}
```

### **ResolveVideoAsync**
- Validate media type phải là `MediaType.Video`
- Throw error nếu không tìm thấy hoặc sai type

```csharp
private async Task<Media> ResolveVideoAsync(
    CreateProductVideoDto videoDto, 
    CancellationToken cancellationToken)
{
    Media? media = await _mediaRepository.GetByFileUrlAsync(
        videoDto.VideoUrl, cancellationToken);
    
    if (media == null)
    {
        throw new ApplicationInvalidOperationException(
            Error.NotFound("Media.NotFound", 
                $"Không tìm thấy video: {videoDto.VideoUrl}")
        );
    }

    if (media.MediaType != MediaType.Video)
    {
        throw new ApplicationInvalidOperationException(
            Error.Validation("Media.InvalidType", 
                $"Media {videoDto.VideoUrl} không phải là video")
        );
    }

    return media;
}
```

---

## 📊 Example Request

### **Complex Product với Images + Video**

```json
{
  "shopId": "550e8400-e29b-41d4-a716-446655440000",
  "categoryId": "660e8400-e29b-41d4-a716-446655440001",
  "name": "iPhone 15 Pro Max",
  "description": "Flagship phone with amazing features",
  "sku": "IPHONE-15-PRO-MAX",
  "images": [
    {
      "imageUrl": "https://cdn.example.com/iphone-15-front.jpg",
      "isCover": true,
      "sortOrder": 0
    },
    {
      "imageUrl": "https://cdn.example.com/iphone-15-back.jpg",
      "isCover": false,
      "sortOrder": 1
    },
    {
      "imageUrl": "https://cdn.example.com/iphone-15-side.jpg",
      "isCover": false,
      "sortOrder": 2
    }
  ],
  "video": {
    "videoUrl": "https://cdn.example.com/iphone-15-demo.mp4",
    "sortOrder": 10
  },
  "options": [
    {
      "name": "Màu sắc",
      "values": ["Titan Tự Nhiên", "Titan Xanh", "Titan Trắng"]
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
      "stock": 50,
      "sku": "IPHONE-15-PRO-MAX-TITAN-256GB",
      "images": [
        {
          "imageUrl": "https://cdn.example.com/variant-titan-front.jpg",
          "isCover": true,
          "sortOrder": 0
        },
        {
          "imageUrl": "https://cdn.example.com/variant-titan-back.jpg",
          "isCover": false,
          "sortOrder": 1
        }
      ]
    }
  ]
}
```

### **Simple Product với chỉ Images (không có Video)**

```json
{
  "shopId": "550e8400-e29b-41d4-a716-446655440000",
  "categoryId": "660e8400-e29b-41d4-a716-446655440001",
  "name": "Cable USB-C",
  "description": "High quality USB-C cable",
  "sku": "CABLE-USBC-001",
  "images": [
    {
      "imageUrl": "https://cdn.example.com/cable-main.jpg",
      "isCover": true,
      "sortOrder": 0
    },
    {
      "imageUrl": "https://cdn.example.com/cable-detail.jpg",
      "isCover": false,
      "sortOrder": 1
    }
  ],
  "video": null,
  "options": [],
  "variants": [],
  "defaultPrice": 199000,
  "defaultStock": 1000
}
```

---

## ✅ Benefits

### **1. Type Safety**
- Đảm bảo images chỉ chứa ảnh, video chỉ chứa video
- Runtime validation với `MediaType` enum

### **2. Clear Validation**
- Dễ dàng validate số lượng: 10 ảnh + 1 video cho product, 3 ảnh cho variant
- Validation message rõ ràng hơn

### **3. Better UX**
- Frontend có thể tách riêng UI cho upload ảnh và upload video
- User experience tốt hơn khi biết chính xác giới hạn

### **4. Maintainable**
- Code dễ đọc, dễ maintain hơn
- Logic rõ ràng, không bị nhầm lẫn giữa ảnh và video

---

## 🚀 Migration Notes

### **Breaking Changes**
⚠️ **API Contract đã thay đổi hoàn toàn**

**Trước:**
```json
{
  "medias": [
    { "mediaUrl": "...", "isCover": true, "sortOrder": 0 }
  ]
}
```

**Bây giờ:**
```json
{
  "images": [
    { "imageUrl": "...", "isCover": true, "sortOrder": 0 }
  ],
  "video": {
    "videoUrl": "...", "sortOrder": 10
  }
}
```

### **Frontend Changes Required**
1. Update request DTOs
2. Separate upload UI for images vs video
3. Update validation logic
4. Show proper limits (10 images, 1 video)

---

## 📁 Files Changed

### **API Layer**
- `src/ECommerceBackend.Api/Controllers/Products/CreateNewProductRequest.cs`
- `src/ECommerceBackend.Api/Controllers/Products/SellerProductController.cs`

### **Application Layer**
- `src/ECommerceBackend.Application/Products/Commands/CreateNewProduct/CreateNewProductCommand.cs`
- `src/ECommerceBackend.Application/Products/Commands/CreateNewProduct/CreateNewProductCommandValidator.cs`
- `src/ECommerceBackend.Application/Products/Commands/CreateNewProduct/CreateNewProductCommandHandler.cs`

---

## 🧪 Testing Checklist

- [ ] Test tạo product với đúng 10 ảnh
- [ ] Test tạo product với 11 ảnh (phải fail)
- [ ] Test tạo product với 0 ảnh (phải fail)
- [ ] Test tạo product với 1 video
- [ ] Test tạo product không có video
- [ ] Test tạo product với video nhưng file là ảnh (phải fail)
- [ ] Test tạo product với ảnh nhưng file là video (phải fail)
- [ ] Test variant với 3 ảnh
- [ ] Test variant với 4 ảnh (phải fail)
- [ ] Test multiple cover images (phải fail)
- [ ] Test duplicate sort orders (phải fail)

---

**Date Created**: November 19, 2025  
**Author**: HDHiep  
**Status**: ✅ Completed & Tested

