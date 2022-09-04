/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.IE.V4
{
	public class StructureIE
	{
		public const int Version = 4;

		const long StaticSize = 4 * 1 // MagicNum (4 * byte)
			+ 1 // StructureVersion (byte)
			+ 1 // NameLength (byte)
			+ 4 // ScreenshotSize (uint)
			+ 1 // StrucWidth (byte)
			+ 1 // StrucHeight (byte)
			//+ 1 // LayerCount (byte)
			+ 4 * Def.MaxLayerSlots // LayerSizes (12 * uint)
			+ 2 // BlockCount (ushort)
			;

		bool m_IsFromFile;
		string m_FilePath;

		public readonly char[] MagicNum;
		public readonly byte StructureVersion;
		public ushort StructureID;

		uint ScreenshotSize;
		byte StrucWidth;
		byte StrucHeight;
		readonly uint[] LayerSizes; // SerializedSizes
		uint[] BlockSizes; // SerializedSizes

		byte[] Name;
		readonly LayerIE[] Layers;
		BlockIE[] Blocks;
		byte[] Screenshot;

		Texture2D ScreenshotTexture;

		public bool IsFromFile() => m_IsFromFile;
		public void _FromFile(bool fromFile) => m_IsFromFile = fromFile;
		public string GetFilePath() => m_FilePath;
		public void _FilePath(string fileName) => m_FilePath = fileName;

		public void SetName(string name)
		{
			//NameLength = (byte)name.Length;
			Name = new byte[name.Length];
			for(int i = 0; i < name.Length; ++i)
			{
				//Name[i] = (byte)name[i];
				Name[i] = Convert.ToByte(name[i]);
			}
		}
		public string GetName()
		{
			var charr = new char[Name.Length];
			for (int i = 0; i < Name.Length; ++i)
			{
				//charr[i] = (char)Name[i];
				charr[i] = Convert.ToChar(Name[i]);
			}
			return new string(charr);
		}
		public int GetWidth() => StrucWidth;
		public int GetHeight() => StrucHeight;
		public void SetWidth(int width) => StrucWidth = (byte)width;
		public void SetHeight(int height) => StrucHeight = (byte)height;
		//public int GetLayerNum() => LayerNum;
		public LayerIE GetLayer(int layer) => Layers[layer];
		public LayerIE[] GetLayers() => Layers;
		public void SetLayer(int slot, LayerIE layer)
		{
			if((slot < 1 || slot > Def.MaxLayerSlots)
				|| (layer != null &&
				(slot != layer.Slot || layer.Slot < 1 || layer.Slot > Def.MaxLayerSlots)))
			{
				Debug.Log("Trying to add a invalid layer to a StructureIE.");
				return;
			}
			if (layer != null)
				slot = layer.Slot;
			//if(layer == null && Layers[slot - 1] != null)
			//{
			//	--LayerNum;
			//}
			//else if(layer != null && Layers[slot - 1] == null)
			//{
			//	++LayerNum;
			//}

			Layers[slot - 1] = layer;
		}
		public void RemoveLayer(int slot)
		{
			if(slot < 1 || slot > Def.MaxLayerSlots)
			{
				Debug.LogWarning("Trying to remove a Layer with an invalid slot " + slot.ToString());
				return;
			}
			Layers[slot - 1] = null;
			LayerSizes[slot - 1] = 0;
		}
		public int AddBlock(BlockIE block)
		{
			if(block.Layer < 1 || block.Layer > Def.MaxLayerSlots)
			{
				Debug.LogWarning("Trying to add a invalid block to a StructureIE.");
				return -1;
			}
			for(int i = 0; i < Blocks.Length; ++i)
			{
				if(Blocks[i] == null)
				{
					Blocks[i] = block;
					return i;
				}
			}
			//++BlockNum;
			var nBlocks = new BlockIE[Blocks.Length + 1];
			Blocks.CopyTo(nBlocks, 0);
			Blocks = nBlocks;
			Blocks[Blocks.Length - 1] = block;
			return Blocks.Length - 1;
		}
		public BlockIE[] GetBlocks() => Blocks;
		public void RemoveBlock(int idxie)
		{
			if(idxie < 0 || idxie >= Blocks.Length)
			{
				Debug.LogWarning("Trying to remove an outside bounds BlockIE.");
				return;
			}
			Blocks[idxie] = null;
		}
		public void RemoveBlock(BlockIE block)
		{
			for(int i = 0; i < Blocks.Length; ++i)
			{
				if(Blocks[i] == block)
				{
					Blocks[i] = null;
					break;
				}
			}
		}
		public void SetScreenshot(Texture2D screenshot)
		{
			ScreenshotTexture = screenshot;
			Screenshot = ScreenshotTexture.EncodeToPNG();
			ScreenshotSize = (uint)Screenshot.Length;
		}
		public Texture2D GetScreenshot() => ScreenshotTexture;
		public StructureIE()
		{
			m_IsFromFile = false;
			MagicNum = new char[] { 'S', 'T', 'R', 'C' };
			StructureVersion = Version;
			LayerSizes = new uint[Def.MaxLayerSlots];
			Layers = new LayerIE[Def.MaxLayerSlots];

			SetDefault();
		}
		public StructureIE(StructureIE copy)
		{
			m_IsFromFile = false;
			MagicNum = new char[] { 'S', 'T', 'R', 'C' };
			StructureVersion = Version;
			LayerSizes = new uint[Def.MaxLayerSlots];
			Layers = new LayerIE[Def.MaxLayerSlots];
			StructureID = copy.StructureID;

			//NameLength = copy.NameLength;
			//LayerNum = copy.LayerNum;
			//BlockNum = copy.BlockNum;
			BlockSizes = new uint[copy.BlockSizes.Length];
			ScreenshotSize = copy.ScreenshotSize;
			StrucWidth = copy.StrucWidth;
			StrucHeight = copy.StrucHeight;

			Name = new byte[copy.Name.Length];
			Blocks = new BlockIE[copy.Blocks.Length];
			Screenshot = new byte[copy.Screenshot.Length];

			for (int i = 0; i < Layers.Length; ++i)
			{
				if (copy.Layers[i] == null)
				{
					Layers[i] = null;
					LayerSizes[i] = 0;
					continue;
				}
				Layers[i] = new LayerIE(copy.Layers[i]);
				LayerSizes[i] = copy.LayerSizes[i];
			}
			for(int i = 0; i < Blocks.Length; ++i)
			{
				if (copy.Blocks[i] == null)
				{
					Blocks[i] = null;
					BlockSizes[i] = 0;
					continue;
				}
				Blocks[i] = new BlockIE(copy.Blocks[i]);
				BlockSizes[i] = copy.BlockSizes[i];
			}
			copy.Name.CopyTo(Name, 0);
			copy.Screenshot.CopyTo(Screenshot, 0);
		}
		public void SetDefault()
		{
			StructureID = 0;

			//NameLength = 0;
			//LayerNum = 0;
			//BlockNum = 0;
			BlockSizes = new uint[0];
			ScreenshotSize = 0;
			StrucWidth = 8;
			StrucHeight = 8;

			for (int i = 0; i < Layers.Length; ++i) { Layers[i] = null; LayerSizes[i] = 0; }

			Name = new byte[0];
			Blocks = new BlockIE[0];
			Screenshot = new byte[0];
		}
		public void SaveStruc(string path)
		{
			var fw = File.OpenWrite(path);

			m_IsFromFile = true;
			m_FilePath = path;

			long dynamicSize = 0;

			ScreenshotSize = (uint)Screenshot.Length;

			//var nBlocks = new List<BlockIE>(BlockNum);
			//for (int i = 0; i < BlockNum; ++i)
			//{
			//	if (Blocks[i] == null)
			//		continue;
			//	nBlocks.Add(Blocks[i]);
			//}

			var tempArray = new byte[StaticSize];
			int lastIdx = 0;

			for (int i = 0; i < MagicNum.Length; ++i)
				tempArray[lastIdx++] = (byte)MagicNum[i];

			tempArray[lastIdx++] = StructureVersion;

			var nameLength = Name.Length > 255 ? 255 : Name.Length;
			tempArray[lastIdx++] = Convert.ToByte(nameLength);
			dynamicSize += nameLength;

			var tempBytes = BitConverter.GetBytes(ScreenshotSize);
			for (int i = 0; i < tempBytes.Length; ++i)
				tempArray[lastIdx++] = tempBytes[i];
			dynamicSize += ScreenshotSize;

			tempArray[lastIdx++] = StrucWidth;
			tempArray[lastIdx++] = StrucHeight;

			//tempArray[lastIdx++] = LayerNum;

			var formatter = new BinaryFormatter();
			var layersBin = new byte[Def.MaxLayerSlots][];
			var memStream = new MemoryStream();
			for (int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				if (Layers[i] == null)
				{
					LayerSizes[i] = 0;
					tempBytes = BitConverter.GetBytes(LayerSizes[i]);
					for (int j = 0; j < tempBytes.Length; ++j)
						tempArray[lastIdx++] = tempBytes[j];
					continue;
				}
				memStream.Position = 0;
				formatter.Serialize(memStream, Layers[i]);
				layersBin[i] = memStream.ToArray();
				LayerSizes[i] = (uint)layersBin[i].Length;
				tempBytes = BitConverter.GetBytes(LayerSizes[i]);
				for (int j = 0; j < tempBytes.Length; ++j)
					tempArray[lastIdx++] = tempBytes[j];
				dynamicSize += LayerSizes[i];
			}

			tempBytes = BitConverter.GetBytes((ushort)Blocks.Length);
			for (int j = 0; j < tempBytes.Length; ++j)
				tempArray[lastIdx++] = tempBytes[j];

			if (lastIdx != tempArray.Length)
				throw new Exception("Not everything has been written, StructureIE 'STATICSIZE'");
			fw.Write(tempArray, 0, tempArray.Length);

			tempArray = new byte[Blocks.Length * 4];
			lastIdx = 0;

			BlockSizes = new uint[Blocks.Length];
			var blocksBin = new byte[Blocks.Length][];
			for (int i = 0; i < Blocks.Length; ++i)
			{
				if (Blocks[i] == null)
				{
					blocksBin[i] = new byte[0];
					BlockSizes[i] = 0;
				}
				else
				{
					memStream.Position = 0;
					formatter.Serialize(memStream, Blocks[i]);
					blocksBin[i] = memStream.ToArray();
					BlockSizes[i] = (uint)blocksBin[i].Length;
					dynamicSize += BlockSizes[i];
				}
				tempBytes = BitConverter.GetBytes(BlockSizes[i]);
				for (int j = 0; j < tempBytes.Length; ++j)
					tempArray[lastIdx++] = tempBytes[j];
			}

			if (lastIdx != tempArray.Length)
				throw new Exception("Not everything has been written, StructureIE 'BLOCKSIZES'");
			fw.Write(tempArray, 0, tempArray.Length);

			// Dynamic Info
			fw.Write(Name, 0, nameLength);

			// Layer Info
			for (int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				if (layersBin[i] == null)
					continue;
				fw.Write(layersBin[i], 0, layersBin[i].Length);
			}

			// Block Info
			for(int i = 0; i < Blocks.Length; ++i)
			{
				if (blocksBin[i].Length == 0)
					continue;
				fw.Write(blocksBin[i], 0, blocksBin[i].Length);
			}

			// Screenshot
			fw.Write(Screenshot, 0, Screenshot.Length);
			
			fw.Close();
		}
		public static StructureIE FromFile(string path)
		{
			var sie = new StructureIE()
			{
				m_IsFromFile = true,
				m_FilePath = path
			};

			var fr = File.OpenRead(path);
			if(!fr.CanRead || fr.Length < StaticSize)
			{
				fr.Close();
				Debug.LogWarning("Invalid structure file, the amount of bytes was invalid or couldn't read from it.");
				return null;
			}

			var tempArray = new byte[StaticSize];
			int lastIdx = 0;
			fr.Read(tempArray, 0, tempArray.Length);
			for(int i = 0; i < 4; ++i)
			{
				if((char)tempArray[lastIdx++] != sie.MagicNum[i])
				{
					Debug.LogWarning("Trying to read the magic num from a structure but was invalid.");
					fr.Close();
					return null;
				}
			}

			var version = tempArray[lastIdx++];
			if(version > Version)
			{
				Debug.LogWarning("Trying to load a structure which version is not supported.");
				fr.Close();
				return null;
			}
			switch(version)
			{
				case 1:
					fr.Close();
					return V1.StructureIE.LoadAndUpgrade(path).Upgrade().Upgrade();
				case 2:
					fr.Close();
					return V2.StructureIE.LoadAndUpgrade(path).Upgrade();
				case 3:
					fr.Close();
					return V3.StructureIE.LoadAndUpgrade(path);
			}

			var nameLength = tempArray[lastIdx++];
			uint dynamicSize = nameLength;
			sie.Name = new byte[nameLength];

			sie.ScreenshotSize = BitConverter.ToUInt32(tempArray, lastIdx);
			sie.Screenshot = new byte[sie.ScreenshotSize];
			dynamicSize += sie.ScreenshotSize;
			lastIdx += 4;

			sie.StrucWidth = tempArray[lastIdx++];
			sie.StrucHeight = tempArray[lastIdx++];

			//sie.LayerNum = tempArray[lastIdx++]; not necessary

			for (int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				sie.LayerSizes[i] = BitConverter.ToUInt32(tempArray, lastIdx);
				dynamicSize += sie.LayerSizes[i];
				lastIdx += 4;
			}
			var blockCount = BitConverter.ToUInt16(tempArray, lastIdx);
			sie.BlockSizes = new uint[blockCount];
			sie.Blocks = new BlockIE[blockCount];
			lastIdx += 2;

			if(lastIdx != tempArray.Length)
			{
				throw new Exception("Not everything has been read, StructureIE 'STATICSIZE'");
			}

			long BlockSizes = StaticSize + blockCount * 4;

			if(fr.Length <= BlockSizes)
			{
				fr.Close();
				Debug.LogWarning("Trying to begin 'BLOCKSIZES' read from a structure, but the file is not long enough");
				return null;
			}

			tempArray = new byte[blockCount * 4];
			fr.Read(tempArray, 0, tempArray.Length);
			lastIdx = 0;
			for(int i = 0; i < sie.BlockSizes.Length; ++i)
			{
				sie.BlockSizes[i] = BitConverter.ToUInt32(tempArray, lastIdx);
				dynamicSize += sie.BlockSizes[i];
				lastIdx += 4;
			}

			if (lastIdx != tempArray.Length)
			{
				throw new Exception("Not everything has been read, StructureIE 'BLOCKSIZES'");
			}

			long DynamicSize = BlockSizes + dynamicSize;

			if(fr.Length < DynamicSize)
			{
				fr.Close();
				Debug.LogWarning("Trying to begin 'DYNAMICSIZE' read from a structure, but the file is not long enough");
				return null;
			}

			tempArray = new byte[dynamicSize];
			fr.Read(tempArray, 0, tempArray.Length);
			lastIdx = 0;

			for (int i = 0; i < sie.Name.Length; ++i)
				sie.Name[i] = tempArray[lastIdx++];

			var formatter = new BinaryFormatter();
			for(int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				if (sie.LayerSizes[i] == 0)
					continue;
				var stream = new MemoryStream(tempArray, lastIdx, (int)sie.LayerSizes[i]);
				sie.Layers[i] = (LayerIE)formatter.Deserialize(stream);
				lastIdx += (int)sie.LayerSizes[i];
			}
			for (int i = 0; i < blockCount; ++i)
			{
				if (sie.BlockSizes[i] == 0)
					continue;
				var stream = new MemoryStream(tempArray, lastIdx, (int)sie.BlockSizes[i]);
				sie.Blocks[i] = (BlockIE)formatter.Deserialize(stream);
				lastIdx += (int)sie.BlockSizes[i];
			}
			for (int i = 0; i < sie.Screenshot.Length; ++i)
				sie.Screenshot[i] = tempArray[lastIdx++];

			sie.ScreenshotTexture = new Texture2D(0, 0);
			sie.ScreenshotTexture.LoadImage(sie.Screenshot, false);

			if (lastIdx != tempArray.Length)
			{
				throw new Exception("Not everything has been read, StructureIE 'DYNAMICSIZE'");
			}
			fr.Close();

			return sie;
		}
		public override bool Equals(object obj)
		{
			return obj is StructureIE iE &&
				this == iE;
		}
		public override int GetHashCode()
		{
			int hashCode = 695569280;
			hashCode = hashCode * -1521134295 + EqualityComparer<char[]>.Default.GetHashCode(MagicNum);
			hashCode = hashCode * -1521134295 + StructureVersion.GetHashCode();
			hashCode = hashCode * -1521134295 + StructureID.GetHashCode();
			hashCode = hashCode * -1521134295 + StrucWidth.GetHashCode();
			hashCode = hashCode * -1521134295 + StrucHeight.GetHashCode();
			hashCode = hashCode * -1521134295 + ScreenshotSize.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<uint[]>.Default.GetHashCode(LayerSizes);
			hashCode = hashCode * -1521134295 + EqualityComparer<uint[]>.Default.GetHashCode(BlockSizes);
			hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + EqualityComparer<LayerIE[]>.Default.GetHashCode(Layers);
			hashCode = hashCode * -1521134295 + EqualityComparer<BlockIE[]>.Default.GetHashCode(Blocks);
			hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(Screenshot);
			hashCode = hashCode * -1521134295 + EqualityComparer<Texture2D>.Default.GetHashCode(ScreenshotTexture);
			return hashCode;
		}
		public static bool operator==(StructureIE left, StructureIE right)
		{
			if (left is null != right is null)
				return false;
			if (left is null && right is null)
				return true;

			if (left.m_IsFromFile != right.m_IsFromFile)
				return false;

			if (left.StructureID != right.StructureID)
				return false;

			if (left.StrucWidth != right.StrucWidth)
				return false;

			if (left.StrucHeight != right.StrucHeight)
				return false;

			if (left.Name.Length != right.Name.Length)
				return false;

			//if (left.LayerNum != right.LayerNum)
			//	return false;

			for(int i = 0; i < left.Name.Length; ++i)
			{
				if (left.Name[i] != right.Name[i])
					return false;
			}

			for(int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				if (left.LayerSizes[i] != right.LayerSizes[i])
					return false;

				if (left.Layers[i] != right.Layers[i])
					return false;
			}

			if (left.Blocks.Length != right.Blocks.Length)
				return false;

			for(int i = 0; i < left.Blocks.Length; ++i)
			{
				if (left.BlockSizes[i] != right.BlockSizes[i])
					return false;

				if (left.Blocks[i] != right.Blocks[i])
					return false;
			}

			if (left.ScreenshotSize != right.ScreenshotSize)
				return false;

			return true;
		}
		public static bool operator !=(StructureIE left, StructureIE right)
		{
			return !(left == right);
		}
	}
}
