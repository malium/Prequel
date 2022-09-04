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

namespace Assets.AI.Quirks
{
	public abstract class IQuirkTrigger
	{
		protected List<IConfig> m_Configuration;
		string m_Name;
		bool m_Inverted;
		Def.QuirkTriggerType m_TriggerType;

		protected IQuirkTrigger(string name, Def.QuirkTriggerType triggerType)
		{
			m_Name = name;
			m_TriggerType = triggerType;
			m_Configuration = new List<IConfig>();
		}
		public string GetName() => m_Name;
		public bool IsInverted() => m_Inverted;
		public void SetInverted(bool inverted) => m_Inverted = inverted;
		public List<IConfig> GetConfig() => m_Configuration;
		public void SetConfig(IConfig config)
		{
			for (int i = 0; i < m_Configuration.Count; ++i)
			{
				var conf = m_Configuration[i];
				if (conf.GetConfigName() == config.GetConfigName())
				{
					conf.FromString(config.GetValueString());
					break;
				}
			}
		}
		protected abstract bool _TestCondition(CMonsterController monster);
		public bool TestCondition(CMonsterController monster)
		{
			var value = _TestCondition(monster);
			if (IsInverted())
				value = !value;
			return value;
		}
		public Def.QuirkTriggerType GetTriggerType() => m_TriggerType;
	}
	public class TriggerAwareness : IQuirkTrigger
	{
		ConfigEnum<Def.MonsterAwarenessState> WantedState;
		public TriggerAwareness()
			: base("AwarenessState", Def.QuirkTriggerType.AwarenessState)
		{
			WantedState = new ConfigEnum<Def.MonsterAwarenessState>("WantedState", Def.MonsterAwarenessState.CALM);
			m_Configuration.Add(WantedState);
		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			return WantedState.GetValue() == monster.GetAwarenessState();
		}
	}
	public class TriggerEntitesHeard : IQuirkTrigger
	{
		ConfigEntityList WantedEntities;
		public TriggerEntitesHeard()
			: base("EntitiesHeard", Def.QuirkTriggerType.EntitiesHeard)
		{
			WantedEntities = new ConfigEntityList("WantedEntities", new List<string>());
			m_Configuration.Add(WantedEntities);
		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			var entities = WantedEntities.GetValue();
			for(int i = 0; i < entities.Count; ++i)
			{
				var entityName = entities[i];
				switch(entityName)
				{
					case "Enemies":
						if (monster.GetHeardEnemies().Count == 0)
							return false;
						break;
					case "Friends":
						if (monster.GetHeardFriends().Count == 0)
							return false;
						break;
					case "Target":
						{
							var targetEntity = monster.GetTargetEntity();
							if (targetEntity == null)
								return false;
							var distance = Vector3.Distance(targetEntity.transform.position, monster.transform.position);
							if (distance > monster.GetMonster().GetFamily().Info.HearingRange)
								return false;
						}
						break;
					case "Self":
						break;
					case "LastHitter":
						{
							var lastHitter = monster.GetLE().GetLastHitter();
							if (lastHitter == null)
								return false;
							var distance = Vector3.Distance(lastHitter.transform.position, monster.transform.position);
							if (distance > monster.GetMonster().GetFamily().Info.HearingRange)
								return false;
						}
						break;
					default:
						var heardEntities = monster.GetHeardEntities();
						bool found = false;
						if (Monsters.FamilyTagDict.ContainsKey(entityName))
						{
							var mtag = Monsters.FamilyTags[Monsters.FamilyTagDict[entityName]];
							foreach (var entity in heardEntities)
							{
								if (entity.GetLEType() != Def.LivingEntityType.Monster)
									continue;
								var mon = entity.GetComponent<CMonster>();
								if (mon == null)
									continue;
								for (int j = 0; j < mtag.Friendships.Count; ++j)
								{
									var tag = mtag.Friendships[j];
									if (!tag.Friend)
										continue;
									if (mon.GetFamily().Name == tag.FamilyName)
									{
										found = true;
										break;
									}
								}
								if (found)
									break;
							}
						}
						else if (Props.FamilyTagDict.ContainsKey(entityName))
						{
							var pTag = Props.FamilyTags[Props.FamilyTagDict[entityName]];
							foreach (var entity in heardEntities)
							{
								if (entity.GetLEType() != Def.LivingEntityType.Prop)
									continue;
								var prop = entity.GetComponent<CProp>();
								if (prop == null)
									continue;
								for (int j = 0; j < pTag.Friendships.Count; ++j)
								{
									var tag = pTag.Friendships[j];
									if (!tag.Friend)
										continue;
									if (prop.GetInfo().FamilyName == tag.FamilyName)
									{
										found = true;
										break;
									}
								}
								if (found)
									break;
							}
						}
						else
						{
							foreach (var heard in heardEntities)
							{
								if (heard.GetName() == entityName)
								{
									found = true;
									break;
								}
							}
						}
						if (!found)
							return false;
						break;
				}
			}
			return true;
		}
	}
	public class TriggerEntitesOnSight : IQuirkTrigger
	{
		ConfigEntityList WantedEntities;
		public TriggerEntitesOnSight()
			: base("EntitiesOnSight", Def.QuirkTriggerType.EntitiesOnSight)
		{
			WantedEntities = new ConfigEntityList("WantedEntities", new List<string>());
			m_Configuration.Add(WantedEntities);
		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			var entities = WantedEntities.GetValue();
			for (int i = 0; i < entities.Count; ++i)
			{
				var entityName = entities[i];
				switch (entityName)
				{
					case "Enemies":
						if (monster.GetSeenEnemies().Count == 0)
							return false;
						break;
					case "Friends":
						if (monster.GetSeenFriends().Count == 0)
							return false;
						break;
					case "Target":
						{
							var targetEntity = monster.GetTargetEntity();
							if (targetEntity == null)
								return false;
							var distance = Vector3.Distance(targetEntity.transform.position, monster.transform.position);
							if (distance > monster.GetMonster().GetFamily().Info.SightRange)
								return false;

							float sightAngle = monster.GetMonster().GetFamily().Info.SightAngle * 0.5f;
							var entityPosXZ = new Vector2(targetEntity.transform.position.x, targetEntity.transform.position.z);
							var curPosXZ = new Vector2(monster.transform.position.x, monster.transform.position.z);
							var entityDir = (entityPosXZ - curPosXZ).normalized;
							var curAngle = Mathf.Abs(GameUtils.AngleBetween2D(monster.GetME().GetDirection(), entityDir)) * Mathf.Rad2Deg;
							if (curAngle > sightAngle)
								return false;
						}
						break;
					case "Self":
						return true;
					case "LastHitter":
						{
							var lastHitter = monster.GetLE().GetLastHitter();
							if (lastHitter == null)
								return false;
							var distance = Vector3.Distance(lastHitter.transform.position, monster.transform.position);
							if (distance > monster.GetMonster().GetFamily().Info.HearingRange)
								return false;

							float sightAngle = monster.GetMonster().GetFamily().Info.SightAngle * 0.5f;
							var entityPosXZ = new Vector2(lastHitter.transform.position.x, lastHitter.transform.position.z);
							var curPosXZ = new Vector2(monster.transform.position.x, monster.transform.position.z);
							var entityDir = (entityPosXZ - curPosXZ).normalized;
							var curAngle = Mathf.Abs(GameUtils.AngleBetween2D(monster.GetME().GetDirection(), entityDir)) * Mathf.Rad2Deg;
							if (curAngle > sightAngle)
								return false;
						}
						break;
					default:
						var seenEntities = monster.GetSeenEntities();
						bool found = false;
						if (Monsters.FamilyTagDict.ContainsKey(entityName))
						{
							var mtag = Monsters.FamilyTags[Monsters.FamilyTagDict[entityName]];
							foreach (var entity in seenEntities)
							{
								if (entity.GetLEType() != Def.LivingEntityType.Monster)
									continue;
								var mon = entity.GetComponent<CMonster>();
								if (mon == null)
									continue;
								for (int j = 0; j < mtag.Friendships.Count; ++j)
								{
									var tag = mtag.Friendships[j];
									if (!tag.Friend)
										continue;
									if (mon.GetFamily().Name == tag.FamilyName)
									{
										found = true;
										break;
									}
								}
								if (found)
									break;
							}
						}
						else if (Props.FamilyTagDict.ContainsKey(entityName))
						{
							var pTag = Props.FamilyTags[Props.FamilyTagDict[entityName]];
							foreach (var entity in seenEntities)
							{
								if (entity.GetLEType() != Def.LivingEntityType.Prop)
									continue;
								var prop = entity.GetComponent<CProp>();
								if (prop == null)
									continue;
								for (int j = 0; j < pTag.Friendships.Count; ++j)
								{
									var tag = pTag.Friendships[j];
									if (!tag.Friend)
										continue;
									if (prop.GetInfo().FamilyName == tag.FamilyName)
									{
										found = true;
										break;
									}
								}
								if (found)
									break;
							}
						}
						else
						{
							foreach (var heard in seenEntities)
							{
								if (heard.GetName() == entityName)
								{
									found = true;
									break;
								}
							}
						}
						if (!found)
							return false;
						break;
				}
			}
			return true;
		}
	}
	public class TriggerSpellState : IQuirkTrigger
	{
		ConfigEnum<Def.MonsterSpellSlots> WantedSpell;
		ConfigEnum<Def.SpellState> WantedState;

