using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Entities;

public class CustomClaim : IdentityUserClaim<string>
{
    public new string Issuer { get;}
    public new string Type { get;}
    public new string OriginalIssuer { get;}
    public new string ValueType { get; }
    public new string Value { get; }
    public new ClaimsIdentity? Subject { get; }
    public new IDictionary<string, string> Properties { get; }
    public IList<User> Owners { get; set; }

    public CustomClaim()
    {
        Owners = new List<User>();
    }

    public virtual void AddClaim(User user, Claim claim)
    {
        user.Claims.Add(claim);
        Owners.Add(user);
    }
}