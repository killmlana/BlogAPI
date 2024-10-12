using BlogAPI.Entities;
using BlogAPI.Models;
using FluentNHibernate.Mapping;

namespace BlogAPI.Mapping;

public class RoleMap : ClassMap<Role>
{ 
    public RoleMap()
    {
        Id(x => x.Id);
        Map(x => x.Name);
        HasMany(x => x.Users).Inverse();
        HasMany<CustomRoleClaim>(x => x.Claims).Inverse().Cascade.All();
    }
}