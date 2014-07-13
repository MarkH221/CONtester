using Isolib.IOPackage;
using System.Collections.Generic;

namespace Isolib.STFSPackage
{
    #region Entries
    /// <summary>Object for STFS File Entry</summary>
    public class Entries : EmbeddedFileEntry
    {
        private BlockRecord[] _blocks;

        /// <summary>The file entry constructor</summary>
        internal Entries(EmbeddedFileEntry entry) : base(entry){ }

        #region Methods
        /// <summary>Clear file entry blocks</summary>
        private bool ClearBlocks()
        {
            try
            {
                _blocks = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>This is the actually embedded file extraction process</summary>
        /// <returns>Return the buffer byte[] of the embedded file in CON file</returns>
        internal byte[] Extract()
        {
            //Settup a temporary file
            var input = new RWStream();
            try
            {
                //We call a function to write the data into a temporary file
                //flag to see if the operation was successful
                var flag = EmbeddedDataExtraction(input);
                var buffer = new byte[input.Length];
                if (flag)
                {
                    //We read the data from the temporary file to a buffer which we will return
                    input.Position = 0L;
                    buffer = input.ReadBytes(buffer.Length);
                    input.Close();
                    return buffer;
                }
                return null;
            }
            catch
            {
                input.Close();
                return null;
            }
        }

        /// <summary>Replace the file method with stream</summary>
        /// <param name="temporaryIo">The read and write stream</param>
        internal bool Replace(RWStream temporaryIo)
        {
            if (!Opened) ReadBlocks();
            if (!temporaryIo.Accessed || (temporaryIo.Length > 0xFFFFFFFF)) return false;
            try
            {
                //getting the amount of blocks this file has
                var currentIoBlockCountFile = Functions.Functions.BlockCountFile(temporaryIo);
                //Getting the amount of blocks a new file should have
                var currentBlock = new List<BlockRecord>(_blocks);

                var newBlock = new List<BlockRecord>();
                for (var i = (int)currentIoBlockCountFile; i < currentBlock.Count; i++)
                {
                    newBlock.Add(currentBlock[i]);
                    currentBlock.RemoveAt(i--);
                }
                var flag = CurrentStfs.WriteEmbeddedFile(ref temporaryIo, currentBlock.ToArray());

                return (flag & ClearBlocks());
            }
            catch
            {
                ClearBlocks();
                return false;
            }
        }

        /// <summary>Read File Entry Blocks</summary>
        private bool ReadBlocks()
        {
            try
            {
                CurrentStfs.GetBlocks(AmountOfDataBlocks, StartingBlockNumberOfFile, out _blocks);
                if (_blocks.Length < AmountOfDataBlocks) ClearBlocks();
                return Opened;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Write the data into a temporary file</summary>
        /// <param name="input"> The I/O input stream</param>
        /// <returns>If true or flase depends if the operation was successful</returns>
        private bool EmbeddedDataExtraction(RWStream input)
        {
            try
            {
                if (!Opened && !ReadBlocks()) return false;
                input.Position = 0;
                uint index;
                for (index = 0; index < AmountOfDataBlocks; index++)
                {
                    CurrentStfs.MainIo.Position = CurrentStfs.GenerateDataOffset(_blocks[index].CurrentBlock);
                    input.WriteBytes((index < (AmountOfDataBlocks - 1)) ? CurrentStfs.MainIo.ReadBytes(0x1000) : CurrentStfs.MainIo.ReadBytes(((SizeOfFileInByte - 1) % 0x1000) + 1));
                }
                input.Flush();
                ClearBlocks();
                return true;
            }
            catch
            {
                ClearBlocks();
                return false;
            }
        }
        #endregion

        #region properties
        /// <summary>Returns if the file blocks are not empty or null</summary>
        private bool Opened
        {
            get
            {
                return ((_blocks != null) && (_blocks.Length > 0));
            }
        }
        #endregion
    }
    #endregion

    #region Folder Entries

    /// <summary>
    /// 
    /// </summary>
    public class FolderEntries : EmbeddedFileEntry
    {
        internal FolderEntries(EmbeddedFileEntry entry) : base(entry) { }

        internal FolderEntries(string inputName, int size, ushort id, ushort folderPathIndicator, Stfs stfsPackage)
            : base(inputName, size, true, id, folderPathIndicator, stfsPackage) { }
    }
    #endregion
}

