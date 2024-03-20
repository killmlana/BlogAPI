using System.Security.Claims;
using BlogAPI.Entities;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class ClaimMap : ClassMap<CustomClaim>
{
    public ClaimMap()
    {
        Id(claim => claim.Id);
        Map(claim => claim.ClaimValue);
        Map(claim => claim.ClaimType);
        Map(claim => claim.UserId);
        References(x => x.User);
    }
}