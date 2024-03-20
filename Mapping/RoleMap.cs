using BlogAPI.Entities;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class RoleMap : ClassMap<Role>
{ 
    public RoleMap()
    {
        Id(x => x.Id);
        Map(x => x.Name);
        HasMany(x => x.Users).Inverse();
    }
}