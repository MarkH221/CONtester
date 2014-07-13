using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using Isolib.IOPackage;
using Isolib.Properties;

namespace Isolib.Functions
{
    /// <summary>Contains various functions</summary>
    public static class Functions
    {
        /// <summary>Insert Bytes into a byte array</summary>
        public static byte[] InsertBytes(byte[] buffer, byte[] bytesToInsert, long position)
        {
            Array.Resize(ref buffer, buffer.Length + bytesToInsert.Length);
            Array.Copy(bytesToInsert, 0, buffer, position, bytesToInsert.Length);
            return buffer;
        }

        /// <summary>Delete Bytes from array</summary>
        public static byte[] DeleteBytes(byte[] buffer, int index, int length)
        {
            Array.Copy(buffer, (index + length), buffer, index, (buffer.Length - (index + length)));
            Array.Resize(ref buffer,  buffer.Length - length);
            return buffer;
        }

        /// <summary>Gets Percentage Value with %</summary>
        public static string GetPercentage(long smallerNumber, long largerNumber, byte decimalPlaces = 0)
        {
            return Math.Round(Convert.ToDecimal((smallerNumber / largerNumber) * 100), decimalPlaces) + "%";
        }

        /// <summary>Combines 2 arrays</summary>
        public static Array CombineArray(Array firstArray, Array secondArray)
        {
            var target = new Array[0] ;
            Array.Resize(ref target, firstArray.Length + secondArray.Length);
            Array.Copy(firstArray, target, firstArray.Length);
            Array.Copy(secondArray, 0, target, firstArray.Length, secondArray.Length);
            return target;
        }

        /// <summary>Check for valid Hex string</summary>
        public static bool IsValidHex(string hex)
        {
            return new Regex("^[A-Fa-f0-9]*$", RegexOptions.IgnoreCase).IsMatch(hex);
        }

        /// <summary>Check for valid Unicode string</summary>
        public static bool IsValidUnicode(string unicode)
        {
            return new Regex("^(\\u0009|[\\u0020-\\u007E]|\\u0085|[\\u00A0-\\uD7FF]|[\\uE000-\\uFFFD])+$", RegexOptions.IgnoreCase).IsMatch(unicode);
        }

        /// <summary>Check for valid Number value</summary>
        public static bool IsValidNumeric(long numeric)
        {
            return new Regex("^[0-9]+\\d", RegexOptions.IgnoreCase).IsMatch(numeric.ToString());
        }

        /// <summary>Check for valid ASCII string</summary>
        public static bool IsValidAscii(string String)
        {
            return new Regex("^([\\x00-\\xff]*)$", RegexOptions.IgnoreCase).IsMatch(String);
        }

        /// <summary>Rounds length and adds size identifier.</summary>
        public static string GetSize(double value)
        {
            if (value < 1024)
                return value + " bytes";
            value = Math.Round(value / 1024);
            if (value < 1024)
                return value + " KB";
            value = Math.Round(value / 1024);
            if (value < 1024)
                return value + " MB";
            value = Math.Round(value / 1024);
            if (value < 1024)
                return value + " GB";
            value = Math.Round(value / 1024);
            if (value < 1024)
                return value + " TB";
            throw new Exception("Unkown Size !");
        }

        /// <summary>Converts image format</summary>
        public static Image ConvertImageFormat(Image image, ImageFormat format)
        {
            Stream stream = new MemoryStream();
            image.Save(stream, format);
            image = Image.FromStream(stream);
            stream.Close();
            return image;
        }

        /// <summary>campares byte array</summary>
        public static bool CompareByteArray(byte[] firstBuffer, byte[] secondBuffer  )
        {
            if (firstBuffer == null || secondBuffer == null)
                throw new Exception("Buffers are empty !");
            for (var i = 0; i <= firstBuffer.Length - 1; i++)
            {
                if (firstBuffer[i] != secondBuffer[i])
                    return false;
            }
            return true;
        }

