using System;
using System.Collections.Generic;
using Isolib.IOPackage;

namespace Isolib.STFSPackage
{
    /// <summary>Contains function/s that deals with STFS volume</summary>
    /// <seealso cref="HeaderData"/>
    public sealed class Descriptor
    {
        private readonly byte _baseByte;
        private byte _shift;
        private readonly uint[] _spaceBetween = new uint[3]; //Depend on the volume type
        private readonly byte[] _fileTableBlock = new byte[5]; // This contains 2 byte for File Table Block Count and 3 Byte for number
        private readonly uint _totalAllocatedBlockCount;
        private readonly BlockRecord _blockRecord = new BlockRecord();

        /// <summary>Working with the file system</summary>
        /// <param name="mainIo"> </param>
        internal Descriptor(RWStream mainIo)
        {
            //Reading the headersize
            mainIo.Position = 0x340;
            var headerSize = mainIo.ReadInt32();
            _baseByte = (byte)(((headerSize + 0xFFF) & 0xF000) >> 12);

            //IMPORTANT====================================================//
            //The volume descriptor block starts at 0x379 with 0x24 length // 
            //=============================================================//
            //EXAMPLE: offset 0x00 in the descriptor block = 0x379 in the file
            //Top hash table hash 0x381 and 0x14 in length

            mainIo.Position = 0x379;//position of the volume descriptor size
            //The size has to be 0x24
            if (mainIo.ReadUInt8() != 0x24)
                throw new Exception("Invalid STFS format");
            //The Reserved has to be 0
            if (mainIo.ReadUInt8() != 0)
                throw new Exception("Invalid STFS format");

            //Compute the bitwis AND of the operand
            var blockSeperation = (byte)(mainIo.ReadUInt8() & 3);

            //The value at 37E - _fileTableBlockNumber[2] determines where the embedded file table begins
            _fileTableBlock = mainIo.ReadBytes(5);

            //Getting the total allocated block count
            mainIo.Position = 0x395L;
            _totalAllocatedBlockCount = mainIo.ReadUInt32();

            var totalUnallocatedBlockCount = mainIo.ReadUInt32();

            //The base byte determines the structure type
            switch (_baseByte)
            {
                case 10:
                    if ((blockSeperation != 0) && (blockSeperation != 2))
                        throw new Exception("Invalid STFS format");
                    SetStructure(StructureType.Type1);
                    break;

                case 11:
                    if (blockSeperation != 1)
                        throw new Exception("Invalid STFS format");
                    SetStructure(StructureType.Type0);
                    break;

                default:
                    throw new Exception("Invalid STFS format");
            }

            //space between is determined by the volume descriptor structure type
            if (_totalAllocatedBlockCount > _spaceBetween[2])
                throw new Exception("Max Block Count Invalid");

            _blockRecord = new BlockRecord(((uint) (((blockSeperation >> 1) & 1) << 30)) | (totalUnallocatedBlockCount << 15));
            uint index;
            for (index = _totalAllocatedBlockCount - 1; index >= 0; index--)
            {
                _totalAllocatedBlockCount = index + 1;
                if (GenerateDataOffset(index) < mainIo.Length) return;
            }
        }

        #region Methods
        /// <summary>Get a block offset using the specified block</summary>
        /// <param name="block">The block to use</param>
        private long BlockToOffset(uint block)
        {
            try
            {
                return (block * 0x1000) + BaseBlock;
            }
            catch (Exception)
            {
                throw new Exception("Unknown Error!");
            }
        }

        /// <summary>Generates a hash base</summary>
        /// <param name="block">The block value</param>
        /// <param name="tree">The tree level</param>
        internal long GenerateBaseOffset(uint block, TreeLevel tree)
        {
            try
            {
                return BlockToOffset(GenerateHashBlock(block, tree));
            }
            catch
            {
                throw new Exception("Unknown error occurred");
            }
        }

        /// <summary>Generates an embedded file block offset</summary>
        /// <param name="block">The block value to use</param>
        private uint GenerateDataBlock(uint block)
        {
            if (block >= 0x4AF768)
            {
                return 0xFFFFFF;
            }
            try
            {
                var dataBlock = (((block / BlockLevel[0]) + 1) << (_shift & 0x1F)) + block;
                if (block < BlockLevel[0])
                    return dataBlock;
                dataBlock += ((block / BlockLevel[1]) + 1) << (_shift & 0x1F);
                if (block < BlockLevel[1])
                    return dataBlock;
                return dataBlock + (((uint) 1) << _shift);
            }
            catch (Exception)
            {
                throw new Exception("Unknown Error!");
            }
        }

        /// <summary>Generates a data offset</summary>
        /// <param name="block">The block value</param>
        internal long GenerateDataOffset(uint block)
        {
            try
            {
                return (block >= 0x4AF768) ? 0xFFFFFFL : BlockToOffset(GenerateDataBlock(block));
            }
            catch (Exception)
            {
                throw new Exception("Unknown Error!");
            }
        }

