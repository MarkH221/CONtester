using Isolib.Algorithms;
using Isolib.Encryptions;
using Isolib.IOPackage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Isolib.STFSPackage
{
    /// <summary>STFS (Secure Transacted File System) is the file system used by 
    /// the Xbox 360 for all packages created and downloaded by the system</summary>
    public class Stfs
    {
        private RWStream _mainIo;
        private HeaderData _headerData;
        private Descriptor _structure;
        private BlockRecord[] _fileBlocks;
        private List<Entries> _embeddedFile = new List<Entries>();
        private readonly List<FolderEntries> _folderDirectory = new List<FolderEntries>();

        #region Constructor

        /// <summary>Stfs Constructor</summary>
        /// <param name="filepath">The stfs file path</param>
        /// <param name="endian">Write or Read endian type</param>
        public Stfs(string filepath, Endian endian = Endian.Big)
        {
            try
            {
                var isBigEndian = endian == Endian.Big; //set endian type
                _mainIo = new RWStream(filepath, isBigEndian); //initalise stream
                Init(); //call init function
            }
            catch(Exception e)
            {
                // Init() can fail on a malformed/non-STFS file. The constructor
                // never returns an instance, so release the exclusively-locked
                // stream here or the file stays "in use" until process exit.
                if (_mainIo != null)
                {
                    try { _mainIo.Close(); }
                    catch { /* preserve the original parse error */ }
                    _mainIo = null;
                }
                throw new Exception(e.Message);
            }
        }
        
        /// <summary>Stfs Constructor</summary>
        /// <param name="buffer">The stfs byte[] buffer</param>
        /// <exception cref="Exception">Error</exception>
        public Stfs(byte[] buffer)
        {
            try
            {
                _mainIo = new RWStream(buffer); //initalise stream
                Init(); //call init function
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region Private Methods
        private void Init()
        {
            try
            {
                //check if the file is the right size if not add missing size
                if (_mainIo.Length % 4096 != 0)
                {
                    //Seek EOF
                    _mainIo.Position = _mainIo.Length;
                    _mainIo.WriteBytes(new byte[4096 - (_mainIo.Length % 4096)]);
                }

                //read header data
                _headerData = new HeaderData(_mainIo);

                //Checking the content type to make sure its the right one - savegame hopefully
                if (_headerData.ContentType.Equals(ContentType.HddInstalledGame) ||
                    _headerData.ContentType.Equals(ContentType.OriginalXboxGame) ||
                    _headerData.ContentType.Equals(ContentType.GamesOnDemand) ||
                    _headerData.ContentType.Equals(ContentType.SocialTitle))
                {
                    throw new Exception("File cannot be parse.");
                }

                //initialize the file volume descriptor
                _structure = new Descriptor(_mainIo);
                _fileBlocks = new BlockRecord[0];
                GetBlocks(_structure.FileTableBlockCount, _structure.FileTableBlock, out _fileBlocks);

                ushort entryId = 0;
                //Get Embedded file details
                foreach (var tmpFileBlock in _fileBlocks)
                {
                    //The currentOffset will be the location of the embedded file details location
                    //e.g. embedded file names and how to generate the offset of the data
                    var currentOffset = GenerateDataOffset(tmpFileBlock.CurrentBlock);
                    for (var i = 0; i < 0x40; i++)
                    {
                        _mainIo.Position = (currentOffset + (0x40 * i));
                        if (_mainIo.ReadUInt8() == 0) continue;
                        _mainIo.Position--;

                        var item = new EmbeddedFileEntry(_mainIo.ReadBytes(0x40), (_mainIo.Position - 0x40), entryId, this);
                        if (item.IsDeleted) continue;

                        //If the embedded file is in the root
                        if (!(item.FolderFlag))
                            _embeddedFile.Add(new Entries(item));
                        //If the file is in a sub directory
                        else
                            _folderDirectory.Add(new FolderEntries(item));
                        entryId++;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>Generates data offsets for embedded files broken down to blocks</summary>
        /// <param name="fileTableBlockCount">The file table block count</param>
        /// <param name="fileTableBlockNumber">The file table block number</param>
        /// <param name="fileBlock">The block that will be sent back out</param>
        internal void GetBlocks(uint fileTableBlockCount, uint fileTableBlockNumber, out BlockRecord[] fileBlock)
        {
            var list = new List<BlockRecord>();
            //Get a template for file blocks
            var item = GetRecord(fileTableBlockNumber, TreeLevel.L0);

            if (item.CurrentBlock >= _structure.BlockCount)
                throw new Exception("Invalid Blocks");
            uint index;
            for (index = 0; index < fileTableBlockCount; index++)
            {
                if (list.Contains(item))break;
                
                list.Add(item);
                if (item.NextBlock == 0xFFFFFF)break;
                if (item.NextBlock >= _structure.BlockCount) throw new Exception("Invalid Blocks");
                item = GetRecord(item.NextBlock, TreeLevel.L0);
            }
            fileBlock = list.ToArray();
        }
        
        /// <summary>Get the Block Record</summary>
        /// <param name="block">The block value</param>
        /// <param name="level">The tree level</param>
        private BlockRecord GetRecord(uint block, TreeLevel level)
        {
            if (level == TreeLevel.Lt) return _structure.BlockRecord;
            var topRecord = _structure.BlockRecord;
            var flag = _structure.Type == StructureType.Type1;

            if (_structure.BlockCount > 0x70E4)
            {
                _mainIo.Position = _structure.GenerateHashOffset(block, TreeLevel.L2) + 20;
                if (flag) _mainIo.Position += topRecord.Index << 12;
                topRecord = new BlockRecord(_mainIo.ReadUInt32());
                if (level == TreeLevel.L2)
                {
                    topRecord.CurrentBlock = block;
                    return topRecord;
                }
            }
            else if (level == TreeLevel.L2)
                return _structure.BlockRecord;
            if (_structure.BlockCount > 170)
            {
                _mainIo.Position = _structure.GenerateHashOffset(block, TreeLevel.L1) + 20;
                if (flag) _mainIo.Position += topRecord.Index << 12;
                topRecord = new BlockRecord(_mainIo.ReadUInt32());
                if (level == TreeLevel.L1)
                {
                    topRecord.CurrentBlock = block;
                    return topRecord;
                }
            }
            else if (level == TreeLevel.L1) return _structure.BlockRecord;
            _mainIo.Position = _structure.GenerateHashOffset(block, TreeLevel.L0) + 20;
            if (flag)
            {
                _mainIo.Position += topRecord.Index << 12;
            }
            return new BlockRecord(_mainIo.ReadUInt32()) { CurrentBlock = block };
        }

        /// <summary>Generates a data offset</summary>
        /// <param name="block">The block value</param>
        internal long GenerateDataOffset(uint block)
        {
            return _structure.GenerateDataOffset(block);
        }

        /// <summary>Generates a hash base</summary>
        /// <param name="block">The block value</param>
        /// <param name="tree">The tree level</param>
        private long GenerateBaseOffset(uint block, TreeLevel tree)
        {
            var baseOffset = _structure.GenerateBaseOffset(block, tree);
            if (_structure.Type == StructureType.Type1)
            {
                baseOffset += GetRecord(block, (TreeLevel)((byte)(tree + 1))).Index << 12;
            }
            return baseOffset;
        }

        /// <summary>Write the embedded file data data file</summary>
        /// <param name="input">Temporary referenced I/O</param>
        /// <param name="blocks">The blocks the data has</param>
        internal bool WriteEmbeddedFile(ref RWStream input, BlockRecord[] blocks)
        {
            if (!input.Accessed || (Functions.Functions.BlockCountFile(input) != blocks.Length))
            {
                return false;
            }
            try
            {
                input.Position = 0L;
                for (var i = 0; i < (blocks.Length); i++)//I removed -1 because sometimes the file doesn't write the ending block my mistake
                {
                    _mainIo.Position = GenerateDataOffset(blocks[i].CurrentBlock);
                    _mainIo.WriteBytes(input.ReadBytes(0x1000));
                }
                _mainIo.Position = GenerateDataOffset(blocks[blocks.Length - 1].CurrentBlock);
                //I had a problem that with big embedded files I can't write to the last few blocks
                //little work around to avoid false return
                if (input.Position == input.Length)
                {
                    _mainIo.Flush();
                    return true;
                }
                _mainIo.WriteBytes(input.ReadBytes(Functions.Functions.BlockRemainder(input)));
                _mainIo.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Generates a hash offset</summary>
        /// <param name="block">The block value</param>
        /// <param name="tree">The tree level</param>
        private long GenerateHashOffset(uint block, TreeLevel tree)
        {
            var offset = _structure.GenerateHashOffset(block, tree);

            if (_structure.Type == StructureType.Type1)
            {
                offset += (GetRecord(block, (TreeLevel)((byte)(tree + 1))).Index << 12);
            }
            return offset;
        }

        /// <summary>Writes a SHA1 hash from one I/O to another - Reads from one I/O, hashes, stores it in another I/O</summary>
        /// <param name="input">The input I/O to read and split to write</param>
        /// <param name="read">The position to read</param>
        /// <param name="write">The position to write</param>
        /// <param name="size">The bytes count to read</param>
        private void WriteHash(long read, long write, int size, ref RWStream input)
        {
            try
            {
                WriteHash(ref input, read, write, size, ref input);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>Writes a SHA1 hash from one I/O to another - Reads from one I/O, hashes, stores it in another I/O</summary>
        /// <param name="input">The input I/O to read</param>
        /// <param name="read">The position to read</param>
        /// <param name="write">The position to write</param>
        /// <param name="size">The bytes count to read</param>
        /// <param name="output">The output to write</param>
        private void WriteHash(ref RWStream input, long read, long write, int size, ref RWStream output)
        {
            try
            {
                input.Position = read;
                var buffer = input.ReadBytes(size);
                output.Position = write;
                buffer = Sha1.Compute(buffer);
                output.WriteBytes(buffer);
                output.Flush();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void WriteDescriptor(ref RWStream tempIo)
        {
            try
            {
                tempIo.Position = 0x379;
                var totalBlocks = _embeddedFile.Count + _folderDirectory.Count;
                if (totalBlocks != 0) totalBlocks = ((totalBlocks - 1) / 0x40) + 1;

                _structure.FolderTableBlockCount = (ushort)totalBlocks;
                tempIo.WriteBytes(_structure.GetDescriptor);
                tempIo.Flush();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>Write New STFS File</summary>
        /// <param name="param">The Rsaparameters to use</param>
        private void WriteHeader(RsaParam param)
        {
            try
            {
                var streamer = new RWStream();
                _headerData.ContentSize = _mainIo.Length - _structure.BaseBlock;
                _headerData.Write(ref streamer);
                streamer.Position = 0x340;
                streamer.WriteInt32(_structure.Type == StructureType.Type0 ? 0xAD0E : 0x971A);
                streamer.Position = streamer.Length;
                streamer.WriteBytes(new byte[0x8e6 + (_structure.BaseBlock - 0xA000)]);
                streamer.Position = 0x379;
                WriteDescriptor(ref streamer);
                long generatedOffset;
                if (_structure.BlockCount <= 170)
                    generatedOffset = GenerateBaseOffset(0, TreeLevel.L0);
                else if (_structure.BlockCount <= 0x70ea)
                    generatedOffset = GenerateBaseOffset(0, TreeLevel.L1);
                else
                    generatedOffset = GenerateBaseOffset(0, TreeLevel.L2);

                WriteHash(ref _mainIo, generatedOffset, 0x381, 0x1000, ref streamer);
                var size = (_structure.BaseBlock == 0xA000) ? 0x9CBC : 0xACBC;
                WriteHash(0x344, 0x32C, size, ref streamer);
                streamer.Position = 0x22C;
                var rgbHash = Sha1.Compute(streamer.ReadBytes(280));
                streamer.Position = 4L;
                if (_headerData.SignatureHeaderType == SignatureType.Con)
                {
                    streamer.WriteBytes(param.Certificate);
                    var key = new RSACryptoServiceProvider();
                    var formatter = new RSAPKCS1SignatureFormatter();
                    key.ImportParameters(param.ParametersRsaKeys);
                    formatter.SetHashAlgorithm("SHA1");
                    formatter.SetKey(key);
                    formatter.CreateSignature(rgbHash);
                    streamer.WriteBytes(Functions.Functions.SimpleScramble(Rsa.GeneratePks1Signature(param.ParametersRsaKeys, rgbHash), true));
                }
                else
                {
                    streamer.WriteBytes(Functions.Functions.ReverseByteArray(Rsa.GeneratePks1Signature(param.ParametersRsaKeys, rgbHash)));
                    streamer.WriteBytes(new byte[0x128]);
                }
                streamer.IsBigEndian = true;
                streamer.Flush();
                //write to mainIo
                _mainIo.Position = 0;
                _mainIo.WriteBytes(streamer.ReadStream());
                _mainIo.Flush();
                streamer.Close();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>Write Hash tables</summary>
        private void WriteTables()
        {
            try
            {
                uint index;
                for (index = 0; index < _structure.BlockCount; index++)
                {
                    WriteHash(GenerateDataOffset(index), GenerateHashOffset(index, TreeLevel.L0), 0x1000, ref _mainIo);
                }

                if (_structure.BlockCount > 170)
                {
                    var num2 = ((_structure.BlockCount - 1) / 170) + 1;
                    for (index = 0; index < num2; index++)
                    {
                        WriteHash(GenerateBaseOffset(index * 170, TreeLevel.L0), GenerateHashOffset(index * 170, TreeLevel.L1), 0x1000, ref _mainIo);
                    }
                    if (_structure.BlockCount > 0x70ea)
                    {
                        num2 = ((_structure.BlockCount - 1) / 0x70ea) + 1;
                        for (index = 0; index < num2; index++)
                        {
                            WriteHash(GenerateHashOffset(index * 0x70ea, TreeLevel.L1), GenerateHashOffset(index * 0x70ea, TreeLevel.L2), 0x1000, ref _mainIo);
                        }
                    }
                }
                _mainIo.Flush();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region public Methods
        /// <summary>Write Header Data and fix CON file</summary>
        /// <returns>A boolean of the success of this function</returns>
        public void Finish()
        {
            try
            {
                var param = new RsaParam();
                WriteTables();
                WriteHeader(param);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Close the Initializer</summary>
        public void Close()
        {
            _fileBlocks = null;
            _embeddedFile = null;
            if (_mainIo != null)
            {
                _mainIo.Close();
            }
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Extraction
        /// <summary>Extract an embedded file from the Stfs file</summary>
        /// <param name="filename">The embedded file name</param>
        /// <returns>The data of the specified embedded file </returns>
        public byte[] Extract(string filename)
        {
            try
            {
                foreach (var entry in _embeddedFile)
                {
                    //Searching for the correct file using the data we have
                    if (entry.Name.ToLower() != filename.ToLower()) continue;
                    return entry.Extract();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            return null;
        }

        /// <summary>Extract an embedded file from the Stfs file</summary>
        /// <param name="index">The embedded file Index</param>
        /// <returns>The data of the specified embedded file </returns>
        public byte[] Extract(int index)
        {
            try
            {
                //Return a byte array buffer of the embedded file - Actual extraction
                return _embeddedFile[index].Extract();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Extract an embedded file from the Stfs file</summary>
        /// <param name="entry">The embedded file entrty file</param>
        /// <returns>The data of the specified embedded file </returns>
        public byte[] Extract(Entries entry)
        {
            try
            {
                return entry.Extract();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Extract an embedded file from the Stfs file</summary>
        /// <param name="filename">The embedded file name</param>
        /// <param name="directory">The directory the file will be extracted to</param>
        public void Extract(string filename, string directory)
        {
            try
            {
                foreach (var entry in _embeddedFile)
                {
                    //Searching for the correct file using the data we have
                    if (entry.Name.ToLower() != filename.ToLower()) continue;
                    var buffer = entry.Extract();
                    var binaryWriter = new BinaryWriter(File.Create(directory + "\\" + filename));
                    if (buffer == null || buffer.Length == 0)
                        binaryWriter.Close();
                    else
                    {
                        binaryWriter.Write(buffer);
                        binaryWriter.Flush();
                        binaryWriter.Close();
                    }
                    break;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Extract an embedded file from the Stfs file</summary>
        /// <param name="entry">The embedded file entrty file</param>
        /// <param name="directory">The directory the file will be extracted to</param>
        public void Extract(Entries entry, string directory)
        {
            try
            {
                var buffer = entry.Extract();
                var binaryWriter = new BinaryWriter(File.Create(directory + "\\" + entry.Name));
                if (buffer == null || buffer.Length == 0)
                    binaryWriter.Close();
                else
                {
                    binaryWriter.Write(buffer);
                    binaryWriter.Flush();
                    binaryWriter.Close();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>Extract an embedded file from the Stfs file</summary>
        /// <param name="index">The embedded file Index</param>
        /// <param name="directory">The directory the file will be extracted to</param>
        public void Extract(int index, string directory)
        {
            try
            {
                var buffer = _embeddedFile[index].Extract();
                var binaryWriter = new BinaryWriter(File.Create(directory + "\\" + _embeddedFile[index].Name));
                if (buffer == null || buffer.Length == 0)
                    binaryWriter.Close();
                else
                {
                    binaryWriter.Write(buffer);
                    binaryWriter.Flush();
                    binaryWriter.Close();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        #endregion

        #region Replace Embedded File
        /// <summary>Replace a file using byte array</summary>
        /// <param name="buffer">Array of bytes that represents the file we will be Replaceing</param>
        /// <param name="embeddedfileName">The embedded file that will be replaced</param>
        public bool Replace(byte[] buffer, string embeddedfileName)
        {
            var replaceFileIo = new RWStream(buffer, true, true);
            try
            {
                //call the Replace(RWStream, filename) function
                return Replace(replaceFileIo, embeddedfileName);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                replaceFileIo.Close();
            }
        }

        /// <summary>Replace a file using I/O Stream</summary>
        /// <param name="streamer">The I/O Stream with the data we want to write</param>
        /// <param name="embeddedfileName">The embedded file that will be replaced</param>
        public bool Replace(RWStream streamer, string embeddedfileName)
        {
            try
            {
                //Incase you are replacing from a filepath
                var lastBackSlash = embeddedfileName.LastIndexOf("\\", StringComparison.Ordinal);
                if (lastBackSlash == -1) lastBackSlash = 0;
                var filename = embeddedfileName.Substring(lastBackSlash);
                foreach (var entry in _embeddedFile)
                {
                    //Searching for the correct file using the data we have
                    if (entry.Name.ToLower() != filename.ToLower()) continue;
                    if (entry.Replace(streamer) == false) return false;
                    _mainIo.Position = entry.SizeOffset;
                    _mainIo.WriteInt32((int) streamer.Length);
                    break;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region properties
        /// <summary>Get details of the header data</summary>
        public HeaderData HeaderData
        {
            get { return _headerData; }
        }
        /// <summary></summary>
        public List<Entries> EmbeddedFiles
        {
            get { return _embeddedFile; }
        }
        internal IEnumerable<FolderEntries> FolderDirectory
        {
            get { return _folderDirectory; }
        }
        internal RWStream MainIo
        {
            get { return _mainIo; }
        }
        #endregion
    }

    #region Enum
    /// <summary> </summary>
    public enum Endian
    {
        /// <summary></summary>
        Big,
        /// <summary></summary>
        Little
    }
    /// <summary>File Signature type enumerations</summary>
    public enum SignatureType : uint
    {
        /// <summary>CON file</summary>
        Con = 0x434F4E20,
        /// <summary>LIVE file</summary>
        Live = 0x4C495645,
        /// <summary>PIR file</summary>
        Pirs = 0x50495253,
        /// <summary>UNKNOWN file</summary>
        Unknown = 0xFFFFFFFF
    }
    /// <summary>Package type enumerations</summary>
    public enum ContentType : uint
    {
        /// <summary>Save game data</summary>
        SavedGame = 1,
        /// <summary>Hard drive installed games</summary>
        HddInstalledGame = 0x4000,
        /// <summary>Original xbox games</summary>
        OriginalXboxGame = 0x5000,
        /// <summary>Social titles</summary>
        SocialTitle = 0x6000,
        /// <summary>Game on demand</summary>
        GamesOnDemand = 0x7000
    }
    /// <summary>Language Type Enumeration</summary>
    public enum LanguagesType : uint
    {
        /// <summary>English Langauage</summary>
        English = 0,
        /// <summary>Japanese Langauage</summary>
        Japanese = 1,
        /// <summary>German Langauage</summary>
        German = 2,
        /// <summary>French Langauage</summary>
        French = 3,
        /// <summary>Spanish Langauage</summary>
        Spanish = 4,
        /// <summary>Italian Langauage</summary>
        Italian = 5,
        /// <summary>Korean Langauage</summary>
        Korean = 6,
        /// <summary>Chinese Langauage</summary>
        Chinese = 7,
        /// <summary>Portugese Langauage</summary>
        Portuguese = 8
    }
    /// <summary>File Structure type enumerations</summary>
    internal enum StructureType
    {
        /// <summary>Structure Type 0</summary>
        Type0,
        /// <summary>Structure Type 1</summary>
        Type1
    }
    /// <summary>The (Tree) Block Level</summary>
    internal enum TreeLevel : byte
    {
        L0 = 0,
        L1 = 1,
        L2 = 2,
        Lt = 3
    }
    #endregion
}