        /// <summary>Gets Image from ByteArray</summary>
        public static Image ImageFromByteArray(byte[] buffer)
        {
            if ((buffer == null) || (buffer.Length < 1))
            {
                var noImage = Resources.noImage;
                //Ternary operators - If no image in the resource folder return null or return the image
                return noImage == null ? null : new Bitmap(noImage);
            }
            Image image;
            using (var stream = new MemoryStream(buffer))
            {
                image = Image.FromStream(stream);
                stream.Close();
            }
            return image;
        }

        /// <summary>Gets image from Stream</summary>
        public static Image ImageFromStream(Stream stream)
        {
            if (stream == null || stream.Length == 0) return null;
            return Image.FromStream(stream);
        }

        #region Endian Swap
        /// <summary>Reverse a value</summary>
        /// <param name="value">The value to be reversed</param>
        public static short EndianSwap(short value)
        {
            var num = value & 0xff;
            var num2 = (value >> 8) & 0xff;
            return (short)((num << 8) | num2);
        }

        /// <summary>Reverse a value</summary>
        /// <param name="value">The value to be reversed</param>
        public static int EndianSwap(int value)
        {
            var num = value & 0xff;
            var num2 = (value >> 8) & 0xff;
            var num3 = (value >> 0x10) & 0xff;
            var num4 = (value >> 0x18) & 0xff;
            return ((((num << 0x18) | (num2 << 0x10)) | (num3 << 8)) | num4);
        }

        /// <summary>Reverse a value</summary>
        /// <param name="value">The value to be reversed</param>
        public static long EndianSwap(long value)
        {
            long num = value & 0xffL;
            long num2 = (value >> 8) & 0xffL;
            long num3 = (value >> 0x10) & 0xffL;
            long num4 = (value >> 0x18) & 0xffL;
            long num5 = (value >> 0x20) & 0xffL;
            long num6 = (value >> 40) & 0xffL;
            long num7 = (value >> 0x30) & 0xffL;
            long num8 = (value >> 0x38) & 0xffL;
            return ((((((((num << 0x38) | (num2 << 0x30)) | (num3 << 40)) | (num4 << 0x20)) | (num5 << 0x18)) | (num6 << 0x10)) | (num7 << 8)) | num8);
        }

        /// <summary>Converts the Endian of a byte array</summary>
        /// <param name="buffer">The Byte Array</param>
        public static byte[] EndianSwap(byte[] buffer)
        {
            Array.Reverse(buffer);
            return buffer;
        }

        /// <summary>Converts the Endian of a byte array</summary>
        /// <param name="value">the double value</param>
        public static double EndianSwap(double value)
        {
            return ByteArrayToDouble(EndianSwap(DoubleToBytesArray(value)));
        }

        /// <summary>Converts the Endian of a byte array</summary>
        /// <param name="value">The float value</param>
        public static float EndianSwap(float value)
        {
            return ByteArrayToSingle(EndianSwap(SingleToBytesArray(value)));
        }
        #endregion

