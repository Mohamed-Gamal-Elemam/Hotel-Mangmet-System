
using HMS.API.Extantion;
using HMS.Core.Contracts;
using HMS.Infrastructure.Data.Contexts;
using HMS.Infrastructure.Data.Repository;
using HMS.Services;
using HMS.Services.Helper;
using HMS.Services.Services;
using HMS.ServicesAbstraction;
using Microsoft.EntityFrameworkCore;

namespace HMS.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<HotelDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            builder.Services.AddAutoMapper(typeof(ServiceAssemblyRefrance).Assembly);
            builder.Services.AddScoped<IRoomService,RoomService>();
            builder.Services.AddTransient   <IAttachementService, AttachementService>();

            var app = builder.Build();

            await app.MigrateDatabaseAsync();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI();
                app.UseSwagger();
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseStaticFiles();


            app.MapControllers();

            app.Run();
        }
    }
}
