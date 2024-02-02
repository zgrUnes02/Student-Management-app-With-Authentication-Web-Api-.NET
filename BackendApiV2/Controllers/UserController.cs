using BackendApiV2.Contexts;
using BackendApiV2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendApiV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public UserController(AppDbContext appDbContext) 
        {
            this.appDbContext = appDbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserResquest loginUserResquest)
        {
            // If there is no credential
            if ( loginUserResquest == null )
            {
                return BadRequest();
            }

            //! Search for user in databse
            var user = await appDbContext.Users.FirstOrDefaultAsync(res => res.Email == loginUserResquest.Email && res.Password == loginUserResquest.Password);
            
            // If user exists
            if ( user == null )
            {
                return NotFound(new { message = "User not found" });
            }

            // return message success if user login
            user.Token = GenerateJwtToken(user);
            return Ok(new 
                { 
                    token = user.Token,
                    message = "Login with success" 
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest registerUserRequest)
        {
            // If there is no credential
            if (registerUserRequest == null)
            {
                return BadRequest();
            }

            // Check if there any user with this email
            var user = await appDbContext.Users.FirstOrDefaultAsync(res => res.Email == registerUserRequest.Email);

            // If there is user with email
            if ( user == null ) 
            {
                var newUser = new User()
                {
                    Id = Guid.NewGuid(),
                    FullName = registerUserRequest.FullName,
                    Email = registerUserRequest.Email,
                    Password = registerUserRequest.Password,
                    Role = registerUserRequest.Role,
                    Token = registerUserRequest.Token,
                };

                // Add the new user and save
                await appDbContext.AddAsync(newUser);
                await appDbContext.SaveChangesAsync();

                // Return message success
                return Ok(new { message = "The account has been created with success" });
            }

            return NotFound(new { message = "This email is already exists" });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("this is my custom Secret key for authnetication");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role , user.Role),
                new Claim(ClaimTypes.Name , user.FullName),
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key) , SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = credentials,
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
