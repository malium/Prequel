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
	public class QuirkStatic : IQuirk
	{
		ConfigBoolean m_UpdatePositionAtExit;
		public QuirkStatic()
			: base("StaticQuirk")
		{
			m_UpdatePositionAtExit = new ConfigBoolean("UpdatePositionAtExit", false);
			m_Configuration.Add(m_UpdatePositionAtExit);
		}
		public override void OnFirstTrigger()
		{
			m_Monster.SetTargetPostion(m_Monster.transform.position);
		}
		public override void UpdateQuirk()
		{

		}
		public override void OnTransitioning(IQuirk nextQuirk)
		{
			if (m_UpdatePositionAtExit.GetValue())
			{
				var target = m_Monster.GetTargetEntity();
				if (target != null)
				{
					m_Monster.SetTargetPostion(target.transform.position);
					m_Monster.SetTargetEntityPosition(target.transform.position);
					var dir = (target.transform.position - m_Monster.transform.position);
					var dirXZ = new Vector2(dir.x, dir.z).normalized;
					m_Monster.GetME().SetDirection(dirXZ);
					m_Monster.GetME().SetSightDirection(dirXZ);
				}
			}
		}
	}
}