using CarRentingSystem.Data;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using CarRentingSystem.Data.Models;
using CarRentingSystem.Models;
using CarRentingSystem.Models.Api.Cars;

namespace CarRentingSystem.Services.Cars
{
    public class CarService : ICarService
    {
        private readonly CarRentingDbContext data;

        public CarService(CarRentingDbContext data)
        {
            this.data = data;
        }

        public CarQueryServiceModel All(
            string brand,
            string searchTerm,
            CarSorting sorting,
            int currentPage,
            int carsPerPage)
        {

            var carsQuery = this.data.Cars.AsQueryable();

            if (!string.IsNullOrWhiteSpace(brand))
            {
                carsQuery = carsQuery.Where(c => c.Brand == brand);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                carsQuery = carsQuery.Where(c =>
                    (c.Brand + " " + c.Model).ToLower().Contains(searchTerm.ToLower()) ||
                    c.Description.ToLower().Contains(searchTerm.ToLower()));
            }

            carsQuery = sorting switch
            {
                //CarSorting.DateCreated => carsQuery.OrderByDescending(c => c.Id),
                CarSorting.Year => carsQuery.OrderByDescending(c => c.Year),
                CarSorting.BrandAndModel => carsQuery.OrderBy(c => c.Brand).ThenBy(c => c.Model),
                CarSorting.DateCreated or _ => carsQuery.OrderByDescending(c => c.Id)
                //_ => carsQuery.OrderByDescending(c => c.Id)
            };

            var totalCars = carsQuery.Count();

            var cars = GetCars((carsQuery
                .Skip((currentPage - 1) * carsPerPage)
                .Take(carsPerPage)));
                

            //var cars = carsQuery
            //    // .OrderByDescending(c => c.Id)
            //    .Skip((currentPage - 1) * carsPerPage)
            //    .Take(carsPerPage)
            //    .Select(c => new CarServiceModel
            //    {
            //        Id = c.Id,
            //        Brand = c.Brand,
            //        Model = c.Model,
            //        Year = c.Year,
            //        ImageUrl = c.ImageUrl,
            //        Category = c.Category.Name
            //    })
            //    .ToList();

            return new CarQueryServiceModel
            {
                TotalCars = totalCars,
                CurrentPage = currentPage,
                CarsPerPage = carsPerPage,
                Cars = cars
            };
        }

        public IEnumerable<CarServiceModel> ByUser(string userId)
            => this.GetCars(this.data.Cars
                .Where(c => c.Dealer.UserId == userId));
        
        //{
            //var carsByUser = this.data
            //    .Cars
            //    .Where(c => c.Dealer.UserId==userId)
            //    .Select(c => new CarServiceModel
            //    {
            //        Id = c.Id,
            //        Brand = c.Brand,
            //        Model = c.Model,
            //        Year = c.Year,
            //        ImageUrl = c.ImageUrl,
            //        Category = c.Category.Name
            //    })
            //    .ToList();
        //}

        public IEnumerable<string> AllCarBrands()
        => this.data
            .Cars
            .Select(c => c.Brand)
            .Distinct()
            .OrderBy(br => br)
            .ToList();

        
        private IEnumerable<CarServiceModel> GetCars(IQueryable<Car> carQuery)
           => carQuery
            .Select(c => new CarServiceModel
        {
            Id = c.Id,
            Brand = c.Brand,
            Model = c.Model,
            Year = c.Year,
            ImageUrl = c.ImageUrl,
            Category = c.Category.Name
        })
        .ToList();

}
}
