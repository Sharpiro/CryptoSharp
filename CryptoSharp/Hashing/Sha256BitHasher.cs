using System.Security.Cryptography;

namespace CryptoSharp.Hashing
{
    public class Sha256BitHasher : I256BitHasher
    {
        public byte[] CreateHash(byte[] plainBytes)
        {
            using (var hasher = SHA256.Create())
            {
                return hasher.ComputeHash(plainBytes);
            }
        }
    }
}