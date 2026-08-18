using HMS.Core.Entities.SecurityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HMS.Infrastructure.Data.Contexts
{
    public class HotelDbContext : IdentityDbContext<HotelUser>
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }


        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HotelUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<StaffUser>().ToTable("StaffUsers");
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
