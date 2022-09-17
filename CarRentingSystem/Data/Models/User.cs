using System.ComponentModel.DataAnnotations;

namespace CarRentingSystem.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using static DataConstants.User;

    public class User: IdentityUser

    {
        [MaxLength(FullNameMaxLength)]
        public string FullName { get; set; }
    }
}
