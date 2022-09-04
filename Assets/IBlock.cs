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

namespace Assets
{
	[Serializable]
	public struct WideStackLinks
	{
		public int PilarWID;
		public int[] StackIdx;
	}
	public abstract class IBlock : MonoBehaviour
	{
		[SerializeField] protected BoxCollider m_BlockBC;
		[SerializeField] protected MeshRenderer m_TopMR;
		[SerializeField] protected MeshRenderer m_MidMR;
		//protected Outline m_TopOutline;
		//protected Outline m_MidOutline;
		[SerializeField] protected AI.CProp m_Prop;
		[SerializeField] protected MaterialFamily m_MaterialFamily;
		[SerializeField] protected int m_Layer;
		[SerializeField] protected int m_PilarIndex;
		[SerializeField] protected float m_Length;
		[SerializeField] protected float m_Height;
		[SerializeField] protected float m_MicroHeight;
		[SerializeField] protected float m_BlockFloatOffset;
		[SerializeField] protected float m_BlockFloatSpeed;
		[SerializeReference] protected CPilar m_Pilar;
		[SerializeField] protected AntComponent[] m_Ants;
		[SerializeField] protected SpriteBackendSprite[] m_Deco;
		[SerializeField] protected int[] m_StackedBlocksIdx;
		[SerializeField] protected WideStackLinks[] m_WideStackedIdx;
		//protected IBlock[] m_StackedBlocks;
		[SerializeField] protected Def.BlockType m_BlockType;
		[SerializeField] protected Def.StairType m_StairType;
		[SerializeField] protected Def.RotationState m_Rotation;

