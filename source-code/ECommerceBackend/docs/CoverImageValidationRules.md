# Cover Image Validation Rules

## 📋 Tổng quan

Đảm bảo mỗi product và variant đều có **đúng 1 cover image** với `sortOrder = 0`.

---

## ✅ Validation Rules

### **1. Product Images**

#### **Rule 1.1: Phải có đúng 1 cover image**
```csharp
.Must(HaveExactlyOneCoverImage)
.WithMessage("Sản phẩm phải có đúng 1 ảnh cover")

private static bool HaveExactlyOneCoverImage(List<CreateProductImageDto> images)
{
    return images.Count(m => m.IsCover) == 1;
}
```

**❌ Fail Cases:**
- 0 ảnh có `IsCover = true`
- 2+ ảnh có `IsCover = true`

**✅ Pass Case:**
- Đúng 1 ảnh có `IsCover = true`

---

#### **Rule 1.2: Cover image phải có sortOrder = 0**
```csharp
.Must(CoverImageHasSortOrderZero)
.WithMessage("Ảnh cover phải có sortOrder = 0")

private static bool CoverImageHasSortOrderZero(List<CreateProductImageDto> images)
{
    CreateProductImageDto? coverImage = images.FirstOrDefault(m => m.IsCover);
    return coverImage == null || coverImage.SortOrder == 0;
}
```

**❌ Fail Case:**
```json
{
  "images": [
    { "imageUrl": "img1.jpg", "isCover": false, "sortOrder": 0 },
    { "imageUrl": "img2.jpg", "isCover": true, "sortOrder": 1 }  // ❌ Cover phải có sortOrder = 0
  ]
}
```

**✅ Pass Case:**
```json
{
  "images": [
    { "imageUrl": "img1.jpg", "isCover": true, "sortOrder": 0 },   // ✅ Cover có sortOrder = 0
    { "imageUrl": "img2.jpg", "isCover": false, "sortOrder": 1 }
  ]
}
```

---

#### **Rule 1.3: SortOrder không được trùng lặp**
```csharp
.Must(HaveUniqueImageSortOrders)
.WithMessage("Thứ tự sắp xếp ảnh không được trùng lặp")

private static bool HaveUniqueImageSortOrders(List<CreateProductImageDto> images)
{
    var sortOrders = images.Select(m => m.SortOrder).ToList();
    return sortOrders.Count == sortOrders.Distinct().Count();
}
```

**❌ Fail Case:**
```json
{
  "images": [
    { "imageUrl": "img1.jpg", "isCover": true, "sortOrder": 0 },
    { "imageUrl": "img2.jpg", "isCover": false, "sortOrder": 1 },
    { "imageUrl": "img3.jpg", "isCover": false, "sortOrder": 1 }  // ❌ Trùng sortOrder
  ]
}
```

---

### **2. Variant Images**

#### **Rule 2.1: Tối đa 1 cover image (optional)**
```csharp
.Must(x => x == null || x.Count(m => m.IsCover) <= 1)
.WithMessage("Variant chỉ được có tối đa 1 ảnh cover")
```

**✅ Pass Cases:**
- Không có ảnh nào
- Có ảnh nhưng không có cover (`IsCover = false` cho tất cả)
- Có đúng 1 ảnh cover

**❌ Fail Case:**
- 2+ ảnh có `IsCover = true`

---

#### **Rule 2.2: Nếu có cover image, phải có sortOrder = 0**
```csharp
.Must(x => x == null || !x.Any() || VariantCoverImageHasSortOrderZero(x))
.WithMessage("Ảnh cover của variant phải có sortOrder = 0")

private static bool VariantCoverImageHasSortOrderZero(List<CreateProductImageDto> images)
{
    CreateProductImageDto? coverImage = images.FirstOrDefault(m => m.IsCover);
    return coverImage == null || coverImage.SortOrder == 0;
}
```

**❌ Fail Case:**
```json
{
  "images": [
    { "imageUrl": "variant1.jpg", "isCover": false, "sortOrder": 0 },
    { "imageUrl": "variant2.jpg", "isCover": true, "sortOrder": 1 }  // ❌ Cover phải có sortOrder = 0
  ]
}
```

**✅ Pass Cases:**
```json
// Case 1: Có cover với sortOrder = 0
{
  "images": [
    { "imageUrl": "variant1.jpg", "isCover": true, "sortOrder": 0 },  // ✅
    { "imageUrl": "variant2.jpg", "isCover": false, "sortOrder": 1 }
  ]
}

// Case 2: Không có cover
{
  "images": [
    { "imageUrl": "variant1.jpg", "isCover": false, "sortOrder": 0 },
    { "imageUrl": "variant2.jpg", "isCover": false, "sortOrder": 1 }
  ]
}

// Case 3: Không có ảnh
{
  "images": null
}
```

---

