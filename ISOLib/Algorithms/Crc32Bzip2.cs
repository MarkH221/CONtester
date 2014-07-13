using System;
using System.IO;
using Isolib.IOPackage;

namespace Isolib.Algorithms
{
    /// <summary>Contains funtions to calculate CRC32 BZIP2</summary>
    public class Crc32Bzip2
    {
        /// <summary>Compute CRC32 BZIP2</summary>
        /// <param name="bytes"> The value that will be calculated</param>
        public static byte[] Compute(byte[] bytes)
        {
            var crcTable = new uint[256];
            const uint crcPolynomial = 0X4C11DB7U;
            const uint crcFinalXor = 0XFFFFFFFFU;

            for (var i = 0; i <= 255; i++)
            {
                var crc = (Convert.ToUInt32(i) << 24);
                
                for (var j = 0; j <= 7; j++)
                {
                    if ((crc & 0X80000000U) != 0)
                    {
                        crc = ((crc << 1) ^ crcPolynomial);
                    }
				    else
				    {
					    crc = (crc << 1);
				    }
			    }
                crcTable[i] = crc;
		    }
            
            var result = 0XFFFFFFFFU;
		    foreach (var b in bytes)
		    {
			    var i = (Convert.ToInt32((int)(result >> 24)) ^ b) & 0XFF;
			    result = (result << 8) ^ crcTable[i];
		    }
		    result = (result ^ crcFinalXor);
		    bytes = BitConverter.GetBytes(Convert.ToUInt32(result));
		    Array.Reverse(bytes);
		    return bytes;
	    }

        /// <summary>Calculate CRC32 BZIP2</summary>
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

        /// <summary>Calculate CRC32 BZIP2</summary>
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

        /// <summary>Calculate CRC32 BZIP using CRC Tables</summary>
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
