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

namespace Assets.AI.Spells
{
	public class NullSpell : ISpell
	{
		public NullSpell()
			:base("NullSpell")
		{
			m_MinCastRange.SetValue(0f);
			m_MaxCastRange.SetValue(100f);
			m_CastTime.SetValue(0f);
			m_ReleaseTime.SetValue(0f);
			m_CooldownTime.SetValue(1f);
		}
		protected override void PerformAttack(CLivingEntity entity, Vector3 pos)
		{

		}
		protected override void PerformCast(CLivingEntity entity, Vector3 pos)
		{

		}
	}
}
