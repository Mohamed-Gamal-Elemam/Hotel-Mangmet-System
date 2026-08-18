using HMS.Core.Contracts;
using HMS.Core.Entities.SecurityModule;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Data.DataSeed
{
    public class DataIntializer : IDataIntializer
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataIntializer(UserManager<HotelUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task InitializeDataAsync()
        {
            if (!_roleManager.Roles.Any())
            {
                var AdminRole = new IdentityRole() { Name = "Admin" };
                var StaffRole = new IdentityRole() { Name = "Staff" };
                var GuestRole = new IdentityRole() { Name = "Guest" };
               await _roleManager.CreateAsync(AdminRole);
               await _roleManager.CreateAsync(StaffRole);
               await _roleManager.CreateAsync(GuestRole);
            }
            if (! _userManager.Users.Any())
            {
                var Admin = new HotelUser()
                {
                    FullName = "Admin.Hotel",
                    Email = "Admin.HMS@gmail.com",
                    UserName = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,

                };
                await _userManager.CreateAsync(Admin,"P@ssw0rd");
                await _userManager.AddToRoleAsync(Admin,"Admin");
            }
        }
    }
}
