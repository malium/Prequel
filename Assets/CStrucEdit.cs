/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
	public class CStrucEdit : IStruc
	{
		[SerializeField] RectInt m_Bounds;
		[SerializeField] int m_IDXIE;
		[SerializeField] CPilar[] m_Pilars;
		[SerializeField] CPilar[] m_DuplicatedPilars;
		List<Def.StrucMod> m_Mods;
		LayerInfo[] m_Layers;
		[SerializeField] List<CBlockEdit>[] m_LayerBlocks;
		//List<LivingEntity> m_LivingEntities;
		[SerializeField] List<AI.CLivingEntity> m_LES;
		[SerializeField] World.Biome m_Biome;
		//StrucQT m_QT;

		List<CPilar> m_WideConvertedPilars;
		List<CPilar> m_WidePreviousPilars;
		List<CBlockEdit> m_WideBlocks;
		Dictionary<int, List<bool>> m_FlagsToRestore;
		//struct VoidInfo
		//{
		//	public CBlockEdit Block;
		//	public int PilarIdx;
		//}
		List<CBlockEdit> m_VoidBlocks;
		struct WideSlice
		{
			public bool Possible;
			public CBlockEdit[] Blocks;
		}
		void OnEntityDeath(AI.CLivingEntity entity)
		{
			if(m_LES.Contains(entity))
			{
				m_LES.Remove(entity);
			}
			GameUtils.DeleteGameobject(entity.gameObject);
		}
		public RectInt Bounds { get => m_Bounds; set => m_Bounds = value; }
		public int IDXIE { get => m_IDXIE; set => m_IDXIE = value; }
		public LayerInfo[] GetLayers() => m_Layers;
		public List<CBlockEdit> GetLayerBlocks(int layer) => m_LayerBlocks[layer - 1];
		public void SetBiome(World.Biome biome)
		{
			if (m_Biome == biome)
				return;
			m_Biome = biome;
		}
		public World.Biome GetBiome() => m_Biome;
		public void ReapplyLayers()
		{
			for(int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				if (!m_Layers[i].IsValid())
					continue;

				var li = m_Layers[i];
				var blocks = m_LayerBlocks[li.Slot - 1];
				var splitIdx = UnityEngine.Random.Range(0, blocks.Count);

				for (int j = 0; j < blocks.Count; ++j)
				{
					var block = blocks[j];
					var prop = block.GetProp();
					var monster = block.GetMonster();
					if (prop != null)
					{
						//if (m_LivingEntities.Contains(prop))
						//	m_LivingEntities.Remove(prop);
						//prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, prop.Prop.BaseHealth);
						if (m_LES.Contains(prop.GetLE()))
							m_LES.Remove(prop.GetLE());
						prop.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, prop.GetLE().GetCurrentHealth());
						block.SetProp(null);
					}
					if (monster != null)
					{
						//if (m_LivingEntities.Contains(monster))
						//	m_LivingEntities.Remove(monster);
						//monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, monster.Info.BaseHealth);
						if (m_LES.Contains(monster.GetLE()))
							m_LES.Remove(monster.GetLE());
						monster.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, monster.GetLE().GetCurrentHealth());
						block.SetMonster(null);
					}
				}

				for (int j = 0; j < splitIdx; ++j)
					ApplyLayer(blocks[j], false);

				for (int j = splitIdx; j < blocks.Count; ++j)
					ApplyLayer(blocks[j], false);
			}
		}
		//public override StrucQT GetQT() => m_QT;
		//public override void SetQT(StrucQT qt) => m_QT = qt;
		public void SetLayer(LayerInfo li, bool ignoreLock = false)
		{
			if(li == null || li.Slot == 0 || li.Slot > Def.MaxLayerSlots)
			{
				Debug.LogWarning("Trying to set a layer with an invalid slot.");
				return;
			}
			m_Layers[li.Slot - 1] = li;
			if (!li.IsValid())
			{
				while(m_LayerBlocks[li.Slot - 1].Count > 0)
				{
					var block = m_LayerBlocks[li.Slot - 1].First();
					var idxie = block.GetIDXIE();
					if (block.GetPilar().GetBlocks().Count > 1)
						block.DestroyBlock(false);
					else
						block.SetLayer(0);
					var blockIE = Structures.Strucs[IDXIE].GetBlocks()[idxie];
					Structures.Strucs[IDXIE].RemoveBlock(blockIE);
					block.SetIDXIE(-1);
				}
				return;
			}

			var blocks = m_LayerBlocks[li.Slot - 1];
			var splitIdx = UnityEngine.Random.Range(0, blocks.Count);

			for(int i = 0; i < blocks.Count; ++i)
			{
				var block = blocks[i];
				var prop = block.GetProp();
				var monster = block.GetMonster();
				if (prop != null)
				{
					//if (m_LivingEntities.Contains(prop))
					//	m_LivingEntities.Remove(prop);
					//prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, prop.Prop.BaseHealth);
					if (m_LES.Contains(prop.GetLE()))
						m_LES.Remove(prop.GetLE());
					prop.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, prop.GetLE().GetCurrentHealth());
					block.SetProp(null);
				}
				if (monster != null)
				{
					//if (m_LivingEntities.Contains(monster))
					//	m_LivingEntities.Remove(monster);
					//monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, monster.Info.BaseHealth);
					if (m_LES.Contains(monster.GetLE()))
						m_LES.Remove(monster.GetLE());
					monster.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, monster.GetLE().GetCurrentHealth());
					block.SetMonster(null);
				}
			}

			for (int i = 0; i < splitIdx; ++i)
				ApplyLayer(blocks[i], ignoreLock);

			for(int i = splitIdx; i < blocks.Count; ++i)
				ApplyLayer(blocks[i], ignoreLock);
		}
		void ApplyLayer(CBlockEdit block, bool ignoreLock = false)
		{
			if(block.GetLayer() == 0 || block.GetLayer() > Def.MaxLayerSlots)
			{
				Debug.LogWarning("Trying to apply an invalid layer.");
				return;
			}

			if (block.GetLockState() == Def.LockState.Locked && !ignoreLock)
				return;

			var li = m_Layers[block.GetLayer() - 1];
			if(li.IsLinkedLayer)
			{
				int nLIIdx = li.LinkedLayers[LayerInfo.RandomFromList(li.LinkedLayers)].ID;
				li = m_Layers[nLIIdx - 1];
				block.SetLinkedTo(nLIIdx);
			}

			List<bool> flags;
			if(block.GetIDXIE() >= 0 && !ignoreLock)
			{
				var strucIE = Structures.Strucs[IDXIE];
				var blocks = strucIE.GetBlocks();
				var bie = blocks[block.GetIDXIE()];
				//if (bie == null)
				//	throw new Exception();
				flags = bie.Flags;
			}
			else
			{
				flags = new List<bool>(Enumerable.Repeat(false, IE.V3.BlockIE.FlagCount));
			}
			// Material assign
			if (!flags[(int)IE.V4.BlockIE.Flag.MaterialFamily])
			{
				List<IDChance> availableFamilies = null;
				if (li.LayerType == Def.BiomeLayerType.OTHER)
				{
					availableFamilies = new List<IDChance>(li.MaterialFamilies.Count);
				}
				else if (li.LayerType != Def.BiomeLayerType.FULLVOID)
				{
					if (m_Biome != null)
						availableFamilies = new List<IDChance>(m_Biome.GetLayers()[(int)li.LayerType].MaterialFamilies);
				}
				if (availableFamilies != null && availableFamilies.Count == 0 && li.LayerType != Def.BiomeLayerType.OTHER)
					availableFamilies = null;
				if (availableFamilies == null)
				{
					block.SetMaterialFamily(null);
				}
				else
				{
					for (int j = 0; j < li.MaterialFamilies.Count; ++j)
					{
						var fam = BlockMaterial.MaterialFamilies[li.MaterialFamilies[j].ID];
						if (fam.GetSet(block.GetBlockType()).Length > 0)
						{
							availableFamilies.Add(li.MaterialFamilies[j]);
						}
					}
					if (availableFamilies.Count > 0)
					{
						GameUtils.UpdateChances2(ref availableFamilies);

						int famIdx = availableFamilies[LayerInfo.RandomFromList(availableFamilies)].ID;
						block.SetMaterialFamily(BlockMaterial.MaterialFamilies[famIdx]);
					}
				}
			}
			// Rotation
			if (!flags[(int)IE.V4.BlockIE.Flag.Rotation])
			{
				if (li.Rotation == Def.RotationState.COUNT)
				{
					int rot = UnityEngine.Random.Range(0, Def.RotationStateCount);
					block.SetRotation((Def.RotationState)rot);
				}
				else
				{
					block.SetRotation(li.Rotation);
				}
			}
			// Block height
			//if(!flags[(int)IE.V3.BlockIE.Flag.Height])
			//{
			//	var pilar = block.GetPilar();
			//	if (pilar.GetBlocks().IndexOf(block) == 0)
			//	{
			//		var diff = li.BlockHeight - block.GetHeight();
			//		bool increase = diff >= 0f;
			//		Action fn = null;
			//		if (increase)
			//		{
			//			fn = () => block.IncreaseHeight();
			//		}
			//		else
			//		{
			//			diff = -diff;
			//			fn = () => block.DecreaseHeight();
			//		}
			//		for (float step = 0f; step < diff; step += 0.5f)
			//		{
			//			fn();
			//			//block.SetHeight(block.GetHeight() + 0.5f * mult);
			//			//if (increase)
			//			//	block.IncreaseHeightCheck();
			//			//else
			//			//	block.DecreseHeightCheck();
			//		}
			//	}
			//}
			//// microheight
			////if (block.GetStackedBlocks()[0] == null)
			//if(!block.IsStackLinkValid(0))
			//{
			//	float microHeightMin;
			//	float microHeightMax;
			//	if(li.LayerType == Def.BiomeLayerType.OTHER)
			//	{
			//		microHeightMin = li.MicroHeightMin;
			//		microHeightMax = li.MicroHeightMax;
			//	}
			//	else if(li.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
			//	{
			//		microHeightMin = 0f;
			//		microHeightMax = 0f;
			//	}
			//	else
			//	{
			//		microHeightMin = m_Biome.GetLayers()[(int)li.LayerType].MicroHeightMin;
			//		microHeightMax = m_Biome.GetLayers()[(int)li.LayerType].MicroHeightMax;
			//	}
			//	float nmHeight;
			//	if(microHeightMin != microHeightMax)
			//	{
			//		var rngVal = UnityEngine.Random.Range(Mathf.FloorToInt(microHeightMin / 0.05f),
			//			Mathf.FloorToInt(microHeightMax / 0.05f) + 1);
			//		nmHeight = rngVal * 0.05f;
			//	}
			//	else
			//	{
			//		nmHeight = microHeightMin;
			//	}
			//	if (nmHeight != block.GetMicroHeight())
			//		block.ChangeMicroheight(nmHeight);
			//}
			// Block length
			if (!flags[(int)IE.V4.BlockIE.Flag.Length])
			{
				float nLen;
				var curLen = block.GetLength() == 3.4f ? 3.5f : block.GetLength();
				if (li.RandomBlockLengthEnabled)
				{
					var minLen = li.BlockLengthMin == 3.4f ? 3.5f : li.BlockLengthMin;
					var maxLen = li.BlockLengthMax == 3.4f ? 3.5f : li.BlockLengthMax;
					var rngVal = UnityEngine.Random.Range(Mathf.FloorToInt(minLen * 2f), Mathf.FloorToInt(maxLen * 2f)+1);

					nLen = rngVal * 0.5f;
					//if (nLen == 3.5f)
					//	nLen = 3.4f;
					//float chance = UnityEngine.Random.value * (li.BlockLengthMax - li.BlockLengthMin) + li.BlockLengthMin;
					//if (chance > 3.0f)
					//{
					//	chance = 3.5f;
					//}
					//else if (chance < 0.66f)
					//{
					//	chance = 0.5f;
					//}
					//else
					//{
					//	// integer part
					//	float ichance = Mathf.Floor(chance);
					//	// float part
					//	float fchance = chance - ichance;
					//	if (fchance >= 0.66f)
					//		chance = ichance + 1f;
					//	else if (fchance < 0.66f && fchance > 0.33f)
					//		chance = ichance + 0.5f;
					//	else if (fchance < 0.33f)
					//		chance = ichance;
					//}

					//nLen = chance;
				}
				else
				{
					nLen = li.BlockLengthMin;
				}
				if (nLen > 0f && nLen != curLen)
				{
					var diff = nLen - curLen;
					bool increase = diff >= 0f;
					Action fn;
					if (increase)
					{
						fn = () => block.IncreaseLength();
					}
					else
					{
						diff = -diff;
						fn = () => block.DecreaseLength();
					}

					for (float step = 0f; step < diff; step += 0.5f)
					{
						fn();
						//float amount = 0.5f;
						//if ((block.GetLength() == 3.4f && !increase) || (block.GetLength() == 3f && increase))
						//	amount = 0.4f;
						//block.SetLength(block.GetLength() + amount * mult);
						//if (increase)
						//	block.IncreaseLengthCheck();
						//else
						//	block.DecreseLengthCheck();
					}
				}
			}
			// Monsters
			//{
			//	if (block.GetMonster() != null)
			//	{
			//		block.GetMonster().GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, block.GetMonster().GetLE().GetCurrentHealth());
			//		block.SetMonster(null);
			//	}
			//	List<IDChance> monFams;
			//	ushort mchance;
			//	if (li.LayerType == Def.BiomeLayerType.OTHER)
			//	{
			//		monFams = li.MonsterFamilies;
			//		mchance = li.MonsterGeneralChance;
			//	}
			//	else if (li.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
			//	{
			//		monFams = null;
			//		mchance = 0;
			//	}
			//	else
			//	{
			//		var blayer = m_Biome.GetLayers()[(int)li.LayerType];
			//		monFams = blayer.MonsterFamilies;
			//		mchance = blayer.MonsterGeneralChance;
			//	}

			//	if (monFams != null && monFams.Count > 0 && mchance > 0 && block.GetBlockType() != Def.BlockType.STAIRS)
			//	{
			//		if (UnityEngine.Random.Range(0, 10000) <= mchance)
			//		{
			//			int mid = monFams[LayerInfo.RandomFromList(monFams)].ID;
			//			var mon = new GameObject().AddComponent<AI.CMonster>();
			//			mon.SetMonster(Monsters.MonsterFamilies[mid]);
			//			mon.transform.position = block.GetPilar().transform.position +
			//				new Vector3(UnityEngine.Random.value, block.GetHeight() + block.GetMicroHeight(), UnityEngine.Random.value);
			//			mon.enabled = Manager.Mgr.HideInfo;
			//			m_LES.Add(mon.GetLE());
			//			mon.GetStatusBars().gameObject.SetActive(false);
			//			mon.GetLE().GetCollider().enabled = false;
			//			mon.GetLE().OnEntityDeath += OnEntityDeath;
			//			block.SetMonster(mon);
			//		}
			//	}
			//}
			//// Props
			//if (!flags[(int)IE.V4.BlockIE.Flag.Prop])
			//{
			//	bool spawnProp = block.GetMonster() == null
			//		&& !block.IsStackLinkValid(1);// block.GetStackedBlocks()[1] == null;

			//	float pdist;
			//	ushort pchance;
			//	List<IDChance> propFams;
			//	if(li.LayerType == Def.BiomeLayerType.OTHER)
			//	{
			//		pchance = li.PropGeneralChance;
			//		pdist = li.PropSafetyDistance;
			//		propFams = li.PropFamilies;
			//	}
			//	else if(li.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
			//	{
			//		pchance = 0;
			//		pdist = 0f;
			//		propFams = null;
			//	}
			//	else
			//	{
			//		var bLayer = m_Biome.GetLayers()[(int)li.LayerType];
			//		pchance = bLayer.PropGeneralChance;
			//		pdist = bLayer.PropSafetyDistance;
			//		propFams = bLayer.PropFamilies;
			//	}
			//	if (propFams != null && propFams.Count == 0)
			//		pchance = 0;

			//	if(pdist > 0f && spawnProp && pchance > 0)
			//	{
			//		var layerBlocks = m_LayerBlocks[block.GetLayer() - 1];
			//		var mHeight = block.GetHeight() + block.GetMicroHeight();
			//		for (int i = 0; i < layerBlocks.Count; ++i)
			//		{
			//			var oblock = layerBlocks[i];
			//			var oProp = oblock.GetProp();
			//			if (oblock == block || oProp == null)
			//				continue;

			//			var oid = oblock.GetPilar().GetStructureID();
			//			var mid = block.GetPilar().GetStructureID();

			//			var oPos = VPosFromPilarID(oid);
			//			var mPos = VPosFromPilarID(mid);

			//			if (Vector2.Distance(oPos, mPos) <= pdist)
			//			{
			//				if (Mathf.Abs(mHeight - (oblock.GetHeight() + oblock.GetMicroHeight())) > pdist)
			//					continue;

			//				spawnProp = false;
			//				break;
			//			}
			//		}
			//	}
			//	if(block.GetProp() != null)
			//	{
			//		block.GetProp().GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, block.GetProp().GetLE().GetCurrentHealth());
			//	}

			//	if(pchance > 0 && spawnProp && block.GetBlockType() != Def.BlockType.STAIRS && propFams != null)
			//	{
			//		if(UnityEngine.Random.Range(0, 10000) <= pchance)
			//		{
			//			int rnd = LayerInfo.RandomFromList(propFams);
			//			int familyID = propFams[rnd].ID;
			//			var propFamily = Props.PropFamilies[familyID];
			//			if (propFamily.Props.Count > 0)
			//			{
			//				var propID = UnityEngine.Random.Range(0, propFamily.Props.Count);
			//				var prop = new GameObject().AddComponent<AI.CProp>();
			//				prop.SetProp(familyID, propID);
			//				prop.SetBlock(block);
			//				block.SetProp(prop);
			//				m_LES.Add(prop.GetLE());
			//				prop.GetSprite().Flip(UnityEngine.Random.value > 0.5f, false);
			//				prop.GetLE().OnEntityDeath += OnEntityDeath;
			//				prop.enabled = Manager.Mgr.HideInfo;
			//			}
			//			else
			//			{
			//				Debug.LogWarning("Trying to spawn a prop from a family with no childs.");
			//			}
			//		}
			//	}
			//}

			// Effects
			//Debug.Log("Applied block effects not finished.");

			// Floating
			//Debug.Log("Block floating not finished.");
		}
		public void AddBlockToLayer(int layer, CBlockEdit block)
		{
			if (m_LayerBlocks[layer - 1].Contains(block))
				return;

			m_LayerBlocks[layer - 1].Add(block);
			if (!m_Layers[layer - 1].IsValid())
				return;

			ApplyLayer(block);
		}
		public void RemoveBlockFromLayer(CBlockEdit block)
		{
			int layer = block.GetLayer();

			if(layer < 1 || layer > Def.MaxLayerSlots)
			{
				Debug.Log("Trying to remove a block from an invalid layer.");
				return;
			}

			if (!m_LayerBlocks[layer - 1].Contains(block))
				return;
			m_LayerBlocks[layer - 1].Remove(block);
		}
		void InterchangePilars(int leftPilar, int rightPilar)
		{
			var lPilar = m_Pilars[leftPilar];
			var rPilar = m_Pilars[rightPilar];

			lPilar.ChangeID(rightPilar);
			rPilar.ChangeID(leftPilar);

			m_Pilars[leftPilar] = rPilar;
			m_Pilars[rightPilar] = lPilar;
		}
		public List<CBlockEdit> SetBlockWide(CBlockEdit block)
		{
			if (block.GetLayer() == 0 || block.IsRemoved())
				return new List<CBlockEdit>();

			void StoreIE(int IdxIE)
			{
				if (!m_FlagsToRestore.ContainsKey(IdxIE))
				{
					var blockIE = Structures.Strucs[IDXIE].GetBlocks()[IdxIE];
					m_FlagsToRestore.Add(IdxIE, new List<bool>(blockIE.Flags));
				}
			}

			var blockPos = VPosFromPilarID(block.GetPilar().GetStructureID());
			if (blockPos.x >= GetWidth() || blockPos.x < 0 ||
				blockPos.y >= GetHeight() || blockPos.y < 0)
				return new List<CBlockEdit>(); // Ouside bounds or too close to them

			var layer = m_Layers[block.GetLayer() - 1];
			if(layer.IsLinkedLayer)
			{
				layer = m_Layers[block.GetLinkedTo() - 1];
			}
			ushort wchance;
			if(layer.LayerType == Def.BiomeLayerType.OTHER)
			{
				wchance = layer.WideBlockChance;
			}
			else if(layer.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
			{
				wchance = 0;
			}
			else
			{
				wchance = m_Biome.GetLayers()[(int)layer.LayerType].WideBlockChance;
			}

			if (wchance == 0)
				return new List<CBlockEdit>();

			var rPos = new Vector2Int(blockPos.x + 1, blockPos.y);
			var bPos = new Vector2Int(blockPos.x, blockPos.y + 1);
			var brPos = new Vector2Int(blockPos.x + 1, blockPos.y + 1);

			var rPID = PilarIDFromVPos(rPos);
			var bPID = PilarIDFromVPos(bPos);
			var brPID = PilarIDFromVPos(brPos);

			var rPilar = m_Pilars[rPID];
			var bPilar = m_Pilars[bPID];
			var brPilar = m_Pilars[brPID];

			if (rPilar == null || bPilar == null || brPilar == null)
				return new List<CBlockEdit>();

			if (rPilar.GetBlocks().Count == 0 || bPilar.GetBlocks().Count == 0 || brPilar.GetBlocks().Count == 0)
				return new List<CBlockEdit>();

			//var layerAvailableMaterials = new List<MaterialFamily>(bLayer.MaterialFamilies.Count);
			if (block.GetMaterialFamily() == null)
				return new List<CBlockEdit>();

			if (block.GetMaterialFamily().WideMaterials.Length == 0)
			{
				return new List<CBlockEdit>();
				//layerAvailableMaterials.Add(block.GetMaterialFamily());
			}
			//else
			//{
			//    for (int i = 0; i < bLayer.MaterialFamilies.Count; ++i)
			//    {
			//        var matFamily = BlockMaterial.MaterialFamilies[bLayer.MaterialFamilies[i].ID];
			//        if (matFamily.WideMaterials.Length > 0)
			//            layerAvailableMaterials.Add(matFamily);
			//    }
			//}

			//if (layerAvailableMaterials.Count == 0)
			//    return false;

			var chance = Mathf.FloorToInt(UnityEngine.Random.value * 10000f);
			if (wchance < chance)
				return new List<CBlockEdit>();

			//var pilarAvailableBlocks = new List<CBlockEdit>[3];

			var lenNorm = block.GetLength() == 3.4f ? 3.5f : block.GetLength();
			int sliceNum = Mathf.FloorToInt(lenNorm * 2f);
			var slices = new WideSlice[sliceNum];

			CBlockEdit GetBlockAtSlice(CPilar pilar, float hSlice)
			{
				for(int i = 0; i < pilar.GetBlocks().Count; ++i)
				{
					var b = (CBlockEdit)pilar.GetBlocks()[i];
					if (b.IsRemoved() /*|| b.GetLayer() == 0*/ || b.GetBlockType() == Def.BlockType.WIDE)
						continue;

					if(b.GetBlockType() == Def.BlockType.STAIRS &&
						(hSlice == (b.GetHeight() + 0.5f) || hSlice == b.GetHeight()))
					{
						break;
					}

					var bot = b.GetHeight() - (b.GetLength() == 3.4f ? 3.5f : b.GetLength());
					if (b.GetHeight() >= hSlice && (hSlice - 0.5f) >= bot)
						return b;
				}
				return null;
			}

			// Store and then duplicate the pilars
			m_WidePreviousPilars.Add(block.GetPilar());
			m_WidePreviousPilars.Add(rPilar);
			m_WidePreviousPilars.Add(bPilar);
			m_WidePreviousPilars.Add(brPilar);

			for(int i = 0; i < 4; ++i)
			{
				m_WidePreviousPilars[m_WidePreviousPilars.Count - (i + 1)].enabled = false;
			}

			int blockPilarIdx = block.GetPilar().GetBlocks().IndexOf(block);

			var oPilar = new GameObject("Temp Pilar orig").AddComponent<CPilar>();
			rPilar = new GameObject("Temp Pilar right").AddComponent<CPilar>();
			bPilar = new GameObject("Temp Pilar bottom").AddComponent<CPilar>();
			brPilar = new GameObject("Temp Pilar rightBottom").AddComponent<CPilar>();

			oPilar.ChangeStruc(this, block.GetPilar().GetStructureID());
			rPilar.ChangeStruc(this, rPID);
			bPilar.ChangeStruc(this, bPID);
			brPilar.ChangeStruc(this, brPID);

			void DuplicateBlocks(CPilar from, CPilar to)
			{
				for (int i = 0; i < from.GetBlocks().Count; ++i)
				{
					var fBlock = from.GetBlocks()[i] as CBlockEdit;
					if (fBlock == null)
						continue;
					var tBlock = to.AddBlock();
					if (fBlock.GetLayer() != 0)
					{
						tBlock.SetIDXIE(fBlock.GetIDXIE());
						var blockIE = Structures.Strucs[m_IDXIE].GetBlocks()[fBlock.GetIDXIE()];
						StoreIE(fBlock.GetIDXIE());
						blockIE.SetFlag(IE.V4.BlockIE.Flag.Length, true);
						if (m_LayerBlocks[fBlock.GetLayer() - 1].Contains(fBlock))
						{
							m_LayerBlocks[fBlock.GetLayer() - 1].Remove(fBlock);
							//m_LayerBlocks[fBlock.GetLayer() - 1].Add(tBlock);
						}
					}
					tBlock.SetLayer(fBlock.GetLayer());
					tBlock.SetMaterialFamily(fBlock.GetMaterialFamily());
					tBlock.SetHeight(fBlock.GetHeight());
					tBlock.SetMicroHeight(fBlock.GetMicroHeight());
					tBlock.SetLength(fBlock.GetLength());
					tBlock.SetStairState(fBlock.GetStairState());
					switch (fBlock.GetBlockType())
					{
						case Def.BlockType.NORMAL:
							if (fBlock.GetTopMaterial() != null)
							{
								tBlock.SetTopMaterial(fBlock.GetTopMaterial());
								tBlock.SetMidMaterial(fBlock.GetMidMaterial());
							}
							break;
						case Def.BlockType.STAIRS:
							tBlock.SetBlockType(fBlock.GetBlockType(), fBlock.GetTopMaterial().StairType);
							break;
						case Def.BlockType.WIDE:
							tBlock.SetWIDE(fBlock.GetHiddenBlocks());
							break;
					}
					tBlock.SetRotation(fBlock.GetRotation());
					
					if(fBlock.IsRemoved())
					{
						var parent = fBlock.GetParentWIDE();
						tBlock.SetRemoved(true);
						tBlock._SetParentWIDE(parent);
						//tBlock._SetParentWIDE(fBlock.GetParentWIDE());
					}
				}
				for (int i = 0; i < from.GetBlocks().Count; ++i)
				{
					var fBlock = (CBlockEdit)from.GetBlocks()[i];
					var fAbove = fBlock.GetStackedAbove(); //fBlock.GetStackedBlocks()[1];
					int fAboveIdx = (fAbove != null) ? fBlock.GetStackedBlocksIdx()[1] /*from.GetBlocks().IndexOf(fAbove)*/ : -1;
					var fBelow = fBlock.GetStackedBelow(); //fBlock.GetStackedBlocks()[0];
					int fBelowIdx = (fAbove != null) ? fBlock.GetStackedBlocksIdx()[0] /*from.GetBlocks().IndexOf(fBelow)*/ : -1;
					var tBlock = (CBlockEdit)to.GetBlocks()[i];
					if(fAbove != null && fAboveIdx >= 0)
					{
						tBlock.GetStackedBlocksIdx()[1] = fAboveIdx;
						//tBlock.GetStackedBlocks()[1] = to.GetBlocks()[fAboveIdx];
					}
					if(fBelow != null && fBelowIdx >= 0)
					{
						tBlock.GetStackedBlocksIdx()[0] = fBelowIdx;
						//tBlock.GetStackedBlocks()[0] = to.GetBlocks()[fBelowIdx];
					}
				}
			}

			DuplicateBlocks(block.GetPilar(), oPilar);
			DuplicateBlocks(m_Pilars[rPID], rPilar);
			DuplicateBlocks(m_Pilars[bPID], bPilar);
			DuplicateBlocks(m_Pilars[brPID], brPilar);
			m_WideConvertedPilars.Add(oPilar);
			m_WideConvertedPilars.Add(rPilar);
			m_WideConvertedPilars.Add(bPilar);
			m_WideConvertedPilars.Add(brPilar);
			m_Pilars[oPilar.GetStructureID()] = oPilar;
			m_Pilars[rPilar.GetStructureID()] = rPilar;
			m_Pilars[bPilar.GetStructureID()] = bPilar;
			m_Pilars[brPilar.GetStructureID()] = brPilar;

			var oBlock = oPilar.GetBlocks()[blockPilarIdx] as CBlockEdit;

			int currentSlice = 0;
			if(oBlock.GetBlockType() == Def.BlockType.STAIRS)
			{
				currentSlice = 1;
				slices[0] = new WideSlice()
				{
					Possible = false,
					Blocks = null
				};
			}

			for (; currentSlice < sliceNum; ++currentSlice)
			{
				var sliceBlocks = new CBlockEdit[3] { null, null, null };
				bool possible = true;

				sliceBlocks[0] = GetBlockAtSlice(rPilar, oBlock.GetHeight() - (currentSlice * 0.5f));
				sliceBlocks[1] = GetBlockAtSlice(bPilar, oBlock.GetHeight() - (currentSlice * 0.5f));
				sliceBlocks[2] = GetBlockAtSlice(brPilar, oBlock.GetHeight() - (currentSlice * 0.5f));

				if(sliceBlocks[0] == null || sliceBlocks[1] == null || sliceBlocks[2] == null)
				{
					possible = false;
				}
				else
				{
					int matFamID = BlockMaterial.MaterialFamilies.IndexOf(block.GetMaterialFamily());
					for(int j = 0; j < sliceBlocks.Length; ++j)
					{
						if(sliceBlocks[j].GetLayer() == 0)
						{
							possible = false;
							break;
						}

						if (block.GetMaterialFamily() == sliceBlocks[j].GetMaterialFamily())
							continue;

						var sbLayer = m_Layers[sliceBlocks[j].GetLayer() - 1];
						if(sbLayer.IsLinkedLayer)
						{
							bool found = false;
							for (int k = 0; k < sbLayer.LinkedLayers.Count; ++k)
							{
								var lLayer = m_Layers[sbLayer.LinkedLayers[k].ID - 1];
								List<IDChance> matFams;
								if (lLayer.LayerType == Def.BiomeLayerType.OTHER)
								{
									matFams = lLayer.MaterialFamilies;
									
								}
								else if(lLayer.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
								{
									continue;
								}
								else
								{
									matFams = m_Biome.GetLayers()[(int)lLayer.LayerType].MaterialFamilies;
								}

								if(matFams != null)
								{
									for (int w = 0; w < matFams.Count; ++w)
									{
										if (matFams[w].ID == matFamID)
										{
											found = true;
											break;
										}
									}
								}
								if (found)
									break;
							}
							if (!found)
							{
								possible = false;
								break;
							}
						}
						else
						{
							bool found = false;
							List<IDChance> matFams = null;
							if (sbLayer.LayerType == Def.BiomeLayerType.OTHER)
							{
								matFams = sbLayer.MaterialFamilies;

							}
							else if (sbLayer.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
							{
								// No-op
							}
							else
							{
								matFams = m_Biome.GetLayers()[(int)sbLayer.LayerType].MaterialFamilies;
							}
							if (matFams != null)
							{
								for (int k = 0; k < matFams.Count; ++k)
								{
									if (matFams[k].ID == matFamID)
									{
										found = true;
										break;
									}
								}
							}
							if(!found)
							{
								possible = false;
								break;
							}
						}
					}
				}

				slices[currentSlice] = new WideSlice()
				{
					Possible = possible,
					Blocks = sliceBlocks
				};
			}

			void SliceBlock(int sliceIdx, float curHeight, int side)
			{
				var sBlock = slices[sliceIdx].Blocks[side];
				var blockIE = Structures.Strucs[m_IDXIE].GetBlocks()[sBlock.GetIDXIE()];
				StoreIE(sBlock.GetIDXIE());
				blockIE.SetFlag(IE.V4.BlockIE.Flag.Length, true);
				if (sBlock.GetHeight() > curHeight) // Top excess part
				{
					var topPart = sBlock.GetPilar().AddBlock();
					topPart.SetIDXIE(sBlock.GetIDXIE());
					topPart.SetLayer(sBlock.GetLayer());
					topPart.SetHeight(sBlock.GetHeight());
					topPart.SetLength(sBlock.GetHeight() - curHeight);
					topPart.SetStairState(sBlock.GetStairState());
					if(sBlock.GetBlockType() == Def.BlockType.STAIRS)
					{
						topPart.SetBlockType(Def.BlockType.STAIRS, topPart.GetTopMaterial().StairType);
					}
					else
					{
						topPart.SetTopMaterial(sBlock.GetTopMaterial());
						topPart.SetMidMaterial(sBlock.GetMidMaterial());
					}
					topPart.SetMicroHeight(sBlock.GetMicroHeight());
					topPart.SetRotation(sBlock.GetRotation());
				}
				if ((sBlock.GetHeight() - (sBlock.GetLength() == 3.4f ? 3.5f : sBlock.GetLength())) < (curHeight - 0.5f)) // Bot excess part
				{
					var botPart = sBlock.GetPilar().AddBlock();
					//botPart.GetStackedBlocks()[1] = sBlock;
					botPart.SetIDXIE(sBlock.GetIDXIE());

					botPart.SetLayer(sBlock.GetLayer());
					botPart.SetHeight(curHeight - 0.5f);
					botPart.SetHeight(curHeight - 0.5f);
					botPart.SetLength((curHeight - 0.5f) - (sBlock.GetHeight() - (sBlock.GetLength() == 3.4f ? 3.5f : sBlock.GetLength())));
					botPart.SetTopMaterial(sBlock.GetTopMaterial());
					botPart.SetMidMaterial(sBlock.GetMidMaterial());
					botPart.SetMicroHeight(sBlock.GetMicroHeight());
					botPart.SetStairState(sBlock.GetStairState());
					botPart.SetBlockType(Def.BlockType.NORMAL);
					botPart.SetRotation(sBlock.GetRotation());
					for (int j = (sliceIdx + 1); j < sliceNum; ++j)
					{
						if (!slices[j].Possible /*|| slices[j].Blocks[side] != sBlock*/)
							continue;
						slices[j].Blocks[side] = botPart;
					}
				}
				sBlock.SetLength(0.5f);
				sBlock.SetHeight(curHeight);
			}

			var doneWides = new List<CBlockEdit>(sliceNum);
			int lastWideSlice = int.MinValue;
			for(int i = 0; i < sliceNum; ++i)
			{
				if (!slices[i].Possible)
					continue;

				var slice = slices[i];
				var curHeight = oBlock.GetHeight() - (i * 0.5f);

				SliceBlock(i, curHeight, 0); // Right
				SliceBlock(i, curHeight, 1); // Bottom
				SliceBlock(i, curHeight, 2); // BottomRight

				// Slice main block
				// have we done a wide in the previous iteration?
				if(lastWideSlice == int.MinValue)
				{
					// we have not done any, top main block part
					if(i > 0)
					{
						var sBlock = oBlock.GetPilar().AddBlock();
						sBlock.SetIDXIE(oBlock.GetIDXIE());
						sBlock.SetLayer(oBlock.GetLayer());
						sBlock.SetMaterialFamily(oBlock.GetMaterialFamily());
						sBlock.SetHeight(oBlock.GetHeight());
						sBlock.SetMicroHeight(oBlock.GetMicroHeight());
						sBlock.SetLength(oBlock.GetHeight() - curHeight);
						//sBlock.SetMicroHeight(oBlock.GetMicroHeight());
						sBlock.SetStairState(oBlock.GetStairState());
						if (oBlock.GetBlockType() == Def.BlockType.STAIRS)
							sBlock.SetBlockType(Def.BlockType.STAIRS, oBlock.GetTopMaterial().StairType);
						//sBlock.SetBlockType(oBlock.GetBlockType());
						sBlock.SetRotation(oBlock.GetRotation());
					}
				}
				// we have done some but not last iteration
				else if(lastWideSlice != (i - 1))
				{
					var sBlock = oBlock.GetPilar().AddBlock();
					sBlock.SetIDXIE(oBlock.GetIDXIE());
					sBlock.SetLayer(oBlock.GetLayer());
					sBlock.SetMaterialFamily(oBlock.GetMaterialFamily());
					var lastHeight = oBlock.GetHeight() - (lastWideSlice * 0.5f);
					sBlock.SetHeight(lastHeight);
					sBlock.SetLength(lastHeight - curHeight);
					sBlock.SetMicroHeight(oBlock.GetMicroHeight());
					sBlock.SetStairState(oBlock.GetStairState());
					sBlock.SetBlockType(Def.BlockType.NORMAL);
					sBlock.SetRotation(oBlock.GetRotation());
				}

				var wBlock = oBlock.GetPilar().AddBlock();
				wBlock.SetIDXIE(oBlock.GetIDXIE());
				wBlock.SetLayer(oBlock.GetLayer());
				wBlock.SetLength(0.5f);
				wBlock.SetHeight(curHeight);
				wBlock.SetMaterialFamily(oBlock.GetMaterialFamily());
				wBlock.SetMicroHeight(oBlock.GetMicroHeight());
				wBlock.SetWIDE(new CBlockEdit[3] { slice.Blocks[0], slice.Blocks[1], slice.Blocks[2] });
				doneWides.Add(wBlock);
				lastWideSlice = i;
			}
			// Bot excess main block
			if(lastWideSlice >= 0 && lastWideSlice < (sliceNum - 1))
			{
				var sBlock = oBlock.GetPilar().AddBlock();
				sBlock.SetIDXIE(oBlock.GetIDXIE());
				sBlock.SetLayer(oBlock.GetLayer());
				sBlock.SetMaterialFamily(oBlock.GetMaterialFamily());
				var lastHeight = oBlock.GetHeight() - ((lastWideSlice + 1) * 0.5f);
				var sHeight = oBlock.GetHeight() - (oBlock.GetLength() == 3.4f ? 3.5f : oBlock.GetLength());
				sBlock.SetHeight(lastHeight);
				sBlock.SetLength(lastHeight - sHeight);
				sBlock.SetMicroHeight(oBlock.GetMicroHeight());
				sBlock.SetStairState(sBlock.GetStairState());
				sBlock.SetBlockType(Def.BlockType.NORMAL);
				sBlock.SetRotation(oBlock.GetRotation());
			}
			// No wide done
			if (lastWideSlice == int.MinValue)
			{
				//oBlock.GetPilar().DestroyPilar(false, true);
				//rPilar.DestroyPilar(false, true);
				//bPilar.DestroyPilar(false, true);
				//brPilar.DestroyPilar(false, true);
				//for(int i = 0; i < 4; ++i)
				//{
				//	var idx = m_WidePreviousPilars.Count - 1;
				//	m_Pilars[m_WidePreviousPilars[idx].GetStructureID()] = m_WidePreviousPilars[idx];
				//	m_WidePreviousPilars[idx].enabled = true;
				//	for(int j = 0; j < m_WidePreviousPilars[idx].GetBlocks().Count; ++j)
				//	{
				//		var restoredBlock = (CBlockEdit)m_WidePreviousPilars[idx].GetBlocks()[j];
				//		if (restoredBlock.GetLayer() == 0)
				//			continue;
				//		m_LayerBlocks[restoredBlock.GetLayer() - 1].Add(restoredBlock);
				//		restoredBlock._CheckStackedLinks();
				//		if (restoredBlock.GetLockState() == Def.LockState.Unlocked)
				//		{
				//			var restoredBlockIE = Structures.Strucs[m_IDXIE].GetBlocks()[restoredBlock.GetIDXIE()];
				//			restoredBlockIE.SetFlag(IE.V3.BlockIE.Flag.Height, false);
				//			restoredBlockIE.SetFlag(IE.V3.BlockIE.Flag.Length, false);
				//		}
				//	}
				//	m_WidePreviousPilars.RemoveAt(idx);
				//	m_WideConvertedPilars.RemoveAt(idx);
				//}
				RestoreLastConvertedPilars();
			}
			else
			{
				oBlock.DestroyBlock(false, true);
			}

			// Wide merge
			for (int i = 0; i < doneWides.Count; ++i)
			{
				int mergeable = 1;
				var curWide = doneWides[i];
				var startHeight = curWide.GetHeight();
				for (int j = i + 1; j < doneWides.Count; ++j)
				{
					var mergeBlocks = j - i;
					var mergeLen = mergeBlocks * 0.5f;
					var mergableHeight = startHeight - mergeLen;
					if (doneWides[j].GetHeight() != mergableHeight || mergeBlocks > 4)
						break;
					++mergeable;
					var hidden = doneWides[j].GetHiddenBlocks();
					doneWides[j].DestroyBlock(false, true);
					if (hidden == null)
						continue;
					for(int k = 0; k < hidden.Length; ++k)
					{
						if (hidden[k] == null)
							continue;
						hidden[k].DestroyBlock(false, true);
					}
				}

				curWide.SetLength(mergeable * 0.5f);
				for(int j = 0; j < (mergeable - 1); ++j)
				{
					doneWides.RemoveAt(i + 1);
				}

				//i = mergable - 1;
			}

			for(int i = 0; i < doneWides.Count; ++i)
			{
				var wide = doneWides[i];
				var wlayer = m_Layers[wide.GetLayer()];
				if (wlayer.IsLinkedLayer)
					wlayer = m_Layers[wide.GetLinkedTo()];
				for(int j = 0; j < wide.GetHiddenBlocks().Length; ++j)
				{
					var hidden = wide.GetHiddenBlocks()[j];
					if (hidden == null)
						continue;
					hidden.SetMicroHeight(wide.GetMicroHeight());
					//if(hidden.GetProp() != null)
					//{
					//	var pid = Props.FamilyDict[hidden.GetProp().GetInfo().Family.FamilyName];
					//	bool isInWideLayer = false;
					//	for(int k = 0; k < wlayer.PropFamilies.Count; ++k)
					//	{
					//		if(wlayer.PropFamilies[k].ID == pid)
					//		{
					//			isInWideLayer = true;
					//			break;
					//		}
					//	}
					//	if(!isInWideLayer)
					//	{
					//		hidden.GetProp().gameObject.SetActive(false);
					//	}
					//}
					//if(hidden.GetMonster() != null)
					//{
					//	var pid = Monsters.FamilyDict[hidden.GetMonster().GetFamily().Name];
					//	bool isInWideLayer = false;
					//	for (int k = 0; k < wlayer.MonsterFamilies.Count; ++k)
					//	{
					//		if (wlayer.MonsterFamilies[k].ID == pid)
					//		{
					//			isInWideLayer = true;
					//			break;
					//		}
					//	}
					//	if (!isInWideLayer)
					//	{
					//		hidden.GetMonster().gameObject.SetActive(false);
					//	}
					//}
				}
			}

			return doneWides;
		}
		//static List<CBlockEdit> m_WideDoneBlocks = new List<CBlockEdit>();
		//static int m_WideCurrentPilar = 0;
		//static int m_WideCurrentBlock = 0;
		void ApplyWideChange()
		{
			//m_WideCurrentPilar = 0;
			//m_WideCurrentBlock = 0;
			//m_WideDoneBlocks.Clear();
			int m_WideCurrentPilar = 0;
			int m_WideCurrentBlock = 0;
			while (true)
			{
				if (m_WideCurrentPilar >= m_Pilars.Length)
					break;

				var pilar = m_Pilars[m_WideCurrentPilar];
				if (pilar == null)
				{
					++m_WideCurrentPilar;
					continue;
				}
				if (pilar.GetBlocks().Count <= m_WideCurrentBlock)
				{
					m_WideCurrentBlock = 0;
					++m_WideCurrentPilar;
					continue;
				}
				if (pilar.GetBlocks().Count == 1 && pilar.GetBlocks()[0].GetLayer() == 0)
				{
					++m_WideCurrentPilar;
					continue;
				}
				// Ouside bounds or too close to them
				var pilarPos = VPosFromPilarID(pilar.GetStructureID());
				if (pilarPos.x >= (GetWidth() - 1) || pilarPos.y >= (GetHeight() - 1))
				{
					++m_WideCurrentPilar;
					continue;
				}

				var rPos = new Vector2Int(pilarPos.x + 1, pilarPos.y);
				var bPos = new Vector2Int(pilarPos.x, pilarPos.y + 1);
				var brPos = new Vector2Int(pilarPos.x + 1, pilarPos.y + 1);

				var rPID = PilarIDFromVPos(rPos);
				var bPID = PilarIDFromVPos(bPos);
				var brPID = PilarIDFromVPos(brPos);

				var rPilar = m_Pilars[rPID];
				var bPilar = m_Pilars[bPID];
				var brPilar = m_Pilars[brPID];

				if (rPilar == null || bPilar == null || brPilar == null)
				{
					++m_WideCurrentPilar;
					continue;
				}

				var cur = pilar.GetBlocks()[m_WideCurrentBlock] as CBlockEdit;
				if (m_WideBlocks.Contains(cur))
				{
					++m_WideCurrentBlock;
					if (pilar.GetBlocks().Count <= m_WideCurrentBlock)
					{
						m_WideCurrentBlock = 0;
						++m_WideCurrentPilar;

					}
					continue;
				}
				if (cur.IsRemoved() || cur.GetBlockType() == Def.BlockType.WIDE)
				{
					++m_WideCurrentBlock;
					if (pilar.GetBlocks().Count <= m_WideCurrentBlock)
					{
						m_WideCurrentBlock = 0;
						++m_WideCurrentPilar;
					}
					continue;
				}

				var wides = SetBlockWide(cur);
				for (int i = 0; i < wides.Count; ++i)
				{
					m_WideBlocks.Add(wides[i]);
					m_WideBlocks.AddRange(wides[i].GetHiddenBlocks());
				}
				if (wides.Count == 0)
					++m_WideCurrentBlock;

				for (int i = 0; i < m_WideBlocks.Count; i += 4)
				{
					var parent = m_WideBlocks[i];
					for (int j = 0; j < parent.GetHiddenBlocks().Length; ++j)
					{
						var child = parent.GetHiddenBlocks()[j];
						if (child == null)
							continue;
						child.SetRemoved(true);
						child._SetParentWIDE(parent);
					}
				}
				//if(wides.Count != 0)
				//{
				//	m_WideCurrentPilar += m_Pilars.Length;
				//}
				//if (pilar.GetBlocks().Count <= m_WideCurrentBlock)
				//{
				//    m_WideCurrentBlock = 0;
				//    ++m_WideCurrentPilar;
				//}
			}
			void applyMicroheight(CPilar pilar, float mheight)
			{
				for(int i = 0; i < pilar.GetBlocks().Count; ++i)
				{
					var block = pilar.GetBlocks()[i] as CBlockEdit;
					if (block == null || block.IsRemoved() || block.GetLayer() == 0)
						continue;
					block.SetMicroHeight(mheight);
				}
			}
			void removeWeirdBlock(CBlockEdit wideBlock)
			{
				var pilar = wideBlock.GetPilar();
				for(int i = 0; i < pilar.GetBlocks().Count; ++i)
				{
					var b = pilar.GetBlocks()[i] as CBlockEdit;
					if (b == wideBlock)
						continue;
					if(b.GetHeight() == wideBlock.GetHeight())
					{
						b.DestroyBlock(false);
					}
				}
			}
			for(int i = 0; i < m_WideBlocks.Count; i += 4)
			{
				var cur = m_WideBlocks[i];
				if (!m_Pilars[cur.GetPilar().GetStructureID()].GetBlocks().Contains(cur))
					continue;

				removeWeirdBlock(cur);

				var mheight = cur.GetMicroHeight();
				var rPilar = cur.GetHiddenBlocks()[0].GetPilar();
				var bPilar = cur.GetHiddenBlocks()[1].GetPilar();
				var brPilar = cur.GetHiddenBlocks()[2].GetPilar();

				applyMicroheight(rPilar, mheight);
				applyMicroheight(bPilar, mheight);
				applyMicroheight(brPilar, mheight);
			}
			m_WideBlocks.Clear();
			//void wideTask()
			//{
			//    if (m_WideCurrentPilar >= m_Pilars.Length)
			//        return;

			//    var pilar = m_Pilars[m_WideCurrentPilar];
			//    if(pilar == null)
			//    {
			//        ++m_WideCurrentPilar;
			//        BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
			//        {
			//            Cost = BackgroundQueue.DefaultTaskCost,
			//            FN = wideTask
			//        });
			//        return;
			//    }

			//    if(pilar.GetBlocks().Count <= m_WideCurrentBlock)
			//    {
			//        m_WideCurrentBlock = 0;
			//        ++m_WideCurrentPilar;
			//        BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
			//        {
			//            Cost = BackgroundQueue.DefaultTaskCost,
			//            FN = wideTask
			//        });
			//        return;
			//    }

			//    // Ouside bounds or too close to them
			//    var pilarPos = VPosFromPilarID(pilar.GetStructureID());
			//    if(pilarPos.x >= (GetWidth() - 1) || pilarPos.y >= (GetHeight() - 1))
			//    {
			//        ++m_WideCurrentPilar;
			//        BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
			//        {
			//            Cost = BackgroundQueue.DefaultTaskCost,
			//            FN = wideTask
			//        });
			//        return;
			//    }

			//    var rPos = new Vector2Int(pilarPos.x + 1, pilarPos.y);
			//    var bPos = new Vector2Int(pilarPos.x, pilarPos.y + 1);
			//    var brPos = new Vector2Int(pilarPos.x + 1, pilarPos.y + 1);

			//    var rPID = PilarIDFromVPos(rPos);
			//    var bPID = PilarIDFromVPos(bPos);
			//    var brPID = PilarIDFromVPos(brPos);

			//    var rPilar = m_Pilars[rPID];
			//    var bPilar = m_Pilars[bPID];
			//    var brPilar = m_Pilars[brPID];

			//    if (rPilar == null || bPilar == null || brPilar == null)
			//    {
			//        ++m_WideCurrentPilar;
			//        BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
			//        {
			//            Cost = BackgroundQueue.DefaultTaskCost,
			//            FN = wideTask
			//        });
			//        return;
			//    }

			//    var cur = (CBlockEdit)pilar.GetBlocks()[m_WideCurrentBlock];
			//    if(m_WideDoneBlocks.Contains(cur))
			//    {
			//        ++m_WideCurrentBlock;
			//        if(pilar.GetBlocks().Count <= m_WideCurrentBlock)
			//        {
			//            m_WideCurrentBlock = 0;
			//            ++m_WideCurrentPilar;

			//        }
			//        BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
			//        {
			//            Cost = BackgroundQueue.DefaultTaskCost,
			//            FN = wideTask
			//        });
			//        return;
			//    }

			//    if(cur.IsRemoved() || cur.GetBlockType() == Def.BlockType.WIDE)
			//    {
			//        ++m_WideCurrentBlock;
			//        if(pilar.GetBlocks().Count <= m_WideCurrentBlock)
			//        {
			//            m_WideCurrentBlock = 0;
			//            ++m_WideCurrentPilar;
			//        }
			//        BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
			//        {
			//            Cost = BackgroundQueue.DefaultTaskCost,
			//            FN = wideTask
			//        });
			//        return;
			//    }
				
				

			//    var wides = SetBlockWide(cur);
			//    for(int i = 0; i < wides.Count; ++i)
			//    {
			//        m_WideDoneBlocks.Add(wides[i]);
			//        m_WideDoneBlocks.AddRange(wides[i].GetHiddenBlocks());
			//    }
			//    ++m_WideCurrentBlock;
			//    if (pilar.GetBlocks().Count <= m_WideCurrentBlock)
			//    {
			//        m_WideCurrentBlock = 0;
			//        ++m_WideCurrentPilar;
			//    }
			//    BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
			//    {
			//        Cost = BackgroundQueue.DefaultTaskCost,
			//        FN = wideTask
			//    });
			//}

			//void cleanTask()
			//{
			//    for(int i = 0; i < m_WideConvertedPilars.Count; ++i)
			//    {
			//        var p = m_WideConvertedPilars[i];
			//        for(int j = 0; j < p.GetBlocks().Count; ++j)
			//        {
			//            var b = (CBlockEdit)p.GetBlocks()[j];
			//            if(b.GetBlockType() == Def.BlockType.WIDE)
			//            {
			//                for(int k = 0; k < b.GetHiddenBlocks().Length; ++k)
			//                {
			//                    var h = b.GetHiddenBlocks()[k];
			//                    h.SetHeight(b.GetHeight());
			//                    h.SetMicroHeight(b.GetMicroHeight());
			//                    h._SetParentWIDE(b);
			//                    h.SetRemoved(true);
			//                }
			//            }
			//        }
			//    }
			//}

			//BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
			//{
			//    Cost = BackgroundQueue.DefaultMaxCostPerFrame,
			//    FN = wideTask
			//});

			//BackgroundQueue.Mgr.ScheduleOnIdleTask(cleanTask);

			//var li = m_Layers[layer - 1];
			//if(li.IsLinkedLayer)
			//{
			//    var layerIdx = li.LinkedLayers[LayerInfo.RandomFromList(li.LinkedLayers)].ID;
			//    li = m_Layers[layerIdx - 1];
			//}
			//if (!li.RandomWideBlockEnabled)
			//    return;
			//var blocks = m_LayerBlocks[layer - 1];
			//var wideBlocks = new List<CBlockEdit[]>();
			//var possibleBlocks = new Dictionary<int, List<CBlockEdit>>();

			//var tempList = new List<CBlockEdit>();

			//CBlockEdit GetBlockAt(int structID, int familyID, float height)
			//{
			//    tempList.Clear();
			//    var vblocks = possibleBlocks[familyID];
			//    for(int i = 0; i < vblocks.Count; ++i)
			//    {
			//        if (vblocks[i].GetPilar().GetStructureID() == structID)
			//            tempList.Add(vblocks[i]);
			//    }
			//    if(tempList.Count == 0)
			//        return null;
			//    float diff = float.MaxValue;
			//    CBlockEdit b = null;
			//    for(int i = 0; i < tempList.Count; ++i)
			//    {
			//        var df = Mathf.Abs(tempList[i].GetHeight() - height);
			//        if(df < diff)
			//        {
			//            diff = df;
			//            b = tempList[i];
			//        }
			//    }
			//    return b;
			//}
			
			//// Obtain all possible blocks and sort them with their materials
			//for (int i = 0; i < blocks.Count; ++i)
			//{
			//    var block = blocks[i];
			//    var family = block.GetMaterialFamily();
			//    if (family.WideMaterials.Length == 0 || block.IsRemoved()
			//        || block.GetBlockType() != Def.BlockType.NORMAL
			//        || block.GetLockState() == Def.LockState.Locked)
			//        continue;
			//    var familyID = BlockMaterial.FamilyDict[family.FamilyInfo.FamilyName];
			//    if (!possibleBlocks.ContainsKey(familyID))
			//        possibleBlocks.Add(familyID, new List<CBlockEdit>(1));
			//    var list = possibleBlocks[familyID];
			//    list.Add(block);
			//    possibleBlocks[familyID] = list;
			//}
			//// Find all the possible groups of 4 blocks of the same material and in the correct positions
			//for(int i = 0; i < possibleBlocks.Count; ++i)
			//{
			//    var pair = possibleBlocks.ElementAt(i);
			//    var blist = pair.Value;
			//    for(int j = 0; j < blist.Count; ++j)
			//    {
			//        var b = blist[j];
			//        var vPos = VPosFromPilarID(b.GetPilar().GetStructureID());
			//        // Ignore border blocks
			//        if (vPos.x == (GetWidth() - 1) || vPos.y == (GetHeight() - 1))
			//            continue;

			//        var bRight = GetBlockAt(
			//            PilarIDFromVPos(new Vector2Int(vPos.x + 1, vPos.y)), pair.Key, b.GetHeight());
			//        if (bRight == null)
			//            continue;
			//        var bBot = GetBlockAt(
			//            PilarIDFromVPos(new Vector2Int(vPos.x, vPos.y + 1)), pair.Key, b.GetHeight());
			//        if (bBot == null)
			//            continue;
			//        var bBotRight = GetBlockAt(
			//            PilarIDFromVPos(new Vector2Int(vPos.x + 1, vPos.y + 1)), pair.Key, b.GetHeight());
			//        if (bBotRight == null)
			//            continue;
			//        var arr = new CBlockEdit[4] { b, bRight, bBot, bBotRight };
			//        // Do not reassign
			//        for (int k = 0; k < arr.Length; ++k)
			//            blist.Remove(arr[k]);
			//        wideBlocks.Add(arr);
			//    }
			//}
			//// Apply wide change
			//for(int i = 0; i < wideBlocks.Count; ++i)
			//{
			//    // Apply the random factor
			//    if (UnityEngine.Random.value > li.WideBlockChance)
			//        continue;
			//    var blist = wideBlocks[i];
			//    var parent = blist[0];
			//    parent.SetWIDE(new CBlockEdit[3] { blist[1], blist[2], blist[3] });
			//}
		}
		void UnapplyWideChange(int layer)
		{

			//var li = m_Layers[layer - 1];
			//if (li.IsLinkedLayer)
			//{
			//    var layerIdx = li.LinkedLayers[LayerInfo.RandomFromList(li.LinkedLayers)].ID;
			//    li = m_Layers[layerIdx - 1];
			//}
			//if (!li.RandomWideBlockEnabled)
			//    return;
			//var layerBlocks = m_LayerBlocks[layer - 1];
			//var blocksIE = Structures.Strucs[m_IDXIE].GetBlocks();
			//for(int i = 0; i < layerBlocks.Count; ++i)
			//{
			//    var block = layerBlocks[i];
			//    if (block.GetBlockType() != Def.BlockType.WIDE)
			//        continue;

			//    if (blocksIE[block.GetIDXIE()].BlockType == Def.BlockType.WIDE)
			//        continue;

			//    block.SetBlockType(Def.BlockType.NORMAL);
			//}
		}
		void RestoreLastConvertedPilars()
		{
			void restoreBlocks(CPilar pilar)
			{
				for (int i = 0; i < pilar.GetBlocks().Count; ++i)
				{
					var block = (CBlockEdit)pilar.GetBlocks()[i];
					if (block.GetLayer() == 0)
						continue;

					m_LayerBlocks[block.GetLayer() - 1].Add(block);
					block._CheckStackedLinks();
					var idxie = block.GetIDXIE();
					if(m_FlagsToRestore.ContainsKey(idxie))
					{
						var blockIE = Structures.Strucs[IDXIE].GetBlocks()[idxie];
						blockIE.Flags = m_FlagsToRestore[idxie];
						m_FlagsToRestore.Remove(idxie);
					}
					//if ((block.GetLockState() == Def.LockState.Unlocked) || 
					//	(block.GetLockState() == Def.LockState.SemiLocked && block.GetStairState() == Def.StairState.POSSIBLE))
					//{
					//	var blockIE = Structures.Strucs[m_IDXIE].GetBlocks()[block.GetIDXIE()];
					//	blockIE.SetFlag(IE.V3.BlockIE.Flag.Height, false);
					//	blockIE.SetFlag(IE.V3.BlockIE.Flag.Length, false);
					//}
				}
			}
			int brIdx = m_WideConvertedPilars.Count - 1;
			int bIdx = m_WideConvertedPilars.Count - 2;
			int rIdx = m_WideConvertedPilars.Count - 3;
			int wIdx = m_WideConvertedPilars.Count - 4;

			var convWide = m_WideConvertedPilars[wIdx];
			var convRight = m_WideConvertedPilars[rIdx];
			var convBottom = m_WideConvertedPilars[bIdx];
			var convBotRight = m_WideConvertedPilars[brIdx];

			int brSID = convBotRight.GetStructureID();
			int bSID = convBottom.GetStructureID();
			int rSID = convRight.GetStructureID();
			int wSID = convWide.GetStructureID();

			convWide.DestroyPilar(false, true);
			convRight.DestroyPilar(false, true);
			convBottom.DestroyPilar(false, true);
			convBotRight.DestroyPilar(false, true);

			m_WidePreviousPilars[wIdx].enabled = true;
			m_WidePreviousPilars[rIdx].enabled = true;
			m_WidePreviousPilars[bIdx].enabled = true;
			m_WidePreviousPilars[brIdx].enabled = true;

			m_Pilars[wSID] = m_WidePreviousPilars[wIdx];
			m_Pilars[rSID] = m_WidePreviousPilars[rIdx];
			m_Pilars[bSID] = m_WidePreviousPilars[bIdx];
			m_Pilars[brSID] = m_WidePreviousPilars[brIdx];

			restoreBlocks(m_Pilars[wSID]);
			restoreBlocks(m_Pilars[rSID]);
			restoreBlocks(m_Pilars[bSID]);
			restoreBlocks(m_Pilars[brSID]);

			m_WideConvertedPilars.RemoveAt(brIdx);
			m_WideConvertedPilars.RemoveAt(bIdx);
			m_WideConvertedPilars.RemoveAt(rIdx);
			m_WideConvertedPilars.RemoveAt(wIdx);

			m_WidePreviousPilars.RemoveAt(brIdx);
			m_WidePreviousPilars.RemoveAt(bIdx);
			m_WidePreviousPilars.RemoveAt(rIdx);
			m_WidePreviousPilars.RemoveAt(wIdx);
		}
		void UnapplyWides()
		{
			while(m_WideConvertedPilars.Count > 0)
			{
				RestoreLastConvertedPilars();
			}
		}
		void ApplyStairChange(int layer)
		{
			var li = m_Layers[layer - 1];
			if (!li.IsLinkedLayer && !li.RandomStairBlockEnabled)
				return;

			var blocks = m_LayerBlocks[layer - 1];
			for (int i = 0; i < blocks.Count; ++i)
			{
				var block = blocks[i];
				if (block.GetStairState() != Def.StairState.POSSIBLE || block.IsRemoved())
					continue;

				LayerInfo tLayer;
				if (li.IsLinkedLayer)
				{
					var layerIdx = li.LinkedLayers[LayerInfo.RandomFromList(li.LinkedLayers)].ID;
					tLayer = m_Layers[layerIdx - 1];
					if (!tLayer.RandomStairBlockEnabled)
						continue;
				}
				else
				{
					tLayer = li;
				}

				if ((UnityEngine.Random.value * 10000f) > tLayer.StairBlockChance)
					continue;

				block.SetBlockType(Def.BlockType.STAIRS);
				if (block.GetProp() != null)
				{
					block.GetProp().enabled = false;
				}
				if (block.GetMonster() != null)
				{
					block.GetMonster().enabled = false;
				}
			}
		}
		void UnapplyStairChange(int layer)
		{
			var li = m_Layers[layer - 1];
			//if (li.IsLinkedLayer)
			//{
			//	var layerIdx = li.LinkedLayers[LayerInfo.RandomFromList(li.LinkedLayers)].ID;
			//	li = m_Layers[layerIdx - 1];
			//}
			//if (!li.RandomStairBlockEnabled)
			//	return;

			var blocks = m_LayerBlocks[layer - 1];
			for (int i = 0; i < blocks.Count; ++i)
			{
				var block = blocks[i];
				if (block.GetStairState() != Def.StairState.POSSIBLE || 
					block.GetBlockType() != Def.BlockType.STAIRS)
					continue;

				block.SetBlockType(Def.BlockType.NORMAL);

				var familyID = li.MaterialFamilies[LayerInfo.RandomFromList(li.MaterialFamilies)].ID;
				block.SetMaterialFamily(BlockMaterial.MaterialFamilies[familyID]);

				if (block.GetProp() != null)
				{
					block.GetProp().enabled = true;
				}
				if (block.GetMonster() != null)
				{
					block.GetMonster().enabled = true;
				}
			}
		}
		public void ApplyStairs(/*bool apply*/)
		{
			//void Apply(CBlockEdit block)
			//{
			//	if (block.GetBlockType() != Def.BlockType.NORMAL)
			//		return;

			//	var layer = m_Layers[block.GetLayer() - 1];
			//	if (layer.IsLinkedLayer)
			//	{
			//		layer = m_Layers[block.GetLinkedTo() - 1];
			//	}

			//	ushort schance, rchance;
			//	if(layer.LayerType == Def.BiomeLayerType.OTHER)
			//	{
			//		schance = layer.StairBlockChance;
			//		rchance = layer.RampBlockChance;
			//	}
			//	else if(layer.LayerType == Def.BiomeLayerType.OTHER || m_Biome == null)
			//	{
			//		schance = rchance = 0;
			//	}
			//	else
			//	{
			//		var blayer = m_Biome.GetLayers()[(int)layer.LayerType];
			//		schance = blayer.StairBlockChance;
			//		rchance = blayer.RampBlockChance;
			//	}

			//	switch(block.GetStairState())
			//	{
			//		case Def.StairState.POSSIBLE:
			//			{
			//				ushort rng = (ushort)(UnityEngine.Random.value * 10000f);
			//				if (rng > schance)
			//					return;
			//				block.SetBlockType(Def.BlockType.STAIRS);
			//			}
			//			break;
			//		case Def.StairState.STAIR_OR_RAMP:
			//			{
			//				ushort rng = (ushort)(UnityEngine.Random.value * 10000f);
			//				if (rng > schance)
			//					return;
			//				ushort rampRNG = (ushort)(UnityEngine.Random.value * 10000f);
			//				if (rampRNG >= rchance)
			//					block.SetBlockType(Def.BlockType.STAIRS);
			//				else
			//					block.SetBlockType(Def.BlockType.STAIRS, Def.StairType.RAMP);
			//			}
			//			break;
			//		case Def.StairState.RAMP_POSSIBLE:
			//			{
			//				ushort rng = (ushort)(UnityEngine.Random.value * 10000f);
			//				if (rng > schance)
			//					return;
			//				block.SetBlockType(Def.BlockType.STAIRS, Def.StairType.RAMP);
			//			}
			//			break;
			//		default:
			//			return;
			//	}

			//	if (block.GetProp() != null)
			//	{
			//		block.GetProp().gameObject.SetActive(false);
			//	}
			//	if (block.GetMonster() != null)
			//	{
			//		block.GetMonster().gameObject.SetActive(false);
			//	}
			//}

			//void Unapply(CBlockEdit block)
			//{
			//	if (block.GetBlockType() != Def.BlockType.STAIRS)
			//		return;

			//	block.SetBlockType(Def.BlockType.NORMAL);

			//	var layer = m_Layers[block.GetLayer() - 1];
			//	if (layer.IsLinkedLayer)
			//	{
			//		layer = m_Layers[block.GetLinkedTo() - 1];
			//	}

			//	List<IDChance> matFams;
			//	if(layer.LayerType == Def.BiomeLayerType.OTHER)
			//	{
			//		matFams = layer.MaterialFamilies;
			//	}
			//	else if(layer.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
			//	{
			//		matFams = null;
			//	}
			//	else
			//	{
			//		matFams = m_Biome.GetLayers()[(int)layer.LayerType].MaterialFamilies;
			//	}
			//	MaterialFamily family = null;
			//	if(matFams != null && matFams.Count > 0)
			//	{
			//		var familyID = matFams[LayerInfo.RandomFromList(matFams)].ID;
			//		family = BlockMaterial.MaterialFamilies[familyID];
			//	}
			//	block.SetMaterialFamily(family);

			//	if(family == null)
			//	{
			//		block.GetTopMR().material.color = Def.BiomeLayerTypeColors[(int)layer.LayerType];
			//		block.GetMidMR().material.color = Def.BiomeLayerTypeColors[(int)layer.LayerType];
			//	}

			//	if (block.GetProp() != null)
			//	{
			//		block.GetProp().gameObject.SetActive(true);
			//	}
			//	if (block.GetMonster() != null)
			//	{
			//		block.GetMonster().gameObject.SetActive(true);
			//	}
			//}

			//Action<CBlockEdit> fn;
			//if (apply)
			//	fn = Apply;
			//else
			//	fn = Unapply;

			for (int i = 0; i < m_Pilars.Length; ++i)
			{
				var pilar = m_Pilars[i];
				if (pilar == null || pilar.GetBlocks().Count == 0 || (pilar.GetBlocks().Count == 1 && pilar.GetBlocks()[0].GetLayer() == 0))
					continue;

				for(int j = 0; j < pilar.GetBlocks().Count; ++j)
				{
					var block = pilar.GetBlocks()[j] as CBlockEdit;
					if (block.GetLayer() == 0 || block.IsRemoved() || !GameUtils.IsStairPossible(block.GetStairState()) || !block.gameObject.activeSelf)
						continue;

					//fn(block);
					if (block.GetBlockType() != Def.BlockType.NORMAL)
						continue;

					var layer = m_Layers[block.GetLayer() - 1];
					if (layer.IsLinkedLayer)
					{
						layer = m_Layers[block.GetLinkedTo() - 1];
					}

					ushort schance, rchance;
					if (layer.LayerType == Def.BiomeLayerType.OTHER)
					{
						schance = layer.StairBlockChance;
						rchance = layer.RampBlockChance;
					}
					else if (layer.LayerType == Def.BiomeLayerType.OTHER || m_Biome == null)
					{
						schance = rchance = 0;
					}
					else
					{
						var blayer = m_Biome.GetLayers()[(int)layer.LayerType];
						schance = blayer.StairBlockChance;
						rchance = blayer.RampBlockChance;
					}

					switch (block.GetStairState())
					{
						case Def.StairState.POSSIBLE:
							{
								ushort rng = (ushort)(UnityEngine.Random.value * 10000f);
								if (rng > schance)
									continue;
								block.SetStairState(Def.StairState.ALWAYS);
								//block.SetBlockType(Def.BlockType.STAIRS, Def.StairType.NORMAL);
							}
							break;
						case Def.StairState.STAIR_OR_RAMP:
							{
								ushort rng = (ushort)(UnityEngine.Random.value * 10000f);
								if (rng > schance)
									continue;
								ushort rampRNG = (ushort)(UnityEngine.Random.value * 10000f);
								if (rampRNG >= rchance)
									block.SetStairState(Def.StairState.ALWAYS);
									//block.SetBlockType(Def.BlockType.STAIRS, Def.StairType.NORMAL);
								else
									block.SetStairState(Def.StairState.RAMP_ALWAYS);
									//block.SetBlockType(Def.BlockType.STAIRS, Def.StairType.RAMP);
							}
							break;
						case Def.StairState.RAMP_POSSIBLE:
							{
								ushort rng = (ushort)(UnityEngine.Random.value * 10000f);
								if (rng > schance)
									continue;
								block.SetStairState(Def.StairState.RAMP_ALWAYS);
								//block.SetBlockType(Def.BlockType.STAIRS, Def.StairType.RAMP);
							}
							break;
						default:
							continue;
					}

					if (block.GetProp() != null)
					{
						block.GetProp().gameObject.SetActive(false);
					}
					if (block.GetMonster() != null)
					{
						block.GetMonster().gameObject.SetActive(false);
					}
				}
			}
		}
		CBlockEdit GetBlockAtSlice(CPilar pi, float hSlice)
		{
			float baseHeight = 0f; //pi.GetStruc().GetHeight();
			for (int i = 0; i < pi.GetBlocks().Count; ++i)
			{
				var bi = pi.GetBlocks()[i] as CBlockEdit;
				if (bi.GetBlockType() == Def.BlockType.WIDE || bi.IsRemoved())
					continue; // Cannot slice a wide

				var height = baseHeight + bi.GetHeight();

				if (bi.GetBlockType() == Def.BlockType.STAIRS &&
					(/*hSlice == (height - 0.5f) ||*/ hSlice == height || hSlice == (height + 0.5f)))
					break; // Trying to slice the middle of a stair

				var len = bi.GetLength() != 3.4f ? bi.GetLength() : 3.5f;
				var bot = height - len;
				if (height >= hSlice &&     // is the block higher or equal to the wanted height
					(hSlice - 0.5f) >= bot) // it is its bot part below or equal to -0.5f the wanted height
					return bi;
			}
			return null;
		}
		bool IsWideLayerValid(int matFamID, int layerID)
		{
			var layer = m_Layers[layerID - 1];
			if(!layer.IsLinkedLayer)
			{
				if (layer.LayerType == Def.BiomeLayerType.OTHER)
				{
					return GameUtils.ContainsAtChance(layer.MaterialFamilies, matFamID) >= 0;
				}
				else if (layer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
				{
					var bLayer = m_Biome.GetLayers()[(int)layer.LayerType];
					return GameUtils.ContainsAtChance(bLayer.MaterialFamilies, matFamID) >= 0;
				}
				else
				{
					return false;
				}
			}
			for(int i = 0; i < layer.LinkedLayers.Count; ++i)
			{
				var llayer = m_Layers[layer.LinkedLayers[i].ID - 1];
				if (llayer.LayerType == Def.BiomeLayerType.OTHER)
				{
					return GameUtils.ContainsAtChance(llayer.MaterialFamilies, matFamID) >= 0;
				}
				else if (llayer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
				{
					var bLayer = m_Biome.GetLayers()[(int)llayer.LayerType];
					return GameUtils.ContainsAtChance(bLayer.MaterialFamilies, matFamID) >= 0;
				}
			}
			return false;
		}
		CBlockEdit SliceBlock(WideSlice[] slices, int sliceIdx, float curHeight, int side)
		{
			var block = slices[sliceIdx].Blocks[side];
			var baseHeight = 0f; //block.Pilar.GetStruc().GetHeight();
			var blockHeight = block.GetHeight() + baseHeight;
			var blockLength = block.GetLength() != 3.4f ? block.GetLength() : 3.5f;

			if (blockHeight > curHeight) // Slice Top excess part
			{
				var nLen = blockHeight - curHeight;

				var topBI = block.GetPilar().AddBlock();
				int topStackIdx = block.GetStackedBlocksIdx()[1];
				if (topStackIdx >= 0)
					++topStackIdx;
				m_LayerBlocks[block.GetLayer() - 1].Add(topBI);
				block.CopyBlockTo(topBI);
				topBI.SetLength(nLen);
				topBI.GetStackedBlocksIdx()[0] = block.GetPilarIndex();
				topBI._SetPilarIndex(block.GetPilarIndex() + 1);
				block.GetStackedBlocksIdx()[1] = block.GetPilarIndex() + 1;
				topBI.GetStackedBlocksIdx()[1] = topStackIdx;
				block.GetPilar().GetBlocks().Insert(topBI.GetPilarIndex(), topBI);
				block.GetPilar().GetBlocks().RemoveAt(block.GetPilar().GetBlocks().Count - 1);
			}

			var botHeight = blockHeight - blockLength;
			// Slice bot excess part
			if (botHeight < (curHeight - 0.5f) && // is there excess at the bottom
				blockHeight > (curHeight - 0.5f)) // is the block in the slice
			{
				var botExcess = Mathf.Abs((curHeight - 0.5f) - botHeight);
				var botBI = block.GetPilar().AddBlock();
				m_LayerBlocks[block.GetLayer() - 1].Add(botBI);
				block.CopyBlockTo(botBI);
				botBI.SetBlockType(Def.BlockType.NORMAL);
				botBI.SetLength(botExcess);
				var lenDiff = blockLength - botExcess;
				botBI.SetHeight(block.GetHeight() - lenDiff);
				botBI._SetPilarIndex(block.GetPilarIndex());
				block.GetPilar().GetBlocks().Insert(block.GetPilarIndex(), botBI);
				block.GetPilar().GetBlocks().RemoveAt(block.GetPilar().GetBlocks().Count - 1);
				block._SetPilarIndex(block.GetPilarIndex() + 1);
				botBI.GetStackedBlocksIdx()[0] = block.GetStackedBlocksIdx()[0];
				botBI.GetStackedBlocksIdx()[1] = block.GetPilarIndex();
				block.GetStackedBlocksIdx()[0] = botBI.GetPilarIndex();

				// Update the slices with the new block
				for (int i = (sliceIdx + 1); i < slices.Length; ++i)
				{
					if (!slices[i].Possible || slices[i].Blocks[side] != block)
						continue;
					slices[i].Blocks[side] = botBI;
				}
			}

			block.SetLength(0.5f);
			block.SetHeight(curHeight - baseHeight);
			block.SetBlockType(Def.BlockType.NORMAL);

			// Fix PilarIndex and stacking indices
			for (int i = 0; i < block.GetPilar().GetBlocks().Count; ++i)
			{
				var b = block.GetPilar().GetBlocks()[i] as CBlockEdit;

				b._SetPilarIndex(i);

				if (b.GetStackedBlocksIdx()[0] >= 0)
					b.GetStackedBlocksIdx()[0] = i - 1;

				if (b.GetStackedBlocksIdx()[1] >= 0)
					b.GetStackedBlocksIdx()[1] = i + 1;
			}
			return block;
		}
		List<CBlockEdit[]> PlaceWide(CBlockEdit bi, CPilar rPi, CPilar bPi, CPilar rbPi/*, List<IDChance> materials*/)
		{
			var len = bi.GetLength() == 3.4f ? 3.5f : bi.GetLength();
			var baseHeight = 0f;
			var height = baseHeight + bi.GetHeight();
			var sliceCount = Mathf.FloorToInt(len * 2f);
			var slices = new WideSlice[sliceCount];

			int currentSlice = 0;
			if (bi.GetBlockType() == Def.BlockType.STAIRS)
			{
				currentSlice = 1;
				slices[0] = new WideSlice()
				{
					Possible = false,
					Blocks = null
				};
			}
			var matFamID = BlockMaterial.FamilyDict[bi.GetMaterialFamily().FamilyInfo.FamilyName];

			int possible = 0;
			for (; currentSlice < sliceCount; ++currentSlice)
			{
				var sliceBlocks = new CBlockEdit[4];
				sliceBlocks[0] = bi;
				sliceBlocks[1] = GetBlockAtSlice(rPi, height - (currentSlice * 0.5f));
				sliceBlocks[2] = GetBlockAtSlice(bPi, height - (currentSlice * 0.5f));
				sliceBlocks[3] = GetBlockAtSlice(rbPi, height - (currentSlice * 0.5f));


				var slice = new WideSlice()
				{
					Blocks = sliceBlocks,
					Possible = sliceBlocks[0] != null && sliceBlocks[1] != null && sliceBlocks[2] != null && sliceBlocks[3] != null
				};
				if (slice.Possible)
				{
					for (int i = 1; i < 4; ++i)
					{
						var b = sliceBlocks[i];
						if (bi.GetLayer() == b.GetLayer())
							continue;
						if(!IsWideLayerValid(matFamID, b.GetLayer()))
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
				return new List<CBlockEdit[]>(); // No-slice possible

			var doneWides = new List<CBlockEdit[]>(sliceCount);
			void FixPilar(CPilar pilar)
			{
				for (int i = 0; i < pilar.GetBlocks().Count; ++i)
				{
					var block = pilar.GetBlocks()[i];
					block._SetPilarIndex(i);
					if (block.GetStackedBlocksIdx()[0] >= 0)
						block.GetStackedBlocksIdx()[0] = i - 1;
					if (block.GetStackedBlocksIdx()[1] >= 0)
						block.GetStackedBlocksIdx()[1] = i + 1;
				}
			}
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

				//tlBlock.SetWIDE(new CBlockEdit[] { trBlock, blBlock, brBlock });
				tlBlock.SetBlockType(Def.BlockType.WIDE);

				tlBlock.SetHeight(curHeight - baseHeight);
				tlBlock.GetWideStackedIdx()[0] = new WideStackLinks()
				{
					PilarWID = trBlock.GetPilar().GetStructureID(),
					StackIdx = trBlock.GetStackedBlocksIdx().Clone() as int[]
				};
				//var trPindex = trBlock.GetPilarIndex();
				tlBlock.GetWideStackedIdx()[1] = new WideStackLinks()
				{
					PilarWID = blBlock.GetPilar().GetStructureID(),
					StackIdx = blBlock.GetStackedBlocksIdx().Clone() as int[]
				};
				//var blPindex = blBlock.GetPilarIndex();
				tlBlock.GetWideStackedIdx()[2] = new WideStackLinks()
				{
					PilarWID = brBlock.GetPilar().GetStructureID(),
					StackIdx = brBlock.GetStackedBlocksIdx().Clone() as int[]
				};
				//var brPindex = brBlock.GetPilarIndex();

				//trBlock.DestroyBlock(false);
				//blBlock.DestroyBlock(false);
				//brBlock.DestroyBlock(false);
				trBlock.SetRemoved(true);
				blBlock.SetRemoved(true);
				brBlock.SetRemoved(true);
				FixPilar(bi.GetPilar());
				FixPilar(rPi);
				FixPilar(bPi);
				FixPilar(rbPi);

				//rPi.GetBlocks().Insert(trPindex, tlBlock); FixPilar(rPi);
				//bPi.GetBlocks().Insert(blPindex, tlBlock); FixPilar(bPi);
				//rbPi.GetBlocks().Insert(brPindex, tlBlock); FixPilar(rbPi);
				//var materialID = materials[LayerInfo.RandomFromList(materials)].ID;
				//tlBlock.SetMaterialFamily(BlockMaterial.MaterialFamilies[materialID]);
				tlBlock.SetMaterialFamily(bi.GetMaterialFamily());
				doneWides.Add(new CBlockEdit[] { tlBlock, trBlock, blBlock, brBlock });
			}
			return doneWides;
		}
		public List<CBlockEdit> ApplyWides(/*bool apply*/)
		{
			//Action<int> fn;
			//if (apply)
			//{
			//	//fn = ApplyWideChange;
			//	ApplyWideChange();
			//}
			//else
			//{
			//	//fn = UnapplyWideChange;
			//	UnapplyWides();
			//}

			//for (int i = 0; i < Def.MaxLayerSlots; ++i)
			//{
			//    if (!m_Layers[i].IsValid())
			//        continue;
			//    m_WideChangePendingLayers.Enqueue(i + 1);
			//}
			//if(m_WideChangePendingLayers.Count > 0)

			//var availableMaterials = new List<IDChance>();
			//var strucIE = Structures.Strucs[m_IDXIE];

			bool IsInvalidPilar(CPilar pilar) => pilar == null || pilar.GetBlocks().Count == 0 || (pilar.GetBlocks().Count > 0 && pilar.GetBlocks()[0].GetLayer() == 0);

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
			var width = Def.MaxStrucSide;
			var height = Def.MaxStrucSide;
			if(m_IDXIE >= 0)
			{
				var ie = Structures.Strucs[m_IDXIE];
				if(ie != null)
				{
					width = ie.GetWidth();
					height = ie.GetHeight();
				}
			}
			var wideSlices = new List<CBlockEdit[]>();
			for (int i = 0; i < m_Pilars.Length; ++i)
			{
				var tlPilar = m_Pilars[i];
				if(IsInvalidPilar(tlPilar))
					continue;

				var tlPos = VPosFromPilarID(tlPilar.GetStructureID());
				if (tlPos.x >= (width - 1) || tlPos.y >= (height - 1))
					continue;
				//if ((tlPos.x + 1) >= strucIE.GetWidth() || (tlPos.y + 1) >= strucIE.GetHeight())
				//	continue; // Wide outside of the world

				var trPos = new Vector2Int(tlPos.x + 1, tlPos.y);
				var blPos = new Vector2Int(tlPos.x,		tlPos.y + 1);
				var brPos = new Vector2Int(tlPos.x + 1, tlPos.y + 1);

				var trID = PilarIDFromVPos(trPos);
				var blID = PilarIDFromVPos(blPos);
				var brID = PilarIDFromVPos(brPos);

				var trPilar = m_Pilars[trID];
				var blPilar = m_Pilars[blID];
				var brPilar = m_Pilars[brID];

				if (IsInvalidPilar(trPilar) || IsInvalidPilar(blPilar) || IsInvalidPilar(brPilar))
					continue; // No available pilars with blocks

				for (int j = 0; j < tlPilar.GetBlocks().Count; ++j)
				{
					var block = tlPilar.GetBlocks()[j] as CBlockEdit;
					if (block == null || block.GetLayer() == 0 || block.GetBlockType() == Def.BlockType.WIDE || block.IsRemoved())
						continue; // Don't Widify a wide lol

					var matFam = block.GetMaterialFamily();
					if (matFam == null || matFam.GetSet(Def.BlockType.WIDE).Length == 0)
						continue; // Wide material not available for this material family

					var layer = m_Layers[block.GetLayer() - 1];
					if (layer.IsLinkedLayer)
					{
						layer = m_Layers[block.GetLinkedTo() - 1];
					}

					//availableMaterials.Clear();
					ushort wideChance = 0;
					if (layer.LayerType == Def.BiomeLayerType.OTHER)
					{
						wideChance = layer.WideBlockChance;
						//availableMaterials.AddRange(layer.MaterialFamilies);
					}
					else if (layer.LayerType != Def.BiomeLayerType.FULLVOID)
					{
						if (m_Biome != null)
						{
							var biomeLayer = m_Biome.GetLayers()[(int)layer.LayerType];
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
					//			if (llID == block.GetLinkedTo())
					//				continue; // We already checked this layer
					//			var llayer = m_Layers[llID - 1];
					//			if (llayer.LayerType == Def.BiomeLayerType.OTHER)
					//			{
					//				AddElements(availableMaterials, llayer.MaterialFamilies);
					//			}
					//			else if (llayer.LayerType != Def.BiomeLayerType.FULLVOID)
					//			{
					//				if (m_Biome != null)
					//				{
					//					var biomeLayer = m_Biome.GetLayers()[(int)llayer.LayerType];
					//					AddElements(availableMaterials, biomeLayer.MaterialFamilies);
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

					var wideRNG = (ushort)UnityEngine.Random.Range(0, 10000);
					if (wideRNG <= wideChance)
					{
						//GameUtils.UpdateChances2(ref availableMaterials);
						var slices = PlaceWide(block, trPilar, blPilar, brPilar/*, availableMaterials*/);
						wideSlices.AddRange(slices);
					}
				}
			}

			var lastStackIdx = new KeyValuePair<int, CBlockEdit>[4]
					{
						new KeyValuePair<int, CBlockEdit>(-1, null),
						new KeyValuePair<int, CBlockEdit>(-1, null),
						new KeyValuePair<int, CBlockEdit>(-1, null),
						new KeyValuePair<int, CBlockEdit>(-1, null),
					};
			var doneWides = new List<CBlockEdit>();
			for (int i = 0; i < wideSlices.Count; ++i)
			{
				int mergeable = 1;
				var wideBlocks = wideSlices[i];
				var wBlock = wideBlocks[0];
				if (wBlock.GetBlockType() != Def.BlockType.WIDE)
					continue; // has been dewidified

				float baseHeight = wBlock.GetHeight();

				for (int j = i + 1; j < i + 5 && j < wideSlices.Count; ++j)
				{
					var tWideBlocks = wideSlices[j];
					var tWide = tWideBlocks[0];
					if (tWide.GetBlockType() != Def.BlockType.WIDE ||
						tWide.GetPilar() != wBlock.GetPilar() ||
						tWide.GetHeight() != (baseHeight - (j - i) * 0.5f))
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
				if (!lastWBlocks[0].GetPilar().GetBlocks().Contains(lastWBlocks[0]))
					continue;
				for (int j = 0; j < lastStackIdx.Length; ++j)
					lastStackIdx[j] = new KeyValuePair<int, CBlockEdit>(lastWBlocks[j].GetStackedBlocksIdx()[0], null);
				//try
				{
					CBlockEdit b = null;
					if (lastStackIdx[0].Key >= 0)
						b = wBlock.GetPilar().GetBlocks()[lastStackIdx[0].Key] as CBlockEdit;
					lastStackIdx[0] = new KeyValuePair<int, CBlockEdit>(lastStackIdx[0].Key, b);
				}
				//catch(Exception e)
				//{
				//	Debug.Log(e.Message);
				//}

				// Remove in pilar blocks
				for (int j = i + 1; j < (i + mergeable); ++j)
				{
					var slice = wideSlices[j];
					for(int k = 0; k < slice.Length; ++k)
					{
						var block = slice[k];
						block.DestroyBlock(false);
					}
				}

				// Remove other blocks
				for (int j = 1; j < wideBlocks.Length; ++j)
				{
					var pilar = m_Pilars[wBlock.GetWideStackedIdx()[j - 1].PilarWID];
					for (int k = i + 1; k < i + mergeable; ++k)
					{
						if (k == (i + mergeable - 1))
						{
							CBlockEdit b = null;
							if (lastStackIdx[j].Key >= 0)
								b = pilar.GetBlocks()[lastStackIdx[j].Key] as CBlockEdit;
							lastStackIdx[j] = new KeyValuePair<int, CBlockEdit>(lastStackIdx[j].Key, b);
						}
						pilar.RemoveBlock(wideSlices[k][j]);
					}
				}
				// Set length and bottom stacks
				for (int j = 0; j < 4; ++j)
				{
					wideBlocks[j].SetLength(mergeable * 0.5f);
					wideBlocks[j].GetStackedBlocksIdx()[0] = lastStackIdx[j].Key;
					if (lastStackIdx[j].Key >= 0 && lastStackIdx[j].Value != null)
						lastStackIdx[j].Value.GetStackedBlocksIdx()[1] = wideBlocks[j].GetPilarIndex();

					CPilar pilar;
					if (j == 0)
						pilar = wideBlocks[0].GetPilar();
					else
						pilar = m_Pilars[wideBlocks[0].GetWideStackedIdx()[j - 1].PilarWID];
					for (int k = 0; k < pilar.GetBlocks().Count; ++k)
					{
						var block = pilar.GetBlocks()[k];
						block._SetPilarIndex(k);
						if (block.GetStackedBlocksIdx()[0] >= 0)
							block.GetStackedBlocksIdx()[0] = k - 1;
						if (block.GetStackedBlocksIdx()[1] >= 0)
							block.GetStackedBlocksIdx()[1] = k + 1;
					}
				}
				doneWides.Add(wideBlocks[0]);
				i += (mergeable - 2);
			}
			return doneWides;
		}
		public void ApplyVoid(/*bool apply*/)
		{
			for(int i = 0; i < m_Pilars.Length; ++i)
			{
				var pilar = m_Pilars[i];
				if (pilar == null)
					continue;

				if (pilar.GetBlocks().Count == 0 || (pilar.GetBlocks().Count == 1 && pilar.GetBlocks()[0].GetLayer() == 0))
					continue;

				for (int j = 0; j < pilar.GetBlocks().Count; ++j)
				{
					var block = pilar.GetBlocks()[j] as CBlockEdit;
					if (block.GetLayer() == 0 || block.IsRemoved())
						continue;

					switch (block.GetVoidState())
					{
						case Def.BlockVoid.SEMIVOID:
							{
								ushort svChance;
								var layer = m_Layers[block.GetLayer() - 1];
								if (layer.IsLinkedLayer)
								{
									layer = m_Layers[block.GetLinkedTo() - 1];
								}
								if (layer.LayerType == Def.BiomeLayerType.OTHER)
									svChance = layer.SemiVoidChance;
								else if (layer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
									svChance = m_Biome.GetLayers()[(int)layer.LayerType].SemiVoidChance;
								else
									svChance = 0;

								var svRNG = UnityEngine.Random.Range(0, 10000);
								if(svRNG <= svChance)
								{
									var above = block.GetStackedAbove() as CBlockEdit;
									var below = block.GetStackedBelow() as CBlockEdit;

									if (above != null)
										above.GetStackedBlocksIdx()[0] = -1;
									if (below != null)
										below.GetStackedBlocksIdx()[1] = -1;
									block.DestroyBlock(false);
									--j;
								}
							}
							break;
						case Def.BlockVoid.FULLVOID:
							pilar.DestroyPilar(false);
							--i;
							continue;
					}
				}
			}
			//if(apply)
			//{
			//	void Voidify(CBlockEdit block)
			//	{
			//		block.gameObject.SetActive(false);

			//		var above = block.GetStackedAbove();
			//		var below = block.GetStackedBelow();

			//		if (above != null)
			//			above.GetStackedBlocksIdx()[0] = -1;
			//		if (below != null)
			//			below.GetStackedBlocksIdx()[1] = -1;

			//		//if(block.GetStackedBlocks()[0] != null)
			//		//{
			//		//	block.GetStackedBlocks()[0].GetStackedBlocks()[1] = null;
			//		//}
			//		//if(block.GetStackedBlocks()[1] != null)
			//		//{
			//		//	block.GetStackedBlocks()[1].GetStackedBlocks()[0] = null;
			//		//}
			//		int idx = block.GetPilarIndex();//block.GetPilar().GetBlocks().IndexOf(block);
			//		block.GetPilar().RemoveBlock(idx);
			//		//block.GetPilar().GetBlocks().RemoveAt(idx);
			//		m_VoidBlocks.Add(block);
			//		if(block.GetProp() != null)
			//		{
			//			block.GetProp().gameObject.SetActive(false);
			//		}
			//		if(block.GetMonster() != null)
			//		{
			//			block.GetMonster().gameObject.SetActive(false);
			//		}
			//	}
			//	for (int i = 0; i < m_Pilars.Length; ++i)
			//	{
			//		var pilar = m_Pilars[i];
			//		if (pilar == null)
			//			continue;

			//		if (pilar.GetBlocks().Count == 0 || (pilar.GetBlocks().Count == 1 && pilar.GetBlocks()[0].GetLayer() == 0))
			//			continue;

			//		for (int j = 0; j < pilar.GetBlocks().Count; ++j)
			//		{
			//			var block = pilar.GetBlocks()[j] as CBlockEdit;
			//			if (block.GetLayer() == 0 || block.IsRemoved())
			//				continue;

			//			switch (block.GetVoidState())
			//			{
			//				case Def.BlockVoid.SEMIVOID:
			//					{
			//						var layer = m_Layers[block.GetLayer() - 1];
			//						if(layer.IsLinkedLayer)
			//						{
			//							layer = m_Layers[block.GetLinkedTo() - 1];
			//						}

			//						ushort svchance;
			//						if(layer.LayerType == Def.BiomeLayerType.OTHER)
			//						{
			//							svchance = layer.SemiVoidChance;
			//						}
			//						else if(layer.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
			//						{
			//							svchance = 0;
			//						}
			//						else
			//						{
			//							svchance = m_Biome.GetLayers()[(int)layer.LayerType].SemiVoidChance;
			//						}

			//						var rng = (ushort)(UnityEngine.Random.value * 10000f);
			//						if (rng > svchance)
			//							continue;
			//						Voidify(block);
			//						--j;
			//					}
			//					break;
			//				case Def.BlockVoid.FULLVOID:
			//					{
			//						Voidify(block);
			//						--j;
			//					}
			//					break;
			//			}
			//		}
			//	}
			//}
			//else
			//{
			//	for(int i = 0; i < m_VoidBlocks.Count; ++i)
			//	{
			//		var block = m_VoidBlocks[i];
			//		block.gameObject.SetActive(true);
			//		if(block.GetProp() != null)
			//		{
			//			block.GetProp().gameObject.SetActive(true);
			//		}
			//		if(block.GetMonster() != null)
			//		{
			//			block.GetMonster().gameObject.SetActive(true);
			//		}
			//		var above = block.GetStackedAbove();
			//		var below = block.GetStackedBelow();
			//		if (above != null)
			//			above.GetStackedBlocksIdx()[0] = block.GetPilarIndex();
			//		if (below != null)
			//			below.GetStackedBlocksIdx()[1] = block.GetPilarIndex();
			//		//if(block.GetStackedBlocks()[0] != null)
			//		//{
			//		//	block.GetStackedBlocks()[0].GetStackedBlocks()[1] = block;
			//		//}
			//		//if(block.GetStackedBlocks()[1] != null)
			//		//{
			//		//	block.GetStackedBlocks()[1].GetStackedBlocks()[0] = block;
			//		//}
			//		block.GetPilar().GetBlocks().Insert(block.GetPilarIndex(), block);

			//		var lowerBlock = block.GetStackedBelow() as CBlockEdit; //block.GetStackedBlocks()[0] as CBlockEdit;
			//		if(lowerBlock == null)
			//		{
			//			continue;
			//		}
			//		var lowestHeight = block.GetHeight() - block.GetLength();
			//		var diff = lowerBlock.GetHeight() - lowestHeight;
			//		int iterations = Mathf.FloorToInt(diff / 0.5f);
			//		for (int j = 0; j < iterations; ++j)
			//		{
			//			block.IncreaseHeight();
			//			//block.SetHeight(block.GetHeight() + 0.5f);
			//			//block.IncreaseHeightCheck();
			//		}
			//		block.SetMicroHeight(lowerBlock.GetMicroHeight());
			//	}
			//	m_VoidBlocks.Clear();
			//}
		}
		public void ApplyMicroheight(List<CBlockEdit> wideBlocks)
		{
			void MicroheightDown(CBlockEdit b, CPilar p, float mh)
			{
				b.SetMicroHeight(mh);
				b.SetMicroheightApplied(true);
				if (b.GetBlockType() == Def.BlockType.WIDE)
				{
					for (int i = 0; i < b.GetWideStackedIdx().Length; ++i)
					{
						var pilar = m_Pilars[b.GetWideStackedIdx()[i].PilarWID];
						if (pilar == null)
						{
							Debug.LogWarning("Couldn't find WideStackedPilar");
							continue;
						}
						CBlockEdit hidden = null;
						for (int j = 0; j < pilar.GetBlocks().Count; ++j)
						{
							var block = pilar.GetBlocks()[j] as CBlockEdit;
							if (block.GetHeight() == b.GetHeight() && block.IsRemoved())
							{
								hidden = block;
								break;
							}
						}
						if (hidden == null)
							continue;

						hidden.SetMicroHeight(mh);
						hidden.SetMicroheightApplied(true);
						var hIdx = pilar.GetBlocks().IndexOf(hidden);
						CBlockEdit hiddenBelow = null;
						if (hidden.GetStackedBlocksIdx()[0] >= 0 && hIdx > 0)
							hiddenBelow = pilar.GetBlocks()[hIdx - 1] as CBlockEdit;
						if (hiddenBelow != null)
							MicroheightDown(hiddenBelow, pilar, mh);
					}
				}

				var idx = p.GetBlocks().IndexOf(b);
				CBlockEdit below = null;
				if (b.GetStackedBlocksIdx()[0] >= 0 && idx > 0)
				{
					below = p.GetBlocks()[idx - 1] as CBlockEdit;
				}
				if (below != null)
					MicroheightDown(below, p, mh);
			}
			void MicroheightUp(CBlockEdit b, CPilar p, float mh)
			{
				b.SetMicroHeight(mh);
				b.SetMicroheightApplied(true);
				if (b.GetBlockType() == Def.BlockType.WIDE)
				{
					for (int i = 0; i < b.GetWideStackedIdx().Length; ++i)
					{
						var pilar = m_Pilars[b.GetWideStackedIdx()[i].PilarWID];
						if (pilar == null)
						{
							Debug.LogWarning("Couldn't find WideStackedPilar");
							continue;
						}
						CBlockEdit hidden = null;
						for (int j = 0; j < pilar.GetBlocks().Count; ++j)
						{
							var block = pilar.GetBlocks()[j] as CBlockEdit;
							if (block.GetHeight() == b.GetHeight() && block.IsRemoved())
							{
								hidden = block;
								break;
							}
						}
						if (hidden == null)
							continue;

						hidden.SetMicroHeight(mh);
						hidden.SetMicroheightApplied(true);
						var hIdx = pilar.GetBlocks().IndexOf(hidden);
						CBlockEdit hiddenBelow = null;
						if (hidden.GetStackedBlocksIdx()[1] >= 0 && hIdx >= 0 && pilar.GetBlocks().Count > (hIdx + 1))
							hiddenBelow = pilar.GetBlocks()[hIdx + 1] as CBlockEdit;
						if (hiddenBelow != null)
							MicroheightUp(hiddenBelow, pilar, mh);
					}
				}

				var idx = p.GetBlocks().IndexOf(b);
				CBlockEdit below = null;
				if (b.GetStackedBlocksIdx()[1] >= 0 && idx >= 0 && p.GetBlocks().Count > (idx + 1))
				{
					below = p.GetBlocks()[idx + 1] as CBlockEdit;
				}
				if (below != null)
					MicroheightUp(below, p, mh);
			}

			void ApplyMicroheight(CBlockEdit b, CPilar p, float mh)
			{
				//if (b.MicroheightApplied)
				//	return;
				var idx = p.GetBlocks().IndexOf(b);
				CBlockEdit below = null;
				if (idx > 0 && b.GetStackedBlocksIdx()[0] >= 0)
					below = p.GetBlocks()[idx - 1] as CBlockEdit;
				//BlockInfo above = null;
				//if (b.StackedIdx[1] >= 0 && idx >= 0 && p.GetBlocks().Count > (idx + 1))
				//	above = p.GetBlocks()[idx + 1];

				if (b.GetBlockType() == Def.BlockType.WIDE || b.IsRemoved())
				{
					//if (above != null)
					//	ApplyMicroheight(above, p, mh);
					if (below != null)
						ApplyMicroheight(below, p, mh);
					return;
				}

				b.SetMicroHeight(mh);
				b.SetMicroheightApplied(true);

				//if (above != null)
				//	ApplyMicroheight(above, p, mh);
				if (below != null)
					ApplyMicroheight(below, p, mh);
			}

			for (int i = 0; i < wideBlocks.Count; ++i)
			{
				var wide = wideBlocks[i];
				if (wide == null || wide.IsMicroheightApplied())
					continue;

				// get the top stacked block
				var pilar = wide.GetPilar();
				var wideIdx = pilar.GetBlocks().IndexOf(wide);
				if (wideIdx < 0)
					continue;

				CBlockEdit topBlock = wide;
				var topIdx = wideIdx;
				//try
				//{
				while (topBlock.GetStackedBlocksIdx()[1] >= 0 && topIdx < pilar.GetBlocks().Count)
				{
					topBlock = pilar.GetBlocks()[topIdx++] as CBlockEdit;
				}
				//}catch(Exception e)
				//{
				//	Debug.Log(e.Message);
				//}
				// obtain the microheight for that block

				var layer = m_Layers[topBlock.GetLayer() - 1];
				if (layer.IsLinkedLayer)
					layer = m_Layers[topBlock.GetLinkedTo() - 1];
				float maxMicroHeigth, minMicroHeight;
				if (layer.LayerType == Def.BiomeLayerType.OTHER)
				{
					maxMicroHeigth = layer.MicroHeightMax;
					minMicroHeight = layer.MicroHeightMin;
				}
				else if (layer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
				{
					var bLayer = m_Biome.GetLayers()[(int)layer.LayerType];
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
					var rngVal = UnityEngine.Random.Range(Mathf.FloorToInt(minMicroHeight * 20f/*/ 0.05f*/),
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

			for(int i = 0; i < m_Pilars.Length; ++i)
			{
				var pilar = m_Pilars[i];
				if (pilar == null)
					continue;

				for (int j = 0; j < pilar.GetBlocks().Count; ++j)
				{
					var block = pilar.GetBlocks()[j] as CBlockEdit;
					if (block == null || block.GetLayer() == 0 || block.GetBlockType() == Def.BlockType.WIDE || block.IsRemoved() || block.IsMicroheightApplied() || block.IsStackLinkValid(1))
						continue;

					var layer = m_Layers[block.GetLayer() - 1];
					if (layer.IsLinkedLayer)
						layer = m_Layers[block.GetLinkedTo() - 1];
					float maxMicroHeigth, minMicroHeight;
					if (layer.LayerType == Def.BiomeLayerType.OTHER)
					{
						maxMicroHeigth = layer.MicroHeightMax;
						minMicroHeight = layer.MicroHeightMin;
					}
					else if (layer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
					{
						var bLayer = m_Biome.GetLayers()[(int)layer.LayerType];
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
						var rngVal = UnityEngine.Random.Range(Mathf.FloorToInt(minMicroHeight * 20f/*/ 0.05f*/),
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

			//for (int i = 0; i < m_Pilars.Length; ++i)
			//{
			//	var pilar = m_Pilars[i];
			//	if (pilar == null)
			//		continue;
			//	for (int j = 0; j < pilar.GetBlocks().Count; ++j)
			//	{
			//		var block = pilar.GetBlocks()[j] as CBlockEdit;
			//		if (block == null || block.GetLayer() == 0)
			//			continue;

			//		if (!block.IsStackLinkValid(0))
			//		{
			//			var layer = m_Layers[block.GetLayer() - 1];
			//			if (layer.IsLinkedLayer)
			//			{
			//				layer = m_Layers[block.GetLinkedTo() - 1];
			//			}

			//			float microHeightMin;
			//			float microHeightMax;
			//			if (layer.LayerType == Def.BiomeLayerType.OTHER)
			//			{
			//				microHeightMin = layer.MicroHeightMin;
			//				microHeightMax = layer.MicroHeightMax;
			//			}
			//			else if (layer.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
			//			{
			//				microHeightMin = 0f;
			//				microHeightMax = 0f;
			//			}
			//			else
			//			{
			//				microHeightMin = m_Biome.GetLayers()[(int)layer.LayerType].MicroHeightMin;
			//				microHeightMax = m_Biome.GetLayers()[(int)layer.LayerType].MicroHeightMax;
			//			}
			//			float nmHeight;
			//			if (microHeightMin != microHeightMax)
			//			{
			//				var rngVal = UnityEngine.Random.Range(Mathf.FloorToInt(microHeightMin * 20f/*/ 0.05f*/),
			//					Mathf.FloorToInt(microHeightMax * 20f/*/ 0.05f*/) + 1);
			//				nmHeight = rngVal * 0.05f;
			//			}
			//			else
			//			{
			//				nmHeight = microHeightMin;
			//			}
			//			if (nmHeight != block.GetMicroHeight())
			//				block.ChangeMicroheight(nmHeight);
			//		}
			//	}
			//}
		}
		public void ApplyPropsMonsters()
		{
			for (int i = 0; i < m_Pilars.Length; ++i)
			{
				var pilar = m_Pilars[i];
				if (pilar == null)
					continue;
				for (int j = 0; j < pilar.GetBlocks().Count; ++j)
				{
					var block = pilar.GetBlocks()[j] as CBlockEdit;
					if (block == null || block.GetLayer() == 0)
						continue;

					if (block.IsStackLinkValid(1))
						continue;

					var layer = m_Layers[block.GetLayer() - 1];
					if (layer.IsLinkedLayer)
					{
						layer = m_Layers[block.GetLinkedTo() - 1];
					}
					var blockIDXIE = block.GetIDXIE();
					var blockIE = Structures.Strucs[m_IDXIE].GetBlocks()[blockIDXIE];
					// Monsters
					{
						if (block.GetMonster() != null)
						{
							block.GetMonster().GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, block.GetMonster().GetLE().GetCurrentHealth());
							block.SetMonster(null);
						}
						List<IDChance> monFams;
						ushort mchance;
						if (layer.LayerType == Def.BiomeLayerType.OTHER)
						{
							monFams = layer.MonsterFamilies;
							mchance = layer.MonsterGeneralChance;
						}
						else if (layer.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
						{
							monFams = null;
							mchance = 0;
						}
						else
						{
							var blayer = m_Biome.GetLayers()[(int)layer.LayerType];
							monFams = blayer.MonsterFamilies;
							mchance = blayer.MonsterGeneralChance;
						}

						if (monFams != null && monFams.Count > 0 && mchance > 0 && block.GetBlockType() != Def.BlockType.STAIRS)
						{
							if (UnityEngine.Random.Range(0, 10000) <= mchance)
							{
								int mid = monFams[LayerInfo.RandomFromList(monFams)].ID;
								var mon = new GameObject().AddComponent<AI.CMonster>();
								mon.SetMonster(Monsters.MonsterFamilies[mid]);
								mon.transform.position = block.GetPilar().transform.position +
									new Vector3(UnityEngine.Random.value, block.GetHeight() + block.GetMicroHeight(), UnityEngine.Random.value);
								mon.enabled = Manager.Mgr.HideInfo;
								m_LES.Add(mon.GetLE());
								mon.GetLE().GetStatusBars().gameObject.SetActive(false);
								mon.GetLE().GetCollider().enabled = false;
								mon.GetLE().OnEntityDeath += OnEntityDeath;
								block.SetMonster(mon);
							}
						}
					}
					// Props
					if (!blockIE.GetFlag(IE.V4.BlockIE.Flag.Prop) /*flags[(int)IE.V4.BlockIE.Flag.Prop]*/)
					{
						bool spawnProp = block.GetMonster() == null
							&& !block.IsStackLinkValid(1);// block.GetStackedBlocks()[1] == null;

						float pdist;
						ushort pchance;
						List<IDChance> propFams;
						if (layer.LayerType == Def.BiomeLayerType.OTHER)
						{
							pchance = layer.PropGeneralChance;
							pdist = layer.PropSafetyDistance;
							propFams = layer.PropFamilies;
						}
						else if(layer.LayerType != Def.BiomeLayerType.FULLVOID && m_Biome != null)
						{
							var bLayer = m_Biome.GetLayers()[(int)layer.LayerType];
							pchance = bLayer.PropGeneralChance;
							pdist = bLayer.PropSafetyDistance;
							propFams = bLayer.PropFamilies;
						}
						else
						{
							pchance = 0;
							pdist = 0f;
							propFams = null;
						}
						if (propFams != null && propFams.Count == 0)
							pchance = 0;

						if (pdist > 0f && spawnProp && pchance > 0)
						{
							var layerBlocks = m_LayerBlocks[block.GetLayer() - 1];
							var mHeight = block.GetHeight() + block.GetMicroHeight();
							for (int k = 0; k < layerBlocks.Count; ++k)
							{
								var oblock = layerBlocks[k];
								var oProp = oblock.GetProp();
								if (oblock == block || oProp == null)
									continue;

								var oid = oblock.GetPilar().GetStructureID();
								var mid = block.GetPilar().GetStructureID();

								var oPos = VPosFromPilarID(oid);
								var mPos = VPosFromPilarID(mid);

								if (Vector2.Distance(oPos, mPos) <= pdist)
								{
									if (Mathf.Abs(mHeight - (oblock.GetHeight() + oblock.GetMicroHeight())) > pdist)
										continue;

									spawnProp = false;
									break;
								}
							}
						}
						if (block.GetProp() != null)
						{
							block.GetProp().GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, block.GetProp().GetLE().GetCurrentHealth());
						}

						if (pchance > 0 && spawnProp && block.GetBlockType() != Def.BlockType.STAIRS && propFams != null)
						{
							if (UnityEngine.Random.Range(0, 10000) <= pchance)
							{
								int rnd = LayerInfo.RandomFromList(propFams);
								int familyID = propFams[rnd].ID;
								var propFamily = Props.PropFamilies[familyID];
								if (propFamily.Props.Count > 0)
								{
									var propID = UnityEngine.Random.Range(0, propFamily.Props.Count);
									var prop = new GameObject().AddComponent<AI.CProp>();
									prop.SetProp(familyID, propID);
									//prop.SetBlock(block);
									prop.GetLE().SetCurrentBlock(block);
									block.SetProp(prop);
									m_LES.Add(prop.GetLE());
									prop.GetSprite().Flip(UnityEngine.Random.value > 0.5f, false);
									prop.GetLE().OnEntityDeath += OnEntityDeath;
									prop.enabled = Manager.Mgr.HideInfo;
								}
								else
								{
									Debug.LogWarning("Trying to spawn a prop from a family with no childs.");
								}
							}
						}
					}
				}
			}
		}
		void FixWides()
		{
			var fixedWides = new List<CBlockEdit>();
			for (int i = 0; i < m_Pilars.Length; ++i)
			{
				var pilar = m_Pilars[i];
				for (int j = 0; j < pilar.GetBlocks().Count; ++j)
				{
					var block = (CBlockEdit)pilar.GetBlocks()[j];
					if (block.GetBlockType() != Def.BlockType.WIDE)
						continue;
					if (fixedWides.Contains(block))
						continue;
					var blocks = new CBlockEdit[4];
					blocks[0] = block;
					int lastIdx = 1;
					foreach (var hBlock in block.GetHiddenBlocks())
						blocks[lastIdx++] = hBlock;

					Vector2Int minPosBlock = new Vector2Int(int.MaxValue, int.MaxValue);
					CBlockEdit newWide = null;
					foreach (var hblock in blocks)
					{
						var pos = VPosFromPilarID(hblock.GetPilar().GetStructureID());
						if (minPosBlock.x >= pos.x && minPosBlock.y >= pos.y)
						{
							minPosBlock = pos;
							newWide = hblock;
						}
					}
					lastIdx = 0;
					var hiddenBlocks = new CBlockEdit[3];
					foreach (var hblock in blocks)
					{
						if (hblock != newWide)
							hiddenBlocks[lastIdx++] = hblock;
					}

					block.SetBlockType(Def.BlockType.NORMAL);
					newWide.SetWIDE(hiddenBlocks);
					fixedWides.Add(newWide);
				}
			}
		}
		void Rotate90()
		{
			void RotateBlocks(CPilar pilar)
			{
				foreach (CBlockEdit block in pilar.GetBlocks())
				{
					switch (block.GetRotation())
					{
						case Def.RotationState.Default:
							block.SetRotation(Def.RotationState.Right);
							break;
						case Def.RotationState.Right:
							block.SetRotation(Def.RotationState.Half);
							break;
						case Def.RotationState.Half:
							block.SetRotation(Def.RotationState.Left);
							break;
						case Def.RotationState.Left:
							block.SetRotation(Def.RotationState.Default);
							break;
					}
				}
			}
			for (int x = 0; x < Def.MaxStrucSide / 2; ++x)
			{
				for (int y = x; y < Def.MaxStrucSide - x - 1; ++y)
				{
					var tID = PilarIDFromVPos(new Vector2Int(x, y));
					var rID = PilarIDFromVPos(new Vector2Int(y, Def.MaxStrucSide - 1 - x));
					var bID = PilarIDFromVPos(new Vector2Int(Def.MaxStrucSide - 1 - x, Def.MaxStrucSide - 1 - y));
					var lID = PilarIDFromVPos(new Vector2Int(Def.MaxStrucSide - 1 - y, x));

					var tlPilar = m_Pilars[tID];
					var rtPilar = m_Pilars[rID];
					var brPilar = m_Pilars[bID];
					var lbPilar = m_Pilars[lID];

					m_Pilars[lID] = tlPilar;
					tlPilar.ChangeID(lID);
					RotateBlocks(tlPilar);

					m_Pilars[tID] = rtPilar;
					rtPilar.ChangeID(tID);
					RotateBlocks(rtPilar);

					m_Pilars[rID] = brPilar;
					brPilar.ChangeID(rID);
					RotateBlocks(brPilar);

					m_Pilars[bID] = lbPilar;
					lbPilar.ChangeID(bID);
					RotateBlocks(lbPilar);
				}
			}
			
			FixWides();
		}
		void FlipHorz()
		{
			void RotateBlocks(CPilar pilar)
			{
				foreach (CBlockEdit block in pilar.GetBlocks())
				{
					if (block.GetRotation() == Def.RotationState.Left)
						block.SetRotation(Def.RotationState.Right);
					else if (block.GetRotation() == Def.RotationState.Right)
						block.SetRotation(Def.RotationState.Left);
				}
			}
			for (int y = 0; y < (Def.MaxStrucSide / 2); ++y)
			{
				for (int x = 0; x < Def.MaxStrucSide; ++x)
				{
					int nY = (Def.MaxStrucSide - 1) - y; // Flipped Y
					int currentID = PilarIDFromVPos(new Vector2Int(x, y));
					int flippedID = PilarIDFromVPos(new Vector2Int(x, nY));
					InterchangePilars(currentID, flippedID);
					RotateBlocks(m_Pilars[currentID]);
					RotateBlocks(m_Pilars[flippedID]);
				}
			}

			FixWides();
		}
		void FlipVert()
		{
			void RotateBlocks(CPilar pilar)
			{
				foreach (CBlockEdit block in pilar.GetBlocks())
				{
					if (block.GetRotation() == Def.RotationState.Default)
						block.SetRotation(Def.RotationState.Half);
					else if (block.GetRotation() == Def.RotationState.Half)
						block.SetRotation(Def.RotationState.Default);
				}
			}
			for (int y = 0; y < Def.MaxStrucSide; ++y)
			{
				for (int x = 0; x < (Def.MaxStrucSide / 2); ++x)
				{
					int nX = (Def.MaxStrucSide - 1) - x; // Flipped X
					int currentID = PilarIDFromVPos(new Vector2Int(x, y));
					int flippedID = PilarIDFromVPos(new Vector2Int(nX, y));
					InterchangePilars(currentID, flippedID);
					RotateBlocks(m_Pilars[currentID]);
					RotateBlocks(m_Pilars[flippedID]);
				}
			}
			
			FixWides();
		}
		public void ApplyModifier(Def.StrucMod modifier)
		{
			switch (modifier)
			{
				case Def.StrucMod.HorzFlip:
					FlipHorz();
					break;
				case Def.StrucMod.VertFlip:
					FlipVert();
					break;
				case Def.StrucMod.Rotated90:
					Rotate90();
					break;
			}
			m_Mods.Add(modifier);
		}
		private void Awake()
		{
			var pos = GameUtils.TransformPosition(new Vector2(transform.position.x, transform.position.z));
			m_Bounds = new RectInt(pos.x, pos.y, 
				Def.MaxStrucSide, Def.MaxStrucSide);
			m_IDXIE = -1;
			m_Pilars = new CPilar[Def.MaxStrucSide * Def.MaxStrucSide];
			m_Mods = new List<Def.StrucMod>();
			m_Layers = new LayerInfo[Def.MaxLayerSlots];
			m_LayerBlocks = new List<CBlockEdit>[Def.MaxLayerSlots];
			for (int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				m_Layers[i] = LayerInfo.GetDefaultLayer();
				m_Layers[i].Slot = i + 1;
				//m_Layers[i].LayerType = (i > (int)Def.BiomeLayerType.OTHER) ? Def.BiomeLayerType.OTHER : (Def.BiomeLayerType)i;
				m_LayerBlocks[i] = new List<CBlockEdit>();
			}
			//m_LivingEntities = new List<LivingEntity>();
			m_LES = new List<AI.CLivingEntity>();
			m_WideConvertedPilars = new List<CPilar>();
			m_WidePreviousPilars = new List<CPilar>();
			m_VoidBlocks = new List<CBlockEdit>();
			m_WideBlocks = new List<CBlockEdit>();
			m_FlagsToRestore = new Dictionary<int, List<bool>>();
		}
		public override int GetWidth()
		{
			return Def.MaxStrucSide;
		}
		public override int GetHeight()
		{
			return Def.MaxStrucSide;
		}
		public void DuplicatePilars()
		{
			if(m_DuplicatedPilars == null || m_DuplicatedPilars.Length != m_Pilars.Length)
			{
				m_DuplicatedPilars = new CPilar[m_Pilars.Length];
			}
			var voidFamily = BlockMaterial.VoidMat[(int)Def.BlockType.NORMAL].Family;
			for (int i = 0; i < m_Pilars.Length; ++i)
			{
				var oPilar = m_Pilars[i];
				if(oPilar == null)
				{
					m_DuplicatedPilars[i] = null;
					continue;
				}

				var nPilar = new GameObject().AddComponent<CPilar>();
				nPilar.ChangeStruc(this, oPilar.GetStructureID());
				//nPilar.GetBlocks().AddRange(Enumerable.Repeat<IBlock>(null, oPilar.GetBlocks().Count));
				nPilar.GetBlocks().Capacity = oPilar.GetBlocks().Count;

				for(int j = 0; j < oPilar.GetBlocks().Count; ++j)
				{
					var oBlock = oPilar.GetBlocks()[j] as CBlockEdit;
					if(oBlock == null)
					{
						nPilar.GetBlocks()[j] = null;
						continue;
					}
					
					var nBlock = nPilar.AddBlock();
					if (oBlock.GetLayer() != 0)
					{
						nBlock.SetIDXIE(oBlock.GetIDXIE());
					}
					int layer = oBlock.GetLayer();
					if(layer == 0)
					{
						nBlock.SetMaterialFamily(voidFamily);
						nBlock.SetLayer(layer);
						continue;
					}

					if (!m_LayerBlocks[layer - 1].Contains(nBlock))
						m_LayerBlocks[layer - 1].Add(nBlock);

					nBlock.SetLayer(layer);
					if(m_Layers[layer - 1].IsLinkedLayer)
					{
						int nLIIdx = m_Layers[layer - 1].LinkedLayers[LayerInfo.RandomFromList(m_Layers[layer - 1].LinkedLayers)].ID;
						nBlock.SetLinkedTo(nLIIdx);
					}
					nBlock.SetMaterialFamily(oBlock.GetMaterialFamily());
					nBlock.SetHeight(oBlock.GetHeight());
					nBlock.SetMicroHeight(oBlock.GetMicroHeight());
					nBlock.SetLength(oBlock.GetLength());
					nBlock.SetStairState(oBlock.GetStairState());
					nBlock.SetVoidState(oBlock.GetVoidState());
					nBlock.SetLockState(oBlock.GetLockState());
					switch (oBlock.GetBlockType())
					{
						case Def.BlockType.NORMAL:
							if (oBlock.GetTopMaterial() != null && oBlock.GetMaterialFamily() != null)
							{
								nBlock.SetTopMaterial(oBlock.GetTopMaterial());
								nBlock.SetMidMaterial(oBlock.GetMidMaterial());
							}
							break;
						case Def.BlockType.STAIRS:
							nBlock.SetBlockType(oBlock.GetBlockType(), oBlock.GetStairType());
							break;
						case Def.BlockType.WIDE:
							nBlock.SetWIDE(oBlock.GetHiddenBlocks());
							break;
					}
					nBlock.SetRotation(oBlock.GetRotation());
					nBlock.GetStackedBlocksIdx()[0] = oBlock.GetStackedBlocksIdx()[0];
					nBlock.GetStackedBlocksIdx()[1] = oBlock.GetStackedBlocksIdx()[1];
				}

				nPilar.gameObject.SetActive(false);
				m_DuplicatedPilars[i] = nPilar;
			}
		}
		public void RestorePilars()
		{
			for(int i = 0; i < m_Pilars.Length; ++i)
			{
				var oPilar = m_Pilars[i];
				if(oPilar != null)
				{
					oPilar.DestroyPilar(false);
				}
				m_Pilars[i] = m_DuplicatedPilars[i];
				if (m_Pilars[i] == null)
					continue;
				m_Pilars[i].gameObject.SetActive(true);
				m_DuplicatedPilars[i] = null;
			}
		}
		public override CPilar[] GetPilars() => m_Pilars;
		public override RectInt GetBounds()
		{
			m_Bounds.position = GameUtils.TransformPosition(new Vector2(transform.position.x, transform.position.z));
			return m_Bounds;
		}
		public override Rect GetBoundsF()
		{
			const float blockSep = 1f * Def.BlockSeparation;
			return new Rect(transform.position.x, transform.position.z, GetWidth() * blockSep, GetHeight() * blockSep);
		}
		//public override List<LivingEntity> GetLivingEntities()
		//{
		//	return m_LivingEntities;
		//}
		public static CStrucEdit CreateDefaultStruc(int width = Def.DefaultStrucSide,
			int height = Def.DefaultStrucSide)
		{
			//if (width > Def.MaxStrucSide || width < Def.MinStrucSide ||
			//	height > Def.MaxStrucSide || height < Def.MinStrucSide)
			//	throw new Exception($"Trying to create a struc with invalid size {width}x{height}");
			width = Mathf.Clamp(width, Def.MinStrucSide, Def.MaxStrucSide);
			height = Mathf.Clamp(height, Def.MinStrucSide, Def.MaxStrucSide);

			// Center the struc
			int startingX = Def.MaxStrucSide / 2 - width / 2;
			int startingY = Def.MaxStrucSide / 2 - height / 2;

			var struc = new GameObject("TempStruct").AddComponent<CStrucEdit>();
			var voidFamily = BlockMaterial.VoidMat[(int)Def.BlockType.NORMAL].Family;

			for (int y = startingY; y < (startingY + height); ++y)
			{
				var yOffset = y * Def.MaxStrucSide;
				for (int x = startingX; x < (startingX + width); ++x)
				{
					var pilarID = yOffset + x;
					var pilar = new GameObject(struc.gameObject.name + "_TempPilar").AddComponent<CPilar>();
					struc.m_Pilars[pilarID] = pilar;
					pilar.ChangeStruc(struc, pilarID);
					var block = pilar.AddBlock();
					block.SetMaterialFamily(voidFamily);
					block.GetTopMR().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
					block.GetTopMR().receiveShadows = true;
					block.GetMidMR().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
					block.GetMidMR().receiveShadows = true;
				}
			}
			return struc;
		}
		public static CStrucEdit LoadStruc(int id, bool copyStruc = false)
		{
			IE.V4.StructureIE ie;
			CStrucEdit struc;
			if(id < 0)
			{
				struc = CreateDefaultStruc();
				struc.IDXIE = Structures.AddStructure();
				ie = Structures.Strucs[struc.IDXIE];
				ie.SetWidth(Def.DefaultStrucSide);
				ie.SetHeight(Def.DefaultStrucSide);
				return struc;
			}

			if(copyStruc)
			{
				var oIE = Structures.Strucs[id];
				var nID = Structures.AddStructure();
				ie = new IE.V4.StructureIE(oIE);
				Structures.Strucs[nID] = ie;
				ie.StructureID = (ushort)nID;
				var oName = ie.GetName();
				if (oName.Length > 0)
				{
					var cleanName = GameUtils.CleanUniqueName(oName);
					string testName = GameUtils.GenerateUniqueName(cleanName, 3, (string toTest) => !Structures.StrucDict.ContainsKey(toTest));
					//int nameID = 0;
					//string testName = cleanName + '_' + nameID.ToString().PadLeft(3, '0');
					//while (Structures.StrucDict.ContainsKey(testName))
					//{
					//	++nameID;
					//	testName = cleanName + '_' + nameID.ToString().PadLeft(3, '0');
					//}
					ie.SetName(testName);
				}
			}
			else
			{
				ie = Structures.Strucs[id];
			}

			struc = CreateDefaultStruc(ie.GetWidth(), ie.GetHeight());
			struc.IDXIE = ie.StructureID;
			var blocks = ie.GetBlocks();

			var sBlocks = new List<CBlockEdit>(blocks.Length);

			// Add the blocks and stacked blocks and assign its layers
			for (int i = 0; i < blocks.Length; ++i)
			{
				var blockIE = blocks[i];
				if (blockIE == null)
					continue;
				var strucID = (int)blockIE.StructureID;
				if (strucID >= struc.m_Pilars.Length)
				{
					Debug.LogWarning($"Trying to load a Strucuture which has a block {i} with with invalid StructureID {blockIE.StructureID}");
					continue;
				}
				var pilar = struc.m_Pilars[strucID];
				CBlockEdit block;
				if (pilar.GetBlocks().Count == 1 && (pilar.GetBlocks()[0] as CBlockEdit).GetIDXIE() < 0)
				{
					block = pilar.GetBlocks()[0] as CBlockEdit;
				}
				else
				{
					//var fBlock = pilar.GetBlocks()[0];
					//var pBlock = pilar.GetBlocks()[pilar.GetBlocks().Count - 1];
					block = pilar.AddBlock();
					//var height = blockIE.GetHeight();
					//block.SetHeight(height);
					//block._CheckStackedLinks();
					//if (blockIE.GetFlag(IE.V3.BlockIE.Flag.Height))
					//{
					//	//block.SetHeight(blockIE.Height);
					//	var diff = blockIE.Height - block.GetHeight();
					//	bool increase = diff >= 0f;
					//	Action fn;
					//	if(increase)
					//	{
					//		fn = () => block.IncreaseHeight();
					//	}
					//	else
					//	{
					//		diff = -diff;
					//		fn = () => block.DecreaseHeight();
					//	}

					//	for (float step = 0f; step < diff; step += 0.5f)
					//	{
					//		fn();
					//		//block.SetHeight(block.GetHeight() + 0.5f * mult);
					//		//if (increase)
					//		//	block.IncreaseHeightCheck();
					//		//else
					//		//	block.DecreseHeightCheck();
					//	}
					//}
					//else
					//{
					//	block.SetHeight(pBlock.GetHeight() + 0.5f);
					//	block.SetMicroHeight(fBlock.GetMicroHeight());
					//	block.GetStackedBlocksIdx()[0] = pBlock.GetPilarIndex();
					//	pBlock.GetStackedBlocksIdx()[1] = block.GetPilarIndex();
					//	//block.GetStackedBlocks()[0] = pBlock;
					//	//pBlock.GetStackedBlocks()[1] = block;
					//}
					//if(blockIE.GetFlag(IE.V4.BlockIE.Flag.Length))
					//{
					//	block.SetLength(blockIE.GetLength());
						//block._CheckStackedLinks();
						//var diff = (blockIE.Length == 3.4f ? 3.5f : blockIE.Length) - (block.GetLength() == 3.4f ? 3.5f : block.GetLength());
						//bool increase = diff >= 0;
						//Action fn;
						//if(increase)
						//{
						//	fn = () => block.IncreaseLength();
						//}
						//else
						//{
						//	diff = -diff;
						//	fn = () => block.DecreaseLength();
						//}

						//for (float step = 0f; step < diff; step += 0.5f)
						//{
						//	fn();
						//	//float amount = 0.5f;
						//	//if ((block.GetLength() == 3.4f && !increase) || (block.GetLength() == 3f && increase))
						//	//	amount = 0.4f;
						//	//block.SetLength(block.GetLength() + amount * mult);
						//	//if (increase)
						//	//	block.IncreaseLengthCheck();
						//	//else
						//	//	block.DecreseLengthCheck();
						//}
				}
				//}
				block.SetIDXIE(i);
				//block.SetLayer(blockIE.Layer);
				sBlocks.Add(block);
			}

			var layers = ie.GetLayers();
			for (int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				if (layers[i] == null)
					continue;
				struc.SetLayer(layers[i].ToLayerInfo());
			}

			CBlockEdit FindBLock(CPilar pilar, int idxie)
			{
				for(int i = 0; i < pilar.GetBlocks().Count; ++i)
				{
					var block = pilar.GetBlocks()[i] as CBlockEdit;
					if (block.GetIDXIE() == idxie)
						return block;
				}
				return null;
			}

			for(int i = 0; i < sBlocks.Count; ++i)
			{
				var block = sBlocks[i];
				var pilar = block.GetPilar();
				var blockIE = blocks[block.GetIDXIE()];
				blockIE.Apply(block);

				if(blockIE.StackedIdx[0] >= 0)
				{
					var below = FindBLock(pilar, blockIE.StackedIdx[0]);
					if(below != null)
					{
						block.GetStackedBlocksIdx()[0] = below.GetPilarIndex();
						below.GetStackedBlocksIdx()[1] = block.GetPilarIndex();
						var belowHeight = below.GetHeight();
						if (below.GetBlockType() == Def.BlockType.STAIRS)
							belowHeight += 0.5f;
						block.SetHeight(belowHeight + block.GetLength());
					}
				}
				if(blockIE.StackedIdx[1] >= 0)
				{
					var above = FindBLock(pilar, blockIE.StackedIdx[1]);
					if (above != null)
					{
						block.GetStackedBlocksIdx()[1] = above.GetPilarIndex();
						above.GetStackedBlocksIdx()[0] = block.GetPilarIndex();
						var height = block.GetHeight();
						if (block.GetBlockType() == Def.BlockType.STAIRS)
							height += 0.5f;
						above.SetHeight(height + above.GetLength());
					}
				}
			}
			for (int i = 0; i < sBlocks.Count; ++i)
			{
				var block = sBlocks[i];
				var blockIE = blocks[block.GetIDXIE()];
				block.SetLayer(blockIE.Layer);
				var ieHeight = blockIE.GetHeight();
				while (block.GetHeight() > ieHeight)
					block.DecreaseHeight();
				while (block.GetHeight() < ieHeight)
					block.IncreaseHeight();
			}

			//for (int i = 0; i < sBlocks.Count; ++i)
			//	sBlocks[i]._CheckStackedLinks();

			return struc;
		}
		public override List<AI.CLivingEntity> GetLES() => m_LES;
	}
}
