using System;
using System.Text;
using CryptoSharp.Models;
using CryptoSharp.Tools;

namespace CryptoSharp.Services
{
    public class TextFormatService
    {
        public TextFormat GetFormat(string data, out byte[] formattedData)
        {
            try
            {
                formattedData = data.GetBytesFromHex();
                return TextFormat.Hex;
            }
            catch (Exception) {/*ignored*/}

            try
            {
                formattedData = Convert.FromBase64String(data);
                return TextFormat.Base64;
            }
            catch (Exception) {/*ignored*/}

            formattedData = Encoding.UTF8.GetBytes(data);
            return TextFormat.PlainText;
        }

        public string Format(byte[] data, TextFormat textFormat)
        {
            string output;
            switch (textFormat)
            {
                case TextFormat.PlainText:
                    output = Encoding.UTF8.GetString(data);
                    break;
                case TextFormat.Hex:
                    output = data.GetHexFromBytes(useSpaces: true);
                    break;
                case TextFormat.Auto:
                case TextFormat.Base64:
                    output = Convert.ToBase64String(data);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
            return output;
        }

        public byte[] Format(string data, TextFormat textFormat)
        {
            byte[] output;
            switch (textFormat)
            {
                case TextFormat.Auto:
                    GetFormat(data, out byte[] formattedData);
                    output = formattedData;
                    break;
                case TextFormat.PlainText:
                    output = Encoding.UTF8.GetBytes(data);
                    break;
                case TextFormat.Hex:
                    output = data.GetBytesFromHex();
                    break;
                case TextFormat.Base64:
                    output = Convert.FromBase64String(data);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
            return output;
        }
    }
}