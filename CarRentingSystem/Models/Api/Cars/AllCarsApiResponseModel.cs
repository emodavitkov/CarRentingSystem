namespace CarRentingSystem.Models.Api.Cars
{
    using System.Collections.Generic;
    public class AllCarsApiResponseModel
    {
        public int CurrentPage { get; init; }

        public int CarsPerPage { get; init; }

        public int TotalCars { get; set; }

        public IEnumerable<CarResponseModel> Cars { get; init; }
    }
}
