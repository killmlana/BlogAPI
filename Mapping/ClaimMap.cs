using System.Security.Claims;
using BlogAPI.Entities;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class ClaimMap : ClassMap<Claim>
{
    public ClaimMap()
    {
        Id().GeneratedBy.Assigned();
        Map(x => x.Issuer);
        Map(x => x.Type);
        Map(x => x.OriginalIssuer);
        Map(x => x.ValueType);
        References<User>(x => x.Value);
    }
}