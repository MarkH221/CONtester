namespace Isolib.STFSPackage
{
    internal class BlockRecord
    {
        private readonly byte[] _block;
        private byte[] _flags;
        
        #region constructor
        /// <summary>Initialize Block Records</summary>
        public BlockRecord()
        {
            _block = new byte[3];
            _flags = new byte[4];
        }

        /// <summary>Initialize Block Records</summary>
        public BlockRecord(uint flag)
        {
            _block = new byte[3];
            Flags = flag;
        }
        #endregion

        #region properties
        /// <summary>Set and get Free Blocks</summary>
        public int BlocksFree
        {
            get
            {
                return (((int) (Flags >> 15)) & 0x7fff);
            }
        }

        /// <summary>Get and Set Flags</summary>
        private uint Flags
        {
            get
            {
                return (uint) ((((_flags[0] << 0x18) | (_flags[1] << 0x10)) | (_flags[2] << 8)) | _flags[3]);
            }
            set
            {
                _flags = Functions.Functions.UInt32ToBytesArray(value);
            }
        }

        /// <summary>Get and Set Index</summary>
        public byte Index
        {
            get
            {
                return (byte) (Indicator & 1);
            }
        }

        /// <summary>Get and Set Indicator</summary>
        private byte Indicator
        {
            get
            {
                return (byte) (_flags[0] >> 6);
            }
        }

        /// <summary>Get and Set Next Block</summary>
        public uint NextBlock
        {
            get
            {
                return (uint) (((_flags[1] << 0x10) | (_flags[2] << 8)) | _flags[3]);
            }
        }

        /// <summary>Get and Set Current Block</summary>
        public uint CurrentBlock
        {
            get
            {
                return (uint) (((_block[0] << 0x10) | (_block[1] << 8)) | _block[2]);
            }
            set
            {
                _block[0] = (byte) ((value >> 0x10) & 0xff);
                _block[1] = (byte) ((value >> 8) & 0xff);
                _block[2] = (byte) (value & 0xff);
            }
        }
        #endregion
    }
}

