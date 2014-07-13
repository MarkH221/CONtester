using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Drawing;
using Isolib.Properties;

namespace Isolib.GPDPackage
{
    /// <summary>Class for Parsing GPD Data</summary>
	public class GPD : IDisposable
	{
		#region Variables
		private readonly long _fileLength;
		private MainHeaders _headers;
		private GameInfos _gameInfo;
		private FreeSpaceInfo _freespaceRecord;
		private bool _disposed;
		private TitleSpecifics1 _titleSpecific1;
		private TitleSpecifics2 _titleSpecific2;
		private TitleSpecifics3 _titleSpecific3;
		#endregion
		
		#region Structures
        /// <summary>Data Structure Containing the parsed data lists</summary>
		public struct DataList
		{
            /// <summary>
            /// list contains Entrydata
            /// </summary>
			static public List<Entries> _EntryList = null;
            /// <summary>
            /// list contains the Erchievements data
            /// </summary>
			static public List<Achievements> _argievements = null;
            /// <summary>
            /// list contains the first SyncInfo (Uint64)
            /// </summary>
			static public List<UInt64> _SyncList1 = null;
            /// <summary>
            /// list contains the second SyncInfo (Uint64)
            /// </summary>
			static public List<UInt64> _SyncList2 = null;
            /// <summary>
            /// list contains the Third SyncInfo (Uint64)
            /// </summary>
			static public List<UInt64> _SyncList3 = null;
            /// <summary>
            /// list contains the SyncData (Uint64)
            /// </summary>
			static public List<SyncData> _SyncData = null;
            /// <summary>
            /// list contains The TitleCredit Earned (Uint64)
            /// </summary>
			static public List<UInt64> _TitleCreditEarned = null;
            /// <summary>
            /// list contains the Title Archievement earned (Uint64)
            /// </summary>
			static public List<UInt64> _TitleArgievementEarned = null;
            /// <summary>
            /// list contains the FreespaceEntries
            /// </summary>
			static public List<FreeSpaceEntries> _FreeSpaceEntries = null;
		}
		
        /// <summary>
        /// Structure with FreespaceInfo
        /// </summary>
		 public struct FreeSpaceInfo
		{
             /// <summary>
             /// freespace total length
             /// </summary>
			public UInt32 TotalLength;
             /// <summary>
             /// freespace negative total length
             /// </summary>
			public UInt32 NegativeTotalLength;
		}
		
        /// <summary>
        /// structure with the main headers
        /// </summary>
		public struct MainHeaders
		{
            /// <summary>
            /// gpd magic
            /// </summary>
			public byte[] Magic;
            /// <summary>
            /// gpd version
            /// </summary>
            public UInt32 version;
            /// <summary>
            /// the entry table length
            /// </summary>
			public UInt32 EntryTableLength;
            /// <summary>
            /// total entries
            /// </summary>
			public UInt32 EntryCount;
            /// <summary>
            /// freespace table length
            /// </summary>
			public UInt32 FreeSpaceTableLength;
            /// <summary>
            /// freespace total entries
            /// </summary>
			public UInt32 FreespaceTableEntryCount;
		}
		
        /// <summary>
        /// All GPD Game Info
        /// </summary>
		public struct GameInfos
		{
            /// <summary>
            /// Game title name
            /// </summary>
			public string TitleName;
            /// <summary>
            /// unknown
            /// </summary>
			public string UnknownString;
            /// <summary>
            /// Game InfoName
            /// </summary>
			public string TitleInfoname;
            /// <summary>
            /// Game TitleID
            /// </summary>
			public string ID;
            /// <summary>
            /// Total Archievements
            /// </summary>
			public UInt32 ArgievementsCount;
            /// <summary>
            /// Total Archievements Unlocked
            /// </summary>
			public UInt32 ArgievementsUnlocked;
            /// <summary>
            /// Total GameScore
            /// </summary>
			public UInt32 TotalGameScore;
            /// <summary>
            /// Unlocked GameScore
            /// </summary>
			public UInt32 UnlockedGameScore;
            /// <summary>
            /// Unknown
            /// </summary>
			public UInt32 Unknown2;
            /// <summary>
            /// Unknown
            /// </summary>
			public UInt64 unknown1;
            /// <summary>
            /// Last Played (Uint64)
            /// </summary>
			public UInt64 LastPlayed;
            /// <summary>
            /// TitleIMage entry Index
            /// </summary>
			public int TitleImageIndex;
		}
		
        /// <summary>
        /// New GpdFile to parse
        /// </summary>
        /// <param name="GPDFile"></param>
		public GPD(string GPDFile)
		{
			
			_disposed = false;
			if (! File.Exists(GPDFile))
			{
				throw (new FileNotFoundException());
			}
			_fileLength = new FileInfo(GPDFile).Length;
			ParseGPD(File.ReadAllBytes(GPDFile));
		}
		
        /// <summary>
        /// New Gpd Array to parse
        /// </summary>
        /// <param name="Buffer"></param>
		public GPD(byte[] Buffer)
		{
			
			_disposed = false;
			if (Buffer == null || Buffer.Length == 0)
			{
				throw (new OutOfMemoryException("There is no Data !"));
			}
			_fileLength = Buffer.Length;
			ParseGPD(Buffer);
		}
		#endregion
		
		#region Enums
        /// <summary>
        /// TitleSpecifics
        /// </summary>
		public enum SaveData
		{
            /// <summary>
            /// First TitleSpecific
            /// </summary>
			TitleSpecific1,
            /// <summary>
            /// Second TitleSpecific
            /// </summary>
			TitleSpecific2,
            /// <summary>
            /// Third TitleSpecific
            /// </summary>
			TitleSpecific3
		}
		#endregion
		
		#region Functions
		#region Some converters
		
		private UInt64 ToUint64(byte[] Bytes, bool Bigendian = false)
		{
			if (Bigendian)
			{
				Array.Reverse(Bytes);
			}
			return BitConverter.ToUInt64(Bytes, 0);
		}
		
		private UInt32 ToUint32(byte[] Bytes, bool Bigendian = false)
		{
			if (Bigendian)
			{
				Array.Reverse(Bytes);
			}
			return BitConverter.ToUInt32(Bytes, 0);
		}
		
		private int ToInt32(byte[] Bytes, bool Bigendian = false)
		{
			if (Bigendian)
			{
				Array.Reverse(Bytes);
			}
			return BitConverter.ToInt32(Bytes, 0);
		}
		
		private UInt16 ToUint16(byte[] Bytes, bool Bigendian = false)
		{
			if (Bigendian)
			{
				Array.Reverse(Bytes);
			}
			return BitConverter.ToUInt16(Bytes, 0);
		}
		
		private short ToInt16(byte[] Bytes, bool Bigendian = false)
		{
			if (Bigendian)
			{
				Array.Reverse(Bytes);
			}
			return BitConverter.ToInt16(Bytes, 0);
		}
		
		private byte[] UInt32ToBytes(UInt32 value, bool Bigendian = false)
		{
			byte[] buffer = BitConverter.GetBytes(value);
			if (Bigendian)
			{
				Array.Reverse(buffer);
			}
			return buffer;
		}
		
		private byte[] Int32ToBytes(int value, bool Bigendian = false)
		{
			byte[] buffer = BitConverter.GetBytes((short) value);
			if (Bigendian)
			{
				Array.Reverse(buffer);
			}
			return buffer;
		}
		
		private byte[] UInt16ToBytes(UInt16 value, bool Bigendian = false)
		{
			byte[] buffer = BitConverter.GetBytes(value);
			if (Bigendian)
			{
				Array.Reverse(buffer);
			}
			return buffer;
		}
		
		private byte[] UInt64ToBytes(UInt64 value, bool Bigendian = false)
		{
			byte[] buffer = BitConverter.GetBytes(value);
			if (Bigendian)
			{
				Array.Reverse(buffer);
			}
			return buffer;
		}
		
		private byte[] Int64ToBytes(long value, bool Bigendian = false)
		{
			byte[] buffer = BitConverter.GetBytes(value);
			if (Bigendian)
			{
				Array.Reverse(buffer);
			}
			return buffer;
		}
		#endregion
		
		#region Private
		private int GetImageIndexById(UInt32 id)
		{
			for (int i = 0; i <= ArchievementsList.Count - 1; i++)
			{
				if (ArchievementsList[i].ImageID == id)
				{
					return System.Convert.ToInt32((uint) i);
				}
			}
			return 0;
		}
		
