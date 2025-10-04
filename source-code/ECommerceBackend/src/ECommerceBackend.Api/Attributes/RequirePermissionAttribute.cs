using ECommerceBackend.Application.Abstracts.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerceBackend.Api.Attributes;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _permissionCode;

    public RequirePermissionAttribute(string permissionCode)
    {
        _permissionCode = permissionCode;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        IUserContext userContext = context.HttpContext.RequestServices.GetRequiredService<IUserContext>();
        IPermissionService permissionService = context.HttpContext.RequestServices.GetRequiredService<IPermissionService>();

        if (!userContext.IsAuthenticated || string.IsNullOrEmpty(userContext.UserId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        bool hasPermission = await permissionService.HasPermissionAsync(userContext.UserId, _permissionCode);
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}
