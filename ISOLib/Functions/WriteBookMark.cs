using System;
using System.Text;
using System.IO;

namespace Isolib.Functions
{
    /// <summary>Write Hex Workshop Bookmarks</summary>
    public class WriteBookmark : IDisposable
    {
        private readonly MemoryStream _stream;
        private UInt32 _count;
        private readonly string _name;
        private readonly string _path;
        private bool _disposed;

        /// <summary>Write Hex Workshop Bookmarks constructor</summary>
        /// <param name="bookMarkName">The name of the bookmark</param>
        /// <param name="outpath">The output path</param>
        public WriteBookmark(string bookMarkName, string outpath)
        {
            _name = bookMarkName;
            _path = outpath;
            _count = 0;
            _disposed = false;
            _stream = new MemoryStream();
            var buffer = new byte[0x1C0];
            Array.Copy(new byte[] {0x0,0xff,0xB4,0x0,0x2}, 0, buffer, 0, 5);
            Array.Copy(new byte[] {0x36,0x76,0xb5,0xf2}, 0, buffer, buffer.Length - 4, 4);
            _stream.Write(buffer, 0, buffer.Length);
        }

        private byte[] Write(string name, string offset, string length, string count)
        {
            var buffer = new byte[0x290];
            using (var writer = new BinaryWriter(new MemoryStream(buffer)))
            {
                writer.Write(Encoding.ASCII.GetBytes(name));
                writer.BaseStream.Position = 0x50;
                writer.Write(Encoding.ASCII.GetBytes(offset));
                writer.BaseStream.Position = 0xd0;
                writer.Write(Encoding.ASCII.GetBytes(length));
                writer.BaseStream.Position = 0x150;
                writer.Write(Encoding.ASCII.GetBytes(count));
                writer.BaseStream.Position = 0x1d4;
                writer.Write(new byte[] {1,1,1});
                writer.BaseStream.Position = 0x28c;
                writer.Write(new byte[] { 3 });
                writer.Flush(); writer.Close();
              }
            return buffer;
        }

        /// <summary>Write bookmarks sections</summary>
        /// <param name="name">The name of section</param>
        /// <param name="offset">The offset of the section</param>
        /// <param name="length">The length of the section</param>
        /// <param name="count">Total entries</param>
        /// <exception cref="Exception">Stream has been Closed execption</exception>
        public void Write(string name, UInt32 offset, UInt32 length, UInt32 count = 1)
        {
            if (_disposed)
                throw new Exception("Stream has been Closed!");
            byte[] a, b,c = null;
            a = BitConverter.GetBytes(offset);
            b = BitConverter.GetBytes(length);
           c = BitConverter.GetBytes(count);
           Array.Reverse(a); Array.Reverse(b); Array.Reverse(c);
            byte[] buffer = Write(name, BitConverter.ToString(a).Replace("-", "").PadLeft(8, Convert.ToChar("0")), BitConverter.ToString(b).Replace("-", "").PadLeft(8, Convert.ToChar("0")), BitConverter.ToString(c).Replace("-", "").PadLeft(8, Convert.ToChar("0")));
            _stream.Write(buffer, 0, buffer.Length);
            _count = Convert.ToUInt32(_count + 1);
            a = null; b = null; c = null;
        }

        /// <summary>Close the bookmark</summary>
        public void Close()
        {
            if (_disposed)
                return;
            var x = _path + "\\" + _name + ".hbk";
            byte[] buffer = _stream.ToArray();
            File.WriteAllBytes(x,buffer);
            _stream.Dispose();
            _stream.Close();
            using (var writer = new BinaryWriter(new FileStream(x, FileMode.Open)))
            {
                writer.BaseStream.Position = 0xac;
                writer.Write(BitConverter.GetBytes(_count));
                writer.Close();
            }
            Dispose();
        }

        /// <summary>Dispose object</summary>
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this), 0);
            _disposed = true;
        }
    }
}
