using System.Runtime.InteropServices.JavaScript;

namespace BlogAPI.Entities;

public class Comment
{
    public virtual string Id { get; set; }
    public virtual User Owner { get; set; }
    public virtual string Content { get; set; }
    public virtual int Likes { get; set; }
    public virtual string PostId { get; set; }
    public virtual IList<Reply> Replies { get; set; }
    public virtual long DateCreated { get; set; }
    public virtual long DateModified { get; set; }

    public Comment(string id, User owner, string content, IList<int> replies, int likes, string postId, long dateCreated, long dateModified)
    {
        Id = id;
        Owner = owner;
        Content = content;
        Likes = likes;
        PostId = postId;
        DateCreated = dateCreated;
        DateModified = dateModified;
        Replies = new List<Reply>();
    }

    public virtual void AddComment(User user)
    {
        user.AddComment(this);
    }

    public virtual void AddReply(Reply reply)
    {
        Replies.Add(reply);
    }
}