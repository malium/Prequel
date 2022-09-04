/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets.AI.Quirks
//{
//	public class SniperQuirk : IQuirk
//	{
//		public new static IConfig[] DefaultConfig = new IConfig[]
//		{
//			CreateConfig("WarningTime", 3f),
//			CreateConfig("AlertTime", 30f),
//			CreateConfig("TargetOutOfRangeQuirk", ""),
//		};

//		ConfigFloat WarningTime;
//		ConfigFloat AlertTime;
//		ConfigString OutOfRangeQuirkName;

//		//bool HasAttacked;
//		CLivingEntity TargetEnemy;

//		public SniperQuirk()
//			:base("SniperQuirk")
//		{
//			TargetEnemy = null;
//			WarningTime = new ConfigFloat(DefaultConfig[0]);
//			AlertTime = new ConfigFloat(DefaultConfig[1]);
//			OutOfRangeQuirkName = new ConfigString(DefaultConfig[2]);
//			//HasAttacked = false;
//			m_Configuration = new List<IConfig>()
//			{
//				WarningTime,
//				AlertTime,
//				OutOfRangeQuirkName,
//			};
			
//		}

//		public override void Death()
//		{
//			if(TargetEnemy != null)
//			{
//				TargetEnemy.OnEntityDeath -= OnTargetDeath;
//			}
//			TargetEnemy = null;
//		}
//		void FaceEnemy()
//		{
//			if (TargetEnemy != null)
//			{
//				Vector2 pos = new Vector2(TargetEnemy.transform.position.x, TargetEnemy.transform.position.z);
//				var dir = (pos - new Vector2(m_Monster.transform.position.x, m_Monster.transform.position.z)).normalized;
//				m_Monster.GetME().SetDirection(dir);
//			}
//		}
//		bool RayTest(CLivingEntity target)
//		{
//			var casterHead = m_Monster.transform.position + new Vector3(0f, m_Monster.GetLE().GetHeight(), 0f);
//			var targetMid = target.transform.position + new Vector3(0f, target.GetHeight() * 0.5f, 0f);
//			var targetHead = target.transform.position + new Vector3(0f, target.GetHeight() * 0.9f, 0f);
//			var headMidDir = (targetMid - casterHead).normalized;
//			var headHeadDir = (targetHead - casterHead).normalized;
//			var headRay = new Ray(casterHead, headHeadDir);
//			var midRay = new Ray(casterHead, headMidDir);
//			m_Monster.GetLE().GetCollider().enabled = false;
//			bool headTest = Physics.Raycast(headRay, out RaycastHit headHit);
//			if(headTest && headHit.collider.gameObject.layer == Def.RCLayerLE)
//			{
//				Debug.DrawRay(headRay.origin, headRay.direction, Color.red);
//				var headLE = headHit.collider.gameObject.GetComponent<CLivingEntity>();
//				if (headLE == target)
//				{
//					Debug.Log("Head Hit!");
//					m_Monster.GetLE().GetCollider().enabled = true;
//					Debug.Break();
//					return true;
//				}
//			}
//			bool midTest = Physics.Raycast(midRay, out RaycastHit midHit);
//			if(midTest && midHit.collider.gameObject.layer == Def.RCLayerLE)
//			{
//				Debug.DrawRay(midRay.origin, midRay.direction, Color.green);
//				var midLE = midHit.collider.gameObject.GetComponent<CLivingEntity>();
//				if (midLE == target)
//				{
//					Debug.Log("Mid Hit!");
//					m_Monster.GetLE().GetCollider().enabled = true;
//					Debug.Break();
//					return true;
//				}
//			}

