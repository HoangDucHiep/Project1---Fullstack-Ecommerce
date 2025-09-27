using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Shops.ShopRegister;
internal sealed class ShopRegisterCommandHandler : ICommandHandler<ShopRegisterCommand, Guid>
{

    private readonly IShopRepository _shopRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShopRegisterCommandHandler(IShopRepository shopRepository, IUnitOfWork unitOfWork)
    {
        _shopRepository = shopRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ShopRegisterCommand request, CancellationToken cancellationToken)
    {
        var shop = Shop.Create(request.Name, request.Description, request.LogoUrl, request.BannerUrl, request.OwnerId);
        _shopRepository.Add(shop);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(shop.Id);
    }
}
