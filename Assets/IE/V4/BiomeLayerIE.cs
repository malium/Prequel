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

namespace Assets.IE.V4
{
	[System.Serializable]
	public class BiomeLayerIE
	{
		public Def.BiomeLayerType LayerType;
		public float MicroHeightMin;
		public float MicroHeightMax;
		public List<NameChance> MaterialFamilies;
		public ushort SemiVoidChance;
		public ushort WideBlockChance;
		public ushort StairBlockChance;
		public ushort RampBlockChance;
		public ushort PropGeneralChance;
		public List<NameChance> PropFamilies;
		public float PropSafetyDistance;
		public ushort MonsterGeneralChance;
		public List<NameChance> MonsterFamilies;

		public BiomeLayerIE()
		{
			LayerType = Def.BiomeLayerType.COUNT;
			MicroHeightMin = MicroHeightMax = 0f;
			MaterialFamilies = new List<NameChance>();
			SemiVoidChance = 5000;
			WideBlockChance = 2500;
			StairBlockChance = 5000;
			RampBlockChance = 5000;
			PropGeneralChance = 2500;
			PropFamilies = new List<NameChance>();
			PropSafetyDistance = 1f;
			MonsterGeneralChance = 2500;
			MonsterFamilies = new List<NameChance>();
		}
		public BiomeLayerIE GetACopy()
		{
			return new BiomeLayerIE()
			{
				LayerType = LayerType,
				MicroHeightMin = MicroHeightMin,
				MicroHeightMax = MicroHeightMax,
				MaterialFamilies = new List<NameChance>(MaterialFamilies),
				SemiVoidChance = SemiVoidChance,
				WideBlockChance = WideBlockChance,
				StairBlockChance = StairBlockChance,
				RampBlockChance = RampBlockChance,
				PropGeneralChance = PropGeneralChance,
				PropFamilies = new List<NameChance>(PropFamilies),
				PropSafetyDistance = PropSafetyDistance,
				MonsterGeneralChance = MonsterGeneralChance,
				MonsterFamilies = new List<NameChance>(MonsterFamilies)
			};
		}
		public World.BiomeLayer ToBiomeLayer()
		{
			var materialFamilies = new List<IDChance>(MaterialFamilies.Count);
			var propFamilies = new List<IDChance>(PropFamilies.Count);
			var monsterFamilies = new List<IDChance>(MonsterFamilies.Count);

			for (int i = 0; i < MaterialFamilies.Count; ++i)
			{
				materialFamilies.Add(
					new IDChance()
					{
						ID = BlockMaterial.FamilyDict[MaterialFamilies[i].ID],
						Chance = MaterialFamilies[i].Chance
					});
			}
			for (int i = 0; i < PropFamilies.Count; ++i)
			{
				propFamilies.Add(
					new IDChance()
					{
						ID = Props.FamilyDict[PropFamilies[i].ID],
						Chance = PropFamilies[i].Chance
					});
			}
			for (int i = 0; i < MonsterFamilies.Count; ++i)
			{
				monsterFamilies.Add(
					new IDChance()
					{
						ID = Monsters.FamilyDict[MonsterFamilies[i].ID],
						Chance = MonsterFamilies[i].Chance
					});
			}
			return new World.BiomeLayer()
			{
				LayerType = LayerType,
				MicroHeightMin = MicroHeightMin,
				MicroHeightMax = MicroHeightMax,
				MaterialFamilies = materialFamilies,
				SemiVoidChance = SemiVoidChance,
				WideBlockChance = WideBlockChance,
				StairBlockChance = StairBlockChance,
				RampBlockChance = RampBlockChance,
				PropGeneralChance = PropGeneralChance,
				PropFamilies = propFamilies,
				PropSafetyDistance = PropSafetyDistance,
				MonsterGeneralChance = MonsterGeneralChance,
				MonsterFamilies = monsterFamilies
			};
		}
		public override bool Equals(object obj)
		{
			return obj is BiomeLayerIE info &&
				   this == info;
		}
		public override int GetHashCode()
		{
			int hashCode = -1629435916;
			hashCode = hashCode * -1521134295 + LayerType.GetHashCode();
			hashCode = hashCode * -1521134295 + MicroHeightMin.GetHashCode();
			hashCode = hashCode * -1521134295 + MicroHeightMax.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<NameChance>>.Default.GetHashCode(MaterialFamilies);
			hashCode = hashCode * -1521134295 + SemiVoidChance.GetHashCode();
			hashCode = hashCode * -1521134295 + WideBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + StairBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + RampBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + PropGeneralChance.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<NameChance>>.Default.GetHashCode(PropFamilies);
			hashCode = hashCode * -1521134295 + PropSafetyDistance.GetHashCode();
			hashCode = hashCode * -1521134295 + MonsterGeneralChance.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<NameChance>>.Default.GetHashCode(MonsterFamilies);
			return hashCode;
		}
		public static bool operator ==(BiomeLayerIE left, BiomeLayerIE right)
		{
			if (left is null != right is null)
				return false;
			if (left is null && right is null)
				return true;

			if (left.LayerType != right.LayerType)
				return false;

			if (left.MicroHeightMin != right.MicroHeightMin)
				return false;
			if (left.MicroHeightMax != right.MicroHeightMax)
				return false;

			if (left.MaterialFamilies.Count != right.MaterialFamilies.Count)
				return false;
			for (int i = 0; i < left.MaterialFamilies.Count; ++i)
			{
				if (left.MaterialFamilies[i].ID != right.MaterialFamilies[i].ID
					|| left.MaterialFamilies[i].Chance != right.MaterialFamilies[i].Chance)
					return false;
			}

			if (left.PropGeneralChance != right.PropGeneralChance)
				return false;
			if (left.PropSafetyDistance != right.PropSafetyDistance)
				return false;
			if (left.PropFamilies.Count != right.PropFamilies.Count)
				return false;
			for (int i = 0; i < left.PropFamilies.Count; ++i)
			{
				if (left.PropFamilies[i].ID != right.PropFamilies[i].ID
					|| left.PropFamilies[i].Chance != right.PropFamilies[i].Chance)
					return false;
			}

			if (left.MonsterGeneralChance != right.MonsterGeneralChance)
				return false;
			if (left.MonsterFamilies.Count != right.MonsterFamilies.Count)
				return false;
			for (int i = 0; i < left.MonsterFamilies.Count; ++i)
			{
				if (left.MonsterFamilies[i].ID != right.MonsterFamilies[i].ID
					|| left.MonsterFamilies[i].Chance != right.MonsterFamilies[i].Chance)
					return false;
			}

			if (left.WideBlockChance != right.WideBlockChance)
				return false;

			if (left.StairBlockChance != right.StairBlockChance)
				return false;

			if (left.RampBlockChance != right.RampBlockChance)
				return false;

			if (left.SemiVoidChance != right.SemiVoidChance)
				return false;

			return true;
		}
		public static bool operator !=(BiomeLayerIE left, BiomeLayerIE right)
		{
			return !(left == right);
		}
	}
}
