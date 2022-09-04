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
	public class BiomeLayer
	{
		public Def.BiomeLayerType LayerType;

		public float MicroHeightMin;
		public float MicroHeightMax;
		public bool MicroHeightEnabled { get => MicroHeightMin != MicroHeightMax; }

		public List<IDChance> MaterialFamilies;

		public ushort SemiVoidChance; // 0 to 10'000
		public bool SemiVoidEnabled { get => SemiVoidChance > 0; }

		public ushort WideBlockChance; // 0 to 10'000
		public bool RandomWideBlockEnabled { get { return WideBlockChance > 0; } }

		public ushort StairBlockChance;
		public bool RandomStairBlockEnabled { get { return StairBlockChance > 0; } }
		public ushort RampBlockChance;

		public ushort PropGeneralChance; // 0 to 10'000
		public List<IDChance> PropFamilies;
		public float PropSafetyDistance;
		public bool RandomPropsEnabled { get => PropFamilies.Count > 0 && PropGeneralChance > 0; }

		public ushort MonsterGeneralChance; // 0 to 10'000
		public List<IDChance> MonsterFamilies;
		public bool RandomMonstersEnabled { get => MonsterFamilies.Count > 0 && MonsterGeneralChance > 0; }

		public BiomeLayer()
		{
			LayerType = Def.BiomeLayerType.COUNT;
			MicroHeightMin = MicroHeightMax = 0f;
			MaterialFamilies = new List<IDChance>();
			SemiVoidChance = 5000;
			WideBlockChance = 2500;
			StairBlockChance = 5000;
			RampBlockChance = 5000;
			PropGeneralChance = 2500;
			PropFamilies = new List<IDChance>();
			PropSafetyDistance = 1f;
			MonsterGeneralChance = 2500;
			MonsterFamilies = new List<IDChance>();
		}
		public BiomeLayer GetACopy()
		{
			return new BiomeLayer()
			{
				LayerType = LayerType,
				MicroHeightMin = MicroHeightMin,
				MicroHeightMax = MicroHeightMax,
				MaterialFamilies = new List<IDChance>(MaterialFamilies),
				SemiVoidChance = SemiVoidChance,
				WideBlockChance = WideBlockChance,
				StairBlockChance = StairBlockChance,
				RampBlockChance = RampBlockChance,
				PropGeneralChance = PropGeneralChance,
				PropFamilies = new List<IDChance>(PropFamilies),
				PropSafetyDistance = PropSafetyDistance,
				MonsterGeneralChance = MonsterGeneralChance,
				MonsterFamilies = new List<IDChance>(MonsterFamilies)
			};
		}
		public IE.V4.BiomeLayerIE ToIE()
		{
			var materialFamilies = new List<NameChance>(MaterialFamilies.Count);
			var propFamilies = new List<NameChance>(PropFamilies.Count);
			var monsterFamilies = new List<NameChance>(MonsterFamilies.Count);

			for (int i = 0; i < MaterialFamilies.Count; ++i)
			{
				var idc = MaterialFamilies[i];

				materialFamilies.Add(
					new NameChance()
					{
						ID = BlockMaterial.MaterialFamilies[idc.ID].FamilyInfo.FamilyName,
						Chance = idc.Chance
					});
			}
			for (int i = 0; i < PropFamilies.Count; ++i)
			{
				var idc = PropFamilies[i];

				propFamilies.Add(
					new NameChance()
					{
						ID = Props.PropFamilies[idc.ID].FamilyName,
						Chance = idc.Chance
					});
			}
			for (int i = 0; i < MonsterFamilies.Count; ++i)
			{
				var idc = MonsterFamilies[i];

				monsterFamilies.Add(
					new NameChance()
					{
						ID = Monsters.MonsterFamilies[idc.ID].Name,
						Chance = idc.Chance
					});
			}

			return new IE.V4.BiomeLayerIE()
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
		public bool IsValid()
		{
			if (LayerType == Def.BiomeLayerType.FULLVOID ||
				LayerType == Def.BiomeLayerType.OTHER ||
				LayerType == Def.BiomeLayerType.COUNT)
				return false;

			if (MaterialFamilies == null || MaterialFamilies.Count == 0)
				return false;

			return true;
		}
		public override bool Equals(object obj)
		{
			return obj is BiomeLayer info &&
				   this == info;
		}
		public override int GetHashCode()
		{
			int hashCode = -1629435916;
			hashCode = hashCode * -1521134295 + LayerType.GetHashCode();
			hashCode = hashCode * -1521134295 + MicroHeightMin.GetHashCode();
			hashCode = hashCode * -1521134295 + MicroHeightMax.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<IDChance>>.Default.GetHashCode(MaterialFamilies);
			hashCode = hashCode * -1521134295 + SemiVoidChance.GetHashCode();
			hashCode = hashCode * -1521134295 + WideBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + StairBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + RampBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + PropGeneralChance.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<IDChance>>.Default.GetHashCode(PropFamilies);
			hashCode = hashCode * -1521134295 + PropSafetyDistance.GetHashCode();
			hashCode = hashCode * -1521134295 + MonsterGeneralChance.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<IDChance>>.Default.GetHashCode(MonsterFamilies);
			return hashCode;
		}
		public static bool operator ==(BiomeLayer left, BiomeLayer right)
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
		public static bool operator !=(BiomeLayer left, BiomeLayer right)
		{
			return !(left == right);
		}
	}
}