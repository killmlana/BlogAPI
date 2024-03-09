namespace BlogAPI.Entities;

public class Reply
{
    public virtual int Id { get; set; }
    public virtual int Owner { get; set; }
    public virtual string Content { get; set; }
    public virtual string Likes { get; set; }
}