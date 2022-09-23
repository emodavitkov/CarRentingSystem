using System.Security.Claims;

using static CarRentingSystem.Areas.Admin.AdminConstants;

namespace CarRentingSystem.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string Id(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.NameIdentifier).Value;

        //public static string GetId(this ClaimsPrincipal user)
        //    => user.FindFirst(ClaimTypes.NameIdentifier).Value;

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(AdministratorRoleName);
    }
}
