/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.World
{
	public class StrucInfo
	{
		const int MaxStrucMods = 5;
		int m_IDXIE;
		List<BlockInfo> m_Blocks;
		PilarInfo[] m_Pilars;
		readonly LayerInfo[] m_Layers;
		Biome m_Biome;
		Vector2Int m_Size;
		Vector2Int m_Position;
		float m_Height;
		List<PilarInfo> m_WorldPilars;
		readonly World m_World;

		public StrucInfo(World world)
		{
			m_IDXIE = -1;
			m_World = world;
			m_Pilars = null;
			m_Blocks = null;
			m_Layers = new LayerInfo[Def.MaxLayerSlots];
			for (int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				var layer = LayerInfo.GetDefaultLayer();
				layer.Slot = i + 1;
				m_Layers[i] = layer;
			}
		}
		public PilarInfo[] GetPilars() => m_Pilars;
		public Vector2Int GetSize() => m_Size;
		public float GetHeight() => m_Height;
		public void SetHeight(float height) => m_Height = height;
		public int IDXIE { get => m_IDXIE; }
		public LayerInfo[] GetLayers() => m_Layers;
		public void SetBiome(Biome biome) => m_Biome = biome;
		public Biome GetBiome() => m_Biome;
		public Vector2Int Position { get => m_Position; }
		public List<PilarInfo> GetWorldPilars() => m_WorldPilars;
		public World GetWorld() => m_World;
		void ForeachBlock(Action<BlockInfo> fn, int start = 0, int end = int.MaxValue)
		{
			for(int i = start; i < end && i < m_Blocks.Count; ++i)
			{
				fn(m_Blocks[i]);
			}
		}
		void SetLinkedTo(BlockInfo bi, System.Random rng)
		{
			var layer = m_Layers[bi.Layer - 1];
			if (layer == null)
			{
				Debug.LogWarning("Trying to set a null Layer to a BlockInfo.");
				return;
			}
			if (layer.IsLinkedLayer)
			{
				int linkedID = layer.LinkedLayers[LayerInfo.RandomFromList(layer.LinkedLayers, rng)].ID;
				bi.LinkedTo = linkedID;
			}
			else
			{
				bi.LinkedTo = bi.Layer;
			}
		}
		void ApplyMaterial(BlockInfo bi, List<IDChance> idc, System.Random rng)
		{
			int famIdx = idc[LayerInfo.RandomFromList(idc, rng)].ID;
			bi.MatFamily = BlockMaterial.MaterialFamilies[famIdx];
			var set = bi.MatFamily.GetSet(bi.BlockType, bi.StairType);

			if (set == null || set.Length == 0)
			{
				var stonefamily = BlockMaterial.MaterialFamilies[BlockMaterial.FamilyDict["Stone"]];
				set = stonefamily.GetSet(bi.BlockType, bi.StairType);
			}

			var matIdx = rng.Next(0, set.Length);
			bi.MatSet = set[matIdx];
		}
		void RemoveUnsupportedMaterials(BlockInfo bi, List<IDChance> idc)
		{
			for(int i = 0; i < idc.Count;)
			{
				var fam = BlockMaterial.MaterialFamilies[idc[i].ID];
				if (fam.GetSet(bi.BlockType, bi.StairType).Length == 0)
				{
					idc.RemoveAt(i);
					continue;
				}
				++i;
			}
		}
		void AddElements(List<IDChance> idc, List<IDChance> nElements)
		{
			for(int i = 0; i < nElements.Count; ++i)
			{
				var element = nElements[i];

				bool found = false;
				for(int j = 0; j < idc.Count; ++j)
				{
					if(element.ID == idc[j].ID)
					{
						found = true;
						idc[j] = new IDChance()
						{
							ID = element.ID,
							Chance = (ushort)(idc[j].Chance + element.Chance)
						};

						break;
					}
				}
				if (!found)
					idc.Add(element);
			}
		}
		void SetMaterial(BlockInfo bi, IE.V4.StructureIE strucIE, List<IDChance> availableIDC, System.Random rng)
		{
			var blockIE = strucIE.GetBlocks()[bi.IDXIE];
			if (blockIE.GetFlag(IE.V4.BlockIE.Flag.MaterialFamily))
			{
				//bi.MatFamily = BlockMaterial.GetMaterialFamily(blockIE.MaterialFamily);
				return; // blocked
			}
			availableIDC.Clear();
			var layer = m_Layers[bi.LinkedTo - 1];
			if (layer.LayerType == Def.BiomeLayerType.OTHER)
			{
				availableIDC.AddRange(layer.MaterialFamilies);
			}
			else if (layer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
			{
				availableIDC.AddRange(m_Biome.GetLayers()[(int)layer.LayerType].MaterialFamilies);
			}

			RemoveUnsupportedMaterials(bi, availableIDC);

			if (availableIDC.Count == 0)
			{
				// Check other layers 
				for(int i = 0; i < m_Layers.Length; ++i)
				{
					if (i == (bi.LinkedTo - 1))
						continue;
					var oLayer = m_Layers[i];

					if(oLayer.LayerType == Def.BiomeLayerType.OTHER)
					{
						AddElements(availableIDC, oLayer.MaterialFamilies);
					}
					else if(oLayer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
					{
						AddElements(availableIDC, m_Biome.GetLayers()[(int)oLayer.LayerType].MaterialFamilies);
					}
				}
				
				RemoveUnsupportedMaterials(bi, availableIDC);


				if(availableIDC.Count == 0)
				{
					// get blue stone material
					var stoneID = BlockMaterial.FamilyDict["Stone"];
					availableIDC.Add(new IDChance()
					{
						ID = stoneID,
						Chance = (ushort)10000
					});
					ApplyMaterial(bi, availableIDC, rng);
				}
				else
				{
					GameUtils.UpdateChances2(ref availableIDC, rng);
					ApplyMaterial(bi, availableIDC, rng);
				}
			}
			else
			{
				GameUtils.UpdateChances2(ref availableIDC, rng);

				ApplyMaterial(bi, availableIDC, rng);
			}
		}
		void SetRotation(BlockInfo bi, IE.V4.StructureIE strucIE, System.Random rng)
		{
			var blockIE = strucIE.GetBlocks()[bi.IDXIE];
			if (blockIE.GetFlag(IE.V4.BlockIE.Flag.Rotation))
			{
				return; // blocked
			}
			var layer = m_Layers[bi.LinkedTo - 1];
			if (layer.Rotation == Def.RotationState.COUNT)
			{
				bi.Rotation = (Def.RotationState)rng.Next(Def.RotationStateCount); //UnityEngine.Random.Range(0, Def.RotationStateCount);
			}
			else
			{
				bi.Rotation = layer.Rotation;
			}
		}
		//void ApplyBlockHeight(BlockInfo bii, bool increase, float offset)
		//{
		//	bii.Height += offset;
		//	if (increase)
		//	{
		//		var above = bii.GetStackedAbove();
		//		if (above == null)
		//		{
		//			above = bii.GetBlockAbove(bii.Height);
		//			bii.StackedIdx[1] = above == null ? -1 : above.PilarIndex;
		//		}

		//		if (above != null)
		//			bii.ApplyAbove((BlockInfo biii) => ApplyBlockHeight(biii, increase, offset));
		//	}
		//	else
		//	{
		//		var below = bii.GetStackedBelow();
		//		if (below == null)
		//		{
		//			below = bii.GetBlockBelow(bii.Height - bii.Length);
		//			bii.StackedIdx[0] = below == null ? -1 : below.PilarIndex;
		//		}

		//		if (below != null)
		//			bii.ApplyBelow((BlockInfo biii) => ApplyBlockHeight(biii, increase, offset));
		//	}
		//	bii.SetUpdatedProperty(Def.BlockProperty.Height, m_UpdateID);
		//}
		//void SetBlockHeight(BlockInfo bi, IE.V4.StructureIE strucIE)
		//{
		//	if (bi.GetUpdatedProperty(Def.BlockProperty.Height) >= m_UpdateID)
		//		return; // Already done

		//	//Debug.Log("SetBlockHeight, S" + strucIE.StructureID.ToString() + " B" + m_IDXIE.ToString());

		//	var blockIE = strucIE.GetBlocks()[bi.IDXIE];
		//	bi.Height = blockIE.GetHeight();
		//	//float height;

		//	//if (blockIE.GetFlag(IE.V4.BlockIE.Flag.Height))
		//	//{
		//	//	height = blockIE.Height;
		//	//}
		//	//else
		//	//{
		//	//	var layer = m_Layers[bi.LinkedTo - 1];
		//	//	height = layer.BlockHeight;
		//	//}

		//	//if (height == bi.Height)
		//	//{
		//	//	bi.SetUpdatedProperty(Def.BlockProperty.Height, m_UpdateID);
		//	//	return;
		//	//}

		//	//if (bi.Pilar.GetBlocks().IndexOf(bi) != 0)
		//	//{
		//	//	bi.SetUpdatedProperty(Def.BlockProperty.Height, m_UpdateID);
		//	//	return;
		//	//}

		//	//var diff = height - bi.Height;
		//	//int times = Mathf.FloorToInt(diff * 2f);
		//	//bool increase = times >= 0;
		//	////times = Mathf.Abs(times);
		//	//Action fn;
		//	//if(increase)
		//	//{
		//	//	fn = () => bi.IncreaseHeight();
		//	//}
		//	//else
		//	//{
		//	//	times = -times;
		//	//	fn = () => bi.DecreaseHeight();
		//	//}
		//	//for (int i = 0; i < times; ++i)
		//	//{
		//	//	fn();
		//	//	//ApplyBlockHeight(bi, increase, 0.5f * mult);
		//	//}
		//}
		//void ApplyBlockLength(BlockInfo bii, bool increase, float offset)
		//{
		//	if (increase)
		//	{
		//		if (bii.Length == 3f)
		//			offset = 0.4f;
		//		bii.Length += offset;
		//		bii.IncreaseLengthCheck();
		//	}
		//	else
		//	{
		//		if (bii.Length == 3.4f)
		//			offset = -0.4f;
		//		bii.Length += offset;
		//		bii.DecreseLengthCheck();
		//	}
		//}
		void SetBlockLength(BlockInfo bi, IE.V4.StructureIE strucIE, System.Random rng)
		{
			var blockIE = strucIE.GetBlocks()[bi.IDXIE];
			float length;
			if (blockIE.GetFlag(IE.V4.BlockIE.Flag.Length))
			{
				return;
			}
			else
			{
				var layer = m_Layers[bi.LinkedTo - 1];
				var minLen = layer.BlockLengthMin == 3.4f ? 3.5f : layer.BlockLengthMin;
				var maxLen = layer.BlockLengthMax == 3.4f ? 3.5f : layer.BlockLengthMax;
				var rngValue = rng.Next(Mathf.FloorToInt(minLen * 2f), Mathf.FloorToInt(maxLen * 2f) + 1);

				length = rngValue * 0.5f;
				if (length == 3.5f)
					length = 3.4f;
			}

			if (bi.Length == length)
				return;

			var diff = length - bi.Length;
			int times = Mathf.FloorToInt(diff * 2f);
			bool increase = times >= 0;
			Action fn;
			if(increase)
			{
				fn = () => bi.IncreaseLength();
			}
			else
			{
				times = -times;
				fn = () => bi.DecreaseLength();
			}
			for (int i = 0; i < times; ++i)
			{
				fn();
			}
			var ieHeight = blockIE.GetHeight();
			while (bi.Height > ieHeight)
				bi.DecreaseHeight();
			while (bi.Height < ieHeight)
				bi.IncreaseHeight();
		}
		void ApplySemiVoid(System.Random rng)
		{
			var fVoidPilars = new List<PilarInfo>();
			for(int i = 0; i < m_Blocks.Count;)
			{
				var bi = m_Blocks[i];
				switch (bi.VoidState)
				{
					case Def.BlockVoid.SEMIVOID:
						{
							ushort svChance = 0;
							var layer = m_Layers[bi.LinkedTo - 1];
							if (layer.LayerType == Def.BiomeLayerType.OTHER)
							{
								svChance = layer.SemiVoidChance;
							}
							else if (layer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
							{
								svChance = m_Biome.GetLayers()[(int)layer.LayerType].SemiVoidChance;
							}
							var svRNG = rng.Next(0, 10000);
							if (svRNG <= svChance)
							{
								bi.Pilar.RemoveBlock(bi);
								m_Blocks.RemoveAt(i);
								continue;
							}
						}
						break;
					case Def.BlockVoid.FULLVOID:
						{
							fVoidPilars.Add(bi.Pilar);
						}
						break;
				}
				++i;
			}
			for(int i = 0; i < fVoidPilars.Count; ++i)
			{
				var pilar = fVoidPilars[i];
				for(int j = 0; j < pilar.GetBlocks().Count; ++j)
				{
					var bIdx = m_Blocks.IndexOf(pilar.GetBlocks()[j]);
					if (bIdx >= 0)
						m_Blocks.RemoveAt(bIdx);
				}
				pilar.GetBlocks().Clear();
				lock (m_World.PilarsLock)
				{
					if (m_World.GetPilars()[pilar.GetWorldID()] == null)
					{
						m_World.GetPilars()[pilar.GetWorldID()] = pilar;
					}
				}
			}
		}
		void ComputeStair(BlockInfo bi, System.Random rng)
		{
			if (bi.StairState == Def.StairState.ALWAYS || bi.StairState == Def.StairState.NONE || bi.StairState == Def.StairState.RAMP_ALWAYS)
			{
				return;
			}

			ushort stairChance = 0, rampChance = 0;
			var layer = m_Layers[bi.LinkedTo - 1];
			if(layer.LayerType == Def.BiomeLayerType.OTHER)
			{
				stairChance = layer.StairBlockChance;
				rampChance = layer.RampBlockChance;
			}
			else if(layer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
			{
				var biomeLayer = m_Biome.GetLayers()[(int)layer.LayerType];
				stairChance = biomeLayer.StairBlockChance;
				rampChance = biomeLayer.RampBlockChance;
			}
			
			ushort rampRNG;
			var stairRNG = (ushort)rng.Next(0, 10000);
			if (stairRNG <= stairChance)
			{
				switch (bi.StairState)
				{
					case Def.StairState.POSSIBLE:
						bi.BlockType = Def.BlockType.STAIRS;
						bi.StairType = Def.StairType.NORMAL;
						break;
					case Def.StairState.STAIR_OR_RAMP:
						bi.BlockType = Def.BlockType.STAIRS;
						rampRNG = (ushort)rng.Next(0, 10000);
						if (rampRNG <= rampChance)
							bi.StairType = Def.StairType.NORMAL;
						else
							bi.StairType = Def.StairType.RAMP;
						break;
					case Def.StairState.RAMP_POSSIBLE:
						bi.BlockType = Def.BlockType.STAIRS;
						bi.StairType = Def.StairType.RAMP;
						break;
				}
			}
		}
		void SetProps(BlockInfo bi, System.Random rng)
		{
			if (bi.IsStackLinkValid(1) || bi.BlockType == Def.BlockType.STAIRS)
				return;

			var layer = m_Layers[bi.LinkedTo - 1];
			ushort propChance = 0;
			float safetyDistance = 0f;
			List<IDChance> propFamilies = null;
			if (layer.LayerType == Def.BiomeLayerType.OTHER)
			{
				propChance = layer.PropGeneralChance;
				safetyDistance = layer.PropSafetyDistance;
				propFamilies = layer.PropFamilies;
			}
			else if (layer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
			{
				var biomeLayer = m_Biome.GetLayers()[(int)layer.LayerType];
				propChance = biomeLayer.PropGeneralChance;
				safetyDistance = biomeLayer.PropSafetyDistance;
				propFamilies = biomeLayer.PropFamilies;
			}
			if (propChance == 0 || propFamilies == null || propFamilies.Count == 0)
				return;
			
			bool spawnProp = true;
			if(safetyDistance > 0f)
			{
				var bpid = bi.Pilar.GetStructureID();
				var bpos = GameUtils.PosFromIDUnsafe(bpid, m_Size.x, m_Size.y);
				for(int i = 0; i < m_Blocks.Count; ++i)
				{
					var block = m_Blocks[i];
					if (block == bi || block.LinkedTo != bi.LinkedTo || block.PropFamilyID < 0)
						continue;

					var pid = block.Pilar.GetStructureID();
					var pos = GameUtils.PosFromIDUnsafe(pid, m_Size.x, m_Size.y);
					if(Vector2.Distance(bpos, pos) <= safetyDistance)
					{
						if (Mathf.Abs(bi.Height - block.Height) > safetyDistance)
							continue;
						spawnProp = false;
						break;
					}
				}
			}

			if (!spawnProp)
				return;

			var chance = (ushort)rng.Next(0, 10000);
			if (chance > propChance)
				return;

			int rnd = LayerInfo.RandomFromList(propFamilies, rng);
			int familyID = propFamilies[rnd].ID;
			var pfamily = Props.PropFamilies[familyID];
			if (pfamily.Props.Count == 0)
				return;
			bi.PropFamilyID = familyID;
		}
		public void ApplyLayers(System.Random rng)
		{
			var strucIE = Structures.Strucs[IDXIE];
			void Run(Action<BlockInfo> fn)
			{
				int start = rng.Next(1, m_Blocks.Count);//  UnityEngine.Random.Range(1, m_Blocks.Length);
				ForeachBlock(fn, start);
				ForeachBlock(fn, 0, start);
			}

			// Set linkedTo
			Run((BlockInfo bi) => SetLinkedTo(bi, rng));

			// Set Rotation
			Run((BlockInfo bi) => SetRotation(bi, strucIE, rng));

			// Block Height
			//Run((BlockInfo bi) => SetBlockHeight(bi, strucIE));

			// Block length
			Run((BlockInfo bi) => SetBlockLength(bi, strucIE, rng));

			// SemiVoid
			ApplySemiVoid(rng);

			// Stairs
			Run((BlockInfo bi) => ComputeStair(bi, rng));

			var availableIDC = new List<IDChance>();
			// Set Material
			Run((BlockInfo bi) => SetMaterial(bi, strucIE, availableIDC, rng));

			Run((BlockInfo bi) => SetProps(bi, rng));
		}
		int ApplyStrucMods(int id, List<Def.StrucMod> strucMods, PilarInfo pi, IE.V4.StructureIE strucIE, List<BlockInfo> toModify)
		{
			toModify.Clear();
			for(int i = 0; i < pi.GetBlocks().Count; ++i)
			{
				var bi = pi.GetBlocks()[i];
				var bie = strucIE.GetBlocks()[bi.IDXIE];
				if (bie.GetFlag(IE.V4.BlockIE.Flag.Rotation))
					toModify.Add(bi);
			}
			for(int i = 0; i < strucMods.Count; ++i)
			{
				var pos = GameUtils.PosFromIDUnsafe(id, Def.MaxStrucSide, Def.MaxStrucSide);
				switch (strucMods[i])
				{
					case Def.StrucMod.HorzFlip:
						{
							pos = new Vector2Int((Def.MaxStrucSide - 1) - pos.x, pos.y);
							for(int j = 0; j < toModify.Count; ++j)
							{
								var bi = toModify[j];
								switch (bi.Rotation)
								{
									case Def.RotationState.Default:
										bi.Rotation = Def.RotationState.Half;
										break;
									case Def.RotationState.Half:
										bi.Rotation = Def.RotationState.Default;
										break;
								}
							}
						}
						break;
					case Def.StrucMod.VertFlip:
						{
							pos = new Vector2Int(pos.x, (Def.MaxStrucSide - 1) - pos.y);
							for (int j = 0; j < toModify.Count; ++j)
							{
								var bi = toModify[j];
								switch (bi.Rotation)
								{
									case Def.RotationState.Right:
										bi.Rotation = Def.RotationState.Left;
										break;
									case Def.RotationState.Left:
										bi.Rotation = Def.RotationState.Right;
										break;
								}
							}
						}
						break;
					case Def.StrucMod.Rotated90:
						{
							pos = new Vector2Int((Def.MaxStrucSide - 1) - pos.y, pos.x);
							for (int j = 0; j < toModify.Count; ++j)
							{
								var bi = toModify[j];
								switch (bi.Rotation)
								{
									case Def.RotationState.Default:
										bi.Rotation = Def.RotationState.Right;
										break;
									case Def.RotationState.Right:
										bi.Rotation = Def.RotationState.Half;
										break;
									case Def.RotationState.Half:
										bi.Rotation = Def.RotationState.Left;
										break;
									case Def.RotationState.Left:
										bi.Rotation = Def.RotationState.Default;
										break;
								}
							}
						}
						break;
				}
				id = GameUtils.IDFromPosUnsafe(pos, Def.MaxStrucSide, Def.MaxStrucSide);
			}
			return id;
		}
		public void LoadStruc(int id, Vector2Int position, System.Random rng)
		{
			if (id < 0 || Structures.Strucs.Count <= id)
				return;

			var strucIE = Structures.Strucs[id];
			if (strucIE == null)
				return;

			m_Position = position;
			m_IDXIE = id;
			var width = strucIE.GetWidth();
			var height = strucIE.GetHeight();
			var ieBlocks = strucIE.GetBlocks();

			m_Blocks = new List<BlockInfo>(ieBlocks.Length); // new BlockInfo[ieBlocks.Length];
			var dict = new Dictionary<int, PilarInfo>(width * height);

			Vector2Int topLeft = new Vector2Int(int.MaxValue, int.MaxValue),
				botRight = new Vector2Int(int.MinValue, int.MinValue);

			BlockInfo FindBLock(PilarInfo pilar, int idxie)
			{
				for (int i = 0; i < pilar.GetBlocks().Count; ++i)
				{
					var block = pilar.GetBlocks()[i];
					if (block.IDXIE == idxie)
						return block;
				}
				return null;
			}

			for (int i = 0; i < ieBlocks.Length; ++i)
			{
				var blockIE = ieBlocks[i];
				if (blockIE == null)
					continue;

				var strucID = (int)blockIE.StructureID;
				if (strucID > Def.MaxBlockID)
				{
					Debug.LogWarning($"Trying to load a StrucInfo which has a block {i} with an invalid StructureID{strucID}.");
					continue;
				}

				//bool firstBlock = true;
				PilarInfo pi;
				if (dict.ContainsKey(strucID))
				{
					pi = dict[strucID];
					//firstBlock = false;
				}
				else
				{
					pi = new PilarInfo(new List<BlockInfo>(), this, strucID);
					dict.Add(strucID, pi);
					var pos = GameUtils.PosFromID(strucID, Def.MaxStrucSide, Def.MaxStrucSide);
					if (pos.x < topLeft.x)
						topLeft.x = pos.x;
					if (pos.x > botRight.x)
						botRight.x = pos.x;
					if (pos.y < topLeft.y)
						topLeft.y = pos.y;
					if (pos.y > botRight.y)
						botRight.y = pos.y;
				}
				var bi = new BlockInfo
				{
					PilarIndex = pi.GetBlocks().Count,
					Pilar = pi,
					IDXIE = i,
					Layer = blockIE.Layer,
					Height = 0f,
					Length = 0.5f,
					MicroHeight = 0f,
					LinkedTo = blockIE.Layer,
					Rotation = (Def.RotationState)rng.Next(Def.RotationStateCount),
					StackedIdx = new int[2] { -1, -1 },
					StairState = blockIE.Stair,
					StairType = blockIE.Stair == Def.StairState.RAMP_ALWAYS ? Def.StairType.RAMP : Def.StairType.NORMAL,
				};

				pi.GetBlocks().Add(bi);
				m_Blocks.Add(bi);
				//m_Blocks[BlockID++] = bi;

				//float stairOffset = 0f;
				
				bi.BlockType = blockIE.BlockType;
				//if (bi.BlockType == Def.BlockType.STAIRS)
				//	stairOffset = 0.5f;

				bi.Height = blockIE.GetHeight();
				
				if (blockIE.GetFlag(IE.V4.BlockIE.Flag.Length))
				{
					bi.Length = blockIE.GetLength();
				}
				if (blockIE.GetFlag(IE.V4.BlockIE.Flag.MaterialFamily))
				{
					bi.MatFamily = BlockMaterial.GetMaterialFamily(blockIE.MaterialFamily);
				}
				if (blockIE.GetFlag(IE.V4.BlockIE.Flag.Rotation))
				{
					bi.Rotation = blockIE.Rotation;
				}
				bi.VoidState = blockIE.BlockVoid;

				if (blockIE.StackedIdx[0] >= 0)
				{
					var below = FindBLock(pi, blockIE.StackedIdx[0]);
					if (below != null)
					{
						bi.StackedIdx[0] = below.PilarIndex;
						below.StackedIdx[1] = bi.PilarIndex;
						var belowHeight = below.Height;
						if (below.BlockType == Def.BlockType.STAIRS)
							belowHeight += 0.5f;
						bi.Height = belowHeight + bi.Length;
					}
				}
				if (blockIE.StackedIdx[1] >= 0)
				{
					var above = FindBLock(pi, blockIE.StackedIdx[1]);
					if (above != null)
					{
						bi.StackedIdx[1] = above.PilarIndex;
						above.StackedIdx[0] = bi.PilarIndex;
						var h = bi.Height;
						if (bi.BlockType == Def.BlockType.STAIRS)
							h += 0.5f;
						above.Height = h + above.Length;
					}
				}

				//if (!firstBlock) // check stacking
				//{
				//	var above = bi.GetBlockAbove(bi.Height + stairOffset);
				//	var below = bi.GetBlockBelow(bi.Height - bi.Length);
				//	if (below == null)
				//		bi.StackedIdx[0] = -1;
				//	else
				//		bi.StackedIdx[0] = below.PilarIndex;

				//	if (above == null)
				//		bi.StackedIdx[1] = -1;
				//	else
				//		bi.StackedIdx[1] = above.PilarIndex;
				//}
			}

			var strucModCount = rng.Next(0, MaxStrucMods + 1);
			var strucMods = new List<Def.StrucMod>(strucModCount);
			for (int i = 0; i < strucModCount; ++i)
			{
				strucMods.Add((Def.StrucMod)rng.Next(0, Def.StrucModCount));
				//Debug.Log(strucMods[i].ToString());
			}

			m_Size = new Vector2Int(botRight.x - topLeft.x + 1, botRight.y - topLeft.y + 1);
			m_Pilars = new PilarInfo[m_Size.x * m_Size.y];
			m_WorldPilars = new List<PilarInfo>(m_Pilars.Length);
			var toModifyBlocks = new List<BlockInfo>();
			for (int i = 0; i < dict.Count; ++i)
			{
				var pair = dict.ElementAt(i);
				var sID = ApplyStrucMods(pair.Key, strucMods, pair.Value, strucIE, toModifyBlocks);
				var pos = GameUtils.PosFromID(sID, Def.MaxStrucSide, Def.MaxStrucSide);
				var nPos = pos - topLeft;
				var nID = GameUtils.IDFromPos(nPos, m_Size.x, m_Size.y);
				m_Pilars[nID] = pair.Value;
				pair.Value._SetStructureID(nID);
				var wPos = m_Position + nPos; // world position;
				var wID = GameUtils.IDFromPos(wPos, m_World.GetWorldSize().x, m_World.GetWorldSize().y);
//				pair.Value._SetWorldID(wID);
				lock(m_World.PilarsLock)
				{
					if(m_World.GetPilars()[wID] == null)
					{
						m_World.GetPilars()[wID] = pair.Value;
						m_WorldPilars.Add(pair.Value);
						pair.Value._SetWorldID(wID);
					}
				}
			}

			var ieLayers = strucIE.GetLayers();
			for (int i = 0; i < ieLayers.Length; ++i)
			{
				var layer = ieLayers[i];
				if (layer == null)
					continue;
				m_Layers[i] = layer.ToLayerInfo();
			}

			ApplyLayers(rng);
		}
	}
}
