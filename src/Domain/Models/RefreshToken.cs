namespace BookingSystem.Domain.Models
{
    public class RefreshToken
    {
        public RefreshToken() { }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
        public bool IsRevoked { get; set; }
        public string? IpAddress { get; set; }
        public User? User { get; set; }

        public RefreshToken(Guid id, string token, DateTime createdAt, DateTime expireDate, string ip)
        {
            Id = Guid.NewGuid();
            UserId = id;
            Token = token;
            CreatedAt = createdAt;
            ExpireDate = expireDate;
            IpAddress = ip;
        }
    }
}