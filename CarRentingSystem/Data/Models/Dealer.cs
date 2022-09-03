namespace CarRentingSystem.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.Dealer;
    public class Dealer
    {
        public int Id { get; init; }
        
        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; init; }

        [Required]
        [MaxLength(PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; }

        [Required]
        public string UserId { get; set; }

        // Can be added but not required
        // public IdentityUser User { get; set; }

        public IEnumerable<Car> Cars { get; init; } = new List<Car>();
    }
}
