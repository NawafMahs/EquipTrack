using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Asp.Versioning;

namespace EquipTrack.Dashboard.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public abstract class BaseApiController : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    protected string GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }

    protected string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    protected bool IsAdmin()
    {
        return GetCurrentUserRole().Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }

    protected bool IsManager()
    {
        var role = GetCurrentUserRole();
        return role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
               role.Equals("Manager", StringComparison.OrdinalIgnoreCase);
    }
}