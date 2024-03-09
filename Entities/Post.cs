using System.Collections.ObjectModel;

namespace BlogAPI.Entities;

public class Post
{
    public virtual string Id { get; set; }
    public virtual User Owner { get; set; }
    public virtual string Title { get; set; }
    public virtual string Content { get; set; }
    public virtual long DateCreated { get; set; }
    public virtual long DateModified { get; set; }
    public virtual int Category { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
    public virtual int Likes { get; set; }

    public Post(string id, User owner, string title, string content, long dateCreated, long dateModified)
    {
        Id = id;
        Owner = owner;
        Title = title;
        Content = content;
        DateCreated = dateCreated;
        DateModified = dateModified;
        Comments = new Collection<Comment>();
    }

    public virtual void AddPost(User user)
    {
        user.AddPost(this);
    }
}