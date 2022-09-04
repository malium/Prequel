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
	public class PilarInfo
	{
		List<BlockInfo> m_Blocks;
		StrucInfo m_Struc;
		int m_StructureID;
		int m_WorldID;
		bool m_HasBeenLoaded;

		public List<BlockInfo> GetBlocks() => m_Blocks;
		public StrucInfo GetStruc() => m_Struc;
		public int GetStructureID() => m_StructureID;
		public void _SetStructureID(int id) => m_StructureID = id;
		public int GetWorldID() => m_WorldID;
		public void _SetWorldID(int id) => m_WorldID = id;
		PilarInfo()
		{
			m_Blocks = null;
			m_Struc = null;
			m_StructureID = -1;
			m_WorldID = -1;
		}
		public PilarInfo(List<BlockInfo> blocks, StrucInfo struc, int strucID)
		{
			m_Blocks = blocks;
			m_Struc = struc;
			m_StructureID = strucID;
		}
		public void RemoveBlock(int idx)
		{
			if (idx < 0 || idx >= m_Blocks.Count)
				return;
			m_Blocks.RemoveAt(idx);
			for (int i = 0; i < m_Blocks.Count; ++i)
			{
				var block = m_Blocks[i];
				block.PilarIndex = i;
				if (block.StackedIdx[0] >= 0)
					block.StackedIdx[0] = i - 1;
				if (block.StackedIdx[1] >= 0)
					block.StackedIdx[1] = i + 1;
			}
			//for(int i = 0; i < m_Blocks.Count; ++i)
			//{
			//	if (i == idx)
			//		continue;
			//	var bi = m_Blocks[i];
			//	if (i > idx)
			//		--bi.PilarIndex;

			//	if(bi.StackedIdx[0] == idx)
			//	{
			//		bi.StackedIdx[0] = -1;
			//	}
			//	else if(bi.StackedIdx[0] > idx)
			//	{
			//		bi.StackedIdx[0] = bi.StackedIdx[0] - 1;
			//	}

			//	if (bi.StackedIdx[1] == idx)
			//	{
			//		bi.StackedIdx[1] = -1;
			//	}
			//	else if (bi.StackedIdx[1] > idx)
			//	{
			//		bi.StackedIdx[1] = bi.StackedIdx[1] - 1;
			//	}
			//}
			//m_Blocks.RemoveAt(idx);
		}
		public void RemoveBlock(BlockInfo bi)
		{
			RemoveBlock(m_Blocks.IndexOf(bi));
		}
		public bool HasBeenLoaded { get => m_HasBeenLoaded; set => m_HasBeenLoaded = value; }
	}
}