		public BoxCollider GetCollider() => m_BlockBC;
		public MeshRenderer GetTopMR() => m_TopMR;
		public MeshRenderer GetMidMR() => m_MidMR;
		public MaterialFamily GetMaterialFamily() => m_MaterialFamily;
		public float GetLength() => m_Length;
		public float GetHeight() => m_Height;
		public float GetMicroHeight() => m_MicroHeight;
		public float GetBlockFloatOffset() => m_BlockFloatOffset;
		public float GetBlockFloatSpeed() => m_BlockFloatSpeed;
		public CPilar GetPilar() => m_Pilar;
		public Def.BlockType GetBlockType() => m_BlockType;
		public Def.StairType GetStairType() => m_StairType;
		public AntComponent[] GetAnts() => m_Ants;
		public SpriteBackendSprite[] GetDeco() => m_Deco;
		public Def.RotationState GetRotation() => m_Rotation;
		public int GetLayer() => m_Layer;
		public int[] GetStackedBlocksIdx() => m_StackedBlocksIdx;
		public WideStackLinks[] GetWideStackedIdx() => m_WideStackedIdx;
		public bool IsStackLinkValid(int position)
		{
			int stackIdx = position > 0 ? 1 : 0;
			var pilarIdx = m_StackedBlocksIdx[stackIdx];
			return pilarIdx >= 0 && pilarIdx != m_PilarIndex && m_Pilar.GetBlocks().Count > pilarIdx && m_Pilar.GetBlocks()[pilarIdx] != null;
		}
		protected IBlock GetStackedBlock(int pilarIdx)
		{
			if (pilarIdx < 0 || pilarIdx == m_PilarIndex || m_Pilar.GetBlocks().Count <= pilarIdx)
				return null;
			return m_Pilar.GetBlocks()[pilarIdx];
		}
		public IBlock GetStackedAbove()
		{
			return GetStackedBlock(m_StackedBlocksIdx[1]);
		}
		public IBlock GetStackedBelow()
		{
			return GetStackedBlock(m_StackedBlocksIdx[0]);
		}
		public abstract void DestroyBlock(bool preserveEntities, bool instant = false);
		protected abstract void UpdateLength();
		public void SetLength(float length = 0.5f)
		{
			float maxHeight, minHeight;
			if (m_BlockType == Def.BlockType.WIDE)
			{
				maxHeight = Blocks.MaxWideHeight;
				minHeight = Blocks.MinWideHeight;
			}
			else
			{
				maxHeight = Blocks.MaxNormalHeight;
				minHeight = Blocks.MinNormalHeight;
			}
			if (length == 0f)
				length = maxHeight;
			length = Mathf.Clamp(Mathf.Abs(length), Mathf.Abs(minHeight), Mathf.Abs(maxHeight));
			if (length == m_Length)
				return;
			Blocks.SetBlock(m_MidMR.gameObject, m_BlockType, Def.BlockMeshType.MID, length);
			//BlockMeshDef.SetBlock(m_MidMR.gameObject, m_BlockType, Def.BlockMeshType.MID, length);
			UpdateLength();
			Vector2 stairOffset = Vector2.zero;
			if (m_BlockType == Def.BlockType.STAIRS)
			{
				stairOffset.x = 0.5f;
				stairOffset.y = 0.25f;
			}
			float width = m_BlockType == Def.BlockType.WIDE ? 2f : 1f;
			width *= 1f + Def.BlockSeparation;
			m_BlockBC.size = new Vector3(width, (length + stairOffset.x) + 0.075f, width);
			m_BlockBC.center = new Vector3(m_BlockBC.center.x, (length * -0.5f + stairOffset.y) - 0.0375f, m_BlockBC.center.z);
			m_Length = length;
		}
		protected abstract void UpdateHeight(float heightDiff);
		public void SetHeight(float height = 0f)
		{
			var nPos = height + m_MicroHeight;
			var diff = nPos - transform.position.y;
			//if (height > 100f || height < -100f)
			//    throw new Exception();
			m_Height = height;
			if (diff == 0f)
				return;
			transform.Translate(0f, diff, 0f, Space.World);
			if (m_Ants != null)
			{
				for (int i = 0; i < m_Ants.Length; ++i)
				{
					if (m_Ants[i] != null)
					{
						m_Ants[i].transform.Translate(0f, diff, 0f, Space.World);
					}
				}
			}
			UpdateHeight(diff);
		}
		IBlock GetBlockAbove(float wantedHeight)
		{
			for (int i = 0; i < m_Pilar.GetBlocks().Count; ++i)
			{
				var block = m_Pilar.GetBlocks()[i];
				var len = block.GetLength();
				if (len == 3.4f)
					len = 3.5f;
				if ((block.GetHeight() - len) == wantedHeight)
					return block;
			}
			return null;
		}
		IBlock GetBlockBelow(float wantedHeight)
		{
			for (int i = 0; i < m_Pilar.GetBlocks().Count; ++i)
			{
				var block = m_Pilar.GetBlocks()[i];
				float h = block.GetHeight() + (block.GetBlockType() == Def.BlockType.STAIRS ? 0.5f : 0f);
				if (h == wantedHeight)
					return block;
			}
			return null;
		}
		void SetInfoToIE(CBlockEdit block, bool length = false)
		{
			//if (block.GetLockState() != Def.LockState.Locked)
			//	block.SetLockState(Def.LockState.SemiLocked);
			var strucIE = Structures.Strucs[((CStrucEdit)m_Pilar.GetStruc()).IDXIE];

			var blockIE = strucIE.GetBlocks()[block.GetIDXIE()];
			blockIE.SetHeight(m_Height);
			if(length)
			{
				blockIE.SetFlag(IE.V4.BlockIE.Flag.Length, true);
				blockIE.SetLength(m_Length);
			}

			if(IsStackLinkValid(0))
			{
				var below = GetStackedBelow() as CBlockEdit;
				var belowIE = strucIE.GetBlocks()[below.GetIDXIE()];
				blockIE.StackedIdx[0] = (short)below.GetIDXIE();
				belowIE.StackedIdx[1] = (short)block.GetIDXIE();
			}
			else
			{
				blockIE.StackedIdx[0] = -1;
			}
			if(IsStackLinkValid(1))
			{
				var above = GetStackedAbove() as CBlockEdit;
				var aboveIE = strucIE.GetBlocks()[above.GetIDXIE()];
				blockIE.StackedIdx[1] = (short)above.GetIDXIE();
				aboveIE.StackedIdx[0] = (short)block.GetIDXIE();
			}
			else
			{
				blockIE.StackedIdx[1] = -1;
			}
		}
		void IncreaseLengthBottomNormal(bool manual)
		{
			SetLength(m_Length + 0.5f);
			if(manual)
				SetInfoToIE(this as CBlockEdit, true);
		}
		void DecreaseLengthBottomNormal(bool manual)
		{
			float amount;
			if (m_Length != 3.4f)
				amount = 0.5f;
			else
				amount = 0.4f;
			SetLength(m_Length - amount);
			if (manual)
				SetInfoToIE(this as CBlockEdit, true);
		}
		void IncreaseLengthBottomShift(bool manual)
		{
			SetLength(m_Length + 0.5f);
			if (manual)
				SetInfoToIE(this as CBlockEdit, true);
			IncreaseHeight(false, false, manual);
		}
		void DecreaseLengthBottomShift(bool manual)
		{
			float amount;
			if (m_Length != 3.4f)
				amount = 0.5f;
			else
				amount = 0.4f;
			SetLength(m_Length - amount);
			if (manual)
				SetInfoToIE(this as CBlockEdit, true);
			DecreaseHeight(false, false, manual);
		}
		void IncreaseLengthOtherNormal(bool manual)
		{
			SetLength(m_Length + 0.5f);
			if (manual)
				SetInfoToIE(this as CBlockEdit, true);
			IncreaseHeight(true, false, manual);
		}
		void DecreaseLengthOtherNormal(bool manual)
		{
			float amount;
			if (m_Length != 3.4f)
				amount = 0.5f;
			else
				amount = 0.4f;
			SetLength(m_Length - amount);
			if (manual)
				SetInfoToIE(this as CBlockEdit, true);
			DecreaseHeight(true, false, manual);
		}
		void IncreaseLengthOtherShift(bool manual)
		{
			SetLength(m_Length + 0.5f);
			if (manual)
				SetInfoToIE(this as CBlockEdit, true);
			var below = GetStackedBelow();
			if(below != null) // lower it recursively
			{
				below.DecreaseHeight(false, true, manual);
			}
			else // Check if we touch a block
			{
				below = GetBlockBelow(m_Height - m_Length);
				if(below != null) // Create the stackLink
				{
					m_StackedBlocksIdx[0] = below.m_PilarIndex;
					below.m_StackedBlocksIdx[1] = m_PilarIndex;
					if (below.m_MicroHeight != m_MicroHeight)
						ChangeMicroheight(below.m_MicroHeight);
				}
			}
		}
		void DecreaseLengthOtherShift(bool manual)
		{
			float amount;
			if (m_Length != 3.4f)
				amount = 0.5f;
			else
				amount = 0.4f;
			SetLength(m_Length - amount);
			if (manual)
				SetInfoToIE(this as CBlockEdit, true);

			var below = GetStackedBelow();
			if(below != null)
			{
				below.IncreaseHeight(false, true, manual);
			}
		}
		// Increases the length by 0.5 and checks stacks
		public void IncreaseLength(bool shift = false, bool manual = false)
		{
			if ((m_BlockType != Def.BlockType.WIDE && (m_Length == 3.4f)) ||
				(m_BlockType == Def.BlockType.WIDE && (m_Length == 2.5f)))
				return; // Maximum Length

			bool bottomBlock = m_PilarIndex == 0;
			if (bottomBlock)
			{
				if (!shift)
					IncreaseLengthBottomNormal(manual);
				else
					IncreaseLengthBottomShift(manual);
			}
			else
			{
				if (!shift)
					IncreaseLengthOtherNormal(manual);
				else
					IncreaseLengthOtherShift(manual);
			}
		}
		// Decreases the length by 0.5 and checks stacks
		public void DecreaseLength(bool shift = false, bool manual = false)
		{
			if (m_Length == 0.5f)
				return; // Minimum Length

			bool bottomBlock = m_PilarIndex == 0;
			if (bottomBlock)
			{
				if (!shift)
					DecreaseLengthBottomNormal(manual);
				else
					DecreaseLengthBottomShift(manual);
			}
			else
			{
				if (!shift)
					DecreaseLengthOtherNormal(manual);
				else
					DecreaseLengthOtherShift(manual);
			}
		}
		// Increases the height by 0.5 and checks stacks
		public void IncreaseHeight(bool fromBelow = false, bool fromAbove = false, bool manual = false)
		{
			SetHeight(m_Height + 0.5f);
			if (manual)
				SetInfoToIE(this as CBlockEdit);

			if (!fromAbove) // If this was not called by the block above
			{
				var above = GetStackedAbove();
				if (above != null) // Increase the above height recursively
				{
					above.IncreaseHeight(true, false, manual);
				}
				else // Check if we hit a block above
				{
					var stairOffset = m_BlockType != Def.BlockType.STAIRS ? 0f : 0.5f;
					above = GetBlockAbove(m_Height + stairOffset);
					if (above != null) // create the stackLink
					{
						m_StackedBlocksIdx[1] = above.m_PilarIndex;
						above.m_StackedBlocksIdx[0] = m_PilarIndex;

						if(above.m_MicroHeight != m_MicroHeight) // Update the microheight from above
							above.ChangeMicroheight(m_MicroHeight);
					}
				}
			}

			if (!fromBelow) // If this was not called by the block below
			{
				var below = GetStackedBelow();
				if (below != null) // Increase the below height recursively
				{
					below.IncreaseHeight(false, true, manual);
				}
			}
		}
		// Decreases the height by 0.5 and checks stacks
		public void DecreaseHeight(bool fromBelow = false, bool fromAbove = false, bool manual = false)
		{
			SetHeight(m_Height - 0.5f);
			if (manual)
				SetInfoToIE(this as CBlockEdit);

			if(!fromAbove) // If this was not called by the block above
			{
				var above = GetStackedAbove();
				if(above != null) // Decrease the above height recursively
				{
					above.DecreaseHeight(true, false, manual);
				}
			}

			if(!fromBelow) // If this was not called by the block below
			{
				var below = GetStackedBelow();
				if(below != null) // Decrease the below height recursively
				{
					below.DecreaseHeight(false, true, manual);
				}
				else // Check if we hit a block below
				{
					below = GetBlockBelow(m_Height - m_Length);
					if(below != null) // create the stackLink
					{
						m_StackedBlocksIdx[0] = below.m_PilarIndex;
						below.m_StackedBlocksIdx[1] = m_PilarIndex;
						if (below.m_MicroHeight != m_MicroHeight) // Update the microheight from below
							ChangeMicroheight(below.m_MicroHeight);
					}
				}
			}
		}
		// Changes the microheight for this block and their above ones
		public void ChangeMicroheight(float mHeight)
		{
			SetMicroHeight(mHeight);
			var above = GetStackedAbove();
			if(above != null)
				above.ChangeMicroheight(mHeight);
		}
		public void SetRotation(Def.RotationState rotation)
		{
			var pos = m_Pilar.transform.position;
			pos.y = m_Pilar.transform.position.y + m_Height + m_MicroHeight;
			transform.SetPositionAndRotation(pos, transform.parent.rotation);
			float wideMult = m_BlockType == Def.BlockType.WIDE ? 2f : 1f;
			switch (rotation)
			{
				case Def.RotationState.Default:
					transform.Rotate(Vector3.up, 90f, Space.Self);
					break;
				case Def.RotationState.Right:
					transform.Translate(Vector3.right * wideMult, Space.Self);
					break;
				case Def.RotationState.Half:
					transform.Translate(new Vector3(1f, 0f, 1f) * wideMult, Space.Self);
					transform.Rotate(Vector3.up, 270f, Space.Self);
					break;
				case Def.RotationState.Left:
					transform.Translate(Vector3.forward * wideMult, Space.Self);
					transform.Rotate(Vector3.up, 180f, Space.Self);
					break;
			}
			m_Rotation = rotation;
		}
		//public void IncreaseLengthCheck(bool invalidateShift = false, bool manual = false)
		//{
		//	if (m_Layer == 0)
		//		return;

