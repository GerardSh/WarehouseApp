namespace WarehouseApp.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using Data;

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
    }
}
