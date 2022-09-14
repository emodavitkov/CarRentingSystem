﻿using CarRentingSystem.Models;
using CarRentingSystem.Services.Cars;

namespace CarRentingSystem.Controllers
{
    using CarRentingSystem.Data;
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Models.Cars;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using CarRentingSystem.Infrastructure;

    public class CarsController : Controller
    {
        private readonly ICarService cars;
        private readonly CarRentingDbContext data;

        public CarsController(ICarService cars, CarRentingDbContext data)
        {
            this.cars = cars;
            this.data = data;
        }


        //public IActionResult All(string brand, string searchTerm, CarSorting sorting)
        public IActionResult All([FromQuery]AllCarsQueryModel query)
          {
        //    var carsQuery = this.data.Cars.AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(query.Brand))
        //    {
        //        carsQuery = carsQuery.Where(c => c.Brand == query.Brand);
        //    }

        //    if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        //    {
        //        carsQuery = carsQuery.Where(c => 
        //            (c.Brand + " " + c.Model).ToLower().Contains(query.SearchTerm.ToLower()) ||
        //            c.Description.ToLower().Contains(query.SearchTerm.ToLower()));
        //    }

        //    carsQuery = query.Sorting switch
        //    {
        //        //CarSorting.DateCreated => carsQuery.OrderByDescending(c => c.Id),
        //        CarSorting.Year => carsQuery.OrderByDescending(c => c.Year),
        //        CarSorting.BrandAndModel => carsQuery.OrderBy(c => c.Brand).ThenBy(c => c.Model),
        //        CarSorting.DateCreated or _ => carsQuery.OrderByDescending(c => c.Id)
        //        //_ => carsQuery.OrderByDescending(c => c.Id)
        //    };

        //    var totalCars = carsQuery.Count();

        //    var cars = carsQuery
        //       // .OrderByDescending(c => c.Id)
        //       .Skip((query.CurrentPage -1)*AllCarsQueryModel.CarsPerPage)
        //       .Take(AllCarsQueryModel.CarsPerPage)
        //        .Select(c => new CarListingViewModel
        //        {
        //            Id = c.Id,
        //            Brand = c.Brand,
        //            Model = c.Model,
        //            Year = c.Year,
        //            ImageUrl = c.ImageUrl,
        //            Category = c.Category.Name
        //        })
        //        .ToList();

        //    //var carBrands = this.data
        //    //    .Cars
        //    //    .Select(c => c.Brand)
        //    //    .Distinct()
        //    //    .OrderBy(br => br)
        //    //    .ToList();

        var queryResult = this.cars.All(
               query.Brand,
               query.SearchTerm,
               query.Sorting,
               query.CurrentPage,
               AllCarsQueryModel.CarsPerPage
            );

        var carBrands = this.cars.AllCarBrands();

            query.TotalCars = queryResult.TotalCars;
            query.Brands=carBrands;
            query.Cars=queryResult.Cars;

            return View(query);


            //return View(new AllCarsQueryModel
            //{
            //    Brand = brand,
            //    Brands = carBrands,
            //    SearchTerm = searchTerm,
            //    Sorting = sorting,
            //    Cars = cars,
            //});
        }


        [Authorize]
        public IActionResult Mine()
        {
            var myCars = this.cars.ByUser(this.User.GetId());

            return View(myCars);
        }


        [Authorize]
        public IActionResult Add()
        {
            // var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // var userIsDealer = this.data.Dealers.Any(d => d.UserId == userId);
            
            // var userIsDealer = this.data
            //    .Dealers
            //    .Any(d => d.UserId == this.User.GetId());

            if (!this.UserIdDealer())
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }


            return View(new AddCarFormModel
            {
                Categories = this.GetCarCategories()
            });
        }

        [HttpPost]
        [Authorize]
        [AutoValidateAntiforgeryToken]
        public IActionResult Add(AddCarFormModel car)
        {
            var dealerId = this.data
                .Dealers
                .Where(d => d.UserId == this.User.GetId())
                .Select(d => d.Id)
                .FirstOrDefault();

            if (dealerId == 0)
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }


            if (!this.data.Categories.Any(c => c.Id == car.CategoryId))
            {
                this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist.");
            }

            ModelState.Remove("Categories");
           
            if (!ModelState.IsValid)
            {
               car.Categories = this.GetCarCategories();

                return View(car);
                
            }

            var carData = new Car
            {
                Brand = car.Brand,
                Model = car.Model,
                Description = car.Description,
                ImageUrl = car.ImageUrl,
                Year = car.Year,
                CategoryId = car.CategoryId,
                DealerId = dealerId,
            };

            this.data.Cars.Add(carData);

            this.data.SaveChanges();

            return RedirectToAction(nameof(All));
            //return RedirectToAction("Index", "Home");
        }

        private bool UserIdDealer()
            => this.data
                .Dealers
                .Any(d => d.UserId == this.User.GetId());

        private IEnumerable<CarCategoryViewModel> GetCarCategories()
        => this.data
                .Categories
                .Select(c => new CarCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

    }
}
