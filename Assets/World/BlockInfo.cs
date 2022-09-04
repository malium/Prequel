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
	public class BlockInfo
	{
		int m_PropFamilyID;
		List<AI.CLivingEntity> m_LivingEntities;
		MaterialFamily m_MaterialFamily;
		MaterialSet m_MaterialSet;
		int m_Layer;
		Def.BiomeLayerType m_BiomeLayer;
		int m_IDXIE;
		int m_LinkedTo;
		float m_Length;
		float m_Height;
		float m_MicroHeight;
		PilarInfo m_Pilar;
		int m_PilarIndex;
		int[] m_StackedIdx;
		WideStackLinks[] m_WideStackedIdx;
		Def.BlockType m_BlockType;
		Def.RotationState m_Rotation;
		Def.StairState m_StairState;
		Def.StairType m_StairType;
		Def.BlockVoid m_VoidState;
		bool m_MicroheightApplied;

		public BlockInfo()
		{
			m_LivingEntities = new List<AI.CLivingEntity>();
			m_StackedIdx = new int[2];
			m_WideStackedIdx = new WideStackLinks[3];
			m_PropFamilyID = -1;
		}
		public BlockInfo GetACopy()
		{
			var b = new BlockInfo()
			{
				m_PropFamilyID = m_PropFamilyID,
				m_MaterialFamily = m_MaterialFamily,
				m_MaterialSet = m_MaterialSet,
				m_Layer = m_Layer,
				m_BiomeLayer = m_BiomeLayer,
				m_IDXIE = m_IDXIE,
				m_LinkedTo = m_LinkedTo,
				m_Length = m_Length,
				m_Height = m_Height,
				m_MicroHeight = m_MicroHeight,
				m_Pilar = m_Pilar,
				m_PilarIndex = m_PilarIndex,
				m_BlockType = m_BlockType,
				m_Rotation = m_Rotation,
				m_StairState = m_StairState,
				m_StairType = m_StairType,
				m_VoidState = m_VoidState,
				m_MicroheightApplied = m_MicroheightApplied
			};
			if (m_StackedIdx != null && b.m_StackedIdx != null)
				m_StackedIdx.CopyTo(b.m_StackedIdx, 0);
			if (m_WideStackedIdx != null && b.m_WideStackedIdx != null)
				m_WideStackedIdx.CopyTo(b.m_WideStackedIdx, 0);
			if (m_LivingEntities != null && b.m_LivingEntities != null)
				b.m_LivingEntities.AddRange(m_LivingEntities);
			return b;
		}
		public bool IsStackLinkValid(int position)
		{
			int stackIdx = position > 0 ? 1 : 0;
			var pilarIdx = m_StackedIdx[stackIdx];
			return pilarIdx >= 0 && pilarIdx != m_PilarIndex && m_Pilar.GetBlocks().Count > pilarIdx && m_Pilar.GetBlocks()[pilarIdx] != null;
		}
		protected BlockInfo GetStackedBlock(int pilarIdx)
		{
			if (pilarIdx < 0 || pilarIdx == m_PilarIndex || m_Pilar.GetBlocks().Count <= pilarIdx)
				return null;
			return m_Pilar.GetBlocks()[pilarIdx];
		}
		public BlockInfo GetStackedAbove()
		{
			return GetStackedBlock(m_StackedIdx[1]);
		}
		public BlockInfo GetStackedBelow()
		{
			return GetStackedBlock(m_StackedIdx[0]);
		}
		public BlockInfo GetBlockAbove(float wantedHeight)
		{
			BlockInfo block = null;
			for (int i = 0; i < m_Pilar.GetBlocks().Count; ++i)
			{
				var b = m_Pilar.GetBlocks()[i];
				var len = b.Length;
				var sLen = b.Length == 3.4f ? 3.5f : len;
				if ((b.Height - len) == wantedHeight || (b.Height - sLen) == wantedHeight)
				{
					block = b;
					break;
				}
			}
			return block;
		}
		public BlockInfo GetBlockBelow(float wantedHeight)
		{
			BlockInfo block = null;
			for (int i = 0; i < m_Pilar.GetBlocks().Count; ++i)
			{
				var b = m_Pilar.GetBlocks()[i];
				float h = b.Height + (b.BlockType == Def.BlockType.STAIRS ? 0.5f : 0f);
				if (h == wantedHeight)
				{
					block = b;
					break;
				}
			}
			return block;
		}
		void IncreaseLengthBottom()
		{
			m_Length += 0.5f;
		}
		void DecreaseLengthBottom()
		{
			float amount;
			if (m_Length != 3.4f)
				amount = 0.5f;
			else
				amount = 0.4f;
			m_Length -= amount;
		}
		void IncreaseLengthOther()
		{
			m_Length += 0.5f;
			IncreaseHeight(true, false);
		}
		void DecreaseLengthOther()
		{
			float amount;
			if (m_Length != 3.4f)
				amount = 0.5f;
			else
				amount = 0.4f;
			m_Height -= amount;
			DecreaseHeight(true, false);
		}
		// Increases the length by 0.5 and checks stacks
		public void IncreaseLength()
		{
			if ((m_BlockType != Def.BlockType.WIDE && (m_Length == 3.4f)) ||
				(m_BlockType == Def.BlockType.WIDE && (m_Length == 2.5f)))
				return; // Maximum Length

			bool bottomBlock = m_PilarIndex == 0;
			if (bottomBlock)
			{
				IncreaseLengthBottom();
			}
			else
			{
				IncreaseLengthOther();
			}
		}
		// Decreases the length by 0.5 and checks stacks
		public void DecreaseLength()
		{
			if (m_Length == 0.5f)
				return; // Minimum Length

			bool bottomBlock = m_PilarIndex == 0;
			if (bottomBlock)
			{
				DecreaseLengthBottom();
			}
			else
			{
				DecreaseLengthOther();
			}
		}
		// Increases the height by 0.5 and checks stacks
		public void IncreaseHeight(bool fromBelow = false, bool fromAbove = false)
		{
			m_Height += 0.5f;

			if (!fromAbove) // If this was not called by the block above
			{
				var above = GetStackedAbove();
				if (above != null) // Increase the above height recursively
				{
					above.IncreaseHeight(true, false);
				}
				else // Check if we hit a block above
				{
					var stairOffset = m_BlockType != Def.BlockType.STAIRS ? 0f : 0.5f;
					above = GetBlockAbove(m_Height + stairOffset);
					if (above != null) // create the stackLink
					{
						m_StackedIdx[1] = above.m_PilarIndex;
						above.m_StackedIdx[0] = m_PilarIndex;

						if (above.m_MicroHeight != m_MicroHeight) // Update the microheight from above
							above.ChangeMicroheight(m_MicroHeight);
					}
				}
			}

			if (!fromBelow) // If this was not called by the block below
			{
				var below = GetStackedBelow();
				if (below != null) // Increase the below height recursively
				{
					below.IncreaseHeight(false, true);
				}
			}
		}
		// Decreases the height by 0.5 and checks stacks
		public void DecreaseHeight(bool fromBelow = false, bool fromAbove = false)
		{
			m_Height -= 0.5f;

			if (!fromAbove) // If this was not called by the block above
			{
				var above = GetStackedAbove();
				if (above != null) // Decrease the above height recursively
				{
					above.DecreaseHeight(true, false);
				}
			}

			if (!fromBelow) // If this was not called by the block below
			{
				var below = GetStackedBelow();
				if (below != null) // Decrease the below height recursively
				{
					below.DecreaseHeight(false, true);
				}
				else // Check if we hit a block below
				{
					below = GetBlockBelow(m_Height - m_Length);
					if (below != null) // create the stackLink
					{
						m_StackedIdx[0] = below.m_PilarIndex;
						below.m_StackedIdx[1] = m_PilarIndex;
						if (below.m_MicroHeight != m_MicroHeight) // Update the microheight from below
							ChangeMicroheight(below.m_MicroHeight);
					}
				}
			}
		}
		// Changes the microheight for this block and their above ones
		public void ChangeMicroheight(float mHeight)
		{
			if (m_MicroHeight == mHeight)
				return;
			m_MicroHeight = mHeight;
			var above = GetStackedAbove();
			if (above != null)
				above.ChangeMicroheight(mHeight);
		}
		//public void IncreaseLengthCheck()
		//{
		//	bool growUp = m_PilarIndex != 0;

		//	var stackedAbove = GetStackedAbove();
		//	var stackedBelow = GetStackedBelow();

		//	if(stackedAbove != null)
		//	{
		//		if(growUp)
		//		{
		//			Height += 0.5f;
		//			IncreaseHeightCheck();
		//			//stackedAbove.IncreaseLengthCheck();
		//		}
		//		else
		//		{
		//			if(stackedBelow != null)
		//			{
		//				stackedBelow.Height -= 0.5f;
		//				if (stackedBelow.PilarIndex != 0)
		//					stackedBelow.DecreaseHeightCheck(true, false);
		//			}
		//			else
		//			{
		//				var len = m_Length == 3.4f ? 3.5f : m_Length;
		//				stackedBelow = GetBlockBelow(m_Height - len);
		//				if(stackedBelow != null)
		//				{
		//					m_StackedIdx[0] = stackedBelow.PilarIndex;
		//					stackedBelow.StackedIdx[1] = m_PilarIndex;
		//					m_MicroHeight = stackedBelow.m_MicroHeight;
		//					ApplyAbove((BlockInfo bi) => { bi.MicroHeight = m_MicroHeight; });
		//				}
		//			}
		//		}
		//	}
		//	else if(stackedBelow != null)
		//	{
		//		if(growUp)
		//		{
		//			m_Height += 0.5f;
		//			stackedAbove = GetBlockAbove(m_Height + (m_BlockType == Def.BlockType.STAIRS ? 0.5f : 0f));
		//			if(stackedAbove != null)
		//			{
		//				m_StackedIdx[1] = stackedAbove.m_PilarIndex;
		//				stackedAbove.StackedIdx[0] = m_PilarIndex;
		//				ApplyAbove((BlockInfo bi) => { bi.MicroHeight = m_MicroHeight; });
		//			}
		//		}
		//		else
		//		{
		//			stackedBelow.Height -= 0.5f;
		//			if (stackedBelow.m_PilarIndex != 0)
		//				stackedBelow.DecreaseHeightCheck(true, false);
		//		}
		//	}
		//	else
		//	{
		//		if(growUp)
		//		{
		//			m_Height += 0.5f;
		//			stackedAbove = GetBlockAbove(m_Height + (m_BlockType == Def.BlockType.STAIRS ? 0.5f : 0f));
		//			if(stackedAbove != null)
		//			{
		//				m_StackedIdx[1] = stackedAbove.m_PilarIndex;
		//				stackedAbove.m_StackedIdx[0] = m_PilarIndex;
		//				ApplyAbove((BlockInfo bi) => { bi.MicroHeight = m_MicroHeight; });
		//			}
		//		}
		//		else
		//		{
		//			var len = m_Length == 3.4f ? 3.5f : m_Length;
		//			stackedBelow = GetBlockBelow(m_Height - len);
		//			if(stackedBelow != null)
		//			{
		//				m_StackedIdx[0] = stackedBelow.m_PilarIndex;
		//				stackedBelow.StackedIdx[1] = m_PilarIndex;
		//				m_MicroHeight = stackedBelow.m_MicroHeight;
		//				ApplyAbove((BlockInfo bi) => { bi.MicroHeight = m_MicroHeight; });
		//			}
		//		}
		//	}
		//}
		//public void DecreseLengthCheck()
		//{
		//	bool shrinkUp = m_PilarIndex == 0;

		//	var stackedBelow = GetStackedBelow();
		//	var stackedAbove = GetStackedAbove();
		//	if (shrinkUp && stackedBelow != null)
		//	{
		//		stackedBelow.Height += 0.5f;
		//		if (stackedBelow.IsStackLinkValid(0))
		//		{
		//			stackedBelow.IncreaseHeightCheck(true, false);
		//		}
		//	}
		//	else if (!shrinkUp)
		//	{
		//		m_Height -= 0.5f;
		//		if (stackedAbove != null)
		//		{
		//			if (!stackedAbove.IsStackLinkValid(1))
		//			{
		//				stackedAbove.m_Height -= 0.5f;
		//				stackedAbove.DecreaseHeightCheck(true, false);
		//			}
		//			else
		//			{
		//				//stackedAbove.DecreseLengthCheck();
		//			}
		//		}
		//	}
		//}
		//public void IncreaseHeightCheck(bool above = false, bool below = false)
		//{
		//	var stackedAbove = GetStackedAbove();
		//	var stackedBelow = GetStackedBelow();

		//	if(!above && stackedAbove != null)
		//	{
		//		stackedAbove.Height += 0.5f;
		//		stackedAbove.IncreaseHeightCheck(false, true);
		//	}
		//	else if(!above)
		//	{
		//		stackedAbove = GetBlockAbove(m_Height + (m_BlockType == Def.BlockType.STAIRS ? 0.5f : 0f));
		//		if(stackedAbove != null)
		//		{
		//			stackedAbove.m_StackedIdx[0] = m_PilarIndex;
		//			m_StackedIdx[1] = stackedAbove.m_PilarIndex;
		//			stackedAbove.m_MicroHeight = m_MicroHeight;
		//			stackedAbove.ApplyAbove((BlockInfo bi) => bi.MicroHeight = m_MicroHeight);
		//		}
		//	}
		//	if(stackedBelow != null)
		//	{
		//		stackedBelow.Height += 0.5f;
		//		stackedBelow.IncreaseHeightCheck(true, false);
		//	}
		//}
		//public void DecreaseHeightCheck(bool above = false, bool below = false)
		//{
		//	var stackedAbove = GetStackedAbove();
		//	var stackedBelow = GetStackedBelow();

		//	if(!above && stackedAbove != null)
		//	{
		//		stackedAbove.Height -= 0.5f;
		//		stackedAbove.DecreaseHeightCheck(false, true);
		//	}
		//	if(!below && stackedBelow != null)
		//	{
		//		stackedBelow.Height -= 0.5f;
		//		stackedBelow.DecreaseHeightCheck(true, false);
		//	}
		//	else if(!below)
		//	{
		//		var len = m_Length == 3.4f ? 3.5f : m_Length;
		//		stackedBelow = GetBlockBelow(m_Height - len);
		//		if(stackedBelow != null)
		//		{
		//			stackedBelow.m_StackedIdx[1] = m_PilarIndex;
		//			m_StackedIdx[0] = stackedBelow.m_PilarIndex;
		//			m_MicroHeight = stackedBelow.m_MicroHeight;
		//			ApplyAbove((BlockInfo bi) => bi.m_MicroHeight = m_MicroHeight);
		//		}
		//	}
		//}
		public void ApplyAbove(Action<BlockInfo> fn)
		{
			var above = GetStackedAbove();
			if (above == null)
				return;
		
			fn(above);
			above.ApplyAbove(fn);
		}
		public void ApplyBelow(Action<BlockInfo> fn)
		{
			var below = GetStackedBelow();
			if (below == null)
				return;

			fn(below);
			below.ApplyBelow(fn);
		}
		public int PropFamilyID { get => m_PropFamilyID; set => m_PropFamilyID = value; }
		public List<AI.CLivingEntity> LivingEntities { get => m_LivingEntities; set => m_LivingEntities = value; }
		public MaterialFamily MatFamily { get => m_MaterialFamily; set => m_MaterialFamily = value; }
		public MaterialSet MatSet { get => m_MaterialSet; set => m_MaterialSet = value; }
		public int Layer { get => m_Layer; set => m_Layer = value; }
		public Def.BiomeLayerType BiomeLayer { get => m_BiomeLayer; set => m_BiomeLayer = value; }
		public int IDXIE { get => m_IDXIE; set => m_IDXIE = value; }
		public int LinkedTo { get => m_LinkedTo; set => m_LinkedTo = value; }
		public float Length { get => m_Length; set => m_Length = value; }
		public float Height { get => m_Height; set => m_Height = value; }
		public float MicroHeight { get => m_MicroHeight; set => m_MicroHeight = value; }
		public PilarInfo Pilar { get => m_Pilar; set => m_Pilar = value; }
		public int PilarIndex { get => m_PilarIndex; set => m_PilarIndex = value; }
		public int[] StackedIdx { get => m_StackedIdx; set => m_StackedIdx = value; }
		public Def.BlockType BlockType { get => m_BlockType; set => m_BlockType = value; }
		public Def.RotationState Rotation { get => m_Rotation; set => m_Rotation = value; }
		public Def.StairState StairState { get => m_StairState; set => m_StairState = value; }
		public Def.StairType StairType { get => m_StairType; set => m_StairType = value; }
		public Def.BlockVoid VoidState { get => m_VoidState; set => m_VoidState = value; }
		public WideStackLinks[] WideStackedIdx { get => m_WideStackedIdx; set => m_WideStackedIdx = value; }
		public bool MicroheightApplied { get => m_MicroheightApplied; set => m_MicroheightApplied = value; }
	}
}
