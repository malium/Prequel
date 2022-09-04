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

namespace Assets.AI
{
	public class CSpellCaster : MonoBehaviour
	{
		public class ItemSlot
		{
			public Items.Item Item;
			public int Count;
		}
		[SerializeReference] CLivingEntity m_LE;
		[SerializeReference] CMovableEntity m_ME;
		[SerializeField] List<Spells.ISpell> m_Spells;
		List<GUIContent> m_SpellGUIContent;
		List<Vector2> m_SpellGUISize;
		[SerializeReference] List<ItemSlot> m_Inventory;
		Dictionary<int, int> m_InventoryDict;
		[SerializeReference] Items.Item[] m_EquipedItems;
		[SerializeReference] bool m_CanUseQuickItems;
		[SerializeField] bool m_IsAttacking;
		[SerializeField] int[] m_QuickItems;
		[SerializeField] Def.QuickItemSlot m_TriggeredQuickItemSlot;

		public float GUIHeight { get; private set; }
		public CLivingEntity GetLE() => m_LE;
		public CMovableEntity GetME() => m_ME;
		public List<ItemSlot> GetItems() => m_Inventory;
		public void Init(CLivingEntity le, CMovableEntity me, List<AttackInfo> spellInfoList)
		{
			m_LE = le;
			m_ME = me;
			m_IsAttacking = false;
			m_TriggeredQuickItemSlot = Def.QuickItemSlot.COUNT;
			m_CanUseQuickItems = true;
			if(m_Spells != null)
			{
				for(int i = 0; i < m_Spells.Count; ++i)
				{
					if (m_Spells[i] != null)
						m_Spells[i].DestroySpell();
				}
			}
			m_Spells = new List<Spells.ISpell>(Enumerable.Repeat<Spells.ISpell>(null, spellInfoList.Count));
			for(int i = 0; i < spellInfoList.Count; ++i)
			{
				var spellInfo = spellInfoList[i];
				if (spellInfo == null)
				{
					m_Spells[i] = Spells.SpellManager.CreateSpell("NullSpell");
					continue;
				}
				var spell = Spells.SpellManager.CreateSpell(spellInfo.AttackName);
				for (int j = 0; j < spellInfo.Configuration.Count; ++j)
				{
					var cur = spellInfo.Configuration[j];
					IConfig enumConf = null;
					if (cur.ConfigType == Def.ConfigType.ENUM)
					{
						for (int k = 0; k < spell.GetConfig().Count; ++k)
						{
							var config = spell.GetConfig()[k];
							if (config.GetConfigName() == cur.Name)
							{
								enumConf = config;
								break;
							}
						}
					}
					spell.SetConfig(cur.Create(enumConf));
				}
				for (int j = 0; j < spellInfo.OnHitConfiguration.Count; ++j)
				{
					var cur = spellInfo.OnHitConfiguration[j];
					spell.AddOnHit(cur);
				}
				spell.SetCaster(m_LE);
				spell.InitSpell();
				m_Spells[i] = spell;
			}

			m_Inventory = new List<ItemSlot>();
			m_InventoryDict = new Dictionary<int, int>();
			m_EquipedItems = new Items.Item[Def.ItemEquipSlotCount];
			m_QuickItems = new int[Def.QuickItemSlotCount];
			for (int i = 0; i < m_QuickItems.Length; ++i) m_QuickItems[i] = -1;
		}
		public Spells.ISpell GetSpell(int spellIdx)
		{
			if (m_Spells == null || spellIdx < 0 || spellIdx >= m_Spells.Count)
				return null;
			return m_Spells[spellIdx];
		}
		public bool IsSpellReady(int spellIdx)
		{
			var spell = GetSpell(spellIdx);
			if (spell == null || !spell.CanAttack())
				return false;
			return true;
		}
		public void UseSpell(int spellIdx, CLivingEntity target, Vector3 pos)
		{
			var spell = GetSpell(spellIdx);
			if (spell == null)
				return;
			spell.Attack(target, pos);
		}
		public bool AddItemToInventory(int itemID, int count = 1)
		{
			if (count <= 0 || !Items.ItemLoader.ItemIDDict.TryGetValue(itemID, out int itemIDX))
				return false; // invalid ID or invalid count value

			ItemSlot slot;
			if(m_InventoryDict.TryGetValue(itemID, out int inventoryIDX))
			{
				// A slot with this type of item was already in use, add this item
				slot = m_Inventory[inventoryIDX];
				for(int i = 0; i < count; ++i)
				{
					++slot.Count;
					slot.Item.OnAddedToInventory(this);
				}
				return true;
			}
			// Add a new slot
			slot = new ItemSlot
			{
				Item = Items.ItemLoader.Items[itemIDX],
				Count = 0
			};
			if (slot.Item == null)
				return false; // Item is not valid
			
			slot.Item = slot.Item.Duplicate(); // We don't want to modify the default one
			for (int i = 0; i < count; ++i)
			{
				++slot.Count;
				slot.Item.OnAddedToInventory(this);
			}
			m_InventoryDict.Add(itemID, m_Inventory.Count);
			m_Inventory.Add(slot);
			return true;

		}
		public bool RemoveItemFromInventory(int itemID, int count = 1)
		{
			if (count <= 0 || !m_InventoryDict.TryGetValue(itemID, out int inventoryIDX))
				return false; // ItemID is not in the inventory or invalid count value

			if (inventoryIDX < 0 || inventoryIDX > m_Inventory.Count)
				return false; // Something went wrong with indices

			var slot = m_Inventory[inventoryIDX];

			// Trigger OnRemoveFromInventory from each element
			var countToRemove = Mathf.Min(slot.Count, count);
			for (int i = 0; i < countToRemove; ++i)
			{
				slot.Item.OnRemoveFromInventory(this);
				--slot.Count;
			}

			// remove the whole slot
			if ((slot.Count - count) <= 0)
			{
				// Update dictionary
				for (int i = inventoryIDX + 1; i < m_Inventory.Count; ++i)
				{
					var oSlot = m_Inventory[i];
					m_InventoryDict[oSlot.Item.GetID()] = i - 1;
				}
				// Remove it from inventory totally
				m_Inventory.RemoveAt(inventoryIDX);
				m_InventoryDict.Remove(itemID);
			}
			return true;
		}
		public bool RemoveItemFromInventory(Items.Item item, int count = 1)
		{
			if (item == null || count <= 0)
				return false;

			return RemoveItemFromInventory(item.GetID(), count);
		}
		public bool CanUseQuickItems() => m_CanUseQuickItems;
		public void EnableUseQuickItems(bool enable) => m_CanUseQuickItems = enable;
		public ItemSlot GetItemSlot(int itemID)
		{
			if (m_InventoryDict.TryGetValue(itemID, out int slotIDX))
				return m_Inventory[slotIDX];
			return null;
		}
		public void UpdateSpells()
		{
			for(int i = 0; i < m_Spells.Count; ++i)
			{
				m_Spells[i]?.OnUpdate();
			}
		}
		public void UpdateItems()
		{
			for(int i = 0; i < m_EquipedItems.Length; ++i)
			{
				m_EquipedItems[i]?.OnUpdate(this);
			}
		}
		public void EquipItem(Items.Item item, Def.ItemEquipSlot slot)
		{
			var oldItem = m_EquipedItems[(int)slot];
			if (item == oldItem)
				return;

			if(oldItem != null)
			{
				oldItem.OnUnequip(this);
				AddItemToInventory(oldItem.GetID());
				m_EquipedItems[(int)slot] = null;
			}

			m_EquipedItems[(int)slot] = item;

			item?.OnEquip(this);
		}
		public Items.Item UnequipItem(Def.ItemEquipSlot slot)
		{
			var item = m_EquipedItems[(int)slot];
			if (item == null)
				return null;

			item.OnUnequip(this);

			m_EquipedItems[(int)slot] = null;

			return item;
		}
		public bool IsAttacking() => m_IsAttacking;
		public void SetIsAttacking(bool attacking) => m_IsAttacking = attacking;
		private void OnGUI()
		{
			if(m_LE == null || m_Spells == null || m_Spells.Count == 0 || m_LE.GUIHeight < 0f || !Manager.Mgr.DebugSpells)
			{
				GUIHeight = 0f;
				return;
			}

			var oldColor = GUI.contentColor;

			var cam = CameraManager.Mgr;

			var wPos = transform.position;
			wPos += new Vector3(0f, m_LE.GetHeight(), 0f);
			var sPos = cam.Camera.WorldToScreenPoint(wPos);

			var heightOffset = m_LE.GUIHeight + (m_ME != null ? m_ME.GUIHeight : 0f);

			if (m_SpellGUIContent == null)
			{
				m_SpellGUIContent = new List<GUIContent>(m_Spells.Count);
				m_SpellGUISize = new List<Vector2>(m_Spells.Count);
			}
			m_SpellGUIContent.Clear();
			m_SpellGUISize.Clear();

			// S0 NAME STATE
			GUIHeight = 0f;
			for(int i = 0; i < m_Spells.Count; ++i)
			{
				var spell = m_Spells[i];
				GUIContent content;
				if(spell != null)
					content = new GUIContent($"S{i} {spell.GetName()} {spell.GetCurrentState()}");
				else
					content = new GUIContent($"S{i} NULL NULL");

				var size = GUI.skin.label.CalcSize(content);

				m_SpellGUIContent.Add(content);
				m_SpellGUISize.Add(size);
				GUIHeight += size.y;
			}

			var sPoint = new Vector2(sPos.x, Screen.height - sPos.y);
			var gPoint = GUIUtility.ScreenToGUIPoint(sPoint);

			float curHeight = gPoint.y - (heightOffset + GUIHeight);
			GUI.contentColor = Color.white;
			for(int i = 0; i < m_Spells.Count; ++i)
			{
				var size = m_SpellGUISize[i];
				var content = m_SpellGUIContent[i];
				var rect = new Rect(gPoint.x, curHeight, size.x, size.y);
				GUI.Label(rect, content);
				curHeight += size.y;
			}

			GUI.contentColor = oldColor;
		}
		private void OnDestroy()
		{
			for (int i = 0; i < m_Spells.Count; ++i)
			{
				var spell = m_Spells[i];
				if (spell == null)
					continue;
				spell.DestroySpell();
			}
		}
		public int[] GetQuickItems() => m_QuickItems;
		public Def.QuickItemSlot GetTriggeredQuickItemSlot() => m_TriggeredQuickItemSlot;
		public bool IsQuickItemTriggered()
		{
			if (!m_CanUseQuickItems)
				return false;

			bool CheckSlot(Def.QuickItemSlot quickItemSlot)
			{
				var itemID = m_QuickItems[(int)quickItemSlot];

				if (itemID >= 0 && m_InventoryDict.TryGetValue(itemID, out int inventoryIDX))
				{
					var slot = m_Inventory[inventoryIDX];
					if (slot.Item.IsActiveItem())
					{
						m_TriggeredQuickItemSlot = quickItemSlot;
						return true;
					}
				}
				return false;
			}

			if (Input.GetKeyDown(KeyCode.E) && CheckSlot(Def.QuickItemSlot.SlotE))
				return true;

			for(int i = 0; i < Def.QuickItemSlotCount - 1; ++i)
			{
				if (Input.GetKeyDown(KeyCode.Alpha1 + i) && CheckSlot(Def.QuickItemSlot.Slot1 + i))
					return true;
			}
			return false;
		}
	}
}
