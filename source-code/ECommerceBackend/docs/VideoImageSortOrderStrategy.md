# Video & Image Sort Order Strategy

## 📋 Tổng quan

Sử dụng **SortOrder** để phân biệt Video và Images trong `ProductMedia` thay vì thêm trường `MediaType` trùng lặp với bảng `Media`.

---

## 🎯 Strategy

### **SortOrder Convention**

| **Media Type** | **SortOrder** | **Display Order** | **IsCover** |
|----------------|---------------|-------------------|-------------|
| **Video** | `-1` | Đầu tiên (first) | `false` |
| **Images** | `0, 1, 2, ...` | Sau video | 1 ảnh có `true` |

---

## ✅ Benefits

### **1. No Data Redundancy**
- ❌ Không cần thêm `MediaType` vào `ProductMedia`
- ✅ `MediaType` chỉ có trong bảng `Media` (single source of truth)
- ✅ Tránh data inconsistency

### **2. Simple Logic**
```csharp
public bool IsVideo() => SortOrder == -1;
public bool IsImage() => SortOrder >= 0;
```

### **3. Natural Ordering**
- SQL: `ORDER BY sort_order ASC` → Video (-1) luôn đầu tiên, sau đó đến ảnh (0, 1, 2, ...)
- Không cần thêm logic phức tạp

### **4. Performance**
- Index: `ix_product_medias_product_sort_order (product_id, sort_order)`
- Query nhanh: Video luôn có `sort_order = -1`

---

## 📝 Domain Model

### **ProductMedia.cs**

```csharp
public class ProductMedia : Entity
{
    public Guid ProductId { get; private set; }
    public Guid? ProductVariantId { get; private set; }
    public Guid MediaId { get; private set; }
    public bool IsCover { get; private set; }
    public int SortOrder { get; private set; } // Video: -1, Images: >= 0
    public bool IsDeleted { get; private set; }
    
    // Navigation properties
    public Product Product { get; private set; } = null!;
    public ProductVariant? ProductVariant { get; private set; }
    public Media Media { get; private set; } = null!;

    /// <summary>
    /// Helper: Check if this is a video (sortOrder == -1)
    /// </summary>
    public bool IsVideo() => SortOrder == -1;

    /// <summary>
    /// Helper: Check if this is an image (sortOrder >= 0)
    /// </summary>
    public bool IsImage() => SortOrder >= 0;
}
```

---

## 🔧 Implementation

### **Create Product Handler**

```csharp
// 6a. Add video FIRST (if exists)
if (videoMedia != null && request.Video != null)
{
    var productMedia = ProductMedia.CreateForProduct(
        productId: product.Id,
        mediaId: videoMedia.Id,
        isCover: false,
        sortOrder: -1 // Video luôn có sortOrder = -1
    );
    await _productMediaRepository.AddAsync(productMedia, cancellationToken);
}

// 6b. Add images AFTER video
foreach ((CreateProductImageDto imageDto, Media media) in request.Images.Zip(imageMediaList))
{
    var productMedia = ProductMedia.CreateForProduct(
        productId: product.Id,
        mediaId: media.Id,
        isCover: imageDto.IsCover,
        sortOrder: imageDto.SortOrder // Images: 0, 1, 2, ...
    );
    await _productMediaRepository.AddAsync(productMedia, cancellationToken);
}
```

---

## 📊 Database Schema

### **Table: product_medias**

| Column | Type | Comment |
|--------|------|---------|
| `id` | uuid | Primary key |
| `product_id` | uuid | Foreign key |
| `product_variant_id` | uuid (nullable) | Foreign key |
| `media_id` | uuid | Foreign key |
| `is_cover` | boolean | Cover image flag |
| `sort_order` | integer | **Video: -1, Images: >= 0** |
| `is_deleted` | boolean | Soft delete |
| `created_at_utc` | timestamptz | Created timestamp |

### **Indexes**

```sql
CREATE INDEX ix_product_medias_product_sort_order 
ON product_medias(product_id, sort_order);

CREATE INDEX ix_product_medias_product_cover 
ON product_medias(product_id, is_cover);
```

