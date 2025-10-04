using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.RBAC;

namespace ECommerceBackend.Application.Roles.CreateRole;


public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, Guid>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        // Check if role code already exists
        Role? existingRole = await _roleRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existingRole != null)
        {
            return Result.Failure<Guid>(RoleErrors.CodeAlreadyExists);
        }

        // Validate permissions
        IEnumerable<Permission> validPermissions = await _permissionRepository.GetByCodesAsync(request.PermissionCodes, cancellationToken);
        var validPermissionCodes = validPermissions.Select(p => p.Code).ToHashSet();
        var invalidPermissions = request.PermissionCodes.Where(pc => !validPermissionCodes.Contains(pc)).ToList();

        if (invalidPermissions.Any())
        {
            return Result.Failure<Guid>(new Error("Role.InvalidPermissions",
                $"Invalid permissions: {string.Join(", ", invalidPermissions)}", ErrorType.Validation));
        }

        // Create role
        var role = Role.CreateCustomRole(request.Code, request.Name, request.Description);
        role.SetPermissions(request.PermissionCodes);

        _roleRepository.Add(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(role.Id);
    }
}
