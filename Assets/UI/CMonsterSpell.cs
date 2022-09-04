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

namespace Assets.UI
{
	public class CMonsterSpell : MonoBehaviour
	{
		public UnityEngine.UI.Text SpellNameText;
		public UnityEngine.UI.Button EditButton;
		public UnityEngine.UI.Button SelectButton;
		public UnityEngine.UI.Button CrossButton;

		public SpellEditorUI SpellEditor;
		public CImageSelectorUI Selector;

		List<CImageSelectorUI.ElementInfo> m_AllSpells;

		public int Slot;
		AI.Spells.ISpell m_Spell;
		public AI.Spells.ISpell GetSpell() => m_Spell;

		private void Awake()
		{
			EditButton.onClick.AddListener(OnEditButton);
			SelectButton.onClick.AddListener(OnSelectButton);
			CrossButton.onClick.AddListener(OnCrossButton);
			m_AllSpells = new List<CImageSelectorUI.ElementInfo>();
		}
		public void OnEditButton()
		{
			SpellEditor.gameObject.SetActive(true);
			SpellEditor.Init(m_Spell, OnEditButtonEnd);
		}
		public void OnEditButtonEnd()
		{
			SpellEditor.gameObject.SetActive(false);
		}
		public void OnSelectButton()
		{
			Selector.gameObject.SetActive(true);
			Selector.Init(m_AllSpells, false, OnSelectButtonEnd, Def.ImageSelectorPosition.Center);
		}
		public void OnSelectButtonEnd()
		{
			Selector.gameObject.SetActive(false);
			var selected = Selector.GetSelected();
			if(selected.Count > 0)
			{
				var spellName = selected[0];
				m_Spell = AI.Spells.SpellManager.CreateSpell(spellName);
				SpellNameText.text = m_Spell.GetName();
				EditButton.interactable = true;
			}
		}
		public void OnCrossButton()
		{
			if (Slot == 0)
			{
				m_Spell = AI.Spells.SpellManager.CreateSpell("NullSpell");
				SpellNameText.text = m_Spell.GetName();
			}
			else
			{
				SpellNameText.text = "None";
				m_Spell = null;
				EditButton.interactable = false;
			}
		}

		public void Init(AI.Spells.ISpell spell)
		{
			m_AllSpells.Clear();
			for (int i = 1; i < AI.Spells.SpellManager.Spells.Count; ++i)
			{
				var s = AI.Spells.SpellManager.Spells[i];
				m_AllSpells.Add(new CImageSelectorUI.ElementInfo()
				{
					Image = null,
					Name = s.GetName()
				});
			}
			m_Spell = spell;
			if(m_Spell == null)
			{
				OnCrossButton();
			}
			else
			{
				SpellNameText.text = m_Spell.GetName();
				EditButton.interactable = true;
			}
		}
	}
}
