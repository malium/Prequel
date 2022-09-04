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
	public class Consumable : IItemEffect
	{
		ConfigEnum<Def.ItemConsumableType> m_ConsumableType;
		public override Def.ItemEffectType GetItemEffectType() => Def.ItemEffectType.Consumable;
		public Consumable()
		{
			m_IsActivable = true;
			m_ConsumableType = new ConfigEnum<Def.ItemConsumableType>("Type", Def.ItemConsumableType.None);
			m_Config.Add(m_ConsumableType);
		}
		public override bool IsActiveReady() => true;
		public override void OnAddToInventory(CSpellCaster sc)
		{
			base.OnAddToInventory(sc);

			m_Item.SetConsumableType(m_ConsumableType.GetValue());
		}
		public override void OnActivate(CSpellCaster sc)
		{
			base.OnActivate(sc);

			sc.RemoveItemFromInventory(m_Item);
		}
	}
}
