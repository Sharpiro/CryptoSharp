using System;
using System.Numerics;
using System.Security.Cryptography;

namespace CryptoSharp.Asymmetric.CustomRSA
{
    public class CustomRSAService
    {
        public int KeySizeBits { get; private set; }
        public BigInteger EStart { get; set; } = 3;
        public BigInteger E { get; private set; }
        public BigInteger P { get; private set; }
        public BigInteger Q { get; private set; }
        public BigInteger N { get; private set; }
        public BigInteger M { get; private set; }
        public BigInteger D { get; private set; }

        private CustomRSAService() { }

        public static CustomRSAService Create(int keySizeBits)
        {
            var rsaService = new CustomRSAService();
            rsaService.KeySizeBits = keySizeBits;
            rsaService.Initialize();
            return rsaService;
        }

        public static CustomRSAService Create(BigInteger e, BigInteger n, BigInteger d = default(BigInteger))
        {
            var rsaService = new CustomRSAService
            {
                E = e,
                N = n,
                D = d
            };
            return rsaService;
        }

        private static BigInteger GetRandomPrime(int size, BigInteger e)
        {
            var number = new BigInteger();
            var randomizer = new RNGCryptoServiceProvider();
            var buffer = new byte[size + 1];
            while (!number.IsProbablePrime(100))
            {
                randomizer.GetBytes(buffer);
                buffer[0] = 1;
                buffer[size] = 0;
                number = new BigInteger(buffer);
            }
            return number;
        }

        private void Initialize()
        {
            if (P == 0)
            {
                P = GetRandomPrime(KeySizeBits, EStart);
            }
            if (Q == 0)
            {
                Q = GetRandomPrime(KeySizeBits, EStart);
            }
            N = P * Q;
            E = EStart;
            M = (P - 1) * (Q - 1);
            while (GCD(E, M) > 1)
            {
                E += 2;
            }
            D = ModInverse(E, M);
        }

        public byte[] Encrypt(byte[] plainBytes)
        {
            var plainInteger = new BigInteger(plainBytes);
            var encrypted = BigInteger.ModPow(plainInteger, E, N);
            var cryptoBytes = encrypted.ToByteArray();
            return cryptoBytes;
        }

        public byte[] Decrypt(byte[] cryptoBytes)
        {
            if (D <= 0) throw new Exception("No private key provided, D must be greater than 0");

            var cryptoInteger = new BigInteger(cryptoBytes);
            var plain = BigInteger.ModPow(cryptoInteger, D, N);
            var plainBytes = plain.ToByteArray();
            return plainBytes;
        }

        private static BigInteger GCD(BigInteger a, BigInteger b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }
            return a == 0 ? b : a;
        }

        private static BigInteger ModInverse(BigInteger a, BigInteger n)
        {
            BigInteger t = new BigInteger(0), newt = new BigInteger(1);
            BigInteger r = n, newr = a;
            while (newr != 0)
            {
                var quotient = r / newr;
                var tempT = t;
                t = newt;
                newt = tempT - quotient * newt;
                var tempR = r;
                r = newr;
                newr = tempR - quotient * newr;
            }
            if (r > 1)
            {
                return -1;
            }
            if (t < 0)
            {
                t = t + n;
            }
            return t;
        }

        private static BigInteger GetRandomNumberLessThan(BigInteger x, int maxBytes, Random randomizer)
        {
            var number = x;
            var buffer = new byte[maxBytes + 1];
            while (number >= x || number == 0)
            {
                randomizer.NextBytes(buffer);
                buffer[maxBytes] = 0;
                number = new BigInteger(buffer);
            }
            return number;
        }
    }
}
