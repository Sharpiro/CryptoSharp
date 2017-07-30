using CryptoSharp.Hashing;
using CryptoSharp.Tools;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSharp
{
    public class AesService
    {
        private readonly IHasher _hasher;

        public AesService(IHasher hasher)
        {
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        public (byte[] Key, byte[] IV) CreateKey()
        {
            using (var aesAlg = Aes.Create()) return (aesAlg.Key, aesAlg.IV);
        }

        public (byte[] Key, byte[] IV) CreateKey(string plainTextKey)
        {
            using (var aesAlg = Aes.Create())
            {
                var plainTextKeyBytes = Encoding.UTF8.GetBytes(plainTextKey);
                var key = _hasher.CreateHash(plainTextKeyBytes);
                return (key, aesAlg.IV);
            }
        }

        //public byte[] Encrypt(string plainText, byte[] key, byte[] iv)
        //{
        //    if (plainText == null || plainText.Length <= 0) throw new ArgumentNullException("plainText");
        //    if (key == null || key.Length <= 0) throw new ArgumentNullException("Key");
        //    if (iv == null || iv.Length <= 0) throw new ArgumentNullException("IV");

        //    byte[] encrypted;
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = key;
        //        aesAlg.IV = iv;

        //        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //        using (var msEncrypt = new MemoryStream())
        //        {
        //            using (var cryptoStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (var swEncrypt = new StreamWriter(cryptoStream))
        //                {
        //                    swEncrypt.Write(plainText);
        //                }
        //                encrypted = msEncrypt.ToArray();
        //            }
        //        }
        //    }
        //    return encrypted;
        //}

        public byte[] EncryptBytes(byte[] plainBytes, byte[] key, byte[] iv)
        {
            if (plainBytes == null || plainBytes.Length <= 0) throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0) throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0) throw new ArgumentNullException("IV");

            byte[] encrypted = new byte[plainBytes.Length];
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        //public string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        //{
        //    if (cipherText == null || cipherText.Length <= 0) throw new ArgumentNullException("cipherText");
        //    if (Key == null || Key.Length <= 0) throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0) throw new ArgumentNullException("IV");

        //    string plaintext = null;

        //    using (var aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = Key;
        //        aesAlg.IV = IV;

        //        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        //        using (var msDecrypt = new MemoryStream(cipherText))
        //        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //        using (var srDecrypt = new StreamReader(csDecrypt))
        //        {
        //            plaintext = srDecrypt.ReadToEnd();
        //        }
        //    }
        //    return plaintext;
        //}

        public byte[] DecryptBytes(byte[] cipherBytes, byte[] Key, byte[] IV)
        {
            if (cipherBytes == null || cipherBytes.Length <= 0) throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0) throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException("IV");

            byte[] plainBytes = null;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(cipherBytes))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    plainBytes = csDecrypt.GetBytes();
                }
            }
            return plainBytes;
        }
    }
}