#### **Rule 2.3: SortOrder không được trùng lặp**
```csharp
.Must(x => x == null || HaveUniqueVariantImageSortOrders(x))
.WithMessage("Thứ tự sắp xếp ảnh trong variant không được trùng lặp")
```

---

## 📊 Complete Examples

### **Example 1: Product with 3 images**

**✅ VALID:**
```json
{
  "images": [
    { "imageUrl": "iphone-front.jpg", "isCover": true, "sortOrder": 0 },   // Cover = 0
    { "imageUrl": "iphone-back.jpg", "isCover": false, "sortOrder": 1 },
    { "imageUrl": "iphone-side.jpg", "isCover": false, "sortOrder": 2 }
  ]
}
```

**❌ INVALID - No cover:**
```json
{
  "images": [
    { "imageUrl": "iphone-front.jpg", "isCover": false, "sortOrder": 0 },
    { "imageUrl": "iphone-back.jpg", "isCover": false, "sortOrder": 1 }
  ]
}
// Error: "Sản phẩm phải có đúng 1 ảnh cover"
```

**❌ INVALID - Multiple covers:**
```json
{
  "images": [
    { "imageUrl": "iphone-front.jpg", "isCover": true, "sortOrder": 0 },
    { "imageUrl": "iphone-back.jpg", "isCover": true, "sortOrder": 1 }
  ]
}
// Error: "Sản phẩm phải có đúng 1 ảnh cover"
```

**❌ INVALID - Cover not at sortOrder 0:**
```json
{
  "images": [
    { "imageUrl": "iphone-front.jpg", "isCover": false, "sortOrder": 0 },
    { "imageUrl": "iphone-back.jpg", "isCover": true, "sortOrder": 1 }
  ]
}
// Error: "Ảnh cover phải có sortOrder = 0"
```

**❌ INVALID - Duplicate sortOrder:**
```json
{
  "images": [
    { "imageUrl": "iphone-front.jpg", "isCover": true, "sortOrder": 0 },
    { "imageUrl": "iphone-back.jpg", "isCover": false, "sortOrder": 1 },
    { "imageUrl": "iphone-side.jpg", "isCover": false, "sortOrder": 1 }
  ]
}
// Error: "Thứ tự sắp xếp ảnh không được trùng lặp"
```

---

### **Example 2: Variant with images**

**✅ VALID - With cover:**
```json
{
  "variants": [
    {
      "optionValues": ["Titan", "256GB"],
      "price": 29990000,
      "stock": 50,
      "images": [
        { "imageUrl": "titan-front.jpg", "isCover": true, "sortOrder": 0 },
        { "imageUrl": "titan-back.jpg", "isCover": false, "sortOrder": 1 }
      ]
    }
  ]
}
```

**✅ VALID - Without cover:**
```json
{
  "variants": [
    {
      "optionValues": ["Titan", "256GB"],
      "price": 29990000,
      "stock": 50,
      "images": [
        { "imageUrl": "titan-front.jpg", "isCover": false, "sortOrder": 0 },
        { "imageUrl": "titan-back.jpg", "isCover": false, "sortOrder": 1 }
      ]
    }
  ]
}
```

**✅ VALID - No images:**
```json
{
  "variants": [
    {
      "optionValues": ["Titan", "256GB"],
      "price": 29990000,
      "stock": 50,
      "images": null
    }
  ]
}
```

**❌ INVALID - Cover not at sortOrder 0:**
```json
{
  "variants": [
    {
      "optionValues": ["Titan", "256GB"],
      "price": 29990000,
      "stock": 50,
      "images": [
        { "imageUrl": "titan-front.jpg", "isCover": false, "sortOrder": 0 },
        { "imageUrl": "titan-back.jpg", "isCover": true, "sortOrder": 1 }
      ]
    }
  ]
}
// Error: "Ảnh cover của variant phải có sortOrder = 0"
```

---

## 🎯 Summary

| **Validation** | **Product** | **Variant** |
|----------------|-------------|-------------|
| **Số lượng cover** | Đúng 1 (required) | Tối đa 1 (optional) |
| **Cover sortOrder** | Phải = 0 | Phải = 0 (nếu có) |
| **SortOrder unique** | ✅ Required | ✅ Required |

---

## 🔍 Why These Rules?

### **1. Consistency**
- Cover image luôn ở vị trí đầu tiên (`sortOrder = 0`)
- Dễ dàng query và display

### **2. UX**
- Frontend biết chính xác ảnh nào là cover
- Không cần logic phức tạp để tìm cover

### **3. Performance**
- Database có thể index theo `sortOrder`
- Query ảnh cover nhanh hơn: `WHERE isCover = true AND sortOrder = 0`

### **4. Data Integrity**
- Mỗi product/variant chỉ có 1 cover image
- Không bị trường hợp conflict

---

**Date Created**: November 19, 2025  
**Author**: HDHiep  
**Status**: ✅ Implemented & Validated


