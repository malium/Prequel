/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Assets.AI
{
	public class CMonsterController : MonoBehaviour
	{
		//public enum State
		//{
		//	PASSIVE,
		//	TRANSITION,
		//	ACTIVE,
		//}

		CLivingEntity m_LE;
		CMovableEntity m_ME;
		CSpellCaster m_SpellCaster;
		CMonster m_Monster;
		//State m_CurrentState;
		Def.MonsterAwarenessState m_AwarenessState;
		Timer m_AwarenessTimer;
		Timer[] m_Timers;
		//float m_NextAwarenessTime;

		List<Quirks.QuirkInfo> m_Quirks;
		Quirks.IQuirk m_CurrentQuirk;

		List<CStruc> m_NearStrucs;
		float m_NearStrucsDistance;

		HashSet<CLivingEntity> m_HeardEntities;
		HashSet<CLivingEntity> m_SeenEntities;
		HashSet<CLivingEntity> m_HeardEnemies;
		HashSet<CLivingEntity> m_SeenEnemies;
		HashSet<CLivingEntity> m_HeardFriends;
		HashSet<CLivingEntity> m_SeenFriends;

		HashSet<CLivingEntity> m_Enemies;

		Dictionary<string, bool> m_Friendships;
		//Vector3 m_PreviousPos;
		Vector3 m_TargetPostion;
		Vector3 m_TargetEntityPosition;
		CLivingEntity m_TargetEntity;
		bool m_HasAttacked;
		Vector3 m_StartPosition;
		float m_TargetStartDistance;
		//CBlock m_CurrentBlock;
		bool m_TargetReached;
		float m_WarningTime;
		float m_AlertTime;
		//static int m_BlockMask = Def.RayCastMask(true, false, false, false);

		public float GUIHeight { get; private set; }

		public bool IsFriend(CLivingEntity entity)
		{
			return !IsEnemy(entity);
		}
		public bool IsEnemy(CLivingEntity entity)
		{
			if (entity == null)
				return false;
			switch (entity.GetLEType())
			{
				case Def.LivingEntityType.ODD:
					if (m_Enemies.Contains(entity))
						return true;
					return !m_Friendships["ODD"];
				case Def.LivingEntityType.Monster:
					{
						if (m_Enemies.Contains(entity))
							return true;
						//var isOddControlled = entity.GetComponent<CMonsterOddController>() != null;
						//if(isOddControlled)
						//{

						//}
						var mon = entity.GetComponent<CMonster>();
						return !m_Friendships[mon.GetFamily().Name];
					}
				default:
					return false;
			}
		}
		public CLivingEntity GetLE() => m_LE;
		public CMovableEntity GetME() => m_ME;
		public CSpellCaster GetSpellCaster() => m_SpellCaster;
		public CMonster GetMonster() => m_Monster;
		public bool HasAttacked() => m_HasAttacked;
		public Def.MonsterAwarenessState GetAwarenessState() => m_AwarenessState;
		public HashSet<CLivingEntity> GetHeardEntities() => m_HeardEntities;
		public HashSet<CLivingEntity> GetHeardEnemies() => m_HeardEnemies;
		public HashSet<CLivingEntity> GetHeardFriends() => m_HeardFriends;
		public HashSet<CLivingEntity> GetSeenEntities() => m_SeenEntities;
		public HashSet<CLivingEntity> GetSeenEnemies() => m_SeenEnemies;
		public HashSet<CLivingEntity> GetSeenFriends() => m_SeenFriends;
		public Timer GetMonsterTimer(Def.MonsterTimers timer) => m_Timers[(int)timer];
		public List<CStruc> _GetNearStrucs() => m_NearStrucs;
		public void _SetNearStructs(List<CStruc> strucs, float distance)
		{
			m_NearStrucs = strucs;
			m_NearStrucsDistance = distance;
		}
		public float _GetNearStrucsDistance() => m_NearStrucsDistance;
		//public List<Quirks.IQuirk> GetPassiveQuirks() => m_PassiveQuirks;
		//public List<Quirks.IQuirk> GetActiveQuirks() => m_ActiveQuirks;
		//public List<Quirks.IQuirk> GetCurrentQuirks() => m_CurrentState == State.PASSIVE ? m_PassiveQuirks : m_ActiveQuirks;
		public Dictionary<string, bool> GetFriendships() => m_Friendships;
		public void OnFamilyUpdated()
		{
			var info = m_Monster.GetFamily().Info;

			m_WarningTime = info.WarnTime;
			m_AlertTime = info.AlertTime;

			m_Friendships.Clear();
			for (int i = 1; i < Monsters.MonsterFamilies.Count; ++i)
			{
				m_Friendships.Add(Monsters.MonsterFamilies[i].Name, false);
			}
			m_Friendships.Add("ODD", false);

			for(int i = 0; i < m_Monster.GetFamily().FamilyTags.Count; ++i)
			{
				var tagName = m_Monster.GetFamily().FamilyTags[i];
				var tag = Monsters.FamilyTags[Monsters.FamilyTagDict[tagName]];

				for (int j = 0; j < tag.Friendships.Count; ++j)
				{
					m_Friendships[tag.Friendships[j].FamilyName] = true;
				}
			}
			
			for (int i = 0; i < info.Friendships.Count; ++i)
			{
				var excp = info.Friendships[i];
				m_Friendships[excp.FamilyName] = excp.Friend;
			}
			m_Quirks.Clear();
			for(int i = 0; i < m_Monster.GetFamily().Info.Quirks.Count; ++i)
			{
				var quirkFamilyInfo = m_Monster.GetFamily().Info.Quirks[i];
				
				var triggers = new List<Quirks.IQuirkTrigger>(quirkFamilyInfo.Triggers.Count);
				for(int j = 0; j < quirkFamilyInfo.Triggers.Count; ++j)
				{
					var triggerInfo = quirkFamilyInfo.Triggers[j];
					var trigger = Quirks.QuirkManager.CreateTrigger(triggerInfo.TriggerType);

					for (int k = 0; k < triggerInfo.Configuration.Count; ++k)
					{
						var conf = triggerInfo.Configuration[k];
						IConfig enumConfig = null;
						if (conf.ConfigType == Def.ConfigType.ENUM)
						{
							for (int w = 0; w < trigger.GetConfig().Count; ++w)
							{
								var config = trigger.GetConfig()[w];
								if (config.GetConfigName() == conf.Name)
								{
									enumConfig = config;
									break;
								}
							}
						}
						trigger.SetConfig(conf.Create(enumConfig));
						trigger.SetInverted(triggerInfo.Inverted);
					}

					triggers.Add(trigger);
				}

				var quirk = Quirks.QuirkManager.CreateQuirk(quirkFamilyInfo.QuirkName);
				for (int j = 0; j < quirkFamilyInfo.Configuration.Count; ++j)
				{
					var conf = quirkFamilyInfo.Configuration[j];
					IConfig enumConfig = null;
					if (conf.ConfigType == Def.ConfigType.ENUM)
					{
						for (int k = 0; k < quirk.GetConfiguration().Count; ++k)
						{
							var config = quirk.GetConfiguration()[k];
							if (config.GetConfigName() == conf.Name)
							{
								enumConfig = config;
								break;
							}
						}
					}
					quirk.SetConfiguration(conf.Create(enumConfig));
				}
				quirk.SetMonster(this);

				var quirkInfo = new Quirks.QuirkInfo()
				{
					Priority = quirkFamilyInfo.Priority,
					Quirk = quirk,
					Triggers = triggers
				};
				m_Quirks.Add(quirkInfo);
			}
			int quirkCount = m_Quirks.Count;
			var tempQuirkList = new List<Quirks.QuirkInfo>(m_Quirks.Count);
			for(int i = 0; i < quirkCount; ++i)
			{
				int maxPriority = int.MinValue;
				int maxPriorityIndex = -1;
				for(int j = 0; j < m_Quirks.Count; ++j)
				{
					var quirk = m_Quirks[j];
					if(quirk.Priority > maxPriority)
					{
						maxPriorityIndex = j;
						maxPriority = quirk.Priority;
					}
				}
				tempQuirkList.Add(m_Quirks[maxPriorityIndex]);
				m_Quirks.RemoveAt(maxPriorityIndex);
			}
			m_Quirks = tempQuirkList;
			// Quirks
			//void InitQuirks(List<QuirkInfo> infos, List<Quirks.IQuirk> quirks)
			//{
			//	quirks.Clear();
			//	for (int i = 0; i < infos.Count; ++i)
			//	{
			//		var quirkConf = infos[i];
			//		var quirk = Quirks.QuirkManager.CreateQuirk(quirkConf.QuirkName);
			//		for (int j = 0; j < quirkConf.Configuration.Count; ++j)
			//		{
			//			var cur = quirkConf.Configuration[j];
			//			IConfig enumConfig = null;
			//			if (cur.ConfigType == ConfigType.ENUM)
			//			{
			//				for (int k = 0; k < quirk.GetCurrentConfig().Count; ++k)
			//				{
			//					var config = quirk.GetCurrentConfig()[k];
			//					if (config.GetConfigName() == cur.Name)
			//					{
			//						enumConfig = config;
			//						break;
			//					}
			//				}
			//			}
			//			quirk.SetConfig(cur.Create(enumConfig));
			//		}
			//		quirks.Add(quirk);
			//		quirk.Init(this);
			//	}
			//}
			//InitQuirks(info.PassiveQuirks, m_PassiveQuirks);
			//InitQuirks(info.ActiveQuirks, m_ActiveQuirks);
			//FillCurrentQuirks(GetCurrentQuirks());
			//for(int i = 0; i < Def.MonsterSpellSlotsCount; ++i)
			//{
			//	var spell = m_Monster.GetSpell((Def.MonsterSpellSlots)i);
			//	if (spell == null)
			//		continue;
			//}
		}
		public void Set(CMonster monster)
		{
			m_Monster = monster;
			m_LE = monster.GetLE();
			m_ME = monster.GetME();
			m_SpellCaster = monster.GetSpellCaster();
			//m_LE.OnReceiveDamage += OnReceiveDamage;
			//m_LE.OnReceiveElement += OnReceiveElement;
			m_LE.OnEntityDeath += OnEntityDeath;
			//m_ME.OnTargetReached += OnTargetReached;
			//m_PreviousPos = transform.position;
			//m_CurrentState = State.PASSIVE;
			OnFamilyUpdated();
		}
		bool SpellFilter(string leName)
		{
			if(m_Friendships.ContainsKey(leName))
			{
				return m_Friendships[leName];
			}
			return false;
		}
		void OnEntityDeath(CLivingEntity entity)
		{
			m_LE.OnEntityDeath -= OnEntityDeath;
			enabled = false;
			m_LE.GetCollider().enabled = false;
			var anim = gameObject.AddComponent<FadeoutAnimation>();
			anim.Set(() => m_Monster.GetSprite().GetColor(),
				(Color color) =>
				{
					m_Monster.GetSprite().SetColor(color);
					m_Monster.GetShadow().SetColor(color);
					if(m_LE.GetStatusBars() != null)
						m_LE.GetStatusBars().SetAlpha(color.a);
				},
				1f, false, () =>
				{
					GameUtils.DeleteGameobject(gameObject);
				});
		}
		public void SetTargetPostion(Vector3 position)
		{
			m_TargetPostion = position;
			m_StartPosition = transform.position;
			m_TargetReached = transform.position == position;
			m_TargetStartDistance = Vector2.Distance(
				new Vector2(position.x, position.z),
				new Vector2(transform.position.x, transform.position.z));
		}
		public Vector3 GetTargetPosition() => m_TargetPostion;
		public bool IsTargetReached() => m_TargetReached;
		public void SetTargetEntity(CLivingEntity entity)
		{
			if (m_TargetEntity == entity)
				return;

			m_TargetEntity = entity;
			m_HasAttacked = false;
			if (m_TargetEntity != null)
			{
				m_AwarenessTimer.Stop();
			}
			else if(m_AwarenessState == Def.MonsterAwarenessState.ALERT)
			{
				m_AwarenessTimer.Reset(m_Monster.GetFamily().Info.AlertTime);
			}
		}
		public CLivingEntity GetTargetEntity() => m_TargetEntity;
		public bool IsTargetEntityPositionInvalid()
		{
			return m_TargetEntityPosition.x < 0f || m_TargetEntityPosition.z < 0f;
		}
		public void SetTargetEntityPosition(Vector3 position) => m_TargetEntityPosition = position;
		public Vector3 GetTargetEntityPosition() => m_TargetEntityPosition;
		public void InvalidateTargetEntityPosition() => m_TargetEntityPosition = new Vector3(float.MinValue, float.MinValue, float.MinValue); 
		void UpdateAwareness()
		{
			// Calm conditions
			if(m_TargetEntity == null &&
				IsTargetEntityPositionInvalid() &&
				m_SeenEnemies.Count == 0 &&
				m_HeardEnemies.Count == 0 &&
				m_AwarenessTimer.IsFinished())
			{
				switch (m_AwarenessState)
				{
					case Def.MonsterAwarenessState.WARN:
						m_AwarenessTimer.Reset(m_Monster.GetFamily().Info.WarnTime);
						return;
					case Def.MonsterAwarenessState.ALERT:
						m_AwarenessTimer.Reset(m_Monster.GetFamily().Info.AlertTime);
						return;
					default:
						return;
				}
			}
			// Warn conditions
			if (m_HeardEnemies.Count > 0 &&
				m_TargetEntity == null &&
				!IsTargetEntityPositionInvalid())
			{
				switch (m_AwarenessState)
				{
					case Def.MonsterAwarenessState.CALM:
						m_AwarenessState = Def.MonsterAwarenessState.WARN;
						return;
					case Def.MonsterAwarenessState.ALERT:
						m_AwarenessTimer.Reset(m_Monster.GetFamily().Info.AlertTime);
						return;
					default:
						return;
				}
			}
			// Alert conditions
			if (m_TargetEntity != null && m_AwarenessTimer.IsFinished())// && m_AwarenessState != Def.MonsterAwarenessState.ALERT)
			{
				m_AwarenessState = Def.MonsterAwarenessState.ALERT;
				//m_AwarenessTimer.Stop();
				return;
			}
		}
		void OnAwerenessChange()
		{
			switch (m_AwarenessState)
			{
				case Def.MonsterAwarenessState.WARN:
					m_AwarenessState = Def.MonsterAwarenessState.CALM;
					InvalidateTargetEntityPosition();
					break;
				case Def.MonsterAwarenessState.ALERT:
					m_TargetEntity = null;
					m_AwarenessState = Def.MonsterAwarenessState.WARN;
					break;
			}
		}
		public void SetAwareness(Def.MonsterAwarenessState state, float time)
		{
			m_AwarenessState = state;
			m_AwarenessTimer.Reset(time);
		}
		void UpdateHeardEntities()
		{
			m_HeardEntities.Clear();
			m_HeardEnemies.Clear();
			m_HeardFriends.Clear();
			m_NearStrucs.Clear();
			float maxRange = m_Monster.GetFamily().Info.HearingRange;
			if (m_LE.GetCurrentStruc() != null)
			{
				var world = World.World.gWorld;
				world.StrucsRangeWPos(new Vector2(transform.position.x, transform.position.z), maxRange, ref m_NearStrucs);
				//var vwPos = GameUtils.TransformPosition(new Vector2(transform.position.x, transform.position.z));
				//var vPos = world.VPosFromVWPos(vwPos);
				//world.StrucsRangeVPos(vPos, Mathf.FloorToInt(maxRange), ref m_NearStrucs);
				//var map = Map.GetCurrent();
				//map.GetStrucsRange(new Vector2(transform.position.x, transform.position.z), maxRange, ref m_NearStrucs);
				for (int i = 0; i < m_NearStrucs.Count; ++i)
				{
					var entities = m_NearStrucs[i].GetLES();
					for (int j = 0; j < entities.Count; ++j)
					{
						var entity = entities[j];
						if (entity == null || entity == GetLE())
							continue; // myself or null
						if (m_HeardEntities.Contains(entity))
							continue; // already added
						if (Vector3.Distance(transform.position, entity.transform.position) > m_Monster.GetFamily().Info.HearingRange)
							continue; // too far
						if(entity.TryGetComponent(out CMovableEntity me))
						{
							if (me.GetMovementState() == Def.MovementState.Stopped)
								continue;
						}
						else
						{
							continue;
						}
						m_HeardEntities.Add(entity);
						if (IsEnemy(entity))
							m_HeardEnemies.Add(entity);
						else
							m_HeardFriends.Add(entity);
					}
				}
			}
			if(m_AwarenessState == Def.MonsterAwarenessState.CALM && m_HeardEnemies.Count > 0)
			{
				var enemyIdx = UnityEngine.Random.Range(0, m_HeardEnemies.Count);
				int idx = 0;
				foreach(var enemy in m_HeardEnemies)
				{
					if(enemyIdx == idx)
					{
						m_TargetEntityPosition = enemy.transform.position;
						break;
					}
					++idx;
				}
			}
			if(m_AwarenessState == Def.MonsterAwarenessState.WARN && m_HeardEnemies.Count == 0 && m_AwarenessTimer.IsFinished())
			{
				m_AwarenessTimer.Reset(m_Monster.GetFamily().Info.WarnTime);
			}
		}
		void UpdateSeenEntities()
		{
			var info = m_Monster.GetFamily().Info;
			float sightRange = info.SightRange;
			float sightAngle = info.SightAngle * 0.5f;
			m_SeenEntities.Clear();
			m_SeenEnemies.Clear();
			m_SeenFriends.Clear();
			if (info.HearingRange < sightRange)
			{
				//var map = Map.GetCurrent();
				m_NearStrucs.Clear();
				var world = World.World.gWorld;
				world.StrucsRangeWPos(new Vector2(transform.position.x, transform.position.z), sightRange, ref m_NearStrucs);
				//var vwPos = GameUtils.TransformPosition(new Vector2(transform.position.x, transform.position.z));
				//var vPos = world.VPosFromVWPos(vwPos);
				//world.StrucsRangeVPos(vPos, Mathf.FloorToInt(sightRange), ref m_NearStrucs);
				//map.GetStrucsRange(new Vector2(transform.position.x, transform.position.z), sightRange, ref m_NearStrucs);
			}
			for(int i = 0; i < m_NearStrucs.Count; ++i)
			{
				var entities = m_NearStrucs[i].GetLES();
				for (int j = 0; j < entities.Count; ++j)
				{
					var entity = entities[j];
					if (entity == null || entity == GetLE())
						continue; // myself or null
					if (m_SeenEntities.Contains(entity))
						continue;
					float curDistance = Vector3.Distance(transform.position, entity.transform.position);
					if (curDistance > sightRange)
					{
						//Debug.Log("Entity not seen, too far " + entity.GetName() + " " + curDistance);
						continue; // too far
					}

					var entityPosXZ = new Vector2(entity.transform.position.x, entity.transform.position.z);
					var curPosXZ = new Vector2(transform.position.x, transform.position.z);
					var entityDir = (entityPosXZ - curPosXZ).normalized;
					var curAngle = Mathf.Abs(GameUtils.AngleBetween2D(m_ME.GetDirection(), entityDir)) * Mathf.Rad2Deg;

					if (curAngle > sightAngle)
					{
						//Debug.Log("Entity not seen, not angle " + entity.GetName() + " " + curAngle.ToString());
						continue; // Out of sight
					}
					//Debug.Log("Entity seen, distance " + curDistance.ToString() + " angle " + curAngle.ToString() + " " + entity.GetName());

					m_SeenEntities.Add(entity);
					if (IsEnemy(entity))
					{
						m_SeenEnemies.Add(entity);
						//Debug.Log("Enemy seen, distance " + curDistance.ToString() + " angle " + curAngle.ToString() + " " + entity.GetName());
					}
					else
					{
						m_SeenFriends.Add(entity);
						//Debug.Log("Friend seen, distance " + curDistance.ToString() + " angle " + curAngle.ToString() + " " + entity.GetName());
					}
				}
			}
			if(m_TargetEntity != null && m_AwarenessTimer.IsFinished() && !m_SeenEnemies.Contains(m_TargetEntity))
			{
				m_TargetEntityPosition = m_TargetEntity.transform.position;
				m_AwarenessTimer.Reset(m_Monster.GetFamily().Info.AlertTime);
				//m_TargetEntity = null;
			}
		}
		void UpdateQuirkTriggers()
		{
			bool triggered = false;
			for(int i = 0; i < m_Quirks.Count; ++i)
			{
				var quirkInfo = m_Quirks[i];
				bool isTriggered = true;
				for(int j = 0; j < quirkInfo.Triggers.Count; ++j)
				{
					var trigger = quirkInfo.Triggers[j];
					if(!trigger.TestCondition(this))
					{
						isTriggered = false;
						//Debug.Log("Quirk: " + quirkInfo.Quirk.GetName() + " not triggered: " + trigger.GetName());
						break;
					}
				}
				if(isTriggered)
				{
					triggered = true;
					if(m_CurrentQuirk != quirkInfo.Quirk)
					{
						if(m_CurrentQuirk != null)
						{
							m_CurrentQuirk.OnTransitioning(quirkInfo.Quirk);
						}
						m_CurrentQuirk = quirkInfo.Quirk;
						m_CurrentQuirk.OnFirstTrigger();
					}
					break;
				}
			}
			if (!triggered)
				m_CurrentQuirk = null;
		}
		//void OnTargetReached(CMovableEntity entity)
		//{
		//	//ForEachQuirk((Quirks.IQuirk quirk) => { quirk.TargetReached(); });
		//}
		//void OnReceiveElement(CLivingEntity receiver, Def.ElementType type, float damageAmout)
		//{
		//	//ForEachQuirk((Quirks.IQuirk quirk) => { quirk.ElementReceived(type, damageAmout); });
		//}
		//void OnReceiveDamage(CLivingEntity caster, CLivingEntity receiver, Def.DamageType type, float damageAmout)
		//{
		//	//ForEachQuirk((Quirks.IQuirk quirk) => { quirk.DamageReceived(caster, type, damageAmout); });
		//}
		//void ForEachQuirk(Action<Quirks.IQuirk> fn)
		//{
		//	for (int i = 0; i < m_CurrentQuirks.Count; ++i)
		//	{
		//		var quirk = m_CurrentQuirks[i];
		//		if (!quirk.IsEnabled())
		//			continue;
		//		fn(quirk);
		//	}
		//}
		void UpdateMovement()
		{
			var oldPos = transform.position;
			if (m_TargetReached)
			{
				m_ME.Impulse(m_ME.GetDirection(), 0f);
				if(m_ME.GetCurrentMoment() > 0f)
					m_LE.UpdateStruc();
			}
			else
			{
				var posXZ = new Vector2(transform.position.x, transform.position.z);
				var targetXZ = new Vector2(m_TargetPostion.x, m_TargetPostion.z);
				var dir = targetXZ - posXZ;
				m_ME.Impulse(dir);
				m_ME.SetSightDirection(dir);
				//dir = m_ME.GetDirection();
				m_LE.UpdateStruc();
				m_LE.UpdateBlock();
			}

			var struc = m_LE.GetCurrentStruc();
			if(struc == null)
			{
				m_LE.UpdateStruc();
				struc = m_LE.GetCurrentStruc();
			}
			if (m_LE.GetCurrentBlock() == null && struc != null)
			{
				m_LE.UpdateBlock();
				//m_CurrentBlock = Map.GetCurrent().GetBlockAt(transform.position);
			}

			Vector2 movXZ = m_ME.UpdateMovement();
			var mov = new Vector3(movXZ.x, GameUtils.Gravity, movXZ.y);
			mov *= Time.deltaTime;

			var cc = m_ME.GetController();
			var collision = cc.Move(mov);
			if (collision.HasFlag(CollisionFlags.CollidedSides))
				m_ME.OnCollision();

			//var tempBlock = Map.GetCurrent().GetBlockAt(transform.position);
			var tempBlock = m_LE.ComputeCurrentBlock() as CBlock;
			if (m_LE.GetCurrentBlock() != null && tempBlock == null)
			{
				transform.position = oldPos;
				m_ME.OnCollision();
			}
			else
			{
				m_LE.SetCurrentBlock(tempBlock);
				//m_CurrentBlock = tempBlock;
			}

			if (!m_TargetReached)
			{
				var dist = Vector2.Distance(
					new Vector2(m_StartPosition.x, m_StartPosition.z),
					new Vector2(transform.position.x, transform.position.z));
				if (dist >= m_TargetStartDistance)
					m_TargetReached = true;
			}
		}
		void UpdateMonsterTimers()
		{
			for (int i = 0; i < Def.MonsterTimerCount; ++i)
				m_Timers[i].Update();
		}
		private void Update()
		{
			if (Manager.Mgr.IsPaused)
				return;
			//var newPos = transform.position;
			//if(m_PreviousPos != newPos)
			//{
			//	ForEachQuirk((Quirks.IQuirk quirk) => { quirk.Moved(m_PreviousPos, newPos); });
			//}

			m_AwarenessTimer.Update();
			m_Monster.FrameUpdate();
			UpdateMovement();
			UpdateMonsterTimers();

			if (m_CurrentQuirk != null)
				m_CurrentQuirk.UpdateQuirk();
			//ForEachQuirk((Quirks.IQuirk quirk) => { quirk.Update(); });

			m_SpellCaster.UpdateSpells();
			m_SpellCaster.UpdateItems();

			m_LE.UpdateElements();
			//m_PreviousPos = transform.position;
			
		}
		//void FillCurrentQuirks(List<Quirks.IQuirk> quirks)
		//{
		//	m_CurrentQuirks.Clear();
		//	for (int i = 0; i < quirks.Count; ++i)
		//	{
		//		var quirk = quirks[i];
		//		if (quirk.IsEnabled())
		//			m_CurrentQuirks.Add(quirk);
		//	}
		//}
		private void FixedUpdate()
		{
			if (Manager.Mgr.IsPaused)
				return;
			//m_LE.UpdateStruc();
			UpdateHeardEntities();
			UpdateSeenEntities();
			UpdateAwareness();
			UpdateQuirkTriggers();
			//FilterEntities();
			//if(m_NearEnemies.Count > 0)
			//{
			//	if(m_CurrentState == State.PASSIVE)
			//	{
			//		FillCurrentQuirks(m_ActiveQuirks);
			//		m_CurrentState = State.ACTIVE;
			//	}
			//}
			//else
			//{
			//	if(m_CurrentState == State.ACTIVE && m_AwarenessState == Def.MonsterAwarenessState.CALM)
			//	{
			//		FillCurrentQuirks(m_PassiveQuirks);
			//		m_CurrentState = State.PASSIVE;
			//	}
			//}

			//ForEachQuirk((Quirks.IQuirk quirk) => { quirk.FixedUpdate(); });
		}
		private void Awake()
		{
			//m_CurrentState = State.PASSIVE;
			//m_PassiveQuirks = new List<Quirks.IQuirk>();
			//m_ActiveQuirks = new List<Quirks.IQuirk>();
			//m_CurrentQuirks = new List<Quirks.IQuirk>();
			m_NearStrucs = new List<CStruc>(8);
			m_HeardEntities = new HashSet<CLivingEntity>();
			m_HeardEnemies = new HashSet<CLivingEntity>();
			m_HeardFriends = new HashSet<CLivingEntity>();
			m_SeenEntities = new HashSet<CLivingEntity>();
			m_SeenEnemies = new HashSet<CLivingEntity>();
			m_SeenFriends = new HashSet<CLivingEntity>();
			m_Enemies = new HashSet<CLivingEntity>();
			//m_NearFriends = new List<CLivingEntity>(32);
			//m_NearEnemies = new List<CLivingEntity>(32);
			m_Friendships = new Dictionary<string, bool>(Monsters.MonsterFamilies.Count);
			m_AwarenessState = Def.MonsterAwarenessState.CALM;
			m_AwarenessTimer = new Timer();
			m_AwarenessTimer.OnTimerTrigger += OnAwerenessChange;
			m_Quirks = new List<Quirks.QuirkInfo>();
			InvalidateTargetEntityPosition();
			m_HasAttacked = false;
			m_TargetEntity = null;
			m_Timers = new Timer[Def.MonsterTimerCount];
			for (int i = 0; i < Def.MonsterTimerCount; ++i)
				m_Timers[i] = new Timer();
		}
		//public void OnQuirkDisabled(Quirks.IQuirk quirk)
		//{
		//	if(m_CurrentQuirks.Contains(quirk))
		//	{
		//		m_CurrentQuirks.Remove(quirk);
		//	}
		//}
		//public void OnQuirkEnabled(Quirks.IQuirk quirk)
		//{
		//	var quirks = GetCurrentQuirks();
		//	if (!quirks.Contains(quirk))
		//		return;
		//	m_CurrentQuirks.Add(quirk);
		//}
		private void OnGUI()
		{
			if(m_LE == null || m_LE.GUIHeight < 0f || !Manager.Mgr.DebugAI)
			{
				GUIHeight = 0f;
				return;
			}

			var oldColor = GUI.contentColor;

			var cam = CameraManager.Mgr;

			var wPos = transform.position;
			wPos += new Vector3(0f, m_LE.GetHeight(), 0f);
			var sPos = cam.Camera.WorldToScreenPoint(wPos);

			string quirkName = m_CurrentQuirk != null ? m_CurrentQuirk.GetName() : "Null";
			var quirkContent = new GUIContent("Quirk: " + quirkName);
			var awerenessContent = new GUIContent("Awareness: " + m_AwarenessState.ToString());
			var awerenessTimerContent	= new GUIContent("Awrn Timer: " + (m_AwarenessTimer.GetTotalTime() - m_AwarenessTimer.GetRemainingTime()).ToString("F2") + '/' + m_AwarenessTimer.GetTotalTime().ToString("F2"));
			var aTimerContent			= new GUIContent("AI Timer A: " + (m_Timers[0].GetTotalTime() - m_Timers[0].GetRemainingTime()).ToString("F2") + '/' + m_Timers[0].GetTotalTime().ToString("F2"));
			var bTimerContent			= new GUIContent("AI Timer B: " + (m_Timers[1].GetTotalTime() - m_Timers[1].GetRemainingTime()).ToString("F2") + '/' + m_Timers[1].GetTotalTime().ToString("F2"));

			var quirkSize = GUI.skin.label.CalcSize(quirkContent);
			var awerenessSize = GUI.skin.label.CalcSize(awerenessContent);
			var awerenessTimerSize = GUI.skin.label.CalcSize(awerenessTimerContent);
			var aTimerSize = GUI.skin.label.CalcSize(aTimerContent);
			var bTimerSize = GUI.skin.label.CalcSize(bTimerContent);

			GUIHeight = quirkSize.y + awerenessSize.y + awerenessTimerSize.y + aTimerSize.y + bTimerSize.y;

			var heightOffset = m_LE.GUIHeight
				+ (m_ME != null ? m_ME.GUIHeight : 0f)
				+ (m_SpellCaster != null ? m_SpellCaster.GUIHeight : 0f);

			var sPoint = new Vector2(sPos.x, Screen.height - sPos.y);
			var gPoint = GUIUtility.ScreenToGUIPoint(sPoint);

			var quirkRect = new Rect(gPoint.x, gPoint.y - (heightOffset + GUIHeight), quirkSize.x, quirkSize.y);
			var awerenessRect = new Rect(gPoint.x, quirkRect.y + quirkSize.y, awerenessSize.x, awerenessSize.y);
			var awerenessTimerRect = new Rect(gPoint.x, awerenessRect.y + awerenessSize.y, awerenessTimerSize.x, awerenessTimerSize.y);
			var aTimerRect = new Rect(gPoint.x, awerenessTimerRect.y + awerenessTimerSize.y, aTimerSize.x, aTimerSize.y);
			var bTimerRect = new Rect(gPoint.x, aTimerRect.y + aTimerSize.y, bTimerSize.x, bTimerSize.y);

			GUI.contentColor = Color.white;
			GUI.Label(quirkRect, quirkContent);
			GUI.Label(awerenessRect, awerenessContent);
			GUI.Label(awerenessTimerRect, awerenessTimerContent);
			GUI.Label(aTimerRect, aTimerContent);
			GUI.Label(bTimerRect, bTimerContent);

			GUI.contentColor = oldColor;
		}
	}
}
