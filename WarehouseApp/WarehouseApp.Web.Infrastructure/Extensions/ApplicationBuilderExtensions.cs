using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WarehouseApp.Data;
using WarehouseApp.Data.Models;
using WarehouseApp.Web.Infrastructure.Logging;
using static WarehouseApp.Common.Constants.RolesConstants;
using static WarehouseApp.Common.OutputMessages.ErrorMessages.ApplicationBuilderExtensions;

namespace WarehouseApp.Web.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope serviceScope = app.ApplicationServices.CreateScope();

            WarehouseDbContext dbContext = serviceScope
                .ServiceProvider
                .GetRequiredService<WarehouseDbContext>()!;
            dbContext.Database.Migrate();

            return app;
        }

        public static IApplicationBuilder SeedDefaultRolesAndAdminUser(
            this IApplicationBuilder app, string email, string username, string password)
        {
            using IServiceScope serviceScope = app.ApplicationServices.CreateAsyncScope();
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userStore = serviceProvider.GetRequiredService<IUserStore<ApplicationUser>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = serviceProvider.GetRequiredService<ILogger<ApplicationBuilderExtensionsLoggerCategory>>();

            Task.Run(async () =>
            {
                string[] roles = AllRoles.ToArray();

                foreach (var roleName in roles)
                {
                    bool roleExists = await roleManager.RoleExistsAsync(roleName);

                    if (!roleExists)
                    {
                        IdentityResult result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                        if (!result.Succeeded)
                        {
                            throw new InvalidOperationException(string.Format(CreatingRole, roleName));
                        }

                        logger.LogInformation("Role {RoleName} was created.", roleName);
                    }
                }

                ApplicationUser? adminUser = await userManager.FindByEmailAsync(email);
                if (adminUser == null)
                {
                    adminUser = await CreateAdminUserAsync(email, username, password, userStore, userManager);
                    logger.LogInformation("Administrator account {Username} was created successfully.", username);
                }

                foreach (var roleName in roles)
                {
                    if (!await userManager.IsInRoleAsync(adminUser, roleName))
                    {
                        IdentityResult userResult = await userManager.AddToRoleAsync(adminUser, roleName);
                        if (!userResult.Succeeded)
                        {
                            throw new InvalidOperationException(string.Format(AddingUserToRole, username, roleName));
                        }

                        logger.LogInformation("User {Username} was added to the '{RoleName}' role.", username, roleName);
                    }
                }

                return app;
            })
                .GetAwaiter()
                .GetResult();

            return app;
        }

        public static IApplicationBuilder AssignWarehousesToAdminByEmail(this IApplicationBuilder app, string adminEmail)
        {
            using IServiceScope serviceScope = app.ApplicationServices.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<WarehouseDbContext>();

            var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();

            if (adminUser == null)
            {
                throw new InvalidOperationException(string.Format(AdminNotFound, adminEmail));
            }

            var warehousesWithoutCreator = dbContext.Warehouses
                .Where(w => w.CreatedByUserId == null)
                .ToList();

            foreach (var warehouse in warehousesWithoutCreator)
            {
                if (warehouse.CreatedByUserId == null)
                {
                    warehouse.CreatedByUserId = adminUser.Id;
                }

                bool alreadyAssigned = dbContext.UsersWarehouses.Any(uw =>
                    uw.ApplicationUserId == adminUser.Id && uw.WarehouseId == warehouse.Id);

                if (!alreadyAssigned)
                {
                    dbContext.UsersWarehouses.Add(new ApplicationUserWarehouse
                    {
                        ApplicationUserId = adminUser.Id,
                        WarehouseId = warehouse.Id
                    });
                }
            }

            dbContext.SaveChanges();

            return app;
        }

        private static async Task<ApplicationUser> CreateAdminUserAsync(string email, string username, string password,
            IUserStore<ApplicationUser> userStore, UserManager<ApplicationUser> userManager)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = email,
                EmailConfirmed = true
            };

            await userStore.SetUserNameAsync(applicationUser, username, CancellationToken.None);
            IdentityResult result = await userManager.CreateAsync(applicationUser, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Format(RegisteringRole, AdminRoleName));
            }

            return applicationUser;
        }
    }
}
