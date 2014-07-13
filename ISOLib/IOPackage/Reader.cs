using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace Isolib.IOPackage
{
    /// <summary>Contains function/s that deals with I/O reading of data</summary>
    public class Reader : IDisposable
    {
        private bool _accessed;
        private BinaryReader _bReader;
        private Stream _fStream;
        private bool _isBigEndian;
        private long _lastPosition;

        #region Constructors
        /// <summary>Initialize Array Streaming</summary>
        /// <param name="byteArray">The Byte Array to Stream</param>
        /// <param name="isBigEndian">Endian Type - Big Endian by Default</param>
        public Reader(byte[] byteArray, bool isBigEndian)
        {
            try
            {
                _fStream = new MemoryStream(byteArray);
                _bReader = new BinaryReader(_fStream);
                _isBigEndian = isBigEndian;
                _accessed = true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Initialize data stream viaI/O Stream</summary>
        /// <param name="stream">TheI/O Stream</param>
        /// <param name="isBigEndian">Endian Type - Big Endian by Default</param>
        public Reader(Stream stream, bool isBigEndian)
        {
            if (!stream.CanWrite)
            {
                throw new Exception("Stream does not have write access!");
            }
            if (!stream.CanRead)
            {
                throw new Exception("Stream does not have read access!");
            }
            _fStream = stream;
            _bReader = new BinaryReader(_fStream);
            _isBigEndian = isBigEndian;
            _accessed = true;
        }

        /// <summary>Initialize File Reader and Writer by specifying file a valid file path</summary>
        /// <param name="path">The file Path</param>
        /// <param name="isBigEndian">Endian Type, Default is true</param>
        public Reader(string path, bool isBigEndian)
        {
            if (!File.Exists(path))
            {
                throw new Exception("File does not exist!");
            }
            _fStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _bReader = new BinaryReader(_fStream);
            _isBigEndian = isBigEndian;
            _accessed = true;
        }

        /// <summary>Initialize File Reader and Writer by specifying file a valid file path</summary>
        /// <param name="path">The file Path</param>
        /// <param name="isBigEndian">Endian Type, Default is true</param>
        /// <param name="fileShare">The File Share Option</param>
        public Reader(string path, bool isBigEndian, FileShare fileShare)
        {
            if (!File.Exists(path))
            {
                throw new Exception("File does not exist!");
            }
            _fStream = new FileStream(path, FileMode.Open, FileAccess.Read, fileShare);
            _bReader = new BinaryReader(_fStream);
            _isBigEndian = isBigEndian;
            _accessed = true;
        }
        #endregion

        #region Reader
        /// <summary>Reads a set size of bytes</summary>
        /// <param name="length">The byte array length</param>
        /// <returns>Byte Array</returns>
        public byte[] ReadBytes(int length)
        {
            return ReadBytes(length, _isBigEndian);
        }

        /// <summary>Reads a set size of bytes</summary>
        /// <param name="length">The byte array length</param>
        /// <param name="isBigEndien">Specifiy if read is in Big Endien Type</param>
        public byte[] ReadBytes(int length, bool isBigEndien)
        {
            try
            {
                if (Position == Length)
                    throw new Exception("Cannot move position past file size");
                if (length == 0)
                    return new byte[0];
                var buffer = new byte[length];
                _fStream.Read(buffer, 0, length);
                if (!isBigEndien)
                    Array.Reverse(buffer);
                return buffer;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 64-bit double (8 bytes)</summary>
        public double ReadDouble()
        {
            try
            {
                return ReadDouble(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 32-bit double (8 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public double ReadDouble(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToDouble(ReadBytes(8, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads Hex String</summary>
        /// <param name="length">The amount of bytes to read</param>
        public string ReadHexString(int length)
        {
            try
            {
                return Functions.Functions.ByteArrayToHexString(ReadBytes(length, _isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 16-bit signed integer (2 bytes)</summary>
        public short ReadInt16()
        {
            try
            {
                return ReadInt16(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 16-bit signed integer (2 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public short ReadInt16(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt16(ReadBytes(2, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 24-bit signed integer (3 bytes)</summary>
        public int ReadInt24()
        {
            try
            {
                return ReadInt24(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 24-bit signed integer (3 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public int ReadInt24(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt24(ReadBytes(3, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 32-bit signed integer (4 bytes)</summary>
        public int ReadInt32()
        {
            try
            {
                return ReadInt32(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 32-bit signed integer (4 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public int ReadInt32(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt32(ReadBytes(4, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 40-bit signed integer (5 bytes)</summary>
        public long ReadInt40()
        {
            try
            {
                return ReadInt40(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 40-bit signed integer (5 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public long ReadInt40(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt40(ReadBytes(5, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 48-bit signed integer (6 bytes)</summary>
        public long ReadInt48()
        {
            try
            {
                return ReadInt48(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 48-bit signed integer (6 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public long ReadInt48(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt48(ReadBytes(6, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 56-bit signed integer (7 bytes)</summary>
        public long ReadInt56()
        {
            try
            {
                return ReadInt56(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 56-bit signed integer (7 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public long ReadInt56(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt56(ReadBytes(7, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 64-bit signed integer (8 bytes)</summary>
        public long ReadInt64()
        {
            try
            {
                return ReadInt64(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 64-bit signed integer (8 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public long ReadInt64(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt64(ReadBytes(8, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 8-bit signed integer (1 byte)</summary>
        public sbyte ReadInt8()
        {
            try
            {
                return _bReader.ReadSByte();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 32-bit Float (4 bytes)</summary>
        public float ReadSingle()
        {
            try
            {
                return ReadSingle(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 32-bit signed integer (4 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public float ReadSingle(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToSingle(ReadBytes(4, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads all the bytes</summary>
        public byte[] ReadStream()
        {
            try
            {
                var buffer = new byte[Length];
                var position = Position;
                Position = 0L;
                long index;
                for (index = 0L; index < buffer.Length; index++)
                {
                    buffer[(int)((IntPtr)index)] = ReadUInt8();
                }
                Position = position;
                return buffer;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads String with specific string type</summary>
        /// <param name="stringType">The Specified String type to read</param>
        /// <param name="length">The byte length of the String to read</param>
        public string ReadString(StringType stringType, int length)
        {
            if (!Enum.IsDefined(typeof(StringType), stringType))
            {
                throw new Exception("Invalid parameters");
            }
            if (Position >= (Length - 1L))
            {
                return "";
            }
            var bytes = ReadBytes(length, _isBigEndian);
            if (stringType != StringType.Unicode)
            {
                return Encoding.ASCII.GetString(bytes);
            }
            return !IsBigEndian ? Encoding.Unicode.GetString(bytes) : Encoding.BigEndianUnicode.GetString(bytes);
        }

        /// <summary>Reads a 16-bit unsigned integer (2 bytes)</summary>
        public ushort ReadUInt16()
        {
            try
            {
                return ReadUInt16(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 16-bit unsigned integer (2 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public ushort ReadUInt16(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt16(ReadBytes(2, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 24-bit unsigned integer (3 bytes)</summary>
        public uint ReadUInt24()
        {
            try
            {
                return ReadUInt24(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 24-bit unsigned integer (3 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public uint ReadUInt24(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt24(ReadBytes(3, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 32-bit unsigned integer (4 bytes)</summary>
        public uint ReadUInt32()
        {
            try
            {
                return ReadUInt32(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 32-bit unsigned integer (4 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public uint ReadUInt32(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt32(ReadBytes(4, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 40-bit unsigned integer (5 bytes)</summary>
        public ulong ReadUInt40()
        {
            try
            {
                return ReadUInt40(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 40-bit unsigned integer (5 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public ulong ReadUInt40(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt40(ReadBytes(5, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 48-bit unsigned integer (6 bytes)</summary>
        public ulong ReadUInt48()
        {
            try
            {
                return ReadUInt48(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 48-bit unsigned integer (6 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public ulong ReadUInt48(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt48(ReadBytes(6, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 56-bit unsigned integer (7 bytes)</summary>
        public ulong ReadUInt56()
        {
            try
            {
                return ReadUInt56(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 56-bit unsigned integer (7 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public ulong ReadUInt56(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt40(ReadBytes(7, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 64-bit unsigned integer (8 bytes)</summary>
        public ulong ReadUInt64()
        {
            try
            {
                return ReadUInt64(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 64-bit unsigned integer (8 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public ulong ReadUInt64(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt64(ReadBytes(8, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 8-bit unsigned integer (1 bytes)</summary>
        public byte ReadUInt8()
        {
            try
            {
                return _bReader.ReadByte();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Search data for specified string</summary>
        /// <param name="value">The value you are searching for</param>
        /// <param name="type">The string type of the value</param>
        /// <param name="startposition">Position For Start Searching</param>
        public int SearchString(string value, StringType type,long startposition = 0)
        {
            if (startposition > Length)
            {
                throw new Exception("Argument out of Range");
            }
            byte[] searchBuffer = null;
            switch (type)
            {
                case StringType.Ascii:
                    searchBuffer = Encoding.ASCII.GetBytes(value.ToCharArray());
                    break;

                case StringType.Unicode:
                    searchBuffer = IsBigEndian ? Encoding.BigEndianUnicode.GetBytes(value.ToCharArray()) : Encoding.Unicode.GetBytes(value.ToCharArray());
                    break;
            }
            int index;
            for (index = (int)startposition; index < Length; index++)
            {
                if ((searchBuffer != null) && ((index + searchBuffer.Length) >= (Length - 1L)))
                    return -1;
                Position = index;
                if (searchBuffer != null)
                {
                    var objB = ReadBytes(searchBuffer.Length, _isBigEndian);
                    if (!Equals(searchBuffer, objB) && !Equals(searchBuffer, Functions.Functions.EndianSwap(objB)))
                    {
                        continue;
                    }
                }
                return index;
            }
            return -1;
        }

        /// <summary>Search the file stream for a specified Hex String</summary>
        /// <param name="data">The data you want to search for</param>
        /// <param name="firstFind">If you want to only search for the first location</param>
        /// <param name="startPosition">Position For start Searching</param>
        /// <returns>The long offset</returns>
        public long[] SearchHexString(string data, bool firstFind, long startPosition = 0)
        {
            try
            {
                //location of the value/s
                var locations = new List<long>();
                long index;
                for (index = startPosition; index < Length; index++)
                {
                    Position = index;

                    var buffer = ReadBytes(data.Length / 2, _isBigEndian);
                    if (data != Functions.Functions.ByteArrayToHexString(buffer)) continue;
                    locations.Add(index);

                    if (firstFind) break;
                }
                return locations.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Search the file stream for a specified Hex String</summary>
        /// <param name="data">The data you want to search for</param>
        /// <returns>The long offset</returns>
        public long SearchHexString(string data)
        {
            try
            {
                //location of the value/s
                long index;
                for (index = Position; index < Length; index++)
                {
                    Position = index;

                    var buffer = ReadBytes(data.Length / 2, _isBigEndian);
                    if (data != Functions.Functions.ByteArrayToHexString(buffer)) continue;
                    //break if not found
                    break;
                }
                return index;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        #endregion

        #region PeekReader
        /// <summary>Peeks a set size of bytes</summary>
        /// <param name="length">The byte array length</param>
        /// <returns>Byte Array</returns>
        public byte[] PeekBytes(int length)
        {
            return PeekBytes(length, _isBigEndian);
        }

        /// <summary>Peeks a set size of bytes</summary>
        /// <param name="length">The byte array length</param>
        /// <param name="isBigEndien">Specifiy if read is in Big Endien Type</param>
        public byte[] PeekBytes(int length, bool isBigEndien)
        {
            try
            {
                long offset = Position;
                if (Position == Length)
                    throw new Exception("Cannot move position past file size");
                if (length == 0)
                    return new byte[0];
                var buffer = new byte[length];
                _fStream.Read(buffer, 0, length);
                Position = offset;
                if (!isBigEndien)
                    Array.Reverse(buffer);
                return buffer;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 64-bit double (8 bytes)</summary>
        public double PeekDouble()
        {
            try
            {
                return PeekDouble(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 32-bit double (8 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public double PeekDouble(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToDouble(PeekBytes(8, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks Hex String</summary>
        /// <param name="length">The amount of bytes to read</param>
        public string PeekHexString(int length)
        {
            try
            {
                return Functions.Functions.ByteArrayToHexString(PeekBytes(length, _isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 16-bit signed integer (2 bytes)</summary>
        public short PeekInt16()
        {
            try
            {
                return PeekInt16(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 16-bit signed integer (2 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public short PeekInt16(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt16(PeekBytes(2, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 24-bit signed integer (3 bytes)</summary>
        public int PeekInt24()
        {
            try
            {
                return PeekInt24(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 24-bit signed integer (3 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public int PeekInt24(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt24(PeekBytes(3, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 32-bit signed integer (4 bytes)</summary>
        public int PeekInt32()
        {
            try
            {
                return PeekInt32(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 32-bit signed integer (4 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public int PeekInt32(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt32(PeekBytes(4, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 40-bit signed integer (5 bytes)</summary>
        public long PeekInt40()
        {
            try
            {
                return PeekInt40(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 40-bit signed integer (5 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public long PeekInt40(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt40(PeekBytes(5, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 48-bit signed integer (6 bytes)</summary>
        public long PeekInt48()
        {
            try
            {
                return PeekInt48(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 48-bit signed integer (6 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public long PeekInt48(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt48(PeekBytes(6, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 56-bit signed integer (7 bytes)</summary>
        public long PeekInt56()
        {
            try
            {
                return PeekInt56(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 56-bit signed integer (7 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public long PeekInt56(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt56(PeekBytes(7, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 64-bit signed integer (8 bytes)</summary>
        public long PeekInt64()
        {
            try
            {
                return PeekInt64(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 64-bit signed integer (8 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public long PeekInt64(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToInt64(PeekBytes(8, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 8-bit signed integer (1 byte)</summary>
        public sbyte PeekInt8()
        {
            try
            {
                var offset = Position;
                var value = _bReader.ReadSByte();
                Position = offset;
                return value;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 32-bit Float (4 bytes)</summary>
        public float PeekSingle()
        {
            try
            {
                return PeekSingle(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 32-bit signed integer (4 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public float PeekSingle(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToSingle(PeekBytes(4, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks String with specific string type</summary>
        /// <param name="stringType">The Specified String type to read</param>
        /// <param name="length">The byte length of the String to read</param>
        public string PeekString(StringType stringType, int length)
        {
            if (!Enum.IsDefined(typeof(StringType), stringType))
            {
                throw new Exception("Invalid parameters");
            }
            if (Position >= (Length - 1L))
            {
                return "";
            }
            var bytes = PeekBytes(length, _isBigEndian);
            if (stringType != StringType.Unicode)
            {
                return Encoding.ASCII.GetString(bytes);
            }
            return !IsBigEndian ? Encoding.Unicode.GetString(bytes) : Encoding.BigEndianUnicode.GetString(bytes);
        }

        /// <summary>Peeks a 16-bit unsigned integer (2 bytes)</summary>
        public ushort PeekUInt16()
        {
            try
            {
                return PeekUInt16(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 16-bit unsigned integer (2 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public ushort PeekUInt16(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt16(PeekBytes(2, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 24-bit unsigned integer (3 bytes)</summary>
        public uint PeekUInt24()
        {
            try
            {
                return PeekUInt24(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 24-bit unsigned integer (3 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public uint PeekUInt24(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt24(PeekBytes(3, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 32-bit unsigned integer (4 bytes)</summary>
        public uint PeekUInt32()
        {
            try
            {
                return PeekUInt32(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 32-bit unsigned integer (4 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public uint PeekUInt32(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt32(PeekBytes(4, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 40-bit unsigned integer (5 bytes)</summary>
        public ulong PeekUInt40()
        {
            try
            {
                return PeekUInt40(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 40-bit unsigned integer (5 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public ulong PeekUInt40(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt40(PeekBytes(5, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 48-bit unsigned integer (6 bytes)</summary>
        public ulong PeekUInt48()
        {
            try
            {
                return PeekUInt48(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 48-bit unsigned integer (6 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public ulong PeekUInt48(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt48(PeekBytes(6, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 56-bit unsigned integer (7 bytes)</summary>
        public ulong PeekUInt56()
        {
            try
            {
                return PeekUInt56(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 56-bit unsigned integer (7 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public ulong PeekUInt56(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt40(PeekBytes(7, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 64-bit unsigned integer (8 bytes)</summary>
        public ulong PeekUInt64()
        {
            try
            {
                return PeekUInt64(_isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 64-bit unsigned integer (8 bytes)</summary>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public ulong PeekUInt64(bool isBigEndian)
        {
            try
            {
                return Functions.Functions.ByteArrayToUInt64(PeekBytes(8, isBigEndian));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Peeks a 8-bit unsigned integer (1 bytes)</summary>
        public byte PeekUInt8()
        {
            try
            {
                var offset = Position;
                var value = _bReader.ReadByte();
                Position = offset;
                return value;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        #endregion

        #region Methods
        /// <summary>Clears buffer by Flushing and Closes theI/O Stream</summary>
        public void Close()
        {
            try
            {
                if (_accessed)
                {
                    Flush();
                    _bReader.Close();
                    _bReader = null;
                    _fStream.Close();
                    _fStream = null;
                    _accessed = false;
                    Dispose();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Release all resources used by reader and writer stream</summary>
        public void Dispose()
        {
            try
            {
                if (_accessed && (_bReader != null))
                {
                    _bReader.BaseStream.Dispose();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        
        /// <summary>Clears buffer for this stream and any unbuffered data will be written</summary>
        public void Flush()
        {
            try
            {
                _bReader.BaseStream.Flush();
                _fStream.Flush();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        #endregion

        #region properties
        /// <summary>Set if current initialization is being accessed</summary>
        public bool Accessed
        {
            get
            {
                return _accessed;
            }
        }

        /// <summary>Set/Get if current initialization is big endian</summary>
        public bool IsBigEndian
        {
            get
            {
                return _isBigEndian;
            }
            set
            {
                _isBigEndian = value;
            }
        }

        /// <summary>Get last position accessed</summary>
        public long LastPosition
        {
            get
            {
                try
                {
                    return _lastPosition;
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message);
                }
            }
        }

        /// <summary>Get/Set if current initialization length</summary>
        public long Length
        {
            get
            {
                try
                {
                    return _fStream.Length;
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message);
                }
            }
            set
            {
                try
                {
                    _fStream.SetLength(value);
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message);
                }
            }
        }

        /// <summary>Get/Set if current initialization position</summary>
        public long Position
        {
            get
            {
                return _fStream.Position;
            }
            set
            {
                try
                {
                    if ((value != _fStream.Position) || (value <= _fStream.Length))
                    {
                        _lastPosition = _fStream.Position;
                        _fStream.Position = value;
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(exception.Message);
                }
            }
        }
        #endregion
    }
}

