# Kế hoạch Tính năng Product (Còn thiếu)

## Hiện trạng

Đã có:
- `POST /api/v1/products/create` - Tạo sản phẩm
- `GET /api/v1/products` - Lấy danh sách sản phẩm (public, có filter/sort/pagination)
- `GET /api/v1/products/{id}/public` - Lấy chi tiết sản phẩm (public)

## Cần làm

### 1. Buyer Endpoints (Public)

#### 1.1 Search & Suggestions

- `GET /api/v1/products/search` - Full-text search với filters nâng cao
  - Query params: `q` (text search), `categoryId`, `shopId`, `minPrice`, `maxPrice`, `hasPromotion`, `pickupProvince`, `pickupDistrict`, `sortBy`, `page`, `pageSize`
  - Tạo `SearchProductsQuery` và handler với Dapper
  - Hỗ trợ PostgreSQL full-text search với GIN indexes

- `GET /api/v1/products/search/suggest` - Gợi ý từ khóa tìm kiếm
  - Tạo bảng `search_keywords` với fields: `id`, `keyword`, `search_count`, `product_count`, `is_active`, `created_at_utc`, `updated_at_utc`
  - Tạo `SearchKeyword` entity và repository
  - Tạo `GetSearchSuggestionsQuery` và handler
  - Background job để sync keywords từ product names (chạy mỗi 6h)

#### 1.2 Product Views

- `GET /api/v1/products/{slug}` - Lấy sản phẩm theo slug
  - Tạo `GetProductBySlugQuery` và handler

- `GET /api/v1/products/category/{categoryId}` - Lấy sản phẩm theo category
  - Reuse `GetProductsQuery` với categoryId filter

- `GET /api/v1/products/shop/{shopId}` - Lấy sản phẩm theo shop
  - Reuse `GetProductsQuery` với shopId filter

#### 1.3 Reviews & Ratings

- `GET /api/v1/products/{id}/reviews` - Lấy reviews của sản phẩm
  - Tạo `ProductReview` entity: `id`, `product_id`, `user_id`, `order_id`, `rating`, `comment`, `images`, `created_at_utc`, `updated_at_utc`
  - Tạo `GetProductReviewsQuery` với pagination

- `POST /api/v1/products/{id}/reviews` - Tạo review (yêu cầu đã mua hàng)
  - Tạo `CreateProductReviewCommand`
  - Validate: user đã mua sản phẩm, chưa review

- `PUT /api/v1/products/reviews/{reviewId}` - Cập nhật review
  - Tạo `UpdateProductReviewCommand`

- `DELETE /api/v1/products/reviews/{reviewId}` - Xóa review
  - Tạo `DeleteProductReviewCommand`

### 2. Seller Endpoints

#### 2.1 Product Management

- `GET /api/v1/seller/products` - Lấy danh sách sản phẩm của seller
  - Tạo `GetSellerProductsQuery` với shopId từ authenticated user
  - Hiển thị tất cả status (Active, Inactive, Draft, etc.)

- `GET /api/v1/seller/products/{id}` - Lấy chi tiết sản phẩm (seller view)
  - Tạo `GetSellerProductQuery` với full details

- `PUT /api/v1/seller/products/{id}` - Cập nhật sản phẩm
  - Tạo `UpdateProductCommand` với validation

- `DELETE /api/v1/seller/products/{id}` - Xóa sản phẩm (soft delete)
  - Tạo `DeleteProductCommand`

- `PATCH /api/v1/seller/products/{id}/status` - Thay đổi status
  - Tạo `ChangeProductStatusCommand`

#### 2.2 Variant Management

- `POST /api/v1/seller/products/{id}/variants` - Thêm variant mới
  - Tạo `AddProductVariantCommand`

- `PUT /api/v1/seller/products/{id}/variants/{variantId}` - Cập nhật variant
  - Tạo `UpdateProductVariantCommand`

- `DELETE /api/v1/seller/products/{id}/variants/{variantId}` - Xóa variant
  - Tạo `DeleteProductVariantCommand`

#### 2.3 Inventory Management

- `GET /api/v1/seller/products/{id}/inventory` - Lấy thông tin tồn kho
  - Tạo `GetProductInventoryQuery`

- `PUT /api/v1/seller/products/{id}/variants/{variantId}/stock` - Cập nhật stock
  - Tạo `UpdateVariantStockCommand`

- `GET /api/v1/seller/products/low-stock` - Lấy sản phẩm sắp hết hàng
  - Tạo `GetLowStockProductsQuery` với threshold config

#### 2.4 Media Management

- `POST /api/v1/seller/products/{id}/media` - Thêm ảnh/video
  - Tạo `AddProductMediaCommand`

- `PUT /api/v1/seller/products/{id}/media/{mediaId}` - Cập nhật media
  - Tạo `UpdateProductMediaCommand`

- `DELETE /api/v1/seller/products/{id}/media/{mediaId}` - Xóa media
  - Tạo `DeleteProductMediaCommand`

### 3. Admin Endpoints

#### 3.1 Product Management

- `GET /api/v1/admin/products` - Lấy tất cả sản phẩm (all shops, all statuses)
  - Tạo `GetAdminProductsQuery` với filters nâng cao

- `GET /api/v1/admin/products/{id}` - Lấy chi tiết sản phẩm (admin view)
  - Tạo `GetAdminProductQuery`

