using System.Security.Cryptography;

namespace CryptoSharp.Hashing
{
    public class MDFive128BitHasher : I128BitHasher, IAtLeast128BitHasher
    {
        public byte[] CreateHash(byte[] plainBytes) => MD5.Create().ComputeHash(plainBytes);
    }
}