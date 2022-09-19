using CarRentingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarRentingSystem.Data;
using CarRentingSystem.Services.Cars;
using CarRentingSystem.Services.Statistics;
using CarRentingSystem.Models.Home;
using System.Linq;
using CarRentingSystem.Services.Cars.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CarRentingSystem.Controllers
{
    public class HomeController : Controller
    {
       // private readonly CarRentingDbContext data;
       private readonly ICarService cars;
        //private readonly IMapper mapper;
        private readonly IStatisticsService statistics;
        private readonly IMemoryCache cache;

        //CarRentingDbContext data,
        // IMapper mapper,
        public HomeController(
            IStatisticsService statistics,
            ICarService cars,
            IMemoryCache cache)
        {
            this.statistics = statistics;
            //this.data = data;
            //this.mapper = mapper;
            this.cars = cars;
            this.cache = cache;
        }

        public IActionResult Index()
        {
            //var totalCars = this.data.Cars.Count();
            //var totalUsers = this.data.Users.Count();

            const string latestCarsCacheKey = "latestCarsCacheKey";
           
            var latestCars = this.cache.Get< List<LatestCarServiceModel>>(latestCarsCacheKey);

            if (latestCars == null)
            {
                latestCars = this.cars
                    .Latest()
                    .ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

                this.cache.Set(latestCarsCacheKey,latestCars, cacheOptions);
            }
            // with auto mapper
            //var latestCars = this.cars
            //    .Latest()
            //    .ToList();

            // shape before moving to service and no auto mapper is needed we can remove it from here also
            ////var cars = this.data
            ////    .Cars
            ////    .OrderByDescending(c => c.Id)
            ////    .ProjectTo<CarIndexViewModel>(this.mapper.ConfigurationProvider)
            ////    .Take(3)
            ////    .ToList();

            // without auto mapper

            //var cars = this.data
            //    .Cars
            //    .OrderByDescending(c => c.Id)
            //    //.Select(c => new CarListingViewModel
            //    .Select(c => new CarIndexViewModel()
            //        {
            //        Id = c.Id,
            //        Brand = c.Brand,
            //        Model = c.Model,
            //        Year = c.Year,
            //        ImageUrl = c.ImageUrl,
            //        //Category = c.Category.Name
            //    })
            //    .Take(3)
            //    .ToList();

            var totalStatistics = this.statistics.Total();

            //return View(cars);

            return View(new IndexViewModel
            {
                TotalCars = totalStatistics.TotalCars,
                TotalUsers = totalStatistics.TotalUsers,
                //Cars = cars,
                //Cars = latestCars.ToList()
                Cars = latestCars
            });
        }
        


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}