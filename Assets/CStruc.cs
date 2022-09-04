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
	public class CStruc : IStruc
	{
		public const int Width = 16;
		public const int Height = 16;
		public const int PilarCount = Width * Height;

		CPilar[] m_Pilars;
		int m_StrucID;
		//List<LivingEntity> m_LivingEntities;
		List<AI.CLivingEntity> m_LES;
		List<CPilar> m_ValidPilars;
		//StrucQT m_QT;

		//bool ComputeSize(CPilar[] pilars, out int topIdx)
		//{
		//    topIdx = -1;
		//    int botIdx = -1;
		//    Vector2Int top = new Vector2Int(-1, -1)
		//        , bot = new Vector2Int(-1, -1);
		//    for (int y = 0; y < Def.MaxStrucSide; ++y)
		//    {
		//        int yOffset = y * Def.MaxStrucSide;
		//        for(int x = 0; x < Def.MaxStrucSide; ++x)
		//        {
		//            int idx = yOffset + x;
		//            var pilar = pilars[idx];
		//            if (pilar == null || (pilar != null && pilar.GetBlocks().Count == 0))
		//                continue;
		//            bool validBlock = false;
		//            for(int i = 0; i < pilar.GetBlocks().Count; ++i)
		//            {
		//                if(((CBlockEdit)pilar.GetBlocks()[i]).GetLayer() != 0)
		//                {
		//                    validBlock = true;
		//                    break;
		//                }
		//            }
		//            if (!validBlock)
		//                continue;
		//            if(topIdx < 0 || (topIdx >= 0 && top.y >= y && top.x > x))
		//            {
		//                topIdx = idx;
		//                top.Set(x, y);
		//            }
		//            if (botIdx < 0 || (botIdx >= 0 && bot.y <= y && bot.x < x))
		//            {
		//                botIdx = idx;
		//                bot.Set(x, y);
		//            }
		//        }
		//    }
		//    if (topIdx < 0 || botIdx < 0)
		//        return false;
		//    m_Width = bot.x - top.x;
		//    m_Height = bot.y - top.y;
		//    if (m_Width < Def.MinStrucSide || m_Width > Def.MaxStrucSide
		//        || m_Height < Def.MinStrucSide || m_Height > Def.MaxStrucSide)
		//    {
		//        return false;
		//    }
		//    return true;
		//}
		private void Awake()
		{
			m_Pilars = new CPilar[Width * Height];
			//m_LivingEntities = new List<LivingEntity>();
			m_LES = new List<AI.CLivingEntity>();
			m_ValidPilars = new List<CPilar>(Width * Height);
		}
		public static CStruc CreateFromMap(int strucID)
		{
			// Virtual position in StrucSpace
			var vPos = GameUtils.PosFromID(strucID, Map.HorizontalStrucs, Map.VerticalStrucs);
			// Virtual position in WorldSpace
			var vwPos = vPos * new Vector2Int(Width, Height);
			// World position in world space
			var wPos = GameUtils.TransformPosition(vwPos);
			var struc = new GameObject($"Struc_({vPos.x},{vPos.y})").AddComponent<CStruc>();
			struc.transform.SetParent(Map.GetCurrent().transform);
			struc.transform.localPosition = new Vector3(wPos.x, 0f, wPos.y);
			//struc.m_Bounds = new RectInt(vwPos, new Vector2Int(Width, Height));
			struc.m_StrucID = strucID;
			return struc;
		}
		public static CStruc CreateFromWorld(int strucID, int strucsHortz, int strucsVert)
		{
			var vsPos = GameUtils.PosFromIDUnsafe(strucID, strucsHortz, strucsVert);
			var vPos = new Vector2Int(vsPos.x * 16, vsPos.y * 16);
			var wPos = GameUtils.TransformPosition(vPos);
			var struc = new GameObject($"Struc_({vPos.x},{vPos.y})_{strucID}").AddComponent<CStruc>();
			struc.transform.localPosition = new Vector3(wPos.x, 0f, wPos.y);
			//struc.m_Bounds = new RectInt(vPos, new Vector2Int(Width, Height));
			struc.m_StrucID = strucID;
			return struc;
		}
		public void LoadFromWorld(World.World world)
		{
			var worldSize = world.GetWorldSize();
			var worldStrucSize = world.GetWorldStrucSize();
			var vsPos = GameUtils.PosFromIDUnsafe(m_StrucID, worldStrucSize.x, worldStrucSize.y);
			var vPos = new Vector2Int(vsPos.x * 16, vsPos.y * 16);
			for(int y = 0; y < Height; ++y)
			{
				for(int x = 0; x < Width; ++x)
				{
					var pilarPos = new Vector2Int(x + vPos.x, y + vPos.y);
					var pilarID = GameUtils.IDFromPosUnsafe(pilarPos, worldSize.x, worldSize.y);
					var vPilar = world.GetPilars()[pilarID];
					if (vPilar == null)
						continue;

					var wPilar = World.WorldPool.GetPilar();
					var sHeight = vPilar.GetStruc().GetHeight();
					//wPilar.transform.SetParent(transform);
					wPilar.gameObject.SetActive(true);
					var sPilarID = GameUtils.IDFromPosUnsafe(new Vector2Int(x, y), Width, Height);
					m_Pilars[sPilarID] = wPilar;
					wPilar.ChangeStruc(this, sPilarID);
					wPilar.transform.localPosition = new Vector3(x * (1f + Def.BlockSeparation), sHeight, y * (1f + Def.BlockSeparation));
					wPilar.transform.localRotation = Quaternion.identity;
					wPilar.transform.localScale = Vector3.one;

					//wPilar.ChangeStruc(this, sPilarID);
					m_ValidPilars.Add(wPilar);

					var vPilarBlocks = vPilar.GetBlocks();
					var wPilarBlocks = wPilar.GetBlocks();
					if (wPilarBlocks.Capacity < vPilarBlocks.Count)
						wPilarBlocks.Capacity = vPilarBlocks.Count;

					for (int i = 0; i < vPilarBlocks.Count; ++i)
					{
						var bi = vPilarBlocks[i];
						CBlock block;
						if (bi.Pilar != vPilar)
						{
							block = World.WorldPool.GetBlock(bi.Length, Def.BlockType.NORMAL, Def.StairType.NORMAL);
							block.gameObject.SetActive(false);
						}
						else
						{
							block = World.WorldPool.GetBlock(bi.Length, bi.BlockType, bi.StairType);
							block.gameObject.SetActive(true);
						}
						wPilarBlocks.Add(block);
						block.transform.SetParent(wPilar.transform);
						block.transform.localPosition = Vector3.zero;
						block.transform.localRotation = Quaternion.identity;
						block.transform.localScale = Vector3.one;

						if (vPilar.HasBeenLoaded)
						{
							for(int j = 0; j < bi.LivingEntities.Count; ++j)
							{
								var le = bi.LivingEntities[j];
								if(le == null)
								{
									bi.LivingEntities.RemoveAt(j);
									--j;
									continue;
								}	
								if(le.GetLEType() == Def.LivingEntityType.Prop)
								{
									var p = le.gameObject.GetComponent<AI.CProp>();
									//p.SetBlock(block);
									p.GetLE().SetCurrentBlock(block);
									block.SetProp(p);
									break;
								}
							}
						}
						else if(bi.PropFamilyID >= 0) // Load prop
						{
							var pFamily = Props.PropFamilies[bi.PropFamilyID];
							var propID = UnityEngine.Random.Range(0, pFamily.Props.Count);
							var prop = new GameObject().AddComponent<AI.CProp>();
							prop.SetProp(bi.PropFamilyID, propID);
							//prop.SetBlock(block);
							prop.GetLE().SetCurrentBlock(block);
							block.SetProp(prop);
							prop.GetSprite().Flip(UnityEngine.Random.value > 0.5f, false);
							bi.LivingEntities.Add(prop.GetLE());
							m_LES.Add(prop.GetLE());
						}

						// Apply 
						block.InitFromWorld(bi);
					}
					vPilar.HasBeenLoaded = true;
				}
			}
			for (int i = 0; i < m_LES.Count; ++i)
			{
				var le = m_LES[i];
				if(le == null)
				{
					m_LES.RemoveAt(i);
					--i;
					continue;
				}
				le.gameObject.SetActive(true);
			}
			//for(int y = vPos.y; y < (vPos.y + Height); ++y)
			//{
			//	for(int x = vPos.x; x < (vPos.x + Width); ++x)
			//	{
			//		var pilarPos = new Vector2Int(x, y);
			//		var pilarID = GameUtils.IDFromPosUnsafe(pilarPos, worldSize.x, worldSize.y);
			//		var vPilar = world.GetPilars()[pilarID];
			//		if (vPilar == null)
			//			continue;

			//		var wPilar = World.WorldPool.GetPilar();
			//		var sHeight = vPilar.GetStruc().GetHeight();
			//		wPilar.transform.SetParent(transform);
			//		wPilar.gameObject.SetActive(true);
			//		wPilar.transform.localPosition = new Vector3(x * (1f + Def.BlockSeparation), sHeight, y * (1f + Def.BlockSeparation));
			//		wPilar.transform.localRotation = Quaternion.identity;
			//		wPilar.transform.localScale = Vector3.one;
			//		var sPilarPos = new Vector2Int(x - bounds.x, y - bounds.y);
			//		var sPilarID = GameUtils.IDFromPosUnsafe(sPilarPos, Width, Height);
			//		m_Pilars[sPilarID] = wPilar;
			//		m_ValidPilars.Add(wPilar);

			//		var vPilarBlocks = vPilar.GetBlocks();
			//		var wPilarBlocks = wPilar.GetBlocks();
			//		if (wPilarBlocks.Capacity < vPilarBlocks.Count)
			//			wPilarBlocks.Capacity = vPilarBlocks.Count;

			//		for(int i = 0; i < vPilarBlocks.Count; ++i)
			//		{
			//			var bi = vPilarBlocks[i];
			//			if (bi.Pilar != vPilar)
			//				continue;
			//			var block = World.WorldPool.GetBlock(bi.Length, bi.BlockType, bi.StairType);
			//			wPilarBlocks.Add(block);
			//			block.gameObject.SetActive(true);
			//			block.transform.SetParent(wPilar.transform);
			//			block.transform.localPosition = Vector3.zero;
			//			block.transform.localRotation = Quaternion.identity;
			//			block.transform.localScale = Vector3.one;

			//			// Apply 
			//			block.InitFromWorld(bi);
			//		}
			//	}
			//}
		}
		public void UnloadFromWorld()
		{
			for(int i = 0; i < m_Pilars.Length; ++i)
			{
				var pilar = m_Pilars[i];
				if (pilar == null)
					continue;
				World.WorldPool.ReturnPilar(pilar);
				m_Pilars[i] = null;
			}
			for (int i = 0; i < m_LES.Count; ++i)
			{
				var le = m_LES[i];
				if (le == null)
				{
					m_LES.RemoveAt(i);
					i--;
					continue;
				}
				m_LES[i].gameObject.SetActive(false);
			}
			m_ValidPilars.Clear();
		}
		public void AssignPilars(List<CStrucEdit> strucs)
		{
			var bounds = GetBounds();
			RectInt GetStrucOverlappingRect(IStruc struc, RectInt rect)
			{
				rect.ClampToBounds(struc.GetBounds());
				return rect;
			}
			RectInt ComputeOverlappingRect(IStruc lStruc, IStruc rStruc)
			{
				var lBounds = lStruc.GetBounds();
				var rBounds = rStruc.GetBounds();
				int left = Mathf.Max(lBounds.x, rBounds.x);
				int top = Mathf.Max(lBounds.y, rBounds.y);
				int right = Mathf.Min(lBounds.x + lBounds.width, rBounds.x + rBounds.width);
				int bottom = Mathf.Min(lBounds.y + lBounds.height, rBounds.y + rBounds.height);
				return new RectInt(left, top, right - left, bottom - top);
			}
			for(int i = 0; i < strucs.Count; ++i)
			{
				var struc = strucs[i];
				var overlapping = ComputeOverlappingRect(this, struc);
				var editRect = GetStrucOverlappingRect(struc, overlapping);
				var gameRect = GetStrucOverlappingRect(this, overlapping);

				for(int y = 0; y < editRect.height; ++y)
				{
					for(int x = 0; x < editRect.width; ++x)
					{
						var curPos = new Vector2Int(x, y);
						var editPos = (editRect.position + curPos) - struc.GetBounds().position;
						var gamePos = (gameRect.position + curPos) - bounds.position;
						var editID = struc.PilarIDFromVPos(editPos);
						var gameID = PilarIDFromVPos(gamePos);
						var editPilar = struc.GetPilars()[editID];
						if (editPilar == null ||
							(editPilar != null && editPilar.GetBlocks().Count == 0))
							continue;
						struc.GetPilars()[editID] = null;
						int blockCount = editPilar.GetBlocks().Count;
						for(int j = 0; j < blockCount; ++j)
						{
							var editBlock = editPilar.GetBlocks()[0] as CBlockEdit;
							//var gameBlock = editPilar.AddGameBlock(false);
							if(editBlock.GetProp() != null)
							{
								//if (struc.GetLivingEntities().Contains(editBlock.GetProp()))
								//	struc.GetLivingEntities().Remove(editBlock.GetProp());
								//if (!m_LivingEntities.Contains(editBlock.GetProp()))
								//	m_LivingEntities.Add(editBlock.GetProp());
								if (struc.GetLES().Contains(editBlock.GetProp().GetLE()))
									struc.GetLES().Remove(editBlock.GetProp().GetLE());
								if (!m_LES.Contains(editBlock.GetProp().GetLE()))
									m_LES.Add(editBlock.GetProp().GetLE());
							}
							if(editBlock.GetMonster() != null)
							{
								//if (struc.GetLivingEntities().Contains(editBlock.GetMonster()))
								//	struc.GetLivingEntities().Remove(editBlock.GetMonster());
								//if (!m_LivingEntities.Contains(editBlock.GetMonster()))
								//	m_LivingEntities.Add(editBlock.GetMonster());
								if (struc.GetLES().Contains(editBlock.GetMonster().GetLE()))
									struc.GetLES().Remove(editBlock.GetMonster().GetLE());
								if (!m_LES.Contains(editBlock.GetMonster().GetLE()))
									m_LES.Add(editBlock.GetMonster().GetLE());
							}
							editBlock.ConvertToGame();
							//gameBlock.CopyFrom(editBlock);
							//editBlock.DestroyBlock(true);
						}
						editPilar.ChangeStruc(this, gameID);
						m_Pilars[gameID] = editPilar;
						m_ValidPilars.Add(editPilar);
					}
				}
			}
		}

		public override int GetWidth() => Width;
		public override int GetHeight() => Height;
		public override CPilar[] GetPilars() => m_Pilars;
		public override RectInt GetBounds()
		{
			return new RectInt(GameUtils.TransformPosition(new Vector2(transform.position.x, transform.position.z)), new Vector2Int(Width, Height));
		}
		public override Rect GetBoundsF()
		{
			return new Rect(new Vector2(transform.position.x, transform.position.z), GameUtils.TransformPosition(new Vector2Int(Width, Height)));
		}
		//public override StrucQT GetQT() => m_QT;
		//public override void SetQT(StrucQT qt) => m_QT = qt;
		//public override List<LivingEntity> GetLivingEntities() => m_LivingEntities;
		public override List<AI.CLivingEntity> GetLES() => m_LES;
		public List<CPilar> GetValidPilars() => m_ValidPilars;
		public int GetStrucID() => m_StrucID;
	}
}
