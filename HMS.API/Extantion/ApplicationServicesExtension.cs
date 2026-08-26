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
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HMS.API.Extantion
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddDbContextServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HotelDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<HotelUser, IdentityRole>()
                .AddEntityFrameworkStores<HotelDbContext>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(ServiceAssemblyRefrance).Assembly);
            });

            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IBookingServices, BookingServices>();
            services.AddTransient<IAttachementService, AttachementService>();
            services.AddScoped<IDataIntializer, DataIntializer>();
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddTransient<IEmailServices, EmailServices>();
            services.AddScoped<IPaymentServices, PaymentServices>();

            services.Configure<EmailSetting>(configuration.GetSection("EmailSetting"));

            return services;
        }

        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
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
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidAudience = configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"] ?? string.Empty)),
                    ClockSkew = TimeSpan.FromHours(1)
                };
            });

            return services;
        }

        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddOpenApi();
            services.AddSwaggerGen();

            return services;
        }
    }
}