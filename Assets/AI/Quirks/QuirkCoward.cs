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
	public class QuirkCoward : IQuirk
	{
		ConfigEnum<Def.CowardEnemySelection> EnemySelection;
		ConfigFloat EnemiesDistance;
		ConfigFloat RunAwayTime;
		ConfigEnum<Def.MonsterTimers> WantedTimer;
		List<CLivingEntity> m_Enemies;
		public QuirkCoward()
			: base("CowardQuirk")
		{
			EnemySelection = new ConfigEnum<Def.CowardEnemySelection>("EnemySelection", Def.CowardEnemySelection.Hearing);
			EnemiesDistance = new ConfigFloat("Distance", 1f);
			RunAwayTime = new ConfigFloat("RunAwayTime", 2f);
			WantedTimer = new ConfigEnum<Def.MonsterTimers>("Wanted Timer", Def.MonsterTimers.TimerA);

			m_Configuration.Add(EnemySelection);
			m_Configuration.Add(EnemiesDistance);
			m_Configuration.Add(RunAwayTime);
			m_Configuration.Add(WantedTimer);

			m_Enemies = new List<CLivingEntity>();
		}
		public override void UpdateQuirk()
		{
			m_Enemies.Clear();
			switch (EnemySelection.GetValue())
			{
				case Def.CowardEnemySelection.Distance:
					{
						var info = m_Monster.GetMonster().GetFamily().Info;
						var distanceChecked = Mathf.Max(info.SightRange, info.HearingRange);
						var distanceToCheck = EnemiesDistance.GetValue();
						var nearStrucs = m_Monster._GetNearStrucs();
						if (distanceChecked < distanceToCheck)
						{
							nearStrucs.Clear();
							var world = World.World.gWorld;
							world.StrucsRangeWPos(new Vector2(m_Monster.transform.position.x, m_Monster.transform.position.z), distanceToCheck, ref nearStrucs);
							//var vwPos = GameUtils.TransformPosition(new Vector2(m_Monster.transform.position.x, m_Monster.transform.position.z));
							//var vPos = world.VPosFromVWPos(vwPos);
							//world.StrucsRangeVPos(vPos, Mathf.FloorToInt(distanceToCheck), ref nearStrucs);
							//var map = Map.GetCurrent();
							//map.GetStrucsRange(new Vector2(m_Monster.transform.position.x, m_Monster.transform.position.z), distanceToCheck, ref nearStrucs);
							m_Monster._SetNearStructs(nearStrucs, distanceToCheck);
						}
						for (int j = 0; j < nearStrucs.Count; ++j)
						{
							var sEntities = nearStrucs[j].GetLES();
							for (int k = 0; k < sEntities.Count; ++k)
							{
								var sEntity = sEntities[k];
								if (sEntity.GetLEType() == Def.LivingEntityType.Prop)
									continue;
								if (m_Monster.IsEnemy(sEntity))
								{
									m_Enemies.Add(sEntity);
								}
							}
						}
					}
					break;
				case Def.CowardEnemySelection.Hearing:
					m_Enemies.AddRange(m_Monster.GetHeardEnemies());
					break;
				case Def.CowardEnemySelection.Sight:
					m_Enemies.AddRange(m_Monster.GetSeenEnemies());
					break;
			}
			if (m_Enemies.Count == 0)
				return;

			m_Monster.GetMonsterTimer(WantedTimer.GetValue()).Reset(RunAwayTime.GetValue());
			var monsterXZ = new Vector2(m_Monster.transform.position.x, m_Monster.transform.position.z);
			var runAwayDir = Vector2.zero;
			for (int i = 0; i < m_Enemies.Count; ++i)
			{
				var enemy = m_Enemies[i];
				var enemyXZ = new Vector2(enemy.transform.position.x, enemy.transform.position.z);
				runAwayDir += (monsterXZ - enemyXZ).normalized;
			}
			runAwayDir = new Vector2(runAwayDir.x / m_Enemies.Count, runAwayDir.y / m_Enemies.Count);

			var targetPos = monsterXZ + runAwayDir * 10f;

			m_Monster.SetTargetPostion(new Vector3(targetPos.x, m_Monster.transform.position.y, targetPos.y));
		}
	}
}