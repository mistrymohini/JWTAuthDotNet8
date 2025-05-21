﻿using JWTAuthDotNet8.Data;
using JWTAuthDotNet8.Entities;
using JWTAuthDotNet8.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthDotNet8.Services
{
    public class AuthService(UserDBContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<string?> LoginAsync(UserModel request)
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
            return CreateToken(user);
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
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