//			m_Monster.GetLE().GetCollider().enabled = true;
//			return false;
//		}
//		public override void NearEnemies(List<CLivingEntity> enemies)
//		{
//			//switch (m_Monster.GetAwarenessState())
//			//{
//			//	case Def.MonsterAwarenessState.CALM:
//			//		m_Monster.SetAwareness(Def.MonsterAwarenessState.WARN, WarningTime.GetValue());
//			//		HasAttacked = false;
//			//		TargetEnemy = null;
//			//		break;
//			//	case Def.MonsterAwarenessState.WARN:
//			//		{
//			//			TargetEnemy = null;
//			//			bool seen = false;
//			//			var sightRange = m_Monster.GetMonster().GetFamily().Info.SightRange;
//			//			var sightAngle = m_Monster.GetMonster().GetFamily().Info.SightAngle * 0.5f;
//			//			for (int i = 0; i < enemies.Count; ++i)
//			//			{
//			//				var cur = enemies[i];
//			//				if (Vector3.Distance(cur.transform.position, m_Monster.transform.position) > sightRange)
//			//					continue;
//			//				var enemyPosXZ = new Vector2(cur.transform.position.x, cur.transform.position.z);
//			//				var curPosXZ = new Vector2(m_Monster.transform.position.x, m_Monster.transform.position.z);
//			//				var enemyDir = (enemyPosXZ - curPosXZ).normalized;

//			//				var curAngle = Mathf.Abs(GameUtils.AngleBetween2D(m_Monster.GetME().GetDirection(), enemyDir)) * Mathf.Rad2Deg;
//			//				if (curAngle > sightAngle)
//			//					continue;

//			//				if (!RayTest(cur))
//			//				{
//			//					Debug.Log("Ray Fail!");
//			//					continue;
//			//				}

//			//				TargetEnemy = cur;
//			//				TargetEnemy.OnEntityDeath += OnTargetDeath;
//			//				seen = true;
//			//				break;
//			//			}
//			//			HasAttacked = false;
//			//			if (seen)
//			//			{
//			//				m_Monster.SetAwareness(Def.MonsterAwarenessState.ALERT, AlertTime.GetValue());
//			//				FaceEnemy();
//			//			}
//			//		}
//			//		break;
//			//	case Def.MonsterAwarenessState.ALERT:
//			//		{
//			//			if (TargetEnemy == null)
//			//			{
//			//				Debug.LogWarning("Seen enemy was null !SniperQuirk");
//			//				return;
//			//			}
//			//			var distance = Vector3.Distance(TargetEnemy.transform.position, m_Monster.transform.position);
//			//			var spell = m_Monster.GetMonster().GetSpell(Def.MonsterSpellSlots.AUTO);
//			//			if(distance <= spell.GetMaxRange() && distance >= spell.GetMinRange())
//			//			{
//			//				m_Monster.SetTargetPostion(m_Monster.transform.position);
//			//				FaceEnemy();
//			//				if (m_Monster.GetMonster().CanAutoAttack())
//			//				{
//			//					m_Monster.GetMonster().AutoAttack(TargetEnemy, TargetEnemy.transform.position);
//			//					HasAttacked = true;
//			//				}
//			//			}
//			//			else
//			//			{
//			//				if(HasAttacked)
//			//				{
//			//					if(OutOfRangeQuirkName.GetValue().Length > 0)
//			//					{
//			//						if (OutOfRangeQuirkName.GetValue().Length > 0)
//			//						{
//			//							IQuirk outOfRangeQuirk = null;
//			//							var quirks = m_Monster.GetCurrentQuirks();
//			//							for (int i = 0; i < quirks.Count; ++i)
//			//							{
//			//								var quirk = quirks[i];
//			//								if (quirk.GetName() == OutOfRangeQuirkName.GetValue())
//			//								{
//			//									outOfRangeQuirk = quirk;
//			//									break;
//			//								}
//			//							}
//			//							if(outOfRangeQuirk == null)
//			//							{
//			//								Debug.LogWarning("Couldn't find quirk " + OutOfRangeQuirkName.GetValue() + " !SniperQuirk.NearEnemies");
//			//								return;
//			//							}
//			//							outOfRangeQuirk.SetConfig(CreateConfig("TargetName", TargetEnemy.gameObject.name));
//			//							outOfRangeQuirk.SetConfig(CreateConfig("TargetOutOfRangeQuirk", m_Name));
//			//							outOfRangeQuirk.SetConfig(CreateConfig("TargetOnRangeQuirk", m_Name));
//			//							outOfRangeQuirk.SetEnabled(true);
//			//						}
//			//						SetEnabled(false);
//			//					}
//			//				}
//			//			}
//			//		}
//			//		break;
//			//}
			
//		}

//		void OnTargetDeath(CLivingEntity entity)
//		{
//			//TargetEnemy = null;
//			//m_Monster.SetAwareness(Def.MonsterAwarenessState.WARN, WarningTime.GetValue());
//		}
//	}
//}
