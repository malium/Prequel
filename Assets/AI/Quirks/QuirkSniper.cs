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
	public class QuirkSniper : IQuirk
	{
		ConfigEnum<Def.MonsterSpellSlots> SpellSlot;

		public QuirkSniper()
			: base("Sniper")
		{
			SpellSlot = new ConfigEnum<Def.MonsterSpellSlots>("Spell", Def.MonsterSpellSlots.AUTO);

			m_Configuration.Add(SpellSlot);
		}
		public override void OnFirstTrigger()
		{
			var targetEntity = m_Monster.GetTargetEntity();
			if (targetEntity == null)
			{
				Debug.LogWarning("Trying to fire a spell onto null targetEntity !SniperQuirk");
				return;
			}
			var slot = SpellSlot.GetValue();
			if (slot == Def.MonsterSpellSlots.COUNT)
			{
				Debug.LogWarning("Trying to fire an invalid slot spell !SniperQuirk");
				return;
			}
			var spell = m_Monster.GetSpellCaster().GetSpell((int)slot);
			if (spell == null || !spell.CanAttack())
				return;

			spell.Attack(targetEntity);
		}
		public override void UpdateQuirk()
		{

		}
	}
}