using CarRentingSystem.Data;
using CarRentingSystem.Data.Models;
using CarRentingSystem.Infrastructure.Extensions;
using CarRentingSystem.Services.Cars;
using CarRentingSystem.Services.Dealers;
using CarRentingSystem.Services.Statistics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CarRentingDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options =>

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CarRentingDbContext>();
//Add identity role above

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMemoryCache();

builder.Services.AddControllersWithViews(options =>
    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>());

builder.Services.AddTransient<IStatisticsService, StatisticsService>();
builder.Services.AddTransient<ICarService, CarService>();
builder.Services.AddTransient<IDealerService, DealerService>();

var app = builder.Build();

app.PrepareDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Car Details",
    pattern: "/Cars/Details/{id}/{information}",
    defaults: new { controller = "Cars", action = "Details" });






app.MapRazorPages();

app.Run();


