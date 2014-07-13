using System;

namespace Isolib.Algorithms
{
    /// <summary>CRC16_CCITT Class </summary>
    public class Crc16Ccitt
    {
        const ushort Poly = 4129;
        readonly ushort[] _table = new ushort[256];
        readonly ushort _initialValue;

        private ushort ComputeHash(byte[] bytes)
        {
            var crc = _initialValue;
            for (var i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ _table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }
        /// <summary>Computes new hash</summary>
        /// <param name="bytes">Bytes to compute</param>
        /// <returns>Generated Hash</returns>
        public byte[] Compute(byte[] bytes)
        {
            var crc = ComputeHash(bytes);
            return BitConverter.GetBytes(crc);
        }

       /// <summary>/// CCITT Values/// </summary>
       public enum InitialCrcValue
       {
           /// <summary> 0 Value</summary>
           Zeros, 
           /// <summary> 0xFFFF Value</summary>
           ValueFfff = 0xffff,
           /// <summary> 0x1D0F Value</summary>
           Value_1D0F = 0x1D0F
       }

        /// <summary>Initialize instance with CCITT Value</summary>
        /// <param name="initialValue"> The CCITT Value</param>
        public Crc16Ccitt(InitialCrcValue initialValue)
        {
            _initialValue = (ushort)initialValue;
            for (var i = 0; i < _table.Length; ++i)
            {
                ushort temp = 0;
                var a = (ushort)(i << 8);
                for (var j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ Poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                _table[i] = temp;
            }
        }
    }
}
