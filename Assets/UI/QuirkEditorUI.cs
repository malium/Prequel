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
	public class QuirkEditorUI : MonoBehaviour
	{
		static Action DefaultOnClose = () => { };
		public UnityEngine.UI.Button CrossButton;
		public CView ConfigView;
		public CView TriggerView;
		public UnityEngine.UI.Button AddTriggerButton;
		public CImageSelectorUI ImageSelectorUI;

		Action m_OnClose;
		AI.Quirks.IQuirk m_Quirk;
		static List<CImageSelectorUI.ElementInfo> m_AllTriggers;
		public AI.Quirks.IQuirk GetQuirk() => m_Quirk;
		public List<AI.Quirks.IQuirkTrigger> GetTriggers()
		{
			var elements = TriggerView.GetElements();
			var triggers = new List<AI.Quirks.IQuirkTrigger>(elements.Count);

			for(int i = 0; i < elements.Count; ++i)
			{
				var elem = TriggerView.GetElements()[i] as CViewElementQuirkTrigger;
				triggers.Add(elem.QuirkTrigger);
			}

			return triggers;
		}

		private void Awake()
		{
			CrossButton.onClick.AddListener(OnCross);
			AddTriggerButton.onClick.AddListener(OnAddTrigger);
		}
		void PrepareTriggers()
		{
			if (m_AllTriggers != null)
				return;

			m_AllTriggers = new List<CImageSelectorUI.ElementInfo>(AI.Quirks.QuirkManager.Triggers.Count);
			for(int i = 0; i < AI.Quirks.QuirkManager.Triggers.Count; ++i)
			{
				var trigger = AI.Quirks.QuirkManager.Triggers[i];
				m_AllTriggers.Add(new CImageSelectorUI.ElementInfo()
				{
					Image = null,
					Name = trigger.GetName()
				});
			}
		}
		public void Init(AI.Quirks.IQuirk quirk, List<AI.Quirks.IQuirkTrigger> triggers, Action onClose = null)
		{
			PrepareTriggers();
			m_Quirk = quirk;
			m_OnClose = onClose;
			if (m_OnClose == null)
				m_OnClose = DefaultOnClose;
			for(int i = 0; i < m_Quirk.GetConfiguration().Count; ++i)
			{
				var conf = m_Quirk.GetConfiguration()[i];
				ConfigView.AddElement(new ViewElementConfigInfo()
				{
					Image = null,
					Text = conf.GetConfigName(),
					Config = conf
				});
			}
			for(int i = 0; i < triggers.Count; ++i)
			{
				var trigger = triggers[i];
				TriggerView.AddElement(new ViewElementTriggerInfo()
				{
					Image = null,
					Inverted = trigger.IsInverted(),
					Text = trigger.GetName(),
					Trigger = trigger
				});
			}
		}
		void OnAddTrigger()
		{
			enabled = false;
			ImageSelectorUI.gameObject.SetActive(true);
			ImageSelectorUI.Init(m_AllTriggers, false, OnAddTriggerEnd, Def.ImageSelectorPosition.Center);
		}
		void OnAddTriggerEnd()
		{
			enabled = true;
			ImageSelectorUI.gameObject.SetActive(false);
			var selectedElements = ImageSelectorUI.GetSelected();
			if(selectedElements.Count > 0)
			{
				var elem = selectedElements[0];
				var defaultTrigger = AI.Quirks.QuirkManager.CreateTrigger(elem);
				TriggerView.AddElement(new ViewElementTriggerInfo()
				{
					Image = null,
					Inverted = false,
					Text = defaultTrigger.GetName(),
					Trigger = defaultTrigger
				});
			}
		}
		void OnCross()
		{
			m_OnClose();
			ConfigView.Clear();
			TriggerView.Clear();
		}
	}
}