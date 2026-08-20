using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.AuthDtos
{
    public class GetUserDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; }
        public string Role { get; set; } = null!;
    }
}
