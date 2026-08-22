using HMS.Core.Entities.SecurityModule;
using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs.AuthDtos;
using Shared.Message;
using Shared.Resposne;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HMS.Services.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailServices _email;
        private readonly ILogger<AuthServices> _logger;

        public AuthServices(UserManager<HotelUser> userManager ,IConfiguration configuration ,IEmailServices email, ILogger<AuthServices> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _email = email;
            _logger = logger;
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
            var UserEmailExist=await _userManager.FindByEmailAsync(registerDto.Email);
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
            var welcomeEmail = new Email
            {
                To = HotelUser.Email,
                Subject = "Welcome to Our Hotel!",
                Body = $"Hello {HotelUser.FullName},\n\nWelcome to our platform! We are thrilled to have you with us."
            };

       
            try
            {
                await _email.SendEmail(welcomeEmail);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the welcome email to user {Email}", registerDto.Email);
            }


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



        public async Task<GenaricResponse<bool>> CreateStaffAsync(StaffUserDto staffData)
        {
            var GenaricResponse=new GenaricResponse<bool>();
            if (staffData is null)
            {
                GenaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                GenaricResponse.Message = "no data to create staff user";
                return GenaricResponse;
            }
            var ExsisStaffeEmail = await _userManager.FindByEmailAsync(staffData.Email);
            if (ExsisStaffeEmail is not null)
            {
                GenaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                GenaricResponse.Message = " Staff email is oredy exsist";
                return GenaricResponse;
            }
            var IsParsed=Enum.TryParse<StaffSpecialistes>(staffData.specialistes, out var StaffSpecialistes);
            if (!IsParsed)
            {
                GenaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                GenaricResponse.Message = " not corect staff specialistes ";
                return GenaricResponse;

            }
            var stuffUser = new StaffUser()
            {
                Email = staffData.Email,
                FullName=staffData.FullName,
                PhoneNumber = staffData.PhoneNumber,
                UserName = staffData.Email.Split('@')[0],
                IsActive = true,
                CreatedAt = DateTime.Now,
                specialistes= StaffSpecialistes
            };
            var result =await _userManager.CreateAsync(stuffUser, staffData.Password);
            if (!result.Succeeded)
            {
                GenaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                GenaricResponse.Message = string.Join('|',result.Errors.Select(E=>E.Description));
                return GenaricResponse;
            }
            await _userManager.AddToRoleAsync(stuffUser, "Staff");

            GenaricResponse.StutesCode = StatusCodes.Status200OK;
            GenaricResponse.Message = "creates Staff User Successfuly";
            GenaricResponse.Data = true;
            return GenaricResponse;

        }

        public async Task<GenaricResponse<IEnumerable<GetUserDto>>> GetUsersAsync()
        {
            var genaricResponse=new GenaricResponse<IEnumerable<GetUserDto>>();
            if (!_userManager.Users.Any()|| _userManager.Users.Count()==0)
            {
                genaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                genaricResponse.Message = "no ueres to show";
                return genaricResponse;
            }
            var ListOfUserToReturn=new List<GetUserDto>();
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user,"Admin"))
                {
                    continue;
                }
                var role=await _userManager.GetRolesAsync(user);
                var UserToReturn = new GetUserDto()
                {
                    Id = user.Id,
                    Email = user.Email!,
                    IsActive = user.IsActive,
                    Role = role.FirstOrDefault()!
                };
                ListOfUserToReturn.Add(UserToReturn);
            }
            genaricResponse.StutesCode = StatusCodes.Status200OK;
            genaricResponse.Message = "Get Users Seccussfly [Guest-Staff]";
            genaricResponse.Data =ListOfUserToReturn ;
            return genaricResponse;
        }

        public async Task<GenaricResponse<bool>> ActivateUserAsync(string Id)
        {
            var GenaricResponse =new GenaricResponse<bool>();
            var user =await _userManager.FindByIdAsync(Id);
            if (user is null)
            {
                GenaricResponse.StutesCode = StatusCodes.Status404NotFound;
                GenaricResponse.Message = "User not exis to Activate";
                return GenaricResponse;
            }

            user.IsActive=true;
            var result= await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                GenaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                GenaricResponse.Message = string.Join("|",result.Errors.Select(E=>E.Description));
                return GenaricResponse;

            }
            GenaricResponse.StutesCode = StatusCodes.Status200OK;
            GenaricResponse.Message = "Success to activate user";
            GenaricResponse.Data = true;
            return GenaricResponse;



        }

        public async Task<GenaricResponse<bool>> DeActivateUserAsync(string Id)
        {
            var GenaricResponse = new GenaricResponse<bool>();
            var user = await _userManager.FindByIdAsync(Id);
            if (user is null)
            {
                GenaricResponse.StutesCode = StatusCodes.Status404NotFound;
                GenaricResponse.Message = "User not exis to Deactivate";
                return GenaricResponse;
            }

            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                GenaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                GenaricResponse.Message = string.Join("|", result.Errors.Select(E => E.Description));
                return GenaricResponse;

            }
            GenaricResponse.StutesCode = StatusCodes.Status200OK;
            GenaricResponse.Message = "Success to deactivate user";
            GenaricResponse.Data = true;
            return GenaricResponse;
        }

        public async Task<GenaricResponse<bool>> CheckEmailExists(string email)
        {
            var genaricResponse=new GenaricResponse<bool>();
            var user = await _userManager.FindByEmailAsync(email);

              genaricResponse.StutesCode= StatusCodes.Status200OK;
              genaricResponse.Message = "Check Email Existes";
              genaricResponse.Data = user is null?false:true;
              return genaricResponse;
            
        }

        public async Task<GenaricResponse<UserProfileDto>> GetProfileAsync(string UserId)
        {
            var genaricResponse = new GenaricResponse<UserProfileDto>();
            var User=await _userManager.FindByIdAsync(UserId);
            if (User == null)
            {
                genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                genaricResponse.Message = "Not found Profile User";
                return genaricResponse;
            }
            var userProfile = new UserProfileDto()
            {
                Email = User.Email!,
                PhoneNumber = User.PhoneNumber!,
                UserName = User.UserName!,
                FullName = User.FullName!,

            };
            genaricResponse.StutesCode=StatusCodes.Status200OK;
            genaricResponse.Message = "Seccess found profile user";
            genaricResponse.Data=userProfile;
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
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                Claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var card = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature);

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
