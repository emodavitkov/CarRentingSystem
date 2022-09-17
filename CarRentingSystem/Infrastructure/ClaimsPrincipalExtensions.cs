using System.Security.Claims;

namespace CarRentingSystem.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Id(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.NameIdentifier).Value;

        //public static string GetId(this ClaimsPrincipal user)
        //    => user.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}
