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
	public class QuirkStareAtTarget : IQuirk
	{
		Timer m_RayDelay;
		CLivingEntity m_Target;
		public QuirkStareAtTarget()
			: base("StareAtTarget")
		{
			m_RayDelay = new Timer(true, true);
			m_RayDelay.OnTimerTrigger += TestTarget;
		}
		void TestTarget()
		{
			var target = m_Monster.GetTargetEntity();
			if (target == null)
				return;

			if (!GameUtils.RayTestEntityToEntity(m_Monster.GetLE(), target, m_Monster.GetMonster().GetFamily().Info.SightRange))
			{
				m_Monster.SetTargetEntity(null);
			}
			else
			{
				m_Monster.SetTargetEntityPosition(target.transform.position);
			}
		}
		public override void OnFirstTrigger()
		{
			m_Monster.SetTargetPostion(m_Monster.transform.position);
			m_RayDelay.Reset(0.5f);
			m_Target = m_Monster.GetTargetEntity();
			TestTarget();
		}
		public override void UpdateQuirk()
		{
			var target = m_Monster.GetTargetEntity();
			if (target == null)
			{
				if (m_Target == null)
					return;

				var seen = m_Monster.GetSeenEnemies();
				if (seen.Contains(m_Target))
				{
					if (!GameUtils.RayTestEntityToEntity(m_Monster.GetLE(), m_Target, m_Monster.GetMonster().GetFamily().Info.SightRange))
						return;
					m_Monster.SetTargetEntity(m_Target);
					target = m_Target;
				}
				else
				{
					return;
				}
			}

			var dir = (target.transform.position - m_Monster.transform.position).normalized;
			var dirXZ = new Vector2(dir.x, dir.z).normalized;
			m_Monster.GetME().SetDirection(dirXZ);
			m_Monster.GetME().SetSightDirection(dirXZ);
		}
		public override void OnTransitioning(IQuirk nextQuirk)
		{
			m_RayDelay.Stop();
		}
	}
}