using System.Runtime.InteropServices.JavaScript;

namespace BlogAPI.Entities;

public class post
{
    public virtual int id { get; set; }
    public virtual int owner { get; set; }
    public virtual string title { get; set; }
    public virtual string content { get; set; }
    public virtual JSType.Date dateCreated { get; set; }
    public virtual JSType.Date dateModified { get; set; }
    public virtual int category { get; set; }
    public virtual ICollection<int> comments { get; set; }
    public virtual int likes { get; set; }
}