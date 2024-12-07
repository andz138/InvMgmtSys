using InventoryManagementSystem.Context;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ML;

namespace InventoryManagementSystem;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddPredictionEnginePool<ProductSalesData, ProductSalesPrediction>()
            .FromFile("InventoryForecastModel.zip");

        // Add services to the container
        builder.Services.AddControllersWithViews(options =>
        {
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

            // Global authorization filter to require login for all pages
            options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
        });
        
        // Allow anonymous access for specific Razor Pages
        builder.Services.AddRazorPages()
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
                options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Register");
                options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/AccessDenied");
            });

        builder.Services.AddDbContext<InventoryMgmtDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<InventoryMgmtDbContext>();
        
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login"; // Redirect unauthenticated users to login page
            options.AccessDeniedPath = "/Identity/Account/AccessDenied"; 
            options.Cookie.HttpOnly = true; // Explicitly allow anonymous access to login and register actions
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;
        });

        // Build the application
        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        
        // Enables identity system's default pages like login and register
        app.MapRazorPages(); 

        app.Run();
    }
}