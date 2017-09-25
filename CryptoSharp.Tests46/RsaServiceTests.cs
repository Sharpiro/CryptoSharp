using System.Linq;
using System.Security.Cryptography;
using CryptoSharp.Asymmetric;
using CryptoSharp.Symmetric;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using CryptoSharp.Hashing;

namespace CryptoSharp.Tests46
{
    [TestClass]
    public class RsaServiceTests
    {
        [TestMethod]
        public void KeyDeliveryTest()
        {
            //const string plainText = "this is the test plain data";
            var aesService = new AesService();
            (byte[] aesKey, byte[] aesIv) = aesService.CreateKey();
            var combinedBytes = aesKey.Concat(aesIv).ToArray();
            var client = RsaService.Create();
            var server = RsaService.Create();

            //send client aes encryption data from client to server
            var cryptoBytes = client.Encrypt(combinedBytes, server.PublicKey);
            var receivedClientAesData = server.Decrypt(cryptoBytes);
            var receivedClientAesKey = receivedClientAesData.Take(32).ToArray();
            var receivedClientAesIv = receivedClientAesData.Reverse().Take(16).Reverse().ToArray();
            Assert.IsTrue(aesKey.SequenceEqual(receivedClientAesKey));
            Assert.IsTrue(aesIv.SequenceEqual(receivedClientAesIv));

            //send client aes encrypted message using received key and iv
            const string responseToClientMessage = "I got your key thanks";
            var responseToClientBytes = Encoding.UTF8.GetBytes(responseToClientMessage);
            var responseToClientBytesCryptoBytes = aesService.Encrypt(responseToClientBytes, receivedClientAesKey, receivedClientAesIv);

            //client decrypts server's response message
            var messageToClientBytes = aesService.Decrypt(responseToClientBytesCryptoBytes, aesKey, aesIv);
            var messageToClientText = Encoding.UTF8.GetString(messageToClientBytes);
            Assert.IsTrue(responseToClientBytes.SequenceEqual(messageToClientBytes));
            Assert.AreEqual(responseToClientMessage, messageToClientText);
        }

        [TestMethod]
        public void DigitalSignatureTest()
        {
            var aesService = new AesService();
            (byte[] aesKey, byte[] aesIv) = aesService.CreateKey();

            const string messageData = "this is a test message";

            var messageBytes = Encoding.UTF8.GetBytes(messageData);
            var sender = RsaService.Create();
            var receiver = RsaService.Create();
            var hasher = new Sha256BitHasher();


            var hashedMessageBytes = hasher.CreateHash(messageBytes);

            var digitalSignature = sender.EncryptPrivate(hashedMessageBytes);
            var cryptoBytes = aesService.Encrypt(messageBytes, aesKey, aesIv);

            var hashMessageBytesFromSignature = receiver.DecryptPublic(digitalSignature, sender.PublicKey);
            var plainMessageBytes = aesService.Decrypt(cryptoBytes, aesKey, aesIv);
            var hashedPlainMessageBytes = hasher.CreateHash(plainMessageBytes);

            Assert.IsTrue(hashMessageBytesFromSignature.SequenceEqual(hashedPlainMessageBytes));
        }

        [TestMethod]
        //http://blog.differentpla.net/blog/2013/03/18/using-bouncy-castle-from-net
        public void CreateCertTest()
        {
            const string subject = "test";
            const string password = "password";
            const int strength = 1024;

            using (var certificate = new CertificateService().CreateCert(subject, password, strength))
            {
                var publicKeyProvider = (RSACryptoServiceProvider)certificate.PublicKey.Key;
                var privateKeyProvider = (RSACryptoServiceProvider)certificate.PrivateKey;

                var rsaService = RsaService.Create(certificate);

                Assert.AreEqual(publicKeyProvider.ToXmlString(false), privateKeyProvider.ToXmlString(false));
                Assert.AreEqual(rsaService.PrivateKey, privateKeyProvider.ToXmlString(true));
                Assert.AreEqual(rsaService.PublicKey, privateKeyProvider.ToXmlString(false));
            }
        }
    }
}