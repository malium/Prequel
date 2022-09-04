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
	public class Item : IXmlSerializable
	{
		string m_Name;
		int m_ID;
		string m_ImageName;
		Def.ItemCategory m_Category;
		Def.ItemSubcategory m_Subcategory;
		string m_Description;
		bool m_ActiveItem;
		Def.ItemConsumableType m_ConsumableType;
		List<IItemEffect> m_Effects;

		public Item()
		{
			m_Name = "";
			m_ID = -1;
			m_Description = "";
			m_ImageName = "";
			m_Effects = new List<IItemEffect>();
			m_Category = Def.ItemCategory.COUNT;
		}
		public Item Duplicate()
		{
			var ret = new Item()
			{
				m_Name = m_Name,
				m_ID = m_ID,
				m_ImageName = m_ImageName,
				m_Category = m_Category,
				m_Subcategory = m_Subcategory,
				m_Description = m_Description,
				m_ActiveItem = m_ActiveItem,
				m_ConsumableType = m_ConsumableType,
				m_Effects = new List<IItemEffect>(m_Effects.Count)
			};
			for(int i = 0; i < m_Effects.Count; ++i)
			{
				var effect = m_Effects[i];
				var nEffect = ItemLoader.CreateItemEffect(effect.GetItemEffectType());
				effect.OnDuplicate(nEffect);
				nEffect.SetItem(ret);
				ret.GetItemEffects().Add(nEffect);
			}
			return ret;
		}
		public Def.ItemCategory GetItemCategory() => m_Category;
		public void SetCategory(Def.ItemCategory category) => m_Category = category;
		public Def.ItemSubcategory GetItemSubcategory() => m_Subcategory;
		public void SetSubcategory(Def.ItemSubcategory subcategory) => m_Subcategory = subcategory;
		public string GetName() => m_Name;
		public void SetName(string name) => m_Name = name;
		public int GetID() => m_ID;
		public void SetID(int id) => m_ID = id;
		public string GetImageName() => m_ImageName;
		public void SetImageName(string name) => m_ImageName = name;
		public string GetDescription() => m_Description;
		public void SetDescription(string description) => m_Description = description;
		public List<IItemEffect> GetItemEffects() => m_Effects;
		public bool IsActiveItem() => m_ActiveItem;
		public void SetIsActive(bool active) => m_ActiveItem = active;
		public Def.ItemConsumableType GetConsumableType() => m_ConsumableType;
		public void SetConsumableType(Def.ItemConsumableType consumableType) => m_ConsumableType = consumableType;
		public void Activate(CSpellCaster sc)
		{
			if (!m_ActiveItem)
				return;
			for(int i = 0; i < m_Effects.Count; ++i)
			{
				var effect = m_Effects[i];
				if(effect.IsActivable() && effect.IsActiveReady())
				{
					effect.OnActivate(sc);
				}
			}
		}
		public void OnAddedToInventory(CSpellCaster sc)
		{
			for (int i = 0; i < m_Effects.Count; ++i)
			{
				m_Effects[i].OnAddToInventory(sc);
			}
		}
		public void OnRemoveFromInventory(CSpellCaster sc)
		{
			for (int i = 0; i < m_Effects.Count; ++i)
			{
				m_Effects[i].OnRemoveFromInventory(sc);
			}
		}
		public void OnEquip(CSpellCaster sc)
		{
			for (int i = 0; i < m_Effects.Count; ++i)
			{
				m_Effects[i].OnEquip(sc);
				//var effect = m_Effects[i];
				//effect.OnEquip(sc);
			}
		}
		public void OnUnequip(CSpellCaster sc)
		{
			for (int i = 0; i < m_Effects.Count; ++i)
			{
				m_Effects[i].OnUnequip(sc);
				//var effect = m_Effects[i];
				//effect.OnEquip(sc);
			}
		}
		public void OnUpdate(CSpellCaster sc)
		{
			for (int i = 0; i < m_Effects.Count; ++i)
			{
				m_Effects[i].OnUpdate(sc);
				//var effect = m_Effects[i];
				//effect.OnEquip(sc);
			}
		}
		public XmlSchema GetSchema() => null;
		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement("Item");
			{
				GameUtils.ReadXMLString(reader, "m_Name", out m_Name);

				GameUtils.ReadXMLInt(reader, "m_ID", out m_ID);

				GameUtils.ReadXMLString(reader, "m_ImageName", out m_ImageName);

				GameUtils.ReadXMLEnum(reader, "m_Category", out m_Category);

				GameUtils.ReadXMLEnum(reader, "m_Subcategory", out m_Subcategory);

				GameUtils.ReadXMLString(reader, "m_Description", out m_Description);

				GameUtils.ReadXMLBool(reader, "m_ActiveItem", out m_ActiveItem);

				reader.ReadStartElement("m_Effects");
				{
					GameUtils.ReadXMLInt(reader, "Count", out int count);

					m_Effects = new List<IItemEffect>(count);
					for(int i = 0; i < count; ++i)
					{
						reader.ReadStartElement("effect_" + i.ToString());
						{
							GameUtils.ReadXMLString(reader, "m_EffectName", out string effectName);

							var itemEffect = ItemLoader.CreateItemEffect(effectName);
							if (itemEffect != null)
							{
								itemEffect.ReadXml(reader);
								itemEffect.SetItem(this);
								m_Effects.Add(itemEffect);
							}
						}
						reader.ReadEndElement();
					}
					reader.ReadEndElement();
				}
				reader.ReadEndElement();
			}
			//reader.ReadEndElement();
		}
		public void WriteXml(XmlWriter writer)
		{
			GameUtils.WriteXMLString(writer, "m_Name", m_Name);

			GameUtils.WriteXMLString(writer, "m_ID", m_ID.ToString());

			GameUtils.WriteXMLString(writer, "m_ImageName", m_ImageName);

			GameUtils.WriteXMLString(writer, "m_Category", m_Category.ToString());

			GameUtils.WriteXMLString(writer, "m_Subcategory", m_Subcategory.ToString());

			GameUtils.WriteXMLString(writer, "m_Description", m_Description);

			GameUtils.WriteXMLString(writer, "m_ActiveItem", m_ActiveItem.ToString());

			writer.WriteStartElement("m_Effects");
			{
				GameUtils.WriteXMLString(writer, "Count", m_Effects.Count.ToString());

				for (int i = 0; i < m_Effects.Count; ++i)
				{
					var effect = m_Effects[i];

					writer.WriteStartElement("effect_" + i.ToString());
					{
						GameUtils.WriteXMLString(writer, "m_EffectName", effect.GetItemEffectType().ToString());
						effect.WriteXml(writer);
					}
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
		}
	}
}
