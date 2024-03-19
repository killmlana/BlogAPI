using System.Security.Claims;
using BlogAPI.Entities;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class ClaimMap : ClassMap<CustomClaim>
{
    public ClaimMap()
    {
        Id().GeneratedBy.Assigned();
        Map(claim => claim.ClaimValue);
        Map(claim => claim.ClaimType);
        References<User>(x => x.UserId).Column("UserId");
    }
}