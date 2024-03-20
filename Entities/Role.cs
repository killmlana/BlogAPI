using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Entities;

public class Role : IdentityRole
{
    public new virtual string Id { get; set; }
    public virtual string Name { get; set; }
    public virtual IList<User> Users { get; set; }

    public Role()
    {
        Users = new List<User>();
    }
    public Role(string id, string name)
    {
        Id = id;
        Name = name;
        Users = new List<User>();
    }
}