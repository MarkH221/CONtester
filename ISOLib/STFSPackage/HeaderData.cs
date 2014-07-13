using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Isolib.IOPackage;

namespace Isolib.STFSPackage
{
    /// <summary>Contains function/s that deals with STFS Header Data</summary>
    public sealed class HeaderData
    {
        #region variable
        private readonly SignatureType _magicType;
        private readonly List<License> _licenseEntry = new List<License>(0xF);
        private byte[] _contentImage;
        private long _contentSize;
        private LanguagesType _language = LanguagesType.English;
        private readonly uint _dataFileCount;
        private readonly long _dataFileCombinedSize;
        private readonly string[] _displayDescriptions = new string[0x12];
        private byte[] _deviceID = new byte[20];
        private readonly byte _discInSet;
        private readonly byte _discNumber;
        private readonly byte _executableType;
        private readonly byte _transferFlag; 
        private readonly uint _mediaID;
        private readonly uint _metaDataVersion = 2;
        private byte[] _packageImage;
        private readonly ContentType _contentType = ContentType.SavedGame;
        private readonly byte _platform;
        private long _profileID;
        private string _publisherName;
        private readonly long _reserved;
        private byte[] _saveConsoleID;
        private readonly uint _saveGameID;
        private string _titleName;
        private readonly uint _titleID = 0xFFFE07D1;
        private readonly string[] _displayName = new string[0x12];
        private readonly uint _version;
        private readonly uint _baseVersion;
        //=====================================================================
        //Metadata Version 2 used for SVOD files
        private readonly byte[] _seasonID;
        private readonly ushort _seasonNumber;
        private readonly byte[] _seriesID;
        private readonly ushort _epidsodeNumber;
        #endregion

