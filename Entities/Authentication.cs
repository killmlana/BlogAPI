using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;

namespace BlogAPI.Entities;

public class Authentication
{   
    //Initializing an instance of the random number generator.
    private static readonly RandomNumberGenerator Rng = System.Security.Cryptography.RandomNumberGenerator.Create();
    
    //Function for salting and hashing the password.
    public static string Hash(string password)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] salt = new byte[16];
        Rng.GetBytes(salt);
        
        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing, 
            Version = Argon2Version.Nineteen,
            TimeCost = 10,
            MemoryCost = 32768,
            Lanes = 5, // higher than "Lanes" doesn't help (or hurt)
            Password = passwordBytes,
            Salt = salt, // >= 8 bytes if not null
            HashLength = 20 // >= 4
        };
        var argon2A = new Argon2(config);
        using SecureArray<byte> hashA = argon2A.Hash();
        var hashString = config.EncodeString(hashA.Buffer);

        return hashString;
    }

    public static bool IsVerified(string password, string hashedPassword)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        if (Argon2.Verify(hashedPassword, passwordBytes, 5))
        {
            return true;
        }
        return false;
    }
}