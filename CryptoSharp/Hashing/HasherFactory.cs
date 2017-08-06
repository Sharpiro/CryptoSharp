using System;

namespace CryptoSharp.Hashing
{
    public class HasherFactory
    {
        public IHasher CreateHasher(HasherType hasherType)
        {
            switch (hasherType)
            {
                case HasherType.None: return new NotHasher();
                case HasherType.MD5: return new MDFive128BitHasher();
                case HasherType.SHA1: return new Sha160BitHasher();
                case HasherType.SHA256: return new Sha256BitHasher();
                default:
                    throw new ArgumentOutOfRangeException($"Unable to create IHasher for HasherType: '{hasherType}'");
            }
        }
    }

    public enum HasherType { None, MD5, SHA1, SHA256 }
    //public enum BytesDisplayType { Base64, Hex, Guid }
}