/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.UI
{
	public class ViewElementTriggerInfo : ViewElementInfo
	{
		public AI.Quirks.IQuirkTrigger Trigger;
		public bool Inverted;
	}
	public class CViewElementQuirkTrigger : CViewElement
	{
		public QuirkEditorUI QuirkUI;
		//public TriggerEditorUI TriggerUI;
		public UnityEngine.UI.Button EditButton;
		public UnityEngine.UI.Toggle InvertedToggle;
		public AI.Quirks.IQuirkTrigger QuirkTrigger;
		public CTriggerEditorUI TriggerEditorUI;

		public override void _OnAwake()
		{
			base._OnAwake();
			EditButton.onClick.AddListener(OnEditButton);
			InvertedToggle.onValueChanged.AddListener(OnInvertedToggle);
		}
		void OnEditButton()
		{
			enabled = false;
			TriggerEditorUI.gameObject.SetActive(true);
			TriggerEditorUI.Init(QuirkTrigger, OnEditButtonEnd);
		}
		void OnEditButtonEnd()
		{
			enabled = true;
			TriggerEditorUI.gameObject.SetActive(false);
		}
		void OnInvertedToggle(bool value)
		{
			QuirkTrigger.SetInverted(value);
		}
		public override void ElementInit(ViewElementInfo info, CView view)
		{
			base.ElementInit(info, view);
			var triggerInfo = info as ViewElementTriggerInfo;
			QuirkTrigger = triggerInfo.Trigger;
			InvertedToggle.isOn = triggerInfo.Inverted;
		}
	}
}
