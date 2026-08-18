using Shared.DTOs.AuthDtos;
using Shared.Resposne;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.ServicesAbstraction
{
    public interface IAuthServices
    {
        public Task<GenaricResponse<UserDto>> RegisterUserAsync(RegisterDto registerDto);
        public Task<GenaricResponse<UserDto>> LoginAsync(LoginDto loginDto);
    }
}
