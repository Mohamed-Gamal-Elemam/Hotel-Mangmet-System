using HMS.Infrastructure.Data.Repository;
using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.AuthDtos;
using Shared.Resposne;

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
        public async Task<ActionResult<GenaricResponse<UserDto>>> Register(RegisterDto registerDto)
        {
            var response=await _authServices.RegisterUserAsync(registerDto);
            return HandelResponse(response);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<GenaricResponse<UserDto>>> Login(LoginDto loginDto)
        {
            var response=await _authServices.LoginAsync(loginDto);
            return HandelResponse(response);
        }
    }
}