---

## 🔍 Query Examples

### **Get all media (video first, then images)**

```sql
SELECT pm.*, m.*
FROM product_medias pm
JOIN medias m ON pm.media_id = m.id
WHERE pm.product_id = @productId 
  AND pm.is_deleted = false
ORDER BY pm.sort_order ASC;  -- -1 (video) first, then 0, 1, 2, ... (images)
```

### **Get only video**

```sql
SELECT pm.*, m.*
FROM product_medias pm
JOIN medias m ON pm.media_id = m.id
WHERE pm.product_id = @productId 
  AND pm.sort_order = -1
  AND pm.is_deleted = false;
```

### **Get only images**

```sql
SELECT pm.*, m.*
FROM product_medias pm
JOIN medias m ON pm.media_id = m.id
WHERE pm.product_id = @productId 
  AND pm.sort_order >= 0
  AND pm.is_deleted = false
ORDER BY pm.sort_order ASC;
```

### **Get cover image**

```sql
SELECT pm.*, m.*
FROM product_medias pm
JOIN medias m ON pm.media_id = m.id
WHERE pm.product_id = @productId 
  AND pm.is_cover = true
  AND pm.sort_order >= 0  -- Cover phải là ảnh
  AND pm.is_deleted = false;
```

---

## 📐 Rules & Constraints

### **Product Level**

✅ **Video:**
- Tối đa **1 video** per product
- `sortOrder = -1` (fixed)
- `isCover = false` (always)

✅ **Images:**
- 1-10 ảnh per product
- `sortOrder = 0, 1, 2, ...` (user-defined)
- Đúng **1 ảnh** có `isCover = true` và `sortOrder = 0`

### **Variant Level**

✅ **Images only** (no video):
- 0-3 ảnh per variant
- `sortOrder = 0, 1, 2, ...`
- Tối đa **1 ảnh** có `isCover = true` và `sortOrder = 0`

---

## 🎨 Frontend Display Logic

### **Example: Product Gallery**

```typescript
interface ProductMedia {
  id: string;
  mediaUrl: string;
  mediaType: 'Image' | 'Video';
  isCover: boolean;
  sortOrder: number;
}

function sortMediaForDisplay(medias: ProductMedia[]): ProductMedia[] {
  return medias.sort((a, b) => a.sortOrder - b.sortOrder);
  // Result: Video (-1) first, then images (0, 1, 2, ...)
}

function getVideo(medias: ProductMedia[]): ProductMedia | null {
  return medias.find(m => m.sortOrder === -1) || null;
}

function getImages(medias: ProductMedia[]): ProductMedia[] {
  return medias.filter(m => m.sortOrder >= 0);
}

function getCoverImage(medias: ProductMedia[]): ProductMedia | null {
  return medias.find(m => m.isCover && m.sortOrder >= 0) || null;
}
```

---

## 🚀 Migration Notes

### **No Schema Change Needed**

✅ Hiện tại `sort_order` đã tồn tại trong bảng `product_medias`

✅ Chỉ cần update:
- Logic trong application code
- Index: thêm composite index `(product_id, sort_order)`

### **Data Migration (if needed)**

```sql
-- Nếu có data cũ, update video sortOrder từ >= 0 sang -1
UPDATE product_medias pm
SET sort_order = -1
FROM medias m
WHERE pm.media_id = m.id
  AND m.media_type = 'Video'
  AND pm.sort_order >= 0;
```

---

## ✅ Summary

| **Aspect** | **Strategy** |
|------------|--------------|
| **Video SortOrder** | `-1` (fixed, always first) |
| **Image SortOrder** | `0, 1, 2, ...` (user-defined) |
| **MediaType Storage** | Only in `Media` table (no redundancy) |
| **Query Logic** | `ORDER BY sort_order ASC` |
| **Performance** | Index: `(product_id, sort_order)` |
| **Validation** | Video: sortOrder = -1, Image: sortOrder >= 0 |

---

**Date Created**: November 19, 2025  
**Author**: HDHiep  
**Status**: ✅ Implemented & Build Successful

