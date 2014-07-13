using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace Isolib.IOPackage
{
    /// <summary>Contains function/s that deals with I/O reading and writing of Data</summary>
    public class RWStream :IDisposable
    {
        private bool _accessed;
        private BinaryReader _bReader;
        private BinaryWriter _bWriter;
        private string _fileName;
        private Stream _fStream;
        private bool _isBigEndian;
        private long _lastPosition;

        #region RwStream Constructors
        #region Main Contructor
        /// <summary>Initialize File Reader and Writer by specifying file a valid file path</summary>
        /// <param name="path">The file Path</param>
        public RWStream(String path)
        {
            ReadWriter(path, true, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>Initialize File Reader and Writer by specifying file a valid file path</summary>
        /// <param name="path">The file Path</param>
        /// <param name="isBigEndian">Endian Type, Default is true</param>
        public RWStream(String path, Boolean isBigEndian)
        {
            ReadWriter(path, isBigEndian, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>Initialize File Reader and Writer by specifying file a valid file path</summary>
        /// <param name="path">The file Path</param>
        /// <param name="isBigEndian">Endian Type, Default is true</param>
        /// <param name="fileMode">The File Mode</param>
        public RWStream(String path, Boolean isBigEndian, FileMode fileMode)
        {
            ReadWriter(path, isBigEndian, fileMode, FileAccess.ReadWrite, FileShare.None);
        }

        /// <summary>Initialize File Reader and Writer by specifying file a valid file path</summary>
        /// <param name="path">The file Path</param>
        /// <param name="isBigEndian">Endian Type, Default is true</param>
        /// <param name="fileMode">The File Mode</param>
        /// <param name="fileAccess">The File Access Option</param>
        public RWStream(String path, Boolean isBigEndian, FileMode fileMode, FileAccess fileAccess)
        {
            ReadWriter(path, isBigEndian, fileMode, fileAccess, FileShare.None);
        }

        /// <summary>Initialize File Reader and Writer by specifying file a valid file path</summary>
        /// <param name="path">The file Path</param>
        /// <param name="isBigEndian">Endian Type, Default is true</param>
        /// <param name="fileMode">The File Mode</param>
        /// <param name="fileAccess">The File Access Option</param>
        /// <param name="fileShare">The File Share Option</param>
        public RWStream(String path, Boolean isBigEndian, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            ReadWriter(path, isBigEndian, fileMode, fileAccess, fileShare);
        }

        /// <summary>Initialize File Reader and Writer by specifying file a valid file path</summary>
        /// <param name="path">The file Path</param>
        /// <param name="isBigEndian">Endian Type, Default is true</param>
        /// <param name="theFileMode">The File Mode</param>
        /// <param name="theFileAccess">The File Access Option</param>
        /// <param name="theFileShare">The File Share Option</param>
        private void ReadWriter(String path, Boolean isBigEndian, FileMode theFileMode, FileAccess theFileAccess, FileShare theFileShare)
        {
            if (!(File.Exists(path)))
            {
                throw new Exception("File does not exist!");
            }
            _fStream = new FileStream(path, theFileMode, theFileAccess, theFileShare);
            _bReader = new BinaryReader(_fStream);
            _bWriter = new BinaryWriter(_fStream);
            _isBigEndian = isBigEndian;
            _accessed = true;
        }
        #endregion

        #region Byte Array to Stream
        /// <summary>Initialize Array Streaming</summary>
        /// <param name="buffer">The Byte Array to Stream</param>
        public RWStream(Byte[] buffer)
        {
            ReadWriteBuffer(buffer, true, true);
        }

        /// <summary>Initialize Array Streaming</summary>
        /// <param name="buffer">The Byte Array to Stream</param>
        /// <param name="isBigEndian">Endian Type - Big Endian by Default</param>
        public RWStream(Byte[] buffer, Boolean isBigEndian)
        {
            ReadWriteBuffer(buffer, true, isBigEndian);
        }

        /// <param name="buffer">The Byte Array to Stream</param>
        /// <param name="writeable">If The Stream supports writing - True by Default</param>
        /// <param name="isBigEndian">Endian Type - Big Endian by Default</param>
        public RWStream(Byte[] buffer, Boolean isBigEndian, Boolean writeable)
        {
            ReadWriteBuffer(buffer, writeable, isBigEndian);
        }

        /// <summary>Initialize Array Streaming</summary>
        /// <param name="byteArray">The Byte Array to Stream</param>
        /// <param name="writeAble">If The Stream supports writing - True by Default</param>
        /// <param name="isBigEndian">Endian Type - Big Endian by Default</param>
        private void ReadWriteBuffer(Byte[] byteArray, Boolean writeAble, Boolean isBigEndian)
        {
            try
            {
                _fStream = new MemoryStream(byteArray, writeAble);
                _bReader = new BinaryReader(_fStream);
                _bWriter = new BinaryWriter(_fStream);
                _isBigEndian = isBigEndian;
                _accessed = true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        /// <summary>Makes a temporary file Stream</summary>
        public RWStream()
        {
            try
            {
                _fileName = Path.GetTempPath() + Guid.NewGuid() + ".ISOLib";
                _fStream = new FileStream(_fileName,FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                _bReader = new BinaryReader(_fStream);
                _bWriter = new BinaryWriter(_fStream);
                _isBigEndian = true;
                _accessed = true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>Initialize data stream via I/O Stream</summary>
        /// <param name="stream">TheI/O Stream</param>
        public RWStream(Stream stream)
        {
            if (!(stream.CanWrite))
                throw new Exception("Stream does not have write access!");
            if (!(stream.CanRead))
                throw new Exception("Stream does not have read access!");
            _fStream = stream;
            _bReader = new BinaryReader(_fStream);
            _bWriter = new BinaryWriter(_fStream);
            _isBigEndian = true;
            _accessed = true;
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
                    _bWriter.Close();
                    _bWriter = null;
                    _bReader.Close();
                    _bReader = null;
                    _fStream.Close();
                    _fStream = null;
                    _accessed = false;
                    if (_fileName != null)
                    {
                        File.Delete(_fileName);
                        _fileName = null;
                    }
                    Dispose();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        


        /// <summary>
        /// Compatibility overload for callers compiled against ISOLib 0.2.2.
        /// The parameter is accepted for source compatibility; temporary files
        /// are handled exactly as in <see cref="Close()"/>.
        /// </summary>
        public void Close(bool deleteTemporaryFile)
        {
            Close();
        }

        /// <summary>Clears buffer for this stream and any unbuffered data will be written</summary>
        public void Flush()
        {
            try
            {
                _bReader.BaseStream.Flush();
                _bWriter.BaseStream.Flush();
                _fStream.Flush();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
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
                if(!isBigEndien)
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
                for (index = 0L; index < buffer.Length; index ++)
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
        public int SearchString(string value, StringType type)
        {
            if (Position > Length)
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
            for (index = (int)Position; index < Length; index++)
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
        /// <returns>The long offset</returns>
        public long[] SearchHexString(string data, bool firstFind)
        {
            try
            {
                //location of the value/s
                var locations = new List<long>();
                long index;
                for (index = Position; index < Length; index++)
                {
                    Position = index;

                    var buffer = ReadBytes(data.Length/2, _isBigEndian);
                    if (data != Functions.Functions.ByteArrayToHexString(buffer)) continue;
                    locations.Add(index);

                    if (firstFind)break;
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
        /// <param name="startoffset">The starting offset </param>
        /// <param name="firstFind">If you want to only search for the first location</param>
        /// <returns>The long offset</returns>
        public long[] SearchHexString(string data,long startoffset, bool firstFind)
        {
            try
            {
                //location of the value/s
                var locations = new List<long>();
                long index;
                for (index = startoffset; index < Length; index++)
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

                    var buffer = ReadBytes(data.Length / 2,_isBigEndian);
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

        #region Writer
        /// <summary>Writes a byte array to the underlying stream</summary>
        /// <param name="buffer">A byte array containing the data to write</param>
        public void WriteBytes(byte[] buffer)
        {
            try
            {
                WriteBytes(buffer, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a byte array to the underlying stream</summary>
        /// <param name="buffer">A byte array containing the data to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteBytes(byte[] buffer, bool isBigEndian)
        {
            try
            {
                if (!isBigEndian)
                    Array.Reverse(buffer);
                _bWriter.Write(buffer);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a region of a byte array to the current stream</summary>
        /// <param name="buffer">A byte array containing the data to write</param>
        /// <param name="index">The starting point to start writing</param>
        /// <param name="count">The amount of bytes to write</param>
        public void WriteBytes(byte[] buffer, int index, int count)
        {
            try
            {
                WriteBytes(buffer, index, count, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a region of a byte array to the current stream</summary>
        /// <param name="buffer">A byte array containing the data to write</param>
        /// <param name="index">The starting point to start writing</param>
        /// <param name="count">The amount of bytes to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteBytes(byte[] buffer, int index, int count, bool isBigEndian)
        {
            try
            {
                if (!isBigEndian)
                    Array.Reverse(buffer);
                _bWriter.Write(buffer, index, count);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 64-bit double (8 bytes)</summary>
        public void WriteDouble(double value)
        {
            try
            {
                WriteDouble(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 32-bit double (8 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteDouble(double value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.DoubleToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes Hex String</summary>
        /// <param name="value">The hex string value to write</param>
        public void WriteHexString(string value)
        {
            try
            {
                WriteBytes(Functions.Functions.HexToBytes(value));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes Hex String</summary>
        /// <param name="value">The hex string value to write</param>
        /// <param name="offset">The offset / index you want to write the hex string</param>
        public void WriteHexString(string value, int offset)
        {
            try
            {
                var buffer = Functions.Functions.HexToBytes(value);
                WriteBytes(buffer, offset, buffer.Length);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 16-bit signed integer (2 bytes)</summary>
        public void WriteInt16(short value)
        {
            try
            {
                WriteInt16(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 16-bit signed integer (2 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteInt16(short value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.Int16ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 24-bit signed integer (3 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteInt24(int value)
        {
            try
            {
                WriteInt24(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 24-bit signed integer (3 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteInt24(int value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.Int24ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 32-bit signed integer (4 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteInt32(int value)
        {
            try
            {
                WriteInt32(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 32-bit signed integer (4 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteInt32(int value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.Int32ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 40-bit signed integer (5 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteInt40(long value)
        {
            try
            {
                WriteInt40(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 40-bit signed integer (5 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteInt40(long value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.Int40ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 48-bit signed integer (6 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteInt48(long value)
        {
            try
            {
                WriteInt48(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 48-bit signed integer (6 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteInt48(long value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.Int48ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 56-bit signed integer (7 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteInt56(long value)
        {
            try
            {
                WriteInt56(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 56-bit signed integer (7 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteInt56(long value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.Int56ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 64-bit signed integer (8 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteInt64(long value)
        {
            try
            {
                WriteInt64(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 64-bit signed integer (8 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteInt64(long value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.Int64ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Write an 8-bit signed integer (1 byte)</summary>
        /// <param name="value">The value to write</param>
        public void WriteInt8(sbyte value)
        {
            try
            {
                _bWriter.Write(value);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads a 32-bit signed integer (4 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteSingle(float value)
        {
            try
            {
                var buffer = Functions.Functions.SingleToBytesArray(value);
                WriteBytes(buffer);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Reads String with specific string type</summary>
        /// <param name="input">The value to write</param>
        /// <param name="type">The Specified String type to read</param>
        /// <param name="length">The byte length of the String to read</param>
        public void WriteString(string input, StringType type, int length)
        {
            if (!Enum.IsDefined(typeof(StringType), type))
            {
                throw new Exception("Invalid Parameters");
            }
            try
            {
                input = input.PadRight(length, '\0');
                switch (type)
                {
                    case StringType.Ascii:
                        WriteBytes(Encoding.ASCII.GetBytes(input.ToCharArray()));
                        return;

                    case StringType.Unicode:
                        WriteBytes(IsBigEndian ? Encoding.BigEndianUnicode.GetBytes(input.ToCharArray()) : Encoding.Unicode.GetBytes(input.ToCharArray()));
                        return;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 16-bit unsigned integer (2 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteUInt16(ushort value)
        {
            try
            {
                WriteUInt16(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 16-bit unsigned integer (2 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteUInt16(ushort value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.UInt16ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 24-bit unsigned integer (3 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteUInt24(uint value)
        {
            try
            {
                WriteUInt24(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 24-bit unsigned integer (3 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteUInt24(uint value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.UInt24ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 32-bit unsigned integer (4 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteUInt32(uint value)
        {
            try
            {
                WriteUInt32(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 32-bit unsigned integer (4 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteUInt32(uint value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.UInt32ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 40-bit unsigned integer (5 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteUInt40(ulong value)
        {
            try
            {
                WriteUInt40(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 40-bit unsigned integer (5 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteUInt40(ulong value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.UInt40ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 48-bit unsigned integer (6 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteUInt48(ulong value)
        {
            try
            {
                WriteUInt48(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 48-bit unsigned integer (6 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteUInt48(ulong value, bool isBigEndian)
        {
            try
            {
                byte[] buffer = Functions.Functions.UInt48ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 56-bit unsigned integer (7 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteUInt56(ulong value)
        {
            try
            {
                WriteUInt56(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 56-bit unsigned integer (7 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteUInt56(ulong value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.UInt56ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 64-bit unsigned integer (8 bytes)</summary>
        /// <param name="value">The value to write</param>
        public void WriteUInt64(ulong value)
        {
            try
            {
                WriteUInt64(value, _isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 64-bit unsigned integer (8 bytes)</summary>
        /// <param name="value">The value to write</param>
        /// <param name="isBigEndian">If the Endien type is Big</param>
        public void WriteUInt64(ulong value, bool isBigEndian)
        {
            try
            {
                var buffer = Functions.Functions.UInt64ToBytesArray(value);
                WriteBytes(buffer, isBigEndian);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Writes a 8-bit unsigned integer (1 byte)</summary>
        /// <param name="value">The value to write</param>
        public void WriteUInt8(byte value)
        {
            try
            {
                _bWriter.Write(value);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        #endregion
        
        #region Properties
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
                return _lastPosition;
            }
        }

        /// <summary>Get/Set if current initialization length</summary>
        public long Length
        {
            get
            {
                return _fStream.Length;
            }
            set
            {
                _fStream.SetLength(value);
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

        /// <summary>Release all resources used by reader and writer stream</summary>
        public void Dispose()
        {
            try
            {
                if (_accessed)
                {
                    if (_bReader != null)
                    {
                        _bReader.BaseStream.Dispose();
                    }
                    if (_bWriter != null)
                    {
                        _bWriter.BaseStream.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}

