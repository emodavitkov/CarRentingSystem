using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CarRentingSystem.Areas.Admin.AdminConstants;

namespace CarRentingSystem.Areas.Admin.Controllers
{
    public class CarsController : AdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
