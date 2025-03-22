namespace BackOfficeInventoryApi.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }=string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
