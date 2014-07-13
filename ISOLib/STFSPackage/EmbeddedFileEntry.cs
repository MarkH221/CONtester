using Isolib.IOPackage;
using System;

namespace Isolib.STFSPackage
{
    /// <summary>Working with embedded file</summary>
    public class EmbeddedFileEntry
    {
        #region variables
        private readonly int _created; //Update date/time stamp of file
        private readonly long _embeddedFileInfoOffset; //The offset of the embedded file information header
        private byte _flag; //Flag Bytes
        private readonly int _itAccessed; //Access date/time stamp of file
        private readonly string _name; //The name of the file
        private readonly uint _amountOfDataBlocks; //The embedded file is broken into blocks which will help us Replace and extract
        private readonly ushort _entryId; //Embedded file ID
        private readonly int _sizeOfFileInByte; //The size of file in bytes (Big Endian)
        private readonly long _sizeOfFileOffset; //Offset for Embded file length
        private readonly ushort _parentEntryId = 0xFFFF; //Indicates the Path of the EmbdedFiles 0xFFFF = root
        private readonly FolderEntries _parentFolder;
        private readonly string _embeddedFileDataOffset; //This is the offset of where the data starts for this embedded file
        private readonly Stfs _currentStfs;
        #endregion

        #region Constructors
        /// <summary>Initialize the Embedded file entry</summary>
        /// <param name="embeddedFileEntry">Empty Embedded file entry</param>
        internal EmbeddedFileEntry(EmbeddedFileEntry embeddedFileEntry)
        {
            _currentStfs = embeddedFileEntry.CurrentStfs;
            _name = embeddedFileEntry._name; //Name embdedFile
            _itAccessed = embeddedFileEntry._itAccessed; //Time Accessed
            _created = embeddedFileEntry._created; // Time created
            _amountOfDataBlocks = embeddedFileEntry._amountOfDataBlocks; //amount of the datablocks
            _embeddedFileInfoOffset = embeddedFileEntry._embeddedFileInfoOffset; // offset for Embded fileinfo
            _flag = embeddedFileEntry._flag; //flag
            _entryId = embeddedFileEntry._entryId; // the Id for the entry
            _parentEntryId = embeddedFileEntry._parentEntryId; // indicates the path for embded file
            _parentFolder = embeddedFileEntry._parentFolder;
            _sizeOfFileOffset = embeddedFileEntry._sizeOfFileOffset; // offset for the embded file length
            _sizeOfFileInByte = embeddedFileEntry._sizeOfFileInByte; // the size of the file
            StartingBlockNumberOfFile = embeddedFileEntry.StartingBlockNumberOfFile; // BlockNumber for embdedFile
            //Get an hex offset of the embedded file data location
            _embeddedFileDataOffset = embeddedFileEntry._embeddedFileDataOffset; // Offset EmbdedFile (data)

            _flag = (byte)((embeddedFileEntry.FolderFlag ? 1 : 0) << 7 | (embeddedFileEntry.UnknownFlag ? 1 : 0) << 6 | _name.Length);
        }

        /// <summary>Initialize the Embedded file entry</summary>
        /// <param name="bufferOfEmbeddedFileInfo">The buffer of an embedded file information</param>
        /// <param name="fileInfoOffset">The offset of an embedded file information</param>
        /// <param name="id">The embedded file ID</param>
        /// <param name="stfsPackage">The Initialized main package</param>
        internal EmbeddedFileEntry(byte[] bufferOfEmbeddedFileInfo, long fileInfoOffset, ushort id, Stfs stfsPackage)
        {
            //ABOUT EMBEDDED FILE INFO==========================================================//
            //The embedded file info is 0x40 (Each embedded file starts at a 4096 byte boundary)//
            //File name @ 0x0 - 0x28                                                            //
            //==================================================================================//
            try
            {
                var stream = new RWStream(bufferOfEmbeddedFileInfo, true, true) { Position = 40 };
                _entryId = id;
                _currentStfs = stfsPackage;
                //used to get namelength
                _flag = stream.ReadUInt8();
                //Checking the length of the embedded file
                if (NameLength > 40) NameLength = 40;

                stream.Position = 0;
                if (NameLength != 0)
                {
                    //Getting the name of the embedded file
                    _name = stream.ReadString(StringType.Ascii, NameLength);
                    Functions.Functions.IsValidXboxName(_name);
                    //gets the starting Block Number
                    stream.Position = 0x2F;
                    stream.IsBigEndian = false;
                    StartingBlockNumberOfFile = stream.ReadUInt24();
                    //gets the Path Indicator
                    stream.IsBigEndian = true;
                    //parent folder details
                    _parentEntryId = stream.ReadUInt16();
                    _parentFolder = new FolderEntries("", 0, 0xFFFF, 0xFFFF, stfsPackage);//root parent folder
                    foreach (var folder in stfsPackage.FolderDirectory)
                    {
                        if (folder.EntryId != _parentEntryId) continue;
                        _parentFolder = folder;
                        break;
                    }
                    //Gets the size of the embded file
                    _sizeOfFileOffset = fileInfoOffset + stream.Position; //sets the offset for EmbdedFile size
                    _sizeOfFileInByte = stream.ReadInt32();//gets the embdedFile length
                    //Calculates & sets the amount of data blocks
                    _amountOfDataBlocks = (uint) (((_sizeOfFileInByte - 1) / 0x1000) + 1);
                    _created = stream.ReadInt32(); //gets the time created
                    _itAccessed = stream.ReadInt32();//gets the time accessed
                    //The start of the embedded file information
                    _embeddedFileInfoOffset = fileInfoOffset;//sets FileOffset (Info)

                    //=========================================================================
                    //Getting the offset of the embedded file data
                    BlockRecord[] blocks;
                    stfsPackage.GetBlocks(_amountOfDataBlocks, StartingBlockNumberOfFile, out blocks);
                    //Get an hex offset of the embedded file data location
                    _embeddedFileDataOffset = "0x"+stfsPackage.GenerateDataOffset(blocks[0].CurrentBlock).ToString("X");
                    stream.Close();//done , so we close the steam.
                }
            }
            catch (Exception)
            {
                NameLength = 0;
            }
        }

