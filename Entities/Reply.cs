namespace BlogAPI.Entities;

public class Reply
{
    public virtual int Id { get; set; }
    public virtual User Owner { get; set; }
    public virtual string Content { get; set; }

    public Reply(int id, User owner, string content)
    {
        Id = id;
        Owner = owner;
        Content = content;
    }

    public virtual void AddReply(Comment comment)
    {
        comment.AddReply(this);
    }
}