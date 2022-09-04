/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using Assets.AI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets.World
//{
//	public class WStruc : IStruc
//	{
//		public const int Size = 16;
//		public const int PilarCount = Size * Size;
//		const int LEPrealloc = 32;

//		CPilar[] m_Pilars;
//		List<AI.CLivingEntity> m_LivingEntities;
//		List<CPilar> m_ValidPilars;
//		RectInt m_Bounds;
//		World m_World;

//		WStruc()
//		{
//			m_Pilars = new CPilar[PilarCount];
//			m_ValidPilars = new List<CPilar>(PilarCount);
//			m_LivingEntities = new List<CLivingEntity>(LEPrealloc);
//		}
//		public void Init(World world)
//		{
//			m_World = world;
//		}
//		public void MoveStruc(Vector2Int pos)
//		{
//			var wPos = GameUtils.TransformPosition(pos);
//			transform.position = new Vector3(wPos.x, 0f, wPos.y);
//			m_Bounds.position = pos;
//			m_Bounds.size = new Vector2Int(Size, Size);
//		}
//		public void ClearStruc()
//		{
//			while (m_ValidPilars.Count > 0)
//			{
//				WorldPool.ReturnPilar(m_ValidPilars[0]);
//				m_ValidPilars.RemoveAt(0);
//			}
//			for (int i = 0; i < m_Pilars.Length; ++i)
//				m_Pilars[i] = null;

//			m_LivingEntities.Clear();
//		}
//		public void LoadPilars()
//		{
//			var wPilars = m_World.GetPilars();

//			for(int i = 0; i < PilarCount; ++i)
//			{
//				var sPos = GameUtils.PosFromID(i, Size, Size);
//				int yOffset = sPos.y * m_World.GetWorldSize().x;
//				int pilarIDX = yOffset + sPos.x;
//				var pilarInfo = wPilars[pilarIDX];
//				if (pilarInfo == null)
//					continue;

//				var sHeight = pilarInfo.GetStruc().GetHeight();
//				var pilar = WorldPool.GetPilar();
//				pilar.gameObject.SetActive(true);
//				pilar.transform.SetParent(transform);
//				pilar.transform.localPosition = new Vector3(sPos.x * (1f + Def.BlockSeparation), sHeight, sPos.y * (1f + Def.BlockSeparation));

//				m_Pilars[i] = pilar;
//				pilar.GetBlocks().AddRange(Enumerable.Repeat<IBlock>(null, pilarInfo.GetBlocks().Count));
				
//				for(int j = 0; j < pilarInfo.GetBlocks().Count; ++j)
//				{
//					var blockInfo = pilarInfo.GetBlocks()[j];
//					var block = WorldPool.GetBlock(blockInfo.Length, blockInfo.BlockType, blockInfo.StairType);

//					pilar.GetBlocks()[j] = block;
//					block.gameObject.SetActive(true);
//					block.transform.SetParent(pilar.transform);

//					// Apply 
//					block.InitFromWorld(blockInfo);
//				}
//			}
//		}
//		public void AddLE(AI.CLivingEntity le)
//		{
//			if (!m_LivingEntities.Contains(le))
//				m_LivingEntities.Add(le);
//		}
//		public override RectInt GetBounds()
//		{
//			throw new NotImplementedException();
//		}
//		public override Rect GetBoundsF()
//		{
//			throw new NotImplementedException();
//		}
//		public override int GetHeight()
//		{
//			throw new NotImplementedException();
//		}
//		public override List<CLivingEntity> GetLES()
//		{
//			throw new NotImplementedException();
//		}
//		public override CPilar[] GetPilars()
//		{
//			throw new NotImplementedException();
//		}
//		public override int GetWidth()
//		{
//			throw new NotImplementedException();
//		}
//		public void RemoveLE(AI.CLivingEntity le)
//		{
//			if (m_LivingEntities.Contains(le))
//				m_LivingEntities.Remove(le);
//		}
//	}
//}
