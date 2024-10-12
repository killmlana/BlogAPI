using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Entities;

public class User : IdentityUser
{
    public new virtual string Id { get; set; }
    public virtual string Name { get; set; }
    public virtual Role Role { get; set; }
    public virtual string? HashedPassword { get; set; }
    public virtual IList<CustomUserClaim> Claims { get; set; }
    public virtual IList<Post> PostHistory { get; set; }
    public virtual IList<Comment> CommentHistory { get; set; }
    public virtual long DateCreated { get; set; }
    
    public virtual IList<RefreshToken> RefreshTokens { get; set; }

    public User()
    {
        PostHistory = new List<Post>();
        CommentHistory = new List<Comment>();
        Claims = new List<CustomUserClaim>();
        RefreshTokens = new List<RefreshToken>();
    }

    public User(string id, string name, string hashedPassword, Role role, long dateCreated)
    {
        Id = id;
        Name = name;
        HashedPassword = hashedPassword;
        Role = role;
        DateCreated = dateCreated;
        PostHistory = new List<Post>();
        CommentHistory = new List<Comment>();
        Claims = new List<CustomUserClaim>();
        RefreshTokens = new List<RefreshToken>();
    }

    public virtual void AddPost(Post post)
    {
        PostHistory.Add(post);
        post.Owner = this;
    }
    
    public virtual void AddClaim(CustomUserClaim userClaim)
    {
        Claims.Add(userClaim);
    }


    public virtual void AddComment(Comment comment)
    {
        CommentHistory.Add(comment);
        comment.Owner = this;
    }

    public virtual void AddRefreshToken(RefreshToken refreshToken)
    {
        RefreshTokens.Add(refreshToken);
    }
}