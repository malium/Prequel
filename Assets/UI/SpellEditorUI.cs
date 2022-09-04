/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Assets.UI
{
	public class SpellEditorUI : MonoBehaviour
	{
		public UnityEngine.UI.Button CrossButton;
		public UnityEngine.UI.Button AddOnHitButton;

		public CView SpellView;
		public CView OnHitView;

		public CImageSelectorUI SelectorUI;

		Action m_OnClose;
		AI.Spells.ISpell m_Spell;
		List<CImageSelectorUI.ElementInfo> m_AllOnHits;

		void OnAddOnHit()
		{
			SelectorUI.gameObject.SetActive(true);
			SelectorUI.Init(m_AllOnHits, false, OnAddOnHitEnd, Def.ImageSelectorPosition.Center);
		}
		void OnAddOnHitEnd()
		{
			SelectorUI.gameObject.SetActive(false);
			var selected = SelectorUI.GetSelected();
			if(selected.Count > 0)
			{
				var sel = selected[0];
				var onHit = AI.Spells.SpellManager.CreateOnHit(sel);
				onHit.SetChance(100f);
				//m_Spell.GetOnHit()[onHit.GetOnHitType()].Add(onHit);
				OnHitView.AddElement(new ViewElementOnHitInfo()
				{
					Image = null,
					Text = onHit.GetName(),
					OnHit = onHit
				});
			}
		}
		
		private void Awake()
		{
			CrossButton.onClick.AddListener(OnCross);
			AddOnHitButton.onClick.AddListener(OnAddOnHit);

			m_AllOnHits = new List<CImageSelectorUI.ElementInfo>();
		}

		public void Init(AI.Spells.ISpell spell, Action onClose)
		{
			m_Spell = spell;
			m_OnClose = onClose;
			for (int i = 0; i < m_Spell.GetConfig().Count; ++i)
			{
				var conf = m_Spell.GetConfig()[i];
				SpellView.AddElement(new ViewElementConfigInfo()
				{
					Image = null,
					Text = conf.GetConfigName(),
					Config = conf
				});
			}
			for(int i = 0; i < Def.OnHitTypeCount; ++i)
			{
				var onHitList = m_Spell.GetOnHit()[(Def.OnHitType)i];
				for(int j = 0; j < onHitList.Count; ++j)
				{
					var onHit = onHitList[j];
					OnHitView.AddElement(new ViewElementOnHitInfo()
					{
						Image = null,
						Text = onHit.GetName(),
						OnHit = onHit
					});
				}
			}
			m_AllOnHits.Clear();
			for(int i = 0; i < AI.Spells.SpellManager.OnHits.Count; ++i)
			{
				m_AllOnHits.Add(new CImageSelectorUI.ElementInfo()
				{
					Image = null,
					Name = AI.Spells.SpellManager.OnHits[i].GetName()
				});
			}
		}

		void OnCross()
		{
			// Dump all OnHits into the Spell
			for (int i = 0; i < Def.OnHitTypeCount; ++i)
				m_Spell.GetOnHit()[(Def.OnHitType)i].Clear();
			for(int i = 0; i < OnHitView.GetElements().Count; ++i)
			{
				var elem = OnHitView.GetElements()[i] as CViewElementOnHit;
				m_Spell.AddOnHit(elem.GetOnHit());
			}

			OnHitView.Clear();
			SpellView.Clear();
			m_OnClose();
		}
	}
}