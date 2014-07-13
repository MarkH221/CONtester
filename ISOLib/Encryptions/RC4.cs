using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualBasic;


namespace Isolib.Encryptions
{
    /// <summary>
    /// RC4 Encryption Class
    /// </summary>
    public class RC4 : IDisposable, ICloneable
    {
        private UInt32[] Table;
        private byte[] xKey;
        private Int32 a;

        private Int32 b;
        /// <summary>
        /// Inititialize new RC4
        /// </summary>
        public RC4()
            : base()
        {
            xKey = null;
        }
        /// <summary>
        ///  Inititialize new RC4 with Key
        /// </summary>
        /// <param name="Key">Encryption Key</param>
        public RC4(ref byte[] Key)
            : base()
        {
            this.Key = Key;
        }

        private void GenerateTable()
        {
            if (xKey == null || xKey.Length == 0)
                throw new CryptographicException("Invalid key");
            if (Table != null)
                Table = null;
            Table = new UInt32[256];
            for (Int32 x = 0; x <= 256 - 1; x++)
            {
                Table[x] = Convert.ToUInt32(x);
            }
            UInt32 Index = default(UInt32);
            UInt32 EndianSwap = default(UInt32);
            for (Int32 x = 0; x <= 256 - 1; x++)
            {
                Index = Convert.ToUInt32((((Index + Table[x]) + xKey[(x % xKey.Length)]) % 256));
                EndianSwap = Table[Convert.ToInt32(Index)];
                Table[Convert.ToInt32(Index)] = Table[x];
                Table[x] = EndianSwap;
            }
        }

        private void Core(ref byte[] Data, int Index, int Count)
        {
            if (Data == null || Data.Length == 0 || Index > Data.Length || (Index + Count) > Data.Length)
                throw new ArgumentException("Invalid arguments");
            UInt32 x = 0;
            for (int i = Index; i <= Count - 1; i++)
            {
                a = ((a + 1) % 256);
                b = Convert.ToInt32(((b + Table[a]) % 256));
                x = Table[a];
                Table[a] = Table[b];
                Table[b] = x;
                Data[i] = Convert.ToByte((Data[i] ^ Table[Convert.ToInt32(((Table[a] + Table[b]) % 256))]));
            }
        }

        /// <summary>
        /// get/set encrytion key
        /// </summary>
        public byte[] Key
        {
            get { return xKey; }
            set
            {
                if (value == null || value.Length == 0)
                    throw new CryptographicException("Invalid key");
                if (xKey != null)
                    xKey = null;
                xKey = new byte[value.Length];
                Array.Copy(value, 0, xKey, 0, value.Length);
                GenerateTable();
                value = null;
            }
        }

        /// <summary>
        /// Transforms Data 
        /// </summary>
        /// <param name="Data">data to proccess</param>
        /// <param name="Index">start index</param>
        /// <param name="Count">Length</param>
        public void TransformBlock(ref byte[] Data, int Index, int Count)
        {
            Core(ref Data, Index, Count);
        }
        /// <summary>
        /// Transforms Data 
        /// </summary>
        /// <param name="Data">data to proccess</param>
        /// <param name="Count">Length </param>
        public void TransformBlock(ref byte[] Data, int Count)
        {
            TransformBlock(ref Data, 0, Count);
        }
        /// <summary>
        /// Transforms Data 
        /// </summary>
        /// <param name="Data">Data To Proccess</param>
        public void TransformBlock(ref byte[] Data)
        {
            TransformBlock(ref Data, 0, Data.Length);
        }
        /// <summary>
        /// Transforms Final Data 
        /// </summary>
        /// <param name="Data">data to proccess</param>
        /// <param name="Index">start index</param>
        /// <param name="Count">Length</param>
        public void TransformFinalBlock(ref byte[] Data, int Index, int Count)
        {
            Core(ref Data, Index, Count);
       }
        /// <summary>
        /// Transforms Final Data 
        /// </summary>
        /// <param name="Data">data to proccess</param>
        /// <param name="Count">Length</param>
        public void TransformFinalBlock(ref byte[] Data, int Count)
        {
            TransformFinalBlock(ref Data, 0, Count);
        }
        /// <summary>
        /// Transforms Final Data 
        /// </summary>
        /// <param name="Data">data to proccess</param>
         public void TransformFinalBlock(ref byte[] Data)
        {
            TransformFinalBlock(ref Data, 0, Data.Length);
        }

        /// <summary>
        /// Reset handles
        /// </summary>
        public void Reset()
        {
            a = 0;
            b = 0;
            GenerateTable();
        }
        /// <summary>
        /// Clone RC4 Instance
        /// </summary>
        /// <returns>Deserialized stream</returns>
        public object Clone()
        {
            BinaryFormatter Formatter = new BinaryFormatter();
            using (MemoryStream Stream = new MemoryStream())
            {
                Formatter.Serialize(Stream, this);
                Stream.Position = 0;
                return Formatter.Deserialize(Stream);
            }
        }
        /// <summary>
        /// Dispose Class
        /// </summary>
        public void Dispose()
        {
            Table = null;
            xKey = null;
            GC.Collect(GC.GetGeneration(this), 0);
        }
    }
}
