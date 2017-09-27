using System;
using System.Security;
using CryptoSharp.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CryptoSharp.Tests46
{
    [TestClass]
    public class SecureStringTests
    {
        [TestMethod]
        public void SecureStringTest()
        {
            var secureString = new SecureString();
            var data = new[] { 'h', 'e', 'l', 'l', 'o', ' ', 'f', 'r', 'i', 'e', 'n', 'd' };

            foreach (var character in data)
            {
                secureString.AppendChar(character);
            }

            var chars = secureString.GetInsecureChars();

            //unsafe
            //{
            //    fixed (byte* item1 = &bytes[0])
            //    fixed (char* item2 = &chars[0])
            //    {
            //        item1 = 12;
            //    }
            //}

            Assert.IsTrue(data.SequenceEqual(chars));

            Array.Clear(chars, 0, chars.Length);
            Array.Clear(data, 0, data.Length);

            Assert.IsTrue(data.All(c => c == 0));
            Assert.IsTrue(chars.All(c => c == 0));
        }
    }
}