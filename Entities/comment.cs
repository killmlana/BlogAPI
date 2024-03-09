using System.Runtime.InteropServices.JavaScript;

namespace BlogAPI.Entities;

public class comment
{
    public virtual int id { get; set; }
    public virtual int owner { get; set; }
    public virtual string content { get; set; }
    public virtual int likes { get; set; }
    public virtual int postID { get; set; }
    public virtual JSType.Date dateCreated { get; set; }
    public virtual JSType.Date dateModified { get; set; }
}