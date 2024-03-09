using System.Collections.ObjectModel;

namespace BlogAPI.Entities;

public sealed class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public int Role { get; set; }
    internal string HashedPassword { get; set; }
    public ICollection<Post> PostHistory { get; set; }
    public ICollection<Comment> CommentHistory { get; set; }
    public long DateCreated { get; set; }

    public User(string id, string username, string hashedPassword, int role, long dateCreated)
    {
        Id = id;
        Username = username;
        HashedPassword = hashedPassword;
        Role = Role;
        DateCreated = dateCreated;
        PostHistory = new Collection<Post>();
        CommentHistory = new Collection<Comment>();
    }
}