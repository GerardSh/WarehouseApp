using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using WarehouseApp.Services.Mapping;
using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Web.ViewModels;
using WarehouseApp.Web.Infrastructure.Extensions;
using WarehouseApp.Services.Data.Interfaces;
using WarehouseApp.Web.Infrastructure.Middlewares;

namespace WarehouseApp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string connectionString = builder.Configuration.GetConnectionString("WarehouseDbConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            string adminEmail = builder.Configuration.GetValue<string>("Administrator:Email")!;
            string adminUsername = builder.Configuration.GetValue<string>("Administrator:Username")!;
            string adminPassword = builder.Configuration.GetValue<string>("Administrator:Password")!;

            builder.Services.AddDbContext<WarehouseDbContext>(options =>
                options.UseSqlServer(connectionString));

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            }

            builder.Services.Configure<IdentityOptions>(options =>
            {
                ConfigureIdentity(options, builder.Configuration);
            });

            // Then configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<WarehouseDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(cfg =>
            {
                cfg.LoginPath = "/Identity/Account/Login";
                cfg.AccessDeniedPath = "/Home/Error/403";
            });

            builder.Services.AddControllersWithViews(cfg =>
            {
                cfg.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            builder.Services.RegisterRepositories(typeof(ApplicationUser).Assembly);
            builder.Services.RegisterUserDefinedServices(typeof(IBaseService).Assembly);

            builder.Services.AddRazorPages();

            WebApplication app = builder.Build();

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).Assembly);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseMiddleware<RedirectLoggedInMiddleware>();

            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.ApplyMigrations();

            if (app.Environment.IsDevelopment())
            {
                app.SeedDefaultRolesAndAdminUser(adminEmail, adminUsername, adminPassword);
                app.AssignWarehousesToAdminByEmail(adminEmail);
            }

            app.Run();
        }

        private static void ConfigureIdentity(IdentityOptions options, IConfiguration configuration)
        {
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
