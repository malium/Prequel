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
	public class QuirkAlertOnFirstSight : IQuirk
	{
		static List<CLivingEntity> m_SeenEnemies;
		ConfigEnum<Def.MonsterTimers> WantedTimer;
		public QuirkAlertOnFirstSight()
			: base("AlertOnFirstSight")
		{
			if (m_SeenEnemies == null)
				m_SeenEnemies = new List<CLivingEntity>();

			WantedTimer = new ConfigEnum<Def.MonsterTimers>("Not Seen Timer", Def.MonsterTimers.TimerA);
			m_Configuration.Add(WantedTimer);
		}
		public override void OnFirstTrigger()
		{
			var seenEnemies = m_Monster.GetSeenEnemies();

			m_SeenEnemies.Clear();
			m_SeenEnemies.AddRange(seenEnemies);
			bool enemySeen = false;
			for (int i = 0; i < seenEnemies.Count; ++i)
			{
				float minDistance = float.MaxValue;
				int minDistanceEnemyIdx = -1;
				int idx = 0;
				foreach (var enemy in m_SeenEnemies)
				{
					var distance = Vector3.Distance(m_Monster.transform.position, enemy.transform.position);
					if (distance < minDistance)
					{
						minDistance = distance;
						minDistanceEnemyIdx = idx;
					}
					++idx;
				}
				if (idx >= 0)
				{
					var enemy = m_SeenEnemies[minDistanceEnemyIdx];
					m_SeenEnemies.RemoveAt(minDistanceEnemyIdx);
					if (GameUtils.RayTestEntityToEntity(m_Monster.GetLE(), enemy, m_Monster.GetMonster().GetFamily().Info.SightRange))
					{
						var dir = (enemy.transform.position - m_Monster.transform.position);
						var dirXZ = new Vector2(dir.x, dir.z).normalized;
						m_Monster.GetME().SetDirection(dirXZ);
						m_Monster.GetME().SetSightDirection(dirXZ);
						m_Monster.SetTargetEntity(enemy);
						m_Monster.SetTargetEntityPosition(enemy.transform.position);
						enemySeen = true;
						break;
					}
				}
			}
			if (!enemySeen)
			{
				m_Monster.GetMonsterTimer(WantedTimer.GetValue()).Reset(1f);
			}
		}
		public override void UpdateQuirk()
		{

		}
	}
}