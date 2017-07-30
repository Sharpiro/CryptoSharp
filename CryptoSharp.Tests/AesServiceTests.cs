using CryptoSharp.Hashing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace CryptoSharp.Tests
{
    [TestClass]
    public class AesServiceTests
    {
        private readonly AesService _aesService;
        private readonly IHasher _hasher = new Sha256Hasher();

        public AesServiceTests()
        {
            _aesService = new AesService(_hasher);
        }

        [TestMethod]
        public void CreateKeyTest()
        {
            (byte[] key, byte[] iv) = _aesService.CreateKey();

            Assert.IsTrue(key.Length == 32);
            Assert.IsTrue(iv.Length == 16);
        }

        [TestMethod]
        public void CreateKeyFromPlainTextTest()
        {
            const string plainTextKey = "this is my key it is special";
            const string expectedKey64 = "1OlRTTIeEFz9jVdwuL8wEDvYLRpYH0J9RU9wOvpjHF4=";
            (byte[] key, byte[] iv) = _aesService.CreateKey(plainTextKey);
            var actualKey64 = Convert.ToBase64String(key);

            Assert.IsTrue(key.Length == 32);
            Assert.IsTrue(iv.Length == 16);
            Assert.AreEqual(expectedKey64, actualKey64);
        }

        [TestMethod]
        public void EncryptDecryptTest()
        {
            const string expectedMessage = "this is my test message";
            var messageBytes = Encoding.UTF8.GetBytes(expectedMessage);
            (byte[] key, byte[] iv) = _aesService.CreateKey();

            var cryptoBytes = _aesService.Encrypt(messageBytes, key, iv);
            var actualMessageBytes = _aesService.Decrypt(cryptoBytes, key, iv);
            var actualMessage = Encoding.UTF8.GetString(actualMessageBytes);

            Assert.AreEqual(expectedMessage, actualMessage);
        }
    }
}