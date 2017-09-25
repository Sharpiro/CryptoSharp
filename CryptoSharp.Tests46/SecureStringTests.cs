using System;
using System.Security;
using CryptoSharp.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoSharp.Tests46
{
    [TestClass]
    public class SecureStringTests
    {
        [TestMethod]
        public void SecureStringTest()
        {
            var secureString = new SecureString();
            var data = new[] { 'h', 'i' };

            foreach (var character in data)
            {
                secureString.AppendChar(character);
            }

            var bytes = secureString.GetInsecureString();
            Array.Clear(bytes, 0, bytes.Length);
        }
    }
}