        /// <summary>Generates a hash Block</summary>
        /// <param name="block">The block value</param>
        /// <param name="tree">The tree level</param>
        private uint GenerateHashBlock(uint block, TreeLevel tree)
        {
            if (block >= 0x4AF768) return 0xFFFFFF;
            try
            {
                switch (tree)
                {
                    case TreeLevel.L0:
                    {
                        var hashBlock = (block / BlockLevel[0]) * _spaceBetween[0];
                        if (block >= BlockLevel[0])
                        {
                            hashBlock += ((block / BlockLevel[1]) + 1) << (_shift & 0x1F);
                            if (block >= BlockLevel[1]) hashBlock += ((uint)1) << _shift;
                        }
                        return hashBlock;
                    }
                    case TreeLevel.L1:
                        if (block >= BlockLevel[1])
                        {
                            return ((SpaceBetween[1] * (block / BlockLevel[1])) + (((uint) 1) << _shift));
                        }
                        return _spaceBetween[0];
                    case TreeLevel.L2:
                        return SpaceBetween[1];
                }
                return 0xFFFFFF;
            }
            catch
            {
                throw new Exception("Unknown error occurred");
            }
        }

        /// <summary>Generates a hash offset</summary>
        /// <param name="block">The block value</param>
        /// <param name="tree">The tree level</param>
        internal long GenerateHashOffset(uint block, TreeLevel tree)
        {
            if (block >= 0x4AF768) return 0xFFFFFFL;
            try
            {
                var generatedHashBlock = GenerateHashBlock(block, tree);
                var offset = BlockToOffset(generatedHashBlock);
                switch (tree)
                {
                    case TreeLevel.L0:
                        offset += 0x18 * (block % BlockLevel[0]);
                        break;
                    case TreeLevel.L1:
                        offset += 0x18 * ((block / BlockLevel[0]) % BlockLevel[0]);
                        break;
                    case TreeLevel.L2:
                        offset += 0x18 * ((block / BlockLevel[1]) % BlockLevel[0]);
                        break;
                }
                return offset;
            }
            catch (Exception)
            {
                throw new Exception("Unknown error occurred");
            }
        }

        /// <summary>Set the values for the two types of volume descriptor</summary>
        /// <param name="type">The volume descriptor type</param>
        private void SetStructure(StructureType type)
        {
            switch (type)
            {
                case StructureType.Type0:
                    _spaceBetween[0] = 0xAB;
                    _spaceBetween[1] = 0x718F;
                    _spaceBetween[2] = 0xFE7DA;
                    _shift = 0;
                    return;

                case StructureType.Type1:
                    _spaceBetween[0] = 0xAC;
                    _spaceBetween[1] = 0x723A;
                    _spaceBetween[2] = 0xFD00B;
                    _shift = 1;
                    return;
            }
        }
        #endregion

        #region properties
        /// <summary>Get the base block</summary>
        internal ushort BaseBlock
        {
            get
            {
                return (ushort) (_baseByte << 12);
            }
        }

        /// <summary>Get Block count</summary>
        internal uint BlockCount
        {
            get
            {
                return _totalAllocatedBlockCount;
            }
        }

        /// <summary>Get Block level</summary>
        private uint[] BlockLevel
        {
            get
            {
                return new uint[] { 170, 0x70E4 };
            }
        }

        /// <summary>Get / Set File table starting block</summary>
        public uint FileTableBlock
        {
            get
            {
                return (uint) (((_fileTableBlock[4] << 0x10) | (_fileTableBlock[3] << 8)) | _fileTableBlock[2]);
            }
        }

        /// <summary>Get / Set File table block count</summary>
        public uint FolderTableBlockCount
        {
            set
            {
                _fileTableBlock[0] = (byte)(value & 0xFF);
                _fileTableBlock[1] = (byte)((value >> 8) & 3); 
            }
        }

        /// <summary>Get / Set File table block count</summary>
        public ushort FileTableBlockCount
        {
            get
            {
                return (ushort) ((_fileTableBlock[1] << 8) | _fileTableBlock[0]);
            }
        }

        /// <summary>Get space between tables </summary>
        private uint[] SpaceBetween
        {
            get
            {
                return _spaceBetween;
            }
        }

        /// <summary>Get table size shift</summary>
        internal StructureType Type
        {
            get
            {
                return (StructureType) _shift;
            }
        }

        internal BlockRecord BlockRecord
        {
            get { return _blockRecord; }
        }

        /// <summary>Returns the STFS volume Descriptor</summary>
        internal byte[] GetDescriptor
        {
            get
            {
                byte item = 1;
                if (Type == StructureType.Type1)
                    item = (byte)(_blockRecord.Index << 1);
                var list = new List<byte>();
                var collection = new byte[2];
                collection[0] = 0x24;
                list.AddRange(collection);
                list.Add(item);
                list.AddRange(_fileTableBlock);
                list.AddRange(new byte[20]);
                list.AddRange(Functions.Functions.UInt32ToBytesArray(_totalAllocatedBlockCount));
                list.AddRange(Functions.Functions.Int32ToBytesArray(_blockRecord.BlocksFree));
                return list.ToArray();
            }
        }
        #endregion
    }
}

