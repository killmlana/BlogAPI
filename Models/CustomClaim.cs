using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Entities;

public class CustomClaim : IdentityUserClaim<string>
{
    public new virtual string Id { get; set; }
    public new virtual string ClaimType { get; set; }
    public new virtual string ClaimValue { get; set; }
    public virtual User User { get; set; }
    
    public virtual Claim ConvertToClaim(string id, string claimType, string claimValue, User user)
    {
        return new CustomClaim() {Id = id, ClaimType = claimType, ClaimValue = claimValue, UserId = user.Id}.ToClaim();
    }
}