using System.Runtime.InteropServices.JavaScript;

namespace BlogAPI.Entities;

public class Comment
{
    public virtual int Id { get; set; }
    public virtual User Owner { get; set; }
    public virtual string Content { get; set; }
    public virtual int Likes { get; set; }
    public virtual int PostId { get; set; }
    public virtual ICollection<int> Replies { get; set; }
    public virtual JSType.Date DateCreated { get; set; }
    public virtual JSType.Date DateModified { get; set; }
}