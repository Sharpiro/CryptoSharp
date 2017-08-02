using System.Security.Cryptography;

namespace CryptoSharp.Hashing
{
    public class Sha256Hasher : I256BitHasher
    {
        public byte[] CreateHash(byte[] plainBytes) => SHA256.Create().ComputeHash(plainBytes);
    }
}