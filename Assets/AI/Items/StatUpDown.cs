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

namespace Assets.AI.Items
{
	public class StatUpDown : IItemEffect
	{
		ConfigEnum<Def.LEStats> m_Stat;
		ConfigFloat m_Amount;
		ConfigBoolean m_AddExtra;
		public override Def.ItemEffectType GetItemEffectType() => Def.ItemEffectType.StatUpDown;
		public StatUpDown()
		{
			m_IsActivable = true;
			m_Stat = new ConfigEnum<Def.LEStats>("Stat", Def.LEStats.Health);
			m_Amount = new ConfigFloat("Amount", 0f);
			m_AddExtra = new ConfigBoolean("Add Extra", false);

			m_Config.Add(m_Stat);
			m_Config.Add(m_Amount);
			m_Config.Add(m_AddExtra);
		}
		public override bool IsActiveReady() => true;
		public override void OnActivate(CSpellCaster sc)
		{
			base.OnActivate(sc);

			var stat = m_Stat.GetValue();
			var amount = m_Amount.GetValue();
			var addExtra = m_AddExtra.GetValue();

			switch (stat)
			{
				case Def.LEStats.Health:
					if(amount > 0f)
					{
						var oHealth = sc.GetLE().GetCurrentHealth();
						sc.GetLE().ReceiveHealing(sc.GetLE(), amount);
						var fHealth = sc.GetLE().GetCurrentHealth();
						var diff = fHealth - oHealth;
						if(diff < amount && addExtra)
						{
							// TODO: AddExtra
						}
					}
					else if(amount < 0f)
					{
						sc.GetLE().ReceiveDamage(sc.GetLE(), Def.DamageType.UNAVOIDABLE, amount);
					}
					break;
				case Def.LEStats.Soulness:
					if (amount > 0f)
					{
						var oSoulness = sc.GetLE().GetCurrentSoulness();
						sc.GetLE().ReceiveSoulness(sc.GetLE(), amount);
						var fSoulness = sc.GetLE().GetCurrentSoulness();
						var diff = fSoulness - oSoulness;
						if (diff < amount && addExtra)
						{
							// TODO: AddExtra
						}
					}
					else if (amount < 0f)
					{
						sc.GetLE().ReduceSoulness(sc.GetLE(), amount);
					}
					break;
				default:
					Debug.LogWarning("Trying to Activate StatUpDown, but the selected LEStat is not handled " + stat.ToString());
					break;
			}
		}
	}
}