		private UInt32 GenerateTotalFreespaceLength(bool withheaders = false)
		{
			UInt32 x = 0;
			for (var i = 0; i <= FreeSpaceEntryList.Count - 1; i++)
			{
				x =  (uint) (x + FreeSpaceEntryList[i].Data.Length);
				if (withheaders)
				{
					x = x + 8;
				}
			}
			return x + 8;
		}
		
		private UInt32 GenerateTotalEntryLength(bool withheaders = false)
		{
			UInt32 x = 0;
			for (int i = 0; i <= EntryList.Count - 1; i++)
			{
				x += EntryList[i].Length;
				if (withheaders)
				{
					x = x + 14;
				}
			}
			return x;
		}
        #endregion
		
		#region Public
		/// <summary>Gets the TitleSpecific</summary>
		/// <param name="TitleSpecific"> The TitkeSpecific to extract</param>
		/// <returns>Extracted data</returns>
		public byte[] ExtractSaveData(SaveData TitleSpecific)
		{
			var buffer = new byte[] {};
			switch (TitleSpecific)
			{
				case SaveData.TitleSpecific1:
					if (TitleSpecific1 == null)
					{
						return null;
					}
					Array.Resize(ref buffer, System.Convert.ToInt32(EntryList[TitleSpecific1.EntryIndex].Data.Length - 24));
					Array.Copy(EntryList[TitleSpecific1.EntryIndex].Data, 24, buffer, 0, TitleSpecific1.Length);
					return buffer;
				case SaveData.TitleSpecific2:
					if (TitleSpecific1 == null)
					{
						return null;
					}
					Array.Resize(ref buffer, System.Convert.ToInt32(EntryList[TitleSpecific2.EntryIndex].Data.Length - 24));
					Array.Copy(EntryList[TitleSpecific2.EntryIndex].Data, 24, buffer, 0, TitleSpecific2.Length);
					return buffer;
				case SaveData.TitleSpecific3:
					if (TitleSpecific1 == null)
					{
						return null;
					}
					Array.Resize(ref buffer, System.Convert.ToInt32(EntryList[TitleSpecific2.EntryIndex].Data.Length - 24));
					Array.Copy(EntryList[TitleSpecific3.EntryIndex].Data, 24, buffer, 0, TitleSpecific3.Length);
					return buffer;
			}
			return null;
		}
		
