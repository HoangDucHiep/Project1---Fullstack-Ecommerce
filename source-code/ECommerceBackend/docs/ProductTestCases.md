# 🧪 PRODUCT API - TEST CASES

**Version:** 1.0  
**Last Updated:** 2024-11-23  
**Total Test Cases:** 38

---

## 📋 TABLE OF CONTENTS

1. [Setup - Upload Media](#part-1-setup---upload-media)
2. [Create Product](#part-2-create-product)
3. [Update Product](#part-3-update-product)
4. [Query Product](#part-4-query-product)
5. [Testing Order](#testing-order)

---

## 🎬 PART 1: SETUP - UPLOAD MEDIA

### Test 1.1: Upload Product Images
- **Endpoint:** `POST /api/v1/media/upload`
- **Action:** Upload 3-5 ảnh sản phẩm
- **Expected:** 
  - ✅ 200 OK
  - Return: `fileUrl`, `mediaId`, `isTemp: true`
- **Save:** Lưu lại các `fileUrl` để dùng cho Create/Update

### Test 1.2: Upload Product Video
- **Endpoint:** `POST /api/v1/media/upload`
- **Action:** Upload 1 video sản phẩm
- **Expected:** 
  - ✅ 200 OK
  - Return: `fileUrl`, `mediaId`, `isTemp: true`, `mediaType: Video`
- **Save:** Lưu lại `fileUrl`

### Test 1.3: Upload Variant Images
- **Endpoint:** `POST /api/v1/media/upload`
- **Action:** Upload 2-3 ảnh cho mỗi variant
- **Expected:** 
  - ✅ 200 OK
  - Return: `fileUrl` cho từng ảnh
- **Save:** Lưu lại các `fileUrl`

---

## 📦 PART 2: CREATE PRODUCT

### Test 2.1: Create Simple Product (Success)
- **Endpoint:** `POST /api/v1/seller/products`
- **Prerequisites:** 
  - Đã upload 2 images + 1 video
  - Có shopId, categoryId hợp lệ
- **Action:** Tạo sản phẩm đơn giản (no variants)
- **Expected:**
  - ✅ 201 Created
  - `hasVariants: false`
  - Ghost variant được tạo tự động
  - Media `isTemp: false` (confirmed)
- **Save:** `productId`

### Test 2.2: Create Complex Product (Success)
- **Endpoint:** `POST /api/v1/seller/products`
- **Prerequisites:**
  - Đã upload product images/video
  - Đã upload variant images
- **Action:** Tạo sản phẩm với 2 options, 4 variants
- **Expected:**
  - ✅ 201 Created
  - `hasVariants: true`
  - `variantCount: 4`
  - 2 options, 4 option values created
  - All media confirmed
- **Save:** `productId`, `optionIds`, `variantIds`

### Test 2.3: Validation - No Cover Image
- **Endpoint:** `POST /api/v1/seller/products`
- **Action:** Tạo product với images nhưng không có `isCover: true`
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "Sản phẩm phải có đúng 1 ảnh cover"

### Test 2.4: Validation - Cover Not SortOrder 0
- **Endpoint:** `POST /api/v1/seller/products`
- **Action:** Cover image có `sortOrder: 1`
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "Ảnh cover phải có sortOrder = 0"

### Test 2.5: Validation - Duplicate Product SKU
- **Endpoint:** `POST /api/v1/seller/products`
- **Action:** Tạo product với SKU đã tồn tại trong shop
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "SKU 'XXX' đã tồn tại trong shop"

### Test 2.6: Validation - Duplicate Variant SKU in Request
- **Endpoint:** `POST /api/v1/seller/products`
- **Action:** 2 variants có cùng SKU
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "SKU variant bị trùng lặp trong request: XXX"

### Test 2.7: Validation - Duplicate Variant SKU in DB
- **Endpoint:** `POST /api/v1/seller/products`
- **Action:** Variant SKU đã tồn tại trong shop (product khác)
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "SKU variant 'XXX' đã tồn tại trong shop"

### Test 2.8: Validation - Invalid Variant Combination
- **Endpoint:** `POST /api/v1/seller/products`
- **Action:** Variant có option value không tồn tại
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "Các variant có tổ hợp option values không hợp lệ"

### Test 2.9: Validation - Too Many Product Images
- **Endpoint:** `POST /api/v1/seller/products`
- **Action:** Upload 11 images
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "Sản phẩm không được có quá 10 hình ảnh"

### Test 2.10: Validation - Too Many Variant Images
- **Endpoint:** `POST /api/v1/seller/products`
- **Action:** Variant có 4 images
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "Variant không được có quá 3 hình ảnh"

### Test 2.11: Validation - Media Not Found
- **Endpoint:** `POST /api/v1/seller/products`
- **Action:** Dùng URL không tồn tại
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "Không tìm thấy ảnh: [URL]"

### Test 2.12: Validation - Wrong Media Type
- **Endpoint:** `POST /api/v1/seller/products`
- **Action:** Dùng video URL trong images field
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "Media [URL] không phải là ảnh"

---

## 🔄 PART 3: UPDATE PRODUCT

### Test 3.1: Update Basic Info (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Prerequisites:** Product đã tạo ở Test 2.1
- **Action:** Đổi name, description
- **Expected:**
  - ✅ 200 OK
  - Name, description updated
  - Slug regenerated
  - Media unchanged

### Test 3.2: Update Price & Stock (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Action:** Đổi defaultPrice, defaultStock
- **Expected:**
  - ✅ 200 OK
  - Ghost variant updated với price/stock mới

### Test 3.3: Add New Image (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Prerequisites:** Upload 1 ảnh mới
- **Action:** Thêm ảnh mới vào danh sách images
- **Expected:**
  - ✅ 200 OK
  - Existing images kept
  - New image created
  - New image confirmed (isTemp: false)

### Test 3.4: Remove Image (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Action:** Bỏ 1 ảnh khỏi danh sách
- **Expected:**
  - ✅ 200 OK
  - Removed image soft deleted
  - Other images kept

### Test 3.5: Reorder Images (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Action:** Đổi sortOrder, đổi cover image
- **Expected:**
  - ✅ 200 OK
  - isCover, sortOrder updated
  - No new images created

### Test 3.6: Replace Video (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Prerequisites:** Upload video mới
- **Action:** Đổi video URL
- **Expected:**
  - ✅ 200 OK
  - Old video soft deleted
  - New video created & confirmed

### Test 3.7: Remove Video (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Action:** Set `video: null`
- **Expected:**
  - ✅ 200 OK
  - Existing video soft deleted

### Test 3.8: Change Status to OutOfStock (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Action:** Set `status: "OutOfStock"`
- **Expected:**
  - ✅ 200 OK
  - Status updated

### Test 3.9: Add Variant (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Prerequisites:** Product có options
- **Action:** Thêm variant mới (không có Id)
- **Expected:**
  - ✅ 200 OK
  - New variant created
  - VariantOptionValues created

### Test 3.10: Update Variant Details (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Action:** Đổi price, stock của variant (có Id)
- **Expected:**
  - ✅ 200 OK
  - Variant details updated

### Test 3.11: Remove Variant (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Action:** Bỏ variant khỏi danh sách
- **Expected:**
  - ✅ 200 OK
  - Variant soft deleted
  - VariantOptionValues soft deleted (CASCADE)
  - Variant images soft deleted (CASCADE)

### Test 3.12: Add Variant Image (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Prerequisites:** Upload ảnh mới
- **Action:** Thêm ảnh vào variant
- **Expected:**
  - ✅ 200 OK
  - New variant image created

### Test 3.13: Remove Variant Image (Success)
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Action:** Bỏ ảnh khỏi variant
- **Expected:**
  - ✅ 200 OK
  - Variant image soft deleted

### Test 3.14: Validation - Product Not Found
- **Endpoint:** `PUT /api/v1/seller/products/{invalid-id}`
- **Expected:**
  - ❌ 404 Not Found
  - Error: "Không tìm thấy sản phẩm"

### Test 3.15: Validation - Product Locked
- **Endpoint:** `PUT /api/v1/seller/products/{locked-product-id}`
- **Action:** Update product có status: Locked
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "Không thể cập nhật sản phẩm đang bị khóa"

### Test 3.16: Validation - Invalid Status Transition
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Action:** Set `status: "Locked"` (seller không được set)
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "Trạng thái không hợp lệ"

### Test 3.17: Validation - SKU Changed to Duplicate
- **Endpoint:** `PUT /api/v1/seller/products/{id}`
- **Action:** Đổi SKU thành SKU đã tồn tại
- **Expected:**
  - ❌ 400 Bad Request
  - Error: "SKU 'XXX' đã tồn tại trong shop"

### Test 3.18: Validation - Same as Create
- **Note:** All validation tests từ Create (2.3 - 2.12) apply cho Update

---

## 🔍 PART 4: QUERY PRODUCT

### Test 4.1: Get Product by ID (Success)
- **Endpoint:** `GET /api/v1/public/products/{id}`
- **Expected:**
  - ✅ 200 OK
  - Full product details
  - Media with confirmed status
  - Variants (if any)

### Test 4.2: Get Product by Slug (Success)
- **Endpoint:** `GET /api/v1/public/products/slug/{slug}`
- **Expected:**
  - ✅ 200 OK
  - Same as Test 4.1

### Test 4.3: Get Product - Not Found
- **Endpoint:** `GET /api/v1/public/products/{invalid-id}`
- **Expected:**
  - ❌ 404 Not Found

### Test 4.4: Verify Media Confirmed
- **Endpoint:** `GET /api/v1/public/products/{id}`
- **Action:** Check media sau khi Create/Update
- **Expected:**
  - All media có `isTemp: false`
  - `confirmedAtUtc` not null

### Test 4.5: Verify Soft Delete
- **Endpoint:** `GET /api/v1/public/products/{id}`
- **Action:** Query sau khi xóa variant/image
- **Expected:**
  - Deleted entities không xuất hiện trong response
  - Only active entities returned

---

## 📊 SUMMARY

| **Category** | **Test Count** |
|--------------|----------------|
| Setup (Upload) | 3 |
| Create Product | 12 |
| Update Product | 18 |
| Query Product | 5 |
| **TOTAL** | **38 test cases** |

---

## 🎯 TESTING ORDER

### Phase 1: Setup
1. Test 1.1 - Upload Product Images
2. Test 1.2 - Upload Product Video
3. Test 1.3 - Upload Variant Images

### Phase 2: Create Product
4. Test 2.1 - Create Simple Product
5. Test 2.3 - 2.12 - Create Validations
6. Test 2.2 - Create Complex Product

### Phase 3: Query Product
7. Test 4.1 - Get Product by ID
8. Test 4.2 - Get Product by Slug
9. Test 4.4 - Verify Media Confirmed

### Phase 4: Update Product - Basic
10. Test 3.1 - Update Basic Info
11. Test 3.2 - Update Price & Stock
12. Test 3.3 - Add New Image
13. Test 3.4 - Remove Image
14. Test 3.5 - Reorder Images
15. Test 3.6 - Replace Video
16. Test 3.7 - Remove Video
17. Test 3.8 - Change Status

### Phase 5: Update Product - Variants
18. Test 3.9 - Add Variant
19. Test 3.10 - Update Variant Details
20. Test 3.11 - Remove Variant (with CASCADE)
21. Test 3.12 - Add Variant Image
22. Test 3.13 - Remove Variant Image

### Phase 6: Update Product - Validations
23. Test 3.14 - Product Not Found
24. Test 3.15 - Product Locked
25. Test 3.16 - Invalid Status Transition
26. Test 3.17 - SKU Changed to Duplicate
27. Test 3.18 - Other Validations

### Phase 7: Verification
28. Test 4.5 - Verify Soft Delete
29. Test 4.3 - Get Product Not Found

---

## 📝 NOTES

### Media URL Strategy
- **URL là identifier** cho media trong Update
- Giữ lại URL cũ → Update metadata only
- Thay đổi URL → Tạo mới
- Không có trong request → Soft delete

### Cascade Delete
- Xóa Variant → Xóa VariantOptionValues + VariantImages
- Xóa Option/Value → Frontend phải xử lý (Validator sẽ reject nếu inconsistent)

### SKU Validation
- Product SKU: unique per shop
- Variant SKU: unique per shop, no duplicates in request

### Status Transitions (Seller)
- ✅ Active ↔ Inactive ↔ OutOfStock
- ❌ Cannot set to Locked (Admin only)

### Media Limits
- Product: 1-10 images + 0-1 video
- Variant: 0-3 images (no video)
- Cover image: exactly 1, sortOrder = 0
- Video: sortOrder = -1 (always first)

---

**End of Test Cases Document**

