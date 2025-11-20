using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Shops.RegisterNewShop;

internal sealed class RegisterNewShopCommandHandler : ICommandHandler<RegisterNewShopCommand, Guid>
{
    private readonly IShopRepository _shopRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterNewShopCommandHandler(
        IShopRepository shopRepository,
        IUnitOfWork unitOfWork)
    {
        _shopRepository = shopRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RegisterNewShopCommand request, CancellationToken cancellationToken)
    {
        // Create shop entity
        var shop = Shop.Create(
            request.Name,
            request.Description,
            request.LogoUrl,
            request.BannerUrl,
            request.OwnerId
        );

        // Add to repository
        _shopRepository.Add(shop);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(shop.Id);
    }
}
