using BlogAPI.Entities;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class UserClaimMap : ClassMap<CustomUserClaim>
{
    public UserClaimMap()
    {
        Id(claim => claim.Id);
        Map(claim => claim.ClaimValue);
        Map(claim => claim.ClaimType);
        References(x => x.User, "UserId");
    }
}