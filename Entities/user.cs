using System.Runtime.InteropServices.JavaScript;

namespace BlogAPI.Entities;

public class user
{
    public virtual int uid { get; set; }
    public virtual string username { get; set; }
    public virtual int role { get; set; }
    internal virtual string hashedPassword { get; set; }
    public virtual ICollection<int> posts { get; set; }
    public virtual ICollection<int> comments { get; set; }
    public virtual JSType.Date dateCreated { get; set; }
}