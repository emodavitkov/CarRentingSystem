using System.Security.Claims;

namespace CarRentingSystem.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}
