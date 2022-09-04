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
	[System.Serializable]
	public struct IDChance
	{
		public int ID;
		public ushort Chance; // 0 to 10'000
	}

	[System.Serializable]
	public struct NameChance
	{
		public string ID;
		public ushort Chance;
	}

	[System.Serializable]
	public class LayerInfo
	{
		public int Slot;

		public List<IDChance> MaterialFamilies;

		public Def.BiomeLayerType LayerType;

		public Def.RotationState Rotation;

		public float MicroHeightMin;
		public float MicroHeightMax;
		public bool MicroHeightEnabled { get => MicroHeightMin != MicroHeightMax; } 

		public float BlockLengthMin;
		public float BlockLengthMax;
		public bool RandomBlockLengthEnabled { get => BlockLengthMin != BlockLengthMax; }

		public ushort PropGeneralChance; // 0 to 10'000
		public List<IDChance> PropFamilies;
		public float PropSafetyDistance;
		public bool RandomPropsEnabled { get => PropFamilies.Count > 0 && PropGeneralChance > 0; }

		public bool SpawnZoneMonsters;
		public bool LayerMonstersRespawn;
		public ushort MonsterGeneralChance; // 0 to 10'000
		public List<IDChance> MonsterFamilies;
		public bool RandomMonstersEnabled { get => MonsterFamilies.Count > 0 && MonsterGeneralChance > 0; }

		//public List<Def.BlockEffects> AppliedEffects;
		//public bool HasEffects { get { return AppliedEffects.Count > 0; } }

		//public float BlockFloatOffset;
		//public float BlockFloatSpeed;
		//public bool BlockFloatEnabled { get { return (BlockFloatOffset > 0f || BlockFloatOffset < 0f) && (BlockFloatSpeed > 0f || BlockFloatSpeed < 0f); } }

		public ushort WideBlockChance; // 0 to 10'000
		public bool RandomWideBlockEnabled { get { return WideBlockChance > 0; } }
		public ushort StairBlockChance;
		public ushort RampBlockChance;
		public bool RandomStairBlockEnabled { get { return StairBlockChance > 0; } }

		public ushort SemiVoidChance; // 0 to 10'000
		public bool SemiVoidEnabled { get => SemiVoidChance > 0; }

		public List<IDChance> LinkedLayers;
		public bool IsLinkedLayer { get { return LinkedLayers.Count > 0; } }

		public void CopyFrom(LayerInfo other)
		{
			MaterialFamilies = new List<IDChance>(other.MaterialFamilies);
			LayerType = other.LayerType;
			Rotation = other.Rotation;
			MicroHeightMin = other.MicroHeightMin;
			MicroHeightMax = other.MicroHeightMax;
			BlockLengthMin = other.BlockLengthMin;
			BlockLengthMax = other.BlockLengthMax;
			PropGeneralChance = other.PropGeneralChance;
			PropFamilies = new List<IDChance>(other.PropFamilies);
			PropSafetyDistance = other.PropSafetyDistance;
			SpawnZoneMonsters = other.SpawnZoneMonsters;
			LayerMonstersRespawn = other.LayerMonstersRespawn;
			MonsterGeneralChance = other.MonsterGeneralChance;
			MonsterFamilies = new List<IDChance>(other.MonsterFamilies);
			WideBlockChance = other.WideBlockChance;
			StairBlockChance = other.StairBlockChance;
			RampBlockChance = other.RampBlockChance;
			SemiVoidChance = other.SemiVoidChance;
			LinkedLayers = new List<IDChance>(other.LinkedLayers);
		}

		public bool IsValid()
		{
			if (Slot == 0 || Slot > Def.MaxLayerSlots)
				return false;

			if (IsLinkedLayer)
			{
				return LinkedLayers.Count >= 2;
			}
			if (LayerType == Def.BiomeLayerType.OTHER && MaterialFamilies.Count == 0)
				return false;
			//if (MaterialFamilies.Count == 0)
			//	return false;
			return true;
		}

		static readonly LayerInfo DefaultLayer = new LayerInfo()
		{
			Slot = 0,
			LayerType = Def.BiomeLayerType.OTHER,
			MaterialFamilies = new List<IDChance>(),
			Rotation = Def.RotationState.COUNT,
			MicroHeightMin = 0f,
			MicroHeightMax = 0f,
			BlockLengthMin = 0.5f,
			BlockLengthMax = 0.5f,
			PropGeneralChance = 0,
			PropFamilies = new List<IDChance>(),
			PropSafetyDistance = 1f,
			SpawnZoneMonsters = true,
			LayerMonstersRespawn = false,
			MonsterGeneralChance = 0,
			MonsterFamilies = new List<IDChance>(),
			//AppliedEffects = new List<Def.BlockEffects>(),
			//BlockFloatOffset = 0f,
			//BlockFloatSpeed = 0f,
			WideBlockChance = 2500,
			StairBlockChance = 5000,
			RampBlockChance = 5000,
			SemiVoidChance = 5000,
			LinkedLayers = new List<IDChance>(),
		};

		public static LayerInfo GetDefaultLayer()
		{
			var li = new LayerInfo();
			li.CopyFrom(DefaultLayer);
			return li;
		}

		public void SortProbabilities()
		{
			List<IDChance> Sort(List<IDChance> list)
			{
				var nList = new List<IDChance>(list.Count);

				while(list.Count > 0)
				{
					int greatestIdx = -1;
					ushort greatestChance = 0;
					for(int i = 0; i < list.Count; ++i)
					{
						if(list[i].Chance >= greatestChance)
						{
							greatestIdx = i;
							greatestChance = list[i].Chance;
						}
					}
					nList.Add(new IDChance()
					{
						ID = list[greatestIdx].ID,
						Chance = list[greatestIdx].Chance
					});
					list.RemoveAt(greatestIdx);
				}

				return nList;
			}
			MaterialFamilies = Sort(MaterialFamilies);
			PropFamilies = Sort(PropFamilies);
			MonsterFamilies = Sort(MonsterFamilies);
			LinkedLayers = Sort(LinkedLayers);
		}
		public static int RandomFromList(List<IDChance> items)
		{
			var rng = UnityEngine.Random.value;
			ushort chance = (ushort)Mathf.FloorToInt(rng * 10000f);
			ushort accum = 0;
			for(int i = 0; i < items.Count; ++i)
			{
				ushort nextChance = (ushort)(accum + (int)items[i].Chance);
				if(chance >= accum && chance <= nextChance)
				{
					return i;
				}
				accum = nextChance;
			}
			Debug.LogWarning($"Couldnt find a suitable random item, chance'{chance}', count'{items.Count}' !RandomFromList");
			return Mathf.FloorToInt(rng * (items.Count - 1));
		}
		public static int RandomFromList(List<IDChance> items, System.Random rng)
		{
			var value = (float)rng.NextDouble();
			ushort chance = (ushort)Mathf.FloorToInt(value * 10000f);
			ushort accum = 0;
			for (int i = 0; i < items.Count; ++i)
			{
				ushort nextChance = (ushort)(accum + (int)items[i].Chance);
				if (chance >= accum && chance <= nextChance)
				{
					return i;
				}
				accum = nextChance;
			}
			Debug.LogWarning($"Couldnt find a suitable random item, chance'{chance}', count'{items.Count}' !RandomFromList");
			return Mathf.FloorToInt(value * (items.Count - 1));
		}
		public void ToLayerIE(ref IE.V4.LayerIE ie)
		{
			ie.Slot = (byte)Slot;

			ie.LinkedLayers = new List<IDChance>(LinkedLayers);

			ie.Rotation = Rotation;

			ie.LayerType = LayerType;

			ie.MaterialFamilies = new List<NameChance>(MaterialFamilies.Count);
			for(int i = 0; i < MaterialFamilies.Count; ++i)
			{
				ie.MaterialFamilies.Add(new NameChance()
				{
					ID = BlockMaterial.MaterialFamilies[MaterialFamilies[i].ID].FamilyInfo.FamilyName,
					Chance = MaterialFamilies[i].Chance
				}) ;
			}

			ie.MicroHeightMin = MicroHeightMin;
			ie.MicroHeightMax = MicroHeightMax;

			ie.BlockLengthMin = BlockLengthMin;
			ie.BlockLengthMax = BlockLengthMax;

			ie.PropChance = PropGeneralChance;

			ie.PropFamilies = new List<NameChance>(PropFamilies.Count);
			for (int i = 0; i < PropFamilies.Count; ++i)
			{
				ie.PropFamilies.Add(new NameChance()
				{
					ID = Props.PropFamilies[PropFamilies[i].ID].FamilyName,
					Chance = PropFamilies[i].Chance
				});
			}

			ie.PropSafetyDistance = PropSafetyDistance;

			ie.MonsterChance = MonsterGeneralChance;

			ie.MonsterFamilies = new List<NameChance>(MonsterFamilies.Count);
			for (int i = 0; i < MonsterFamilies.Count; ++i)
			{
				ie.MonsterFamilies.Add(new NameChance()
				{
					ID = Monsters.MonsterFamilies[MonsterFamilies[i].ID].Name,
					Chance = MonsterFamilies[i].Chance
				});
			}

			ie.LayerMonstersRespawn = LayerMonstersRespawn;
			ie.SpawnZoneMonsters = SpawnZoneMonsters;

			ie.WideBlockChance = WideBlockChance;
			ie.StairBlockChance = StairBlockChance;
			ie.SemiVoidChance = SemiVoidChance;
			ie.RampBlockChance = RampBlockChance;
		}

		public override bool Equals(object obj)
		{
			return obj is LayerInfo info &&
				   this == info;
		}

		public override int GetHashCode()
		{
			int hashCode = -287130571;
			hashCode = hashCode * -1521134295 + Slot.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<IDChance>>.Default.GetHashCode(MaterialFamilies);
			hashCode = hashCode * -1521134295 + LayerType.GetHashCode();
			hashCode = hashCode * -1521134295 + Rotation.GetHashCode();
			hashCode = hashCode * -1521134295 + MicroHeightMin.GetHashCode();
			hashCode = hashCode * -1521134295 + MicroHeightMax.GetHashCode();
			hashCode = hashCode * -1521134295 + BlockLengthMin.GetHashCode();
			hashCode = hashCode * -1521134295 + BlockLengthMax.GetHashCode();
			hashCode = hashCode * -1521134295 + PropGeneralChance.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<IDChance>>.Default.GetHashCode(PropFamilies);
			hashCode = hashCode * -1521134295 + PropSafetyDistance.GetHashCode();
			hashCode = hashCode * -1521134295 + SpawnZoneMonsters.GetHashCode();
			hashCode = hashCode * -1521134295 + LayerMonstersRespawn.GetHashCode();
			hashCode = hashCode * -1521134295 + MonsterGeneralChance.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<IDChance>>.Default.GetHashCode(MonsterFamilies);
			hashCode = hashCode * -1521134295 + WideBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + StairBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + RampBlockChance.GetHashCode();
			hashCode = hashCode * -1521134295 + SemiVoidChance.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<IDChance>>.Default.GetHashCode(LinkedLayers);
			return hashCode;
		}

		public static bool operator ==(LayerInfo left, LayerInfo right)
		{
			if (left is null != right is null)
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

			for (int i = 0; i < left.MaterialFamilies.Count; ++i)
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

		public static bool operator !=(LayerInfo left, LayerInfo right)
		{
			return !(left == right);
		}
	}
}
