using CarRentingSystem.Services.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CarRentingSystem.Areas.Admin.AdminConstants;

namespace CarRentingSystem.Areas.Admin.Controllers
{
    public class CarsController : AdminController
    {
        private readonly ICarService cars;

        public CarsController(ICarService cars)
        {
            this.cars = cars;
        }

        public IActionResult All()
        {
            var cars = this.cars
                .All(publicOnly: false)
                .Cars;

            return View(cars);
        }

        public IActionResult ChangeVisibility(int id)
        {
            this.cars.ChangeVisibility(id);

            return RedirectToAction(nameof(All));
        }
    }
}
