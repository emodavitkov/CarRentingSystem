using CarRentingSystem.Data;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarRentingSystem.Data.Models;
using CarRentingSystem.Models;
using CarRentingSystem.Models.Api.Cars;
using CarRentingSystem.Services.Cars.Models;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace CarRentingSystem.Services.Cars
{
    public class CarService : ICarService
    {
        private readonly CarRentingDbContext data;
        private readonly IConfigurationProvider mapper;

        public CarService(CarRentingDbContext data, IMapper mapper)
        {
            this.data = data;
            this.mapper = mapper.ConfigurationProvider;
        }

        public CarQueryServiceModel All(
            string brand = null,
            string searchTerm = null,
            CarSorting sorting = CarSorting.DateCreated,
            int currentPage = 1,
            int carsPerPage = int.MaxValue,
            bool publicOnly = true)
        {

            // var carsQuery = this.data.Cars.AsQueryable();

            var carsQuery = this.data.Cars
                .Where(c => !publicOnly || c.IsPublic);

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

        // with auto mapper below
        public CarDetailsServiceModel Details(int id) 
            => this.data
                 .Cars
                 .Where(c => c.Id == id)
                 .ProjectTo<CarDetailsServiceModel>(this.mapper)
                 .FirstOrDefault();


        //public CarDetailsServiceModel Details(int id)
        //    => this.data
        //        .Cars
        //        .Where(c => c.Id == id)
        //        .Select(c => new CarDetailsServiceModel
        //        {
        //            Id = c.Id,
        //            Brand = c.Brand,
        //            Model = c.Model,
        //            Description = c.Description,
        //            Year = c.Year,
        //            ImageUrl = c.ImageUrl,
        //            CategoryId = c.CategoryId,
        //            CategoryName = c.Category.Name,
        //            DealerId = c.DealerId,
        //            DealerName = c.Dealer.Name,
        //            UserId = c.Dealer.UserId,
        //        })
        //        .FirstOrDefault();


        public int Create(string brand, 
            string model,
            string description,
            string imageUrl,
            int year,
            int categoryId,
            int dealerId)
        {
            var carData = new Car
            {
                Brand = brand,
                Model = model,
                Description = description,
                ImageUrl = imageUrl,
                Year = year,
                CategoryId = categoryId,
                DealerId = dealerId,
                IsPublic = false
            };

            this.data.Cars.Add(carData);
            this.data.SaveChanges();

            return carData.Id;
        }

        public bool Edit(int id,
            string brand, 
            string model, 
            string description, 
            string imageUrl, 
            int year, 
            int categoryId,
            bool isPublic)
        {
            var carData = this.data.Cars.Find(id);


            if (carData == null)
            {
                return false;
            }

            //if (carData.DealerId != dealerId)
            //{
            //    return false;
            //}

            carData.Brand = brand;
            carData.Model = model;
            carData.Description = description;
            carData.ImageUrl = imageUrl;
            carData.Year = year;
            carData.CategoryId = categoryId;
            carData.IsPublic = isPublic;


            this.data.SaveChanges();

            return true;
        }

        public IEnumerable<CarServiceModel> ByUser(string userId)
            => this.GetCars(this.data.Cars
                .Where(c => c.Dealer.UserId == userId));

        public bool IsByDealer(int carId, int dealerId)
            => this.data
                .Cars
                .Any(c => c.Id == carId && c.DealerId == dealerId);

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


        public void ChangeVisibility(int carId)
        {
            var car = this.data.Cars.Find(carId);

            car.IsPublic = !car.IsPublic;

            this.data.SaveChanges();
        }

        public IEnumerable<string> AllBrands()
            => this.data
                .Cars
                .Select(c => c.Brand)
                .Distinct()
                .OrderBy(br => br)
                .ToList();

        public IEnumerable<CarCategoryServiceModel> AllCategories()
            => this.data
                .Categories
                .ProjectTo<CarCategoryServiceModel>(this.mapper)
                .ToList();

        //public IEnumerable<CarCategoryServiceModel> AllCategories()
        //    => this.data
        //        .Categories
        //        .Select(c => new CarCategoryServiceModel
        //        {
        //            Id = c.Id,
        //            Name = c.Name
        //        })
        //        .ToList();

        public bool CategoryExists(int categoryId)
            => this.data
                .Categories
                .Any(c => c.Id == categoryId);

        private IEnumerable<CarServiceModel> GetCars(IQueryable<Car> carQuery)
            => carQuery
                .ProjectTo<CarServiceModel>(this.mapper)
                .ToList();

        //private IEnumerable<CarServiceModel> GetCars(IQueryable<Car> carQuery)
        //    => carQuery
        //        .Select(c => new CarServiceModel
        //        {
        //            Id = c.Id,
        //            Brand = c.Brand,
        //            Model = c.Model,
        //            Year = c.Year,
        //            ImageUrl = c.ImageUrl,
        //            CategoryName = c.Category.Name,
        //            IsPublic = c.IsPublic
        //        })
        //        .ToList();

        public IEnumerable<LatestCarServiceModel> Latest()
       => this.data
           .Cars
           .Where(c => c.IsPublic)
           .OrderByDescending(c => c.Id)
           .ProjectTo<LatestCarServiceModel>(this.mapper)
           .Take(3)
           .ToList();

    }

}
