namespace CryptoSharp.Hashing
{
    public interface IHasher
    {
        byte[] CreateHash(byte[] plainBytes);
    }
}