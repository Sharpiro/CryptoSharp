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
            if (string.IsNullOrEmpty(plainTextKey)) throw new ArgumentNullException(nameof(plainTextKey));

            using (var aesAlg = Aes.Create())
            {
                var plainTextKeyBytes = Encoding.UTF8.GetBytes(plainTextKey);
                var key = _hasher.CreateHash(plainTextKeyBytes);
                return (key, aesAlg.IV);
            }
        }

        public byte[] Encrypt(byte[] plainBytes, byte[] key, byte[] iv)
        {
            if (plainBytes == null || plainBytes.Length <= 0) throw new ArgumentNullException(nameof(plainBytes));
            if (key == null || key.Length <= 0) throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0) throw new ArgumentNullException(nameof(iv));

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
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        public byte[] Decrypt(byte[] cipherBytes, byte[] key, byte[] iv)
        {
            if (cipherBytes == null || cipherBytes.Length <= 0) throw new ArgumentNullException(nameof(cipherBytes));
            if (key == null || key.Length <= 0) throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0) throw new ArgumentNullException(nameof(iv));

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(cipherBytes))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    return csDecrypt.GetBytes();
                }
            }
        }
    }
}