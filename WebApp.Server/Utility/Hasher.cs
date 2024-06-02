using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace WebApp.Server.Utility;

public class Hasher
{
    public static string sha256_hash(string value)
    {
        string hashed = "";
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            foreach (var b in bytes)
            {
                hashed += $"{b:X2}";
            }
        }

        return hashed;
    }
}