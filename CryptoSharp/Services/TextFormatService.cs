using System;
using System.Linq;
using System.Text;
using CryptoSharp.Models;
using CryptoSharp.Tools;

// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
namespace CryptoSharp.Services
{
    public class TextFormatService
    {
        public TextFormat GetFormat(string data)
        {
            try
            {
                data.GetBytesFromHex();
                return TextFormat.Hex;
            }
            catch (Exception) {/*ignored*/}

            try
            {
                Convert.FromBase64String(data);
                return TextFormat.Base64;
            }
            catch (Exception) {/*ignored*/}
            return TextFormat.PlainText;
        }

        public string Format(byte[] bytes, TextFormat textFormat)
        {
            string output;
            switch (textFormat)
            {
                case TextFormat.Auto:
                case TextFormat.PlainText:
                    output = Encoding.UTF8.GetString(bytes);
                    break;
                case TextFormat.Hex:
                    output = bytes.Select(b => b.ToString("X2")).StringJoin(" ");
                    break;
                case TextFormat.Base64:
                    output = Convert.ToBase64String(bytes);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
            return output;
        }

        public byte[] Format(string data, TextFormat textFormat)
        {
            byte[] inputBytes;
            switch (textFormat)
            {
                case TextFormat.Auto:
                case TextFormat.PlainText:
                    inputBytes = Encoding.UTF8.GetBytes(data);
                    break;
                case TextFormat.Hex:
                    inputBytes = data.GetBytesFromHex();
                    break;
                case TextFormat.Base64:
                    inputBytes = Convert.FromBase64String(data);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
            return inputBytes;
        }
    }
}