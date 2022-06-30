namespace Fanex.NAP.DCS.FP.RawTransaction.Service
{
    using System;
    using System.Collections.Generic;

    public static class BrowserlessDetection
    {
        private const string HexDigit = "0123456789abcdef";

        private const string EncryptKey = "&f％p#s@m?b,c:*!~^._";

        public const double TimeOutToken = 3600; ////86400;

        /// <summary>
        /// Encrypt a non-empty encrypted string.
        /// </summary>
        /// <param name="plaintext">Text need to encrypt.</param>
        /// <returns>Return encrypted text.</returns>
        public static string Encrypt(string plaintext)
        {
            var encryptedText = RC4(EncryptKey, plaintext);
            return EncodeToHex(encryptedText);
        }

        /// <summary>
        /// Decrypt a non-empty encrypted string.
        /// </summary>
        /// <param name="token">Text need to decrypt.</param>
        /// <returns>Return decrypted text.</returns>
        public static string Decrypt(string token)
        {
            var decryptedText = DecodeHex(token);
            return RC4(EncryptKey, decryptedText);
        }

        /// <summary>
        /// RC4 algorithms.
        /// </summary>
        /// <param name="key">Key to encode.</param>
        /// <param name="value">Value to encode.</param>
        /// <returns>Return encoded text.</returns>
        private static string RC4(string key, string value)
        {
            char[] ks = key.ToCharArray();
            char[] vs = value.ToCharArray();
            int c = key.Length;
            int d = value.Length;
            int[] s = new int[256];
            int i = 0;
            for (i = 0; i < 256; i++)
            {
                s[i] = i;
            }

            int j = 0;
            int x;
            for (i = 0; i < 256; i++)
            {
                j = (j + s[i] + (int)ks[i % c]) % 256;
                x = s[i];
                s[i] = s[j];
                s[j] = x;
            }

            i = 0;
            j = 0;
            string ct = string.Empty;
            for (int y = 0; y < d; y++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                x = s[i];
                s[i] = s[j];
                s[j] = x;
                ct += Convert.ToChar((int)vs[y] ^ s[(s[i] + s[j]) % 256]);
            }

            return ct;
        }

        /// <summary>
        /// Encode data to hex format.
        /// </summary>
        /// <param name="data">Data need to be encoded.</param>
        /// <returns>Return encoded data.</returns>
        private static string EncodeToHex(string data)
        {
            var b16_digits = HexDigit;
            string[] b16_map = new string[256];
            for (var i = 0; i < 256; i++)
            {
                b16_map[i] = string.Concat(b16_digits[i >> 4], b16_digits[i & 15]);
            }

            int c = data.Length;
            List<string> result = new List<string>();

            for (var i = 0; i < c; i++)
            {
                result.Add(b16_map[Convert.ToInt32(data[i])]);
            }

            return string.Join(null, result.ToArray());
        }

        /// <summary>
        /// Hex decoder.
        /// </summary>
        /// <param name="data">The data for decoding.</param>
        /// <returns>The decoded data.</returns>
        public static string DecodeHex(string data)
        {
            var b16_digits = HexDigit;
            var map = new char[256];
            for (var i = 0; i < 256; i++)
            {
                map[Convert.ToInt32(string.Concat(b16_digits[i >> 4], b16_digits[i & 15]), 16)] = Convert.ToChar(i);
            }

            var length = data.Length;
            if (length % 2 != 0)
            {
                data = '0' + data;
            }

            var result = new List<char>();

            for (var i = 0; i < length; i += 2)
            {
                result.Add(map[Convert.ToInt32(data.Substring(i, 2), 16)]);
            }

            return new string(result.ToArray());
        }
    }
}
