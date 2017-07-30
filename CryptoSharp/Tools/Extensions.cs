using System.IO;

namespace CryptoSharp.Tools
{
    public static class Extensions
    {
        public static byte[] GetBytes(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}