using CinemaApp.Services.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Web.ViewModels;

namespace WarehouseApp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("WarehouseDbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<WarehouseDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                ConfigureIdentity(options, builder.Configuration);
            });

            // Then configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<WarehouseDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).Assembly);

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
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }

        private static void ConfigureIdentity(IdentityOptions options, IConfiguration configuration)
        {
            // Password settings
            //options.Password.RequireDigit = true;
            //options.Password.RequireLowercase = true;
            //options.Password.RequireNonAlphanumeric = true;
            //options.Password.RequireUppercase = true;
            //options.Password.RequiredLength = 8;
            //options.Password.RequiredUniqueChars = 4;

            // Lockout settings
            //options.Lockout.AllowedForNewUsers = true;
            //options.Lockout.MaxFailedAccessAttempts = 5;
            //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            // User settings
            //options.User.RequireUniqueEmail = true;

            // Sign-in settings
            //options.SignIn.RequireConfirmedAccount = true;

            var identitySection = configuration.GetSection("Identity");

            // Password settings loaded from configuration
            options.Password.RequireDigit = identitySection.GetValue<bool>("Password:RequireDigit");
            options.Password.RequireLowercase = identitySection.GetValue<bool>("Password:RequireLowercase");
            options.Password.RequireUppercase = identitySection.GetValue<bool>("Password:RequireUppercase");
            options.Password.RequireNonAlphanumeric = identitySection.GetValue<bool>("Password:RequireNonAlphanumeric");
            options.Password.RequiredLength = identitySection.GetValue<int>("Password:RequiredLength");
            options.Password.RequiredUniqueChars = identitySection.GetValue<int>("Password:RequiredUniqueChars");

            // Lockout settings
            options.Lockout.AllowedForNewUsers = identitySection.GetValue<bool>("Lockout:AllowedForNewUsers");
            options.Lockout.MaxFailedAccessAttempts = identitySection.GetValue<int>("Lockout:MaxFailedAccessAttempts");
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(
                identitySection.GetValue<int>("Lockout:DefaultLockoutTimeSpanMinutes"));

            // User settings
            options.User.RequireUniqueEmail = identitySection.GetValue<bool>("User:RequireUniqueEmail");

            // Sign-in settings
            options.SignIn.RequireConfirmedAccount = identitySection.GetValue<bool>("SignIn:RequireConfirmedAccount");
        }
    }
}
