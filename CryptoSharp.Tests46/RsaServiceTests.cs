using System.Linq;
using CryptoSharp.Asymmetric;
using CryptoSharp.Symmetric;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoSharp.Tests46
{
    [TestClass]
    public class RsaServiceTests
    {
        [TestMethod]
        public void TestTest()
        {
            //const string plainText = "this is the test plain data";
            var aesService = new AesService();
            (byte[] aesKey, byte[] aesIv) = aesService.CreateKey();
            var client = RsaService.Create();
            var server = RsaService.Create();

            //send client public key from client to server
            //var clientPublicKeyBytes = Encoding.UTF8.GetBytes(client.PublicKey);
            var cryptoBytes = client.Encrypt(aesKey, server.PublicKey);
            var actualPlainBytes = server.Decrypt(cryptoBytes);
            //var actualPlainText = Encoding.UTF8.GetString(actualPlainBytes);
            Assert.IsTrue(aesKey.SequenceEqual(actualPlainBytes));

            //var serverResponse = server.Encrypt(actualPlainText, actualPlainBytes)
            //server.Encrypt()


            //Assert.AreEqual(plainText, actualPlainText);


        }
    }
}