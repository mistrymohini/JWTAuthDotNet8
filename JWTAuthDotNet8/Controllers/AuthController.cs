using JWTAuthDotNet8.Entities;
using JWTAuthDotNet8.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthDotNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        public static User user=new User();
        [HttpPost("register")]
        public ActionResult<User> Register(UserModel userModel)
        {
            var hashPassword = new PasswordHasher<User>()
                .HashPassword(user, userModel.Password);

            user.Username=userModel.Username;
            user.PasswordHash = hashPassword;

            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserModel userModel)
        {
            if (user.Username != userModel.Username) 
            {
                return BadRequest("User Not Found");
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, userModel.Password) == PasswordVerificationResult.Failed)
            {
                return BadRequest("Wrong Password");
            }
            string token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>()
            { 
                new Claim(ClaimTypes.Name,user.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings: Issuer"),
                audience: configuration.GetValue<string>("AppSettings: Audience"),
                claims: claims, 
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials:creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
