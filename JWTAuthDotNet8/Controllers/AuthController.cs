using JWTAuthDotNet8.Entities;
using JWTAuthDotNet8.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthDotNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
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
            string token = "success";
            return Ok(token);
        }
    }
}
