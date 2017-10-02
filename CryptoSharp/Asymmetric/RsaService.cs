using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CryptoSharp.Asymmetric
{
    public class RsaService
    {
        private readonly int _keySize;
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
            using (var rsaProvider = new RSACryptoServiceProvider(_keySize))
            {
                rsaProvider.FromXmlString(PrivateKey);

                var rsaParams = rsaProvider.ExportParameters(true);
                var d = GetBig(rsaParams.D);
                var modulus = GetBig(rsaParams.Modulus);
                var numData = GetBig(AddPadding(plainBytes));
                var encData = BigInteger.ModPow(numData, d, modulus);

                return encData.ToByteArray();
            }
        }

        public byte[] DecryptPublic(byte[] cryptoBytes, string senderPublickey)
        {
            using (var rsaProvider = new RSACryptoServiceProvider(_keySize))
            {
                rsaProvider.FromXmlString(senderPublickey);

                var numEncData = new BigInteger(cryptoBytes);

                var rsaParams = rsaProvider.ExportParameters(false);
                var exponent = GetBig(rsaParams.Exponent);
                var modulus = GetBig(rsaParams.Modulus);

                var decData = BigInteger.ModPow(numEncData, exponent, modulus);

                var data = decData.ToByteArray();
                var result = new byte[data.Length - 1];
                Array.Copy(data, result, result.Length);
                result = RemovePadding(result);

                Array.Reverse(result);
                return result;
            }
        }

        public byte[] Encrypt(byte[] plainBytes, string receiverPublicKey)
        {
            using (var rsaProvider = new RSACryptoServiceProvider(_keySize))
            {
                rsaProvider.FromXmlString(receiverPublicKey);
                var cryptoBytes = rsaProvider.Encrypt(plainBytes, true);
                return cryptoBytes;
            }
        }

        public byte[] Decrypt(byte[] cryptoBytes)
        {
            using (var rsaProvider = new RSACryptoServiceProvider(_keySize))
            {
                rsaProvider.FromXmlString(PrivateKey);
                var plainBytes = rsaProvider.Decrypt(cryptoBytes, true);
                return plainBytes;
            }
        }

        public static RsaService Create(int keySize = 1024, string privateKey = null)
        {
            using (var csp = new RSACryptoServiceProvider(keySize))
            {
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
