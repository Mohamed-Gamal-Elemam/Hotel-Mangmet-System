using HMS.Core.Contracts;
using HMS.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HMS.API.Extantion
{
    public static class WebApplicarionRegister
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await dbContext.Database.MigrateAsync();
            }
            return app;
        }

        public static async Task<WebApplication> SeedingIdentityDataAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataIntializer>();
            await dataInitializer.InitializeDataAsync();
            return app;
        }
    }
}