		public TriggerSpellState()
			: base("SpellState", Def.QuirkTriggerType.SpellState)
		{
			WantedSpell = new ConfigEnum<Def.MonsterSpellSlots>("WantedSpell", Def.MonsterSpellSlots.AUTO);
			WantedState = new ConfigEnum<Def.SpellState>("WantedState", Def.SpellState.IDLE);
			m_Configuration.Add(WantedSpell);
			m_Configuration.Add(WantedState);
		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			var spellSlot = WantedSpell.GetValue();
			if (spellSlot == Def.MonsterSpellSlots.COUNT)
				return false;
			var spell = monster.GetSpellCaster().GetSpell((int)spellSlot);
			if (spell == null)
				return false;
			return spell.GetCurrentState() == WantedState.GetValue();
		}
	}
	public class TriggerEntitiesNear : IQuirkTrigger
	{
		ConfigEntityList WantedEntities;
		ConfigFloat Distance;
		public TriggerEntitiesNear()
			: base("EntitiesNear", Def.QuirkTriggerType.EntitiesNear)
		{
			WantedEntities = new ConfigEntityList("WantedEntities", new List<string>());
			Distance = new ConfigFloat("Distance", 0f);

			m_Configuration.Add(WantedEntities);
			m_Configuration.Add(Distance);
		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			var info = monster.GetMonster().GetFamily().Info;
			var distanceChecked = Mathf.Max(info.SightRange, info.HearingRange);
			var distanceToCheck = Distance.GetValue();
			var nearStrucs = monster._GetNearStrucs();
			if (distanceChecked < distanceToCheck)
			{
				nearStrucs.Clear();
				var world = World.World.gWorld;
				world.StrucsRangeWPos(new Vector2(monster.transform.position.x, monster.transform.position.z), distanceToCheck, ref nearStrucs);
				//var vwPos = GameUtils.TransformPosition(new Vector2(monster.transform.position.x, monster.transform.position.z));
				//var vPos = world.VPosFromVWPos(vwPos);
				//world.StrucsRangeVPos(vPos, Mathf.FloorToInt(distanceToCheck), ref nearStrucs);
				//var map = Map.GetCurrent();
				//map.GetStrucsRange(new Vector2(monster.transform.position.x, monster.transform.position.z), distanceToCheck, ref nearStrucs);
				monster._SetNearStructs(nearStrucs, distanceToCheck);
			}
				
			var entities = WantedEntities.GetValue();
			for (int i = 0; i < entities.Count; ++i)
			{
				var entityName = entities[i];
				switch (entityName)
				{
					case "Enemies":
						{
							bool foundEnemies = false;
							for (int j = 0; j < nearStrucs.Count; ++j)
							{
								var sEntities = nearStrucs[j].GetLES();
								for (int k = 0; k < sEntities.Count; ++k)
								{
									var sEntity = sEntities[k];
									if (sEntity.GetLEType() == Def.LivingEntityType.Prop)
										continue;
									if (!monster.IsEnemy(sEntity))
										continue;
									var dist = Vector3.Distance(sEntity.transform.position, monster.transform.position);
									if (dist <= distanceToCheck)
									{
										foundEnemies = true;
										break;
									}
								}
							}

							if (!foundEnemies)
								return false;
						}
						break;
					case "Friends":
						{
							bool foundFriends = false;
							for (int j = 0; j < nearStrucs.Count; ++j)
							{
								var sEntities = nearStrucs[j].GetLES();
								for (int k = 0; k < sEntities.Count; ++k)
								{
									var sEntity = sEntities[k];
									if (sEntity.GetLEType() == Def.LivingEntityType.Prop)
										continue;
									if (!monster.IsFriend(sEntity))
										continue;
									var dist = Vector3.Distance(sEntity.transform.position, monster.transform.position);
									if (dist <= distanceToCheck)
									{
										foundFriends = true;
										break;
									}
								}
							}

							if (!foundFriends)
								return false;
						}
						break;
					case "Target":
						{
							var targetEntity = monster.GetTargetEntity();
							if (targetEntity == null)
								return false;

							var dist = Vector3.Distance(targetEntity.transform.position, monster.transform.position);
							if (dist > distanceToCheck)
								return false;
						}
						break;
					case "Self":
						return true;
					case "LastHitter":
						{
							var lastHitter = monster.GetLE().GetLastHitter();
							if (lastHitter == null)
								return false;
							var distance = Vector3.Distance(lastHitter.transform.position, monster.transform.position);
							if (distance > distanceToCheck)
								return false;
						}
						break;
					default:
						{
							bool foundEntity = false;
							if (Monsters.FamilyTagDict.ContainsKey(entityName))
							{
								var mtag = Monsters.FamilyTags[Monsters.FamilyTagDict[entityName]];
								for(int j = 0; j < nearStrucs.Count; ++j)
								{
									var sEntities = nearStrucs[j].GetLES();
									for(int k = 0; k < sEntities.Count; ++k)
									{
										var sEntity = sEntities[k];
										if (sEntity.GetLEType() != Def.LivingEntityType.Monster)
											continue;
										var mon = sEntity.GetComponent<CMonster>();
										if (mon == null)
											continue;
										var dist = Vector3.Distance(sEntity.transform.position, monster.transform.position);
										if (dist > distanceToCheck)
											continue;
										for (int w = 0; w < mtag.Friendships.Count; ++w)
										{
											var tag = mtag.Friendships[w];
											if (!tag.Friend)
												continue;
											if(mon.GetFamily().Name == tag.FamilyName)
											{
												foundEntity = true;
												break;
											}
										}
										if (foundEntity)
											break;
									}
									if (foundEntity)
										break;
								}
							}
							else if (Props.FamilyTagDict.ContainsKey(entityName))
							{
								var pTag = Props.FamilyTags[Props.FamilyTagDict[entityName]];
								for (int j = 0; j < nearStrucs.Count; ++j)
								{
									var sEntities = nearStrucs[j].GetLES();
									for (int k = 0; k < sEntities.Count; ++k)
									{
										var sEntity = sEntities[k];
										if (sEntity.GetLEType() != Def.LivingEntityType.Prop)
											continue;
										var prop = sEntity.GetComponent<CProp>();
										if (prop == null)
											continue;
										var dist = Vector3.Distance(sEntity.transform.position, monster.transform.position);
										if (dist > distanceToCheck)
											continue;
										for (int w = 0; w < pTag.Friendships.Count; ++w)
										{
											var tag = pTag.Friendships[w];
											if (!tag.Friend)
												continue;
											if (prop.GetInfo().FamilyName == tag.FamilyName)
											{
												foundEntity = true;
												break;
											}
										}
										if (foundEntity)
											break;
									}
									if (foundEntity)
										break;
								}
							}
							else
							{
								for (int j = 0; j < nearStrucs.Count; ++j)
								{
									var sEntities = nearStrucs[j].GetLES();
									for (int k = 0; k < sEntities.Count; ++k)
									{
										var sEntity = sEntities[k];
										if (sEntity.GetName() != entityName)
											continue;
										var dist = Vector3.Distance(sEntity.transform.position, monster.transform.position);
										if (dist <= distanceToCheck)
										{
											foundEntity = true;
											break;
										}
									}
									if (foundEntity)
										break;
								}
							}
							if (!foundEntity)
								return false;
						}
						break;
				}
			}
			return true;
		}
	}
	public class TriggerTargetNotNull : IQuirkTrigger
	{
		public TriggerTargetNotNull()
			:base("TargetNotNull", Def.QuirkTriggerType.TargetNotNull)
		{

		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			return monster.GetTargetEntity() != null;
		}
	}
	public class TriggerTimerReady : IQuirkTrigger
	{
		ConfigEnum<Def.MonsterTimers> WantedTimer;
		public TriggerTimerReady()
			:base("TimerReady", Def.QuirkTriggerType.TimerReady)
		{
			WantedTimer = new ConfigEnum<Def.MonsterTimers>("Wanted Timer", Def.MonsterTimers.TimerA);
			m_Configuration.Add(WantedTimer);
		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			return monster.GetMonsterTimer(WantedTimer.GetValue()).IsFinished();
		}
	}
	public class TriggerHasAttacked : IQuirkTrigger
	{
		public TriggerHasAttacked()
			:base("HasAttacked", Def.QuirkTriggerType.HasAttacked)
		{

		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			return monster.HasAttacked();
		}
	}
	public class TriggerTargetReached : IQuirkTrigger
	{
		public TriggerTargetReached()
			:base("TargetReached", Def.QuirkTriggerType.TargetReached)
		{

		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			return monster.IsTargetReached();
		}
	}
	public class TriggerStatus : IQuirkTrigger
	{
		ConfigEnum<Def.LEStats> m_Stat;
		ConfigEnum<Def.StatType> m_StatType;
		ConfigEnum<Def.Comparison> m_Comparison;
		ConfigFloat m_Value;
		public TriggerStatus()
			:base("Status", Def.QuirkTriggerType.Status)
		{
			m_Stat = new ConfigEnum<Def.LEStats>("Stat", Def.LEStats.Health);
			m_StatType = new ConfigEnum<Def.StatType>("Type", Def.StatType.PCT);
			m_Comparison = new ConfigEnum<Def.Comparison>("Compare", Def.Comparison.EQUAL);
			m_Value = new ConfigFloat("Value", 0f);

			m_Configuration.Add(m_Stat);
			m_Configuration.Add(m_StatType);
			m_Configuration.Add(m_Comparison);
			m_Configuration.Add(m_Value);
		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			float currentValue;
			switch (m_Stat.GetValue())
			{
				case Def.LEStats.Health:
					if(m_StatType.GetValue() == Def.StatType.FIXED)
					{
						currentValue = monster.GetLE().GetCurrentHealth();
					}
					else
					{
						currentValue = (monster.GetLE().GetCurrentHealth() / monster.GetLE().GetMaxHealth()) * 100f;
					}
					break;
				case Def.LEStats.Soulness:
					if (m_StatType.GetValue() == Def.StatType.FIXED)
					{
						currentValue = monster.GetLE().GetCurrentSoulness();
					}
					else
					{
						currentValue = (monster.GetLE().GetCurrentSoulness() / monster.GetLE().GetMaxSoulness()) * 100f;
					}
					break;
				default:
					Debug.LogWarning("Unhandled LEStat " + m_Stat.GetValue().ToString());
					return false;
			}
			switch (m_Comparison.GetValue())
			{
				case Def.Comparison.LESS:
					return m_Value.GetValue() < currentValue;
				case Def.Comparison.LESSEQUAL:
					return m_Value.GetValue() <= currentValue;
				case Def.Comparison.EQUAL:
					return m_Value.GetValue() == currentValue;
				case Def.Comparison.GREATEREQUAL:
					return m_Value.GetValue() >= currentValue;
				case Def.Comparison.GREATER:
					return m_Value.GetValue() > currentValue;
				default:
					Debug.LogWarning("Unhandled Comparison " + m_Comparison.GetValue().ToString());
					return false;
			}
			//return true;
		}
	}
	public class TriggerTargetStatus : IQuirkTrigger
	{
		ConfigEnum<Def.LEStats> m_Stat;
		ConfigEnum<Def.StatType> m_StatType;
		ConfigEnum<Def.Comparison> m_Comparison;
		ConfigFloat m_Value;
		public TriggerTargetStatus()
			: base("TargetStatus", Def.QuirkTriggerType.TargetStatus)
		{
			m_Stat = new ConfigEnum<Def.LEStats>("Stat", Def.LEStats.Health);
			m_StatType = new ConfigEnum<Def.StatType>("Type", Def.StatType.PCT);
			m_Comparison = new ConfigEnum<Def.Comparison>("Compare", Def.Comparison.EQUAL);
			m_Value = new ConfigFloat("Value", 0f);

			m_Configuration.Add(m_Stat);
			m_Configuration.Add(m_StatType);
			m_Configuration.Add(m_Comparison);
			m_Configuration.Add(m_Value);
		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			var target = monster.GetTargetEntity();
			if (target == null)
				return false;
			float currentValue;
			switch (m_Stat.GetValue())
			{
				case Def.LEStats.Health:
					if (m_StatType.GetValue() == Def.StatType.FIXED)
					{
						currentValue = target.GetCurrentHealth();
					}
					else
					{
						currentValue = (target.GetCurrentHealth() / target.GetMaxHealth()) * 100f;
					}
					break;
				case Def.LEStats.Soulness:

					if (m_StatType.GetValue() == Def.StatType.FIXED)
					{
						currentValue = target.GetCurrentSoulness();
					}
					else
					{
						currentValue = (target.GetCurrentSoulness() / target.GetMaxSoulness()) * 100f;
					}
					break;
				default:
					Debug.LogWarning("Unhandled LEStat " + m_Stat.GetValue().ToString());
					return false;
			}
			switch (m_Comparison.GetValue())
			{
				case Def.Comparison.LESS:
					return m_Value.GetValue() < currentValue;
				case Def.Comparison.LESSEQUAL:
					return m_Value.GetValue() <= currentValue;
				case Def.Comparison.EQUAL:
					return m_Value.GetValue() == currentValue;
				case Def.Comparison.GREATEREQUAL:
					return m_Value.GetValue() >= currentValue;
				case Def.Comparison.GREATER:
					return m_Value.GetValue() > currentValue;
				default:
					Debug.LogWarning("Unhandled Comparison " + m_Comparison.GetValue().ToString());
					return false;
			}
			//return true;
		}
	}
	public class TriggerOnHit : IQuirkTrigger
	{
		ConfigFloat m_HitWithinTime;
		public TriggerOnHit()
			:base("OnHit", Def.QuirkTriggerType.OnHit)
		{
			m_HitWithinTime = new ConfigFloat("HitWithinTime", 0.5f);
			m_Configuration.Add(m_HitWithinTime);
		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			var le = monster.GetLE();
			var diff = Time.time - le.GetLastHitTime();
			return diff <= m_HitWithinTime.GetValue();
		}
	}
	public class TriggerTargetIs : IQuirkTrigger
	{
		ConfigEnum<Def.TargetIs> m_Type;
		public TriggerTargetIs()
			:base("TargetIs", Def.QuirkTriggerType.TargetIs)
		{
			m_Type = new ConfigEnum<Def.TargetIs>("Type", Def.TargetIs.ENEMY);

			m_Configuration.Add(m_Type);
		}
		protected override bool _TestCondition(CMonsterController monster)
		{
			var target = monster.GetTargetEntity();
			if (target == null)
				return false;

			switch (m_Type.GetValue())
			{
				case Def.TargetIs.FRIEND:
					return monster.IsFriend(target);
				case Def.TargetIs.ENEMY:
					return monster.IsEnemy(target);
				case Def.TargetIs.PROP:
					return target.GetLEType() == Def.LivingEntityType.Prop;
				case Def.TargetIs.MONSTER:
					return target.GetLEType() == Def.LivingEntityType.Monster;
				case Def.TargetIs.ODD:
					if(target.GetLEType() == Def.LivingEntityType.Monster)
					{
						return target.GetName() == "ODD";
					}
					break;
				default:
					Debug.LogWarning("Unhandled TargetIs " + m_Type.GetValueString());
					break;
			}
			return false;
		}
	}
}
