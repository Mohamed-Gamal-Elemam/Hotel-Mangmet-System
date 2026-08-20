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
        public Task<GenaricResponse<bool>> CreateStaffAsync(StaffUserDto staffData);
        public Task<GenaricResponse<IEnumerable<GetUserDto>>> GetUsersAsync();
        public Task<GenaricResponse<bool>> ActivateUserAsync(string Id);
        public Task<GenaricResponse<bool>> DeActivateUserAsync(string Id);
        public Task<GenaricResponse<bool>> CheckEmailExists(string email);
        public Task<GenaricResponse<UserProfileDto>> GetProfileAsync (string UserId);

        
        
    }
}
