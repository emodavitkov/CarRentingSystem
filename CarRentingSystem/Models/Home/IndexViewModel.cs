using CarRentingSystem.Services.Cars.Models;

namespace CarRentingSystem.Models.Home
{
    using System.Collections.Generic;
    public class IndexViewModel
    {
        public int TotalCars { get; init; }

        public int TotalUsers { get; init; }

        public int TotalRents { get; init; }

        public IList<LatestCarServiceModel> Cars { get; init; }

        //public IEnumerable<CarIndexViewModel> Cars { get; init; }
    }
}
