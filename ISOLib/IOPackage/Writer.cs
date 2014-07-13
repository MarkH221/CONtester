using System;
using System.IO;
using System.Text;

namespace Isolib.IOPackage
{
    /// <summary>Contains function/s that deals with I/O writing of data</summary>
    public class Writer : IDisposable
    {
        private bool _accessed;
        private BinaryWriter _bWriter;
        private string _fileName;
        private Stream _fStream;
        private bool _isBigEndian;
        private long _lastPosition;

        #region Constructors
        /// <summary>Makes a temporary file Stream</summary>
        public Writer()
        {
             try
             {
                 _fileName = Path.GetTempPath() + Guid.NewGuid() + ".ISOLib";
                 _fStream = File.Create(_fileName);
                 _bWriter = new BinaryWriter(_fStream);
                 _isBigEndian = true;
                 _accessed = true;
             }
             catch (Exception e)
             {
                 throw new Exception(e.Message);
             }
        }

        /// <param name="path">The file Path</param>
        /// <param name="isBigEndian">Endian Type, Default is true</param>
        public void Write(String path,bool isBigEndian)
        {
            if (!(File.Exists(path)))
            {
                throw new Exception("File does not exist!");
            }
            _fStream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None);
            _bWriter = new BinaryWriter(_fStream);
            _isBigEndian = isBigEndian;
            _accessed = true;
        }

        /// <summary>Initialize data stream viaI/O Stream</summary>
        /// <param name="stream">TheI/O Stream</param>
        public Writer(Stream stream)
        {
            if (!(stream.CanWrite))
            {
                throw new Exception("Stream does not have write access!");
            }
            if (!(stream.CanRead))
            {
                throw new Exception("Stream does not have read access!");
            }
            _fStream = stream;
            _bWriter = new BinaryWriter(_fStream);
            _isBigEndian = true;
            _accessed = true;
        }
        
        /// <summary>Initialize Array Streaming</summary>
        /// <param name="byteArray">The Byte Array to Stream</param>
        public Writer(Byte[] byteArray)
        {
            try
            {
                _fStream = new MemoryStream(byteArray, true);
                _bWriter = new BinaryWriter(_fStream);
                _isBigEndian = true;
                _accessed = true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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

        /// <summary>Release all resources used by reader and writer stream</summary>
        public void Dispose()
        {
            try
            {
                if (_accessed && (_bWriter != null))
                {
                    _bWriter.BaseStream.Dispose();
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
                _bWriter.BaseStream.Flush();
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
            private get
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

