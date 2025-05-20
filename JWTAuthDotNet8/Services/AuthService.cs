using JWTAuthDotNet8.Data;
using JWTAuthDotNet8.Entities;
using JWTAuthDotNet8.Models;

namespace JWTAuthDotNet8.Services
{
    public class AuthService(UserDBContext context, IConfiguration configuration) : IAuthService
    {
        Task<string?> IAuthService.LoginAsync(UserModel request)
        {
            throw new NotImplementedException();
        }

        Task<User?> IAuthService.RegisterAsync(UserModel request)
        {
            throw new NotImplementedException();
        }
    }
}
