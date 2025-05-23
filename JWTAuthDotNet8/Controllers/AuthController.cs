using JWTAuthDotNet8.Entities;
using JWTAuthDotNet8.Models;
using JWTAuthDotNet8.Services;
using Microsoft.AspNetCore.Authorization;
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
    public class AuthController(IAuthService authService) : ControllerBase
    {
        public static User user=new User();
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserModel userModel)
        {
            var user=await authService.RegisterAsync(userModel);
            if (user is null)
                return BadRequest("User already exist!");

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseModel>> Login(UserModel userModel)
        {
            var result = await authService.LoginAsync(userModel);
            if (result is null)
                return BadRequest("Invalid Username or Password!");

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndPoint() {
            return Ok("You are Authenticated!");
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndPoint()
        {
            return Ok("You are an Admin!");
        }
    }
}
