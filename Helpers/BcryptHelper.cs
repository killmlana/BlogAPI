using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BlogAPI.Entities;
using BlogAPI.Models;


namespace BlogAPI.Helpers;

//TODO fix memory issue with hashing passwords.

public class BcryptHelper
{   
    //Initializing an instance of the random number generator.
    private static readonly RandomNumberGenerator Rng = System.Security.Cryptography.RandomNumberGenerator.Create();
    
    //Method for salting and hashing the password.
    public static string Hash(string password)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        return hashedPassword;
    }

    public static bool IsVerified(User user, UserDTO userDto)
    {
        if (BCrypt.Net.BCrypt.Verify(userDto.password, user.HashedPassword))
        {
            return true;
        }
        return false;
    }
    
}