		//	bool shift = Input.GetKey(KeyCode.LeftShift);
		//	if (invalidateShift)
		//		shift = false;

		//	bool growUp = m_PilarIndex != 0;





		//	//var curPilarID = m_Pilar.GetBlocks().IndexOf(this);
		//	if (shift)
		//		growUp = !growUp;

		//	var stackedAbove = GetStackedAbove();
		//	var stackedBelow = GetStackedBelow();
		//	if(stackedAbove != null)
		//	{
		//		if(growUp)
		//		{
		//			SetHeight(m_Height + 0.5f);
		//			if(manual)
		//				SetInfoToIE((CBlockEdit)this);
		//			stackedAbove.SetHeight(stackedAbove.GetHeight() + 0.5f);
		//			stackedAbove.IncreaseHeightCheck(false, true, manual);
		//			//stackedAbove.IncreaseLengthCheck(invalidateShift || (m_PilarIndex == 0 && shift));
		//		}
		//		else
		//		{
		//			if(stackedBelow != null)
		//			{
		//				stackedBelow.SetHeight(stackedBelow.m_Height - 0.5f);
		//				if (manual)
		//					SetInfoToIE(stackedBelow as CBlockEdit);
		//				if (stackedBelow.GetPilarIndex() != 0)
		//					stackedBelow.IncreaseLengthCheck();
		//			}
		//			else
		//			{
		//				var len = m_Length == 3.4f ? 3.5f : m_Length;
		//				stackedBelow = GetBlockBelow(m_Height - len);
		//				if(stackedBelow != null)
		//				{
		//					m_StackedBlocksIdx[0] = stackedBelow.m_PilarIndex;
		//					stackedBelow.m_StackedBlocksIdx[1] = m_PilarIndex;
		//					SetMicroHeight(stackedBelow.GetMicroHeight());
		//					GameUtils.UpdateMicroheightAbove(this);
		//				}
		//			}
		//		}
		//	}
		//	else if(stackedBelow != null)
		//	{
		//		if(growUp)
		//		{
		//			SetHeight(m_Height + 0.5f);
		//			if (manual)
		//				SetInfoToIE((CBlockEdit)this);
		//			stackedAbove = GetBlockAbove(m_Height + (m_BlockType == Def.BlockType.STAIRS ? 0.5f : 0f));
		//			if(stackedAbove != null)
		//			{
		//				m_StackedBlocksIdx[1] = stackedAbove.m_PilarIndex;
		//				stackedAbove.m_StackedBlocksIdx[0] = m_PilarIndex;
		//				stackedAbove.SetMicroHeight(m_MicroHeight);
		//				GameUtils.UpdateMicroheightAbove(stackedAbove);
		//			}
		//		}
		//		else
		//		{
		//			stackedBelow.SetHeight(stackedBelow.m_Height - 0.5f);
		//			if (manual)
		//				SetInfoToIE(stackedBelow as CBlockEdit);
		//			if (stackedBelow.m_PilarIndex != 0)
		//				stackedBelow.IncreaseLengthCheck();
		//		}
		//	}
		//	else
		//	{
		//		if(growUp)
		//		{
		//			SetHeight(m_Height + 0.5f);
		//			stackedAbove = GetBlockAbove(m_Height + (m_BlockType == Def.BlockType.STAIRS ? 0.5f : 0f));
		//			if(stackedAbove != null)
		//			{
		//				m_StackedBlocksIdx[1] = stackedAbove.m_PilarIndex;
		//				stackedAbove.m_StackedBlocksIdx[0] = m_PilarIndex;
		//				stackedAbove.SetMicroHeight(m_MicroHeight);
		//				GameUtils.UpdateMicroheightAbove(stackedAbove);
		//			}
		//		}
		//		else
		//		{
		//			var len = m_Length == 3.4f ? 3.5f : m_Length;
		//			stackedBelow = GetBlockBelow(m_Height - len);
		//			if(stackedBelow != null)
		//			{
		//				m_StackedBlocksIdx[0] = stackedBelow.m_PilarIndex;
		//				stackedBelow.m_StackedBlocksIdx[1] = m_PilarIndex;
		//				SetMicroHeight(stackedBelow.GetMicroHeight());
		//				GameUtils.UpdateMicroheightAbove(this);
		//			}
		//		}
		//	}
		//}
		//public void DecreseLengthCheck(bool invalidateShift = false, bool manual = false)
		//{
		//	if (m_Layer == 0)
		//		return;

