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
	public static class WorldPool
	{
		static int[] m_BlockIDs;
		static List<CBlock>[] m_BlockPool;
		static List<CPilar> m_PilarPool;
		static GameObject m_BlockPoolGO;
		static GameObject m_PilarPoolGO;
		static bool m_Initialized;
		static int GetPoolIndex(float length, Def.BlockType blockType, Def.StairType stairType)
		{
			if (length == 3.4f)
				length = 3.5f;
			int lengthOffset = (int)(length * 2f) - 1;
			const int NormalOffset = 0;
			const int StairOffset = 7;
			const int RampOffset = StairOffset + 7;
			const int WideOffset = RampOffset + 7;
			switch (blockType)
			{
				case Def.BlockType.NORMAL:
					lengthOffset = Mathf.Min(lengthOffset, 6);
					return NormalOffset + lengthOffset;
				case Def.BlockType.STAIRS:
					lengthOffset = Mathf.Min(lengthOffset, 6);
					int stairOffset = 0;
					switch (stairType)
					{
						case Def.StairType.NORMAL:
							stairOffset = StairOffset;
							break;
						case Def.StairType.RAMP:
							stairOffset = RampOffset;
							break;
					}
					return stairOffset + lengthOffset;
				case Def.BlockType.WIDE:
					lengthOffset = Mathf.Min(lengthOffset, 4);
					return WideOffset + lengthOffset;
			}
			Debug.LogWarning("Invaid BlockType " + blockType.ToString());
			return 0;
		}
		public static void Init()
		{
			if (m_Initialized)
				return;

			m_BlockPoolGO = new GameObject("BlockPool");
			m_PilarPoolGO = new GameObject("PilarPool");

			const int PoolSize = 3 * 7 + 5;
			m_BlockPool = new List<CBlock>[PoolSize];
			m_BlockIDs = new int[PoolSize];

			for(int i = 0; i < PoolSize; ++i)
			{
				m_BlockPool[i] = new List<CBlock>();
				m_BlockIDs[i] = 0;
			}

			const int MaxPilars = World.OnCamStrucs * CStruc.PilarCount;
			m_PilarPool = new List<CPilar>(MaxPilars);
			int pilarID = 0;
			for(int i = 0; i < MaxPilars; ++i)
			{
				var pilar = new GameObject("Pilar_" + (pilarID++).ToString().PadLeft(4, '0')).AddComponent<CPilar>();
				pilar.gameObject.SetActive(false);
				pilar.transform.SetParent(m_PilarPoolGO.transform);
				m_PilarPool.Add(pilar);
			}

			m_Initialized = true;
		}
		public static CBlock GetBlock(float length, Def.BlockType blockType = Def.BlockType.NORMAL, Def.StairType stairType = Def.StairType.NORMAL)
		{
			length = Mathf.Abs(length);
			int poolIdx = GetPoolIndex(length, blockType, stairType);
			CBlock block;
			var pool = m_BlockPool[poolIdx];
			if(pool.Count > 0)
			{
				block = pool[0];
				pool.RemoveAt(0);
				block.gameObject.SetActive(true);
				block.InitGameBlock(new BlockDef()
				{
					BlockBC = block.GetCollider(),
					TopMR = block.GetTopMR(),
					MidMR = block.GetMidMR(),
					Prop = null,
					MaterialFamily = null,
					Layer = 0,
					Length = length,
					Height = 0f,
					MicroHeight = 0f,
					BlockFloatOffset = 0f,
					BlockFloatSpeed = 0f,
					BlockType = blockType,
					Pilar = null,
					Ants = null,
					Deco = null,
					StackedBlocksIdx = new int[2] { -1, -1 },
					MidMaterialPart = null,
					TopMaterialPart = null,
					Monster = null,
					Rotation = Def.RotationState.Default,
					StairType = stairType
				});
				return block;
			}
			var id = m_BlockIDs[poolIdx]++;
			block = new GameObject($"Block_{length}_{blockType}_{stairType}_{id}").AddComponent<CBlock>();
			//Debug.Log("Block Pool " + poolIdx.ToString() + " ID " + id.ToString());
			float width;
			if (blockType == Def.BlockType.WIDE)
				width = 2f;
			else
				width = 1f;
			width *= 1f + Def.BlockSeparation;

			Vector2 stairOffset = Vector2.zero;
			if(blockType == Def.BlockType.STAIRS)
			{
				stairOffset.x = 0.5f;
				stairOffset.y = 0.25f;
			}

			var blockBC = block.gameObject.AddComponent<BoxCollider>();
			blockBC.size = new Vector3(width, (length + stairOffset.x) + 0.075f, width);
			blockBC.center = new Vector3(width * -0.5f, (length * -0.5f + stairOffset.y) - 0.0375f, width * 0.5f);

			var topMR = new GameObject("Top").AddComponent<MeshRenderer>();
			topMR.gameObject.layer = Def.RCLayerBlock;
			Blocks.SetBlock(topMR.gameObject, blockType, Def.BlockMeshType.TOP, length, stairType);
			topMR.transform.SetParent(block.transform);

			var midMR = new GameObject("Mid").AddComponent<MeshRenderer>();
			midMR.gameObject.layer = Def.RCLayerBlock;
			Blocks.SetBlock(midMR.gameObject, blockType, Def.BlockMeshType.MID, length, stairType);
			midMR.transform.SetParent(block.transform);

#if UNITY_2021
			topMR.staticShadowCaster = true;
			midMR.staticShadowCaster = true;
#endif

			block.InitGameBlock(new BlockDef()
			{
				BlockBC = blockBC,
				TopMR = topMR,
				MidMR = midMR,
				Prop = null,
				Monster = null,
				MaterialFamily = null,
				TopMaterialPart = null,
				MidMaterialPart = null,
				Layer = 0,
				Length = length,
				Height = 0f,
				MicroHeight = 0f,
				BlockFloatOffset = 0f,
				BlockFloatSpeed = 0f,
				Pilar = null,
				Ants = null,
				Deco = null,
				StackedBlocksIdx = new int[2] {-1, -1},
				BlockType = blockType,
				StairType = stairType,
				Rotation = Def.RotationState.Default
			});

			return block;
		}
		public static void ReturnBlock(CBlock block)
		{
			if(block == null)
			{
				Debug.LogWarning("Trying to return a null block to WorldPool.");
				return;
			}
			int poolIdx = GetPoolIndex(block.GetLength(), block.GetBlockType(), block.GetStairType());

			m_BlockPool[poolIdx].Add(block);
			block.gameObject.SetActive(false);
			if(m_BlockPoolGO != null)
				block.transform.SetParent(m_BlockPoolGO.transform);
		}
		public static CPilar GetPilar()
		{
			if (m_PilarPool == null || m_PilarPool.Count == 0)
				throw new Exception("PilarPool not initialized or exhausted, something went wrong!");

			var pilar = m_PilarPool[0];
			m_PilarPool.RemoveAt(0);
			return pilar;
		}
		public static void ReturnPilar(CPilar pilar)
		{
			var blocks = pilar.GetBlocks();
			for(int i = 0; i < blocks.Count; ++i)
			{
				var block = blocks[i];
				//if (block == null)
				//	continue;
				ReturnBlock(block as CBlock);
			}
			blocks.Clear();
			m_PilarPool.Add(pilar);
			pilar.gameObject.SetActive(false);
			if(m_PilarPoolGO != null)
				pilar.transform.SetParent(m_PilarPoolGO.transform);
		}
	}
}
