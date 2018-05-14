using System;
using System.IO;
using System.Security.Cryptography;

namespace CryptoSharp.Symmetric
{
    public static class SimpleAes
    {
        public static (byte[] Key, byte[] IV) CreateKey()
        {
            using (var aesAlg = Aes.Create())
            {
                return (aesAlg.Key, aesAlg.IV);
            }
        }

        public static byte[] Encrypt(byte[] plainBytes, byte[] key, byte[] iv)
        {
            if (plainBytes == null || plainBytes.Length <= 0) throw new ArgumentNullException(nameof(plainBytes));
            if (key == null || key.Length <= 0) throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0) throw new ArgumentNullException(nameof(iv));

            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null) throw new CryptographicException("Could not create AES algorithm");

                aesAlg.Key = key;
                aesAlg.IV = iv;
                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        public static byte[] Decrypt(byte[] cryptoBytes, byte[] key, byte[] iv)
        {
            if (cryptoBytes == null || cryptoBytes.Length <= 0) throw new ArgumentNullException(nameof(cryptoBytes));
            if (key == null || key.Length <= 0) throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0) throw new ArgumentNullException(nameof(iv));

            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null) throw new CryptographicException("Could not create AES algorithm");

                aesAlg.Key = key;
                aesAlg.IV = iv;
                using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                using (var msDecrypt = new MemoryStream(cryptoBytes))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var plainStream = new MemoryStream())
                {
                    csDecrypt.CopyTo(plainStream);
                    return plainStream.ToArray();
                }
            }
        }
    }
}