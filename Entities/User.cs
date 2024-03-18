using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Entities;

public class User : IdentityUser
{
    public new virtual string Id { get; set; }
    public virtual string Username { get; set; }
    public virtual Role Role { get; set; }
    public virtual string? HashedPassword { get; set; }
    public virtual IList<Claim> Claims { get; set; }
    public virtual IList<Post> PostHistory { get; set; }
    public virtual IList<Comment> CommentHistory { get; set; }
    public virtual long DateCreated { get; set; }

    public User()
    {
        PostHistory = new List<Post>();
        CommentHistory = new List<Comment>();
        Claims = new List<Claim>();
    }

    public User(string id, string username, string hashedPassword, Role role, long dateCreated)
    {
        Id = id;
        Username = username;
        HashedPassword = hashedPassword;
        Role = role;
        DateCreated = dateCreated;
        PostHistory = new List<Post>();
        CommentHistory = new List<Comment>();
        Claims = new List<Claim>();
    }

    public virtual void AddPost(Post post)
    {
        PostHistory.Add(post);
        post.Owner = this;
    }

    public virtual void AddComment(Comment comment)
    {
        CommentHistory.Add(comment);
        comment.Owner = this;
    }
}