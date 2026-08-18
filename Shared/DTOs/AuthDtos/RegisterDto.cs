using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs.AuthDtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage ="Email is Requires")]
        [EmailAddress(ErrorMessage ="Enter valid email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is Requires")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "PhoneNumber is Requires")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "FullName is Requires")]
        public string FullName { get; set; } = null!;

    }
}
