namespace CryptoSharp.Hashing
{
    public class NotHasher : IHasher
    {
        public byte[] CreateHash(byte[] plainBytes)
        {
            return plainBytes;
        }
    }
}