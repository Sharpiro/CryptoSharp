using System;
using System.IO;
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

namespace CryptoSharp
{
    public class CertificateService
    {
        public X509Certificate2 CreateCert(string subject, string password, int strength)
        {
            const string signatureAlgorithm = "SHA256WithRSA";

            //get bouncy castle cert
            var random = new SecureRandom(new CryptoApiRandomGenerator());
            var certificateGenerator = new X509V3CertificateGenerator();
            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);
            certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);
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
                store.Save(stream, password.ToCharArray(), random);
                var convertedCertificate =
                    new X509Certificate2(
                        stream.ToArray(), password,
                        X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                return convertedCertificate;
            }
        }
    }
}