		//	bool shift = Input.GetKey(KeyCode.LeftShift);
		//	if (invalidateShift)
		//		shift = false;

		//	bool shrinkUp = m_PilarIndex == 0;
		//	if (shift)
		//		shrinkUp = !shrinkUp;

		//	var stackedBelow = GetStackedBelow();
		//	var stackedAbove = GetStackedAbove();

		//	if(shrinkUp && stackedBelow != null)
		//	{
		//		stackedBelow.SetHeight(stackedBelow.m_Height + 0.5f);
		//		if (manual)
		//			SetInfoToIE(stackedBelow as CBlockEdit);
		//		if(stackedBelow.IsStackLinkValid(0))
		//		//if (below.m_StackedBlocks[0] != null)
		//		{
		//			stackedBelow.DecreseLengthCheck(stackedBelow.m_PilarIndex == 0);
		//		}
		//	}
		//	else if(!shrinkUp)
		//	{
		//		SetHeight(m_Height - 0.5f);
		//		if (manual)
		//			SetInfoToIE((CBlockEdit)this);
		//		if (stackedAbove != null)
		//		{
		//			//if (above.m_StackedBlocks[1] == null)
		//			if(!stackedAbove.IsStackLinkValid(1))
		//			{
		//				stackedAbove.SetHeight(stackedAbove.m_Height - 0.5f);
		//				if (manual)
		//					SetInfoToIE(stackedAbove as CBlockEdit);
		//			}
		//			else
		//			{
		//				stackedAbove.DecreseLengthCheck(m_PilarIndex == 0);
		//			}
		//		}
		//	}
		//}
		//public void IncreaseHeightCheck(bool above = false, bool below = false, bool manual = false)
		//{
		//	if (m_Layer == 0)
		//		return;

