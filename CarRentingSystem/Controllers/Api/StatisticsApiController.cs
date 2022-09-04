namespace CarRentingSystem.Controllers.Api
{
    using CarRentingSystem.Data;
    using Microsoft.AspNetCore.Mvc;
    using CarRentingSystem.Models.Api.Statistics;

    [ApiController]
    [Route("api/statistics")]
    public class StatisticsApiController : ControllerBase
    {
        private readonly CarRentingDbContext data;

        public StatisticsApiController(CarRentingDbContext data)
            => this.data = data;

        [HttpGet]
        public StatisticsResponseModel GetStatistics()
        {
            var totalCars = this.data.Cars.Count();
            var totalUsers = this.data.Users.Count();

            //var statistics = new StatisticsResponseModel
            //{
            //    TotalCars = totalCars,
            //    TotalUsers = totalUsers,
            //    TotalRents = 0,
            //};
            // return statistics;

            return new StatisticsResponseModel
            {
                TotalCars = totalCars,
                TotalUsers = totalUsers,
                TotalRents = 0,
            };

        }
    }
}
