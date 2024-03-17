using BlogAPI.Entities;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Id(x => x.Id);
        Map(x => x.Username);
        Map(x => x.HashedPassword);
        References(x => x.Role);
        Map(x => x.DateCreated);
        HasMany(x => x.PostHistory).Inverse().Cascade.All();
        HasMany(x => x.CommentHistory).Inverse().Cascade.All();
    }
}