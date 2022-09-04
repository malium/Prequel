/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using UnityEngine;

namespace Assets
{
	public class CBlockEdit : IBlock
	{
		[SerializeField] int m_IDXIE;
		[SerializeField] int m_LinkedTo;
		[SerializeField] bool m_Removed;
		[SerializeField] bool m_Highlighted;
		[SerializeField] bool m_Selected;
		[SerializeField] bool m_Anchor;
		[SerializeField] AI.CMonster m_Monster;
		[SerializeField] CBlockEdit[] m_HiddenBlocks;
		[SerializeField] CBlockEdit m_ParentWIDE;
		Outline m_TopOutline;
		Outline m_MidOutline;
		SpriteRenderer m_AnchorSR;
		SpriteRenderer m_StairSR;
		SpriteRenderer m_LayerSR;
		SpriteRenderer m_LockSR;
		SpriteRenderer m_VoidSR;
		[SerializeField] MaterialPart m_TopMaterialPart;
		[SerializeField] MaterialPart m_MidMaterialPart;
		[SerializeField] Def.StairState m_StairState;
		[SerializeField] Def.LockState m_Lock;
		[SerializeField] Def.BlockVoid m_Void;
		bool m_MicroheightApplied;

		public int GetIDXIE() => m_IDXIE;
		public bool IsRemoved() => m_Removed;
		public bool IsHighlighted() => m_Highlighted;
		public bool IsSelected() => m_Selected;
		public bool IsAnchor() => m_Anchor;
		public int GetLinkedTo() => m_LinkedTo;
		public void SetLinkedTo(int linkedTo) => m_LinkedTo = linkedTo;
		public CBlockEdit[] GetHiddenBlocks() => m_HiddenBlocks;
		public AI.CMonster GetMonster() => m_Monster;
		public void _SetParentWIDE(CBlockEdit parent) => m_ParentWIDE = parent;
		public CBlockEdit GetParentWIDE() => m_ParentWIDE;
		public SpriteRenderer GetAnchorRnd() => m_AnchorSR;
		public SpriteRenderer GetStairRnd() => m_StairSR;
		public SpriteRenderer GetLayerRnd() => m_LayerSR;
		public SpriteRenderer GetLockRnd() => m_LockSR;
		public SpriteRenderer GetVoidRnd() => m_VoidSR;
		public MaterialPart GetTopMaterial() => m_TopMaterialPart;
		public MaterialPart GetMidMaterial() => m_MidMaterialPart;
		public Def.StairState GetStairState() => m_StairState;
		public Def.LockState GetLockState() => m_Lock;
		public Def.BlockVoid GetVoidState() => m_Void;
		public bool IsMicroheightApplied() => m_MicroheightApplied;
		public void SetMicroheightApplied(bool applied) => m_MicroheightApplied = applied;

