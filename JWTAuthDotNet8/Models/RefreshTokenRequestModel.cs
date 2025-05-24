namespace JWTAuthDotNet8.Models
{
    public class RefreshTokenRequestModel
    {
        public Guid UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
