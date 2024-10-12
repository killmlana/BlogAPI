namespace BlogAPI.Entities;

public class RefreshToken
{ 
        public virtual string Token { get; set; } 
        public virtual User User { get; set; }
        public virtual long Expires { get; set; }
        public virtual bool IsExpired => DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= Expires;
        public virtual long Created { get; set; }
        public virtual long? Revoked { get; set; }
        public virtual bool IsActive => Revoked == null && !IsExpired;
        
        public virtual void AddRefreshToken(User user)
        {
                user.AddRefreshToken(this);
        }
        
}