        /// <summary>Initialize the Embedded file entry</summary>
        /// <param name="nameIn">Embedded file name</param>
        /// <param name="theSizeOfFileInByte">The size of file in bytes (Big Endian)</param>
        /// <param name="isFolder">The input Folder</param>
        /// <param name="id">The embedded file id</param>
        /// <param name="thePathIndicator">The path Indicator to show if the file is located in the root or sub directory</param>
        /// <param name="stfsPackage">The Initialized main package</param>
        internal EmbeddedFileEntry(string nameIn, int theSizeOfFileInByte, bool isFolder, ushort id, ushort thePathIndicator, Stfs stfsPackage)
        {
            //sets the Embded File entries
            _entryId = id;
            _currentStfs = stfsPackage;
            _parentEntryId = thePathIndicator;
            if (stfsPackage != null)
            {
                foreach (var folder in stfsPackage.FolderDirectory)
                {
                    if (folder.EntryId != _parentEntryId) continue;
                    _parentFolder = folder;
                    break;
                }
            }
            _name = (nameIn.Length >= 40) ? nameIn.Substring(0, 40) : nameIn;
            _flag = (byte)(((isFolder ? 1 : 0) << 7) | _name.Length);
            _created = Functions.Functions.DateTimeToFatTimeInt(DateTime.Now);
            _itAccessed = _created;

            if (isFolder)
            {
                _sizeOfFileInByte = 0;
                StartingBlockNumberOfFile = 0;
                _amountOfDataBlocks = 0;
            }
            else
            {
                _sizeOfFileInByte = theSizeOfFileInByte;
                if (_sizeOfFileInByte != 0)
                {
                    _amountOfDataBlocks = (uint) (((_sizeOfFileInByte - 1) / 0x1000) + 1);
                }
            }
        }
        #endregion

        #region  public properties
        /// <summary>Get the time last accessed</summary>
        public DateTime Accessed
        {
            get
            {
                return Functions.Functions.IntToFatTimeDateTime(_itAccessed);
            }
        }

        /// <summary>Get the time Created</summary>
        public DateTime Created
        {
            get
            {
                return Functions.Functions.IntToFatTimeDateTime(_created);
            }
        }

        /// <summary>Get the embedded file Data starting offset</summary>
        public string DataOffset
        {
            get
            {
                return _embeddedFileDataOffset;
            }
        }

        /// <summary>The parent directory/folder name</summary>
        public FolderEntries ParentFolder
        {
            get { return _parentFolder; }
        }

        /// <summary>Check if item is deleted</summary>
        public bool IsDeleted
        {
            get
            {
                return (NameLength == 0);
            }
        }

        /// <summary>Get the entry name</summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>Get the embedded file size</summary>
        public string Size
        {
            get { return Functions.Functions.GetSize(_sizeOfFileInByte); }
        }

        /// <summary>The offset of the file size</summary>
        public long SizeOffset
        {
            get { return _sizeOfFileOffset; }
        }
        #endregion

        #region private & internal properties
        /// <summary>Get the entry ID</summary>
        private ushort EntryId
        {
            get
            {
                return _entryId;
            }
        }

        /// <summary>Get the embedded file name length</summary>
        private byte NameLength
        {
            get
            {
                return (byte)(_flag & 0x3F);
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > 40)
                {
                    value = 40;
                }
                _flag = (byte)((((FolderFlag ? 1 : 0)) << 7) | (((UnknownFlag ? 1 : 0)) << 6) | value);
            }
        }

        /// <summary>Check if the flag can be identified</summary>
        private bool UnknownFlag
        {
            get
            {
                return (((_flag >> 6) & 1) == 1);
            }
        }

        /// <summary>Get the folder flag</summary>
        internal bool FolderFlag
        {
            get
            {
                return (((_flag >> 7) & 1) == 1);
            }
        }

        internal Stfs CurrentStfs
        {
            get { return _currentStfs; }
        }
        
        /// <summary>Get the folder pointer</summary>
        internal ushort ParentEntryId
        {
            get
            {
                return _parentEntryId;
            }
        }

        /// <summary>The amount of data blocks</summary>
        internal uint AmountOfDataBlocks
        {
            get { return _amountOfDataBlocks; }
        }

        /// <summary>Size of embedded file in byte</summary>
        internal int SizeOfFileInByte
        {
            get { return _sizeOfFileInByte; }
        }

        internal uint StartingBlockNumberOfFile { get; private set; }
        #endregion
    }
}

