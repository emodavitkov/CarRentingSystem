using CarRentingSystem.Data;
using System.Linq;

namespace CarRentingSystem.Services.Dealers
{
    public class DealerService : IDealerService
    {
        private readonly CarRentingDbContext data;

        public DealerService(CarRentingDbContext data)
        {
            this.data = data;
        }

        public bool IsDealer(string userId)
        => this.data
            .Dealers
            .Any(d => d.UserId == userId);
    }
}
