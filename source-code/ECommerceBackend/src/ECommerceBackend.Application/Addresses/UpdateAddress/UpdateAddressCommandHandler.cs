using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Addresses;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Addresses;

namespace ECommerceBackend.Application.Addresses.UpdateAddress;

internal sealed class UpdateAddressCommandHandler : ICommandHandler<UpdateAddressCommand, AddressDto>
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public UpdateAddressCommandHandler(
        IAddressRepository addressRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<AddressDto>> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        // Lấy thông tin user hiện tại
        if (!_userContext.IsAuthenticated)
        {
            return Result.Failure<AddressDto>(AddressErrors.Forbidden());
        }

        var currentUserId = Guid.Parse(_userContext.UserId!);

        // Tìm địa chỉ cần cập nhật
        Address? existingAddress = await _addressRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existingAddress is null)
        {
            return Result.Failure<AddressDto>(AddressErrors.NotFound());
        }

        // Kiểm tra quyền sở hữu
        if (existingAddress.UserId != currentUserId)
        {
            return Result.Failure<AddressDto>(AddressErrors.Forbidden());
        }

        // Cập nhật các trường
        existingAddress.Update(
            name: request.Name,
            phone: request.Phone,
            province: request.Province,
            district: request.District,
            ward: request.Ward,
            addressLine: request.AddressLine,
            isDefault: request.IsDefault,
            isPickUpAddress: request.IsPickUpAddress,
            isReturnAddress: request.IsReturnAddress
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(existingAddress.ToAddressDto());
    }
}
