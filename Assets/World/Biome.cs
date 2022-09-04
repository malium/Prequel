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
	public class Biome
	{
		BiomeLayer[] m_Layers;
		int[] m_BiomeStat;
		int m_MinDistance;
		int m_IDXIE;

		public Biome()
		{
			m_IDXIE = -1;
			m_Layers = new BiomeLayer[Def.BiomeTypeCount];
			for(int i = 0; i < m_Layers.Length; ++i)
			{
				var layer = new BiomeLayer
				{
					LayerType = (Def.BiomeLayerType)i
				};
				m_Layers[i] = layer;
			}
			m_BiomeStat = new int[Def.BiomeStatCount * 2];
			m_MinDistance = 1;
			for (int i = 0; i < m_BiomeStat.Length; ++i) m_BiomeStat[i] = 0;
		}
		public void GetBiomeStat(Def.BiomeStat stat, out int min, out int max)
		{
			int index = ((int)stat) * 2;
			min = m_BiomeStat[index];
			max = m_BiomeStat[index + 1];
		}
		public void SetBiomeStat(Def.BiomeStat stat, int min, int max)
		{
			int index = ((int)stat) * 2;
			m_BiomeStat[index] = min;
			m_BiomeStat[index + 1] = max;
		}
		public int GetMinDistance() => m_MinDistance;
		public void SetMinDistance(int distance) => m_MinDistance = distance;
		public int[] _GetStatValue() => m_BiomeStat;
		public int IDXIE { get => m_IDXIE; set => m_IDXIE = value; }
		public BiomeLayer[] GetLayers() => m_Layers;
		public bool IsCompatible(List<Def.BiomeLayerType> needed)
		{
			for(int i = 0; i < needed.Count; ++i)
			{
				if (needed[i] == Def.BiomeLayerType.OTHER)
					continue;

				if(!m_Layers[(int)needed[i]].IsValid())
				{
					return false;
				}
			}
			return true;
		}
	}
}
