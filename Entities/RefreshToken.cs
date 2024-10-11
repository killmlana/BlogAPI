namespace BlogAPI.Entities;

public class RefreshToken
{ 
        public string Token { get; set; } 
        public User User { get; set; }
        public long Expires { get; set; }
        public bool IsExpired => DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= Expires;
        public long Created { get; set; }
        public long? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        
        public virtual void AddRefreshToken(User user)
        {
                user.AddRefreshToken(this);
        }
}