#if NET46
using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CryptoSharp.Asymmetric
{
    public class RsaService
    {
        private readonly int _keySize = 1024;
        public string PrivateKey { get; }
        public string PublicKey { get; }

        private RsaService(string privateKey, string publicKey, int keySize)
        {
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
            _keySize = keySize;
        }

        public int GetMaxBytes(int keySize) => (keySize - 384) / 8 + 37;

        public byte[] EncryptPrivate(byte[] plainBytes)
        {
            var rsaProvider = new RSACryptoServiceProvider(_keySize);
            rsaProvider.FromXmlString(PrivateKey);


            RSAParameters rsaParams = rsaProvider.ExportParameters(true);
            BigInteger D = GetBig(rsaParams.D);
            BigInteger Modulus = GetBig(rsaParams.Modulus);
            BigInteger numData = GetBig(AddPadding(plainBytes));
            BigInteger encData = BigInteger.ModPow(numData, D, Modulus);

            return encData.ToByteArray();
        }

        public byte[] DecryptPublic(byte[] cryptoBytes, string senderPublickey)
        {
            var rsaProvider = new RSACryptoServiceProvider(_keySize);
            rsaProvider.FromXmlString(senderPublickey);

            BigInteger numEncData = new BigInteger(cryptoBytes);

            RSAParameters rsaParams = rsaProvider.ExportParameters(false);
            BigInteger Exponent = GetBig(rsaParams.Exponent);
            BigInteger Modulus = GetBig(rsaParams.Modulus);

            BigInteger decData = BigInteger.ModPow(numEncData, Exponent, Modulus);

            byte[] data = decData.ToByteArray();
            byte[] result = new byte[data.Length - 1];
            Array.Copy(data, result, result.Length);
            result = RemovePadding(result);

            Array.Reverse(result);
            return result;
        }

        public byte[] Encrypt(byte[] plainBytes, string receiverPublicKey)
        {
            var rsaProvider = new RSACryptoServiceProvider(_keySize);
            rsaProvider.FromXmlString(receiverPublicKey);

            var cryptoBytes = rsaProvider.Encrypt(plainBytes, true);

            return cryptoBytes;
        }

        public byte[] Decrypt(byte[] cryptoBytes)
        {
            var rsaProvider = new RSACryptoServiceProvider(_keySize);
            rsaProvider.FromXmlString(PrivateKey);

            var plainBytes = rsaProvider.Decrypt(cryptoBytes, true);

            return plainBytes;
        }

        public static RsaService Create(int keySize = 1024, string privateKey = null)
        {
            var csp = new RSACryptoServiceProvider(keySize);
            if (string.IsNullOrEmpty(privateKey))
            {
                privateKey = csp.ToXmlString(true);
            }
            else
            {
                csp.FromXmlString(privateKey);
            }
            var publicKey = csp.ToXmlString(false);
            return new RsaService(privateKey, publicKey, keySize);
        }

        public static RsaService Create(X509Certificate2 cert)
        {
            if (!cert.HasPrivateKey) throw new InvalidOperationException("cert must have private key as well as public key");

            var rsa = (RSACryptoServiceProvider)cert.PrivateKey;
            var privateKey = rsa.ToXmlString(true);
            var publicKey = rsa.ToXmlString(false);
            return new RsaService(privateKey, publicKey, rsa.KeySize);
        }

        private static BigInteger GetBig(byte[] data)
        {
            var inArr = (byte[])data.Clone();
            Array.Reverse(inArr);  // Reverse the byte order
            var final = new byte[inArr.Length + 1];  // Add an empty byte at the end, to simulate unsigned BigInteger (no negatives!)
            Array.Copy(inArr, final, inArr.Length);

            return new BigInteger(final);
        }

        // Add 4 byte random padding, first bit *Always On*
        private static byte[] AddPadding(byte[] data)
        {
            var rnd = new Random();
            var paddings = new byte[4];
            rnd.NextBytes(paddings);
            paddings[0] = (byte)(paddings[0] | 128);

            var results = new byte[data.Length + 4];

            Array.Copy(paddings, results, 4);
            Array.Copy(data, 0, results, 4, data.Length);
            return results;
        }

        private static byte[] RemovePadding(byte[] data)
        {
            var results = new byte[data.Length - 4];
            Array.Copy(data, results, results.Length);
            return results;
        }
    }
}
#endif