using CarRentingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CarRentingSystem.Data;
using CarRentingSystem.Models.Home;

namespace CarRentingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly CarRentingDbContext data;

        public HomeController(CarRentingDbContext data)
        {
            this.data = data;
        }

        public IActionResult Index()
        {
            var totalCars = this.data.Cars.Count();

            var cars = this.data
                .Cars
                .OrderByDescending(c => c.Id)
                //.Select(c => new CarListingViewModel
                .Select(c => new CarIndexViewModel()
                    {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    ImageUrl = c.ImageUrl,
                    //Category = c.Category.Name
                })
                .Take(3)
                .ToList();

            //return View(cars);

            return View(new IndexViewModel
            {
                TotalCars = totalCars,
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