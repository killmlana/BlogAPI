using BlogAPI.Models;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class RoleClaimMap : ClassMap<CustomRoleClaim>
{
    public RoleClaimMap()
    {
        Id(claim => claim.Id);
        Map(claim => claim.ClaimValue);
        Map(claim => claim.ClaimType);
        References(x => x.Role, "RoleId");
    }
}