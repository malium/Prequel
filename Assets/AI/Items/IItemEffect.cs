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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Assets.AI.Items
{
	public abstract class IItemEffect
	{
		protected List<IConfig> m_Config;
		protected string m_DisplayString;
		protected bool m_IsActivable;
		protected Item m_Item;

		public IItemEffect()
		{
			m_Config = new List<IConfig>();
		}
		public void SetItem(Item item) => m_Item = item;
		public void SetConfig(IConfig config)
		{
			for (int i = 0; i < m_Config.Count; ++i)
			{
				var conf = m_Config[i];
				if (conf.GetConfigName() != config.GetConfigName())
					continue;
				conf.FromString(config.GetValueString());
			}
		}
		public List<IConfig> GetConfig() => m_Config;
		public abstract Def.ItemEffectType GetItemEffectType();
		public string GetDisplayString() => m_DisplayString;
		public void SetDisplayString(string str) => m_DisplayString = str;
		public bool IsActivable() => m_IsActivable;
		public void SetIsActivable(bool activable) => m_IsActivable = activable;
		public virtual void OnAddToInventory(CSpellCaster sc)
		{

		}
		public virtual void OnRemoveFromInventory(CSpellCaster sc)
		{

		}
		public virtual void OnDuplicate(IItemEffect nEffect)
		{
			for (int i = 0; i < m_Config.Count; ++i)
				nEffect.SetConfig(m_Config[i]);
		}
		public virtual bool IsActiveReady() => false;
		public virtual void OnEquip(CSpellCaster sc)
		{

		}
		public virtual void OnUnequip(CSpellCaster sc)
		{

		}
		public virtual void OnUpdate(CSpellCaster sc)
		{

		}
		public virtual void OnActivate(CSpellCaster sc)
		{

		}
		public virtual void WriteXml(XmlWriter writer)
		{
			for(int i = 0; i < m_Config.Count; ++i)
			{
				var cfg = m_Config[i];
				var name = cfg.GetConfigName().Replace(' ', '_');
				GameUtils.WriteXMLString(writer, name, cfg.GetValueString());
			}
		}
		public virtual void ReadXml(XmlReader reader)
		{
			for(int i = 0; i < m_Config.Count; ++i)
			{
				var cfg = m_Config[i];
				var name = cfg.GetConfigName().Replace(' ', '_');
				if (GameUtils.ReadXMLString(reader, name, out string value))
					cfg.FromString(value);
			}
		}
	}
}