		//	bool shift = Input.GetKey(KeyCode.LeftShift);

		//	var upBlock = GetStackedAbove(); //(CBlockEdit)m_StackedBlocks[1];
		//	var dnBlock = GetStackedBelow(); //(CBlockEdit)m_StackedBlocks[0];
		//	if (above)
		//		upBlock = null;
		//	if (below)
		//		dnBlock = null;

		//	if (upBlock != null)
		//	{
		//		upBlock.SetHeight(upBlock.m_Height + 0.5f);
		//		if (manual)
		//			SetInfoToIE(upBlock as CBlockEdit);
		//		upBlock.IncreaseHeightCheck(false, true);
		//	}
		//	else
		//	{
		//		upBlock = GetBlockAbove(m_Height + (m_BlockType == Def.BlockType.STAIRS ? 0.5f : 0f));
		//		if(!above && upBlock != null)
		//		{
		//			upBlock.m_StackedBlocksIdx[0] = m_PilarIndex;
		//			//upBlock.m_StackedBlocks[0] = this;
		//			m_StackedBlocksIdx[1] = upBlock.m_PilarIndex;
		//			//m_StackedBlocks[1] = upBlock;
		//			upBlock.SetMicroHeight(m_MicroHeight);
		//			GameUtils.UpdateMicroheightAbove(upBlock);
		//		}
		//	}
		//	if(dnBlock != null)
		//	{
		//		if(shift)
		//		{
		//			dnBlock.m_StackedBlocksIdx[1] = -1;
		//			//dnBlock.m_StackedBlocks[1] = null;
		//			m_StackedBlocksIdx[0] = -1;
		//			//m_StackedBlocks[0] = null;
		//		}
		//		else
		//		{
		//			dnBlock.SetHeight(dnBlock.m_Height + 0.5f);
		//			if (manual)
		//				SetInfoToIE(dnBlock as CBlockEdit);
		//			dnBlock.IncreaseHeightCheck(true);
		//		}
		//	}
		//	if (manual)
		//		SetInfoToIE((CBlockEdit)this);
		//}
		//public void DecreseHeightCheck(bool above = false, bool below = false, bool manual = false)
		//{
		//	if (m_Layer == 0)
		//		return;

