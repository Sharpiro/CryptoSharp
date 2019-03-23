using System.Security.Cryptography;

namespace CryptoSharp.Hashing
{
    public class Sha160BitHasher : I160BitHasher
    {
        public byte[] CreateHash(byte[] plainBytes)
        {
            using (var hasher = SHA1.Create())
            {
                return hasher.ComputeHash(plainBytes);
            }
        }
    }
}