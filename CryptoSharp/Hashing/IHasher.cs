namespace CryptoSharp.Hashing
{
    public interface IHasher
    {
        byte[] CreateHash(byte[] plainBytes);
    }

    public interface I256BitHasher : IHasher { }

    public interface I128BitHasher : IHasher { }
}