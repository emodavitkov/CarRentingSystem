using Microsoft.AspNetCore.Mvc;
using CarRentingSystem.Models.Api.Cars;
using CarRentingSystem.Services.Cars;

namespace CarRentingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/cars")]
    public class CarsApiController : ControllerBase
    {
        // private readonly CarRentingDbContext data;

        private readonly ICarService cars;

        //public CarsApiController(CarRentingDbContext data)
        //    => this.data = data;

        public CarsApiController(ICarService cars)
           => this.cars = cars;

        //[HttpGet]
        //public IEnumerable GetCar()
        //{
        //    return this.data.Cars.ToList();
        //}

        //[HttpGet]
        //[Route("{id}")]
        //public IActionResult GetDetails(int id)
        //{
        //    var car = this.data.Cars.Find(id);

        //    if (car == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(car);
        //}

        //[HttpPost]
        //public IActionResult SaveCar(Car car)
        //{
        //    return Ok();
        //}

        //[HttpGet("{id}")]
        //public ActionResult<Car> GetCar()
        //{
        //    var car = this.data.Cars.FirstOrDefault();

        //    if (car == null)
        //    {
        //        return NotFound();
        //    }

        //    return car;
        //}

        // [HttpGet]
        //public ActionResult<AllCarsApiResponseModel> All([FromQuery] AllCarsApiRequestModel query)
        //{

        [HttpGet]
        public CarQueryServiceModel All([FromQuery] AllCarsApiRequestModel query)
        {
            //var carsQuery = this.data.Cars.AsQueryable();

            //if (!string.IsNullOrWhiteSpace(query.Brand))
            //{
            //    carsQuery = carsQuery.Where(c => c.Brand == query.Brand);
            //}

            //if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            //{
            //    carsQuery = carsQuery.Where(c =>
            //        (c.Brand + " " + c.Model).ToLower().Contains(query.SearchTerm.ToLower()) ||
            //        c.Description.ToLower().Contains(query.SearchTerm.ToLower()));
            //}

            //carsQuery = query.Sorting switch
            //{
            //    //CarSorting.DateCreated => carsQuery.OrderByDescending(c => c.Id),
            //    CarSorting.Year => carsQuery.OrderByDescending(c => c.Year),
            //    CarSorting.BrandAndModel => carsQuery.OrderBy(c => c.Brand).ThenBy(c => c.Model),
            //    CarSorting.DateCreated or _ => carsQuery.OrderByDescending(c => c.Id)
            //    //_ => carsQuery.OrderByDescending(c => c.Id)
            //};

            //var totalCars = carsQuery.Count();

            //var cars = carsQuery
            //   // .OrderByDescending(c => c.Id)
            //   .Skip((query.CurrentPage - 1) * query.CarsPerPage)
            //   .Take(query.CarsPerPage)
            //    .Select(c => new CarResponseModel
            //    {
            //        Id = c.Id,
            //        Brand = c.Brand,
            //        Model = c.Model,
            //        Year = c.Year,
            //        ImageUrl = c.ImageUrl,
            //        Category = c.Category.Name
            //    })
            //    .ToList();

            //return new Removed_AllCarsApiResponseModel
            //{
            //    CurrentPage = query.CurrentPage,
            //    CarsPerPage = query.CarsPerPage,
            //    TotalCars = totalCars,
            //    Cars = cars,
            //};

            return this.cars.All(
                query.Brand,
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                query.CarsPerPage);

        }
    }
}
