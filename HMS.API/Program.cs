
using HMS.API.Extantion;
using HMS.Core.Contracts;
using HMS.Core.Entities.SecurityModule;
using HMS.Infrastructure.Data.Contexts;
using HMS.Infrastructure.Data.DataSeed;
using HMS.Infrastructure.Data.Repository;
using HMS.Infrastructure.ExternalServices;
using HMS.Services;
using HMS.Services.Helper;
using HMS.Services.Services;
using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(ServiceAssemblyRefrance).Assembly);
            });
            builder.Services.AddScoped<IRoomService,RoomService>();
            builder.Services.AddTransient<IAttachementService, AttachementService>();
            builder.Services.AddScoped<IDataIntializer,DataIntializer>();
            builder.Services.AddIdentity<HotelUser, IdentityRole>()
                .AddEntityFrameworkStores<HotelDbContext>();
            builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSetting"));

            builder.Services.AddScoped<IAuthServices,AuthServices>();
            builder.Services.AddTransient<IEmailServices, EmailServices>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                        ValidAudience = builder.Configuration["JWT:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
                        ClockSkew = TimeSpan.FromHours(1)
                    };
                });
            
            var app = builder.Build();

            await app.MigrateDatabaseAsync();
            await app.SeedingIdentityDataAsync();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI();
                app.UseSwagger();
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapControllers();

            app.Run();
        }
    }
}
