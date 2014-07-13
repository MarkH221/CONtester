using System;
using System.IO;
using Isolib.IOPackage;

namespace Isolib.Algorithms
{
    /// <summary>Contains funtions to calculate Checksum32</summary>
    public class Checksum32
    {
        /// <summary>Computes Checksum32</summary>
        /// <param name="buffer">the Input to compute the hash code</param>
        public static byte[] Compute(byte[] buffer)
        {
            uint unsignedTotal = 0;
            foreach (byte bufferValue in buffer)
            {
                unsignedTotal += bufferValue;
            }
            //To turn it into a 32bit 
            buffer = BitConverter.GetBytes(Convert.ToUInt32(unsignedTotal));
            Array.Reverse(buffer);
            return buffer;
        }

        /// <summary>Calculate Checksum32</summary>
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

        /// <summary>Calculate Checksum32</summary>
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

        /// <summary>Calculate Checksum32 using CRC Tables</summary>
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

