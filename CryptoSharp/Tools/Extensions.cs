using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

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

        public static char[] GetInsecureString(this SecureString secureString)
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
    }
}