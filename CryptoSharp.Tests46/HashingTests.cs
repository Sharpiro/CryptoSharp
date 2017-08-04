using System;
using System.Text;
using CryptoSharp.Hashing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoSharp.Tests46
{
    [TestClass]
    public class HashingTests
    {
        private readonly IHasher _hasher= new Sha256BitHasher();

        [TestMethod]
        public void Sha256CreateHashTest()
        {
            const string plainTextKey = "this is my key it is special";
            const string expectedHashedKey64 = "1OlRTTIeEFz9jVdwuL8wEDvYLRpYH0J9RU9wOvpjHF4=";
            var plainTextKeyBytes = Encoding.UTF8.GetBytes(plainTextKey);
            var hash = _hasher.CreateHash(plainTextKeyBytes);

            var actualHashedKey64 = Convert.ToBase64String(hash);

            Assert.AreEqual(expectedHashedKey64, actualHashedKey64);
        }
    }
}