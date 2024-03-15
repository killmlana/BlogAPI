using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BlogAPI.Contracts;
using BlogAPI.Entities;
using BlogAPI.Models;


namespace BlogAPI.Helpers;

//TODO fix memory issue with hashing passwords.

public class BcryptHelper : IBcryptHelper
{   
    
    //Method for salting and hashing the password.
    public string Hash(string password)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        return hashedPassword;
    }

    public bool IsVerified(User user, string password)
    {
        if (BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
        {
            return true;
        }
        return false;
    }
    
}