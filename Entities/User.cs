using System.Collections.ObjectModel;
using System.Runtime.InteropServices.JavaScript;

namespace BlogAPI.Entities;

public class User
{
    public virtual int Id { get; set; }
    public virtual string Username { get; set; }
    public virtual int Role { get; set; }
    internal virtual string HashedPassword { get; set; }
    public virtual ICollection<Post> PostHistory { get; set; }
    public virtual ICollection<Comment> CommentHistory { get; set; }
    public virtual JSType.Date DateCreated { get; set; }

    public User()
    {
        PostHistory = new Collection<Post>();
        CommentHistory = new Collection<Comment>();
    }

    public virtual void AddHashedPassword(string hash)
    {
        HashedPassword = hash;
    }
}