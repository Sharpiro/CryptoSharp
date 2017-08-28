namespace CryptoSharp.Hashing
{
    public interface IHasher
    {
        byte[] CreateHash(byte[] plainBytes);
    }

    public interface IAtLeast128BitHasher : IHasher { }

    public interface IAtLeast256BitHasher : IAtLeast128BitHasher { }

    public interface I128BitHasher : IAtLeast128BitHasher { }

    public interface I160BitHasher : IAtLeast128BitHasher { }

    public interface I256BitHasher : IAtLeast256BitHasher { }

    public interface I512BitHasher : I256BitHasher { }
}