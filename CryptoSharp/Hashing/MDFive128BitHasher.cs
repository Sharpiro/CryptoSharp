using System.Security.Cryptography;

namespace CryptoSharp.Hashing
{
    public class MDFive128BitHasher : I128BitHasher, IAtLeast128BitHasher
    {
        public byte[] CreateHash(byte[] plainBytes)
        {
            using (var hasher = MD5.Create())
            {
                return hasher.ComputeHash(plainBytes);
            }
        }
    }
}