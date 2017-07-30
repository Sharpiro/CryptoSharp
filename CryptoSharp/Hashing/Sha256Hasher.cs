using System.Security.Cryptography;

namespace CryptoSharp.Hashing
{
    public class Sha256Hasher : IHasher
    {
        public byte[] CreateHash(byte[] plainBytes)
        {
            return SHA256.Create().ComputeHash(plainBytes);
        }
    }
}