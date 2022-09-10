namespace CarRentingSystem.Models.Api.Cars
{
    using System.Collections.Generic;
    public class Removed_AllCarsApiResponseModel
    {
        public int CurrentPage { get; init; }

        public int CarsPerPage { get; init; }

        public int TotalCars { get; set; }

        public IEnumerable<Removed_CarResponseModel> Cars { get; init; }
    }
}
