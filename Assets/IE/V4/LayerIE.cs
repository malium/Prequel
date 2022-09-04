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
	public class LayerIE
	{
		public byte Slot;

		public Def.BiomeLayerType LayerType;

		public List<IDChance> LinkedLayers;

		public Def.RotationState Rotation;

		public List<NameChance> MaterialFamilies;

		public float MicroHeightMin;
		public float MicroHeightMax;

		public float BlockLengthMin;
		public float BlockLengthMax;

		public ushort PropChance;
		public List<NameChance> PropFamilies;
		public float PropSafetyDistance;

		public ushort MonsterChance;
		public List<NameChance> MonsterFamilies;
		public bool LayerMonstersRespawn;
		public bool SpawnZoneMonsters;
		
		public ushort WideBlockChance;
		public ushort StairBlockChance;
		public ushort RampBlockChance;
		public ushort SemiVoidChance;

		public LayerIE()
		{
			Slot = 0;
			LinkedLayers = new List<IDChance>();
			LayerType = Def.BiomeLayerType.OTHER;
			Rotation = Def.RotationState.COUNT;
			MaterialFamilies = new List<NameChance>();
			BlockLengthMin = 0f;
			BlockLengthMax = 0f;
			PropChance = 0;
			PropFamilies = new List<NameChance>();
			PropSafetyDistance = 0f;
			MonsterChance = 0;
			MonsterFamilies = new List<NameChance>();
			LayerMonstersRespawn = false;
			SpawnZoneMonsters = true;
			WideBlockChance = 0;
			StairBlockChance = 0;
			SemiVoidChance = 0;
			RampBlockChance = 0;
		}
		public LayerIE(LayerIE copy)
		{
			Slot = copy.Slot;
			LinkedLayers = new List<IDChance>(copy.LinkedLayers);
			LayerType = copy.LayerType;
			Rotation = copy.Rotation;
			MaterialFamilies = new List<NameChance>(copy.MaterialFamilies);
			BlockLengthMin = copy.BlockLengthMin;
			BlockLengthMax = copy.BlockLengthMax;
			PropChance = copy.PropChance;
			PropFamilies = new List<NameChance>(copy.PropFamilies);
			PropSafetyDistance = copy.PropSafetyDistance;
			MonsterChance = copy.MonsterChance;
			MonsterFamilies = new List<NameChance>(copy.MonsterFamilies);
			LayerMonstersRespawn = copy.LayerMonstersRespawn;
			SpawnZoneMonsters = copy.SpawnZoneMonsters;
			WideBlockChance = copy.WideBlockChance;
			StairBlockChance = copy.StairBlockChance;
			SemiVoidChance = copy.SemiVoidChance;
			MicroHeightMin = copy.MicroHeightMin;
			MicroHeightMax = copy.MicroHeightMax;
			RampBlockChance = copy.RampBlockChance;
		}
		public LayerInfo ToLayerInfo()
		{
			var li = new LayerInfo()
			{
				Slot = Slot,
				LayerType = LayerType,
				MaterialFamilies = new List<IDChance>(MaterialFamilies.Count),
				Rotation = Rotation,
				MicroHeightMin = MicroHeightMin,
				MicroHeightMax = MicroHeightMax,
				BlockLengthMin = BlockLengthMin,
				BlockLengthMax = BlockLengthMax,
				PropGeneralChance = PropChance,
				PropFamilies = new List<IDChance>(PropFamilies.Count),
				PropSafetyDistance = PropSafetyDistance,
				SpawnZoneMonsters = SpawnZoneMonsters,
				LayerMonstersRespawn = LayerMonstersRespawn,
				MonsterGeneralChance = MonsterChance,
				MonsterFamilies = new List<IDChance>(MonsterFamilies.Count),
				LinkedLayers = new List<IDChance>(LinkedLayers),
				SemiVoidChance = SemiVoidChance,
				WideBlockChance = WideBlockChance,
				StairBlockChance = StairBlockChance,
				RampBlockChance = RampBlockChance
			};
			for(int i = 0; i < MaterialFamilies.Count; ++i)
			{
				li.MaterialFamilies.Add(
					new IDChance()
					{
						ID = BlockMaterial.FamilyDict[MaterialFamilies[i].ID],
						Chance = MaterialFamilies[i].Chance
					});
			}
			for (int i = 0; i < PropFamilies.Count; ++i)
			{
				li.PropFamilies.Add(
					new IDChance()
					{
						ID = Props.FamilyDict[PropFamilies[i].ID],
						Chance = PropFamilies[i].Chance
					});
			}
			for (int i = 0; i < MonsterFamilies.Count; ++i)
			{
				li.MonsterFamilies.Add(
					new IDChance()
					{
						ID = Monsters.FamilyDict[MonsterFamilies[i].ID],
						Chance = MonsterFamilies[i].Chance
					});
			}

			return li;
		}
		public override bool Equals(object obj)
		{
			return obj is LayerIE iE &&
				   this == iE;
		}
		public override int GetHashCode()
		{
			int hashCode = 394788369;
			hashCode = hashCode * -1521134295 + Slot.GetHashCode();
			hashCode = hashCode * -1521134295 + LayerType.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<IDChance>>.Default.GetHashCode(LinkedLayers);
			hashCode = hashCode * -1521134295 + Rotation.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<NameChance>>.Default.GetHashCode(MaterialFamilies);
			hashCode = hashCode * -1521134295 + MicroHeightMin.GetHashCode();
			hashCode = hashCode * -1521134295 + MicroHeightMax.GetHashCode();
			hashCode = hashCode * -1521134295 + BlockLengthMin.GetHashCode();
			hashCode = hashCode * -1521134295 + BlockLengthMax.GetHashCode();
			hashCode = hashCode * -1521134295 + PropChance.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<NameChance>>.Default.GetHashCode(PropFamilies);
			hashCode = hashCode * -1521134295 + PropSafetyDistance.GetHashCode();
			hashCode = hashCode * -1521134295 + MonsterChance.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<NameChance>>.Default.GetHashCode(MonsterFamilies);
			hashCode = hashCode * -1521134295 + LayerMonstersRespawn.GetHashCode();
			hashCode = hashCode * -1521134295 + SpawnZoneMonsters.GetHashCode();
			hashCode = hashCode * -1521134295 + WideBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + StairBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + RampBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + SemiVoidChance.GetHashCode();
			return hashCode;
		}
		public static bool operator ==(LayerIE left, LayerIE right)
		{
			if (left is null && !(right is null))
				return false;
			if (!(left is null) && right is null)
				return false;
			if (left is null && right is null)
				return true;

			if (left.Slot != right.Slot)
				return false;

			if (left.LayerType != right.LayerType)
				return false;

			if (left.LinkedLayers.Count != right.LinkedLayers.Count)
				return false;

			for (int i = 0; i < left.LinkedLayers.Count; ++i)
			{
				if (left.LinkedLayers[i].ID != right.LinkedLayers[i].ID
					|| left.LinkedLayers[i].Chance != right.LinkedLayers[i].Chance)
					return false;
			}

			if (left.LinkedLayers.Count > 0)
				return true;

			if (left.MaterialFamilies.Count != right.MaterialFamilies.Count)
				return false;

			for(int i = 0; i < left.MaterialFamilies.Count; ++i)
			{
				if (left.MaterialFamilies[i].ID != right.MaterialFamilies[i].ID
					|| left.MaterialFamilies[i].Chance != right.MaterialFamilies[i].Chance)
					return false;
			}

			if (left.Rotation != right.Rotation)
				return false;

			if (left.MicroHeightMin != right.MicroHeightMin)
				return false;

			if (left.MicroHeightMax != right.MicroHeightMax)
				return false;
			//if (left.MicroHeightOffset != right.MicroHeightOffset)
			//    return false;

			if (left.BlockLengthMin != right.BlockLengthMin)
				return false;

			if (left.BlockLengthMax != right.BlockLengthMax)
				return false;

			if (left.PropChance != right.PropChance)
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

			if (left.MonsterChance != right.MonsterChance)
				return false;

			if (left.SpawnZoneMonsters != right.SpawnZoneMonsters)
				return false;

			if (left.LayerMonstersRespawn != right.LayerMonstersRespawn)
				return false;

			if (left.MonsterFamilies.Count != right.MonsterFamilies.Count)
				return false;

			for (int i = 0; i < left.MonsterFamilies.Count; ++i)
			{
				if (left.MonsterFamilies[i].ID != right.MonsterFamilies[i].ID
					|| left.MonsterFamilies[i].Chance != right.MonsterFamilies[i].Chance)
					return false;
			}

			//if (left.AppliedEffects.Count != right.AppliedEffects.Count)
			//    return false;

			//for (int i = 0; i < left.AppliedEffects.Count; ++i)
			//{
			//    if (left.AppliedEffects[i] != right.AppliedEffects[i])
			//        return false;
			//}

			//if (left.BlockFloatOffset != right.BlockFloatOffset)
			//    return false;

			//if (left.BlockFloatSpeed != right.BlockFloatSpeed)
			//    return false;

			if (left.WideBlockChance != right.WideBlockChance)
				return false;

			if (left.StairBlockChance != right.StairBlockChance)
				return false;

			if (left.RampBlockChance != right.RampBlockChance)
				return false;

			return true;
		}
		public static bool operator !=(LayerIE left, LayerIE right)
		{
			return !(left == right);
		}
	}
}
