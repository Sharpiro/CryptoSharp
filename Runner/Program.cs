using CryptoSharp.Asymmetric.CustomRSA;
using CryptoSharp.Symmetric;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using static System.Console;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //var testBytes = new byte[] { 1, 2, 3 };
            //(var key, var iv) = SimpleAes.CreateKey();
            //var plainBytes = key.Concat(iv).ToArray();
            //var plainHex = string.Join(string.Empty, plainBytes.Select(b => b.ToString("x2")));
            var plainHex = "fb86b909a356da4b639ad0a767d0595d3119440a1eb9e2c504ee102c57af497f5326bf6a7d08bdbb17019d6407c77be9";
            var plainBytes = GetBytesFromHex(plainHex);
            //Debug.Assert(plainBytes.Length == 32 + 16);
            //var cryptoBytes = SimpleAes.Encrypt(testBytes, key, iv);
            //var decryptedBytes = SimpleAes.Decrypt(cryptoBytes, key, iv);
            //Debug.Assert(testBytes.SequenceEqual(decryptedBytes));

            var e = new BigInteger(7);
            var n = BigInteger.Parse("522806553487135022947979710062295163573133494999739899086216549037195002661539576520853599318297663743426372773771723005330074567544450324957918152900097");
            var d = BigInteger.Parse("373433252490810730677128364330210831123666782142671356490154677883710716186756589892531398008495512038063359957925071200972864532192059518154993437666743");
            var rsaService = CustomRSAService.Create(e, n, d);
            //var rsaService = CustomRSAService.Create(32);
            //var plainBytes = GetBytesFromHex("dd3e0fc7784d45f99e789bd94058fc3add3e0fc7784d45f99e789bd94058fc3a");
            var cryptoBytes = rsaService.Encrypt(plainBytes);
            //var cryptoBase64 = Convert.ToBase64String(cryptoBytes);
            var cryptoBase64 = "sthvYiX3MDIjr9SEXPOn1IHmpay7QtYsLEjWwQrYUGNzUj6LRTrkoQQ1Ct/B587nVsNP7O/lWKzeWoCn4mPL+A==";
            //var decryptedBytes = rsaService.Decrypt(cryptoBytes);
            //var plainText = string.Join(string.Empty, decryptedBytes.Select(b => b.ToString("x2")).ToArray());
            //WriteLine(plainText);
            //Debug.Assert(plainBytes.SequenceEqual(decryptedBytes));

            WriteLine("done");
            ReadKey();
        }

        public static byte[] GetBytesFromHex(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (int i = 0, j = 0; i <= hexString.Length; i++)
            {
                if (i == 0 || i % 2 != 0) continue;
                var stringByte = $"{hexString[i - 2]}{hexString[i - 1]}";
                bytes[j++] = Convert.ToByte(stringByte, 16);
            }
            return bytes;
        }
    }
}