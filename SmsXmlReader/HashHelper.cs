using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SmsXmlReader
{
    public class HashHelper
    {
        private SHA1 sha1Hash;

        private HashHelper()
        {
            sha1Hash = SHA1.Create();
        }

        public static HashHelper Instance => new HashHelper();

        public byte[] HashString(string data)
        {
            return sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        public byte[] HashStream(Stream data)
        {
            return sha1Hash.ComputeHash(data);
        }

        public static byte[] CombineHashes(byte[] hash1, byte[] hash2)
        {
            // Check the arguments
            if (hash1 == null) throw new ArgumentNullException("hash1");
            if (hash2 == null) throw new ArgumentNullException("hash2");
            if (hash1.Length != hash2.Length) throw new ApplicationException("Hash lengths are not the same");

            byte[] Result = new byte[hash1.Length];

            for (int i = 0; i < hash1.Length; i++)
                Result[i] = (byte) (hash1[i] ^ hash2[i]);
 
            return Result;
        }
    }
}
