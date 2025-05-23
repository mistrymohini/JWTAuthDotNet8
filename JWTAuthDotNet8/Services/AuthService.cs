using JWTAuthDotNet8.Data;
using JWTAuthDotNet8.Entities;
using JWTAuthDotNet8.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuthDotNet8.Services
{
    public class AuthService(UserDBContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<TokenResponseModel?> LoginAsync(UserModel request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user is null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var response = new TokenResponseModel
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };

            return response;
        }

        public async Task<User?> RegisterAsync(UserModel request)
        {
            if( await context.Users.AnyAsync(u=>u.Username==request.Username))
            {
                return null;
            }
            var user = new User();

            var hashPassword = new PasswordHasher<User>()
              .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashPassword;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomnumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomnumber); 
            return Convert.ToBase64String(randomnumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshtoken=GenerateRefreshToken();    
            user.RefreshToken = refreshtoken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshtoken;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role)
            };
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecureAndRandomKeyThatLooksJustAwesomeAndNeedsToBeVeryLongLongLong"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: "MyAwesomeApp",
                audience: "MyAwesomeAudience",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
