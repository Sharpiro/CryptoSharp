using System.Security.Cryptography;

namespace CryptoSharp.Hashing
{
    public class Sha512BitHasher : I512BitHasher
    {
        public byte[] CreateHash(byte[] plainBytes)
        {
            using (var hasher = SHA512.Create())
            {
                return hasher.ComputeHash(plainBytes);
            }
        }
    }
}