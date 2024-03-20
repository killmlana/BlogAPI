using BlogAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models;

public class CustomRoleClaim : IdentityRoleClaim<string>
{
    public new virtual string Id { get; set; }
    public new virtual string ClaimType { get; set; }
    public new virtual string ClaimValue { get; set; }
    public new virtual string RoleId { get; set; }
    public virtual Role Role { get; set; }
}