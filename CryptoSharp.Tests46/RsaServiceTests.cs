using System.Linq;
using CryptoSharp.Asymmetric;
using CryptoSharp.Symmetric;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

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
    }
}