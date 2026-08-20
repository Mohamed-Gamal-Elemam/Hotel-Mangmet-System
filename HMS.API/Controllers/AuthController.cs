using HMS.Infrastructure.Data.Repository;
using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.AuthDtos;
using Shared.Resposne;
using System.Security.Claims;

namespace HMS.API.Controllers
{

    public class AuthController : BaseApiController
    {
        private readonly IAuthServices _authServices;

        public AuthController( IAuthServices authServices)
        {
            _authServices = authServices;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<GenaricResponse<UserDto>>> Register([FromBody] RegisterDto registerDto)
        {
            var response=await _authServices.RegisterUserAsync(registerDto);
            return HandelResponse(response);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<GenaricResponse<UserDto>>> Login([FromBody] LoginDto loginDto)
        {
            var response=await _authServices.LoginAsync(loginDto);
            return HandelResponse(response);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost("Create-StaffUser")]
        public async Task<ActionResult<GenaricResponse<bool>>> CreateStaffUserAsync([FromBody] StaffUserDto staffData)
        {
            var response = await _authServices.CreateStaffAsync(staffData);
            return HandelResponse(response);
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("GetAllUser")]
        public async Task<ActionResult<GenaricResponse<IEnumerable<GetUserDto>>>> GetAllUsersFromAdmin()
        {
            var response = await _authServices.GetUsersAsync();
            return HandelResponse(response);
        }
        [Authorize(Roles ="Admin")]
        [HttpPut("users/{id}/activate")]
        public async Task<ActionResult<GenaricResponse<bool>>> ActivateUser([FromQuery] string id)
        {
            var response=await _authServices.ActivateUserAsync(id);
            return HandelResponse(response);
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("users/{id}/deactivate")]
        public async Task<ActionResult<GenaricResponse<bool>>> DeactivateUser([FromQuery] string id)
        {
            var response=await _authServices.DeActivateUserAsync(id);
            return HandelResponse(response);
        }
        [HttpGet("EmailExists")]
        public async Task<ActionResult<GenaricResponse<bool>>> CheckEmailExises(string email)
        {
            var response = await _authServices.CheckEmailExists(email);
            return HandelResponse(response);
        }
        [Authorize]
        [HttpGet("Profile")]
        public async Task<ActionResult<GenaricResponse<UserProfileDto>>> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _authServices.GetProfileAsync(userId!);
            return HandelResponse(response);

        }
        
    }
}
