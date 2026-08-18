using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.AuthDtos
{
    public class UserDto
    {
        public string Email { get; set; } = null!;
        public string FullName  { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
