/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
	public class CBlock : IBlock
	{
		Collider m_StairCollider;
		List<AI.CLivingEntity> m_Monsters;

		public void AddMonster(AI.CLivingEntity le)
		{
			if (!m_Monsters.Contains(le))
				m_Monsters.Add(le);
		}
		public void RemoveMonster(AI.CLivingEntity le)
		{
			if (m_Monsters.Contains(le))
				m_Monsters.Remove(le);
		}
		public Collider GetStairCollider() => m_StairCollider;
		public override void DestroyBlock(bool preserveEntities, bool instant = false)
		{
			m_Pilar.RemoveBlock(this);
			var struc = m_Pilar.GetStruc();
			if(m_BlockType == Def.BlockType.WIDE)
			{
				var id = m_Pilar.GetStructureID();
				var pos = struc.VPosFromPilarID(id);
				var rPilar = struc.GetPilars()[struc.PilarIDFromVPos(new Vector2Int(pos.x + 1, pos.y))];
				var bPilar = struc.GetPilars()[struc.PilarIDFromVPos(new Vector2Int(pos.x, pos.y + 1))];
				var brPilar = struc.GetPilars()[struc.PilarIDFromVPos(new Vector2Int(pos.x + 1, pos.y + 1))];

				if (rPilar.GetBlocks().Contains(this))
					rPilar.GetBlocks().Remove(this);
				if (bPilar.GetBlocks().Contains(this))
					bPilar.GetBlocks().Remove(this);
				if (brPilar.GetBlocks().Contains(this))
					brPilar.GetBlocks().Remove(this);
			}
			if (m_Prop != null && !preserveEntities)
			{
				//m_Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, m_Prop.GetTotalHealth());
				//if (struc != null && struc.GetLivingEntities().Contains(m_Prop))
				//	struc.GetLivingEntities().Remove(m_Prop);
				m_Prop.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Prop.GetLE().GetCurrentHealth());
				//if (struc != null && struc.GetLES().Contains(m_Prop.GetLE()))
				//	struc.GetLES().Remove(m_Prop.GetLE());
				//m_Prop = null;
			}
			if(m_Monsters != null && m_Monsters.Count > 0 && !preserveEntities)
			{
				for(int i = 0; i < m_Monsters.Count; ++i)
				{
					var mon = m_Monsters[i];
					mon.ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, mon.GetCurrentHealth());
				}
			}
			//if (m_Monster != null && !preserveEntities)
			//{
				//m_Monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, m_Monster.GetTotalHealth());
				//if (struc != null && struc.GetLivingEntities().Contains(m_Monster))
				//	struc.GetLivingEntities().Remove(m_Monster);
				//m_Monster.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Monster.GetLE().GetCurrentHealth());
				//if (struc != null && struc.GetLES().Contains(m_Monster.GetLE()))
				//	struc.GetLES().Remove(m_Monster.GetLE());
				//m_Monster = null;
			//}
			for (int i = 0; i < m_Ants.Length; ++i)
			{
				if (m_Ants[i] != null)
					GameUtils.DeleteGameobject(m_Ants[i].gameObject, instant);
			}
			for(int i = 0; i < m_Deco.Length; ++i)
			{
				if (m_Deco[i] != null)
					GameUtils.DeleteGameobject(m_Deco[i].gameObject, instant);
			}
			GameUtils.DeleteGameobject(gameObject, instant);
		}
		protected override void UpdateHeight(float heightDiff)
		{
			// No-op
		}
		protected override void LateInitBlock()	
		{
			if(m_Monsters == null)
				m_Monsters = new List<AI.CLivingEntity>();
		}
		public override void SetBlockFloat(float offset = 0, float speed = 0)
		{
			throw new NotImplementedException();
		}
		public override void SetProp(AI.CProp prop) => m_Prop = prop;
		protected override void PilarNameChange(int prevNameLength)
		{
			// No-op
		}
		protected override void UpdateLength()
		{
			m_MidMR.receiveShadows = true;
			m_MidMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		}
		public override Def.BlockState GetBlockState() => Def.BlockState.Game;
		public void InitGameBlock(BlockDef def)
		{
			m_BlockBC = def.BlockBC;
			m_BlockBC.gameObject.layer = Def.RCLayerBlock;
			m_BlockBC.enabled = true;
			m_TopMR = def.TopMR;
			m_TopMR.enabled = true;
			m_MidMR = def.MidMR;
			m_MidMR.enabled = true;
			m_Prop = def.Prop;
			if (m_Prop != null)
			{
				m_Prop.enabled = true;
				m_Prop.GetLE().SetCurrentBlock(this);
				//m_Prop.SetBlock(this);
			}
			if(def.Monster != null)
			{
				m_Monsters.Add(def.Monster.GetLE());
			}
			//m_Monster = def.Monster;
			//if (m_Monster != null)
			//	m_Monster.enabled = true;
			m_MaterialFamily = def.MaterialFamily;
			m_Layer = def.Layer;
			m_Length = def.Length;
			m_Height = def.Height;
			m_MicroHeight = def.MicroHeight;
			m_Pilar = def.Pilar;
			if(m_Pilar != null && !m_Pilar.GetBlocks().Contains(this))
				m_Pilar.GetBlocks().Add(this);
			m_Ants = def.Ants;
			if (m_Ants != null)
			{
				for (int i = 0; i < m_Ants.Length; ++i)
				{
					if (m_Ants[i] != null)
						m_Ants[i].enabled = true;
				}
			}
			m_Deco = def.Deco;
			if (m_Deco != null)
			{
				for (int i = 0; i < m_Deco.Length; ++i)
				{
					if (m_Deco[i] != null)
						m_Deco[i].enabled = true;
				}
			}
			//m_StackedBlocks = def.StackedBlocks;
			m_StackedBlocksIdx = def.StackedBlocksIdx;

			var above = GetStackedAbove();
			var below = GetStackedBelow();
			if (above != null)
				above.GetStackedBlocksIdx()[0] = m_PilarIndex;
			if (below != null)
				below.GetStackedBlocksIdx()[1] = m_PilarIndex;
			//if (m_StackedBlocks[0] != null)
			//	m_StackedBlocks[0].GetStackedBlocks()[1] = this;
			//if (m_StackedBlocks[1] != null)
			//	m_StackedBlocks[1].GetStackedBlocks()[0] = this;
			m_BlockType = def.BlockType;
			m_Rotation = def.Rotation;
			m_StairType = def.StairType;
			if(m_BlockType == Def.BlockType.STAIRS)
			{
				switch (m_StairType)
				{
					case Def.StairType.NORMAL:
						BoxCollider stairCollider = m_TopMR.gameObject.GetComponent<BoxCollider>();
						if(stairCollider == null)
							stairCollider = m_TopMR.gameObject.AddComponent<BoxCollider>();
						stairCollider.center = new Vector3(-0.5f, 0.25f, 0.25f);
						stairCollider.size = new Vector3(1f, 0.5f, 0.5f);
						m_StairCollider = stairCollider;
						m_StairCollider.gameObject.layer = Def.RCLayerBlock;
						m_StairCollider.enabled = true;
						m_BlockBC.size = new Vector3(1f + 1f * Def.BlockSeparation, m_Length + 0.075f, 1f + 1f * Def.BlockSeparation);
						m_BlockBC.center = new Vector3(-0.5f, (m_Length * -0.5f) - 0.0375f, 0.5f);

						break;
					case Def.StairType.RAMP:
						var rampCollider = m_TopMR.gameObject.GetComponentInChildren<MeshCollider>();
						if(rampCollider == null)
						{
							rampCollider = Instantiate(Blocks.RampCollider);
							rampCollider.gameObject.layer = Def.RCLayerBlock;
							rampCollider.transform.SetParent(m_TopMR.transform, false);
						}
						m_StairCollider = rampCollider;
						m_StairCollider.gameObject.layer = Def.RCLayerBlock;
						m_StairCollider.enabled = true;
						m_BlockBC.size = new Vector3(1f + 1f * Def.BlockSeparation, (m_Length + 0.075f) - 0.1f, 1f + 1f * Def.BlockSeparation);
						m_BlockBC.center = new Vector3(-0.5f, ((m_Length * -0.5f) - 0.0375f) - 0.05f, 0.5f);
						m_StairType = Def.StairType.RAMP;
						break;
					default:
						Debug.LogWarning("Unknown StairType " + def.TopMaterialPart.StairType.ToString());
						break;
				}
			}
		}
		public void InitFromWorld(World.BlockInfo info)
		{
			m_Pilar = transform.parent.GetComponent<CPilar>();
			m_PilarIndex = info.PilarIndex;
			m_MaterialFamily = info.MatFamily;
			if(info.MatSet == null)
			{
				var biomeMaterial = BlockMaterial.BiomeMaterials[(int)info.BiomeLayer];
				m_TopMR.material = biomeMaterial;
				m_MidMR.material = biomeMaterial;
			}
			else
			{
				m_TopMR.material = info.MatSet.TopPart.Mat;
				m_MidMR.material = info.MatSet.BottomPart.Mat;
			}
			m_Prop = null;
			if(m_Monsters != null)
				m_Monsters.Clear();
			//m_Prop = info.Prop;
			//m_Monsters = info.Monsters;
			m_Layer = info.Layer;
			m_Length = info.Length;
			m_Height = info.Height;
			m_MicroHeight = info.MicroHeight;
			transform.localPosition = new Vector3(transform.localPosition.x,
				m_Height + m_MicroHeight, transform.localPosition.z);
			SetRotation(info.Rotation);
			
			//SetHeight(info.Height);
			//SetMicroHeight(info.MicroHeight);
			m_StackedBlocksIdx = info.StackedIdx;
		}
		protected override void Enable(bool enable)
		{
			if (m_BlockBC != null)
				m_BlockBC.enabled = enable;
			if (m_StairCollider != null)
				m_StairCollider.enabled = enable;
			if (m_TopMR != null)
				m_TopMR.enabled = enable;
			if (m_MidMR != null)
				m_MidMR.enabled = enable;
			if (m_Prop != null)
				m_Prop.gameObject.SetActive(enable);
			if (m_Monsters != null)
			{
				for (int i = 0; i < m_Monsters.Count; ++i)
				{
					var mon = m_Monsters[i];
					if (mon == null)
						continue;
					mon.gameObject.SetActive(enable);
				}
			}
		}
	}
}