        /// <summary>Header Data Constructor</summary>
        /// <param name="stream">The RWStream of the data file</param>
        internal HeaderData(RWStream stream)
        {
            try
            {
                stream.Position = 0;
                var magic = stream.ReadUInt32();
                //Check if there is a matching header value
                if (Enum.IsDefined(typeof(SignatureType), magic))
                    _magicType = (SignatureType)magic;//safty cast
                else
                    throw new Exception("Invalid Package - #Magic: " + string.Format(_magicType.ToString("X4")));

                //Reading data licenses and adding them to the license entry table
                stream.Position = 0x22C;
                byte index;
                for (index = 0; index < 0x10; index++)//Reading the ID, Bits and Flags
                    _licenseEntry.Add(new License(stream.ReadInt64(), stream.ReadInt32(), stream.ReadInt32()));

                stream.Position = 0x344;
                _contentType = (ContentType)stream.ReadUInt32();
                _metaDataVersion = stream.ReadUInt32();
                _contentSize = stream.ReadInt64();
                _mediaID = stream.ReadUInt32();
                _version = stream.ReadUInt32();
                _baseVersion = stream.ReadUInt32();
                _titleID = stream.ReadUInt32();
                _platform = stream.ReadUInt8();
                _executableType = stream.ReadUInt8();
                _discNumber = stream.ReadUInt8();
                _discInSet = stream.ReadUInt8();
                _saveGameID = stream.ReadUInt32();
                _saveConsoleID = stream.ReadBytes(5);
                _profileID = stream.ReadInt64();

                stream.Position = 0x39D;
                _dataFileCount = stream.ReadUInt32();
                _dataFileCombinedSize = stream.ReadInt64();
                _reserved = stream.ReadInt64();
                _seriesID = stream.ReadBytes(0x10);
                _seasonID = stream.ReadBytes(0x10);
                _seasonNumber = stream.ReadUInt16();
                _epidsodeNumber = stream.ReadUInt16();

                stream.Position += 40;
                _deviceID = stream.ReadBytes(20);

                //Getting the displayed names each 0x80
                for (index = 0; index < _displayName.Length; index++)
                    _displayName[index] = stream.ReadString(StringType.Unicode, 0x80).Replace("\0", "");
                for (index = 0; index < _displayDescriptions.Length; index++)
                    _displayDescriptions[index] = stream.ReadString(StringType.Unicode, 0x80).Replace("\0", "");

                _publisherName = stream.ReadString(StringType.Unicode, 0x80).Replace("\0", "");
                _titleName = stream.ReadString(StringType.Unicode, 0x80).Replace("\0", "");

                stream.Position = 0x1711;
                _transferFlag = stream.ReadUInt8();
                //get the size of the thumbnail image
                var thumbnailImageSize = stream.ReadInt32();
                stream.Position = 0x171A;
                //checking the size because the max size is 0x4000
                _packageImage = stream.ReadBytes((thumbnailImageSize < 0x4000) ? thumbnailImageSize : 0x4000);
                stream.Position = 0x1716;
                thumbnailImageSize = stream.ReadInt32();
                stream.Position = 0x571A;
                _contentImage = stream.ReadBytes((thumbnailImageSize < 0x4000) ? thumbnailImageSize : 0x4000);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #region Method
        /// <summary>Write the header data</summary>
        /// <param name="stream">The writer stream</param>
        public bool Write(ref RWStream stream)
        {
            try
            {
                stream.Position = 0;
                stream.WriteUInt32((uint)_magicType);
                if ((stream == null) || !stream.Accessed)return false;
                stream.Position = 0x22C;
                foreach (var license in _licenseEntry)
                {
                    stream.WriteInt64(license.LicenseID);
                    stream.WriteInt32(license.LicenseBits);
                    stream.WriteInt32(license.LicenseFlags);
                }
                //Position should be 812
                stream.Position = 0x344;
                stream.WriteUInt32((uint) _contentType);
                stream.WriteUInt32(_metaDataVersion);
                stream.WriteInt64(_contentSize);
                stream.WriteUInt32(_mediaID);
                stream.WriteUInt32(_version);
                stream.WriteUInt32(_baseVersion);
                stream.WriteUInt32(_titleID);
                stream.WriteUInt8(_platform);
                stream.WriteUInt8(_executableType);
                stream.WriteUInt8(_discNumber);
                stream.WriteUInt8(_discInSet);
                stream.WriteUInt32(_saveGameID);
                stream.WriteBytes(_saveConsoleID);
                stream.WriteInt64(_profileID);

                //position should be 889
                stream.Position = 0x39D;
                stream.WriteUInt32(_dataFileCount);
                stream.WriteInt64(_dataFileCombinedSize);
                stream.WriteInt64(_reserved);
                stream.WriteBytes(_seriesID);
                stream.WriteBytes(_seasonID);
                stream.WriteUInt16(_seasonNumber);
                stream.WriteUInt16(_epidsodeNumber);

                //position should be 981
                stream.Position += 0x28;
                stream.WriteBytes(_deviceID);

                int index;
                for (index = 0; index < _displayName.Length; index++)
                    stream.WriteString(_displayName[index], StringType.Unicode, 0x40);
                //3345 position
                for (index = 0; index < _displayDescriptions.Length; index++)
                    stream.WriteString(_displayDescriptions[index], StringType.Unicode, 0x40);
                //5649
                stream.WriteString(_publisherName, StringType.Unicode, 0x40);
                //5777
                stream.WriteString(_titleName, StringType.Unicode, 0x40);

                //5905
                stream.Position = 0x1711;
                stream.WriteUInt8(_transferFlag);

                //write the thumbnail image - package image
                //write the length of the image
                stream.WriteInt32(_packageImage.Length);
                stream.Position = 0x171A;
                //write the image
                //5910
                stream.WriteBytes(_packageImage);
                //write the remainder bytes
                stream.WriteBytes(new byte[0x4000 - _packageImage.Length]);
                //write the thumbnail image - content image
                //write the length of the image
                stream.Position = 0x1716;
                stream.WriteInt32(_contentImage.Length);
                stream.Position = 0x571AL;
                //write the image
                stream.WriteBytes(_contentImage);
                //write the remainder
                stream.WriteBytes(new byte[0x4000 - _contentImage.Length]);
                //33225
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region properties
        /// <summary>Get and set the header content image in image</summary>
        public Image ContentImage
        {
            get
            {
                return Functions.Functions.ImageFromByteArray(ContentImageByte);
            }
            set
            {
                var buffer = Functions.Functions.ImageToByteArray(value, ImageFormat.Png);
                if (buffer.Length <= 0x4000)
                {
                    _contentImage = buffer;
                }
            }
        }

        /// <summary>Get and set the header content image in byte array</summary>
        public byte[] ContentImageByte
        {
            get
            {
                return _contentImage;
            }
            set
            {
                if (value.Length <= 0x4000)
                {
                    _contentImage = value;
                }
            }
        }

        /// <summary>Get the content size</summary>
        public long ContentSize
        {
            get {return _contentSize;}
            set { _contentSize = value; }
        }

        /// <summary>Get the content size</summary>
        public string ContentSizeOnDisk
        {
            get { return Functions.Functions.GetSize(_contentSize); }
        }

        /// <summary>Get and Set the header descriptions </summary>
        public string Description
        {
            get
            {
                return _displayDescriptions[(byte) _language];
            }
            set
            {
                if (value.Length > 0x80)
                {
                    value = value.Substring(0, 0x80);
                }
                _displayDescriptions[(byte) _language] = value;
            }
        }

        /// <summary>Get and Set the Device ID</summary>
        public String DeviceID
        {
            get { return Functions.Functions.ByteArrayToHexString(_deviceID); }
            set
            {
                var varArray = Functions.Functions.HexToBytes(value);
                Functions.Functions.SetByteArray(ref _deviceID, varArray);
            }
        }

        /// <summary>Get and set the header package image in image</summary>
        public Image PackageImage
        {
            get
            {
                return Functions.Functions.ImageFromByteArray(PackageImageByte);
            }
            set
            {
                var buffer = Functions.Functions.ImageToByteArray(value, ImageFormat.Png);
                if (buffer.Length <= 0x4000)
                {
                    _packageImage = buffer;
                }
            }
        }

        /// <summary>Get and set the header package image in byte array</summary>
        public byte[] PackageImageByte
        {
            get
            {
                return _packageImage;
            }
            set
            {
                if (value.Length <= 0x4000)
                {
                    _packageImage = value;
                }
            }
        }

        /// <summary>Get and Set the Profile ID</summary>
        public String ProfileID
        {
            get
            {
                return Functions.Functions.LongToHexString(_profileID);
            }
            set
            {
                var varArray = Functions.Functions.HexToBytes(value);
                _profileID = Functions.Functions.ByteArrayToInt64(varArray);
            }
        }

        /// <summary>Get the title Id of the file</summary>
        public String TitleID
        {
            get { return Functions.Functions.LongToHexString(_titleID); }
        }

        /// <summary>Get and Set Publisher </summary>
        public string PublisherName
        {
            get
            {
                return _publisherName;
            }
            set
            {
                if (value.Length > 0x80)
                {
                    value = value.Substring(0, 0x80);
                }
                _publisherName = value;
            }
        }

        /// <summary>Data Language</summary>
        public LanguagesType Language
        {
            set { _language = value; }
        }

        /// <summary>Get and set the console id</summary>
        public String ConsoleID
        {
            get
            {
                return Functions.Functions.ByteArrayToHexString(_saveConsoleID);
            }
            set
            {
                if (value.Length > 10)
                    _saveConsoleID = Functions.Functions.HexToBytes("FFFFFFFFFF");
                _saveConsoleID = Functions.Functions.HexToBytes(value);
            }
        }

        /// <summary>Get and set the header magic type</summary>
        public SignatureType SignatureHeaderType
        { 
            get{return _magicType;} 
        }

        /// <summary>Get and set the header displayed title</summary>
        public string DisplayName
        {
            get
            {
                return _displayName[(byte) _language];
            }
            set
            {
                if (value.Length > 0x80)
                {
                    value = value.Substring(0, 0x80);
                }
                _displayName[(byte) _language] = value;
            }
        }

        /// <summary>Get and set the header package title</summary>
        public string TitleName
        {
            get
            {
                return _titleName;
            }
            set
            {
                if (value.Length > 0x80)
                {
                    value = value.Substring(0, 0x80);
                }
                _titleName = value;
            }
        }

        /// <summary>Get the Media ID</summary>
        public string MediaID
        {
            get { return Functions.Functions.LongToHexString(_mediaID); }
        }

        /// <summary>Get and set the header package type</summary>
        public ContentType ContentType
        {
            get
            {
                return _contentType;
            }
        }
        #endregion
    }
}