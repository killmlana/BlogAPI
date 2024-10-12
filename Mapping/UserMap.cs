using BlogAPI.Entities;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Id(x => x.Id);
        Map(x => x.Name);
        Map(x => x.HashedPassword);
        References(x => x.Role).Not.LazyLoad();
        HasMany(x => x.Claims).Inverse().Not.LazyLoad();
        Map(x => x.DateCreated);
        HasMany(x => x.PostHistory).Inverse().Cascade.All().Not.LazyLoad();
        HasMany(x => x.CommentHistory).Inverse().Cascade.All().Not.LazyLoad();
        HasMany(x => x.RefreshTokens).Inverse().Not.LazyLoad();
    }
}