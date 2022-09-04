/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.World
{
	struct PlacingInfo
	{
		public Vector2Int Position;
		public int StrucIDXIE;
		public int BiomeIDXIE;
	}
	struct PlacementInfo
	{
		public Vector2Int Size;
		public int BiomeIDXIE;
	}
	struct WideSlice
	{
		public bool Possible;
		public BlockInfo[] Blocks;
	}
	class StructureWorkData
	{
		public List<PlacingInfo> Placings;
		public int Start;
		public int End;
		public System.Random RNG;
	}
	public class World : MonoBehaviour
	{
		public const int MaxWorldSize = 32000;
		public const int DefaultSize = 4000;
		public const int OnCamStrucs = 25;
		const bool ShowBiomeCubes = false;
		const int CPUCores = 4;

		public static readonly string[] DefaultStructureNames = new string[] { "SmallIsland_000", "SmallIsland_001", "SmallIsland_002", "SmallIsland_003", "SmallIsland_004",
			"SmallIsland_005", "SmallIsland_006", "SmallIsland_007", "SmallIsland_008", "SmallIsland_009", "SmallIsland_010", "SmallIsland_011",
			"SmallIsland_012", "SmallIsland_013", "SmallIsland_014", "SmallIsland_015" };
		//public string[] StructureNames = new string[] { "WideTest_000" };
		[SerializeField] Vector2Int m_Size;
		[SerializeField] Vector2Int m_StrucWorldSize;
		[SerializeField] Vector2 m_Offset;
		[SerializeField] int m_Seed;
		[SerializeField] bool m_WideDebug;
		string[] m_StructureNames;
		PilarInfo[] m_Pilars;
		List<StrucInfo> m_StrucInfos;
		CStruc[] m_Strucs;

		public static World gWorld;

		// WideDebug
		int m_DBGStrucIDX;
		int m_DBGPilarIDX;
		int m_DBGBlockIDX;
		System.Random m_DBGRng;

		//WStruc[] m_Strucs;
		public int NextPositionOffsetMin = 4;
		public int NextPositionOffsetMax = 50;
		public readonly object PilarsLock = new object();
		GameObject[] m_StrucCubes;
		[SerializeField] Perlin[] m_Perlins;
		int[][] m_BiomeStat;
		int[] m_BiomeMap;
		static readonly float[] PerlinDef = new float[4 * Def.WorldPerlinCount]
		{
			3f,    10f, 0f,  20f,		// Density
			0.75f, 4f,  0f,  0f,		// Temperature
			0.75f, 4f,  0f,  20f,		// Height
			5f,    7f,  20f, 20f,		// Soulness
			3f,    4f,  7f,  7f,		// Wealth
			20f,   35f, 0f,  0f			// Bump
		};
		const float InvHorzSize = 1f / CStruc.Width;
		const float InvVertSize = 1f / CStruc.Height;

		public World()
		{
			//m_Strucs = new WStruc[OnCamStrucs];
			m_StrucInfos = new List<StrucInfo>();
			m_Perlins = new Perlin[Def.WorldPerlinCount];
			for (int i = 0; i < m_Perlins.Length; ++i)
				m_Perlins[i] = new Perlin();
		}
		private void Awake()
		{
			//for (int i = 0; i < OnCamStrucs; ++i)
			//{
			//	var struc = new GameObject("WorldStruc_" + i.ToString()).AddComponent<WStruc>();
			//	struc.transform.SetParent(transform);
			//	struc.Init(this);
			//	m_Strucs[i] = struc;
			//}
		}
		struct BiomePlaceInfo
		{
			public int IDXIE;
			public int Chance;
		}
		void PlaceStruc(PlacingInfo info, int strucID, /*List<int> biomeList, List<BiomePlaceInfo> placingList,*/ System.Random rng)
		{
			var struc = new StrucInfo(this);

			// biome selection
			//int maxChance = int.MinValue;
			//for(int i = 0; i < biomeList.Count; ++i)
			//{
			//	var idxIE = biomeList[i];
			//	var biomeIE = BiomeLoader.Biomes[idxIE];

			//	int chance = 0;
			//	biomeIE.GetBiomeStat(Def.BiomeStat.Density, out int min, out int max);
			//	if (info.Density >= min && info.Density <= max)
			//		++chance;

			//	biomeIE.GetBiomeStat(Def.BiomeStat.Temperature, out min, out max);
			//	if (info.Temperature >= min && info.Temperature <= max)
			//		++chance;

			//	biomeIE.GetBiomeStat(Def.BiomeStat.Height, out min, out max);
			//	if (info.Height >= min && info.Height <= max)
			//		++chance;

			//	biomeIE.GetBiomeStat(Def.BiomeStat.Soulness, out min, out max);
			//	if (info.Soulness >= min && info.Soulness <= max)
			//		++chance;

			//	biomeIE.GetBiomeStat(Def.BiomeStat.Wealth, out min, out max);
			//	if (info.Wealth >= min && info.Wealth <= max)
			//		++chance;

			//	if(chance > maxChance)
			//	{
			//		placingList.Clear();
			//		placingList.Add(new BiomePlaceInfo() { IDXIE = idxIE, Chance = chance });
			//		maxChance = chance;
			//	}
			//	else if(chance == maxChance)
			//	{
			//		placingList.Add(new BiomePlaceInfo() { IDXIE = idxIE, Chance = chance });
			//	}
			//}
			//Biome biome = null;
			//if(placingList.Count > 0)
			//{
			//	var biomeInfo = placingList[rng.Next(0, placingList.Count)];
			//	var biomeIE = BiomeLoader.Biomes[biomeInfo.IDXIE];
			//	biome = biomeIE.ToBiome();
			//}

			var biome = BiomeLoader.Biomes[info.BiomeIDXIE].ToBiome();
			struc.SetBiome(biome);
			var worldID = GameUtils.IDFromPosUnsafe(info.Position, m_Size.x, m_Size.y);
			var heightPerlin = m_Perlins[(int)Def.WorldPerlins.Height].Samples;
			var bumpPerlin = m_Perlins[(int)Def.WorldPerlins.Bump].Samples;

			var height = Mathf.FloorToInt((heightPerlin[worldID] * Def.WorldHeightRange * 2f - Def.WorldHeightRange)) * 0.5f;
			var bump = Mathf.FloorToInt(bumpPerlin[worldID] * Def.WorldBumpRange * 2f) * 0.5f;

			float sHeight = height + bump;
			struc.SetHeight(sHeight);

			//int PerlinOffset(float perlin) => Mathf.FloorToInt(NextPositionOffsetMax * (1f - perlin) + NextPositionOffsetMin * perlin);
			struc.LoadStruc(info.StrucIDXIE, info.Position, rng);
			//m_CurrentPerlinValue = m_PerlinSamples[GameUtils.IDFromPos(position, m_Size.x, m_Size.y)];

			//var xOffset = PerlinOffset(m_CurrentPerlinValue);
			m_StrucInfos[strucID] = struc;
		}
		/* Virtual Structure Position from Virtual Position */
		public static Vector2Int VSPosFromVPos(Vector2Int vPos)
		{
			return new Vector2Int(
				Mathf.FloorToInt(vPos.x * 0.0625f),
				Mathf.FloorToInt(vPos.y * 0.0625f));
		}
		public static Vector2 SPosFromVPos(Vector2Int vPos)
		{
			return new Vector2(
				vPos.x * 0.0625f,
				vPos.y * 0.0625f);
		}
		/* Virtual Position from Virtual Structure Position */
		public static Vector2Int VPosFromVSPos(Vector2Int vsPos)
		{
			return new Vector2Int(
				vsPos.x * 16,
				vsPos.y * 16);
		}
		/* Structure ID from Virtual Structure Position */
		public int StrucIDFromVSPos(Vector2Int vsPos)
		{
			vsPos.Set(GameUtils.Mod(vsPos.x, m_StrucWorldSize.x), GameUtils.Mod(vsPos.y, m_StrucWorldSize.y));
			return GameUtils.IDFromPosUnsafe(vsPos, m_StrucWorldSize.x, m_StrucWorldSize.y);
		}
		/* Virtual Structure Position from Structure ID */
		public Vector2Int VSPosFromStrucID(int strucID)
		{
			return GameUtils.PosFromIDUnsafe(strucID, m_StrucWorldSize.x, m_StrucWorldSize.y);
		}
		/* Virtual Position from Virtual World Position (taking infinite world in account) */
		public Vector2Int VPosFromVWPos(Vector2Int vwPos)
		{
			return new Vector2Int(GameUtils.Mod(vwPos.x, m_Size.x), GameUtils.Mod(vwPos.y, m_Size.y));
		}
		public int PilarIDFromVPos(Vector2Int vPos)
		{
			if(!GameUtils.IDFromPosSafe(vPos, m_Size.x, m_Size.y, out int id))
			{
				Debug.LogWarning($"Trying to obtain an invalid position ({vPos}), Map:({m_Size}), maybe you are using Virtual World Coordinates.");
				return 0;
			}
			return id;
		}
		BlockInfo GetBlockAtSlice(PilarInfo pi, float hSlice)
		{
			float baseHeight = pi.GetStruc().GetHeight();
			for (int i = 0; i < pi.GetBlocks().Count; ++i)
			{
				var bi = pi.GetBlocks()[i];
				if (bi.BlockType == Def.BlockType.WIDE || bi.Pilar != pi)
					continue; // Cannot slice a wide

				var height = baseHeight + bi.Height;

				if (bi.BlockType == Def.BlockType.STAIRS &&
					(/*hSlice == (height - 0.5f) || */hSlice == height || hSlice == (height + 0.5f)))
					break; // Trying to slice the middle of a stair

				var len = bi.Length != 3.4f ? bi.Length : 3.5f;
				var bot = height - len;
				if (height >= hSlice &&		// is the block higher or equal to the wanted height
					(hSlice - 0.5f) >= bot) // it is its bot part below or equal to -0.5f the wanted height
					return bi;
			}
			return null;
		}
		BlockInfo SliceBlock(WideSlice[] slices, int sliceIdx, float curHeight, int side)
		{
			var block = slices[sliceIdx].Blocks[side];
			var baseHeight = block.Pilar.GetStruc().GetHeight();
			var blockHeight = block.Height + baseHeight;
			var blockLength = block.Length != 3.4f ? block.Length : 3.5f;

			block.PilarIndex = block.Pilar.GetBlocks().IndexOf(block);

			if (blockHeight > curHeight) // Slice Top excess part
			{
				var nLen = blockHeight - curHeight;
				var topBI = block.GetACopy();
				topBI.Length = nLen;
				topBI.StackedIdx[0] = block.PilarIndex;
				topBI.PilarIndex = block.PilarIndex + 1;
				block.StackedIdx[1] = block.PilarIndex + 1;
				if (topBI.StackedIdx[1] >= 0)
					topBI.StackedIdx[1] = topBI.StackedIdx[1] + 1;
				block.Pilar.GetBlocks().Insert(block.PilarIndex + 1, topBI);
				block.PropFamilyID = -1;
			}
			
			var botHeight = blockHeight - blockLength;
			// Slice bot excess part
			if (botHeight < (curHeight - 0.5f) && // is there excess at the bottom
				blockHeight > (curHeight - 0.5f)) // is the block in the slice
			{
				var botExcess = Mathf.Abs((curHeight - 0.5f) - botHeight);
				var botBI = block.GetACopy();
				botBI.Length = botExcess;
				var lenDiff = blockLength - botExcess;
				botBI.Height = block.Height - lenDiff;
				botBI.BlockType = Def.BlockType.NORMAL;
				botBI.PilarIndex = block.PilarIndex;
				block.Pilar.GetBlocks().Insert(block.PilarIndex, botBI);
				++block.PilarIndex;
				botBI.StackedIdx[0] = block.StackedIdx[0];
				botBI.StackedIdx[1] = block.PilarIndex;
				block.StackedIdx[0] = botBI.PilarIndex;
				botBI.PropFamilyID = -1;
				// Update the slices with the new block
				for(int i = (sliceIdx + 1); i < slices.Length; ++i)
				{
					if (!slices[i].Possible || slices[i].Blocks[side] != block)
						continue;
					slices[i].Blocks[side] = botBI;
				}
			}

			block.Length = 0.5f;
			block.Height = curHeight - baseHeight;
			block.BlockType = Def.BlockType.NORMAL;
			// Fix PilarIndex and stacking indices
			for (int i = 0; i < block.Pilar.GetBlocks().Count; ++i)
			{
				var b = block.Pilar.GetBlocks()[i];

				b.PilarIndex = i;

				if (b.StackedIdx[0] >= 0)
					b.StackedIdx[0] = i - 1;

				if (b.StackedIdx[1] >= 0)
					b.StackedIdx[1] = i + 1;
			}
			return block;
		}
		KeyValuePair<BlockInfo, PilarInfo> GetWideHidden(BlockInfo wideBlock, int hiddenIdx)
		{
			var pilar = m_Pilars[wideBlock.WideStackedIdx[hiddenIdx].PilarWID];
			BlockInfo hidden = null;
			for(int i = 0; i < pilar.GetBlocks().Count; ++i)
			{
				var block = pilar.GetBlocks()[i];
				if(block.Height == wideBlock.Height && block.Pilar == wideBlock.Pilar)
				{
					hidden = block;
					break;
				}
			}
			return new KeyValuePair<BlockInfo, PilarInfo>(hidden, pilar);
		}
		bool IsWideLayerValid(int matFamID, BlockInfo bi)
		{
			var struc = bi.Pilar.GetStruc();
			var biome = struc.GetBiome();

			var layer = struc.GetLayers()[bi.Layer - 1];
			if(!layer.IsLinkedLayer)
			{
				if(layer.LayerType == Def.BiomeLayerType.OTHER)
				{
					return GameUtils.ContainsAtChance(layer.MaterialFamilies, matFamID) >= 0;
				}
				else if(layer.LayerType != Def.BiomeLayerType.FULLVOID && biome != null)
				{
					var bLayer = biome.GetLayers()[(int)layer.LayerType];
					return GameUtils.ContainsAtChance(bLayer.MaterialFamilies, matFamID) >= 0;
				}
				else
				{
					return false;
				}
			}
			for(int i = 0; i < layer.LinkedLayers.Count; ++i)
			{
				var llayer = struc.GetLayers()[layer.LinkedLayers[i].ID - 1];
				if (llayer.LayerType == Def.BiomeLayerType.OTHER)
				{
					return GameUtils.ContainsAtChance(llayer.MaterialFamilies, matFamID) >= 0;
				}
				else if (llayer.LayerType != Def.BiomeLayerType.FULLVOID && biome != null)
				{
					var bLayer = biome.GetLayers()[(int)llayer.LayerType];
					return GameUtils.ContainsAtChance(bLayer.MaterialFamilies, matFamID) >= 0;
				}
			}
			return false;
		}
		void Unwidify(BlockInfo bi)
		{
			float height = bi.Height + bi.Pilar.GetStruc().GetHeight();
			var wPilar = bi.Pilar;
			BlockInfo wBlock = null;
			var wHeight = wPilar.GetStruc().GetHeight();
			for(int i = 0; i < wPilar.GetBlocks().Count; ++i)
			{
				var block = wPilar.GetBlocks()[i];
				var bHeight = block.Height + wHeight;
				if (!(block.BlockType == Def.BlockType.WIDE || block.Pilar != wPilar) || bHeight != height)
					continue;
				wBlock = block;
				break;
			}
			if(wBlock == null || wBlock.WideStackedIdx == null)
			{
				Debug.Log("Something went wrong");
				return;
			}
			if(wBlock.Pilar != wPilar)
			{
				for(int i = 0; i < wBlock.Pilar.GetBlocks().Count; ++i)
				{
					var block = wBlock.Pilar.GetBlocks()[i];
					if(block.BlockType == Def.BlockType.WIDE && block.Height == wBlock.Height)
					{
						wBlock = block;
						break;
					}
				}
			}
			if(wBlock.BlockType != Def.BlockType.WIDE)
			{
				Debug.Log("Something went wrong");
				return;
			}
			wBlock.BlockType = Def.BlockType.NORMAL;
			BlockInfo tBlock;
			for(int i = 0; i < wBlock.WideStackedIdx.Length; ++i)
			{
				var wstack = wBlock.WideStackedIdx[i];
				var pilar = m_Pilars[wstack.PilarWID];
				if (pilar == null)
					continue;
				var baseHeight = pilar.GetStruc().GetHeight();
				tBlock = null;
				for(int j = 0; j < pilar.GetBlocks().Count; ++j)
				{
					var block = pilar.GetBlocks()[j];
					var bHeight = block.Height + baseHeight;
					if (block.Pilar == pilar || bHeight != height)
						continue;
					tBlock = block;
					break;
				}
				if (tBlock == null)
				{
					Debug.Log("Something went wrong");
					return;
				}
				tBlock.BlockType = Def.BlockType.NORMAL;
				tBlock.Pilar = pilar;
			}
		}
		void FixWidePilar(PilarInfo pi)
		{
#pragma warning disable CS0162 // Se detectó código inaccesible
			if (false)
			{
				for (int i = 0; i < pi.GetBlocks().Count; ++i)
				{
					var block = pi.GetBlocks()[i];
					var bHeight0 = block.Height;
					var bHeight1 = block.BlockType == Def.BlockType.STAIRS ? bHeight0 + 0.5f : bHeight0;
					for (int j = i + 1; j < pi.GetBlocks().Count; ++j)
					{
						var test = pi.GetBlocks()[j];
						var tHeight0 = test.Height;
						var tHeight1 = test.BlockType == Def.BlockType.STAIRS ? tHeight0 + 0.5f : tHeight0;
						if (bHeight0 == tHeight0 || bHeight0 == tHeight1 || bHeight1 == tHeight0 || bHeight1 == tHeight1)
						{
							if ((block.BlockType == Def.BlockType.WIDE || block.Pilar != pi) && (test.BlockType != Def.BlockType.WIDE && pi == test.Pilar))
							{
								pi.RemoveBlock(test);
								Debug.Log("Block inside wide");
							}
							else if ((block.BlockType != Def.BlockType.WIDE && block.Pilar == pi) && (test.BlockType == Def.BlockType.WIDE || test.Pilar != pi))
							{
								pi.RemoveBlock(block);
								--i;
								Debug.Log("Block inside wide");
								break;
							}
							else // wide x wide
							{
								Debug.Log("Wide inside wide");
								Unwidify(test);
								pi.RemoveBlock(test);
							}
						}
					}
				}
			}
#pragma warning restore CS0162 // Se detectó código inaccesible
			for(int i = 0; i < pi.GetBlocks().Count; ++i)
			{
				var block = pi.GetBlocks()[i];
				block.PilarIndex = i;

				if (block.StackedIdx[0] >= 0)
					block.StackedIdx[0] = i - 1;
				if (block.StackedIdx[1] >= 0)
					block.StackedIdx[1] = i + 1;
			}
		}
		List<BlockInfo[]> PlaceWide(BlockInfo bi, PilarInfo rPi, PilarInfo bPi, PilarInfo rbPi, System.Random rng)
		{
			var len = bi.Length == 3.4f ? 3.5f : bi.Length;
			var baseHeight = bi.Pilar.GetStruc().GetHeight();
			var height = baseHeight + bi.Height;
			var sliceCount = Mathf.FloorToInt(len * 2f);
			var slices = new WideSlice[sliceCount];

			int currentSlice = 0;
			if(bi.BlockType == Def.BlockType.STAIRS)
			{
				currentSlice = 1;
				slices[0] = new WideSlice()
				{
					Possible = false,
					Blocks = null
				};
			}
			var matFamID = BlockMaterial.FamilyDict[bi.MatFamily.FamilyInfo.FamilyName];

			int possible = 0;
			for (; currentSlice < sliceCount; ++currentSlice)
			{
				var sliceBlocks = new BlockInfo[4];
				sliceBlocks[0] = bi;
				sliceBlocks[1] = GetBlockAtSlice(rPi,	height - (currentSlice * 0.5f));
				sliceBlocks[2] = GetBlockAtSlice(bPi,	height - (currentSlice * 0.5f));
				sliceBlocks[3] = GetBlockAtSlice(rbPi,	height - (currentSlice * 0.5f));

				var slice = new WideSlice()
				{
					Blocks = sliceBlocks,
					Possible = sliceBlocks[0] != null && sliceBlocks[1] != null && sliceBlocks[2] != null && sliceBlocks[3] != null
				};
				if(slice.Possible)
				{
					for(int i = 1; i < 4; ++i)
					{
						var b = sliceBlocks[i];
						if(!IsWideLayerValid(matFamID, b))
						{
							slice.Possible = false;
							break;
						}
					}
				}

				possible += slice.Possible ? 1 : 0;
				slices[currentSlice] = slice;
			}
			if (possible == 0)
				return null; // No-slice possible

			var doneWides = new List<BlockInfo[]>(sliceCount);
			for (int i = 0; i < sliceCount; ++i)
			{
				var slice = slices[i];
				if (!slice.Possible)
					continue;

				var curHeight = height - (i * 0.5f);

				var tlBlock = SliceBlock(slices, i, curHeight, 0); // TopLeft
				var trBlock = SliceBlock(slices, i, curHeight, 1); // TopRight
				var blBlock = SliceBlock(slices, i, curHeight, 2); // BotLeft
				var brBlock = SliceBlock(slices, i, curHeight, 3); // BotRight

				var wide = tlBlock;
				wide.BlockType = Def.BlockType.WIDE;
				wide.Length = 0.5f;

				wide.Height = curHeight - baseHeight; //bi.Height - (i * 0.5f);
				wide.WideStackedIdx = new WideStackLinks[3]
				{
					new WideStackLinks()
					{
						PilarWID = rPi.GetWorldID(),
						StackIdx = trBlock.StackedIdx.Clone() as int[]
					},
					new WideStackLinks()
					{
						PilarWID = bPi.GetWorldID(),
						StackIdx = blBlock.StackedIdx.Clone() as int[]
					},
					new WideStackLinks()
					{
						PilarWID = rbPi.GetWorldID(),
						StackIdx = brBlock.StackedIdx.Clone() as int[]
					}
				};
				//wide.PilarIndex = tlBlock.PilarIndex;
				//tlBlock.Pilar.GetBlocks()[tlBlock.PilarIndex] = wide;
				trBlock.Pilar = wide.Pilar; //trBlock.BlockType = Def.BlockType.WIDE;
				blBlock.Pilar = wide.Pilar; //blBlock.BlockType = Def.BlockType.WIDE;
				brBlock.Pilar = wide.Pilar; //brBlock.BlockType = Def.BlockType.WIDE;

				//trBlock.Pilar.GetBlocks()[trBlock.PilarIndex] = null;
				//trBlock.Pilar.RemoveBlock(trBlock);
				//blBlock.Pilar.GetBlocks()[blBlock.PilarIndex] = null;
				//blBlock.Pilar.RemoveBlock(blBlock);
				//brBlock.Pilar.GetBlocks()[brBlock.PilarIndex] = null;
				//brBlock.Pilar.RemoveBlock(brBlock);	
				wide.MatFamily = bi.MatFamily;
				wide.MatSet = bi.MatFamily.WideMaterials[rng.Next(0, bi.MatFamily.WideMaterials.Length)];
				//var materialID = materials[LayerInfo.RandomFromList(materials, rng)].ID;
				//wide.MatFamily = BlockMaterial.MaterialFamilies[materialID];
				//wide.MatSet = wide.MatFamily.WideMaterials[rng.Next(0, wide.MatFamily.WideMaterials.Length)];
				doneWides.Add(new BlockInfo[] { wide, trBlock, blBlock, brBlock });
			}

			FixWidePilar(bi.Pilar);
			FixWidePilar(rPi);
			FixWidePilar(bPi);
			FixWidePilar(rbPi);

			return doneWides;
		}
		//void AddElements(List<IDChance> idc, List<IDChance> nElements)
		//{
		//	for (int i = 0; i < nElements.Count; ++i)
		//	{
		//		var element = nElements[i];

		//		bool found = false;
		//		for (int j = 0; j < idc.Count; ++j)
		//		{
		//			if (element.ID == idc[j].ID)
		//			{
		//				found = true;
		//				idc[j] = new IDChance()
		//				{
		//					ID = element.ID,
		//					Chance = (ushort)(idc[j].Chance + element.Chance)
		//				};

		//				break;
		//			}
		//		}
		//		if (!found)
		//			idc.Add(element);
		//	}
		//}
		//void RemoveUnsupportedWideMaterials(List<IDChance> idc)
		//{
		//	for (int i = 0; i < idc.Count;)
		//	{
		//		var fam = BlockMaterial.MaterialFamilies[idc[i].ID];
		//		if (fam.GetSet(Def.BlockType.WIDE).Length == 0)
		//		{
		//			idc.RemoveAt(i);
		//			continue;
		//		}
		//		++i;
		//	}
		//}
		void FixProps(System.Random rng)
		{
			//for (int i = 0; i < m_StrucInfos.Count; ++i)
			//{
			//	var strucInfo = m_StrucInfos[i];
			//	var pilars = strucInfo.GetWorldPilars();
			//	for (int j = 0; j < pilars.Count; ++j)
			//	{
			//		var pilar = pilars[j];
			//		var struc = pilar.GetStruc();
			//		var biome = struc.GetBiome();
			//		for (int k = 0; k < pilar.GetBlocks().Count; ++k)
			//		{
			//			var block = pilar.GetBlocks()[k];
			//			if (block == null || block.IsStackLinkValid(1))
			//				continue;
			//			var layer = struc.GetLayers()[block.LinkedTo - 1];

			//		}
			//	}
			//}
		}
		List<BlockInfo> WideWork(System.Random rng)
		{
			//var availableMaterials = new List<IDChance>();

			bool IsInvalidPilar(PilarInfo pilar) => pilar == null || pilar.GetBlocks().Count == 0;

			var wideSlices = new List<BlockInfo[]>();
			// Instead looking all pilars just look the ones that got into the world
			for (int i = 0; i < m_StrucInfos.Count; ++i)
			{
				var strucInfo = m_StrucInfos[i];
				var pilars = strucInfo.GetWorldPilars();
				for(int j = 0; j < pilars.Count; ++j)
				{
					var pilar = pilars[j];
					var wID = pilar.GetWorldID();
					var wPos = GameUtils.PosFromIDUnsafe(wID, m_Size.x, m_Size.y);
					if ((wPos.x + 1) >= m_Size.x || (wPos.y + 1) >= m_Size.y)
						continue; // Wide outside of the world

					var rPos = new Vector2Int(wPos.x + 1,	wPos.y);
					var bPos = new Vector2Int(wPos.x,		wPos.y + 1);
					var rbPos = new Vector2Int(wPos.x + 1,	wPos.y + 1);
					var rID = GameUtils.IDFromPosUnsafe(rPos, m_Size.x, m_Size.y);
					var bID = GameUtils.IDFromPosUnsafe(bPos, m_Size.x, m_Size.y);
					var rbID = GameUtils.IDFromPosUnsafe(rbPos, m_Size.x, m_Size.y);

					var rPilar = m_Pilars[rID];
					var bPilar = m_Pilars[bID];
					var rbPilar = m_Pilars[rbID];
					if(IsInvalidPilar(rPilar) || IsInvalidPilar(bPilar) || IsInvalidPilar(rbPilar))
						continue; // No available pilars with blocks

					for(int k = 0; k < pilar.GetBlocks().Count; ++k)
					{
						var block = pilar.GetBlocks()[k];
						if (block.BlockType == Def.BlockType.WIDE || block.Pilar != pilar)
							continue; // Don't Widify a wide lol

						var layer = strucInfo.GetLayers()[block.LinkedTo - 1];
						var matFam = block.MatFamily;
						if (matFam == null || matFam.GetSet(Def.BlockType.WIDE).Length == 0)
							continue; // Wide material not available for this material family

						//availableMaterials.Clear();
						ushort wideChance = 0;
						if(layer.LayerType == Def.BiomeLayerType.OTHER)
						{
							wideChance = layer.WideBlockChance;
							//availableMaterials.AddRange(layer.MaterialFamilies);
						}
						else if(layer.LayerType != Def.BiomeLayerType.FULLVOID)
						{
							if(strucInfo.GetBiome() != null)
							{
								var biomeLayer = strucInfo.GetBiome().GetLayers()[(int)layer.LayerType];
								wideChance = biomeLayer.WideBlockChance;
								//availableMaterials.AddRange(biomeLayer.MaterialFamilies);
							}
						}
						//RemoveUnsupportedWideMaterials(availableMaterials);
						//if (availableMaterials.Count == 0)
						//{
						//	if (layer.IsLinkedLayer)
						//	{
						//		for (int w = 0; w < layer.LinkedLayers.Count; ++w)
						//		{
						//			var llID = layer.LinkedLayers[w].ID;
						//			if (llID == block.LinkedTo)
						//				continue; // We already checked this layer
						//			var llayer = strucInfo.GetLayers()[llID - 1];
						//			if(llayer.LayerType == Def.BiomeLayerType.OTHER)
						//			{
						//				AddElements(availableMaterials, llayer.MaterialFamilies);
						//				//availableMaterials.AddRange(llayer.MaterialFamilies);
						//			}
						//			else if(llayer.LayerType != Def.BiomeLayerType.FULLVOID)
						//			{
						//				if(strucInfo.GetBiome() != null)
						//				{
						//					var biomeLayer = strucInfo.GetBiome().GetLayers()[(int)llayer.LayerType];
						//					AddElements(availableMaterials, biomeLayer.MaterialFamilies);
						//					//availableMaterials.AddRange(biomeLayer.MaterialFamilies);
						//				}
						//			}
						//		}
						//		RemoveUnsupportedWideMaterials(availableMaterials);
						//		if (availableMaterials.Count == 0)
						//			continue; // No available materials neither on the main layer nor the linked layers
						//	}
						//	else
						//	{
						//		continue; // No available materials on the main layer
						//	}
						//}
						var wideRNG = (ushort)rng.Next(0, 10000);
						if(wideRNG <= wideChance)
						{
							//GameUtils.UpdateChances2(ref availableMaterials, rng);
							var placen = PlaceWide(block, rPilar, bPilar, rbPilar/*, availableMaterials*/, rng);
							if (placen != null && placen.Count > 0)
							{
								wideSlices.AddRange(placen);
								break;
							}
						}
					}
				}
			}
			// Fix stacks
			//for (int i = 0; i < m_StrucInfos.Count; ++i)
			//{
			//	var strucInfo = m_StrucInfos[i];
			//	var pilars = strucInfo.GetWorldPilars();
			//	for (int j = 0; j < pilars.Count; ++j)
			//	{
			//		var pilar = pilars[j];
			//		for(int k = 0; k < pilar.GetBlocks().Count; ++k)
			//		{
			//			var block = pilar.GetBlocks()[k];
			//			block.PilarIndex = k;
			//			if (block.StackedIdx[0] >= 0)
			//				block.StackedIdx[0] = k - 1;
			//			if (block.StackedIdx[1] >= 0)
			//				block.StackedIdx[1] = k + 1;
			//		}
			//	}
			//}
			// Merge the wide slices
			var lastStackIdx = new KeyValuePair<int, BlockInfo>[4]
					{
						new KeyValuePair<int, BlockInfo>(-1, null),
						new KeyValuePair<int, BlockInfo>(-1, null),
						new KeyValuePair<int, BlockInfo>(-1, null),
						new KeyValuePair<int, BlockInfo>(-1, null),
					};
			var doneWides = new List<BlockInfo>(wideSlices.Count);
			for (int i = 0; i < wideSlices.Count; ++i)
			{
				int mergeable = 1;
				var wideBlocks = wideSlices[i];
				var wBlock = wideBlocks[0];
				if (wBlock.BlockType != Def.BlockType.WIDE)
					continue; // has been dewidified

				float baseHeight = wBlock.Height;

				for (int j = i + 1; j < i + 5 && j < wideSlices.Count; ++j)
				{
					var tWideBlocks = wideSlices[j];
					var tWide = tWideBlocks[0];
					if (tWide.BlockType != Def.BlockType.WIDE || tWide.Pilar != wBlock.Pilar || tWide.Height != (baseHeight - (j - i) * 0.5f))
						break;
					++mergeable;
				}
				if (mergeable < 2)
				{
					doneWides.Add(wBlock);
					continue;
				}

				// Set the last stack
				var lastWBlocks = wideSlices[i + mergeable - 1];
				if (!lastWBlocks[0].Pilar.GetBlocks().Contains(lastWBlocks[0]))
					continue;
				for (int j = 0; j < lastStackIdx.Length; ++j)
					lastStackIdx[j] = new KeyValuePair<int, BlockInfo>(lastWBlocks[j].StackedIdx[0], null);
				//try
				{
					BlockInfo b = null;
					if (lastStackIdx[0].Key >= 0)
						b = wBlock.Pilar.GetBlocks()[lastStackIdx[0].Key];
					lastStackIdx[0] = new KeyValuePair<int, BlockInfo>(lastStackIdx[0].Key, b);
				}
				//catch(Exception e)
				//{
				//	Debug.Log(e.Message);
				//}
				// Remove in pilar blocks
				for (int j = i + 1; j < (i + mergeable); ++j)
				{
					wBlock.Pilar.RemoveBlock(wideSlices[j][0]);
				}

				// Remove other blocks
				for (int j = 1; j < wideBlocks.Length; ++j)
				{
					var pilar = m_Pilars[wBlock.WideStackedIdx[j - 1].PilarWID];
					for (int k = i + 1; k < i + mergeable; ++k)
					{
						if (k == (i + mergeable - 1))
						{
							BlockInfo b = null;
							if (lastStackIdx[j].Key >= 0)
								b = pilar.GetBlocks()[lastStackIdx[j].Key];
							lastStackIdx[j] = new KeyValuePair<int, BlockInfo>(lastStackIdx[j].Key, b);
						}
						pilar.RemoveBlock(wideSlices[k][j]);
					}
				}
				// Set length and bottom stacks
				for (int j = 0; j < 4; ++j)
				{
					wideBlocks[j].Length = mergeable * 0.5f;
					wideBlocks[j].StackedIdx[0] = lastStackIdx[j].Key;
					if(lastStackIdx[j].Key >= 0 && lastStackIdx[j].Value != null) 
						lastStackIdx[j].Value.StackedIdx[1] = wideBlocks[j].PilarIndex;

					PilarInfo pilar;
					if (j == 0)
						pilar = wideBlocks[0].Pilar;
					else
						pilar = m_Pilars[wideBlocks[0].WideStackedIdx[j - 1].PilarWID];
					for (int k = 0; k < pilar.GetBlocks().Count; ++k)
					{
						var block = pilar.GetBlocks()[k];
						block.PilarIndex = k;
						if (block.StackedIdx[0] >= 0)
							block.StackedIdx[0] = k - 1;
						if (block.StackedIdx[1] >= 0)
							block.StackedIdx[1] = k + 1;
					}
				}
				doneWides.Add(wideBlocks[0]);
				i += (mergeable - 2);
			}
			return doneWides;
		}
		bool DebugWideTick()
		{
			bool IsInvalidPilar(PilarInfo pilar) => pilar == null || pilar.GetBlocks().Count == 0;
			if (m_DBGStrucIDX >= m_StrucInfos.Count)
			{
				Debug.Log("Finished wide work");
				return true;
			}
			var strucInfo = m_StrucInfos[m_DBGStrucIDX];
			var pilars = strucInfo.GetWorldPilars();
			var pilar = pilars[m_DBGPilarIDX];
			var wID = pilar.GetWorldID();
			var wPos = GameUtils.PosFromIDUnsafe(wID, m_Size.x, m_Size.y);
			if ((wPos.x + 1) >= m_Size.x || (wPos.y + 1) >= m_Size.y)
			{
				Debug.Log("Wide outside of the world");
				++m_DBGPilarIDX;
				if(m_DBGPilarIDX >= pilars.Count)
				{
					++m_DBGStrucIDX;
					m_DBGPilarIDX = 0;
					m_DBGBlockIDX = 0;
				}
				return false;
			}
			var rPos = new Vector2Int(wPos.x + 1, wPos.y);
			var bPos = new Vector2Int(wPos.x, wPos.y + 1);
			var rbPos = new Vector2Int(wPos.x + 1, wPos.y + 1);
			var rID = GameUtils.IDFromPosUnsafe(rPos, m_Size.x, m_Size.y);
			var bID = GameUtils.IDFromPosUnsafe(bPos, m_Size.x, m_Size.y);
			var rbID = GameUtils.IDFromPosUnsafe(rbPos, m_Size.x, m_Size.y);

			var rPilar = m_Pilars[rID];
			var bPilar = m_Pilars[bID];
			var rbPilar = m_Pilars[rbID];

			if (IsInvalidPilar(rPilar) || IsInvalidPilar(bPilar) || IsInvalidPilar(rbPilar))
			{
				Debug.Log("No available pilars with blocks");
				++m_DBGPilarIDX;
				if (m_DBGPilarIDX >= pilars.Count)
				{
					++m_DBGStrucIDX;
					m_DBGPilarIDX = 0;
					m_DBGBlockIDX = 0;
				}
				return false;
			}

			var blocks = pilar.GetBlocks();
			var block = blocks[m_DBGBlockIDX];
			if(block.BlockType == Def.BlockType.WIDE)
			{
				Debug.Log("Block was already WIDE");
				++m_DBGBlockIDX;
				if(m_DBGBlockIDX >= blocks.Count)
				{
					++m_DBGPilarIDX;
					m_DBGBlockIDX = 0;
					if (m_DBGPilarIDX >= pilars.Count)
					{
						++m_DBGStrucIDX;
						m_DBGPilarIDX = 0;
						m_DBGBlockIDX = 0;
					}
				}
				return false;
			}

			var layer = strucInfo.GetLayers()[block.LinkedTo - 1];
			var matFam = block.MatFamily;
			if (matFam == null || matFam.GetSet(Def.BlockType.WIDE).Length == 0)
			{
				Debug.Log("Wide material not available for this material family");
				++m_DBGBlockIDX;
				if (m_DBGBlockIDX >= blocks.Count)
				{
					++m_DBGPilarIDX;
					m_DBGBlockIDX = 0;
					if (m_DBGPilarIDX >= pilars.Count)
					{
						++m_DBGStrucIDX;
						m_DBGPilarIDX = 0;
						m_DBGBlockIDX = 0;
					}
				}
				return false;
			}
			ushort wideChance = 0;
			if (layer.LayerType == Def.BiomeLayerType.OTHER)
			{
				wideChance = layer.WideBlockChance;
			}
			else if (layer.LayerType != Def.BiomeLayerType.FULLVOID)
			{
				if (strucInfo.GetBiome() != null)
				{
					var biomeLayer = strucInfo.GetBiome().GetLayers()[(int)layer.LayerType];
					wideChance = biomeLayer.WideBlockChance;
				}
			}

			var wideRNG = (ushort)m_DBGRng.Next(0, 10000);
			if(wideRNG > wideChance)
			{
				Debug.Log("WIDE didn't had chance");
				++m_DBGBlockIDX;
				if (m_DBGBlockIDX >= blocks.Count)
				{
					++m_DBGPilarIDX;
					m_DBGBlockIDX = 0;
					if (m_DBGPilarIDX >= pilars.Count)
					{
						++m_DBGStrucIDX;
						m_DBGPilarIDX = 0;
						m_DBGBlockIDX = 0;
					}
				}
				return false;
			}
			var placen = PlaceWide(block, rPilar, bPilar, rbPilar, m_DBGRng);
			if(placen == null || placen.Count == 0)
			{
				Debug.Log("WIDE couldn't be placed");
				++m_DBGBlockIDX;
				if (m_DBGBlockIDX >= blocks.Count)
				{
					++m_DBGPilarIDX;
					m_DBGBlockIDX = 0;
					if (m_DBGPilarIDX >= pilars.Count)
					{
						++m_DBGStrucIDX;
						m_DBGPilarIDX = 0;
						m_DBGBlockIDX = 0;
					}
				}
				return false;
			}

			Debug.Log($"WIDE placed at Struc{m_DBGStrucIDX} ({wPos.x},{wPos.y})");
			++m_DBGBlockIDX;
			if (m_DBGBlockIDX >= blocks.Count)
			{
				++m_DBGPilarIDX;
				m_DBGBlockIDX = 0;
				if (m_DBGPilarIDX >= pilars.Count)
				{
					++m_DBGStrucIDX;
					m_DBGPilarIDX = 0;
					m_DBGBlockIDX = 0;
				}
			}
			return true;
		}
		void MicroheightWork(List<BlockInfo> wideBlocks, System.Random rng)
		{
			void MicroheightDown(BlockInfo b, PilarInfo p, float mh)
			{
				b.MicroHeight = mh;
				b.MicroheightApplied = true;
				if(b.BlockType == Def.BlockType.WIDE)
				{
					for(int i = 0; i < b.WideStackedIdx.Length; ++i)
					{
						var pilar = m_Pilars[b.WideStackedIdx[i].PilarWID];
						if (pilar == null)
						{
							Debug.LogWarning("Couldn't find WideStackedPilar");
							continue;
						}
						BlockInfo hidden = null;
						for (int j = 0; j < pilar.GetBlocks().Count; ++j)
						{
							var block = pilar.GetBlocks()[j];
							if(block.Height == b.Height && block.Pilar == p)
							{
								hidden = block;
								break;
							}
						}
						if (hidden == null)
							continue;

						hidden.MicroHeight = mh;
						hidden.MicroheightApplied = true;
						var hIdx = pilar.GetBlocks().IndexOf(hidden);
						BlockInfo hiddenBelow = null;
						if (hidden.StackedIdx[0] >= 0 && hIdx > 0)
							hiddenBelow = pilar.GetBlocks()[hIdx - 1];
						if (hiddenBelow != null)
							MicroheightDown(hiddenBelow, pilar, mh);
					}
				}
				
				var idx = p.GetBlocks().IndexOf(b);
				BlockInfo below = null;
				if(b.StackedIdx[0] >= 0 && idx > 0)
					below = p.GetBlocks()[idx - 1];
				if (below != null)
					MicroheightDown(below, p, mh);
			}
			void MicroheightUp(BlockInfo b, PilarInfo p, float mh)
			{
				b.MicroHeight = mh;
				b.MicroheightApplied = true;
				if (b.BlockType == Def.BlockType.WIDE)
				{
					for (int i = 0; i < b.WideStackedIdx.Length; ++i)
					{
						var pilar = m_Pilars[b.WideStackedIdx[i].PilarWID];
						if (pilar == null)
						{
							Debug.LogWarning("Couldn't find WideStackedPilar");
							continue;
						}
						BlockInfo hidden = null;
						for (int j = 0; j < pilar.GetBlocks().Count; ++j)
						{
							var block = pilar.GetBlocks()[j];
							if (block.Height == b.Height && block.Pilar == p)
							{
								hidden = block;
								break;
							}
						}
						if (hidden == null)
							continue;

						hidden.MicroHeight = mh;
						hidden.MicroheightApplied = true;
						var hIdx = pilar.GetBlocks().IndexOf(hidden);
						BlockInfo hiddenBelow = null;
						if (hidden.StackedIdx[1] >= 0 && hIdx >= 0 && pilar.GetBlocks().Count > (hIdx + 1))
							hiddenBelow = pilar.GetBlocks()[hIdx + 1];
						if (hiddenBelow != null)
							MicroheightUp(hiddenBelow, pilar, mh);
					}
				}

				var idx = p.GetBlocks().IndexOf(b);
				BlockInfo above = null;
				if (b.StackedIdx[1] >= 0 && idx >= 0 && p.GetBlocks().Count > (idx + 1))
				{
					above = p.GetBlocks()[idx + 1];
				}
				if (above != null)
					MicroheightUp(above, p, mh);
			}

			void ApplyMicroheight(BlockInfo b, PilarInfo p, float mh)
			{
				//if (b.MicroheightApplied)
				//	return;
				var idx = p.GetBlocks().IndexOf(b);
				BlockInfo below = null;
				if (idx > 0 && b.StackedIdx[0] >= 0)
					below = p.GetBlocks()[idx - 1];
				//BlockInfo above = null;
				//if (b.StackedIdx[1] >= 0 && idx >= 0 && p.GetBlocks().Count > (idx + 1))
				//	above = p.GetBlocks()[idx + 1];

				if (b.BlockType == Def.BlockType.WIDE || b.Pilar != p)
				{
					//if (above != null)
					//	ApplyMicroheight(above, p, mh);
					if (below != null)
						ApplyMicroheight(below, p, mh);
					return;
				}

				b.MicroHeight = mh;
				b.MicroheightApplied = true;

				//if (above != null)
				//	ApplyMicroheight(above, p, mh);
				if (below != null)
					ApplyMicroheight(below, p, mh);
			}

			for(int i = 0; i < wideBlocks.Count; ++i)
			{
				var wide = wideBlocks[i];
				if (wide == null || wide.MicroheightApplied)
					continue;

				// get the top stacked block
				var pilar = wide.Pilar;
				var wideIdx = pilar.GetBlocks().IndexOf(wide);
				if (wideIdx < 0)
					continue;

				var struc = pilar.GetStruc();
				var biome = struc.GetBiome();
				BlockInfo topBlock = wide;
				var topIdx = wideIdx;
				//try
				//{
					while (topBlock.StackedIdx[1] >= 0 && topIdx < pilar.GetBlocks().Count)
					{
						topBlock = pilar.GetBlocks()[topIdx++];
					}
				//}catch(Exception e)
				//{
				//	Debug.Log(e.Message);
				//}
				// obtain the microheight for that block

				var layer = struc.GetLayers()[topBlock.LinkedTo - 1];
				float maxMicroHeigth, minMicroHeight;
				if (layer.LayerType == Def.BiomeLayerType.OTHER)
				{
					maxMicroHeigth = layer.MicroHeightMax;
					minMicroHeight = layer.MicroHeightMin;
				}
				else if (layer.LayerType != Def.BiomeLayerType.FULLVOID && biome != null)
				{
					var bLayer = biome.GetLayers()[(int)layer.LayerType];
					maxMicroHeigth = bLayer.MicroHeightMax;
					minMicroHeight = bLayer.MicroHeightMin;
				}
				else
				{
					maxMicroHeigth = minMicroHeight = 0f;
				}
				float microHeight;
				if (maxMicroHeigth != minMicroHeight)
				{
					var rngVal = rng.Next(Mathf.FloorToInt(minMicroHeight * 20f/*/ 0.05f*/),
						Mathf.FloorToInt(maxMicroHeigth * 20f/*/ 0.05f*/ + 1));
					microHeight = rngVal * 0.05f;
				}
				else
				{
					microHeight = maxMicroHeigth;
				}

				MicroheightUp(wide, pilar, microHeight);
				MicroheightDown(wide, pilar, microHeight);
			}

			for (int i = 0; i < m_StrucInfos.Count; ++i)
			{
				var strucInfo = m_StrucInfos[i];
				var pilars = strucInfo.GetWorldPilars();
				for (int j = 0; j < pilars.Count; ++j)
				{
					var pilar = pilars[j];
					var struc = pilar.GetStruc();
					var biome = struc.GetBiome();
					for(int k = 0; k < pilar.GetBlocks().Count; ++k)
					{
						var block = pilar.GetBlocks()[k];
						if (block == null || block.BlockType == Def.BlockType.WIDE || block.Pilar != pilar || block.MicroheightApplied || block.IsStackLinkValid(1))
							continue;

						//if(block.BlockType == Def.BlockType.WIDE)
						//{
						//	wideBlocks.Add(block);
						//	continue;
						//}

						var layer = struc.GetLayers()[block.LinkedTo - 1];
						float maxMicroHeigth, minMicroHeight;
						if(layer.LayerType == Def.BiomeLayerType.OTHER)
						{
							maxMicroHeigth = layer.MicroHeightMax;
							minMicroHeight = layer.MicroHeightMin;
						}
						else if(layer.LayerType != Def.BiomeLayerType.FULLVOID && biome != null)
						{
							var bLayer = biome.GetLayers()[(int)layer.LayerType];
							maxMicroHeigth = bLayer.MicroHeightMax;
							minMicroHeight = bLayer.MicroHeightMin;
						}
						else
						{
							maxMicroHeigth = minMicroHeight = 0f;
						}
						float microHeight;
						if(maxMicroHeigth != minMicroHeight)
						{
							var rngVal = rng.Next(Mathf.FloorToInt(minMicroHeight * 20f/*/ 0.05f*/),
								Mathf.FloorToInt(maxMicroHeigth * 20f/*/ 0.05f*/ + 1));
							microHeight = rngVal * 0.05f;
						}
						else
						{
							microHeight = maxMicroHeigth;
						}
						ApplyMicroheight(block, pilar, microHeight);
					}
				}
			}
			
			//for(int i = 0; i < wideBlocks.Count; ++i)
			//{
			//	var block = wideBlocks[i];
			//	if (block.MicroheightApplied)
			//		continue;

			//	var pilar = block.Pilar;
			//	var struc = pilar.GetStruc();
			//	var biome = struc.GetBiome();
			//	var layer = struc.GetLayers()[block.LinkedTo - 1];
			//	float maxMicroHeigth, minMicroHeight;
			//	if (layer.LayerType == Def.BiomeLayerType.OTHER)
			//	{
			//		maxMicroHeigth = layer.MicroHeightMax;
			//		minMicroHeight = layer.MicroHeightMin;
			//	}
			//	else if (layer.LayerType != Def.BiomeLayerType.FULLVOID && biome != null)
			//	{
			//		var bLayer = biome.GetLayers()[(int)layer.LayerType];
			//		maxMicroHeigth = bLayer.MicroHeightMax;
			//		minMicroHeight = bLayer.MicroHeightMin;
			//	}
			//	else
			//	{
			//		maxMicroHeigth = minMicroHeight = 0f;
			//	}
			//	float microHeight;
			//	if (maxMicroHeigth != minMicroHeight)
			//	{
			//		var rngVal = rng.Next(Mathf.FloorToInt(minMicroHeight * 20f/*/ 0.05f*/),
			//			Mathf.FloorToInt(maxMicroHeigth * 20f/*/ 0.05f*/ + 1));
			//		microHeight = rngVal * 0.05f;
			//	}
			//	else
			//	{
			//		microHeight = maxMicroHeigth;
			//	}
			//	MicroheightUp(block, pilar, microHeight);
			//	MicroheightDown(block, pilar, microHeight);
			//}
		}
		void PlaceStructures()
		{
			//UnityEngine.Profiling.Profiler.BeginSample("StrucCollection");
			var structureIDs = new int[m_StructureNames.Length];
			var structureSizes = new Vector2Int[m_StructureNames.Length];
			var sizes = new List<Vector2Int>(m_StructureNames.Length);
			for (int i = 0; i < m_StructureNames.Length; ++i)
			{
				structureIDs[i] = Structures.StrucDict[m_StructureNames[i]];
				var ie = Structures.Strucs[structureIDs[i]];
				var size = new Vector2Int(ie.GetWidth(), ie.GetHeight());
				structureSizes[i] = size;
				if (!sizes.Contains(size))
					sizes.Add(size);
			}
			//UnityEngine.Profiling.Profiler.EndSample();
			var RNG = new System.Random(m_Seed);

			var toPlace = new List<PlacingInfo>();

			var availablePlacements = new List<PlacementInfo>(structureIDs.Length);
			var availableStructures = new List<int>(structureIDs.Length);

			var densityMap = m_BiomeStat[(int)Def.BiomeStat.Density];
			var temperatureMap = m_BiomeStat[(int)Def.BiomeStat.Temperature];
			var heightMap = m_BiomeStat[(int)Def.BiomeStat.Height];
			var soulnessMap = m_BiomeStat[(int)Def.BiomeStat.Soulness];
			var wealthMap = m_BiomeStat[(int)Def.BiomeStat.Wealth];

			var densityPerlin = m_Perlins[(int)Def.WorldPerlins.Density];
			//var temperaturePerlin = m_Perlins[(int)Def.BiomeStat.Temperature];
			//var heightPerlin = m_Perlins[(int)Def.BiomeStat.Height];
			//var soulnessPerlin = m_Perlins[(int)Def.BiomeStat.Soulness];
			//var wealthPerlin = m_Perlins[(int)Def.BiomeStat.Wealth];

			// Set the BiomeMap
			//UnityEngine.Profiling.Profiler.BeginSample("BiomeAssign");
			var biomeList = new List<int>(BiomeLoader.Biomes.Count);
			for (int i = 0; i < BiomeLoader.Biomes.Count; ++i)
			{
				var biome = BiomeLoader.Biomes[i];
				if (biome == null)
					continue;
				biomeList.Add(i);
			}
			var placingList = new List<BiomePlaceInfo>(biomeList.Count);
			for (int i = 0; i < m_BiomeMap.Length; ++i)
			{
				if (m_BiomeMap[i] >= 0)
					continue;

				placingList.Clear();

				int maxChance = int.MinValue;
				for (int j = 0; j < biomeList.Count; ++j)
				{
					var idxIE = biomeList[j];
					var biomeIE = BiomeLoader.Biomes[idxIE];

					var density = densityMap[i];
					var temperature = temperatureMap[i];
					var height = heightMap[i];
					var soulness = soulnessMap[i];
					var wealth = wealthMap[i];

					int chance = 0;
					biomeIE.GetBiomeStat(Def.BiomeStat.Density, out int min, out int max);
					if (density >= min && density <= max)
						++chance;

					biomeIE.GetBiomeStat(Def.BiomeStat.Temperature, out min, out max);
					if (temperature >= min && temperature <= max)
						++chance;

					biomeIE.GetBiomeStat(Def.BiomeStat.Height, out min, out max);
					if (height >= min && height <= max)
						++chance;

					biomeIE.GetBiomeStat(Def.BiomeStat.Soulness, out min, out max);
					if (soulness >= min && soulness <= max)
						++chance;

					biomeIE.GetBiomeStat(Def.BiomeStat.Wealth, out min, out max);
					if (wealth >= min && wealth <= max)
						++chance;

					if (chance > maxChance)
					{
						placingList.Clear();
						placingList.Add(new BiomePlaceInfo() { IDXIE = idxIE, Chance = chance });
						maxChance = chance;
					}
					else if (chance == maxChance)
					{
						placingList.Add(new BiomePlaceInfo() { IDXIE = idxIE, Chance = chance });
					}
				}
				Biome biome = null;
				if (placingList.Count > 0)
				{
					var biomeInfo = placingList[RNG.Next(0, placingList.Count)];
					var biomeIE = BiomeLoader.Biomes[biomeInfo.IDXIE];
					biome = biomeIE.ToBiome();
				}
				if(biome != null)
				{
					var initPos = GameUtils.PosFromIDUnsafe(i, m_Size.x, m_Size.y); // it should be impossible to go outside of bounds
					var size = biome.GetMinDistance();
					var area = size * size;
					for(int j = 0; j < area; ++j)
					{
						var pos = initPos + GameUtils.PosFromIDUnsafe(j, size, size); // it should be impossible to go outside of bounds
						if (pos.x < 0 || pos.x >= m_Size.x || pos.y < 0 || pos.y >= m_Size.y)
							continue;
						var id = GameUtils.IDFromPosUnsafe(pos, m_Size.x, m_Size.y); // it should be impossible to go outside of bounds
						m_BiomeMap[id] = biome.IDXIE;
					}
				}
			}
			//UnityEngine.Profiling.Profiler.EndSample();

			var biomeCommon = new Dictionary<int, int>(biomeList.Count);
			void SetBiomeCommon() { for (int i = 0; i < biomeList.Count; ++i) biomeCommon.Add(biomeList[i], 0); }
			void ResetBiomeCommon() { biomeCommon.Clear(); SetBiomeCommon(); }
			var biomeMaxCommon = new List<int>(biomeList.Count);
			int maxCommonAmount;

			//UnityEngine.Profiling.Profiler.BeginSample("ChooseStructures");
			for (int i = 0; i < densityPerlin.Samples.Length; ++i)
			{
				availablePlacements.Clear();
				
				var perlinChance = densityPerlin.Samples[i];
				var rngChance = (float)RNG.NextDouble();
				if (rngChance > perlinChance)
					continue;

				var strucPos = GameUtils.PosFromIDUnsafe(i, m_Size.x, m_Size.y); // it should be impossible to go outside of bounds
				for (int j = 0; j < sizes.Count; ++j)
				{
					var testSize = sizes[j];
					if ((strucPos.x + testSize.x > m_Size.x) || strucPos.y + testSize.y > m_Size.y)
						continue;
					
					ResetBiomeCommon();
					maxCommonAmount = int.MinValue;
					biomeMaxCommon.Clear();

					var area = testSize.x * testSize.y;
					bool canFit = true;
					var pinfo = new PlacementInfo
					{
						Size = testSize
					};

					for (int k = 0; k < area; ++k)
					{
						var testOffset = GameUtils.PosFromIDUnsafe(k, testSize.x, testSize.y); // it should be impossible to go outside of bounds
						var testPos = strucPos + testOffset;
						var testID = GameUtils.IDFromPosUnsafe(testPos, m_Size.x, m_Size.y); // it should be impossible to go outside of bounds
						var testDensity = densityPerlin.Samples[testID];
						if (testDensity < 0f)
						{
							canFit = false;
							break;
						}
						var biomeIDXIE = m_BiomeMap[testID];
						if(biomeCommon.ContainsKey(biomeIDXIE))
						{
							var val = biomeCommon[biomeIDXIE] + 1;
							biomeCommon[biomeIDXIE] = val;
							if(val > maxCommonAmount)
							{
								maxCommonAmount = val;
								biomeMaxCommon.Clear();
								biomeMaxCommon.Add(biomeIDXIE);
							}
							else if(val == maxCommonAmount)
							{
								biomeMaxCommon.Add(biomeIDXIE);
							}	
						}
						//var density = densityMap[testID];
						//var temp = temperatureMap[testID];
						//var height = heightMap[testID];
						//var soulness = soulnessMap[testID];
						//var wealth = wealthMap[testID];
						//pinfo.Density += density;
						//pinfo.Temperature += temp;
						//pinfo.Height += height;
						//pinfo.Soulness += soulness;
						//pinfo.Wealth += wealth;
					}
					if (canFit)
					{
						//pinfo.Density =		Mathf.RoundToInt(pinfo.Density		/ (float)area);
						//pinfo.Temperature = Mathf.RoundToInt(pinfo.Temperature	/ (float)area);
						//pinfo.Height =		Mathf.RoundToInt(pinfo.Height		/ (float)area);
						//pinfo.Soulness =	Mathf.RoundToInt(pinfo.Soulness		/ (float)area);
						//pinfo.Wealth =		Mathf.RoundToInt(pinfo.Wealth		/ (float)area);
						pinfo.BiomeIDXIE = biomeMaxCommon[RNG.Next(0, biomeMaxCommon.Count)];
						availablePlacements.Add(pinfo);
					}
				}
				if (availablePlacements.Count == 0)
					continue;

				availableStructures.Clear();

				// pick a size
				var sizeIdx = RNG.Next(0, availablePlacements.Count);
				var placementInfo = availablePlacements[sizeIdx];
				for(int j = 0; j < structureSizes.Length; ++j)
				{
					if (structureSizes[j] == placementInfo.Size)
						availableStructures.Add(j);
				}

				// pick a structure
				var strucIdx = RNG.Next(0, availableStructures.Count);
				var strucIDXIE = structureIDs[availableStructures[strucIdx]];
				toPlace.Add(new PlacingInfo()
				{
					StrucIDXIE = strucIDXIE,
					Position = strucPos,
					BiomeIDXIE = placementInfo.BiomeIDXIE
					//Density = placementInfo.Density,
					//Temperature = placementInfo.Temperature,
					//Height = placementInfo.Height,
					//Soulness = placementInfo.Soulness,
					//Wealth = placementInfo.Wealth
				});

				// erase density values, to avoid entire superposition
				int eraseLength = Mathf.FloorToInt(NextPositionOffsetMin * perlinChance + NextPositionOffsetMax * (1f - perlinChance));
				var startPosition = strucPos + new Vector2Int(placementInfo.Size.x / 2 - eraseLength / 2, placementInfo.Size.y / 2 - eraseLength / 2);
				int eraseArea = eraseLength * eraseLength;
				for (int j = 0; j < eraseArea; ++j)
				{
					var erasePos = startPosition + GameUtils.PosFromIDUnsafe(j, eraseLength, eraseLength);  // it should be impossible to go outside of bounds
					if (erasePos.x < 0 || erasePos.x >= m_Size.x || erasePos.y < 0 || erasePos.y >= m_Size.y)
						continue;
					var eraseID = GameUtils.IDFromPosUnsafe(erasePos, m_Size.x, m_Size.y);  // it should be impossible to go outside of bounds
					//if (eraseID < 0 || eraseID >= m_PerlinSamples.Length)
					//	continue;
					densityPerlin.Samples[eraseID] = -1f;
				}
			}

			m_StrucInfos.AddRange(Enumerable.Repeat<StrucInfo>(null, toPlace.Count));
			//UnityEngine.Profiling.Profiler.EndSample();

			var coreCount = CPUCores;

			//UnityEngine.Profiling.Profiler.BeginSample("PlaceStructures");
			if (coreCount > 0)
			{
				var threads = new Thread[coreCount];
				int split = Mathf.CeilToInt(toPlace.Count / (float)coreCount);
				for (int i = 0; i < coreCount; ++i)
				{
					var thread = new Thread(StructureThreadWork);
					thread.Start(new StructureWorkData() { Placings = toPlace, Start = i * split, End = (i + 1) * split, RNG = new System.Random(i) });
					threads[i] = thread;
				}
				for (int i = 0; i < coreCount; ++i)
					threads[i].Join();
			}
			else
			{
				for (int i = 0; i < toPlace.Count; ++i)
					PlaceStruc(toPlace[i], i, RNG);
			}
			if (m_WideDebug)
			{
				m_DBGStrucIDX = 0;
				m_DBGPilarIDX = 0;
				m_DBGBlockIDX = 0;
				m_DBGRng = RNG;
				return;
			}
			//UnityEngine.Profiling.Profiler.EndSample();
			//UnityEngine.Profiling.Profiler.BeginSample("WideWork");
			var wides = WideWork(RNG);
			//UnityEngine.Profiling.Profiler.EndSample();

			//UnityEngine.Profiling.Profiler.BeginSample("MicroheightWork");
			MicroheightWork(wides, RNG);
			//UnityEngine.Profiling.Profiler.EndSample();
			//var toPlace = new List<PlacingInfo>();
			//UnityEngine.Random.InitState(m_Seed);
			FixProps(RNG);
			//var availableStrucs = new List<int>(structureIDs.Length);
			//for(int i = 0; i < m_PerlinSamples.Length; ++i)
			//{
			//	var prob = m_PerlinSamples[i];
			//	var chance = UnityEngine.Random.value;
			//	if (chance > prob)
			//		continue;
			//	availableStrucs.Clear();
			//	var strucPos = GameUtils.PosFromID(i, m_Size.x, m_Size.y);
			//	for(int j = 0; j < structureIDs.Length; ++j)
			//	{
			//		var ie = Structures.Strucs[structureIDs[j]];
			//		var width = ie.GetWidth();
			//		var height = ie.GetHeight();
			//		if ((strucPos.x + width >= m_Size.x) || strucPos.y + height >= m_Size.y)
			//			continue;
			//		var area = width * height;

			//		bool canFit = true;
			//		for(int k = 0; k < area; ++k)
			//		{
			//			var testOffset = GameUtils.PosFromID(k, width, height);
			//			var testPos = strucPos + testOffset;
			//			var testID = GameUtils.IDFromPos(testPos, m_Size.x, m_Size.y);
			//			if(testID < 0f)
			//			{
			//				canFit = false;
			//				break;
			//			}
			//		}
			//		if (canFit)
			//		{
			//			availableStrucs.Add(j);
			//		}
			//	}
			//	if (availableStrucs.Count == 0)
			//		continue;

			//	int strucIDX = availableStrucs[UnityEngine.Random.Range(0, availableStrucs.Count)];
			//	int strucID = structureIDs[strucIDX];
			//	var strucIE = Structures.Strucs[strucID];

			//	toPlace.Add(new PlacingInfo() { Position = strucPos, StrucIDXIE = strucID });
			//	int eraseLength = Mathf.FloorToInt(NextPositionOffsetMin * prob + NextPositionOffsetMax * (1f - prob));
			//	var startPosition = strucPos + new Vector2Int(strucIE.GetWidth() / 2 - eraseLength / 2, strucIE.GetHeight() / 2 - eraseLength / 2);
			//	int eraseArea = eraseLength * eraseLength;
			//	for (int j = 0; j < eraseArea; ++j)
			//	{
			//		var erasePos = startPosition + GameUtils.PosFromID(j, eraseLength, eraseLength);
			//		if (erasePos.x < 0 || erasePos.x >= m_Size.x || erasePos.y < 0 || erasePos.y >= m_Size.y)
			//			continue;
			//		var eraseID = GameUtils.IDFromPos(erasePos, m_Size.x, m_Size.y);
			//		//if (eraseID < 0 || eraseID >= m_PerlinSamples.Length)
			//		//	continue;
			//		m_PerlinSamples[eraseID] = -1f;
			//	}
			//}
			////UpdatePerlin();
			//m_StrucInfos.AddRange(Enumerable.Repeat<StrucInfo>(null, toPlace.Count));

			//var coreCount = CPUCores;
			//var threads = new Thread[coreCount];
			//int split = Mathf.CeilToInt(toPlace.Count / (float)coreCount);

			//for(int i = 0; i < coreCount; ++i)
			//{
			//	int start = i * split;
			//	int end = (i + 1) * split;
			//	var rng = new System.Random(i);
			//	void fn() => ThreadWork(toPlace, start, end, rng);
			//	var thread = new Thread(fn);
			//	thread.Start();
			//	threads[i] = thread;
			//}
			//for (int i = 0; i < coreCount; ++i)
			//	threads[i].Join();
		}
		void StructureThreadWork(object obj)
		{
			var data = (StructureWorkData)obj;
			//var biomeList = new List<int>(BiomeLoader.Biomes.Count);
			//for(int i = 0; i < BiomeLoader.Biomes.Count; ++i)
			//{
			//	var biome = BiomeLoader.Biomes[i];
			//	if (biome == null)
			//		continue;
			//	biomeList.Add(i);
			//}
			//var placingList = new List<BiomePlaceInfo>(biomeList.Count);
			//UnityEngine.Profiling.Profiler.BeginThreadProfiling("PlaceStructuresWork", "PlaceStruc");
			for (int i = data.Start; i < data.End && i < data.Placings.Count; ++i)
			{
				var info = data.Placings[i];
				//UnityEngine.Profiling.Profiler.BeginSample("PlaceStruc" + i.ToString());
				PlaceStruc(info, i, /*biomeList, placingList,*/ data.RNG);
				//UnityEngine.Profiling.Profiler.EndSample();
				//placingList.Clear();
			}
			//UnityEngine.Profiling.Profiler.EndThreadProfiling();
		}
		void PerlinThreadWork(object obj)
		{
			int perlinID = (int)obj;
			//UnityEngine.Profiling.Profiler.BeginThreadProfiling("PerlinThreadWork", "Perlin" + perlinID.ToString());
			//UnityEngine.Profiling.Profiler.BeginSample("Perlin" + perlinID.ToString());
			var perlin = m_Perlins[perlinID];
			perlin.Size = m_Size;
			perlin.Seed = m_Seed + perlinID;
			perlin.Offset = m_Offset;
			var rng = new System.Random(perlin.Seed);
			var fMin = PerlinDef[perlinID * 4];
			var fRange = PerlinDef[perlinID * 4 + 1] - fMin;
			perlin.Frequency = ((float)rng.NextDouble()) * fRange + fMin;
			var cMin = PerlinDef[perlinID * 4 + 2];
			var cRange = PerlinDef[perlinID * 4 + 3] - cMin;
			perlin.Contrast = ((float)rng.NextDouble()) * cRange + cMin;
			perlin.Generate();
			perlin.Seamlessify();
			if (perlinID < m_BiomeStat.Length)
			{
				//UnityEngine.Profiling.Profiler.BeginSample(((Def.BiomeStat)perlinID).ToString());
				for (int i = 0; i < perlin.Samples.Length; ++i)
				{
					m_BiomeStat[perlinID][i] = Mathf.RoundToInt(perlin.Samples[i] * 4f - 2f);
				}
				//UnityEngine.Profiling.Profiler.EndSample();
			}
			//UnityEngine.Profiling.Profiler.EndSample();
			//UnityEngine.Profiling.Profiler.EndThreadProfiling();
		}
		public void Generate(Vector2Int size, int seed, List<string> structureNames, Vector2 offset = default, bool wideDebug = false)
		{
			if (size.x % CStruc.Height != 0 || size.y % CStruc.Height != 0)
				throw new Exception("Trying to create a world with now power of 16 size!" + size.ToString());

			if (structureNames == null || structureNames.Count == 0)
				throw new Exception("Trying to create a 0 structure world");

			//UnityEngine.Profiling.Profiler.BeginSample("WorldGen");
			var now = DateTime.Now;
			m_Size = size;
			m_StrucWorldSize = new Vector2Int((int)(m_Size.x / CStruc.Width), (int)(m_Size.y / CStruc.Height));
			m_StructureNames = structureNames.ToArray();
			m_Offset = offset;
			m_WideDebug = wideDebug;
			m_Pilars = new PilarInfo[m_Size.x * m_Size.y];
			m_BiomeMap = Enumerable.Repeat(-1, m_Pilars.Length).ToArray();
			m_BiomeStat = new int[Def.BiomeStatCount][];
			for (int i = 0; i < m_BiomeStat.Length; ++i) m_BiomeStat[i] = new int[m_Size.x * m_Size.y];
			if(m_Strucs != null)
			{
				for(int i = 0; i < m_Strucs.Length; ++i)
				{
					var struc = m_Strucs[i];
					if (struc != null)
					{
						struc.UnloadFromWorld();
						for (int j = 0; j < struc.GetLES().Count; ++j)
							GameUtils.DeleteGameobject(struc.GetLES()[j].gameObject);
						GameUtils.DeleteGameobject(struc.gameObject);
					}
				}
			}
			var strucCount = m_StrucWorldSize.x * m_StrucWorldSize.y;
			m_Strucs = new CStruc[strucCount];
			for (int i = 0; i < m_Strucs.Length; ++i)
			{
				var struc = CStruc.CreateFromWorld(i, m_StrucWorldSize.x, m_StrucWorldSize.y);
				struc.transform.SetParent(transform, true);
				struc.gameObject.SetActive(false);
				m_Strucs[i] = struc;
			}
			m_Seed = seed;

			// Compute perlins
			//UnityEngine.Profiling.Profiler.BeginSample("PerlinGenerate");
			var threads = new Thread[Def.WorldPerlinCount];
			for(int i = 0; i < m_Perlins.Length; ++i)
			{
				var thread = new Thread(PerlinThreadWork);
				thread.Start(i);
				threads[i] = thread;
			}
			for (int i = 0; i < threads.Length; ++i)
				threads[i].Join();
			//UnityEngine.Profiling.Profiler.EndSample();
			//for (int i = 0; i < m_Perlins.Length; ++i)
			//{
			//	var perlin = m_Perlins[i];
			//	perlin.Size = size;
			//	perlin.Seed = m_Seed + i;
			//	var rng = new System.Random(perlin.Seed);
			//	perlin.Offset = offset;
			//	var fMin = PerlinDef[i * 4];
			//	var fRange = PerlinDef[i * 4 + 1] - fMin;
			//	perlin.Frequency = ((float)rng.NextDouble()) * fRange + fMin;
			//	var cMin = PerlinDef[i * 4 + 2];
			//	var cRange = PerlinDef[i * 4 + 3] - cMin;
			//	perlin.Contrast = ((float)rng.NextDouble()) * cRange + cMin;
			//	perlin.Generate();
			//}
			m_StrucInfos.Clear();
			//UnityEngine.Profiling.Profiler.BeginSample("PlaceStructures");
			PlaceStructures();
			//UnityEngine.Profiling.Profiler.EndSample();
			var after = DateTime.Now;

			var diff = after - now;

			Debug.Log("Finished Loading map, " + m_StrucInfos.Count.ToString() + " structures in " + diff.ToString());

			if (ShowBiomeCubes)
			{
#pragma warning disable CS0162 // Se detectó código inaccesible
				if (m_StrucCubes != null)
#pragma warning restore CS0162 // Se detectó código inaccesible
				{
					for (int i = 0; i < m_StrucCubes.Length; ++i)
						GameUtils.DeleteGameobject(m_StrucCubes[i]);
				}
				m_StrucCubes = new GameObject[m_StrucInfos.Count];
				int maxValue = 360;
				int colorOffset = maxValue / BiomeLoader.Biomes.Count;
				for (int i = 0; i < BiomeLoader.Biomes.Count; ++i)
				{
					var biome = BiomeLoader.Biomes[i];
					if (biome == null)
						continue;
					var name = biome.GetName();
					var fColor = Color.HSVToRGB((colorOffset * i) / 360f, 1f, 1f);
					var color = new Color32((byte)Mathf.RoundToInt(fColor.r * 255f),
						(byte)Mathf.RoundToInt(fColor.g * 255f),
						(byte)Mathf.RoundToInt(fColor.b * 255f),
						255);
					Debug.Log("Biome " + name + " color is 0x" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2"));
				}
				var cValue = Mathf.FloorToInt(maxValue / (float)m_StrucInfos.Count);
				var smoothnessID = Shader.PropertyToID("_Smoothness");
				for (int i = 0; i < m_StrucInfos.Count; ++i)
				{
					var struc = m_StrucInfos[i];
					if (struc == null)
						continue;

					var biomeIDXIE = struc.GetBiome().IDXIE;
					var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
					go.layer = Def.RCLayerBlock;
					go.name = "Struc_" + i.ToString();
					//var color = GameUtils.UnpackColor((uint)((UnityEngine.Random.Range(0, maxValue) << 8) + 0xFF));
					var fColor = Color.HSVToRGB((colorOffset * biomeIDXIE) / 360f, 1f, 1f);
					fColor.a = 1f;
					//var color = GameUtils.UnpackColor((uint)((colorOffset * biomeIDXIE) << 8) + 0xFF);
					m_StrucCubes[i] = go;
					var wPos = GameUtils.TransformPosition(struc.Position);
					go.transform.SetParent(transform);
					go.transform.position = new Vector3(wPos.x, struc.GetHeight(), wPos.y);
					go.transform.localScale = new Vector3(struc.GetSize().x * (1f + Def.BlockSeparation), 1f, struc.GetSize().y * (1f + Def.BlockSeparation));
					go.transform.Translate(new Vector3(struc.GetSize().x * 0.5f, 0f, struc.GetSize().y * 0.5f), Space.World);
					var rnd = go.GetComponent<MeshRenderer>();
					rnd.material = new Material(rnd.material)
					{
						color = fColor,
					};
					rnd.material.SetFloat(smoothnessID, 0f);
				}
			}

			gWorld = this;
			//UnityEngine.Profiling.Profiler.EndSample();
		}
		public void DebugContinue()
		{
			while (!DebugWideTick()) ;
		}
		//public void Generate(Vector2Int size, int seed, float frequency = 10f, float contrast = 0f, Vector2 offset = default)
		//{
		//	if (size.x % WStruc.Size != 0 || size.y % WStruc.Size != 0)
		//		throw new Exception("Trying to create a world with now power of 16 size!" + size.ToString());

		//	var now = DateTime.Now;
		//	m_Size = size;
		//	m_Pilars = new PilarInfo[m_Size.x * m_Size.y];

		//	//m_Seed = seed;
		//	//m_PerlinFrequency = frequency;
		//	//m_PerlinContrast = contrast;
		//	//m_PerlinOffset = offset;
		//	//GeneratePerlin();
		//	m_StrucInfos.Clear();
		//	PlaceStructures();
		//	var after = DateTime.Now;

		//	var diff = after - now;

		//	Debug.Log("Finished Loading map, " + m_StrucInfos.Count.ToString() + " structures in " + diff.ToString());

		//	if(m_StrucCubes != null)
		//	{
		//		for (int i = 0; i < m_StrucCubes.Length; ++i)
		//			GameUtils.DeleteGameobject(m_StrucCubes[i]);
		//	}
		//	m_StrucCubes = new GameObject[m_StrucInfos.Count];
		//	int maxValue = 0xFFFFFF;
		//	//var cValue = Mathf.FloorToInt(maxValue / (float)m_StrucInfos.Count);
		//	for(int i = 0; i < m_StrucInfos.Count; ++i)
		//	{
		//		var struc = m_StrucInfos[i];
		//		if (struc == null)
		//			continue;

		//		var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//		go.layer = Def.RCLayerBlock;
		//		go.name = "Struc_" + i.ToString();
		//		var color = GameUtils.UnpackColor((uint)((UnityEngine.Random.Range(0, maxValue) << 8) + 0xFF));
		//		m_StrucCubes[i] = go;
		//		var wPos = GameUtils.TransformPosition(struc.Position);
		//		go.transform.SetParent(transform);
		//		go.transform.position = new Vector3(wPos.x, 0f, wPos.y);
		//		go.transform.localScale = new Vector3(struc.GetSize().x * (1f + Def.BlockSeparation), 1f, struc.GetSize().y * (1f + Def.BlockSeparation));
		//		go.transform.Translate(new Vector3(struc.GetSize().x * 0.5f, 0f, struc.GetSize().y * 0.5f), Space.World);
		//		var rnd = go.GetComponent<MeshRenderer>();
		//		rnd.material = new Material(rnd.material)
		//		{
		//			color = color
		//		};
		//	}
		//}
		public List<StrucInfo> GetStrucInfos() => m_StrucInfos;
		public CStruc StrucFromStrucID(int strucID)
		{
			if (strucID < 0 || strucID >= m_Strucs.Length)
				return null;
			return m_Strucs[strucID];
		}
		public CStruc StrucFromVSPos(Vector2Int vsPos)
		{
			return StrucFromStrucID(StrucIDFromVSPos(vsPos));
		}
		public CStruc StrucFromVPos(Vector2Int vPos)
		{
			return StrucFromVSPos(VSPosFromVPos(vPos));
		}
		public CStruc StrucFromVWPos(Vector2Int vwPos)
		{
			return StrucFromVPos(VPosFromVWPos(vwPos));
		}
		public CStruc StrucFromWPos(Vector2 wPos)
		{
			return StrucFromVWPos(GameUtils.TransformPosition(wPos));
		}
		public void StrucsRangeVPos(Vector2Int vCenter, int radius, ref List<CStruc> strucs)
		{
			var fOffset = new Vector2(radius * InvHorzSize, radius * InvVertSize);
			var iOffset = new Vector2Int(Mathf.CeilToInt(fOffset.x), Mathf.CeilToInt(fOffset.y));
			var sPos = SPosFromVPos(vCenter);
			var centerStrucPos = new Vector2Int(Mathf.FloorToInt(sPos.x), Mathf.FloorToInt(sPos.y));
			var startPoint = centerStrucPos - iOffset;
			var finalPoint = centerStrucPos + iOffset;
			for(int y = startPoint.y; y < finalPoint.y; ++y)
			{
				var yMod = GameUtils.Mod(y, m_StrucWorldSize.y);
				//var yModPlus = GameUtils.Mod(y + 1, m_StrucWorldSize.y);
				for(int x = startPoint.x; x < finalPoint.x; ++x)
				{
					var xMod = GameUtils.Mod(x, m_StrucWorldSize.x);
					//var xModPlus = GameUtils.Mod(x + 1, m_StrucWorldSize.x);
					//var tlPos = new Vector2Int(xMod, yMod);
					//var trPos = new Vector2(xModPlus, yMod);
					//var blPos = new Vector2(xMod, yModPlus);
					//var brPos = new Vector2(xModPlus, yModPlus);

					var strucRect = new RectInt(new Vector2Int(x, y), new Vector2Int(1, 1));

					var relativeCenter = sPos - strucRect.center;
					var offsetFromCorner = new Vector2(Mathf.Abs(relativeCenter.x), Mathf.Abs(relativeCenter.y)) - new Vector2(0.5f, 0.5f);

					var separation = Mathf.Min(Mathf.Max(offsetFromCorner.x, offsetFromCorner.y), 0f) + new Vector2(Mathf.Max(offsetFromCorner.x, 0f), Mathf.Max(offsetFromCorner.y, 0f)).magnitude - fOffset.x;
					
					if(separation <= 0f)
					{
						var struc = m_Strucs[StrucIDFromVSPos(new Vector2Int(xMod, yMod))];
						if (!strucs.Contains(struc))
							strucs.Add(struc);
						continue;
					}

					//if(GameUtils.DistanceMod(sPos, tlPos, m_StrucWorldSize) <= fOffset.x)
					////if(Vector2.Distance(sPos, tlPos) <= fOffset.x)
					//{
					//	var struc = m_Strucs[StrucIDFromVSPos(new Vector2Int(Mathf.FloorToInt(tlPos.x), Mathf.FloorToInt(tlPos.y)))];
					//	if (!strucs.Contains(struc))
					//		strucs.Add(struc);
					//	continue;
					//}
					//if (GameUtils.DistanceMod(sPos, trPos, m_StrucWorldSize) <= fOffset.x)
					////if (Vector2.Distance(sPos, trPos) <= fOffset.x)
					//{
					//	var struc = m_Strucs[StrucIDFromVSPos(new Vector2Int(Mathf.FloorToInt(tlPos.x), Mathf.FloorToInt(tlPos.y)))];
					//	if (!strucs.Contains(struc))
					//		strucs.Add(struc);
					//	continue;
					//}
					//if (GameUtils.DistanceMod(sPos, blPos, m_StrucWorldSize) <= fOffset.x)
					////if (Vector2.Distance(sPos, blPos) <= fOffset.x)
					//{
					//	var struc = m_Strucs[StrucIDFromVSPos(new Vector2Int(Mathf.FloorToInt(tlPos.x), Mathf.FloorToInt(tlPos.y)))];
					//	if (!strucs.Contains(struc))
					//		strucs.Add(struc);
					//	continue;
					//}
					//if (GameUtils.DistanceMod(sPos, brPos, m_StrucWorldSize) <= fOffset.x)
					////if (Vector2.Distance(sPos, brPos) <= fOffset.x)
					//{
					//	var struc = m_Strucs[StrucIDFromVSPos(new Vector2Int(Mathf.FloorToInt(tlPos.x), Mathf.FloorToInt(tlPos.y)))];
					//	if (!strucs.Contains(struc))
					//		strucs.Add(struc);
					//	continue;
					//}
				}
			}
		}
		public void StrucsRangeVWPos(Vector2Int vwCenter, int radius, ref List<CStruc> strucs)
		{
			StrucsRangeVPos(VPosFromVWPos(vwCenter), radius, ref strucs);
		}
		public void StrucsRangeWPos(Vector2 center, float radius, ref List<CStruc> strucs)
		{
			StrucsRangeVWPos(GameUtils.TransformPosition(center), Mathf.FloorToInt(radius), ref strucs);
		}
		public CPilar GetPilarAtVPos(Vector2Int vPos)
		{
			var vsPos = VSPosFromVPos(vPos);
			var strucID = StrucIDFromVSPos(vsPos);
			if (strucID < 0 || strucID > m_Strucs.Length)
				return null;
			var struc = m_Strucs[strucID];
			if (struc == null)
				return null;
			var vStrucPos = new Vector2Int(vsPos.x * CStruc.Width, vsPos.y * CStruc.Height);
			var vPilarPos = vPos - vStrucPos;
			if (vPilarPos.x < 0 || vPilarPos.x >= CStruc.Width ||
				vPilarPos.y < 0 || vPilarPos.y >= CStruc.Height)
				return null;
			var id = struc.PilarIDFromVPos(vPilarPos);
			if (id < 0 || id >= struc.GetPilars().Length)
				return null;
			return struc.GetPilars()[id];
		}
		public CPilar GetPilarAtVWPos(Vector2Int vwPos)
		{
			return GetPilarAtVPos(VPosFromVWPos(vwPos));
		}
		public CPilar GetPilarAtWPos(Vector2 wPos)
		{
			return GetPilarAtVWPos(GameUtils.TransformPosition(wPos));
		}
		public CBlock GetBLockAtVPos(Vector2Int vPos, float height)
		{
			var pilar = GetPilarAtVPos(vPos);
			if (pilar == null || pilar.GetBlocks().Count == 0)
				return null;

			return pilar.GetClosestBlock(height) as CBlock;
		}
		public CBlock GetBlockAtVWPos(Vector2Int vwPos, float height)
		{
			return GetBLockAtVPos(VPosFromVWPos(vwPos), height);
		}
		public CBlock GetBlockAtWPos(Vector3 wPos)
		{
			return GetBlockAtVWPos(GameUtils.TransformPosition(new Vector2(wPos.x, wPos.z)), wPos.y);
		}
		public CStruc[] GetStrucs() => m_Strucs;
		public Vector2Int GetWorldSize() => m_Size;
		public Vector2Int GetWorldStrucSize() => m_StrucWorldSize;
		public PilarInfo[] GetPilars() => m_Pilars;
		private void OnDestroy()
		{
			if (m_Strucs != null)
			{
				for (int i = 0; i < m_Strucs.Length; ++i)
				{
					var struc = m_Strucs[i];
					if (struc != null)
					{
						struc.UnloadFromWorld();
						for (int j = 0; j < struc.GetLES().Count; ++j)
							GameUtils.DeleteGameobject(struc.GetLES()[j].gameObject);
						GameUtils.DeleteGameobject(struc.gameObject);
					}
				}
			}
			gWorld = null;
		}
		//public WStruc[] GetWorldStrucs() => m_Strucs;
	}
	
}
