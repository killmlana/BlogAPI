namespace BlogAPI.Entities;

public class User
{
    public virtual string Id { get; set; }
    public virtual string Username { get; set; }
    public virtual int Role { get; set; }
    internal virtual string HashedPassword { get; set; }
    public virtual IList<Post> PostHistory { get; set; }
    public virtual IList<Comment> CommentHistory { get; set; }
    public virtual long DateCreated { get; set; }

    public User(string id, string username, string hashedPassword, int role, long dateCreated)
    {
        Id = id;
        Username = username;
        HashedPassword = hashedPassword;
        Role = Role;
        DateCreated = dateCreated;
        PostHistory = new List<Post>();
        CommentHistory = new List<Comment>();
    }

    public void AddPost(Post post)
    {
        PostHistory.Add(post);
    }

    public void AddComment(Comment comment)
    {
        CommentHistory.Add(comment);
    }
}