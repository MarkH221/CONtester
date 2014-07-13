using System.IO;
using System.Security.Cryptography;
using Isolib.IOPackage;

namespace Isolib.Algorithms
{
    /// <summary>Contains funtions to calculate SHA1</summary>
    public class Sha1
    {
        /// <summary>Computes a SHA1 hash on specified data</summary>
        /// <param name="buffer">the Input to compute the hash code</param>
        /// <seealso cref="Compute(byte[],int,int)"/>
        public static byte[] Compute(byte[] buffer)
        {
            return new SHA1CryptoServiceProvider().ComputeHash(buffer);
        }

        /// <summary>Computes a SHA1 hash on specified data</summary>
        /// <param name="buffer">the Input to compute the hash code</param>
        /// <param name="offset">The offset into the byte array from which to begin using data</param>
        /// <param name="count">The number of bytes the array to use as data</param>
        public static byte[] Compute(byte[] buffer, int offset, int count)
        {
            return new SHA1CryptoServiceProvider().ComputeHash(buffer, offset, count);
        }

        /// <summary>Computes a SHA1 hash on specified data</summary>
        /// <param name="stream">The data stream to compute the hash code</param>
        /// <param name="offset">The offset into the byte array from which to begin using data</param>
        /// <param name="count">The number of bytes the array to use as data</param>
        public static byte[] Compute(Stream stream, long offset, int count)
        {
            var reader = new BinaryReader(stream);
            reader.BaseStream.Position = offset;
            var buffer = reader.ReadBytes(count);
            reader.Close();
            return Compute(buffer);
        }

        /// <summary>Computes a SHA1 hash on specified data</summary>
        /// <param name="stream">The data RWStream</param>
        /// <param name="offset">The offset into the byte array from which to begin using data</param>
        /// <param name="count">The number of bytes the array to use as data</param>
        public static byte[] Compute(RWStream stream, long offset, int count)
        {
            stream.Position = offset;
            byte[] buffer = stream.ReadBytes(count);
            return Compute(buffer);
        }
    }
}