- `PUT /api/v1/admin/products/{id}` - Cập nhật sản phẩm (admin override)
  - Tạo `AdminUpdateProductCommand`

- `DELETE /api/v1/admin/products/{id}` - Xóa vĩnh viễn sản phẩm
  - Tạo `AdminDeleteProductCommand`

- `PATCH /api/v1/admin/products/{id}/lock` - Lock/Unlock sản phẩm
  - Tạo `LockProductCommand`

#### 3.2 Review Moderation

- `GET /api/v1/admin/reviews` - Lấy tất cả reviews
  - Tạo `GetAdminReviewsQuery` với filters

- `DELETE /api/v1/admin/reviews/{reviewId}` - Xóa review vi phạm
  - Tạo `AdminDeleteReviewCommand`

- `PATCH /api/v1/admin/reviews/{reviewId}/hide` - Ẩn/hiện review
  - Tạo `HideReviewCommand`

#### 3.3 Search Keywords Management

- `GET /api/v1/admin/search-keywords` - Lấy danh sách keywords
  - Tạo `GetSearchKeywordsQuery`

- `POST /api/v1/admin/search-keywords` - Thêm keyword thủ công
  - Tạo `CreateSearchKeywordCommand`

- `PUT /api/v1/admin/search-keywords/{id}` - Cập nhật keyword
  - Tạo `UpdateSearchKeywordCommand`

- `DELETE /api/v1/admin/search-keywords/{id}` - Xóa keyword
  - Tạo `DeleteSearchKeywordCommand`

- `POST /api/v1/admin/search-keywords/sync` - Trigger sync từ product names
  - Tạo `SyncSearchKeywordsCommand`

### 4. Database Changes

#### 4.1 Indexes cần thêm

```sql
-- Full-text search indexes
CREATE INDEX idx_products_name_gin ON products USING GIN (to_tsvector('english', name));
CREATE INDEX idx_products_description_gin ON products USING GIN (to_tsvector('english', description));

-- Search optimization
CREATE INDEX idx_products_status_category ON products (status, category_id);
CREATE INDEX idx_products_status_shop ON products (status, shop_id);
CREATE INDEX idx_products_slug ON products (slug);
```

#### 4.2 Bảng mới

- `search_keywords` - Lưu từ khóa tìm kiếm phổ biến
- `product_reviews` - Lưu đánh giá sản phẩm
- `product_review_images` - Lưu ảnh trong reviews

### 5. Background Jobs

- `SearchKeywordSyncJob` - Sync keywords từ product names (mỗi 6h)
- `ProductStockAlertJob` - Gửi thông báo sản phẩm sắp hết hàng (mỗi ngày)

### 6. Shared Components

- `ProductAccessLevel` enum - Public, Seller, Admin
- `ProductDetailAssembler` service - Build DTOs (đã có nhưng bị xóa, cần restore)
- `ProductFilterDto` - Shared filter DTO
- `ProductSortBy` enum - Các kiểu sort

## Thứ tự ưu tiên

1. **Phase 1 - Search & Suggestions** (Quan trọng nhất)
   - Search keywords table & sync
   - Search suggestions endpoint
   - Full-text search endpoint

2. **Phase 2 - Seller Management**
   - Seller CRUD endpoints
   - Variant management
   - Inventory management

3. **Phase 3 - Reviews & Ratings**
   - Review entity & repository
   - Buyer review endpoints
   - Admin moderation

4. **Phase 4 - Admin Management**
   - Admin product management
   - Search keywords management
   - Review moderation

## Files cần tạo/sửa

### Domain Layer

- `SearchKeyword.cs` - Entity
- `ProductReview.cs` - Entity
- `ISearchKeywordRepository.cs` - Repository interface
- `IProductReviewRepository.cs` - Repository interface

### Infrastructure Layer

- `SearchKeywordConfiguration.cs` - EF config
- `ProductReviewConfiguration.cs` - EF config
- `SearchKeywordRepository.cs` - Repository implementation
- `ProductReviewRepository.cs` - Repository implementation
- `SearchKeywordSyncJob.cs` - Background job
- Migration files cho các bảng mới

### Application Layer

- Queries và Commands như đã list ở trên
- DTOs tương ứng
- Validators cho các commands

### API Layer

- `BuyerProductController.cs` - Public endpoints
- `SellerProductController.cs` - Seller endpoints
- `AdminProductController.cs` - Admin endpoints
- Request/Response DTOs

## TODO List

- [ ] Tạo SearchKeyword entity, repository và EF configuration
- [ ] Implement search suggestions endpoint với query từ search_keywords table
- [ ] Implement full-text search endpoint với PostgreSQL GIN indexes
- [ ] Tạo background job để sync keywords từ product names
- [ ] Implement Seller CRUD endpoints (GET, PUT, DELETE, PATCH status)
- [ ] Implement variant management endpoints (Add, Update, Delete)
- [ ] Implement inventory management endpoints (Get inventory, Update stock, Low stock alert)
- [ ] Implement media management endpoints (Add, Update, Delete media)
- [ ] Tạo ProductReview entity, repository và EF configuration
- [ ] Implement buyer review endpoints (GET, POST, PUT, DELETE)
- [ ] Implement admin product management endpoints
- [ ] Implement admin review moderation endpoints
- [ ] Implement admin search keywords management endpoints


