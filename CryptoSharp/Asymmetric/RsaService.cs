#if NET46
using System;
using System.Security.Cryptography;

namespace CryptoSharp.Asymmetric
{
    public class RsaService
    {
        //private readonly string _privateKey;
        //private readonly string _publicKey;
        private const int KeySize = 1024;
        public string PrivateKey { get; }
        public string PublicKey { get; }

        public RsaService(string privateKey, string publicKey)
        {
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
        }

        public int GetMaxBytes(int keySize) => (keySize - 384) / 8 + 37;

        //public string CreatePublicKey()
        //{
        //    var csp = new RSACryptoServiceProvider(2048);
        //    var pubKey = csp.ToXmlString(false);
        //    return pubKey
        //}

        //public static string CreatePrivateKey()
        //{
        //    var csp = new RSACryptoServiceProvider(2048);
        //    var privKey = csp.ToXmlString(true);
        //    return privKey;
        //}

        //public string CreateKeys()
        //{
        //    var csp = new RSACryptoServiceProvider(2048);
        //    var privKey = csp.ToXmlString(true);
        //    return privKey;
        //}

        public byte[] Encrypt(byte[] plainBytes, string targetPublicKey)
        {
            var rsaProvider = new RSACryptoServiceProvider(KeySize);
            rsaProvider.FromXmlString(targetPublicKey);

            var cryptoBytes = rsaProvider.Encrypt(plainBytes, true);

            return cryptoBytes;
        }

        public byte[] Decrypt(byte[] cryptoBytes)
        {
            var rsaProvider = new RSACryptoServiceProvider(KeySize);
            rsaProvider.FromXmlString(PrivateKey);

            var plainBytes = rsaProvider.Decrypt(cryptoBytes, true);

            return plainBytes;

        }

        public static RsaService Create(string privateKey = null)
        {
            var csp = new RSACryptoServiceProvider(KeySize);
            if (string.IsNullOrEmpty(privateKey))
            {
                privateKey = csp.ToXmlString(true);
            }
            else
            {
                csp.FromXmlString(privateKey);
            }
            var publicKey = csp.ToXmlString(false);
            return new RsaService(privateKey, publicKey);
        }
    }
}
#endif