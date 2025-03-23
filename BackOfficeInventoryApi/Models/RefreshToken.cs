namespace BackOfficeInventoryApi.Models
{
    public class RefreshToken
    {
        public Guid UserId { get; set; }

        public required string RefreshTokens { get; set; }
    }
}
