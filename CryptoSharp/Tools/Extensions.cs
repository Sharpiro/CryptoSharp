using System;
using JetBrains.Annotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

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

        public static byte[] GetBytesFromHex(this string hexString)
        {
            hexString = hexString.Replace(" ", string.Empty);
            if (hexString.Length % 2 != 0) throw new InvalidOperationException("Hex string must be an even amount of characters in X2 format: 'FF'");
            var match = Regex.Match(hexString, "[^a-fA-F\\d\\s:]");
            if (match.Success) throw new InvalidOperationException("Hex string contains invalid characters");
            return Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                .ToArray();
        }

        public static char[] GetInsecureChars(this SecureString secureString)
        {
            unsafe
            {
                var unmanagedBytes = Marshal.SecureStringToGlobalAllocAnsi(secureString);
                char[] chars;
                try
                {
                    var byteArray = (byte*)unmanagedBytes.ToPointer();
                    var pEnd = byteArray;
                    while (*pEnd++ != 0) { }
                    var length = (int)(pEnd - byteArray - 1);

                    chars = new char[length];
                    for (var i = 0; i < length; ++i)
                    {
                        // Work with data in byte array as necessary, via pointers, here
                        var dataAtIndex = *(byteArray + i);
                        chars[i] = (char)dataAtIndex;
                    }
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocAnsi(unmanagedBytes);
                }
                return chars;
            }
        }

        public static byte[] GetBytes([NotNull]this RandomNumberGenerator randomizer, int length)
        {
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));
            if (length < 1) throw new ArgumentOutOfRangeException(nameof(length), "length must be larger than 0");
            var buffer = new byte[length];
            randomizer.GetBytes(buffer);
            return buffer;
        }
    }
}