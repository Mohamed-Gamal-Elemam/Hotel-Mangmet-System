using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.AuthDtos
{
    public class UserProfileDto
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        }
}
