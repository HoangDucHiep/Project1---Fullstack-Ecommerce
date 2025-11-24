# Product Create & Update - Test Cases Documentation

## Table of Contents
1. [Create Product Feature](#create-product-feature)
2. [Update Product Feature](#update-product-feature)
3. [Common Validation Rules](#common-validation-rules)
4. [Test Scenarios](#test-scenarios)

---

## Create Product Feature

### Endpoint
```http
POST /api/v1/seller/products
Authorization: Bearer {seller_token}
Content-Type: application/json
```

### Request Structure

#### Simple Product (No Variants)
```json
{
  "shopId": "guid",
  "categoryId": "guid",
  "name": "string (required, max 200 chars)",
  "description": "string (required, max 5000 chars)",
  "sku": "string (required, max 100 chars, unique per shop)",
  "status": "Active|Inactive|Locked|OutOfStock",
  "images": [
    {
      "imageUrl": "string (valid S3 URL, confirmed media)",
      "isCover": "boolean (exactly 1 must be true)",
      "sortOrder": "integer (>= 0, unique, cover must be 0)"
    }
  ],
  "video": {
    "videoUrl": "string (valid S3 URL, confirmed media)"
  },
  "defaultPrice": "decimal (> 0, required for simple product)",
  "defaultStock": "integer (>= 0, required for simple product)",
  "defaultWeight": "decimal (>= 0, optional)",
  "defaultHeight": "decimal (>= 0, optional)",
  "defaultWidth": "decimal (>= 0, optional)",
  "defaultLength": "decimal (>= 0, optional)"
}
```

#### Complex Product (With Variants)
```json
{
  "shopId": "guid",
  "categoryId": "guid",
  "name": "string (required, max 200 chars)",
  "description": "string (required, max 5000 chars)",
  "sku": "string (required, max 100 chars, unique per shop)",
  "status": "Active|Inactive|Locked|OutOfStock",
  "images": [
    {
      "imageUrl": "string",
      "isCover": "boolean",
      "sortOrder": "integer"
    }
  ],
  "video": {
    "videoUrl": "string"
  },
  "options": [
    {
      "name": "string (e.g., 'Color')",
      "values": [
        { "value": "string (e.g., 'Red')" }
      ]
    }
  ],
  "variants": [
    {
      "optionValues": ["string (must match option values)"],
      "price": "decimal (> 0)",
      "stock": "integer (>= 0)",
      "sku": "string (unique per shop)",
      "status": "Active|Inactive|Locked|OutOfStock",
      "weight": "decimal (>= 0, optional)",
      "height": "decimal (>= 0, optional)",
      "width": "decimal (>= 0, optional)",
      "length": "decimal (>= 0, optional)",
      "images": [
        {
          "imageUrl": "string",
          "isCover": "boolean (max 1 per variant)",
          "sortOrder": "integer (>= 0, unique, cover must be 0 if exists)"
        }
      ]
    }
  ]
}
```

### Response Structure (201 Created)
```json
{
  "success": true,
  "message": "Tạo sản phẩm thành công",
  "data": {
    "id": "guid",
    "shopId": "guid",
    "categoryId": "guid",
    "name": "string",
    "description": "string",
    "slug": "string (auto-generated)",
    "sku": "string",
    "status": "Active|Inactive|Locked|OutOfStock",
    "createdAtUtc": "datetime",
    "updatedAtUtc": "datetime",
    "medias": [
      {
        "id": "guid",
        "productId": "guid",
        "productVariantId": "guid | null",
        "mediaId": "guid",
        "isCover": "boolean",
        "sortOrder": "integer (-1 for video, >= 0 for images)",
        "createdAtUtc": "datetime",
        "media": {
          "id": "guid",
          "fileName": "string",
          "originalFileName": "string",
          "fileUrl": "string",
          "fileSize": "integer",
          "mimeType": "string",
          "mediaType": "Image|Video",
          "width": "integer | null",
          "height": "integer | null",
          "duration": "integer | null",
          "isTemp": "boolean",
          "confirmedAtUtc": "datetime",
          "createdAtUtc": "datetime"
        }
      }
    ],
    "hasVariants": "boolean",
    "variantCount": "integer",
    
    // For Simple Product
    "price": "decimal",
    "stock": "integer",
    "weight": "decimal",
    "height": "decimal",
    "width": "decimal",
    "length": "decimal",
    
    // For Complex Product
    "options": [
      {
        "name": "string",
        "values": ["string"]
      }
    ],
    "variants": [
      {
        "id": "guid",
        "optionValues": ["string"],
        "price": "decimal",
        "stock": "integer",
        "sku": "string",
        "status": "Active|Inactive|Locked|OutOfStock",
        "weight": "decimal",
        "height": "decimal",
        "width": "decimal",
        "length": "decimal",
        "variantMedias": [
          {
            "id": "guid",
            "productId": "guid",
            "productVariantId": "guid",
            "mediaId": "guid",
            "isCover": "boolean",
            "sortOrder": "integer",
            "createdAtUtc": "datetime",
            "media": { /* same as above */ }
          }
        ],
        "createdAtUtc": "datetime"
      }
    ],
    "minPrice": "decimal",
    "maxPrice": "decimal",
    "totalStock": "integer"
  },
  "traceId": "string"
}
```

### Validation Rules (Create)

#### Product Level
1. **Name**: Required, 1-200 characters
2. **Description**: Required, 1-5000 characters
3. **SKU**: Required, max 100 characters, unique per shop
4. **Status**: Must be valid enum value
5. **Shop**: Must exist and be active
6. **Category**: Must exist and be active

#### Images (Product Level)
1. **Count**: 1-10 images required
2. **Cover Image**: Exactly 1 image must have `isCover = true`
3. **Cover Sort Order**: Cover image must have `sortOrder = 0`
4. **Sort Order**: All images must have unique sortOrder >= 0
5. **Media**: Must exist, be confirmed, and be of type `Image`

#### Video (Product Level)
1. **Optional**: Video is optional
2. **Media**: If provided, must exist, be confirmed, and be of type `Video`
3. **Sort Order**: Automatically set to `-1`

#### Options & Values
1. **Option Count**: 1-3 options for complex products
2. **Option Name**: Required, max 100 characters
3. **Value Count**: 2-10 values per option
4. **Value**: Required, max 100 characters

#### Variants
1. **Count**: At least 1 variant required if options provided
2. **Option Values**: Must match number of options
3. **Option Values Match**: Each value must belong to corresponding option
4. **SKU**: Required, max 100 characters, unique per shop
5. **SKU Uniqueness in Request**: No duplicate SKUs within the same request
6. **Price**: > 0
7. **Stock**: >= 0
8. **Status**: Must be valid enum value
9. **Images**: 0-3 images per variant
10. **Cover Image**: Max 1 cover image per variant
11. **Cover Sort Order**: If cover exists, must have `sortOrder = 0`

#### Simple Product
1. **Default Price**: Required, > 0
2. **Default Stock**: Required, >= 0
3. **No Options/Variants**: Must not provide options or variants

---

## Update Product Feature

### Endpoint
```http
PUT /api/v1/seller/products/{id}
Authorization: Bearer {seller_token}
Content-Type: application/json
```

### Request Structure

#### Simple Product Update
```json
{
  "categoryId": "guid",
  "name": "string",
  "description": "string",
  "sku": "string",
  "status": "Active|Inactive|Locked|OutOfStock",
  "images": [
    {
      "imageUrl": "string (URL used as identifier)",
      "isCover": "boolean",
      "sortOrder": "integer"
    }
  ],
  "video": {
    "videoUrl": "string"
  },
  "defaultPrice": "decimal",
  "defaultStock": "integer",
  "defaultWeight": "decimal (optional)",
  "defaultHeight": "decimal (optional)",
  "defaultWidth": "decimal (optional)",
  "defaultLength": "decimal (optional)"
}
```

#### Complex Product Update
```json
{
  "categoryId": "guid",
  "name": "string",
  "description": "string",
  "sku": "string",
  "status": "Active|Inactive|Locked|OutOfStock",
  "images": [
    {
      "imageUrl": "string",
      "isCover": "boolean",
      "sortOrder": "integer"
    }
  ],
  "video": {
    "videoUrl": "string"
  },
  "options": [
    {
      "id": "guid (optional, for update)",
      "name": "string (used as identifier if no id)",
      "values": [
        {
          "id": "guid (optional, for update)",
          "value": "string (used as identifier if no id)"
        }
      ]
    }
  ],
  "variants": [
    {
      "id": "guid (optional, for update)",
      "optionValues": ["string"],
      "price": "decimal",
      "stock": "integer",
      "sku": "string",
      "status": "Active|Inactive|Locked|OutOfStock",
      "weight": "decimal (optional)",
      "height": "decimal (optional)",
      "width": "decimal (optional)",
      "length": "decimal (optional)",
      "images": [
        {
          "imageUrl": "string",
          "isCover": "boolean",
          "sortOrder": "integer"
        }
      ]
    }
  ]
}
```

### Response Structure (200 OK)
Same as Create Product response structure.

### Update Logic

#### Incremental Updates
The update feature uses **incremental update strategy** instead of full replacement:

1. **Images/Video (Product Level)**
   - **Keep**: Same URL → Keep existing, update metadata (isCover, sortOrder)
   - **Add**: New URL → Create new ProductMedia
   - **Delete**: Missing URL → Soft delete existing ProductMedia

2. **Options**
   - **Match by**: `Id` (if provided) → `Name` (if no Id)
   - **Keep**: Matched → Update name
   - **Add**: Not matched → Create new
   - **Delete**: Not in request → Soft delete (cascade to values and variants)

3. **Option Values**
   - **Match by**: `Id` (if provided) → `Value` (if no Id)
   - **Keep**: Matched → Update value
   - **Add**: Not matched → Create new
   - **Delete**: Not in request → Soft delete

4. **Variants**
   - **Match by**: `Id` (if provided)
   - **Keep**: Matched → Update details and status
   - **Add**: No Id → Create new
   - **Delete**: Not in request → Soft delete (cascade to VariantOptionValues and VariantMedia)

5. **Variant Images**
   - **Keep**: Same URL → Keep existing, update metadata
   - **Add**: New URL → Create new
   - **Delete**: Missing URL → Soft delete

6. **Variant Option Values**
   - **Keep**: Same optionValueId → Keep existing
   - **Add**: New optionValueId → Create new
   - **Delete**: Missing optionValueId → Soft delete

#### Special Cases

1. **Simple → Complex**: Ghost variant is soft deleted
2. **Complex → Simple**: All variants, options, values are soft deleted
3. **Slug**: Auto-regenerated on every update
4. **SKU Change**: Validated for uniqueness per shop

### Validation Rules (Update)

Same as Create, plus:

1. **Product Existence**: Product must exist
2. **Ownership**: User must be the shop owner
3. **Shop Status**: Shop must be active
4. **Media URL Change**: When changing image/video URLs, new media must be confirmed

---

## Common Validation Rules

### Media Validation
1. Media must exist in database
2. Media must be confirmed (not temp)
3. Media type must match usage (Image for images, Video for video)
4. Media must belong to the same shop (if applicable)

### SKU Validation
1. Must be unique per shop
2. Must not duplicate within same request (for variants)
3. Max 100 characters
4. Required for products and variants

### Status Validation
- **Product Status**: `Active`, `Inactive`, `Locked`, `OutOfStock`
- **Variant Status**: `Active`, `Inactive`, `Locked`, `OutOfStock`

### Sort Order Strategy
- **Video**: Always `-1` (set automatically)
- **Images**: `>= 0`, unique within scope
- **Cover Image**: Must be `0`

---

## Test Scenarios

### Create Product Test Cases

#### TC-CR-01: Create Simple Product - Success
**Prerequisites:**
- Valid seller authentication
- Active shop
- Valid category
- 3 confirmed images uploaded
- 1 confirmed video uploaded

**Request:**
```json
{
  "shopId": "{shop_id}",
  "categoryId": "{category_id}",
  "name": "Apple iPhone 15 Pro Max 256GB",
  "description": "Latest iPhone with A17 Pro chip...",
  "sku": "IPHONE-15-PRO-MAX-256GB",
  "status": "Active",
  "images": [
    {
      "imageUrl": "https://.../image1.webp",
      "isCover": true,
      "sortOrder": 0
    },
    {
      "imageUrl": "https://.../image2.webp",
      "isCover": false,
      "sortOrder": 1
    },
    {
      "imageUrl": "https://.../image3.webp",
      "isCover": false,
      "sortOrder": 2
    }
  ],
  "video": {
    "videoUrl": "https://.../video.mp4"
  },
  "defaultPrice": 29990000,
  "defaultStock": 100,
  "defaultWeight": 221,
  "defaultHeight": 0.82,
  "defaultWidth": 7.67,
  "defaultLength": 15.96
}
```

**Expected Response:**
- Status: 201 Created
- `hasVariants: false`
- `variantCount: 0`
- Ghost variant created internally (not visible)
- Product media: 4 items (3 images + 1 video)
- All media confirmed
- Slug generated
- Price, stock, dimensions populated

---

#### TC-CR-02: Create Complex Product with 2 Options - Success
**Prerequisites:**
- Valid seller authentication
- Active shop
- Valid category
- Product images uploaded
- Variant images uploaded

**Request:**
```json
{
  "shopId": "{shop_id}",
  "categoryId": "{category_id}",
  "name": "Cáp Sạc USB-C Lightning Apple",
  "description": "Cáp sạc chính hãng Apple...",
  "sku": "APPLE-CABLE-USBC-LIGHTNING",
  "status": "Active",
  "images": [
    {
      "imageUrl": "https://.../cable-cover.webp",
      "isCover": true,
      "sortOrder": 0
    },
    {
      "imageUrl": "https://.../cable-2.webp",
      "isCover": false,
      "sortOrder": 1
    }
  ],
  "video": {
    "videoUrl": "https://.../cable-video.mp4"
  },
  "options": [
    {
      "name": "Màu dây",
      "values": [
        { "value": "Trắng" },
        { "value": "Đen" }
      ]
    },
    {
      "name": "Độ dài",
      "values": [
        { "value": "1m" },
        { "value": "2m" }
      ]
    }
  ],
  "variants": [
    {
      "optionValues": ["Trắng", "1m"],
      "price": 450000,
      "stock": 100,
      "sku": "APPLE-CABLE-WHITE-1M",
      "status": "Active",
      "weight": 25,
      "height": 1.5,
      "width": 200,
      "length": 10
    },
    {
      "optionValues": ["Trắng", "2m"],
      "price": 550000,
      "stock": 80,
      "sku": "APPLE-CABLE-WHITE-2M",
      "status": "Active",
      "weight": 35,
      "height": 1.5,
      "width": 200,
      "length": 15
    },
    {
      "optionValues": ["Đen", "1m"],
      "price": 450000,
      "stock": 120,
      "sku": "APPLE-CABLE-BLACK-1M",
      "status": "Active",
      "weight": 25,
      "height": 1.5,
      "width": 200,
      "length": 10
    },
    {
      "optionValues": ["Đen", "2m"],
      "price": 550000,
      "stock": 90,
      "sku": "APPLE-CABLE-BLACK-2M",
      "status": "Active",
      "weight": 35,
      "height": 1.5,
      "width": 200,
      "length": 15
    }
  ]
}
```

**Expected Response:**
- Status: 201 Created
- `hasVariants: true`
- `variantCount: 4`
- `options`: 2 options with correct values
- `variants`: 4 variants with correct combinations
- `minPrice: 450000`, `maxPrice: 550000`
- `totalStock: 390`
- Each variant has correct optionValues

---

#### TC-CR-03: Create Product with Variant Images - Success
**Prerequisites:**
- All media uploaded and confirmed

**Request:**
```json
{
  // ... basic product info
  "options": [
    {
      "name": "Color",
      "values": [
        { "value": "Red" },
        { "value": "Blue" }
      ]
    }
  ],
  "variants": [
    {
      "optionValues": ["Red"],
      "price": 100000,
      "stock": 50,
      "sku": "PROD-RED",
      "status": "Active",
      "images": [
        {
          "imageUrl": "https://.../red-1.webp",
          "isCover": true,
          "sortOrder": 0
        },
        {
          "imageUrl": "https://.../red-2.webp",
          "isCover": false,
          "sortOrder": 1
        }
      ]
    },
    {
      "optionValues": ["Blue"],
      "price": 100000,
      "stock": 60,
      "sku": "PROD-BLUE",
      "status": "Active",
      "images": [
        {
          "imageUrl": "https://.../blue-1.webp",
          "isCover": true,
          "sortOrder": 0
        }
      ]
    }
  ]
}
```

**Expected Response:**
- Status: 201 Created
- Variant "Red" has 2 images in `variantMedias`
- Variant "Blue" has 1 image in `variantMedias`
- Each variant's cover image has `sortOrder: 0`

---

#### TC-CR-04: Create Product - Missing Cover Image (Error)
**Request:**
```json
{
  // ... basic info
  "images": [
    {
      "imageUrl": "https://.../image1.webp",
      "isCover": false,  // ❌ No cover image!
      "sortOrder": 0
    }
  ],
  "defaultPrice": 100000,
  "defaultStock": 50
}
```

**Expected Response:**
- Status: 400 Bad Request
- Error: "Sản phẩm phải có đúng 1 ảnh cover"

---

#### TC-CR-05: Create Product - Cover Image Wrong Sort Order (Error)
**Request:**
```json
{
  // ... basic info
  "images": [
    {
      "imageUrl": "https://.../image1.webp",
      "isCover": true,
      "sortOrder": 1  // ❌ Cover must be 0!
    },
    {
      "imageUrl": "https://.../image2.webp",
      "isCover": false,
      "sortOrder": 0
    }
  ],
  "defaultPrice": 100000,
  "defaultStock": 50
}
```

**Expected Response:**
- Status: 400 Bad Request
- Error: "Ảnh cover phải có sortOrder = 0"

---

#### TC-CR-06: Create Product - Duplicate SKU (Error)
**Request:**
```json
{
  // ... basic info
  "sku": "EXISTING-SKU",  // ❌ Already exists in this shop!
  "defaultPrice": 100000,
  "defaultStock": 50
}
```

**Expected Response:**
- Status: 409 Conflict
- Error: "SKU 'EXISTING-SKU' đã tồn tại trong shop này"

---

#### TC-CR-07: Create Product - Duplicate Variant SKU in Request (Error)
**Request:**
```json
{
  // ... basic info
  "options": [
    {
      "name": "Size",
      "values": [
        { "value": "S" },
        { "value": "M" }
      ]
    }
  ],
  "variants": [
    {
      "optionValues": ["S"],
      "price": 100000,
      "stock": 50,
      "sku": "PROD-SIZE-S",
      "status": "Active"
    },
    {
      "optionValues": ["M"],
      "price": 100000,
      "stock": 60,
      "sku": "PROD-SIZE-S",  // ❌ Duplicate!
      "status": "Active"
    }
  ]
}
```

**Expected Response:**
- Status: 400 Bad Request
- Error: "Variant SKU trùng lặp trong request: PROD-SIZE-S"

---

#### TC-CR-08: Create Product - Unconfirmed Media (Error)
**Request:**
```json
{
  // ... basic info
  "images": [
    {
      "imageUrl": "https://.../temp-image.webp",  // ❌ isTemp = true!
      "isCover": true,
      "sortOrder": 0
    }
  ],
  "defaultPrice": 100000,
  "defaultStock": 50
}
```

**Expected Response:**
- Status: 400 Bad Request
- Error: "Media chưa được xác nhận"

---

#### TC-CR-09: Create Product - Wrong Media Type (Error)
**Request:**
```json
{
  // ... basic info
  "images": [
    {
      "imageUrl": "https://.../video.mp4",  // ❌ Video URL in images!
      "isCover": true,
      "sortOrder": 0
    }
  ],
  "defaultPrice": 100000,
  "defaultStock": 50
}
```

**Expected Response:**
- Status: 400 Bad Request
- Error: "Media không phải là hình ảnh"

---

#### TC-CR-10: Create Product - Variant Option Values Mismatch (Error)
**Request:**
```json
{
  // ... basic info
  "options": [
    {
      "name": "Color",
      "values": [{ "value": "Red" }]
    },
    {
      "name": "Size",
      "values": [{ "value": "S" }]
    }
  ],
  "variants": [
    {
      "optionValues": ["Red"],  // ❌ Missing Size!
      "price": 100000,
      "stock": 50,
      "sku": "PROD-RED",
      "status": "Active"
    }
  ]
}
```

**Expected Response:**
- Status: 400 Bad Request
- Error: "Số lượng giá trị option trong variant không khớp với số lượng option"

---

#### TC-CR-11: Create Product - Too Many Images (Error)
**Request:**
```json
{
  // ... basic info
  "images": [
    // 11 images (max is 10)
  ],
  "defaultPrice": 100000,
  "defaultStock": 50
}
```

**Expected Response:**
- Status: 400 Bad Request
- Error: "Sản phẩm không được có quá 10 hình ảnh"

---

#### TC-CR-12: Create Product - Too Many Variant Images (Error)
**Request:**
```json
{
  // ... basic info
  "variants": [
    {
      "optionValues": ["Red"],
      "price": 100000,
      "stock": 50,
      "sku": "PROD-RED",
      "status": "Active",
      "images": [
        // 4 images (max is 3 for variant)
      ]
    }
  ]
}
```

**Expected Response:**
- Status: 400 Bad Request
- Error: "Variant không được có quá 3 hình ảnh"

---

### Update Product Test Cases

#### TC-UP-01: Update Product Details - Success
**Prerequisites:**
- Product exists with id `{product_id}`
- User is shop owner

**Request:**
```json
PUT /api/v1/seller/products/{product_id}
{
  "categoryId": "{category_id}",
  "name": "Updated Product Name",
  "description": "Updated description",
  "sku": "UPDATED-SKU",
  "status": "Active",
  "images": [
    // Same URLs as before
  ],
  "video": {
    "videoUrl": "https://..."  // Same as before
  },
  "defaultPrice": 35000000,  // Changed
  "defaultStock": 150  // Changed
}
```

**Expected Response:**
- Status: 200 OK
- Name, description, SKU updated
- Price and stock updated
- Slug regenerated (new timestamp)
- `updatedAtUtc` changed
- Media unchanged (same URLs)

---

#### TC-UP-02: Update Variant Details (Price, Stock, Status) - Success
**Prerequisites:**
- Complex product with variants exists

**Request:**
```json
{
  // ... basic info
  "options": [
    {
      "name": "Màu dây",  // Match by name (no id needed)
      "values": [
        { "value": "Trắng" },
        { "value": "Đen" }
      ]
    },
    {
      "name": "Độ dài",
      "values": [
        { "value": "1m" },
        { "value": "2m" }
      ]
    }
  ],
  "variants": [
    {
      "id": "{variant_id}",  // Existing variant
      "optionValues": ["Trắng", "1m"],
      "price": 499000,  // Changed from 450000
      "stock": 150,  // Changed from 100
      "sku": "APPLE-CABLE-WHITE-1M",
      "status": "Inactive",  // Changed from Active
      "weight": 25,
      "height": 1.5,
      "width": 200,
      "length": 10
    },
    // ... other variants unchanged
  ]
}
```

**Expected Response:**
- Status: 200 OK
- Variant with id `{variant_id}` updated:
  - `price: 499000`
  - `stock: 150`
  - `status: "Inactive"`
- Other variants unchanged
- `minPrice`, `maxPrice`, `totalStock` recalculated

---

#### TC-UP-03: Add Variant Images - Success
**Prerequisites:**
- Variant exists without images
- Images uploaded and confirmed

**Request:**
```json
{
  // ... basic info
  "variants": [
    {
      "id": "{variant_id}",
      "optionValues": ["Đen", "2m"],
      "price": 550000,
      "stock": 90,
      "sku": "APPLE-CABLE-BLACK-2M",
      "status": "Active",
      "images": [  // ✅ Adding images
        {
          "imageUrl": "https://.../black-2m-1.webp",
          "isCover": true,
          "sortOrder": 0
        },
        {
          "imageUrl": "https://.../black-2m-2.webp",
          "isCover": false,
          "sortOrder": 1
        }
      ]
    }
  ]
}
```

**Expected Response:**
- Status: 200 OK
- Variant has 2 new images in `variantMedias`
- Cover image has `sortOrder: 0`, `isCover: true`

---

#### TC-UP-04: Update Product Images - Incremental
**Prerequisites:**
- Product has 3 images

**Request:**
```json
{
  // ... basic info
  "images": [
    {
      "imageUrl": "https://.../old-image1.webp",  // Keep (same URL)
      "isCover": true,
      "sortOrder": 0
    },
    {
      "imageUrl": "https://.../new-image.webp",  // Add (new URL)
      "isCover": false,
      "sortOrder": 1
    }
    // old-image2.webp and old-image3.webp not in list → will be deleted
  ]
}
```

**Expected Response:**
- Status: 200 OK
- Product has 2 images:
  - old-image1.webp (kept)
  - new-image.webp (added)
- old-image2.webp and old-image3.webp soft deleted

---

#### TC-UP-05: Remove Variant - Soft Delete with Cascade
**Prerequisites:**
- Product has 4 variants
- Variant to remove has images and option values

**Request:**
```json
{
  // ... basic info
  "variants": [
    {
      "id": "{variant_1_id}",
      // ... details
    },
    {
      "id": "{variant_2_id}",
      // ... details
    },
    {
      "id": "{variant_3_id}",
      // ... details
    }
    // variant_4 not in list → will be soft deleted
  ]
}
```

**Expected Response:**
- Status: 200 OK
- `variantCount: 3` (was 4)
- Variant 4 soft deleted
- Variant 4's VariantOptionValues soft deleted (cascade)
- Variant 4's ProductMedia soft deleted (cascade)
- `totalStock` recalculated (sum of remaining 3 variants)

---

#### TC-UP-06: Update Product Video - Replace
**Prerequisites:**
- Product has existing video
- New video uploaded

**Request:**
```json
{
  // ... basic info
  "video": {
    "videoUrl": "https://.../new-video.mp4"  // Different URL
  }
}
```

**Expected Response:**
- Status: 200 OK
- Old video soft deleted
- New video added with `sortOrder: -1`

---

#### TC-UP-07: Remove Product Video
**Prerequisites:**
- Product has existing video

**Request:**
```json
{
  // ... basic info
  "video": null  // Remove video
}
```

**Expected Response:**
- Status: 200 OK
- Video soft deleted
- Product has no video

---

#### TC-UP-08: Simple Product → Complex Product (Add Options/Variants)
**Prerequisites:**
- Simple product exists (has ghost variant internally)

**Request:**
```json
{
  // ... basic info
  "options": [
    {
      "name": "Color",
      "values": [
        { "value": "Red" },
        { "value": "Blue" }
      ]
    }
  ],
  "variants": [
    {
      "optionValues": ["Red"],
      "price": 100000,
      "stock": 50,
      "sku": "PROD-RED",
      "status": "Active"
    },
    {
      "optionValues": ["Blue"],
      "price": 100000,
      "stock": 60,
      "sku": "PROD-BLUE",
      "status": "Active"
    }
  ]
  // No defaultPrice, defaultStock (complex product doesn't need them)
}
```

**Expected Response:**
- Status: 200 OK
- `hasVariants: true` (was false)
- `variantCount: 2` (was 0)
- Ghost variant soft deleted
- 2 new variants created
- Options and values created

---

#### TC-UP-09: Update Options - Match by Name (No ID)
**Prerequisites:**
- Complex product with options exists

**Request:**
```json
{
  // ... basic info
  "options": [
    {
      // No id, match by name
      "name": "Màu dây",  // Existing option
      "values": [
        { "value": "Trắng" },  // Existing
        { "value": "Đen" },  // Existing
        { "value": "Xanh" }  // New value
      ]
    },
    {
      "name": "Độ dài",
      "values": [
        { "value": "1m" },
        { "value": "2m" }
      ]
    }
  ],
  "variants": [
    // Must update variants to use new "Xanh" value
  ]
}
```

**Expected Response:**
- Status: 200 OK
- Option "Màu dây" updated (not recreated)
- New value "Xanh" added
- Other values kept

---

#### TC-UP-10: Update - Duplicate SKU Error
**Request:**
```json
{
  // ... basic info
  "sku": "ANOTHER-PRODUCT-SKU"  // ❌ Belongs to different product in same shop!
}
```

**Expected Response:**
- Status: 409 Conflict
- Error: "SKU 'ANOTHER-PRODUCT-SKU' đã tồn tại trong shop này"

---

#### TC-UP-11: Update - Unauthorized (Not Shop Owner)
**Prerequisites:**
- User is not the shop owner

**Request:**
```json
{
  // ... any update
}
```

**Expected Response:**
- Status: 403 Forbidden
- Error: "Bạn không có quyền cập nhật sản phẩm này"

---

#### TC-UP-12: Update - Product Not Found
**Request:**
```json
PUT /api/v1/seller/products/{non_existent_id}
{
  // ... any update
}
```

**Expected Response:**
- Status: 404 Not Found
- Error: "Không tìm thấy sản phẩm"

---

#### TC-UP-13: Update Variant Images - Incremental
**Prerequisites:**
- Variant has 2 existing images

**Request:**
```json
{
  // ... basic info
  "variants": [
    {
      "id": "{variant_id}",
      // ... other details
      "images": [
        {
          "imageUrl": "https://.../existing-1.webp",  // Keep
          "isCover": true,
          "sortOrder": 0
        },
        {
          "imageUrl": "https://.../new-image.webp",  // Add
          "isCover": false,
          "sortOrder": 1
        }
        // existing-2.webp not in list → will be deleted
      ]
    }
  ]
}
```

**Expected Response:**
- Status: 200 OK
- Variant has 2 images:
  - existing-1.webp (kept)
  - new-image.webp (added)
- existing-2.webp soft deleted

---

#### TC-UP-14: Update - Change Variant Option Values
**Prerequisites:**
- Variant exists with optionValues ["Red", "M"]

**Request:**
```json
{
  // ... basic info
  "variants": [
    {
      "id": "{variant_id}",
      "optionValues": ["Blue", "L"],  // Changed both values
      "price": 100000,
      "stock": 50,
      "sku": "PROD-BLUE-L",
      "status": "Active"
    }
  ]
}
```

**Expected Response:**
- Status: 200 OK
- Old VariantOptionValues soft deleted
- New VariantOptionValues created
- Variant updated

---

### Performance Test Cases

#### TC-PERF-01: Create Product with Maximum Complexity
**Request:**
```json
{
  // ... basic info
  "images": [
    // 10 images (max)
  ],
  "video": {
    // 1 video
  },
  "options": [
    {
      "name": "Option1",
      "values": [
        // 10 values (max)
      ]
    },
    {
      "name": "Option2",
      "values": [
        // 10 values
      ]
    },
    {
      "name": "Option3",
      "values": [
        // 10 values
      ]
    }
  ],
  "variants": [
    // 1000 variants (10 x 10 x 10)
    // Each with 3 images
  ]
}
```

**Expected:**
- Should complete within reasonable time (< 30s)
- All data created correctly
- No memory issues

---

#### TC-PERF-02: Update Product with Many Variants
**Request:**
Update a product with 1000 variants, changing prices for all.

**Expected:**
- Incremental update should be efficient
- Only changed fields updated
- Complete within reasonable time

---

### Edge Cases

#### TC-EDGE-01: Create Product with Empty Optional Fields
**Request:**
```json
{
  // ... required fields
  "video": null,
  "defaultWeight": null,
  "defaultHeight": null,
  "defaultWidth": null,
  "defaultLength": null
}
```

**Expected:**
- Status: 201 Created
- Optional fields set to null/default
- Product created successfully

---

#### TC-EDGE-02: Update Product - No Changes
**Request:**
Send exact same data as current product.

**Expected:**
- Status: 200 OK
- No entities recreated (incremental update detects no changes)
- `updatedAtUtc` changed
- Slug regenerated

---

#### TC-EDGE-03: Create Product with Unicode Characters
**Request:**
```json
{
  "name": "Sản phẩm Tiếng Việt có dấu ĂÂĐÊÔƠƯăâđêôơư",
  "description": "Mô tả có emoji 🎉🎊🎈",
  "sku": "UNICODE-SKU-✓"
}
```

**Expected:**
- Status: 201 Created
- Unicode handled correctly
- Slug generated without special characters

---

## Summary

### Key Features Tested
1. ✅ Simple Product Creation
2. ✅ Complex Product Creation (Options + Variants)
3. ✅ Product Update (Incremental)
4. ✅ Variant Update
5. ✅ Media Management (Images + Video)
6. ✅ Variant Images
7. ✅ Soft Delete with Cascade
8. ✅ SKU Uniqueness Validation
9. ✅ Cover Image Validation
10. ✅ Sort Order Strategy
11. ✅ Option/Value Matching (by name/value)
12. ✅ Simple ↔ Complex Product Conversion

### Total Test Cases: 42
- **Create**: 12 test cases
- **Update**: 14 test cases
- **Performance**: 2 test cases
- **Edge Cases**: 3 test cases
- **Additional Scenarios**: 11 test cases

### Automation Recommendations
1. Use integration tests for full flow
2. Use unit tests for validation logic
3. Mock media repository for faster tests
4. Use test data builders for complex requests
5. Test with real database for EF Core query filters
6. Load test with 1000+ variants
7. Test concurrent updates (optimistic concurrency)

