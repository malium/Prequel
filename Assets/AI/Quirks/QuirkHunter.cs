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
	public class QuirkHunter : IQuirk
	{
		ConfigEnum<Def.MonsterTimers> WantedTimer;
		public QuirkHunter()
			: base("Hunter")
		{
			WantedTimer = new ConfigEnum<Def.MonsterTimers>("Wanted Timer", Def.MonsterTimers.TimerA);
			m_Configuration.Add(WantedTimer);
		}
		public override void OnFirstTrigger()
		{
			if (!m_Monster.IsTargetEntityPositionInvalid())
			{
				m_Monster.SetTargetPostion(m_Monster.GetTargetEntityPosition());
				m_Monster.GetMonsterTimer(WantedTimer.GetValue()).Reset(2f);
			}
		}
		public override void UpdateQuirk()
		{
			if (m_Monster.IsTargetReached())
				m_Monster.GetMonsterTimer(WantedTimer.GetValue()).Stop();
		}
		public override void OnTransitioning(IQuirk nextQuirk)
		{
			m_Monster.GetMonsterTimer(WantedTimer.GetValue()).Stop();
		}
	}
}