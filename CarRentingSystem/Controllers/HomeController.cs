using CarRentingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarRentingSystem.Data;
using CarRentingSystem.Models.Home;
using CarRentingSystem.Services.Statistics;

namespace CarRentingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly CarRentingDbContext data;
        private readonly IMapper mapper;
        private readonly IStatisticsService statistics;

        public HomeController(
            IStatisticsService statistics,
            CarRentingDbContext data, 
            IMapper mapper)
        {
            this.statistics = statistics;
            this.data = data;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            //var totalCars = this.data.Cars.Count();
            //var totalUsers = this.data.Users.Count();

            // with auto mapper
            var cars = this.data
                .Cars
                .OrderByDescending(c => c.Id)
                .ProjectTo<CarIndexViewModel>(this.mapper.ConfigurationProvider)
                .Take(3)
                .ToList();

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
                Cars = cars,
            });
        }
        


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}