using EquipTrack.Domain.Entities;
using System.Security.Claims;

namespace EquipTrack.Infrastructure.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}