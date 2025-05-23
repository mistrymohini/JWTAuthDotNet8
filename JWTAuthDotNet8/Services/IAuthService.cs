﻿using JWTAuthDotNet8.Entities;
using JWTAuthDotNet8.Models;

namespace JWTAuthDotNet8.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserModel request);

        Task<TokenResponseModel?> LoginAsync(UserModel request);

        Task<TokenResponseModel?> RefreshTokenAsync(RefreshTokenRequestModel request);
    }
}
