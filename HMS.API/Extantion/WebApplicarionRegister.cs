using HMS.Core.Contracts;
using HMS.Infrastructure.Data.Contexts;
using HMS.Infrastructure.Data.DataSeed;
using Microsoft.EntityFrameworkCore;

namespace HMS.API.Extantion
{
    public static class WebApplicarionRegister
    {
        public static async Task<WebApplication> MigrateDatabaseAsync (this WebApplication app)
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
            await using var Scope = app.Services.CreateAsyncScope();
            var DataIntializer = Scope.ServiceProvider.GetRequiredService<IDataIntializer>();
            await DataIntializer.InitializeDataAsync();
            return app;

        }
    }
}
