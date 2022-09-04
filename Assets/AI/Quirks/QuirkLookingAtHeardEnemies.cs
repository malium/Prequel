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
	public class QuirkLookingAtHeardEnemies : IQuirk
	{
		ConfigFloat SightDirectionDelay;
		Timer m_Timer;
		public QuirkLookingAtHeardEnemies()
			: base("LookingHeardEnemies")
		{
			SightDirectionDelay = new ConfigFloat("SightDirectionDelay", 0f);
			m_Configuration.Add(SightDirectionDelay);
			m_Timer = new Timer(true, true);
			m_Timer.OnTimerTrigger += OnTimer;
		}
		public override void OnFirstTrigger()
		{
			m_Monster.SetTargetPostion(m_Monster.transform.position);
			var delay = SightDirectionDelay.GetValue();
			if (delay > 0f)
			{
				m_Timer.Reset(delay);
			}
			FindTarget();
		}
		public override void OnTransitioning(IQuirk nextQuirk)
		{
			m_Timer.Stop();
		}
		void FindTarget()
		{
			var heardEnemies = m_Monster.GetHeardEnemies();
			float minDistance = float.MaxValue;
			CLivingEntity minDistanceEnemy = null;
			foreach (var enemy in heardEnemies)
			{
				var distance = Vector3.Distance(m_Monster.transform.position, enemy.transform.position);
				if (distance < minDistance)
				{
					minDistance = distance;
					minDistanceEnemy = enemy;
				}
			}
			if (minDistanceEnemy != null)
			{
				m_Monster.SetTargetEntityPosition(minDistanceEnemy.transform.position);
			}
		}
		void FaceTarget()
		{
			if (!m_Monster.IsTargetEntityPositionInvalid())
			{
				var dir = (m_Monster.GetTargetEntityPosition() - m_Monster.transform.position);
				var dirXZ = new Vector2(dir.x, dir.z).normalized;
				m_Monster.GetME().SetDirection(dirXZ);
				m_Monster.GetME().SetSightDirection(dirXZ);
			}
		}
		void OnTimer()
		{
			FaceTarget();
			FindTarget();
		}
		public override void UpdateQuirk()
		{
			var delay = SightDirectionDelay.GetValue();
			if (delay > 0f)
			{
				m_Timer.Update();
			}
			else
			{
				OnTimer();
			}
		}
	}
}