using System.Collections.ObjectModel;
using System.Runtime.InteropServices.JavaScript;

namespace BlogAPI.Entities;

public class Post
{
    public virtual int Id { get; set; }
    public virtual User Owner { get; set; }
    public virtual string Title { get; set; }
    public virtual string Content { get; set; }
    public virtual JSType.Date DateCreated { get; set; }
    public virtual JSType.Date DateModified { get; set; }
    public virtual int Category { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
    public virtual int Likes { get; set; }

    public Post()
    {
        Comments = new Collection<Comment>();
    }
    
    public virtual void AddPost(Post post)
    {
        PostHistory.Add(post);
    }

    public virtual void AddComment(Comment comment)
    {
        CommentHistory.Add(comment);
        
    }
    
}