using System.Security.Cryptography;

namespace CryptoSharp.Hashing
{
    public class Sha512BitHasher : I512BitHasher
    {
        public byte[] CreateHash(byte[] plainBytes) => SHA512.Create().ComputeHash(plainBytes);
    }
}