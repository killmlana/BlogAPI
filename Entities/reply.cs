namespace BlogAPI.Entities;

public class reply
{
    public virtual int uid { get; set; }
    public virtual int owner { get; set; }
    public virtual string content { get; set; }
    
}