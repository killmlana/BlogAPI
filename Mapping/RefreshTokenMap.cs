using BlogAPI.Entities;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class RefreshTokenMap : ClassMap<RefreshToken>
{
    public RefreshTokenMap()
    {
        Id(x => x.Token);
        References(x => x.User).Not.LazyLoad();
        Map(x => x.Expires);
        Map(x => x.Created);
        Map(x => x.Revoked);
    }
    
}