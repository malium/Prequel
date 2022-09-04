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
	public class QuirkChangeTarget : IQuirk
	{
		ConfigEnum<Def.ChangeTargetType> m_TargetType;
		ConfigEnum<Def.ChangeTargetSelection> m_TargetSelection;
		ConfigEntityList m_Entity;

		public QuirkChangeTarget()
			:base("ChangeTarget")
		{
			m_TargetType = new ConfigEnum<Def.ChangeTargetType>("Type", Def.ChangeTargetType.Distance);
			m_TargetSelection = new ConfigEnum<Def.ChangeTargetSelection>("Selection", Def.ChangeTargetSelection.SeenEnemies);
			m_Entity = new ConfigEntityList("Entity", new List<string>());

			m_Configuration.Add(m_TargetType);
			m_Configuration.Add(m_TargetSelection);
			m_Configuration.Add(m_Entity);
		}
		CLivingEntity TestEntity(HashSet<CLivingEntity> entities, List<string> entityTypes)
		{
			for (int i = 0; i < entityTypes.Count; ++i)
			{
				var type = entityTypes[i];
				switch (type)
				{
					case "Enemies":
					case "Friends":
					case "Target":
						break;
					case "Self":
						return m_Monster.GetLE();
					case "LastHitter":
						{
							var lastHitter = m_Monster.GetLE().GetLastHitter();
							if(lastHitter != null)
							{
								return lastHitter;
							}
						}
						break;
					default:
						if(Monsters.FamilyTagDict.ContainsKey(type))
						{
							var mtag = Monsters.FamilyTags[Monsters.FamilyTagDict[type]];
							foreach(var entity in entities)
							{
								if (entity.GetLEType() != Def.LivingEntityType.Monster)
									continue;
								var mon = entity.GetComponent<CMonster>();
								if (mon == null)
									continue;
								for(int j = 0; j < mtag.Friendships.Count; ++j)
								{
									var tag = mtag.Friendships[j];
									if (!tag.Friend)
										continue;
									if(mon.GetFamily().Name == tag.FamilyName)
									{
										return entity;
									}
								}
							}
						}
						else if(Props.FamilyTagDict.ContainsKey(type))
						{
							var pTag = Props.FamilyTags[Props.FamilyTagDict[type]];
							foreach(var entity in entities)
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
										return entity;
									}
								}
							}
						}
						else
						{
							foreach(var entity in entities)
							{
								if (entity.GetName() == type)
									return entity;
							}
						}
						break;
				}
			}
			return null;
		}
		CLivingEntity TestWeakest(HashSet<CLivingEntity> entities)
		{
			CLivingEntity newTarget = null;
			float weakest = float.MaxValue;
			foreach (var entity in entities)
			{
				if (entity.GetCurrentHealth() < weakest)
				{
					newTarget = entity;
					weakest = entity.GetCurrentHealth();
				}
			}
			return newTarget;
		}
		CLivingEntity TestStrongest(HashSet<CLivingEntity> entities)
		{
			CLivingEntity newTarget = null;
			float strongest = float.MinValue;
			foreach (var entity in entities)
			{
				if(entity.GetCurrentHealth() > strongest)
				{
					newTarget = entity;
					strongest = entity.GetCurrentHealth();
				}
			}
			return newTarget;
		}
		CLivingEntity TestDistance(HashSet<CLivingEntity> entities)
		{
			CLivingEntity newTarget = null;
			float smallestDist = float.MaxValue;
			foreach (var entity in entities)
			{
				var dist = Vector3.Distance(m_Monster.transform.position, entity.transform.position);
				if (smallestDist > dist)
				{
					newTarget = entity;
					smallestDist = dist;
				}
			}
			return newTarget;
		}
		void Test()
		{
			CLivingEntity newTarget = null;
			switch (m_TargetSelection.GetValue())
			{
				case Def.ChangeTargetSelection.HeardFriends:
					{
						switch (m_TargetType.GetValue())
						{
							case Def.ChangeTargetType.Distance:
								newTarget = TestDistance(m_Monster.GetHeardFriends());
								break;
							case Def.ChangeTargetType.Entity:
								newTarget = TestEntity(m_Monster.GetHeardFriends(), m_Entity.GetValue());
								break;
							case Def.ChangeTargetType.Weakest:
								newTarget = TestWeakest(m_Monster.GetHeardFriends());
								break;
							case Def.ChangeTargetType.Strongest:
								newTarget = TestStrongest(m_Monster.GetHeardFriends());
								break;
							default:
								Debug.LogWarning("Unhandled ChangeTargetType " + m_TargetType.ToString());
								break;
						}
					}
					break;
				case Def.ChangeTargetSelection.SeenFriends:
					{
						switch (m_TargetType.GetValue())
						{
							case Def.ChangeTargetType.Distance:
								newTarget = TestDistance(m_Monster.GetSeenFriends());
								break;
							case Def.ChangeTargetType.Entity:
								newTarget = TestEntity(m_Monster.GetSeenFriends(), m_Entity.GetValue());
								break;
							case Def.ChangeTargetType.Weakest:
								newTarget = TestWeakest(m_Monster.GetSeenFriends());
								break;
							case Def.ChangeTargetType.Strongest:
								newTarget = TestStrongest(m_Monster.GetSeenFriends());
								break;
							default:
								Debug.LogWarning("Unhandled ChangeTargetType " + m_TargetType.ToString());
								break;
						}
					}
					break;
				case Def.ChangeTargetSelection.HeardEnemies:
					switch (m_TargetType.GetValue())
					{
						case Def.ChangeTargetType.Distance:
							newTarget = TestDistance(m_Monster.GetHeardEnemies());
							break;
						case Def.ChangeTargetType.Entity:
							newTarget = TestEntity(m_Monster.GetHeardEnemies(), m_Entity.GetValue());
							break;
						case Def.ChangeTargetType.Weakest:
							newTarget = TestWeakest(m_Monster.GetHeardEnemies());
							break;
						case Def.ChangeTargetType.Strongest:
							newTarget = TestStrongest(m_Monster.GetHeardEnemies());
							break;
						default:
							Debug.LogWarning("Unhandled ChangeTargetType " + m_TargetType.ToString());
							break;
					}
					break;
				case Def.ChangeTargetSelection.HeardEntities:
					switch (m_TargetType.GetValue())
					{
						case Def.ChangeTargetType.Distance:
							newTarget = TestDistance(m_Monster.GetHeardEntities());
							break;
						case Def.ChangeTargetType.Entity:
							newTarget = TestEntity(m_Monster.GetHeardEntities(), m_Entity.GetValue());
							break;
						case Def.ChangeTargetType.Weakest:
							newTarget = TestWeakest(m_Monster.GetHeardEntities());
							break;
						case Def.ChangeTargetType.Strongest:
							newTarget = TestStrongest(m_Monster.GetHeardEntities());
							break;
						default:
							Debug.LogWarning("Unhandled ChangeTargetType " + m_TargetType.ToString());
							break;
					}
					break;
				case Def.ChangeTargetSelection.SeenEnemies:
					switch (m_TargetType.GetValue())
					{
						case Def.ChangeTargetType.Distance:
							newTarget = TestDistance(m_Monster.GetSeenEnemies());
							break;
						case Def.ChangeTargetType.Entity:
							newTarget = TestEntity(m_Monster.GetSeenEnemies(), m_Entity.GetValue());
							break;
						case Def.ChangeTargetType.Weakest:
							newTarget = TestWeakest(m_Monster.GetSeenEnemies());
							break;
						case Def.ChangeTargetType.Strongest:
							newTarget = TestStrongest(m_Monster.GetSeenEnemies());
							break;
						default:
							Debug.LogWarning("Unhandled ChangeTargetType " + m_TargetType.ToString());
							break;
					}
					break;
				case Def.ChangeTargetSelection.SeenEntities:
					switch (m_TargetType.GetValue())
					{
						case Def.ChangeTargetType.Distance:
							newTarget = TestDistance(m_Monster.GetSeenEntities());
							break;
						case Def.ChangeTargetType.Entity:
							newTarget = TestEntity(m_Monster.GetSeenEntities(), m_Entity.GetValue());
							break;
						case Def.ChangeTargetType.Weakest:
							newTarget = TestWeakest(m_Monster.GetSeenEntities());
							break;
						case Def.ChangeTargetType.Strongest:
							newTarget = TestStrongest(m_Monster.GetSeenEntities());
							break;
						default:
							Debug.LogWarning("Unhandled ChangeTargetType " + m_TargetType.ToString());
							break;
					}
					break;
				default:
					Debug.LogWarning("Unhandled ChangeTargetSelection " + m_TargetSelection.ToString());
					break;
			}
			if (newTarget == null)
				return;

			m_Monster.SetTargetEntity(newTarget);
		}
		public override void OnFirstTrigger()
		{
			Test();
		}
		public override void UpdateQuirk()
		{
			Test();
		}
	}
}
