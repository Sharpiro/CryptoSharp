using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using CryptoSharp.Tools;

namespace CryptoSharp
{
    public class CertificateService
    {
        private const string SignatureAlgorithm = "SHA256WithRSA";
        private readonly RandomNumberGenerator _randomizer = RandomNumberGenerator.Create();

        private int _randomPasswordSize = 8;
        //private readonly Random _randomizer = new Random();

        public int RandomPasswordSize
        {
            get => _randomPasswordSize;
            set => _randomPasswordSize = value < 6 ? 6 : value;
        }

        public X509Certificate2 CreateCert(string subject, string password = null, int strength = 1024)
        {
            //get bouncy castle cert
            var random = new SecureRandom(new CryptoApiRandomGenerator());
            var certificateGenerator = new X509V3CertificateGenerator();
            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);
            certificateGenerator.SetSignatureAlgorithm(SignatureAlgorithm);
            certificateGenerator.SetIssuerDN(new X509Name($"CN={subject}"));
            certificateGenerator.SetSubjectDN(new X509Name($"CN={subject}"));
            certificateGenerator.SetNotBefore(DateTime.UtcNow.Date);
            certificateGenerator.SetNotAfter(DateTime.UtcNow.Date.AddYears(2));
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(new KeyGenerationParameters(random, strength));
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);
            var issuerKeyPair = subjectKeyPair;
            var certificate = certificateGenerator.Generate(issuerKeyPair.Private, random);

            //get win cert
            var store = new Pkcs12Store();
            var friendlyName = certificate.SubjectDN.ToString();
            var certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { certificateEntry });
            using (var stream = new MemoryStream())
            {
                //password = !string.IsNullOrEmpty(password) ? password : string.Join(string.Empty, Enumerable.Repeat(0, 8).Select(i => ((byte)_randomizer.Next(0, (int)Math.Pow(2, 8))).ToString("X2")));
                password = !string.IsNullOrEmpty(password) ? password : _randomizer.GetBytes(RandomPasswordSize).Select(b => b.ToString("X2")).StringJoin();
                store.Save(stream, password.ToCharArray(), random);
                return new X509Certificate2(stream.ToArray(), password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            }
        }
    }
}