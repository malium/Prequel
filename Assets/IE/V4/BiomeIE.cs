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
	[System.Serializable]
	public class BiomeIE
	{
		public const int Version = 4;
		const int StaticSize = 5 // MagicNum (byte)
				+ 1 // IEVersion (byte)
				+ 2 // BiomeID (ushort)
				+ 1 // LayerCount (byte)
				+ 1 * Def.BiomeStatCount * 2 // BiomeStat Min Max (sbyte)
				+ 2 // BiomeMinDistance (ushort)
				+ 2 // NameLength (ushort)
				+ Def.BiomeTypeCount * 4; // LayerSizes (uint)

		bool m_IsFromFile;
		string m_FilePath;

		public readonly char[] MagicNum;
		public readonly byte IEVersion;
		public ushort BiomeID;

		sbyte[] m_StatValue;
		ushort m_MinDistance;

		byte LayerCount;
		ushort NameLength;
		readonly uint[] LayerSizes;

		byte[] Name;
		readonly BiomeLayerIE[] Layers;

		public BiomeIE()
		{
			m_IsFromFile = false;
			MagicNum = new char[] { 'B', 'I', 'O', 'M', 'E' };
			IEVersion = Version;
			LayerSizes = new uint[Def.BiomeTypeCount];
			Layers = new BiomeLayerIE[Def.BiomeTypeCount];
			m_StatValue = new sbyte[Def.BiomeStatCount * 2];
			m_MinDistance = 1;
			SetDefault();
		}
		public void SetDefault()
		{
			NameLength = 0;
			Name = new byte[0];

			LayerCount = 0;
			for (int i = 0; i < Layers.Length; ++i)  { Layers[i] = null; LayerSizes[i] = 0; }
			for (int i = 0; i < m_StatValue.Length; ++i) m_StatValue[i] = 0;
		}
		public void SetName(string name)
		{
			NameLength = (ushort)name.Length;
			Name = new byte[NameLength];
			for (int i = 0; i < name.Length; ++i)
			{
				Name[i] = (byte)name[i];
			}
		}
		public string GetName()
		{
			var charr = new char[NameLength];
			for (int i = 0; i < NameLength; ++i)
				charr[i] = (char)Name[i];
			return new string(charr);
		}
		public int GetMinDistance() => m_MinDistance;
		public void SetMinDistance(int distance) => m_MinDistance = (ushort)distance;
		public void GetBiomeStat(Def.BiomeStat stat, out int min, out int max)
		{
			int index = ((int)stat) * 2;
			min = m_StatValue[index];
			max = m_StatValue[index + 1];
		}
		public void SetBiomeStat(Def.BiomeStat stat, int min, int max)
		{
			int index = ((int)stat) * 2;
			m_StatValue[index] = (sbyte)min;
			m_StatValue[index + 1] = (sbyte)max; 
		}
		public sbyte[] _GetStatValue() => m_StatValue;
		public BiomeLayerIE GetLayer(Def.BiomeLayerType layer) => Layers[(int)layer];
		public BiomeLayerIE[] GetLayers() => Layers;
		public void SetLayer(Def.BiomeLayerType layerType, BiomeLayerIE layer)
		{
			if(layerType == Def.BiomeLayerType.FULLVOID
				|| layerType == Def.BiomeLayerType.OTHER
				|| layerType == Def.BiomeLayerType.COUNT)
			{
				Debug.LogWarning("Trying to set an invalid BiomeLayerIE");
				return;
			}

			if (layer == null && Layers[(int)layerType] != null)
				--LayerCount;
			else if (layer != null && Layers[(int)layerType] == null)
				++LayerCount;

			if (layer != null && layer.LayerType != layerType)
			{
				Debug.LogWarning("Trying to set a BiomeLayerIE with a different BiomeLayerType, "
					+ "Layer: " + layer.LayerType.ToString() + " Requested: " + layerType.ToString());
			}

			Layers[(int)layerType] = layer;

			if (layer == null)
				LayerSizes[(int)layerType] = 0;
		}
		public static BiomeIE FromFile(string path)
		{
			var bie = new BiomeIE()
			{
				m_IsFromFile = true,
				m_FilePath = path
			};

			var fr = File.OpenRead(path);
			if (!fr.CanRead || fr.Length < StaticSize)
			{
				fr.Close();
				Debug.LogWarning("Invalid biome file, the amount of bytes was invalid or couldn't read from it.");
				return null;
			}

			var tempArray = new byte[StaticSize];
			int lastIdx = 0;
			fr.Read(tempArray, 0, tempArray.Length);

			for (int i = 0; i < bie.MagicNum.Length; ++i)
			{
				if ((char)tempArray[lastIdx++] != bie.MagicNum[i])
				{
					Debug.LogWarning("Trying to read the magic num from a biome but was invalid.");
					fr.Close();
					return null;
				}
			}

			var version = tempArray[lastIdx++];
			if (version > Version)
			{
				Debug.LogWarning("Trying to load a biome which version is not supported.");
				fr.Close();
				return null;
			}

			bie.BiomeID = BitConverter.ToUInt16(tempArray, lastIdx);
			lastIdx += 2;

			bie.LayerCount = tempArray[lastIdx++];

			for (int i = 0; i < Def.BiomeStatCount * 2; ++i)
				bie.m_StatValue[i] = (sbyte)(tempArray[lastIdx++] - 128);

			bie.m_MinDistance = BitConverter.ToUInt16(tempArray, lastIdx);
			lastIdx += 2;

			bie.NameLength = BitConverter.ToUInt16(tempArray, lastIdx);
			lastIdx += 2;
			bie.Name = new byte[bie.NameLength];
			uint dynamicSize = bie.NameLength;

			for (int i = 0; i < Def.BiomeTypeCount; ++i)
			{
				bie.LayerSizes[i] = BitConverter.ToUInt32(tempArray, lastIdx);
				dynamicSize += bie.LayerSizes[i];
				lastIdx += 4;
			}

			if (lastIdx != tempArray.Length)
			{
				throw new Exception("Not everything has been read, BiomeIE 'STATICSIZE'");
			}

			if (fr.Length < (StaticSize + dynamicSize))
			{
				fr.Close();
				Debug.LogWarning("Trying to begin 'DYNAMICSIZE' read from a biome, but the file is not long enough");
				return null;
			}

			tempArray = new byte[dynamicSize];
			fr.Read(tempArray, 0, tempArray.Length);
			lastIdx = 0;

			for (int i = 0; i < bie.Name.Length; ++i)
				bie.Name[i] = tempArray[lastIdx++];

			var formatter = new BinaryFormatter();
			for(int i = 0; i < Def.BiomeTypeCount; ++i)
			{
				if (bie.LayerSizes[i] == 0)
					continue;

				var stream = new MemoryStream(tempArray, lastIdx, (int)bie.LayerSizes[i]);
				bie.Layers[i] = (BiomeLayerIE)formatter.Deserialize(stream);
				lastIdx += (int)bie.LayerSizes[i];
			}

			if (lastIdx != tempArray.Length)
			{
				throw new Exception("Not everything has been read, BiomeIE 'DYNAMICSIZE'");
			}
			fr.Close();

			return bie;
		}
		public void SaveBiome(string path)
		{
			var fi = new FileInfo(path);
			if (fi.Exists)
				fi.Delete();
			var fw = fi.Create();

			m_IsFromFile = true;
			m_FilePath = path;

			var tempArray = new byte[StaticSize];
			int lastIdx = 0;

			for (int i = 0; i < MagicNum.Length; ++i)
				tempArray[lastIdx++] = (byte)MagicNum[i];

			tempArray[lastIdx++] = IEVersion;

			var tempBytes = BitConverter.GetBytes(BiomeID);
			for (int i = 0; i < tempBytes.Length; ++i)
				tempArray[lastIdx++] = tempBytes[i];

			tempArray[lastIdx++] = LayerCount;

			for (int i = 0; i < Def.BiomeStatCount * 2; ++i)
				tempArray[lastIdx++] = (byte)(m_StatValue[i] + 128);

			tempBytes = BitConverter.GetBytes(m_MinDistance);
			for (int i = 0; i < tempBytes.Length; ++i)
				tempArray[lastIdx++] = tempBytes[i];

			tempBytes = BitConverter.GetBytes(NameLength);
			for (int i = 0; i < tempBytes.Length; ++i)
				tempArray[lastIdx++] = tempBytes[i];
			uint dynamicSize = NameLength;

			var formatter = new BinaryFormatter();
			var layersBin = new byte[Def.BiomeTypeCount][];
			var memStream = new MemoryStream();
			for (int i = 0; i < Def.BiomeTypeCount; ++i)
			{
				if (Layers[i] == null)
				{
					LayerSizes[i] = 0;
				}
				else
				{
					memStream.Position = 0;
					formatter.Serialize(memStream, Layers[i]);
					layersBin[i] = memStream.ToArray();
					LayerSizes[i] = (uint)layersBin[i].Length;
				}

				dynamicSize += LayerSizes[i];
				tempBytes = BitConverter.GetBytes(LayerSizes[i]);
				for (int j = 0; j < tempBytes.Length; ++j)
					tempArray[lastIdx++] = tempBytes[j];
			}

			if (lastIdx != tempArray.Length)
				throw new Exception("Not everything has been written, BiomeIE 'STATICSIZE'");

			fw.Write(tempArray, 0, tempArray.Length);

			fw.Write(Name, 0, Name.Length);

			for (int i = 0; i < Def.BiomeTypeCount; ++i)
			{
				if (layersBin[i] == null || LayerSizes[i] == 0)
					continue;
				fw.Write(layersBin[i], 0, layersBin[i].Length);
			}
			fw.Close();
		}
		public World.Biome ToBiome()
		{
			var biome = new World.Biome
			{
				IDXIE = BiomeID,
			};

			biome.SetMinDistance(GetMinDistance());
			
			for(int i = 0; i < Layers.Length; ++i)
			{
				if (Layers[i] == null)
					continue;
				biome.GetLayers()[i] = Layers[i].ToBiomeLayer();
			}
			for (int i = 0; i < m_StatValue.Length; ++i)
				biome._GetStatValue()[i] = (int)m_StatValue[i];
			return biome;
		}
		public bool IsFromFile() => m_IsFromFile;
		public void _FromFile(bool fromFile) => m_IsFromFile = fromFile;
		public string GetFilePath() => m_FilePath;
		public void _FilePath(string fileName) => m_FilePath = fileName;
		public override bool Equals(object obj)
		{
			return obj is BiomeIE iE &&
				   this == iE;
		}
		public override int GetHashCode()
		{
			int hashCode = -1889381495;
			hashCode = hashCode * -1521134295 + EqualityComparer<char[]>.Default.GetHashCode(MagicNum);
			hashCode = hashCode * -1521134295 + IEVersion.GetHashCode();
			hashCode = hashCode * -1521134295 + LayerCount.GetHashCode();
			hashCode = hashCode * -1521134295 + NameLength.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<uint[]>.Default.GetHashCode(LayerSizes);
			hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + EqualityComparer<BiomeLayerIE[]>.Default.GetHashCode(Layers);
			return hashCode;
		}
		public static bool operator ==(BiomeIE left, BiomeIE right)
		{
			if (left is null != right is null)
				return false;
			if (left is null && right is null)
				return true;

			if (left.LayerCount != right.LayerCount)
				return false;

			if (left.NameLength != right.NameLength)
				return false;
			for (int i = 0; i < left.NameLength; ++i)
			{
				if (left.Name[i] != right.Name[i])
					return false;
			}

			for (int i = 0; i < Def.BiomeTypeCount; ++i)
			{
				if (left.Layers[i] != right.Layers[i])
					return false;
			}

			return true;
		}
		public static bool operator !=(BiomeIE left, BiomeIE right)
		{
			return !(left == right);
		}
	}
}