		/// <summary>Gets the TitleSpecific</summary>
		/// <param name="TitleSpecific"> The TitkeSpecific to extract</param>
		/// <param name="Filepath">The Filepath to save the file</param>
		/// <returns>True if succes</returns>
		public bool ExtractSaveData(SaveData TitleSpecific, string Filepath)
		{
			if (Filepath == null || Filepath == "")
			{
				return false;
			}
			try
			{
				File.WriteAllBytes(Filepath, ExtractSaveData(TitleSpecific));
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		
		/// <summary>Extract Availible Savedata to the outpath</summary>
		/// <param name="outpath">Path To Extract the availible data</param>
		/// <returns>True if Succes</returns>
		public bool ExtractAllSavedata(string outpath)
		{
			if (! outpath.EndsWith("\\") || outpath.EndsWith("/"))
			{
				outpath = outpath + "\\";
			}
			try
			{
				if (TitleSpecific1 != null)
				{
					File.WriteAllBytes(outpath + "Save1.DAT", ExtractSaveData(SaveData.TitleSpecific1));
				}
				if (TitleSpecific2 != null)
				{
                    File.WriteAllBytes(outpath + "Save1.DAT", ExtractSaveData(SaveData.TitleSpecific2));
				}
				if (TitleSpecific3 != null)
				{
                    File.WriteAllBytes(outpath + "Save3.DAT", ExtractSaveData(SaveData.TitleSpecific3));
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
		
		/// <summary>Extract All Savedata Combines Together</summary>
		/// <returns>The Combined Data</returns>
		public byte[] ExtractCombinedSaves()
		{
			byte[] buffer = new byte[] {};
			Array.Resize(ref buffer, SaveDataTotalLength());
			using (BinaryWriter w = new BinaryWriter(new MemoryStream(buffer)))
			{
				if (TitleSpecific1 != null)
				{
					w.Write(ExtractSaveData(SaveData.TitleSpecific1));
				}
				if (TitleSpecific2 != null)
				{
					w.Write(ExtractSaveData(SaveData.TitleSpecific2));
				}
				if (TitleSpecific3 != null)
				{
					w.Write(ExtractSaveData(SaveData.TitleSpecific3));
				}
				w.Flush();
				w.Close();
			}
			
			return buffer;
		}
		
		/// <summary>Extract All Savedata Combines Together</summary>
		/// <param name="filepath">Filepath To write the data </param>
		/// <returns>True if Succes</returns>
		public bool ExtractCombinedSaves(string filepath)
		{
			if (filepath == null || filepath == "")
			{
				return false;
			}
			File.WriteAllBytes(filepath, ExtractCombinedSaves());
			return true;
		}
		
		/// <summary>Extract the EntryData</summary>
		/// <param name="EntryIndex">The Entry Index</param>
		/// <returns>The extracted Entry Data</returns>
		public byte[] ExtractEntryData(int EntryIndex)
		{
			if (EntryIndex > EntryList.Count)
			{
				throw (new IndexOutOfRangeException());
			}
			return EntryList[EntryIndex].Data;
		}
		
		/// <summary>Extract the EntryData</summary>
		/// <param name="EntryIndex">The Entry Index</param>
		/// <param name="Filepath">The FilePath to write the data </param>
		/// <returns>True if Succes</returns>
		public bool ExtractEntryData(int EntryIndex, string Filepath)
		{
			if (EntryIndex > ArchievementsList.Count)
			{
				return false;
			}
			if (Filepath == null || Filepath == "")
			{
				return false;
			}
			try
			{
				File.WriteAllBytes(Filepath, ExtractEntryData(EntryIndex));
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		
		/// <summary>Extract the EntryData</summary>
		/// <param name="outpath">Path to Extract all EntryData</param>
		/// <returns>True if Succes</returns>
		public bool ExtractAllEntries(string outpath)
		{
			if (outpath == null || outpath == "")
			{
				return false;
			}
			if (! outpath.EndsWith("\\") || outpath.EndsWith("/"))
			{
				outpath = outpath + "\\";
			}
			try
			{
				for (int i = 0; i <= EntryList.Count - 1; i++)
				{
					File.WriteAllBytes(outpath + "\\Entry(" + i + ")", ExtractEntryData(i));
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		
		/// <summary>Extracts the TitleImage</summary>
		/// <returns>Image</returns>
		public Image ExtractTitleImage()
		{
            if (EntryList[_gameInfo.TitleImageIndex].Data.Length == 0) return Resources.noImage;
			return Image.FromStream(new MemoryStream(EntryList[_gameInfo.TitleImageIndex].Data));
		}
		
		/// <summary>
		/// Extracts the TitleImage
		/// </summary>
		/// <param name="FilePath">The Filepath to write the Image</param>
		/// <returns>True if succes</returns>
		/// <remarks></remarks>
		public bool ExtractTitleImage(string FilePath)
		{
			if (FilePath == null || FilePath == "")
			{
				return false;
			}
			try
			{
				ExtractTitleImage().Save(FilePath);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		
		/// <summary>
		/// Extract The ArchievementImage
		/// </summary>
		/// <param name="ImageIndex">The Index for the archievementImage</param>
		/// <returns>the extracted Image</returns>
		/// <remarks></remarks>
		public Image ExtractArchievementImage(int ImageIndex)
		{
			if (ImageIndex > ArchievementsList.Count)
			{
				return null;
			}
			if (ArchievementsList[ImageIndex].ImageEntryIndex == -1)
			{
				return Resources.noImage;
			}
			return Image.FromStream(new MemoryStream(EntryList[System.Convert.ToInt32(ArchievementsList[ImageIndex].ImageEntryIndex)].Data));
		}
		
		/// <summary>
		/// Extract The ArchievementImage
		/// </summary>
		/// <param name="ImageIndex">The Index for the archievementImage</param>
		/// <param name="Filepath">The Filepath to write the Image</param>
		/// <returns>True if Succes</returns>
		/// <remarks></remarks>
		public bool ExtractArchievementImage(int ImageIndex, string Filepath)
		{
			if (ImageIndex > ArchievementsList.Count)
			{
				return false;
			}
			if (Filepath == null || Filepath == "")
			{
				return false;
			}
			try
			{
				ExtractArchievementImage(ImageIndex).Save(Filepath);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		
		/// <summary>
		/// Extract All ArchievementsImages
		/// </summary>
		/// <param name="outpath">The Path to extract the images to</param>
		/// <returns>True if Succes</returns>
		/// <remarks></remarks>
		public bool ExtractAllArchievementsImages(string outpath)
		{
			if (outpath == null || outpath == "")
			{
				return false;
			}
			if (! outpath.EndsWith("\\") || outpath.EndsWith("/"))
			{
				outpath = outpath + "\\";
			}
			try
			{
				for (int i = 0; i <= ArchievementsList.Count - 1; i++)
				{
					ExtractArchievementImage(i).Save(outpath + System.Convert.ToString((ArchievementsList[i].ImageID)));
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		
		/// <summary>
		/// Extract the FreeSpacedata
		/// </summary>
		/// <param name="index">Index for the FreespaceBlock to extract</param>
		/// <returns>FreespaceData</returns>
		/// <remarks></remarks>
		public byte[] ExtractFreeSpaceBlock(int index)
		{
			if (index > FreeSpaceEntryList.Count)
			{
				throw (new IndexOutOfRangeException());
			}
			return FreeSpaceEntryList[index].Data;
		}
		
		/// <summary>
		/// Extract the FreeSpacedata
		/// </summary>
		/// <param name="index">Index for the FreespaceBlock to extract</param>
		/// <param name="Filepath">The Filepath to write the data</param>
		/// <returns>True if Succes</returns>
		/// <remarks></remarks>
		public bool ExtractFreeSpaceBlock(int index, string Filepath)
		{
			if (index > FreeSpaceEntryList.Count)
			{
				return false;
			}
			if (string.IsNullOrEmpty(Filepath))
			{
				return false;
			}
			File.WriteAllBytes(Filepath, ExtractFreeSpaceBlock(index));
			return true;
		}
		
		/// <summary>
		/// Extracts all freespacedata to the outpath
		/// </summary>
		/// <param name="Outpath">Path to Extract all Freespace Data</param>
		/// <returns>True if Succes</returns>
		/// <remarks></remarks>
		public bool ExtractAllFreeSpaceBlocks(string Outpath)
		{
			if (string.IsNullOrEmpty(Outpath))
			{
				return false;
			}
			if (! Outpath.EndsWith("\\") || Outpath.EndsWith("/"))
			{
				Outpath = Outpath + "\\";
			}
			try
			{
				for (int i = 0; i <= FreeSpaceEntryList.Count - 1; i++)
				{
					File.WriteAllBytes(Outpath + "FreeSpaceBlock(" + i.ToString() + ")", ExtractFreeSpaceBlock(i));
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		
		/// <summary>
		/// Replace the EntryData
		/// </summary>
		/// <param name="index">the Index for the entry to replace</param>
		/// <param name="data">the data for replacing</param>
		/// <returns>True if succes</returns>
		/// <remarks></remarks>
		public bool ReplaceEntryData(int index, byte[] data)
		{
			if (EntryList == null || index > EntryList.Count)
			{
				return false;
			}
			EntryList[index].Data = data;
			return true;
		}
		
		/// <summary>
		/// Replace the Freespace Entry Data
		/// </summary>
		/// <param name="index">Index for the freespace to replace</param>
		/// <param name="data">the data for replacing</param>
		/// <returns>True if Succes</returns>
		/// <remarks></remarks>
		public bool ReplaceFreeSpaceEntryData(int index, byte[] data)
		{
			if (FreeSpaceEntryList == null || index > FreeSpaceEntryList.Count)
			{
				return false;
			}
			FreeSpaceEntryList[index].Data = data;
			return true;
		}
		
		/// <summary>
		/// Replace the savedata
		/// </summary>
		/// <param name="data">The data for Replacing</param>
		/// <param name="Titlespecific">The TitleSpecific to replace</param>
		/// <returns>True if Succes</returns>
		/// <remarks></remarks>
		public bool ReplaceSavedata(byte[] data, SaveData Titlespecific)
		{
			if (data == null || data.Length == 0)
			{
				return false;
			}
			var s = new MemoryStream();
			using (var w = new BinaryWriter(s))
			{
				switch (Titlespecific)
				{
					case SaveData.TitleSpecific1:
						if (TitleSpecific1 == null)
						{
							return false;
						}
						s = new MemoryStream();
						w.Write(UInt32ToBytes(TitleSpecific1.ID, true));
						w.Write(UInt32ToBytes(TitleSpecific1.Value1, true));
						w.Write(UInt64ToBytes(TitleSpecific1.Value2, true));
						w.Write(Int32ToBytes(data.Length, true));
						w.Write(UInt64ToBytes(TitleSpecific1.Value3, true));
						w.Write(data);
						ReplaceEntryData(TitleSpecific1.EntryIndex, s.ToArray());
						break;
					case SaveData.TitleSpecific2:
						if (TitleSpecific2 == null)
						{
							return false;
						}
						s = new MemoryStream();
						w.Write(UInt32ToBytes(TitleSpecific2.ID, true));
						w.Write(UInt32ToBytes(TitleSpecific2.Value1, true));
						w.Write(UInt64ToBytes(TitleSpecific2.Value2, true));
						w.Write(Int32ToBytes(data.Length, true));
						w.Write(UInt64ToBytes(TitleSpecific2.Value3, true));
						w.Write(data);
						ReplaceEntryData(TitleSpecific2.EntryIndex, s.ToArray());
						break;
					case SaveData.TitleSpecific3:
						if (TitleSpecific3 == null)
						{
							return false;
						}
						s = new MemoryStream();
						w.Write(UInt32ToBytes(TitleSpecific3.ID, true));
						w.Write(UInt32ToBytes(TitleSpecific3.Value1, true));
						w.Write(UInt64ToBytes(TitleSpecific3.Value2, true));
						w.Write(Int32ToBytes(data.Length, true));
						w.Write(UInt64ToBytes(TitleSpecific3.Value3, true));
						w.Write(data);
						ReplaceEntryData(TitleSpecific3.EntryIndex, s.ToArray());
						break;
					default:
						return false;
				}
                w.Close();  
			}
			s.Close();
			return true;
		}
		
		/// <summary>
		/// gets the total Savelength availible
		/// </summary>
		/// <returns>the Total Length</returns>
		/// <remarks></remarks>
		public int SaveDataTotalLength()
		{
			long size = 0;
			if (TitleSpecific1 != null)
			{
				size += EntryList[TitleSpecific1.EntryIndex].Data.Length - 24;
			}
			if (TitleSpecific2 != null)
			{
				size += EntryList[TitleSpecific2.EntryIndex].Data.Length - 24;
			}
			if (TitleSpecific3 != null)
			{
				size += EntryList[TitleSpecific3.EntryIndex].Data.Length - 24;
			}
			return (int) size;
		}
		
		/// <summary>
		/// Gets the Rebuilded Gpd
		/// </summary>
		/// <returns>New gpd Data</returns>
		/// <remarks></remarks>
		public byte[] GetRebuild()
		{
			if (EntryList == null || EntryList.Count == 0)
			{
				return null;
			}
		    var buffer = new byte[(int) (_fileLength - 1) + 1];
			using (var writer = new BinaryWriter(new MemoryStream(buffer)))
			{
				writer.Write(_headers.Magic);
				writer.Write(UInt32ToBytes(_headers.version, true));
				writer.Write(UInt32ToBytes(_headers.EntryTableLength, true));
				writer.Write(UInt32ToBytes(_headers.EntryCount, true));
				writer.Write(UInt32ToBytes(_headers.FreeSpaceTableLength, true));
				writer.Write(UInt32ToBytes(_headers.FreespaceTableEntryCount, true));
			    long last;
			    for (int i = 0; i <= EntryList.Count - 1; i++)
				{
					writer.Write(UInt16ToBytes(EntryList[i].XNameSpace, true));
					writer.Write(UInt64ToBytes(EntryList[i].ID, true));
					writer.Write(UInt32ToBytes(System.Convert.ToUInt32(EntryList[i].Offset), true));
					writer.Write(UInt32ToBytes(EntryList[i].Length, true));
					last = writer.BaseStream.Position;
					writer.BaseStream.Position = EntryList[i].Offset + ((_headers.EntryTableLength * 18) + (_headers.FreeSpaceTableLength * 8) + 24);
					writer.Write(GenerateNewEntry(i));
					writer.BaseStream.Position = last;
				}
				writer.BaseStream.Position = 0x18 + (_headers.EntryTableLength * 18);
				for (int i = 0; i <= FreeSpaceEntryList.Count - 1; i++)
				{
					writer.Write(UInt32ToBytes(System.Convert.ToUInt32(FreeSpaceEntryList[i].Offset), true));
					writer.Write(UInt32ToBytes(FreeSpaceEntryList[i].Length, true));
					last = writer.BaseStream.Position;
					writer.BaseStream.Position = FreeSpaceEntryList[i].Offset + (((_headers.EntryTableLength * 18) + (_headers.FreeSpaceTableLength * 8)) + 24);
					writer.Write(System.Convert.ToBoolean(FreeSpaceEntryList[i].Data));
					writer.BaseStream.Position = last;
				}
				writer.Write(UInt32ToBytes(_freespaceRecord.TotalLength, true));
				writer.Write(UInt32ToBytes(_freespaceRecord.NegativeTotalLength, true));
				writer.Flush();
				writer.Close();
			}
			
			return buffer;
		}
		
		/// <summary>
		/// Writes the Rebuilded gpd data to the filepath
		/// </summary>
		/// <param name="filepath">The filepath to write the rebuilded data</param>
		/// <returns>True if Succes</returns>
		/// <remarks></remarks>
		public bool RebuildFile(string filepath)
		{
			if (string.IsNullOrEmpty(filepath))
			{
				return false;
			}
			if (! File.Exists(filepath))
			{
				return false;
			}
			File.WriteAllBytes(filepath, GetRebuild());
			return true;
		}
		#endregion
		#endregion
		
		#region Private Subs
		private void ParseGPD(byte[] file)
		{
			using (var Reader = new BinaryReader(new MemoryStream(file)))
			{
				//Getting Header Info
				Setnew();
				var magic = new byte[] {0x58, 0x44, 0x42, 0x46};
				_headers.Magic = Reader.ReadBytes(4);
				for (int i = 0; i <= magic.Length  - 1; i++)
				{
					if (_headers.Magic[i] != magic[i])
					{
						throw (new InvalidDataException("Invalid GPD Data"));
					}
				}
				_headers.version = ToUint32(Reader.ReadBytes(4), true);
				_headers.EntryTableLength = ToUint32(Reader.ReadBytes(4), true);
				_headers.EntryCount = ToUint32(Reader.ReadBytes(4), true);
				_headers.FreeSpaceTableLength = ToUint32(Reader.ReadBytes(4), true);
				_headers.FreespaceTableEntryCount = ToUint32(Reader.ReadBytes(4), true);
				//Getting Entry Tables
			    for (long i = 0; i <= _headers.EntryCount - 1; i++)
				{
					UInt16 space = ToUint16(Reader.ReadBytes(2), true);
					UInt64 id = ToUint64(Reader.ReadBytes(8), true);
					long offset = ToUint32(Reader.ReadBytes(4), true);
					UInt32 length = ToUint32(Reader.ReadBytes(4), true);
					long last = Reader.BaseStream.Position;
					long realoffset = offset + ((_headers.EntryTableLength * 18) + (_headers.FreeSpaceTableLength * 8) + 24);
					Reader.BaseStream.Position = realoffset;
					byte[] data = Reader.ReadBytes(System.Convert.ToInt32(length));
					Reader.BaseStream.Position = last;
					DataList._EntryList.Add(new Entries(space, id, offset, length, data)); //adding entryinfo into a list.
					EntryList[(int) i].BlockOffset = realoffset;
					ReadNameSpace((int) i); //gettings Namespace Info
				}
				//Listing the FreeSpace Tables here...
				Reader.BaseStream.Position = 0x18 + (_headers.EntryTableLength  * 18);
			    for (int i = 0; i <= (int) (_headers.FreespaceTableEntryCount - 2); i++)
				{
					var a = ToUint32(Reader.ReadBytes(4), true);
					UInt32 b = ToUint32(Reader.ReadBytes(4), true);
					long d = Reader.BaseStream.Position;
                    Reader.BaseStream.Position = a + (((_headers.EntryTableLength * 18) + (_headers.FreeSpaceTableLength * 8)) + 24);
					byte[] c = Reader.ReadBytes(System.Convert.ToInt32(b));
					Reader.BaseStream.Position = d;
					DataList._FreeSpaceEntries.Add(new FreeSpaceEntries(i, a, b, c));
				}
				_freespaceRecord.TotalLength = ToUint32(Reader.ReadBytes(4), true);
				_freespaceRecord.NegativeTotalLength = ToUint32(Reader.ReadBytes(4), true);
				Reader.Close();
			}
			
		}
		
		private void ReadNameSpace(int EntryIndex)
		{
			//Variables for the argievos
			UInt32 _Magic;
			UInt32 _AchievementID;
			UInt32 _ImageID;
			UInt32 _Flags;
			int _Gamerscore;
			long _UnlockTime;
			string _Name;
			string _LockedDescription;
			string _UnlockedDescription;
			using (BinaryReader Reader = new BinaryReader(new MemoryStream(DataList._EntryList[EntryIndex].Data)))
			{
				if (DataList._EntryList[EntryIndex].XNameSpace == 1) //Argievements
				{
					if (DataList._EntryList[EntryIndex].ID == 0x100000000) // SyncList
					{
						for (int i = 0; i <= DataList._EntryList[EntryIndex].Data.Length - 1; i += + 8)
						{
							DataList._SyncList1.Add(ToUint64(Reader.ReadBytes(8), true));
						}
					} // SyncData
					else if (DataList._EntryList[EntryIndex].ID == 0x200000000)
					{
						DataList._SyncData.Add(new SyncData(EntryIndex, ToUint64(Reader.ReadBytes(8), true), ToUint64(Reader.ReadBytes(8), true), ToUint64(Reader.ReadBytes(8), true)));
					} //argievos
					else
					{
						_Magic = ToUint32(Reader.ReadBytes(4), true);
						_AchievementID = ToUint32(Reader.ReadBytes(4), true);
						_ImageID = ToUint32(Reader.ReadBytes(4), true);
						_Gamerscore = ToInt32(Reader.ReadBytes(4), true);
						_Flags = ToUint32(Reader.ReadBytes(4), true);
						_UnlockTime = (int) (ToUint64(Reader.ReadBytes(8), true));
						int count = 0;
						long last;
						last = Reader.BaseStream.Position;
						while (ToInt16(Reader.ReadBytes(2)) != 0)
						{
							count += 2;
						}
						Reader.BaseStream.Position = last;
						_Name = System.Convert.ToString(Encoding.BigEndianUnicode.GetChars(Reader.ReadBytes(count)));
						Reader.BaseStream.Position += 2;
						last = Reader.BaseStream.Position;
						count = 0;
						while (ToInt16(Reader.ReadBytes(2)) != 0)
						{
							count += 2;
						}
						Reader.BaseStream.Position = last;
						_LockedDescription = System.Convert.ToString(Encoding.BigEndianUnicode.GetChars(Reader.ReadBytes(count)));
						Reader.BaseStream.Position += 2;
						last = Reader.BaseStream.Position;
						count = 0;
						while (ToInt16(Reader.ReadBytes(2)) != 0)
						{
							count += 2;
						}
						Reader.BaseStream.Position = last;
						_UnlockedDescription = System.Convert.ToString(Encoding.BigEndianUnicode.GetChars(Reader.ReadBytes(count)));
						DataList._argievements.Add(new Achievements(EntryIndex, _Magic, _AchievementID, _ImageID, _Flags, _Gamerscore, _UnlockTime, _Name, _LockedDescription, _UnlockedDescription));
					}
				} //Image
				else if (DataList._EntryList[EntryIndex].XNameSpace == 2)
				{
					if (DataList._EntryList[EntryIndex].ID == 0x8000)
					{
						_gameInfo.TitleImageIndex = EntryIndex;
					}
					else
					{
						if (ArchievementsList.Count > 0)  ArchievementsList[GetImageIndexById(System.Convert.ToUInt32(DataList._EntryList[EntryIndex].ID))].ImageEntryIndex = EntryIndex;
					}
				} //Settings
				else if (DataList._EntryList[EntryIndex].XNameSpace == 3)
				{
					if (DataList._EntryList[EntryIndex].ID == 0x10040038) //Gamerscore Earned On Title
					{
						for (int i = 0; i <= DataList._EntryList[EntryIndex].Data.Length - 1; i += + 8)
						{
							DataList._TitleCreditEarned.Add(ToUint64(Reader.ReadBytes(8), true));
						}
					} //Achievements Unlocked on Title
					else if (DataList._EntryList[EntryIndex].ID == 0x10040039)
					{
						for (int i = 0; i <= DataList._EntryList[EntryIndex].Data.Length - 1; i += + 8)
						{
							DataList._TitleArgievementEarned.Add(ToUint64(Reader.ReadBytes(8), true));
						}
					} //Sync ID Table
					else if (DataList._EntryList[EntryIndex].ID == 0x100000000)
					{
						for (int i = 0; i <= DataList._EntryList[EntryIndex].Data.Length - 1; i += + 8)
						{
							DataList._SyncList2.Add(ToUint64(Reader.ReadBytes(8), true));
						}
					} //Sync Record
					else if (DataList._EntryList[EntryIndex].ID == 0x200000000)
					{
						DataList._SyncData.Add(new SyncData(EntryIndex, ToUint64(Reader.ReadBytes(8), true), ToUint64(Reader.ReadBytes(8), true), ToUint64(Reader.ReadBytes(8), true)));
					} // TitleSpecific1
					else if (DataList._EntryList[EntryIndex].ID == 0x63E83FFF)
					{
						UInt32 a;
						UInt32 b;
						UInt64 c;
						UInt32 d;
						UInt32 e;
						long f;
						a = ToUint32(Reader.ReadBytes(4), true);
						b = ToUint32(Reader.ReadBytes(4), true);
						c = ToUint64(Reader.ReadBytes(8), true);
						d = ToUint32(Reader.ReadBytes(4), true);
						e = ToUint32(Reader.ReadBytes(4), true);
						f = DataList._EntryList[EntryIndex].Offset + 24;
						_titleSpecific1 = new TitleSpecifics1(EntryIndex, a, b, c, d, e, f);
					} // TitleSpecific2
					else if (DataList._EntryList[EntryIndex].ID == 0x63E83FFE)
					{
						UInt32 a;
						UInt32 b;
						UInt64 c;
						UInt32 d;
						UInt32 e;
						long f;
						a = ToUint32(Reader.ReadBytes(4), true);
						b = ToUint32(Reader.ReadBytes(4), true);
						c = ToUint64(Reader.ReadBytes(8), true);
						d = ToUint32(Reader.ReadBytes(4), true);
						e = ToUint32(Reader.ReadBytes(4), true);
						f = DataList._EntryList[EntryIndex].Offset + 24;
						_titleSpecific2 = new TitleSpecifics2(EntryIndex, a, b, c, d, e, f);
					} // TitleSpecific3
					else if (DataList._EntryList[EntryIndex].ID == 0x63E83FFD)
					{
						UInt32 a;
						UInt32 b;
						UInt64 c;
						UInt32 d;
						UInt32 e;
						long f;
						a = ToUint32(Reader.ReadBytes(4), true);
						b = ToUint32(Reader.ReadBytes(4), true);
						c = ToUint64(Reader.ReadBytes(8), true);
						d = ToUint32(Reader.ReadBytes(4), true);
						e = ToUint32(Reader.ReadBytes(4), true);
						f = DataList._EntryList[EntryIndex].Offset + 24;
						_titleSpecific3 = new TitleSpecifics3(EntryIndex, a, b, c, d, e, f);
					}
				} //Title
				else if (DataList._EntryList[EntryIndex].XNameSpace == 4)
				{
					if (DataList._EntryList[EntryIndex].ID == 0x100000000) // SyncList
					{
						for (int i = 0; i <= DataList._EntryList[EntryIndex].Data.Length - 1; i += + 8)
						{
							DataList._SyncList3.Add(ToUint64(Reader.ReadBytes(8), true));
						}
					} // SyncData
					else if (DataList._EntryList[EntryIndex].ID == 0x200000000)
					{
						DataList._SyncData.Add(new SyncData(EntryIndex, ToUint64(Reader.ReadBytes(8), true), ToUint64(Reader.ReadBytes(8), true), ToUint64(Reader.ReadBytes(8), true)));
					}
					else
					{
						_gameInfo.ID = BitConverter.ToString(Reader.ReadBytes(4)).Replace("-","");
						_gameInfo.ArgievementsCount = ToUint32(Reader.ReadBytes(4), true);
						_gameInfo.ArgievementsUnlocked = ToUint32(Reader.ReadBytes(4), true);
						_gameInfo.TotalGameScore = ToUint32(Reader.ReadBytes(4), true);
						_gameInfo.UnlockedGameScore = ToUint32(Reader.ReadBytes(4), true);
						_gameInfo.unknown1 = ToUint64(Reader.ReadBytes(8), true);
						_gameInfo.Unknown2 = ToUint32(Reader.ReadBytes(4), true);
						_gameInfo.LastPlayed = ToUint64(Reader.ReadBytes(8), true);
						long last = Reader.BaseStream.Position;
						int count = 0;
						while (ToUint16(Reader.ReadBytes(2)) != 0)
						{
							count += 2;
						}
						Reader.BaseStream.Position = last;
						_gameInfo.TitleName = System.Convert.ToString(Encoding.BigEndianUnicode.GetChars(Reader.ReadBytes(count)));
					}
				} //Strings
				else if (DataList._EntryList[EntryIndex].XNameSpace == 5)
				{
					if (DataList._EntryList[EntryIndex].ID == 0x8000) // Title Information
					{
						_gameInfo.TitleInfoname = Encoding.BigEndianUnicode.GetString(DataList._EntryList[EntryIndex].Data, 0, System.Convert.ToInt32(DataList._EntryList[EntryIndex].Length));
					}
					else
					{
						_gameInfo.UnknownString = Encoding.BigEndianUnicode.GetString(DataList._EntryList[EntryIndex].Data, 0, System.Convert.ToInt32(DataList._EntryList[EntryIndex].Length));
					}
				} //Argievements Security
				else if (DataList._EntryList[EntryIndex].XNameSpace == 6)
				{
					Console.WriteLine("Case" + EntryIndex.ToString());
				}
				Reader.Close();
			}
			
		}
		
		private byte[] GenerateNewEntry(int EntryIndex)
		{
			byte[] buffer;
			using (MemoryStream stream = new MemoryStream())
			{
				using (BinaryWriter w = new BinaryWriter(stream))
				{
					if (DataList._EntryList[EntryIndex].XNameSpace == 1) //Argievements
					{
						if (DataList._EntryList[EntryIndex].ID == 0x100000000) // SyncList
						{
							for (int i = 0; i <= SyncList1.Count - 1; i++)
							{
								w.Write(UInt64ToBytes(SyncList1[i], true));
							}
						} // SyncData
						else if (DataList._EntryList[EntryIndex].ID == 0x200000000)
						{
							w.Write(UInt64ToBytes(DataList._SyncData[0].NextSyncID, true));
							w.Write(UInt64ToBytes(DataList._SyncData[0].LastSyncID, true));
							w.Write(UInt64ToBytes(DataList._SyncData[0].LastSyncTime, true));
						} //argievos
						else
						{
							w.Write(UInt32ToBytes(ArchievementsList[EntryIndex].Magic, true));
							w.Write(UInt32ToBytes(ArchievementsList[EntryIndex].ID, true));
							w.Write(UInt32ToBytes(ArchievementsList[EntryIndex].ImageID, true));
							w.Write(Int32ToBytes(System.Convert.ToInt32(ArchievementsList[EntryIndex].GameScore), true));
							w.Write(UInt32ToBytes(ArchievementsList[EntryIndex].Flags, true));
							w.Write(UInt64ToBytes(System.Convert.ToUInt64(ArchievementsList[EntryIndex].UnlockTime), true));
							w.Write(Encoding.BigEndianUnicode.GetBytes(ArchievementsList[EntryIndex].Name));
							w.Write(new byte[] {0, 0});
							w.Write(Encoding.BigEndianUnicode.GetBytes(ArchievementsList[EntryIndex].LockedDescription));
							w.Write(new byte[] {0, 0});
							w.Write(Encoding.BigEndianUnicode.GetBytes(ArchievementsList[EntryIndex].UnlockedDescription));
							w.Write(new byte[] {0, 0});
						}
					}
					else if (DataList._EntryList[EntryIndex].XNameSpace == 2)
					{
						if (DataList._EntryList[EntryIndex].ID == 0x8000) // Information
						{
							return EntryList[_gameInfo.TitleImageIndex].Data;
						}
						else
						{
							return EntryList[ArchievementsList[GetImageIndexById(System.Convert.ToUInt32(DataList._EntryList[EntryIndex].ID))].ImageEntryIndex].Data;
						}
					} //Settings
					else if (DataList._EntryList[EntryIndex].XNameSpace == 3)
					{
						if (DataList._EntryList[EntryIndex].ID == 0x10040038) //Gamerscore Earned On Title
						{
							for (int i = 0; i <= DataList._TitleCreditEarned.Count - 1; i++)
							{
								w.Write(UInt64ToBytes(DataList._TitleCreditEarned[i], true));
							}
						} //Achievements Unlocked on Title
						else if (DataList._EntryList[EntryIndex].ID == 0x10040039)
						{
							for (int i = 0; i <= DataList._TitleArgievementEarned.Count - 1; i++)
							{
								w.Write(UInt64ToBytes(DataList._TitleArgievementEarned[i], true));
							}
						} //Sync ID Table
						else if (DataList._EntryList[EntryIndex].ID == 0x100000000)
						{
							for (int i = 0; i <= DataList._SyncList2.Count - 1; i++)
							{
								w.Write(UInt64ToBytes(DataList._SyncList2[i], true));
							}
						} // SyncData
						else if (DataList._EntryList[EntryIndex].ID == 0x200000000)
						{
							w.Write(UInt64ToBytes(DataList._SyncData[1].NextSyncID, true));
							w.Write(UInt64ToBytes(DataList._SyncData[1].LastSyncID, true));
							w.Write(UInt64ToBytes(DataList._SyncData[1].LastSyncTime, true));
						} // TitleSpecific1
						else if (DataList._EntryList[EntryIndex].ID == 0x63E83FFF)
						{
							byte[] X = ExtractSaveData(SaveData.TitleSpecific1);
							w.Write(UInt32ToBytes(TitleSpecific1.ID, true));
							w.Write(UInt32ToBytes(TitleSpecific1.Value1, true));
							w.Write(UInt64ToBytes(TitleSpecific1.Value2, true));
							w.Write(Int32ToBytes(X.Length, true));
							w.Write(UInt32ToBytes(TitleSpecific1.Value3, true));
							w.Write(X);
						} // TitleSpecific2
						else if (DataList._EntryList[EntryIndex].ID == 0x63E83FFE)
						{
							byte[] X = ExtractSaveData(SaveData.TitleSpecific2);
							w.Write(UInt32ToBytes(TitleSpecific2.ID, true));
							w.Write(UInt32ToBytes(TitleSpecific2.Value1, true));
							w.Write(UInt64ToBytes(TitleSpecific2.Value2, true));
							w.Write(Int32ToBytes(X.Length, true));
							w.Write(UInt32ToBytes(TitleSpecific2.Value3, true));
							w.Write(X);
						} // TitleSpecific3
						else if (DataList._EntryList[EntryIndex].ID == 0x63E83FFD)
						{
							byte[] X = ExtractSaveData(SaveData.TitleSpecific3);
							w.Write(UInt32ToBytes(TitleSpecific3.ID, true));
							w.Write(UInt32ToBytes(TitleSpecific3.Value1, true));
							w.Write(UInt64ToBytes(TitleSpecific3.Value2, true));
							w.Write(Int32ToBytes(X.Length, true));
							w.Write(UInt32ToBytes(TitleSpecific3.Value3, true));
							w.Write(X);
						}
					} //Title
					else if (DataList._EntryList[EntryIndex].XNameSpace == 4)
					{
						if (DataList._EntryList[EntryIndex].ID == 0x100000000) // SyncList
						{
							for (int i = 0; i <= SyncList3.Count - 1; i++)
							{
								w.Write(UInt64ToBytes(DataList._SyncList3[i], true));
							}
						} // SyncData
						else if (DataList._EntryList[EntryIndex].ID == 0x200000000)
						{
							w.Write(UInt64ToBytes(DataList._SyncData[2].NextSyncID, true));
							w.Write(UInt64ToBytes(DataList._SyncData[2].LastSyncID, true));
							w.Write(UInt64ToBytes(DataList._SyncData[2].LastSyncTime, true));
						}
						else
						{
							w.Write(Encoding.ASCII.GetBytes(_gameInfo.ID));
							w.Write(UInt32ToBytes(_gameInfo.ArgievementsCount, true));
							w.Write(UInt32ToBytes(_gameInfo.ArgievementsUnlocked, true));
							w.Write(UInt32ToBytes(_gameInfo.TotalGameScore, true));
							w.Write(UInt32ToBytes(_gameInfo.UnlockedGameScore, true));
							w.Write(UInt64ToBytes(_gameInfo.unknown1, true));
							w.Write(UInt32ToBytes(_gameInfo.Unknown2, true));
							w.Write(UInt64ToBytes(_gameInfo.LastPlayed, true));
							w.Write(Encoding.BigEndianUnicode.GetBytes(_gameInfo.TitleName));
						}
					} //Strings
					else if (DataList._EntryList[EntryIndex].XNameSpace == 5)
					{
						if (DataList._EntryList[EntryIndex].ID == 0x8000) // Title Information
						{
							w.Write(Encoding.BigEndianUnicode.GetBytes(_gameInfo.TitleInfoname));
						}
						else
						{
							w.Write(Encoding.BigEndianUnicode.GetBytes(_gameInfo.UnknownString));
						}
					} //Argievements Security
					else if (DataList._EntryList[EntryIndex].XNameSpace == 6)
					{
						Console.WriteLine("Case" + EntryIndex.ToString());
					}
					w.Flush();
					w.Close();
					buffer = stream.ToArray();
					stream.Dispose();
					stream.Close();
				}
			}
			
			return buffer;
		}
		
		private void Setnew()
		{
			DataList._EntryList = new List<Entries>();
			DataList._argievements = new List<Achievements>();
			DataList._SyncList1 = new List<UInt64>();
			DataList._SyncList2 = new List<UInt64>();
			DataList._SyncList3 = new List<UInt64>();
			DataList._SyncData = new List<SyncData>();
			DataList._TitleCreditEarned = new List<UInt64>();
			DataList._TitleArgievementEarned = new List<UInt64>();
			DataList._FreeSpaceEntries = new List<FreeSpaceEntries>();
			_headers = new MainHeaders();
			
			_gameInfo = new GameInfos();
		}
		
		private void Clear()
		{
			DataList._EntryList.Clear();
			DataList._argievements.Clear();
			DataList._SyncList1.Clear();
			DataList._SyncList2.Clear();
			DataList._SyncList3.Clear();
			DataList._SyncData.Clear();
			DataList._TitleCreditEarned.Clear();
			DataList._TitleArgievementEarned.Clear();
			DataList._FreeSpaceEntries.Clear();
			_headers = new MainHeaders();
			
			_gameInfo = new GameInfos();
		}
		#endregion
		
		#region Closing
        /// <summary>
        /// Close and clear Parsed Data
        /// </summary>
		public void Close()
		{
			if (_disposed)
			{
				return;
			}
			Clear();
			this.Dispose();
		}
		/// <summary>
		/// Dispose this Class
		/// </summary>
		public void Dispose()
		{
            if (_disposed)
            {
                return;
            }
            GC.SuppressFinalize(this);
		GC.Collect(GC.GetGeneration(this), 0);
			_disposed = true;
		}
		#endregion
		
		#region Properties
		#region ReadOnly
        /// <summary>
        /// Get FreeSpace records
        /// </summary>
		public FreeSpaceInfo FreeSpaceRecord
		{
			get
			{
				return _freespaceRecord;
			}
		}

        /// <summary>
        /// Get Mainheaders
        /// </summary>
		public MainHeaders Headers
		{
			get
			{
				return _headers;
			}
		}

        /// <summary>
        /// Get Gpd Info
        /// </summary>
		public GameInfos GPDInfo
		{
			get
			{
				return _gameInfo;
			}
		}

        /// <summary>
        /// Gets the EntryList
        /// </summary>
		public List<Entries> EntryList
		{
			get
			{
				return DataList._EntryList;
			}
		}

        /// <summary>
        /// Get the Archievements
        /// </summary>
		public List<Achievements> ArchievementsList
		{
			get
			{
				return DataList._argievements;
			}
		}

        /// <summary>
        /// Get first Synclist
        /// </summary>
		public List<UInt64> SyncList1
		{
			get
			{
				return DataList._SyncList1;
			}
		}

		/// <summary>
		/// Get second synclist
		/// </summary>
		public List<UInt64> SyncList2
		{
			get
			{
				return DataList._SyncList2;
			}
		}
		
        /// <summary>
        /// Get Third synclist
        /// </summary>
		public List<UInt64> SyncList3
		{
			get
			{
				return DataList._SyncList3;
			}
		}
		
        /// <summary>
        /// Get the Syncdata
        /// </summary>
		public List<SyncData> XSyncData
		{
			get
			{
				return DataList._SyncData;
			}
		}
		
        /// <summary>
        /// get Title Credit earned
        /// </summary>
		public List<UInt64> TitleCreditEarned
		{
			get
			{
				return DataList._TitleCreditEarned;
			}
		}
		
        /// <summary>
        /// Get Title Archievements Earned
        /// </summary>
		public List<UInt64> TitleArchievementEarned
		{
			get
			{
				return DataList._TitleArgievementEarned;
			}
		}
		
        /// <summary>
        /// Get the Freespace entry list
        /// </summary>
		public List<FreeSpaceEntries> FreeSpaceEntryList
		{
			get
			{
				return DataList._FreeSpaceEntries;
			}
		}
		
        /// <summary>
        /// Gets the First TitleSpecific
        /// </summary>
		public TitleSpecifics1 TitleSpecific1
		{
			get
			{
				return _titleSpecific1;
			}
		}

        /// <summary>
        /// Gets the Second TitleSpecific
        /// </summary>
		public TitleSpecifics2 TitleSpecific2
		{
			get
			{
				return _titleSpecific2;
			}
		}

        /// <summary>
        /// Gets the Third TitleSpecific
        /// </summary>
		public TitleSpecifics3 TitleSpecific3
		{
			get
			{
				return _titleSpecific3;
			}
		}
		#endregion
		#endregion
		
		#region Classes For Holding  parsed data
        /// <summary>
        /// Entry Class for holding the parsed data
        /// </summary>
		public  class Entries
		{
			
			private UInt16 _Namespace;
			private UInt64 _ID;
			private long _Offset;
			private UInt32 _Length;
			private byte[] _data;
			private long _BlockOffset;
			
            /// <summary>
            /// Set new Entry Variables
            /// </summary>
            /// <param name="xNamespace">Entry Namespace</param>
            /// <param name="ID">Entry ID</param>
            /// <param name="Offset">Entry Offset</param>
            /// <param name="Length">Entry data length</param>
            /// <param name="Data">Entry Data</param>
			public Entries(UInt16 xNamespace, UInt64 ID, long Offset, UInt32 Length, byte[] Data)
			{
				_Namespace = xNamespace;
				_ID = ID;
				_Offset = Offset;
				_Length = Length;
				_data = Data;
			}
			
            /// <summary>
            /// get/set Entry Namespace
            /// </summary>
			public UInt16 XNameSpace
			{
				get
				{
					return _Namespace;
				}
				set
				{
					_Namespace = value;
				}
			}

            /// <summary>
            /// get/set Entry ID
            /// </summary>
			public UInt64 ID
			{
				get
				{
					return _ID;
				}
				set
				{
					_ID = value;
				}
			}
            /// <summary>
            /// get/set Entry Offset
            /// </summary>
			public long Offset
			{
				get
				{
					return _Offset;
				}
				set
				{
					_Offset = value;
				}
			}

            /// <summary>
            /// get/set Entry BlockOffset
            /// </summary>
			public long BlockOffset
			{
				get
				{
					return _BlockOffset;
				}
				set
				{
					_BlockOffset = value;
				}
			}

            /// <summary>
            /// get/set Entry data length
            /// </summary>
			public UInt32 Length
			{
				get
				{
					return (uint) _data.Length;
				}
				set
				{
					_Length = value;
				}
			}
            /// <summary>
            /// get/set Entry Data
            /// </summary>
			public byte[] Data
			{
				get
				{
					return _data;
				}
				set
				{
					_data = value;
				}
			}
		}
        /// <summary>
        /// Archievements Class for holding the parsed info
        /// </summary>
		public class Achievements
		{
			
			private UInt32 _Magic;
			private UInt32 _AchievementID;
			private UInt32 _ImageID;
			private UInt32 _Flags;
			private int _Gamerscore;
			private int _entryindex;
			private long _UnlockTime;
			private string _Name;
			private string _LockedDescription;
			private string _UnlockedDescription;
			private int _ImageEntryIndex;
			
            /// <summary>
            /// Set new Archievements
            /// </summary>
            /// <param name="entryindex">Archievement Entry Index</param>
            /// <param name="magic">Archievement Magic</param>
            /// <param name="ID">Archievement ID</param>
            /// <param name="ImageID">Archievement Image ID</param>
            /// <param name="Flags">Archievement Flags</param>
            /// <param name="Gamerscore">Archievement Score</param>
            /// <param name="UnlockTime">Archievement Unlocked time</param>
            /// <param name="Name">Archievement Name</param>
            /// <param name="LockedDescription">Archievement Locked Description</param>
            /// <param name="UnlockedDescription">Archievement Unlocked Description</param>
			public Achievements(int entryindex, UInt32 magic, UInt32 ID, UInt32 ImageID, UInt32 Flags, int Gamerscore, long UnlockTime, string Name, string LockedDescription, string UnlockedDescription)
			{
				_Magic = magic;
				
				_AchievementID = ID;
				_ImageID = ImageID;
				_Flags = Flags;
				
				_Gamerscore = Gamerscore;
				_UnlockTime = UnlockTime;
				_Name = Name;
				
				_LockedDescription = LockedDescription;
				_UnlockedDescription = UnlockedDescription;
				_entryindex = entryindex;
			}
			
            /// <summary>
            /// Get Archievement EntryIndex
            /// </summary>
			public int EntryIndex
			{
				get
				{
					return _entryindex;
				}
			}
			/// <summary>
            /// Get Archievement Magic
			/// </summary>
			public UInt32 Magic
			{
				get
				{
					return _Magic;
				}
			}
			
            /// <summary>
            /// Get Archievement ID
            /// </summary>
			public UInt32 ID
			{
				get
				{
					return _AchievementID;
				}
				set
				{
					_AchievementID = value;
				}
			}
			
            /// <summary>
            /// get/Set Archievement ImageID
            /// </summary>
			public UInt32 ImageID
			{
				get
				{
					return _ImageID;
				}
				set
				{
					_ImageID = value;
				}
			}
			/// <summary>
            /// get/set Archievement Flags
			/// </summary>
			public UInt32 Flags
			{
				get
				{
					return _Flags;
				}
				set
				{
					_Flags = value;
				}
			}
			/// <summary>
            /// Get/Set Archievement Gamescore
			/// </summary>
			public int GameScore
			{
				get
				{
					return _Gamerscore;
				}
				set
				{
					_Gamerscore = value;
				}
			}
			
            /// <summary>
            /// Get/Set Archievement Unlocked Time
            /// </summary>
			public long UnlockTime
			{
				get
				{
					return _UnlockTime;
				}
				set
				{
					_UnlockTime = value;
				}
			}

            /// <summary>
            /// Get Archievement Name
            /// </summary>
			public string Name
			{
				get
				{
					return _Name;
				}
			}

            /// <summary>
            /// Get Archievement Locked Description
            /// </summary>
			public string LockedDescription
			{
				get
				{
					return _LockedDescription;
				}
			}

            /// <summary>
            /// Get Archievement Unlocked Time
            /// </summary>
            public string UnlockedDescription
            {
                get
                {
                    return _UnlockedDescription;
                }
            }
            /// <summary>
            /// Get/Set Archievement Entry Index
            /// </summary>
			public int ImageEntryIndex
			{
				get
				{
					return _ImageEntryIndex;
				}
				set
				{
					_ImageEntryIndex = value;
				}
			}
		}

        /// <summary>
        /// Class for holding The syncdata
        /// </summary>
		public class SyncData
		{
			
			private UInt64 _NextSynxID;
			private UInt64 _LastSyncID;
			private UInt64 _LastSyncTime;
			private int _Entryindex;
			
            /// <summary>
            /// Set new Syncdata variables
            /// </summary>
            /// <param name="Entryindex">Syncdata Entry index</param>
            /// <param name="nextid">Syncdata Nex Sync ID</param>
            /// <param name="LastID">Syncdata First Sync ID</param>
            /// <param name="LastTime">Syncdata Last Sync Time</param>
			public SyncData(int Entryindex, UInt64 nextid, UInt64 LastID, UInt64 LastTime)
			{
				_NextSynxID = nextid;
				_LastSyncID = LastID;
				_LastSyncTime = LastTime;
				_Entryindex = Entryindex;
			}
			/// <summary>
            /// Get Syncdata Entry Index
			/// </summary>
			public int EntryIndex
			{
				get
				{
					return _Entryindex;
				}
			}

            /// <summary>
            /// Get/set Syncdata NextSync ID
            /// </summary>
			public UInt64 NextSyncID
			{
				get
				{
					return _NextSynxID;
				}
				set
				{
					_NextSynxID = value;
				}
			}

            /// <summary>
            /// Get/set Syncdata LastSync ID
            /// </summary>
			public UInt64 LastSyncID
			{
				get
				{
					return _LastSyncID;
				}
				set
				{
					_LastSyncID = value;
				}
			}
            /// <summary>
            /// Get/set Syncdata Last Sync Time
            /// </summary>
			public UInt64 LastSyncTime
			{
				get
				{
					return _LastSyncTime;
				}
				set
				{
					_LastSyncTime = value;
				}
			}
		}

        /// <summary>
        /// Class For holding the first TitleSpecific Info
        /// </summary>
		public class TitleSpecifics1
		{
		    private UInt32 _ID;
			private UInt32 _Value1;
			private UInt64 _Value2;
			private UInt32 _Length;
			private UInt32 _Value3;
			private long _Offset;
			private int _EntryIndex;

            /// <summary>
            /// Set New TitleSpecific Variables
            /// </summary>
            /// <param name="index">TitleSpecific Entry Index</param>
            /// <param name="id">TitleSpecific ID</param>
            /// <param name="value1">TitleSpecific Unknown Value</param>
            /// <param name="value2">TitleSpecific Unknown Value</param>
            /// <param name="length">TitleSpecific Data Length </param>
            /// <param name="value3">TitleSpecific Unknown Value</param>
            /// <param name="Offset">TitleSpecific Offset</param>
			public TitleSpecifics1(int index, UInt32 id, UInt32 value1, UInt64 value2, UInt32 length, UInt32 value3, long Offset)
			{
				_ID = id;
				
				_Value1 = value1;
				_Value2 = value2;
				
				_Length = length;
				_Value3 = value3;
				
				_Offset = Offset;
				_EntryIndex = index;
			}
			
            /// <summary>
            /// Get TitleSpecific EntryIndex
            /// </summary>
			public int EntryIndex
			{
				get
				{
					return _EntryIndex;
				}
			}

            /// <summary>
            /// Get TitleSpecific Offset
            /// </summary>
			public long Offset
			{
				get
				{
					return _Offset;
				}
			}

            /// <summary>
            /// Get TitleSpecific ID
            /// </summary>
			public UInt32 ID
			{
				get
				{
					return _ID;
				}
			}

            /// <summary>
            /// Get TitleSpecific Value1
            /// </summary>
			public UInt32 Value1
			{
				get
				{
					return _Value1;
				}
			}

            /// <summary>
            /// Get TitleSpecific Data Length
            /// </summary>
			public UInt32 Length
			{
				get
				{
					return _Length;
				}
			}
            /// <summary>
            /// Get TitleSpecific Value2
            /// </summary>
			public UInt64 Value2
			{
				get
				{
					return _Value2;
				}
			}
            /// <summary>
            /// Get TitleSpecific Value3
            /// </summary>
			public UInt32 Value3
			{
				get
				{
					return _Value3;
				}
			}
		}

        /// <summary>
        /// Class For holding the Second TitleSpecific Info
        /// </summary>
        public class TitleSpecifics2
        {
            private UInt32 _ID;
            private UInt32 _Value1;
            private UInt64 _Value2;
            private UInt32 _Length;
            private UInt32 _Value3;
            private long _Offset;
            private int _EntryIndex;

            /// <summary>
            /// Set New TitleSpecific Variables
            /// </summary>
            /// <param name="index">TitleSpecific Entry Index</param>
            /// <param name="id">TitleSpecific ID</param>
            /// <param name="value1">TitleSpecific Unknown Value</param>
            /// <param name="value2">TitleSpecific Unknown Value</param>
            /// <param name="length">TitleSpecific Data Length </param>
            /// <param name="value3">TitleSpecific Unknown Value</param>
            /// <param name="Offset">TitleSpecific Offset</param>
            public TitleSpecifics2(int index, UInt32 id, UInt32 value1, UInt64 value2, UInt32 length, UInt32 value3, long Offset)
            {
                _ID = id;

                _Value1 = value1;
                _Value2 = value2;

                _Length = length;
                _Value3 = value3;

                _Offset = Offset;
                _EntryIndex = index;
            }

            /// <summary>
            /// Get TitleSpecific EntryIndex
            /// </summary>
            public int EntryIndex
            {
                get
                {
                    return _EntryIndex;
                }
            }

            /// <summary>
            /// Get TitleSpecific Offset
            /// </summary>
            public long Offset
            {
                get
                {
                    return _Offset;
                }
            }

            /// <summary>
            /// Get TitleSpecific ID
            /// </summary>
            public UInt32 ID
            {
                get
                {
                    return _ID;
                }
            }

            /// <summary>
            /// Get TitleSpecific Value1
            /// </summary>
            public UInt32 Value1
            {
                get
                {
                    return _Value1;
                }
            }

            /// <summary>
            /// Get TitleSpecific Data Length
            /// </summary>
            public UInt32 Length
            {
                get
                {
                    return _Length;
                }
            }
            /// <summary>
            /// Get TitleSpecific Value2
            /// </summary>
            public UInt64 Value2
            {
                get
                {
                    return _Value2;
                }
            }
            /// <summary>
            /// Get TitleSpecific Value3
            /// </summary>
            public UInt32 Value3
            {
                get
                {
                    return _Value3;
                }
            }
        }

        /// <summary>
        /// Class For holding the Third TitleSpecific Info
        /// </summary>
        public class TitleSpecifics3
        {
            private UInt32 _ID;
            private UInt32 _Value1;
            private UInt64 _Value2;
            private UInt32 _Length;
            private UInt32 _Value3;
            private long _Offset;
            private int _EntryIndex;

            /// <summary>
            /// Set New TitleSpecific Variables
            /// </summary>
            /// <param name="index">TitleSpecific Entry Index</param>
            /// <param name="id">TitleSpecific ID</param>
            /// <param name="value1">TitleSpecific Unknown Value</param>
            /// <param name="value2">TitleSpecific Unknown Value</param>
            /// <param name="length">TitleSpecific Data Length </param>
            /// <param name="value3">TitleSpecific Unknown Value</param>
            /// <param name="Offset">TitleSpecific Offset</param>
            public TitleSpecifics3(int index, UInt32 id, UInt32 value1, UInt64 value2, UInt32 length, UInt32 value3, long Offset)
            {
                _ID = id;

                _Value1 = value1;
                _Value2 = value2;

                _Length = length;
                _Value3 = value3;

                _Offset = Offset;
                _EntryIndex = index;
            }

            /// <summary>
            /// Get TitleSpecific EntryIndex
            /// </summary>
            public int EntryIndex
            {
                get
                {
                    return _EntryIndex;
                }
            }

            /// <summary>
            /// Get TitleSpecific Offset
            /// </summary>
            public long Offset
            {
                get
                {
                    return _Offset;
                }
            }

            /// <summary>
            /// Get TitleSpecific ID
            /// </summary>
            public UInt32 ID
            {
                get
                {
                    return _ID;
                }
            }

            /// <summary>
            /// Get TitleSpecific Value1
            /// </summary>
            public UInt32 Value1
            {
                get
                {
                    return _Value1;
                }
            }

            /// <summary>
            /// Get TitleSpecific Data Length
            /// </summary>
            public UInt32 Length
            {
                get
                {
                    return _Length;
                }
            }
            /// <summary>
            /// Get TitleSpecific Value2
            /// </summary>
            public UInt64 Value2
            {
                get
                {
                    return _Value2;
                }
            }
            /// <summary>
            /// Get TitleSpecific Value3
            /// </summary>
            public UInt32 Value3
            {
                get
                {
                    return _Value3;
                }
            }
        }
		/// <summary>
		/// Class For holding The FreespaceEntries
		/// </summary>
		public class FreeSpaceEntries
		{
			
			private long _offset;
		    private int Index;
			private byte[] _data;
            /// <summary>
            /// Set new FreespaceEntry Variables
            /// </summary>
            /// <param name="entryindex"> Freespace Entry Index</param>
            /// <param name="offset">Freespace Offset</param>
            /// <param name="length">Freespace Length</param>
            /// <param name="xdata">Freespace Data</param>
			public FreeSpaceEntries(int entryindex, long offset, UInt32 length, byte[] xdata)
			{
				_offset = offset;

                Index = entryindex;
				_data = xdata;
			}
			/// <summary>
			/// Get Freespace Offset
			/// </summary>
			public long Offset
			{
				get
				{
					return _offset;
				}
			}
			/// <summary>
			/// Get Freespace Data Length
			/// </summary>
			public UInt32 Length
			{
				get
				{
					return (uint) _data.Length;
				}
			}
			/// <summary>
            /// Get Freespace Entry Index
            /// </summary>
			public int Entryindex
			{
				get
				{
					return Index;
				}
			}
			/// <summary>
			/// Get/Set Freespace Data
			/// </summary>
			public byte[] Data
			{
				get
				{
					return _data;
				}
				set
				{
					_data = value;
				}
			}
			
		}
		#endregion
		
	}
	
}
