namespace JWTAuthDotNet8.Models
{
    public class TokenResponseModel
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
