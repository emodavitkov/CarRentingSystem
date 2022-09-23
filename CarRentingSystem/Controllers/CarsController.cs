using AutoMapper;
using CarRentingSystem.Models;
using CarRentingSystem.Services.Cars;
using CarRentingSystem.Services.Dealers;

using static CarRentingSystem.WebConstants;

namespace CarRentingSystem.Controllers
{
    using CarRentingSystem.Data;
    using CarRentingSystem.Data.Models;
    using CarRentingSystem.Models.Cars;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using CarRentingSystem.Infrastructure.Extensions;

    public class CarsController : Controller
    {
        private readonly ICarService cars;
        private readonly IDealerService dealers;
        private readonly IMapper mapper;
       // private readonly CarRentingDbContext data;

        public CarsController(
            ICarService cars,
            IDealerService dealers,
            IMapper mapper)
            //CarRentingDbContext data 
        
        {
            this.cars = cars;
            //this.data = data;
            this.dealers = dealers;
            this.mapper = mapper;
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

        var carBrands = this.cars.AllBrands();

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
            var myCars = this.cars.ByUser(this.User.Id());

            return View(myCars);
        }

        public IActionResult Details(int id, string information)
        {
            var car = this.cars.Details(id);

            if (information != car.GetInformation())
            {
                return BadRequest();
            }

            return View(car);
        }

        [Authorize]
        public IActionResult Add()
        {
            // var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // var userIsDealer = this.data.Dealers.Any(d => d.UserId == userId);

            // var userIsDealer = this.data
            //    .Dealers
            //    .Any(d => d.UserId == this.User.GetId());

            //if (!this.UserIdDealer())
            //{
            //    return RedirectToAction(nameof(DealersController.Become), "Dealers");
            //}

            if (!this.dealers.IsDealer(this.User.Id()))
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            return View(new CarFormModel
            {
                //  Categories = this.GetCarCategories()
                Categories = this.cars.AllCategories()
            });
        }

        [HttpPost]
        [Authorize]
        [AutoValidateAntiforgeryToken]
        public IActionResult Add(CarFormModel car)
        {
            var dealerId = this.dealers.IdByUser(this.User.Id());

            //var dealerId = this.data
            //    .Dealers
            //    .Where(d => d.UserId == this.User.GetId())
            //    .Select(d => d.Id)
            //    .FirstOrDefault();


            if (dealerId == 0)
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }


            if (!this.cars.CategoryExists(car.CategoryId))
            {
                this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist.");
            }

            //if (!this.data.Categories.Any(c => c.Id == car.CategoryId))
            //{
            //    this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist.");
            //}

            ModelState.Remove("Categories");
           
            if (!ModelState.IsValid)
            {
                // car.Categories = this.GetCarCategories();

                car.Categories = this.cars.AllCategories();
                return View(car);
                
            }

            // moved to service Create
            //var carData = new Car
            //{
            //    Brand = car.Brand,
            //    Model = car.Model,
            //    Description = car.Description,
            //    ImageUrl = car.ImageUrl,
            //    Year = car.Year,
            //    CategoryId = car.CategoryId,
            //    DealerId = dealerId,
            //};


            //this.data.Cars.Add(carData);

            //this.data.SaveChanges();

           var carId =  this.cars.Create(
            car.Brand,
            car.Model,
            car.Description,
            car.ImageUrl,
            car.Year,
            car.CategoryId,
            dealerId);


            TempData[GlobalMessageKey] = "Your car was added successfully and it is awaiting for approval!";

            return RedirectToAction(nameof(Details), new { id = carId, information = car.GetInformation() });
            
            // return RedirectToAction(nameof(All));
            //return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var userId = this.User.Id();

            if (!this.dealers.IsDealer(userId) && !User.IsAdmin())
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            var car = this.cars.Details(id);

            if (car.UserId != userId && !User.IsAdmin())
            {
                return Unauthorized();
            }

            // use auto mapper
            var carForm = this.mapper.Map<CarFormModel>(car);
            // auto mapper cannot map this, so we are doing it manually:
            carForm.Categories = this.cars.AllCategories();

            return View(carForm);

            // auto mapper instead of this: 
            //return View(new CarFormModel
            //{
            //    Brand = car.Brand,
            //    Model = car.Model,
            //    Description = car.Description,
            //    ImageUrl = car.ImageUrl,
            //    Year = car.Year,
            //    CategoryId = car.CategoryId,
            //    Categories = this.cars.AllCategories(),
            //});
        }

        [HttpPost]
        [Authorize]

        public IActionResult Edit(int id, CarFormModel car)
        {
            var dealerId = this.dealers.IdByUser(this.User.Id());

            if (dealerId == 0 && !User.IsAdmin())
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }


            if (!this.cars.CategoryExists(car.CategoryId))
            {
                this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist.");
            }

            ModelState.Remove("Categories");

            if (!ModelState.IsValid)
            {
                car.Categories = this.cars.AllCategories();

                return View(car);
            }

            if (!this.cars.IsByDealer(id, dealerId) && !User.IsAdmin())
            {
                return BadRequest();
            }

            var edited = this.cars.Edit(
                id,
                car.Brand,
                car.Model,
                car.Description,
                car.ImageUrl,
                car.Year,
                car.CategoryId,
                this.User.IsAdmin());

            if (!edited)
            {
                return BadRequest();
            }

            TempData[GlobalMessageKey] = $"Your car was edited{(this.User.IsAdmin() ? string.Empty : " and is awaiting for approval!")}!";

            return RedirectToAction(nameof(Details), new { id, information = car.GetInformation() });

            // return RedirectToAction(nameof(All));
        }

        //private bool UserIdDealer()
        //    => this.data
        //        .Dealers
        //        .Any(d => d.UserId == this.User.GetId());

        //private IEnumerable<CarCategoryViewModel> GetCarCategories()
        //=> this.data
        //        .Categories
        //        .Select(c => new CarCategoryViewModel
        //        {
        //            Id = c.Id,
        //            Name = c.Name
        //        })
        //        .ToList();

    }
}