		//	bool shift = Input.GetKey(KeyCode.LeftShift);

		//	var upBlock = GetStackedAbove(); //(CBlockEdit)m_StackedBlocks[1];
		//	var dnBlock = GetStackedBelow(); //(CBlockEdit)m_StackedBlocks[0];
		//	if (above)
		//		upBlock = null;
		//	if (below)
		//		dnBlock = null;

		//	if(upBlock != null)
		//	{
		//		if(shift)
		//		{
		//			upBlock.m_StackedBlocksIdx[0] = -1;
		//			//upBlock.m_StackedBlocks[0] = null;
		//			m_StackedBlocksIdx[1] = -1;
		//			//m_StackedBlocks[1] = null;
		//		}
		//		else
		//		{
		//			upBlock.SetHeight(upBlock.m_Height - 0.5f);
		//			if (manual)
		//				SetInfoToIE(upBlock as CBlockEdit);
		//			upBlock.DecreseHeightCheck(false, true);
		//		}
		//	}
		//	if(dnBlock != null)
		//	{
		//		dnBlock.SetHeight(dnBlock.m_Height - 0.5f);
		//		if (manual)
		//			SetInfoToIE(dnBlock as CBlockEdit);
		//		dnBlock.DecreseHeightCheck(true);
		//	}
		//	else
		//	{
		//		var len = m_Length == 3.4f ? 3.5f : m_Length;
		//		dnBlock = GetBlockBelow(m_Height - len);
		//		if(!below && dnBlock != null)
		//		{
		//			dnBlock.m_StackedBlocksIdx[1] = m_PilarIndex;
		//			//dnBlock.m_StackedBlocks[1] = this;
		//			m_StackedBlocksIdx[0] = dnBlock.m_PilarIndex;
		//			//m_StackedBlocks[0] = dnBlock;
		//			SetMicroHeight(dnBlock.GetMicroHeight());
		//			GameUtils.UpdateMicroheightAbove(this);
		//		}
		//	}
		//	if (manual)
		//		SetInfoToIE((CBlockEdit)this);
		//}
		public void _CheckStackedLinks()
		{
			float stairOffset = (m_BlockType == Def.BlockType.STAIRS ? 0.5f : 0f);
			var below = GetBlockBelow(m_Height - m_Length);
			var above = GetBlockAbove(m_Height + stairOffset);
			m_StackedBlocksIdx[1] = above != null ? above.m_PilarIndex : -1;
			m_StackedBlocksIdx[0] = below != null ? below.m_PilarIndex : -1;

			if (below != null)
				SetMicroHeight(below.GetMicroHeight());
		}
		public abstract void SetProp(AI.CProp prop);
		public AI.CProp GetProp() => m_Prop;
		public void SetMicroHeight(float microHeight)
		{
			if (m_MicroHeight == microHeight)
				return;
			m_MicroHeight = microHeight;
			SetHeight(m_Height);
		}
		public abstract void SetBlockFloat(float offset = 0f, float speed = 0f);
		public abstract Def.BlockState GetBlockState();
		//protected abstract void Enable();
		//protected abstract void Disable();
		protected abstract void Enable(bool enable);
		private void OnEnable()
		{
			Enable(true);
			//Enable();
		}
		private void OnDisable()
		{
			Enable(false);
			//Disable();
		}
		protected abstract void PilarNameChange(int prevNameLength);
		public void _OnPilarNameChange(int prevNameLength)
		{
			m_TopMR.gameObject.name = m_Pilar.gameObject.name + m_TopMR.gameObject.name.Substring(prevNameLength);
			m_MidMR.gameObject.name = m_Pilar.gameObject.name + m_MidMR.gameObject.name.Substring(prevNameLength);
			gameObject.name = m_Pilar.gameObject.name + gameObject.name.Substring(prevNameLength);
			PilarNameChange(prevNameLength);
		}
		protected abstract void LateInitBlock();
		public int GetPilarIndex() => m_PilarIndex;
		public void _SetPilarIndex(int idx) => m_PilarIndex = idx;
		public void InitBlock(CPilar pilar, Def.BlockType type, float height = 0f, float length = 0f)
		{
			m_Pilar = pilar;

			var top = new GameObject(gameObject.name + "_top")
			{
				layer = Def.RCLayerBlock
			};
			Blocks.SetBlock(top, type, Def.BlockMeshType.TOP);
			//BlockMeshDef.SetBlock(top, type, Def.BlockMeshType.TOP);
			m_TopMR = top.GetComponent<MeshRenderer>();
			m_TopMR.transform.SetParent(transform);

			var mid = new GameObject(gameObject.name + "_mid")
			{
				layer = Def.RCLayerBlock
			};
			Blocks.SetBlock(mid, type, Def.BlockMeshType.MID);
			//BlockMeshDef.SetBlock(mid, type, Def.BlockMeshType.MID);
			m_MidMR = mid.GetComponent<MeshRenderer>();
			m_MidMR.transform.SetParent(transform);

			transform.Rotate(Vector3.up, 90.0f, Space.World);

			m_BlockBC = gameObject.AddComponent<BoxCollider>();
			m_BlockBC.center = new Vector3(-0.5f, -0.5f, 0.5f);

			gameObject.layer = Def.RCLayerBlock;

			m_BlockType = type;
			m_MaterialFamily = BlockMaterial.VoidMat[0].Family;

			//m_TopOutline = top.AddComponent<Outline>();
			//m_TopOutline.enabled = false;
			//m_TopOutline.OutlineMode = Outline.Mode.OutlineAll;
			//m_MidOutline = mid.AddComponent<Outline>();
			//m_MidOutline.enabled = false;
			//m_MidOutline.OutlineMode = Outline.Mode.OutlineAll;

			m_Ants = new AntComponent[Def.DecoPositionCount];
			m_Deco = new SpriteBackendSprite[Def.DecoPositionCount];

			//m_StackedBlocks = new IBlock[2]{ null, null };
			m_StackedBlocksIdx = new int[2] { -1, -1 };
			m_WideStackedIdx = new WideStackLinks[3]
			{
				new WideStackLinks(){ PilarWID = -1, StackIdx = new int[2]{ -1, -1 } },
				new WideStackLinks(){ PilarWID = -1, StackIdx = new int[2]{ -1, -1 } },
				new WideStackLinks(){ PilarWID = -1, StackIdx = new int[2]{ -1, -1 } }
			};

			LateInitBlock();

			SetHeight(height);
			SetLength(length);
		}
	}
}
