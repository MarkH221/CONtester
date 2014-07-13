using System;

namespace Isolib.Algorithms
{
    /// <summary>Computes a Hash-based Message Authentication Code (HMAC) using the MD5 hash function.</summary>
    public class Hmacmd5
    {
        private readonly System.Security.Cryptography.HMACMD5 _hash;
        /// <summary>Set new HMACMD5 instance</summary>
        public Hmacmd5()
        {
            _hash = new System.Security.Cryptography.HMACMD5();
        }

        /// <summary>
        /// Generate hash from byte array
        /// </summary>
        /// <param name="buffer">byte array to compute</param>
        /// <param name="offset">start position</param>
        /// <param name="length">Total length to read</param>
        /// <returns>computed hash</returns>
        public byte[] Compute(byte[] buffer, long offset, int length)
        {
            return _hash.ComputeHash(buffer, Convert.ToInt32(offset), length);
        }

        /// <summary>
        /// get/set Cryptography Key
        /// </summary>
        public byte[] Key
        {
            get { return _hash.Key; }
            set
            {
                if (value != null) _hash.Key = value;
            }
        }
        /// <summary>
        /// Generate hash from byte array
        /// </summary>
        /// <param name="buffer">byte array to compute</param>
        /// <returns>computed hash</returns>
        public byte[] Compute(byte[] buffer)
        {
            return _hash.ComputeHash(buffer);
        }
    }
}