		public void CopyBlockTo(CBlockEdit block)
		{
			block.SetLayer(m_Layer);
			block.SetLinkedTo(m_LinkedTo);
			block.SetAnchor(m_Anchor);
			block.SetVoidState(m_Void);
			block.SetLockState(m_Lock);
			block.SetLength(m_Length);
			block.SetHeight(m_Height);
			block.SetMicroHeight(m_MicroHeight);
			block.SetRotation(m_Rotation);
			block.SetStairState(m_StairState);
			block.SetBlockType(m_BlockType, m_StairType);
			block.SetMaterialFamily(m_MaterialFamily);
			block.SetMicroheightApplied(m_MicroheightApplied);
			block.SetIDXIE(m_IDXIE);
		}
		public void SetWIDE(CBlockEdit[] hiddenBlocks)
		{
			if(hiddenBlocks == null || (hiddenBlocks != null && hiddenBlocks.Length != 3))
			{
				Debug.Log("Trying to change into a WIDE block but the provided hidden blocks were invalid.");
				SetBlockType(Def.BlockType.NORMAL);
				return;
			}
			SetBlockType(Def.BlockType.WIDE);
			m_HiddenBlocks = hiddenBlocks;
			for (int i = 0; i < m_HiddenBlocks.Length; ++i)
			{
				var block = m_HiddenBlocks[i];
				if (block == null)
					continue;
				block.m_ParentWIDE = this;
				block.SetLayer(m_Layer);
				block.SetRemoved(true);
				block.SetHeight(m_Height);
				block.SetLength(m_Length);
				for (int j = 0; j < block.GetPilar().GetBlocks().Count; ++j)
				{
					var oblock = ((CBlockEdit)block.GetPilar().GetBlocks()[j]);
					if (oblock == block)
						continue;
					if (oblock.IsRemoved())
					{
						var parent = oblock.GetParentWIDE();
						if(parent != null)
							parent.SetMicroHeight(m_MicroHeight);
					}
					else
					{
						oblock.SetMicroHeight(m_MicroHeight);
					}
				}
			}
		}
		void UnsetWIDE()
		{
			if(m_HiddenBlocks == null)
			{
				Debug.Log("Trying to unset a wide block, but it has invlid hidden blocks.");
				return;
			}

			for (int i = 0; i < m_HiddenBlocks.Length; ++i)
			{
				var b = m_HiddenBlocks[i];
				if (b == null)
					continue;
				b.SetRemoved(false);
				b.m_ParentWIDE = null;
			}
			m_HiddenBlocks = null;
		}
		//static MaterialPropertyBlock gPropertyBlock;
		public void SetTopMaterial(MaterialPart materialPart)
		{
			m_TopMaterialPart = materialPart;
			//if (m_TopMR.material.GetTexture(Def.MaterialTextureID) == null)
			//{
			//	Material.Destroy(m_MidMR.material);
			//	m_MidMR.material = null;
			//}
			m_TopMR.material = materialPart.Mat;
			//if (gPropertyBlock == null)
			//    gPropertyBlock = new MaterialPropertyBlock();
			//gPropertyBlock.SetFloat(Def.Material_TA_Index, materialPart.ArrayIdx);
			//m_TopMR.SetPropertyBlock(gPropertyBlock);
		}
		public void SetMidMaterial(MaterialPart materialPart)
		{
			m_MidMaterialPart = materialPart;
			//if(m_MidMR.material.GetTexture(Def.MaterialTextureID) == null)
			//{
			//	Material.Destroy(m_MidMR.material);
			//	m_MidMR.material = null;
			//}
			m_MidMR.material = materialPart.Mat;
			//if (gPropertyBlock == null)
			//    gPropertyBlock = new MaterialPropertyBlock();
			//gPropertyBlock.SetFloat(Def.Material_TA_Index, materialPart.ArrayIdx);
			//m_MidMR.SetPropertyBlock(gPropertyBlock);
		}
		public void SetBlockType(Def.BlockType type, Def.StairType stairType = Def.StairType.COUNT)
		{
			if (m_BlockType == type)
				return;

			//if (m_BlockType == Def.BlockType.WIDE)
			//	UnsetWIDE();

			Def.StairType sType = m_StairType;
			if (stairType != Def.StairType.COUNT)
				sType = stairType;

			Blocks.SetBlock(m_TopMR.gameObject, type, Def.BlockMeshType.TOP, 0f, sType);
			Blocks.SetBlock(m_MidMR.gameObject, type, Def.BlockMeshType.MID, 0f, sType);
			//BlockMeshDef.SetBlock(m_TopMR.gameObject, type, Def.BlockMeshType.TOP, 0f);
			//BlockMeshDef.SetBlock(m_MidMR.gameObject, type, Def.BlockMeshType.MID, 0f);

			if (type == Def.BlockType.WIDE)
			{
				m_AnchorSR.transform.localScale =   new Vector3(1.6f, 1.6f, 1f);
				m_LayerSR.transform.localScale =    new Vector3(1.6f, 1.6f, 1f);
				m_LockSR.transform.localScale =     new Vector3(1.6f, 1.6f, 1f);
				if (m_StairState == Def.StairState.ALWAYS)
					SetStairState(Def.StairState.NONE);
			}
			else
			{
				m_AnchorSR.transform.localScale =   new Vector3(0.8f, 0.8f, 1f);
				m_LayerSR.transform.localScale =    new Vector3(0.8f, 0.8f, 1f);
				m_LockSR.transform.localScale =     new Vector3(0.8f, 0.8f, 1f);
			}
			if (m_BlockType == Def.BlockType.STAIRS)
			{
				m_LockSR.transform.Translate(0f, -0.5f, 0f, Space.World);
				m_LayerSR.transform.Translate(0f, -0.5f, 0f, Space.World);
				m_VoidSR.transform.Translate(0f, -0.5f, 0f, Space.World);
			}

			if (m_Layer != 0)
			{
				m_TopMR.receiveShadows = true;
				m_TopMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
				m_MidMR.receiveShadows = true;
				m_MidMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			}

			switch (type)
			{
				case Def.BlockType.NORMAL:
					m_BlockBC.size = new Vector3(1f * Def.BlockSeparation, m_Length, 1f * Def.BlockSeparation);
					m_BlockBC.center = new Vector3(-0.5f, m_Length * -0.5f, 0.5f);
					break;
				case Def.BlockType.STAIRS:
					m_BlockBC.size = new Vector3(1f * Def.BlockSeparation, m_Length + 0.5f, 1f * Def.BlockSeparation);
					m_BlockBC.center = new Vector3(-0.5f, m_Length * -0.5f + 0.25f, 0.5f);
					m_LockSR.transform.Translate(0f, 0.5f, 0f, Space.World);
					m_LayerSR.transform.Translate(0f, 0.5f, 0f, Space.World);
					m_VoidSR.transform.Translate(0f, 0.5f, 0f, Space.World);
					break;
				case Def.BlockType.WIDE:
					m_BlockBC.size = new Vector3(2f * Def.BlockSeparation, m_Length, 2f * Def.BlockSeparation);
					m_BlockBC.center = new Vector3(-1f, m_Length * -0.5f, 1f);
					break;
			}
			m_BlockType = type;
			SetMaterialFamily(m_MaterialFamily, sType);
			var tempLen = m_Length;
			m_Length = 0f;
			SetLength(tempLen);
			var tempRot = m_Rotation;
			m_Rotation = Def.RotationState.COUNT;
			SetRotation(tempRot);
		}
		//public void SetStairType(Def.StairType type)
		//{
		//	if (type == m_StairType || type == Def.StairType.COUNT)
		//		return;

			
		//	if(m_StairState == Def.StairState.ALWAYS)
		//	{

