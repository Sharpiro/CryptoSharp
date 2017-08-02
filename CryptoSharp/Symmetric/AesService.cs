using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CryptoSharp.Hashing;
using CryptoSharp.Tools;

namespace CryptoSharp.Symmetric
{
    public class AesService
    {
        private readonly I256BitHasher _keyHasher;
        private readonly I128BitHasher _ivHasher;

        public AesService(I256BitHasher keyHasher, I128BitHasher ivHasher)
        {
            _keyHasher = keyHasher ?? throw new ArgumentNullException(nameof(keyHasher));
            _ivHasher = ivHasher ?? throw new ArgumentNullException(nameof(keyHasher));
        }

        public AesService()
        {
            _keyHasher = new Sha256Hasher();
            _ivHasher = new MDFive128BitHasher();
        }

        public (byte[] Key, byte[] IV) CreateKey()
        {
            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null) throw new CryptographicException("Could not create AES algorithm");

                return (aesAlg.Key, aesAlg.IV);
            }
        }

        public (byte[] Key, byte[] IV) CreateKey(string plainTextKey)
        {
            if (string.IsNullOrEmpty(plainTextKey)) throw new ArgumentNullException(nameof(plainTextKey));

            var plainTextKeyBytes = Encoding.UTF8.GetBytes(plainTextKey);
            var key = _keyHasher.CreateHash(plainTextKeyBytes);
            var iv = _ivHasher.CreateHash(plainTextKeyBytes);
            return (key, iv);
        }

        public byte[] Encrypt(byte[] plainBytes, byte[] key, byte[] iv)
        {
            if (plainBytes == null || plainBytes.Length <= 0) throw new ArgumentNullException(nameof(plainBytes));
            if (key == null || key.Length <= 0) throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0) throw new ArgumentNullException(nameof(iv));

            using (var aesAlg = Aes.Create())
            {
                if (aesAlg == null) throw new CryptographicException("Could not create AES algorithm");

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
                if (aesAlg == null) throw new CryptographicException("Could not create AES algorithm");

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