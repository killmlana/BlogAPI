using BlogAPI.Entities;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class PostMap : ClassMap<Post>
{
    public PostMap()
    {
        Id(x => x.Id);
        References(x => x.Owner);
        Map(x => x.Title);
        Map(x => x.Content);
        Map(x => x.DateCreated);
        Map(x => x.DateModified);
        HasMany(x => x.Comments).Inverse().Cascade.All();
    }
}