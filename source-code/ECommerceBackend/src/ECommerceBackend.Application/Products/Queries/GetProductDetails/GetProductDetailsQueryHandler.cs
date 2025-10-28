using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Products;
using ECommerceBackend.Application.Products.Services;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Products.Queries.GetProductDetails;

public sealed class GetProductDetailsQueryHandler : IQueryHandler<GetProductDetailsQuery, ProductDetailDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IProductOptionTypeRepository _productOptionTypeRepository;
    private readonly IProductMediaRepository _productMediaRepository;
    private readonly ProductDetailAssembler _assembler;

    public GetProductDetailsQueryHandler(
        IProductRepository productRepository,
        IProductVariantRepository productVariantRepository,
        IProductOptionTypeRepository productOptionTypeRepository,
        IProductMediaRepository productMediaRepository,
        ProductDetailAssembler assembler)
    {
        _productRepository = productRepository;
        _productVariantRepository = productVariantRepository;
        _productOptionTypeRepository = productOptionTypeRepository;
        _productMediaRepository = productMediaRepository;
        _assembler = assembler;
    }

    public async Task<Result<ProductDetailDto>> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            return Result.Failure<ProductDetailDto>(ProductErrors.NotFound(request.ProductId));
        }

        // Load associated data
        List<ProductMedia> productMedias = await _productMediaRepository.GetByProductIdAsync(product.Id, cancellationToken);
        List<ProductVariant> productVariants = await _productVariantRepository.GetByProductIdAsync(product.Id, cancellationToken);
        List<ProductOptionType> productOptions = await _productOptionTypeRepository.GetByProductIdAsync(product.Id, cancellationToken);

        // Media DTOs
        List<ProductMediaDto> mediaDtos = await _assembler.BuildMediaDtos(productMedias, cancellationToken);

        bool hasVariants = productOptions.Any();

        if (hasVariants)
        {
            // Options
            List<ProductOptionDto> optionDtos = await _assembler.BuildOptionDtos(productOptions, cancellationToken);

            // Variants
            List<ProductVariantDetailDto> variantDtos = await _assembler.BuildVariantDtos(productVariants, cancellationToken);

            return _assembler.BuildProductDetail(
                product,
                mediaDtos,
                productOptions,
                productVariants,
                optionDtos,
                variantDtos,
                simpleOverrides: null
            );
        }
        else
        {
            return _assembler.BuildProductDetail(
                product,
                mediaDtos,
                productOptions,
                productVariants,
                optionDtos: null,
                variantDtos: null,
                simpleOverrides: null // Query không có override -> lấy từ ghost variant
            );
        }
    }
}
