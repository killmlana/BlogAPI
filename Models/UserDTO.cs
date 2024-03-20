using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models;

public class UserDTO
{
    [Required, 
     StringLength(16,MinimumLength = 2), 
     RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters are allowed.")]
    public virtual string username { get; set; }
    [Required, MinLength(8)]
    public virtual string password { get; set; }
}