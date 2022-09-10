using CarRentingSystem.Services.Statistics;

namespace CarRentingSystem.Controllers.Api
{
    using CarRentingSystem.Data;
    using Microsoft.AspNetCore.Mvc;
    using CarRentingSystem.Models.Api.Statistics;

    [ApiController]
    [Route("api/statistics")]
    public class StatisticsApiController : ControllerBase
    {
        private readonly IStatisticsService statistics;

        public StatisticsApiController(IStatisticsService statistics)
            => this.statistics = statistics;

        [HttpGet]
        public StatisticsServiceModel GetStatistics()
        {
            //var totalCars = this.data.Cars.Count();
            //var totalUsers = this.data.Users.Count();

            //var totalStatistics = this.statistics.Total();

            //var statistics = new StatisticsResponseModel
            //{
            //    TotalCars = totalCars,
            //    TotalUsers = totalUsers,
            //    TotalRents = 0,
            //};
            // return statistics;

            //return new Removed_StatisticsResponseModel
            //{
            //    TotalCars = totalCars,
            //    TotalUsers = totalUsers,
            //    TotalRents = 0,
            //};

            return this.statistics.Total();

        }
    }
}