		//	}
		//}
		public void SetLayer(int layer)
		{
			if (m_Layer == layer)
				return;

			if (m_Layer != 0)
				((CStrucEdit)m_Pilar.GetStruc()).RemoveBlockFromLayer(this);

			if(layer == 0)
			{
				m_LayerSR.enabled = false;
				SetAnchor(false);
				SetStairState(Def.StairState.NONE);
				SetLockState(Def.LockState.Unlocked);
				SetVoidState(Def.BlockVoid.NORMAL);

				var fullVoid = BlockMaterial.VoidMat[(int)m_BlockType];
				SetMaterialFamily(fullVoid.Family);
				//var topMat = fullVoid.TopPart;
				//var midMat = fullVoid.BottomPart;

				SetBlockType(Def.BlockType.NORMAL);

				m_TopMR.receiveShadows = false;
				m_TopMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				//m_TopMR.material = topMat.Mat;
				m_MidMR.receiveShadows = false;
				m_MidMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				//m_MidMR.material = midMat.Mat;

				if (m_Prop != null)
				{
					//m_Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, m_Prop.GetTotalHealth());
					m_Prop.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Prop.GetLE().GetCurrentHealth());
					m_Prop = null;
				}
				if (m_Monster != null)
				{
					//m_Monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, m_Monster.GetTotalHealth());
					m_Monster.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Monster.GetLE().GetCurrentHealth());
					m_Monster = null;
				}
				SetLength(0.5f);
				SetHeight(0f);
				SetMicroHeight(0f);
				SetRotation(Def.RotationState.Default);
				m_Layer = layer;
				return;
			}
			if (m_LayerSR == null)
				throw new Exception();
			m_LayerSR.sprite = EditorSprites.GetSprite($"LayerSprite_{layer}");
			m_LayerSR.enabled = !Manager.Mgr.HideInfo;
			m_TopMR.receiveShadows = true;
			m_TopMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			m_MidMR.receiveShadows = true;
			m_MidMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			m_Layer = layer;
			((CStrucEdit)m_Pilar.GetStruc()).AddBlockToLayer(layer, this);
		}
		public void SetMaterialFamily(MaterialFamily family, Def.StairType stairType = Def.StairType.COUNT)
		{
			m_MaterialFamily = family;
			if(m_MaterialFamily == null)
			{
				var struc = m_Pilar.GetStruc() as CStrucEdit;
				var layer = struc.GetLayers()[m_Layer - 1];
				if (layer.IsLinkedLayer)
					layer = struc.GetLayers()[m_LinkedTo - 1];

				var mat = BlockMaterial.BiomeMaterials[(int)layer.LayerType];
				m_TopMR.material = mat;
				m_MidMR.material = mat;
				return;
			}
			Def.StairType sType = m_StairType;
			if (stairType != Def.StairType.COUNT)
				sType = stairType;
			var set = m_MaterialFamily.GetSet(m_BlockType, sType);

			if (set == null || set.Length == 0)
			{
				var stonefamily = BlockMaterial.MaterialFamilies[BlockMaterial.FamilyDict["Stone"]];
				set = stonefamily.GetSet(m_BlockType, sType);
			}
				
			var matIdx = UnityEngine.Random.Range(0, set.Length);
			var fullMat = set[matIdx];
			SetTopMaterial(fullMat.TopPart);
			SetMidMaterial(fullMat.BottomPart);
		}
		public new void SetMicroHeight(float microHeight)
		{
			if (m_MicroHeight == microHeight)
				return;

			//if (m_BlockType == Def.BlockType.WIDE)
			//{
			//    if (m_HiddenBlocks == null || (m_HiddenBlocks != null && m_HiddenBlocks.Length != 3))
			//        throw new Exception("Trying to change the block height from a WIDE block, but the HiddenBlocks is invalid");

			//    for (int i = 0; i < m_HiddenBlocks.Length; ++i)
			//    {
			//        m_HiddenBlocks[i].SetMicroHeight(microHeight);
			//        for (int j = 0; j < m_HiddenBlocks[i].GetPilar().GetBlocks().Count; ++j)
			//        {
			//            var block = m_HiddenBlocks[i].GetPilar().GetBlocks()[j] as CBlockEdit;
			//            if (block == m_HiddenBlocks[i])
			//                continue;
			//            if (block.IsRemoved())
			//            {
			//                var parent = block.GetParentWIDE();
			//                if(parent != null)
			//                {
			//                    parent.SetMicroHeight(microHeight);
			//                }
			//            }
			//            else
			//            {
			//                block.SetMicroHeight(microHeight);
			//            }
			//        }
			//    }
			//}
			//else if (m_Removed)
			//{
			//    return;
			//}
			m_MicroHeight = microHeight;
			SetHeight(m_Height);
		}
		public void SetMonster(AI.CMonster monster) => m_Monster = monster;
		public override void SetProp(AI.CProp prop) => m_Prop = prop;
		public void SetStairState(Def.StairState state)
		{
			if (m_StairState == state)
				return;

			bool enable = !Manager.Mgr.HideInfo;
			var struc = m_Pilar.GetStruc() as CStrucEdit;
			switch (state)
			{
				case Def.StairState.NONE:
					m_StairType = Def.StairType.NORMAL;
					m_StairSR.enabled = false;
					SetBlockType(Def.BlockType.NORMAL);
					break;
				case Def.StairState.POSSIBLE:
					m_StairType = Def.StairType.NORMAL;
					SetAnchor(false);
					m_StairSR.enabled = enable;
					m_StairSR.sprite = EditorSprites.Sprites[EditorSprites.Dict["StairSprite"]];
					SetBlockType(Def.BlockType.NORMAL);
					break;
				case Def.StairState.ALWAYS:
					m_StairType = Def.StairType.NORMAL;
					SetAnchor(false);
					m_StairSR.enabled = false;

					if (m_Prop != null)
					{
						if (struc.GetLES().Contains(m_Prop.GetLE()))
							struc.GetLES().Remove(m_Prop.GetLE());

						m_Prop.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Prop.GetLE().GetCurrentHealth());
					}
					if (m_Monster != null)
					{
						if (struc.GetLES().Contains(m_Monster.GetLE()))
							struc.GetLES().Remove(m_Monster.GetLE());

						m_Monster.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Monster.GetLE().GetCurrentHealth());
					}

					SetBlockType(Def.BlockType.STAIRS);
					break;
				case Def.StairState.RAMP_POSSIBLE:
					m_StairType = Def.StairType.NORMAL;
					SetAnchor(false);
					m_StairSR.enabled = enable;
					m_StairSR.sprite = EditorSprites.Sprites[EditorSprites.Dict["RampSprite"]];
					SetBlockType(Def.BlockType.NORMAL);
					break;
				case Def.StairState.STAIR_OR_RAMP:
					m_StairType = Def.StairType.NORMAL;
					SetAnchor(false);
					m_StairSR.enabled = enable;
					m_StairSR.sprite = EditorSprites.Sprites[EditorSprites.Dict["RandomStairRampSprite"]];
					SetBlockType(Def.BlockType.NORMAL);
					break;
				case Def.StairState.RAMP_ALWAYS:
					m_StairType = Def.StairType.RAMP;
					SetAnchor(false);
					m_StairSR.enabled = false;

					if (m_Prop != null)
					{
						if (struc.GetLES().Contains(m_Prop.GetLE()))
							struc.GetLES().Remove(m_Prop.GetLE());

						m_Prop.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Prop.GetLE().GetCurrentHealth());
					}
					if (m_Monster != null)
					{
						if (struc.GetLES().Contains(m_Monster.GetLE()))
							struc.GetLES().Remove(m_Monster.GetLE());

						m_Monster.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Monster.GetLE().GetCurrentHealth());
					}

					SetBlockType(Def.BlockType.STAIRS);
					break;
			}
			if (m_Void == Def.BlockVoid.SEMIVOID)
			{
				if (state != Def.StairState.NONE && state != Def.StairState.ALWAYS && state != Def.StairState.RAMP_ALWAYS)
				{
					m_VoidSR.sprite = EditorSprites.GetSprite("SemiVoidSmallSprite");
				}
				else
				{
					m_VoidSR.sprite = EditorSprites.GetSprite("SemiVoidSprite");
				}
			}
			
			SetMaterialFamily(m_MaterialFamily);
			m_StairState = state;
		}
		public void SetAnchor(bool anchor)
		{
			if (anchor == m_Anchor)
				return;

			m_AnchorSR.enabled = anchor && !Manager.Mgr.HideInfo;
			if (anchor)
				SetStairState(Def.StairState.NONE);
			m_Anchor = anchor;
		}
		public void SetLockState(Def.LockState lockState)
		{
			switch (lockState)
			{
				case Def.LockState.Unlocked:
					m_LockSR.enabled = false;
					break;
				case Def.LockState.SemiLocked:
					m_LockSR.sprite = EditorSprites.GetSprite("SemiLockedSprite");
					m_LockSR.enabled = !Manager.Mgr.HideInfo;
					break;
				case Def.LockState.Locked:
					m_LockSR.sprite = EditorSprites.GetSprite("LockedSprite");
					m_LockSR.enabled = !Manager.Mgr.HideInfo;
					break;
			}
			m_Lock = lockState;
		}
		public void SetRemoved(bool removed)
		{
			m_TopMR.enabled = !removed;
			m_MidMR.enabled = !removed;
			m_BlockBC.enabled = !removed;

			if (removed)
			{
				m_LockSR.enabled = false;
				m_LayerSR.enabled = false;
				m_AnchorSR.enabled = false;
				m_StairSR.enabled = false;
				m_VoidSR.enabled = false;
			}
			else if (!Manager.Mgr.HideInfo)
			{
				m_LockSR.enabled = m_Lock != Def.LockState.Unlocked;
				m_LayerSR.enabled = m_Layer != 0;
				m_AnchorSR.enabled = m_Anchor;
				m_StairSR.enabled = GameUtils.IsStairPossible(m_StairState);
				m_VoidSR.enabled = m_Void != Def.BlockVoid.NORMAL;
			}

			m_Removed = removed;
		}
		public override void DestroyBlock(bool preserveEntities, bool instant = false)
		{
			SetLayer(0);
			//if(m_BlockType == Def.BlockType.WIDE)
			//{
			//	for(int i = 0; i < m_WideStackedIdx.Length; ++i)
			//	{
			//		var stackInfo = m_WideStackedIdx[i];
			//		var pilar = m_Pilar.GetStruc().GetPilars()[stackInfo.PilarWID];
			//		pilar.RemoveBlock(this);
			//	}
			//}
			m_Pilar.RemoveBlock(this);
			//m_Pilar.GetBlocks().Remove(this);
			var Struc = m_Pilar.GetStruc();
			if (m_Prop != null && !preserveEntities)
			{
				//m_Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, m_Prop.GetTotalHealth());
				//if (Struc != null && Struc.GetLivingEntities().Contains(m_Prop))
				//    Struc.GetLivingEntities().Remove(m_Prop);
				m_Prop.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Prop.GetLE().GetCurrentHealth());
				if (Struc != null && Struc.GetLES().Contains(m_Prop.GetLE()))
					Struc.GetLES().Remove(m_Prop.GetLE());
				m_Prop = null;
			}
			if (m_Monster != null && !preserveEntities)
			{
				//m_Monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, m_Monster.GetTotalHealth());
				//if (Struc != null && Struc.GetLivingEntities().Contains(m_Monster))
				//	Struc.GetLivingEntities().Remove(m_Monster);
				m_Monster.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Monster.GetLE().GetCurrentHealth());
				if (Struc != null && Struc.GetLES().Contains(m_Monster.GetLE()))
					Struc.GetLES().Remove(m_Monster.GetLE());
				m_Monster = null;
			}
			//var below = GetStackedBelow();
			//var above = GetStackedAbove();
			//if (below != null)
			//	below.GetStackedBlocksIdx()[1] = -1;
			//if (above != null)
			//	above.GetStackedBlocksIdx()[0] = -1;
			//if (m_StackedBlocks[0] != null)
			//	m_StackedBlocks[0].GetStackedBlocks()[1] = null;
			//if (m_StackedBlocks[1] != null)
			//	m_StackedBlocks[1].GetStackedBlocks()[0] = null;
			GameUtils.DeleteGameobject(gameObject, instant);
			GameUtils.DeleteGameobject(m_AnchorSR.gameObject, instant);
			GameUtils.DeleteGameobject(m_LayerSR.gameObject, instant);
			GameUtils.DeleteGameobject(m_StairSR.gameObject, instant);
			GameUtils.DeleteGameobject(m_LockSR.gameObject, instant);
			GameUtils.DeleteGameobject(m_VoidSR.gameObject, instant);
			for (int i = 0; i < m_Ants.Length; ++i)
			{
				if (m_Ants[i] != null)
					GameUtils.DeleteGameobject(m_Ants[i].gameObject, instant);
			}
		}
		public void SetVoidState(Def.BlockVoid blockVoid)
		{
			m_Void = blockVoid;
			if(!Manager.Mgr.HideInfo)
			{
				switch (m_Void)
				{
					case Def.BlockVoid.NORMAL:
						m_VoidSR.enabled = false;
						break;
					case Def.BlockVoid.SEMIVOID:
						if (m_StairState != Def.StairState.NONE && m_StairState != Def.StairState.ALWAYS && m_StairState != Def.StairState.RAMP_ALWAYS)
							m_VoidSR.sprite = EditorSprites.GetSprite("SemiVoidSmallSprite");
						else
							m_VoidSR.sprite = EditorSprites.GetSprite("SemiVoidSprite");
						m_VoidSR.enabled = true;
						break;
					case Def.BlockVoid.FULLVOID:
						m_VoidSR.sprite = EditorSprites.GetSprite("VoidSprite");
						m_VoidSR.enabled = true;
						break;
				}
			}
		}
		public void SetTopAnt(int version = -1, Def.AntTopDirection direction =
			Def.AntTopDirection.COUNT)
		{
			if (m_Ants[0] != null)
			{
				GameUtils.DeleteGameobject(m_Ants[0].gameObject);
				m_Ants[0] = null;
			}
			if (direction == Def.AntTopDirection.COUNT || version < 0)
			{
				return;
			}
			AntDef def = null;
			switch (direction)
			{
				case Def.AntTopDirection.SOUTH_NORTH:
				case Def.AntTopDirection.NORTH_SOUTH:
				case Def.AntTopDirection.EAST_WEST:
				case Def.AntTopDirection.WEST_EAST:
					if (version >= AntManager.StraightAnts.Count)
						throw new Exception("Trying to set a TOP Straight Ant but with a higher version than the available ones.");
					def = AntManager.StraightAnts[version];
					break;
				case Def.AntTopDirection.SOUTH_EAST:
				case Def.AntTopDirection.WEST_SOUTH:
				case Def.AntTopDirection.NORTH_WEST:
				case Def.AntTopDirection.EAST_NORTH:
					if (version >= AntManager.TurnRightAnts.Count)
						throw new Exception("Trying to set a TOP Straight Ant but with a higher version than the available ones.");
					def = AntManager.TurnRightAnts[version];
					break;
				case Def.AntTopDirection.SOUTH_WEST:
				case Def.AntTopDirection.NORTH_EAST:
				case Def.AntTopDirection.WEST_NORTH:
				case Def.AntTopDirection.EAST_SOUTH:
					if (version >= AntManager.TurnLeftAnts.Count)
						throw new Exception("Trying to set a TOP Straight Ant but with a higher version than the available ones.");
					def = AntManager.TurnLeftAnts[version];
					break;
			}
			var ant = m_Ants[0] = new GameObject(gameObject.name + "_ANT_TOP").AddComponent<AntComponent>();
			ant.SetAnt(def, (int)direction);
			ant.transform.SetParent(m_Pilar.transform);
			var baseHeight = m_Height + m_MicroHeight;
			ant.transform.localPosition = new Vector3(0.501f, baseHeight + 0.01f, 0.501f);
			switch (direction)
			{
				case Def.AntTopDirection.SOUTH_NORTH:
					ant.transform.localRotation = Quaternion.Euler(270f, 0f, 90f);
					break;
				case Def.AntTopDirection.NORTH_SOUTH:
					ant.transform.localRotation = Quaternion.Euler(270f, 0f, 90f);
					ant.Renderer.flipY = true;
					break;
				case Def.AntTopDirection.EAST_WEST:
					ant.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
					break;
				case Def.AntTopDirection.WEST_EAST:
					ant.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
					ant.Renderer.flipY = true;
					break;
				case Def.AntTopDirection.SOUTH_WEST:
					ant.transform.localRotation = Quaternion.Euler(270f, 90f, 0f);
					break;
				case Def.AntTopDirection.SOUTH_EAST:
					ant.transform.localRotation = Quaternion.Euler(270f, 90f, 0f);
					break;
				case Def.AntTopDirection.NORTH_WEST:
					ant.transform.localRotation = Quaternion.Euler(270f, 270f, 0f);
					break;
				case Def.AntTopDirection.NORTH_EAST:
					ant.transform.localRotation = Quaternion.Euler(270f, 270f, 0f);
					break;
				case Def.AntTopDirection.WEST_NORTH:
					ant.transform.localRotation = Quaternion.Euler(270f, 180f, 0f);
					break;
				case Def.AntTopDirection.WEST_SOUTH:
					ant.transform.localRotation = Quaternion.Euler(270f, 180f, 0f);
					break;
				case Def.AntTopDirection.EAST_NORTH:
					ant.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
					break;
				case Def.AntTopDirection.EAST_SOUTH:
					ant.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
					break;
			}
			ant.Renderer.flipX = true;
			ant.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
		}
		public void SetSideAnt(int version = -1,
			Def.SpaceDirection direction = Def.SpaceDirection.COUNT,
			bool upDirection = true)
		{
			if (direction == Def.SpaceDirection.COUNT)
				return;
			if (m_Ants[((int)direction) + 1] != null)
			{
				GameObject.Destroy(m_Ants[((int)direction) + 1].gameObject);
				m_Ants[(int)direction + 1] = null;
			}

			if (version < 0)
			{
				return;
			}

			if (version >= AntManager.StraightAnts.Count)
				throw new Exception("Trying to set a SIDE Ant but with a higher version than the available ones.");

			var ant = m_Ants[((int)direction) + 1] = new GameObject(gameObject.name + "_ANT_" + direction.ToString()).AddComponent<AntComponent>();
			ant.SetAnt(AntManager.StraightAnts[version], ((int)direction) * 2 + (upDirection ? 0 : 1));
			ant.transform.SetParent(m_Pilar.transform);

			var baseHeight = m_Height + m_MicroHeight;
			switch (direction)
			{
				case Def.SpaceDirection.NORTH:
					ant.transform.localPosition = new Vector3(-0.02f, baseHeight - 0.5f, 0.501f);
					ant.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
					ant.Renderer.flipY = !upDirection;
					break;
				case Def.SpaceDirection.SOUTH:
					ant.transform.localPosition = new Vector3(1.02f, baseHeight - 0.5f, 0.501f);
					ant.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
					ant.Renderer.flipY = !upDirection;
					ant.Renderer.flipX = true;
					break;
				case Def.SpaceDirection.EAST:
					ant.transform.localPosition = new Vector3(0.501f, baseHeight - 0.5f, 1.02f);
					ant.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
					ant.Renderer.flipY = !upDirection;
					break;
				case Def.SpaceDirection.WEST:
					ant.transform.localPosition = new Vector3(0.501f, baseHeight - 0.5f, -0.02f);
					ant.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
					ant.Renderer.flipY = !upDirection;
					break;
			}
			ant.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
		}
		public void SetDeco(int decoID, Def.DecoPosition position)
		{

		}
		public void SetIDXIE(int idxie)
		{
			m_IDXIE = idxie;
		}
		public override void SetBlockFloat(float offset = 0, float speed = 0)
		{
			throw new NotImplementedException();
		}
		public void SetHighlighted(bool highlight)
		{
			if (highlight == m_Highlighted)
				return;

			Color color = Color.red;
			bool midOutline = false;
			if(m_Selected)
			{
				highlight = true;
				color = Color.green;
				midOutline = true;
			}
			if (m_TopOutline.OutlineColor != color)
				m_TopOutline.OutlineColor = color;

			if (midOutline && m_MidOutline.OutlineColor != color)
				m_MidOutline.OutlineColor = color;

			m_TopOutline.enabled = highlight;
			if(midOutline)
				m_MidOutline.enabled = highlight;

			if (!highlight)
				m_MidOutline.enabled = false;

			m_Highlighted = highlight;
		}
		public void SetSelected(bool selected)
		{
			if (m_Selected == selected)
				return;

			m_Selected = selected;
			SetHighlighted(selected);
		}
		public override Def.BlockState GetBlockState()
		{
			return Def.BlockState.Edit;
		}
		protected override void Enable(bool enable)
		{
			if (m_BlockBC != null)
				m_BlockBC.enabled = enable;
			if (enable)
			{
				if (!m_Removed)
				{
					if (m_TopMR != null)
						m_TopMR.enabled = true;
					if (m_MidMR != null)
						m_MidMR.enabled = true;

					if (!Manager.Mgr.HideInfo)
					{
						if (m_Anchor && m_AnchorSR != null)
							m_AnchorSR.enabled = true;
						if (m_Layer != 0 && m_LayerSR != null)
							m_LayerSR.enabled = true;
						if (m_Lock != Def.LockState.Unlocked && m_LockSR != null)
							m_LockSR.enabled = true;
						if (GameUtils.IsStairPossible(m_StairState) && m_StairSR != null)
							m_StairSR.enabled = true;
						if (m_Void != Def.BlockVoid.NORMAL && m_VoidSR != null)
							m_VoidSR.enabled = true;
					}
				}
			}
			else
			{
				if(m_TopMR != null)
					m_TopMR.enabled = false;
				if(m_MidMR != null)
					m_MidMR.enabled = false;

				if(m_AnchorSR != null)
					m_AnchorSR.enabled = false;
				if(m_LayerSR != null)
					m_LayerSR.enabled = false;
				if(m_LockSR != null)
					m_LockSR.enabled = false;
				if(m_StairSR != null)
					m_StairSR.enabled = false;
				if(m_VoidSR != null)
					m_VoidSR.enabled = false;
			}
			if (m_Prop != null)
				m_Prop.enabled = enable;
			if (m_Monster != null)
				m_Monster.enabled = enable;
		}
		protected override void PilarNameChange(int prevNameLength)
		{
			m_AnchorSR.gameObject.name = m_Pilar.gameObject.name + m_AnchorSR.gameObject.name.Substring(prevNameLength);
			m_StairSR.gameObject.name = m_Pilar.gameObject.name + m_StairSR.gameObject.name.Substring(prevNameLength);
			m_LockSR.gameObject.name = m_Pilar.gameObject.name + m_LockSR.gameObject.name.Substring(prevNameLength);
			m_LayerSR.gameObject.name = m_Pilar.gameObject.name + m_LayerSR.gameObject.name.Substring(prevNameLength);
		}
		protected override void LateInitBlock()
		{
			m_IDXIE = -1;

			m_TopOutline = m_TopMR.gameObject.AddComponent<Outline>();
			m_TopOutline.enabled = false;
			m_TopOutline.OutlineMode = Outline.Mode.OutlineAll;
			m_MidOutline = m_MidMR.gameObject.AddComponent<Outline>();
			m_MidOutline.enabled = false;
			m_MidOutline.OutlineMode = Outline.Mode.OutlineAll;

			// Creation
			m_AnchorSR = new GameObject(gameObject.name + "_anchor").AddComponent<SpriteRenderer>();
			m_LayerSR = new GameObject(gameObject.name + "_layer").AddComponent<SpriteRenderer>();
			m_StairSR = new GameObject(gameObject.name + "_stair").AddComponent<SpriteRenderer>();
			m_LockSR = new GameObject(gameObject.name + "_lock").AddComponent<SpriteRenderer>();
			m_VoidSR = new GameObject(gameObject.name + "_void").AddComponent<SpriteRenderer>();

			// RenderLayer set
			m_AnchorSR.gameObject.layer = Def.RCLayerBlock;
			m_LayerSR.gameObject.layer = Def.RCLayerBlock;
			m_StairSR.gameObject.layer = Def.RCLayerBlock;
			m_LockSR.gameObject.layer = Def.RCLayerBlock;
			m_VoidSR.gameObject.layer = Def.RCLayerBlock;

			// Parent set
			m_AnchorSR.transform.SetParent(m_Pilar.transform);
			m_LayerSR.transform.SetParent(m_Pilar.transform);
			m_StairSR.transform.SetParent(m_Pilar.transform);
			m_LockSR.transform.SetParent(m_Pilar.transform);
			m_VoidSR.transform.SetParent(m_Pilar.transform);

			// Position, rotation and scale
			var pos = m_Pilar.transform.position;
			pos.y = m_Height + m_MicroHeight;

			m_AnchorSR.transform.Translate(new Vector3(pos.x, pos.y + 0.01f, pos.z), Space.World);
			m_LayerSR.transform.Translate(new Vector3(pos.x, pos.y + 0.02f, pos.z), Space.World);
			m_StairSR.transform.Translate(new Vector3(pos.x, pos.y + 0.03f, pos.z), Space.World);
			m_LockSR.transform.Translate(new Vector3(pos.x, pos.y + 0.04f, pos.z), Space.World);
			m_VoidSR.transform.Translate(new Vector3(pos.x, pos.y + 0.05f, pos.z), Space.World);

			m_AnchorSR.transform.Rotate(Vector3.right, 90.0f, Space.World);
			m_AnchorSR.transform.Rotate(Vector3.up, -90.0f, Space.World);
			m_LayerSR.transform.Rotate(Vector3.right, 90.0f, Space.World);
			m_LayerSR.transform.Rotate(Vector3.up, -90.0f, Space.World);
			m_StairSR.transform.Rotate(Vector3.right, 90.0f, Space.World);
			m_StairSR.transform.Rotate(Vector3.up, -90.0f, Space.World);
			m_LockSR.transform.Rotate(Vector3.right, 90.0f, Space.World);
			m_LockSR.transform.Rotate(Vector3.up, -90.0f, Space.World);
			m_VoidSR.transform.Rotate(Vector3.right, 90.0f, Space.World);
			m_VoidSR.transform.Rotate(Vector3.up, -90.0f, Space.World);

			m_AnchorSR.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
			m_LayerSR.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
			m_StairSR.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
			m_LockSR.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
			m_VoidSR.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);

			// Sprite texture set
			{
				m_AnchorSR.drawMode = SpriteDrawMode.Simple;
				m_AnchorSR.spriteSortPoint = SpriteSortPoint.Center;
				m_AnchorSR.material = Materials.GetMaterial(Def.Materials.Sprite);
				m_AnchorSR.sprite = EditorSprites.GetSprite("AnchorSprite");
				m_AnchorSR.enabled = false;
			}
			{
				m_LayerSR.drawMode = SpriteDrawMode.Simple;
				m_LayerSR.spriteSortPoint = SpriteSortPoint.Center;
				m_LayerSR.material = Materials.GetMaterial(Def.Materials.Sprite);
				m_LayerSR.enabled = false;
			}
			{
				m_StairSR.drawMode = SpriteDrawMode.Simple;
				m_StairSR.spriteSortPoint = SpriteSortPoint.Center;
				m_StairSR.material = Materials.GetMaterial(Def.Materials.Sprite);

				m_StairSR.sprite = EditorSprites.GetSprite("StairSprite");
				m_StairSR.enabled = false;
			}
			{
				m_LockSR.drawMode = SpriteDrawMode.Simple;
				m_LockSR.spriteSortPoint = SpriteSortPoint.Center;
				m_LockSR.material = Materials.GetMaterial(Def.Materials.Sprite);
				m_LockSR.enabled = false;
			}
			{
				m_VoidSR.drawMode = SpriteDrawMode.Simple;
				m_VoidSR.spriteSortPoint = SpriteSortPoint.Center;
				m_VoidSR.material = Materials.GetMaterial(Def.Materials.Sprite);
				m_VoidSR.enabled = false;
			}
		}
		protected override void UpdateLength()
		{
			if (m_Layer != 0)
			{
				m_MidMR.receiveShadows = true;
				m_MidMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			}
		}
		protected override void UpdateHeight(float heightDiff)
		{
			m_AnchorSR.transform.Translate(new Vector3(0f, heightDiff, 0f), Space.World);
			m_LockSR.transform.Translate(new Vector3(0f, heightDiff, 0f), Space.World);
			m_StairSR.transform.Translate(new Vector3(0f, heightDiff, 0f), Space.World);
			m_LayerSR.transform.Translate(new Vector3(0f, heightDiff, 0f), Space.World);
			m_VoidSR.transform.Translate(new Vector3(0f, heightDiff, 0f), Space.World);
		}
		public CBlock ConvertToGame()
		{
			var def = new BlockDef()
			{
				BlockBC = m_BlockBC,
				TopMR = m_TopMR,
				MidMR = m_MidMR,
				Prop = m_Prop,
				Monster = m_Monster,
				MaterialFamily = m_MaterialFamily,
				Layer = m_Layer,
				Length = m_Length,
				MicroHeight = m_MicroHeight,
				Pilar = m_Pilar,
				Ants = m_Ants,
				Deco = m_Deco,
				StackedBlocksIdx = m_StackedBlocksIdx,
				BlockType = m_BlockType,
				StairType = m_StairType,
				Rotation = m_Rotation,
				Height = m_Height,
				TopMaterialPart = m_TopMaterialPart,
				MidMaterialPart = m_MidMaterialPart,
			};
			GameUtils.DeleteGameobject(m_AnchorSR.gameObject);
			GameUtils.DeleteGameobject(m_LayerSR.gameObject);
			GameUtils.DeleteGameobject(m_StairSR.gameObject);
			GameUtils.DeleteGameobject(m_LockSR.gameObject);
			GameUtils.DeleteGameobject(m_VoidSR.gameObject);
			m_Pilar.GetBlocks().Remove(this);
			Component.Destroy(this);
			var gameBlock = gameObject.AddComponent<CBlock>();
			gameBlock.InitGameBlock(def);
			return gameBlock;
		}
	}
}
