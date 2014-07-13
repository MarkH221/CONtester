using System;
using System.IO;
using Isolib.IOPackage;

namespace Isolib.Algorithms
{
    /// <summary>Contains funtions to calculate Adler32</summary>
    public class Adler32
    {
        /// <summary>Calculate Adler32</summary>
        /// <param name="buffer"> The value that will be calculated</param>
        public static byte[] Compute(byte[] buffer)
        {
            const uint adlerMod = 65521;

            uint a = 1;
            uint b = 0;
            if (buffer != null)
                foreach (var bit in buffer)
                {
                    a = (a + bit) % adlerMod;
                    b = (b + a) % adlerMod;
                }
            var output = BitConverter.GetBytes(Convert.ToUInt32((b << 16) | a));
            Array.Reverse(output);
            return output;
        }

        /// <summary>Calculate Adler32</summary>
        /// <param name="value">The Byte Array value</param>
        /// <param name="offset">The offset into the byte array from which to begin using data</param>
        /// <param name="count">The number of bytes the array to use as data</param>
        public static byte[] Compute(byte[] value, long offset, int count)
        {
            var input = new MemoryStream(value);
            var reader = new BinaryReader(input);
            reader.BaseStream.Position = offset;
            var buffer = reader.ReadBytes(count);
            input.Close();
            reader.Close();
            return Compute(buffer);
        }

        /// <summary>Calculate Adler32</summary>
        /// <param name="stream">The data stream</param>
        /// <param name="offset">The offset into the byte array from which to begin using data</param>
        /// <param name="count">The number of bytes the array to use as data</param>
        public static byte[] Compute(Stream stream, long offset, int count)
        {
            using (var reader = new BinaryReader(stream))
            {
                reader.BaseStream.Position = offset;
                var buffer = reader.ReadBytes(count);
                reader.Close();
                return Compute(buffer);
            }
        }

        /// <summary>Calculate Adler32 using CRC Tables</summary>
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
