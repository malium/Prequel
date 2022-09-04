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
//	public class HunterQuirk : IQuirk
//	{
//		public new static IConfig[] DefaultConfig = new IConfig[]
//		{
//			CreateConfig("TargetOutOfRangeQuirk", ""),
//			CreateConfig("TargetOnRangeQuirk", ""),
//			CreateConfig("ApproachPct", 0.1f),
//		};

//		ConfigString TargetOutOfRangeQuirk;
//		ConfigString TargetOnRangeQuirk;
//		ConfigFloat ApproachPct;
//		ConfigString TargetName;

//		CLivingEntity TargetEnemy;
//		//float m_NextDistanceCheck;

//		public HunterQuirk()
//			:base("HunterQuirk")
//		{
//			TargetEnemy = null;
//			TargetOutOfRangeQuirk = new ConfigString(DefaultConfig[0]);
//			TargetOnRangeQuirk = new ConfigString(DefaultConfig[1]);
//			ApproachPct = new ConfigFloat(DefaultConfig[2]);
//			TargetName = new ConfigString("TargetName", "");

//			m_Configuration = new List<IConfig>()
//			{
//				TargetOutOfRangeQuirk,
//				TargetOnRangeQuirk,
//				ApproachPct,
//				TargetName,
//			};
//		}

//		public override void Death()
//		{
//			SetEnabled(false);
//		}
//		public override void OnDisabled()
//		{
//			if (TargetEnemy != null)
//			{
//				TargetEnemy.OnEntityDeath -= OnTargetDeath;
//			}
//			TargetEnemy = null;
//			TargetName.SetValue("");
//			//m_NextDistanceCheck = 0f;
//		}
//		public override void OnEnabled()
//		{
//			if (TargetEnemy == null && TargetName.GetValue().Length > 0)
//			{
//				var go = GameObject.Find(TargetName.GetValue());
//				if (go != null)
//					go.TryGetComponent(out TargetEnemy);
//				if (TargetEnemy != null)
//					TargetEnemy.OnEntityDeath += OnTargetDeath;
//			}
//		}
//		public override void NearEnemies(List<CLivingEntity> enemies)
//		{
//			//if (TargetEnemy == null)
//			//	return;

//			//if(!enemies.Contains(TargetEnemy)) // Out of hear range
//			//{
//			//	if (TargetOutOfRangeQuirk.GetValue().Length > 0)
//			//	{
//			//		var quirks = m_Monster.GetCurrentQuirks();
//			//		for (int i = 0; i < quirks.Count; ++i)
//			//		{
//			//			var quirk = quirks[i];
//			//			if (quirk.GetName() == TargetOutOfRangeQuirk.GetValue())
//			//			{
//			//				quirk.SetEnabled(true);
//			//				break;
//			//			}
//			//		}
//			//	}
//			//	SetEnabled(false);
//			//}

//			//if (Time.time < m_NextDistanceCheck)
//			//	return;

//			//var distance = Vector3.Distance(TargetEnemy.transform.position, m_Monster.transform.position);
//			//var spell = m_Monster.GetMonster().GetSpell(Def.MonsterSpellSlots.AUTO);
//			//if(distance > spell.GetMaxRange()) // Needs to approach
//			//{
//			//	float spellRange = spell.GetMaxRange() - spell.GetMinRange();
//			//	float pct = spellRange * ApproachPct.GetValue();
//			//	var nDistance = spell.GetMaxRange() - pct;
//			//	var dir = (TargetEnemy.transform.position - m_Monster.transform.position).normalized;
//			//	var nPositon = m_Monster.transform.position + dir * nDistance;
//			//	//Debug.LogWarning("SetTarget not done! !HunterQuirk");
//			//	m_Monster.SetTargetPostion(nPositon);
//			//	var time = nDistance / (m_Monster.GetME().GetMaxSpeed() * 0.75f);
//			//	//m_Monster.GetME().SetTarget(nPositon, dir);
//			//	//m_Monster.GetME().UpdateTarget();
//			//	m_NextDistanceCheck = Time.time + time;
//			//}
//			//else if(distance < spell.GetMinRange()) // Needs to back off
//			//{
//			//	float spellRange = spell.GetMaxRange() - spell.GetMinRange();
//			//	float pct = spellRange * ApproachPct.GetValue();
//			//	var nDistance = spell.GetMinRange() + pct;
//			//	var dir = (TargetEnemy.transform.position - m_Monster.transform.position).normalized;
//			//	var nPosition = TargetEnemy.transform.position + dir * -nDistance;
//			//	Debug.LogWarning("SetTarget not done! !HunterQuirk");
//			//	m_Monster.SetTargetPostion(nPosition);
//			//	var time = nDistance / (m_Monster.GetME().GetMaxSpeed() * 0.75f);
//			//	//m_Monster.GetME().SetTarget(nPosition, dir);
//			//	//m_Monster.GetME().UpdateTarget();
//			//	m_NextDistanceCheck = Time.time + time;
//			//}
//			//else // In range
//			//{
//			//	if(TargetOnRangeQuirk.GetValue().Length > 0)
//			//	{
//			//		var quirks = m_Monster.GetCurrentQuirks();
//			//		for(int i = 0; i < quirks.Count; ++i)
//			//		{
//			//			var quirk = quirks[i];
//			//			if(quirk.GetName() == TargetOnRangeQuirk.GetValue())
//			//			{
//			//				quirk.SetEnabled(true);
//			//				break;
//			//			}
//			//		}
//			//	}
//			//	SetEnabled(false);
//			//}
//		}
//		void OnTargetDeath(CLivingEntity entity)
//		{
//			SetEnabled(false);
//		}
//	}
//}
