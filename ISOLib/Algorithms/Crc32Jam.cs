using System;
using System.IO;
using Isolib.IOPackage;

namespace Isolib.Algorithms
{
    /// <summary>Contains funtions to calculate CRC32-JAM</summary>
    public class Crc32Jam
    {
        /// <summary>Calculate CRC32-JAM</summary>
        /// <param name="buffer"> The value that will be calculated</param>
        public static byte[] Compute(byte[] buffer)
        {
            var crcTable = new uint[256];
            const uint crcInitialValue = 0XFFFFFFFFU;

            var tmp = crcInitialValue;
            foreach (var b in buffer)
            {
                tmp = (tmp >> 8) ^ crcTable[Convert.ToUInt32(tmp ^ b) & 0XFF];
            }
            buffer = BitConverter.GetBytes(Convert.ToUInt32(tmp));
            Array.Reverse(buffer);
            return buffer;
        }

        /// <summary>Calculate CRC32-JAM</summary>
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

        /// <summary>Calculate CRC32-JAM</summary>
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

        /// <summary>Calculate CRC32 - JAM using CRC Tables</summary>
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
