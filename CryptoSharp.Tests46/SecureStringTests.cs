using System;
using System.Security;
using CryptoSharp.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;

namespace CryptoSharp.Tests46
{
    [TestClass]
    public class SecureStringTests
    {
        [TestMethod]
        public void SecureStringTest()
        {
            var secureString = new SecureString();
            //var data = "aGVsbG8gd29ybGQ=";
            var data = new[] { 'h', 'e', 'l', 'l', 'o', ' ', 'f', 'r', 'i', 'e', 'n', 'd' };
            //var data = "hello friend";

            foreach (var character in data)
            {
                secureString.AppendChar(character);
            }

            var chars = secureString.GetInsecureString();

            Array.Clear(data, 0, data.Length);
            //unsafe
            //{
            //    fixed (byte* item1 = &bytes[0])
            //    fixed (char* item2 = &chars[0])
            //    {
            //        item1 = 12;
            //    }
            //}
            Assert.AreEqual("hello friend", new string(chars));
            Array.Clear(chars, 0, chars.Length);

            Assert.IsTrue(data.All(c => c == 0));
            Assert.IsTrue(chars.All(c => c == 0));
        }
    }
}