        #region To Hex String
        /// <summary>Convert Long to String Hex</summary>
        /// <param name="value">The byte array</param>
        /// <returns>Returns an hex string value</returns>
        public static string LongToHexString(long value)
        {
            try
            {
                var buffer = Int64ToBytesArray(value);
                return BitConverter.ToString(buffer).Replace("-", "");
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Convert float to String Hex</summary>
        /// <param name="value">The float value</param>
        /// <returns>Returns an hex string value</returns>
        public static string FloatToHexString(float value)
        {
            try
            {
                return ByteArrayToHexString(SingleToBytesArray(value));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Convert Byte Array to String Hex</summary>
        /// <param name="value">The byte array</param>
        /// <returns>Returns an hex string value</returns>
        public static string ByteArrayToHexString(byte[] value)
        {
            try
            {
                return BitConverter.ToString(value).Replace("-", "");
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Convert a decimal value to 4 bytes hex string</summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The hex string</returns>
        public static string DecimalToHexString4Bytes(Decimal value)
        {
            try
            {
                byte[] output = DecimalTo4BytesArray(value);
                return (ByteArrayToHexString(output));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Convert a decimal value to hex string</summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The hex string</returns>
        public static string DecimalToHexString(Decimal value)
        {
            try
            {
                byte[] output = DecimalToBytesArray(value);
                return (ByteArrayToHexString(output));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Edit a String Hex</summary>
        /// <param name="hexString">The valid String you want to edit</param>
        /// <param name="firstCharPos">The first Character we want to keep Position -starting from 0</param>
        /// <param name="finalCharPos">The Final Character we want to keep</param>
        /// <returns>Returns an hex string value</returns>
        public static string StringToHexString(string hexString, int firstCharPos, int finalCharPos)
        {
            try
            {
                hexString = hexString.Replace(" ", "");
                hexString = hexString.Replace("-", "");
                hexString = hexString.Replace("0x", "");
                hexString = hexString.Replace("0X", "");
                var str = hexString.Remove(finalCharPos + 1, hexString.Length - (finalCharPos + 1));
                var str2 = str.Substring(firstCharPos, str.Length - firstCharPos);
                if ((str2.Length % 2) != 0)
                {
                    return "0" + str2;
                }
                return null;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        #endregion

        /// <summary>Converts an Int32 FatTime to a DateTime</summary>
        /// <param name="dateTime">The integer value to convert</param>
        public static DateTime IntToFatTimeDateTime(int dateTime)
        {
            var num = (short)(dateTime >> 0x10);
            var num2 = (short)(dateTime & 0xffff);
            if ((num == 0) && (num2 == 0))
            {
                return DateTime.Now;
            }
            return new DateTime(((num & 0xfe00) >> 9) + 0x7bc, (num & 480) >> 5, num & 0x1f, (num2 & 0xf800) >> 11, (num2 & 0x7e0) >> 5, (num2 & 0x1f) * 2);
        }

        /// <summary>Converts an Int64 FileTime to a DateTime</summary>
        /// <param name="input">The long value to convert</param>
        public static DateTime LongToDateTime(long input)
        {
            try
            {
                return DateTime.FromFileTime(input);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        /// <summary>Converts a DateTime to an Int64 FileTime</summary>
        /// <param name="input">The date time input</param>
        /// <returns></returns>
        public static long DateTimeToInt64(DateTime input)
        {
            try
            {
                return input.ToFileTime();
            }
            catch
            {
                return DateTime.Now.ToFileTime();
            }
        }

        /// <summary>Returns an all 0 to DT</summary>
        public static DateTime DateTimeZero
        {
            get
            {
                return new DateTime(0L);
            }
        }

        #region Values to byte Array
        /// <summary>Converts Integer 16 to 2 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] Int16ToBytesArray(Int16 value)
        {
            var buffer = new Byte[2];
            buffer[1] = (Byte)(value & 0xFF);
            buffer[0] = (Byte)((value >> 8) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Unsigned Integer 16 to 2 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] UInt16ToBytesArray(UInt16 value)
        {
            var buffer = new Byte[2];
            buffer[1] = (Byte)(value & 0xFF);
            buffer[0] = (Byte)((value >> 8) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Integer 24 to 3 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] Int24ToBytesArray(Int32 value)
        {
            if (value < -8388608 || value > 8388607)
                throw new Exception("Invalid value");
            var buffer = new Byte[3];
            buffer[2] = (Byte)(value & 0xFF);
            buffer[1] = (Byte)((value >> 8) & 0xFF);
            buffer[0] = (Byte)((value >> 16) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Unsigned Integer 24 to 3 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] UInt24ToBytesArray(UInt32 value)
        {
            if (value < 0 || value > 16777215)
                throw new Exception("Invalid value");
            var buffer = new Byte[3];
            buffer[2] = (Byte)(value & 0xFF);
            buffer[1] = (Byte)((value >> 8) & 0xFF);
            buffer[0] = (Byte)((value >> 16) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Integer 32 to 4 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] Int32ToBytesArray(Int32 value)
        {
            var buffer = new Byte[4];
            buffer[3] = (Byte)(value & 0xFF);
            buffer[2] = (Byte)((value >> 8) & 0xFF);
            buffer[1] = (Byte)((value >> 16) & 0xFF);
            buffer[0] = (Byte)((value >> 24) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Unsigned Integer 32 to 4 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] UInt32ToBytesArray(UInt32 value)
        {
            var buffer = new Byte[4];
            buffer[3] = (Byte)(value & 0xFF);
            buffer[2] = (Byte)((value >> 8) & 0xFF);
            buffer[1] = (Byte)((value >> 16) & 0xFF);
            buffer[0] = (Byte)((value >> 24) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Integer 40 to 5 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] Int40ToBytesArray(Int64 value)
        {
            if (value < -549755813888 || value > 549755813887)
            {
                throw new Exception("Invalid value");
            }
            var buffer = new Byte[5];
            buffer[4] = (Byte)(value & 0xFF);
            buffer[3] = (Byte)((value >> 8) & 0xFF);
            buffer[2] = (Byte)((value >> 16) & 0xFF);
            buffer[1] = (Byte)((value >> 24) & 0xFF);
            buffer[0] = (Byte)((value >> 32) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Unsigned Integer 40 to 5 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] UInt40ToBytesArray(UInt64 value)
        {
            if (value < 0 || value > 1099511627775)
            {
                throw new Exception("Invalid value");
            }
            var buffer = new Byte[5];
            buffer[4] = (Byte)(value & 0xFF);
            buffer[3] = (Byte)((value >> 8) & 0xFF);
            buffer[2] = (Byte)((value >> 16) & 0xFF);
            buffer[1] = (Byte)((value >> 24) & 0xFF);
            buffer[0] = (Byte)((value >> 32) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Integer 64 to 6 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] Int48ToBytesArray(Int64 value)
        {
            var buffer = new Byte[6];
            buffer[5] = (Byte)(value & 0xFF);
            buffer[4] = (Byte)((value >> 8) & 0xFF);
            buffer[3] = (Byte)((value >> 16) & 0xFF);
            buffer[2] = (Byte)((value >> 24) & 0xFF);
            buffer[1] = (Byte)((value >> 32) & 0xFF);
            buffer[0] = (Byte)((value >> 40) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Unsigned Integer 64 to 6 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] UInt48ToBytesArray(UInt64 value)
        {
            var buffer = new Byte[6];
            buffer[5] = (Byte)(value & 0xFF);
            buffer[4] = (Byte)((value >> 8) & 0xFF);
            buffer[3] = (Byte)((value >> 16) & 0xFF);
            buffer[2] = (Byte)((value >> 24) & 0xFF);
            buffer[1] = (Byte)((value >> 32) & 0xFF);
            buffer[0] = (Byte)((value >> 40) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Integer 64 to 7 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] Int56ToBytesArray(Int64 value)
        {
            var buffer = new Byte[7];
            buffer[6] = (Byte)(value & 0xFF);
            buffer[5] = (Byte)((value >> 8) & 0xFF);
            buffer[4] = (Byte)((value >> 16) & 0xFF);
            buffer[3] = (Byte)((value >> 24) & 0xFF);
            buffer[2] = (Byte)((value >> 32) & 0xFF);
            buffer[1] = (Byte)((value >> 40) & 0xFF);
            buffer[0] = (Byte)((value >> 48) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Unsigned Integer 64 to 7 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] UInt56ToBytesArray(UInt64 value)
        {
            var buffer = new Byte[7];
            buffer[6] = (Byte)(value & 0xFF);
            buffer[5] = (Byte)((value >> 8) & 0xFF);
            buffer[4] = (Byte)((value >> 16) & 0xFF);
            buffer[3] = (Byte)((value >> 24) & 0xFF);
            buffer[2] = (Byte)((value >> 32) & 0xFF);
            buffer[1] = (Byte)((value >> 40) & 0xFF);
            buffer[0] = (Byte)((value >> 48) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Integer 64 to 8 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] Int64ToBytesArray(Int64 value)
        {
            var buffer = new Byte[8];
            buffer[7] = (Byte)(value & 0xFF);
            buffer[6] = (Byte)((value >> 8) & 0xFF);
            buffer[5] = (Byte)((value >> 16) & 0xFF);
            buffer[4] = (Byte)((value >> 24) & 0xFF);
            buffer[3] = (Byte)((value >> 32) & 0xFF);
            buffer[2] = (Byte)((value >> 40) & 0xFF);
            buffer[1] = (Byte)((value >> 48) & 0xFF);
            buffer[0] = (Byte)((value >> 56) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Unsigned Integer 64 to 8 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] UInt64ToBytesArray(UInt64 value)
        {
            var buffer = new Byte[8];
            buffer[7] = (Byte)(value & 0xFF);
            buffer[6] = (Byte)((value >> 8) & 0xFF);
            buffer[5] = (Byte)((value >> 16) & 0xFF);
            buffer[4] = (Byte)((value >> 24) & 0xFF);
            buffer[3] = (Byte)((value >> 32) & 0xFF);
            buffer[2] = (Byte)((value >> 40) & 0xFF);
            buffer[1] = (Byte)((value >> 48) & 0xFF);
            buffer[0] = (Byte)((value >> 56) & 0xFF);
            return buffer;
        }

        /// <summary>Converts Single to 4 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] SingleToBytesArray(Single value)
        {
            var buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            return buffer;
        }

        /// <summary>Converts Double to 8 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] DoubleToBytesArray(Double value)
        {
            var buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            return buffer;
        }

        /// <summary>Converts Decimal to 8 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] DecimalToBytesArray(Decimal value)
        {
            var buffer = BitConverter.GetBytes(System.Int64.Parse(value.ToString()));
            Array.Reverse(buffer);
            return buffer;
        }

        /// <summary>Converts Decimal to 4 Byte array</summary>
        /// <param name="value">The value to be converted</param>
        public static Byte[] DecimalTo4BytesArray(Decimal value)
        {
            var buffer = BitConverter.GetBytes(System.Int32.Parse(value.ToString()));
            Array.Reverse(buffer);
            return buffer;
        }

        /// <summary>Converts an Image to a byte array</summary>
        /// <param name="image"> The Image</param>
        /// <param name="format"> The Image Type</param>
        public static Byte[] ImageToByteArray(Image image, ImageFormat format)
        {
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, format);
            var returnVal = memoryStream.ToArray();
            memoryStream.Dispose();
            return returnVal;
        }

        /// <summary>Get a piece of a full byte array</summary>
        /// <param name="piece">The Full byte you want to take a piece from</param>
        /// <param name="startOffset">The starting offset</param>
        /// <param name="size">The full size of your new byte piece</param>
        public static Byte[] BytePiece(Byte[] piece, UInt32 startOffset, UInt32 size)
        {
            var buffer = new Byte[size];

            for (var i = 0; i < size; i++)
            {
                buffer[i] = piece[startOffset + i];
            }
            return buffer;
        }
        #endregion

        #region ByteArray to Values(Integer, Decimal & Pieces)
        /// <summary>Converts 2 Bytes Array to Unsigned Integer 16</summary>
        /// <param name="buffer">byte Array</param>
        public static UInt16 ByteArrayToUInt16(byte[] buffer)
        {
            if (buffer.Length > 2)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 2)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return (ushort)(buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 2 Bytes Array to Integer 16</summary>
        /// <param name="buffer">byte Array</param>
        public static Int16 ByteArrayToInt16(Byte[] buffer)
        {
            if (buffer.Length > 2)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 2)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return (short)(buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 3 Bytes Array to Integer 24</summary>
        /// <param name="buffer">byte Array</param>
        public static Int32 ByteArrayToInt24(Byte[] buffer)
        {
            if (buffer.Length > 3)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 3)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return (buffer[2] << 16 | buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 3 Bytes Array to Unsigned Unsigned Integer 24</summary>
        /// <param name="buffer">byte Array</param>
        public static UInt32 ByteArrayToUInt24(Byte[] buffer)
        {
            if (buffer.Length > 3)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 3)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return ((uint)buffer[2] << 16 | (uint)buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 4 Bytes Array to Integer 32</summary>
        /// <param name="buffer">byte Array</param>
        public static Int32 ByteArrayToInt32(Byte[] buffer)
        {
            if (buffer.Length > 4)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 4)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return (buffer[3] << 24 | buffer[2] << 16 | buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 4 Bytes Array to Unsigned Integer 32</summary>
        /// <param name="buffer">byte Array</param>
        public static UInt32 ByteArrayToUInt32(Byte[] buffer)
        {
            if (buffer.Length > 4)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 4)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return (uint)(buffer[3] << 24 | buffer[2] << 16 | buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 5 Bytes Array to Integer 64</summary>
        /// <param name="buffer">byte Array</param>
        public static Int64 ByteArrayToInt40(Byte[] buffer)
        {
            if (buffer.Length > 5)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 5)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return ((long)buffer[4] << 32 | (long)buffer[3] << 24 | (long)buffer[2] << 16 | (long)buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 5 Bytes Array to Unsigned Integer 64</summary>
        /// <param name="buffer">byte Array</param>
        public static UInt64 ByteArrayToUInt40(Byte[] buffer)
        {
            if (buffer.Length > 5)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 5)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return ((ulong)buffer[4] << 32 | (ulong)buffer[3] << 24 | (ulong)buffer[2] << 16 | (ulong)buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 6 Bytes Array to Integer 64</summary>
        /// <param name="buffer">byte Array</param>
        public static Int64 ByteArrayToInt48(Byte[] buffer)
        {
            if (buffer.Length > 6)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 6)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return ((long)buffer[5] << 40 | (long)buffer[4] << 32 | (long)buffer[3] << 24 | (long)buffer[2] << 16 | (long)buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 6 Bytes Array to Unsigned Integer 64</summary>
        /// <param name="buffer">byte Array</param>
        public static UInt64 ByteArrayToUInt48(Byte[] buffer)
        {
            if (buffer.Length > 6)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 6)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return ((ulong)buffer[5] << 40 | (ulong)buffer[4] << 32 | (ulong)buffer[3] << 24 | (ulong)buffer[2] << 16 | (ulong)buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 7 Bytes Array to Integer 64</summary>
        /// <param name="buffer">byte Array</param>
        public static Int64 ByteArrayToInt56(Byte[] buffer)
        {
            if (buffer.Length > 7)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 7)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return ((long)buffer[6] << 48 | (long)buffer[5] << 40 | (long)buffer[4] << 32 | (long)buffer[3] << 24 | (long)buffer[2] << 16 | (long)buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 7 Bytes Array to Unsigned Integer 64</summary>
        /// <param name="buffer">byte Array</param>
        public static UInt64 ByteArrayToUInt56(Byte[] buffer)
        {
            if (buffer.Length > 7)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 7)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return ((ulong)buffer[6] << 48 | (ulong)buffer[5] << 40 | (ulong)buffer[4] << 32 | (ulong)buffer[3] << 24 | (ulong)buffer[2] << 16 | (ulong)buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 8 Bytes Array to Integer 64</summary>
        /// <param name="buffer">byte Array</param>
        public static Int64 ByteArrayToInt64(Byte[] buffer)
        {
            if (buffer.Length > 8)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 8)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return ((long)buffer[7] << 56 | (long)buffer[6] << 48 | (long)buffer[5] << 40 | (long)buffer[4] << 32 | (long)buffer[3] << 24 | (long)buffer[2] << 16 | (long)buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts 8 Bytes Array to Unsigned Integer 64</summary>
        /// <param name="buffer">byte Array</param>
        public static UInt64 ByteArrayToUInt64(Byte[] buffer)
        {
            if (buffer.Length > 8)
                throw new Exception("Buffer size too big");
            if (buffer.Length < 8)
                throw new Exception("Buffer size too small");
            Array.Reverse(buffer);
            return ((ulong)buffer[7] << 56 | (ulong)buffer[6] << 48 | (ulong)buffer[5] << 40 | (ulong)buffer[4] << 32 | (ulong)buffer[3] << 24 | (ulong)buffer[2] << 16 | (ulong)buffer[1] << 8 | buffer[0]);
        }

        /// <summary>Converts Byte Array to Single/Float</summary>
        /// <param name="buffer">The Byte Array</param>
        public static Single ByteArrayToSingle(Byte[] buffer)
        {
            try
            {
                Array.Reverse(buffer);
                return BitConverter.ToSingle(buffer, 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>Converts Byte Array to Double</summary>
        /// <param name="buffer">The Byte Array</param>
        public static Double ByteArrayToDouble(Byte[] buffer)
        {
            try
            {
                Array.Reverse(buffer);
                return BitConverter.ToDouble(buffer, 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        /// <summary>Convert hex string to Int</summary>
        /// <param name="value">The string(hex) value</param>
        /// <returns>A long value of the given hex</returns>
        public static int HexToInt(String value)
        {
            try
            {
                if (!IsValidHex(value))
                    throw new Exception(String.Format("{0} is not a valid hex.", value));
                return ByteArrayToInt32(HexToBytes(value));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>Convert hex string to Long</summary>
        /// <param name="value">The string(hex) value</param>
        /// <returns>A long value of the given hex</returns>
        public static long HexToLong(String value)
        {
            try
            {
                if (!IsValidHex(value))
                    throw new Exception(String.Format("{0} is not a valid hex.", value));
                return ByteArrayToInt64(HexToBytes(value));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>Converts a Hex string to bytes</summary>
        /// <param name="input">Is the String input</param>
        public static byte[] HexToBytes(String input)
        {
            input = input.Replace(" ", "");
            input = input.Replace("-", "");
            input = input.Replace("0x", "");
            input = input.Replace("0X", "");
            if ((input.Length % 2) != 0)
                input = "0" + input;
            var output = new byte[(input.Length / 2)];

            try
            {
                int index;
                for (index = 0; index < output.Length; index++)
                {
                    output[index] = Convert.ToByte(input.Substring((index * 2), 2), 16);
                }
                return output;
            }
            catch
            {
                throw new Exception("Invalid byte Input");
            }
        }

        /// <summary>Reverse array of bytes</summary>
        /// <param name="piece">The byte array to use</param>
        public static byte[] ReverseByteArray(byte[] piece)
        {
            Array.Reverse(piece);
            return piece;
        }

        /// <summary>Converts a DateTime to an Int32 FatTime</summary>
        /// <param name="dateTime">The date Time to convert</param>
        public static int DateTimeToFatTimeInt(DateTime dateTime)
        {
            if (dateTime.Year < 0x7BC)
                dateTime = new DateTime(0x7BC, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            var time = ((dateTime.Hour << 11) | (dateTime.Minute << 5)) | (dateTime.Second >> 1);
            var date = (((dateTime.Year - 0x7BC) << 9) | (dateTime.Month << 5)) | dateTime.Day;
            return ((date << 0x10) | time);
        }

        #region Internal Use Only
        /// <summary>Returns the count of blocks this file contains</summary>
        /// <param name="theTemporaryIo">The temporary I/O</param>
        internal static uint BlockCountFile(RWStream theTemporaryIo)
        {
            try
            {
                if (theTemporaryIo.Length == 0L)
                    return 0;
                return (uint)(((theTemporaryIo.Length - 1L) / 0x1000L) + 1L);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Return the count of remaining blocks to write or read</summary>
        /// <param name="temporaryIo">The temporary Io</param>
        internal static int BlockRemainder(RWStream temporaryIo)
        {
            try
            {
                if (temporaryIo.Length == 0L && temporaryIo.Length > 0xFFFFFFFFL) return 0;
                return (int)(temporaryIo.Length % 0x1000L);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Fix the path/name of the embedded file in case of bad character</summary>
        /// <param name="input">The String input path/name</param>
        internal static string FixEmbeddedFilePath(string input)
        {
            if (String.IsNullOrEmpty(input))
                return "";
            input = input.Replace('\\', '/');
            if (input[0] == '/')
                input = input.Substring(1, input.Length - 1);
            if (input[input.Length - 1] == '/')
                input = input.Substring(0, input.Length - 1);
            return input;
        }

        /// <summary>Checks if the name is a valid Xbox Name</summary>
        /// <param name="name">String value</param>
        /// <returns>True if valid</returns>
        internal static void IsValidXboxName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                var list = new List<char> { '"', '*', '/', ':', '<', '\\', '|', '\x00ff' };
                int index;
                for (index = 0; index <= 0x1F; index++)
                    list.Add((char)index);
                for (index = 0x3E; index <= 0x3F; index++)
                    list.Add((char)index);
                for (index = 0x7F; index <= 0xFE; index++)
                    list.Add((char)index);
                if (name.IndexOfAny(list.ToArray()) == -1) return;
                throw new Exception("Contains invalid characters");
            }
            throw new Exception("Contains invalid characters");
        }

        /// <summary>Copy a range of elements from one array to another</summary>
        /// <param name="destinationArray">Data's destination</param>
        /// <param name="sourceArray">Contains Data to copy</param>
        internal static void SetByteArray(ref byte[] destinationArray, byte[] sourceArray)
        {
            Array.Copy(sourceArray, 0, destinationArray, 0, (sourceArray.Length >= destinationArray.Length) ? destinationArray.Length : sourceArray.Length);
        }

        /// <summary>EndianSwaps bytes in 8 byte blocks, Reverse true if reverse each 8 byte blocks</summary>
        /// <param name="sourceArrayPiece">The byte array to use</param>
        /// <param name="reverse">If to reverse the byte array</param>
        internal static byte[] SimpleScramble(byte[] sourceArrayPiece, bool reverse)
        {
            int index;
            if ((sourceArrayPiece.Length % 8) != 0)
                throw new Exception("Input not divisible by 8");
            //stream the source array piece
            var stream = new RWStream(sourceArrayPiece, true, true);
            for (index = 0; index < (sourceArrayPiece.Length / 2); index += 8)
            {
                stream.Position = index;
                var buffer = stream.ReadBytes(8);
                stream.Position = (sourceArrayPiece.Length - index) - 8;
                var buffer2 = stream.ReadBytes(8);
                stream.Position = index;
                stream.WriteBytes(buffer2);
                stream.Position = (sourceArrayPiece.Length - index) - 8;
                stream.WriteBytes(buffer);
                stream.Flush();
            }
            if (reverse)
            {
                for (index = 0; index < sourceArrayPiece.Length; index += 8)
                    Array.Reverse(sourceArrayPiece, index, 8);
            }
            stream.Close();
            return sourceArrayPiece;
        }
        #endregion
    }
}
