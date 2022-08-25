using System.ComponentModel.DataAnnotations;

namespace CarRentingSystem.Models.Cars
{
    using System.Collections.Generic;

    public class AllCarsQueryModel
    {
        public const int CarsPerPage = 3;


        public string Brand { get; init; }

        [Display(Name = "Search by text")]
        public string SearchTerm { get; init; }

        public CarSorting Sorting { get; init; }

        public int CurrentPage { get; init; } = 1;

        public int  TotalCars { get; set; }

        public IEnumerable<string> Brands { get; set; }

        public IEnumerable<CarListingViewModel> Cars { get; set; }
    }
}
