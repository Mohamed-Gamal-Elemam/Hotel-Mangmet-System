using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Core.Entities.SecurityModule
{
    public class HotelUser :IdentityUser
    {
        public string FullName { get; set; } = null!;
        public bool  IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
