using login.Configurations;
using login.Models.DTOs;
using login.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace login.Controllers
{
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        //private readonly JwtConfig _jwtConfig;

        public AuthenticationController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            //_jwtConfig = jwtConfig;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterationRequestDto requestDto)
        {
            //Validate the incoming request
            if (ModelState.IsValid)
            {
                //check if the email already exist
                var user_exist = await _userManager.FindByEmailAsync(requestDto.Email);
                if(user_exist != null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Email already exist"
                        }
                    });  
                }

                // create a user
                var new_user = new IdentityUser()
                {
                    Email = requestDto.Email,
                    UserName = requestDto.Email
                };

                var is_created = await _userManager.CreateAsync(new_user, requestDto.Password);

                if(is_created.Succeeded){
                    //Generate the token
                    var token = GenerateJwtToken(new_user);

                    return Ok(new AuthResult()
                    {
                        Result = true,
                        Token = token
                    });
                }
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>() {
                        "server_error"
                    }
                });
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
                });
            }

            //check if the user exist
            var existing_user = await _userManager.FindByEmailAsync(requestDto.Email);
            if(existing_user == null)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Invalid payload"
                    }
                });
            }

            //check the password
            var isCorrect = await _userManager.CheckPasswordAsync(existing_user, requestDto.Password);
            if (!isCorrect)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Invalied credentials"
                    }
                });
            }

            var jwtToken = GenerateJwtToken(existing_user);
            return Ok(new AuthResult()
            {
                Result = true,
                Token = jwtToken
            });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            //Token descripter
            var tokenDescripter = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, value:user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),

                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescripter);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
