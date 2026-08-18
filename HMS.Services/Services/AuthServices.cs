using HMS.Core.Entities.SecurityModule;
using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs.AuthDtos;
using Shared.Resposne;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace HMS.Services.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthServices(UserManager<HotelUser> userManager ,IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<GenaricResponse<UserDto>> RegisterUserAsync(RegisterDto registerDto)
        {
            var GenaricResponse=new GenaricResponse<UserDto>();
            if (registerDto == null)
            {
                GenaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                GenaricResponse.Message = "no data to register";
                return GenaricResponse;
            }
            var UserEmailExist=_userManager.FindByEmailAsync(registerDto.Email);
            if (UserEmailExist is not null)
            {
                GenaricResponse.StutesCode= StatusCodes.Status400BadRequest;
                GenaricResponse.Message = "This Email Alredy Exist";
                return GenaricResponse;
            }
            var HotelUser = new HotelUser()
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                UserName = registerDto.Email.Split('@')[0],
                FullName = registerDto.FullName,
                IsActive=true,
                CreatedAt = DateTime.Now,   

            };

            var result= await _userManager.CreateAsync(HotelUser,registerDto.Password);
            if (!result.Succeeded)
            {
                GenaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                GenaricResponse.Message = string.Join("|",result.Errors.Select(r => r.Description));
                return GenaricResponse;

            }
            await _userManager.AddToRoleAsync(HotelUser, "Guest");

            GenaricResponse.StutesCode = StatusCodes.Status200OK;
            GenaricResponse.Message = "success to register New Guest";
            GenaricResponse.Data = new UserDto()
            {
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                Token = await GenrateTokenAsync(HotelUser)
            };
            return GenaricResponse;





        }
        public async Task<GenaricResponse<UserDto>> LoginAsync(LoginDto loginDto)
        {
            var genaricResponse = new GenaricResponse<UserDto>();
            if (loginDto is null)
            {
                genaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                genaricResponse.Message = "No data to login";
                return genaricResponse;
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                genaricResponse.StutesCode = StatusCodes.Status401Unauthorized;
                genaricResponse.Message = "Invaild Email or Password";
                return genaricResponse;

            }

            if (!user.IsActive)
            {
                genaricResponse.StutesCode= StatusCodes.Status403Forbidden;
                genaricResponse.Message = "Your account is deactivate ,please call the Adminstrator";
                return genaricResponse;
            }
            var correctPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!correctPassword)
            {
                genaricResponse.StutesCode = StatusCodes.Status401Unauthorized;
                genaricResponse.Message = "Invaild Email or Password";
                return genaricResponse;

            }
            genaricResponse.StutesCode = StatusCodes.Status200OK;
            genaricResponse.Message = "Login Seccussed";
            genaricResponse.Data = new UserDto()
            {
                FullName = user.FullName,
                Email = user.Email,
                Token = await GenrateTokenAsync(user)
            };
            return genaricResponse;
        }

        private async Task<string> GenrateTokenAsync(HotelUser user)
        {
            var Claims = new List<Claim>()
            {
               new Claim(JwtRegisteredClaimNames.Email, user.Email!),
               new Claim(JwtRegisteredClaimNames.NameId, user.Id),
               new Claim("Activated" ,user.IsActive.ToString())
            };
           var roles=await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
               Claims.Add( new Claim(ClaimTypes.Role, role));
            }

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var card=new SigningCredentials(authKey,SecurityAlgorithms.HmacSha256Signature);

            var Token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: Claims,
                signingCredentials: card,
                expires: DateTime.Now.AddHours(1)
                );
            
            var TokenHandler = new JwtSecurityTokenHandler();
            var tokenString = TokenHandler.WriteToken(Token);
            return tokenString;




        }

    }
}
