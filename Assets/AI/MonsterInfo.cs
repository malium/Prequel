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

namespace Assets.AI
{
	public struct FamilyFriendship
	{
		public string FamilyName;
		public bool Friend;
	}
	public struct TriggerInfo
	{
		public Def.QuirkTriggerType TriggerType;
		public List<ConfigInfo> Configuration;
		public bool Inverted;
	}
	public struct QuirkInfo
	{
		public string QuirkName;
		public List<TriggerInfo> Triggers;
		public int Priority;
		public List<ConfigInfo> Configuration;
	}
	public struct OnHitConfig
	{
		public Def.OnHitType OnHitType;
		public string Configuration;
	}
	public class AttackInfo
	{
		public string AttackName;
		public List<ConfigInfo> Configuration;
		public List<OnHitConfig> OnHitConfiguration;
	}
	public class MonsterInfo
	{
		public static readonly string Path = UnityEngine.Application.streamingAssetsPath + "/Monsters";
		public string MonsterFamily;
		public float BaseHealth;
		public float HealthRegen;
		public float BaseSoulness;
		public float SoulnessRegen;
		public float BaseSpeed;
		public float Weight;
		public float SightRange;
		public float SightAngle;
		public float HearingRange;
		public float SpriteScale;
		public float StepDistance;
		public float PhysicalResistance;
		public bool PhysicalHealing;
		public float ElementalResistance;
		public bool ElementalHealing;
		public float UltimateResistance;
		public bool UltimateHealing;
		public float SoulResistance;
		public bool SoulHealing;
		public float PoisonResistance;
		public bool PoisonHealing;
		public float WarnTime = -1f;
		public float AlertTime = -1f;
		public List<FamilyFriendship> Friendships;
		public List<QuirkInfo> Quirks;
		public AttackInfo[] Attacks;
	}
}
