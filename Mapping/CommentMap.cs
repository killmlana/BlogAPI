using BlogAPI.Entities;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class CommentMap : ClassMap<Comment>
{
    public CommentMap()
    {
        Id(x => x.Id);
        References(x => x.Owner);
        Map(x => x.Content);
        Map(x => x.DateCreated);
        Map(x => x.DateModified);
        References(x => x.Post);
    }
}