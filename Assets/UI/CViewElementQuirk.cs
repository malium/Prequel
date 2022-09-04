/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.UI
{
	public class ViewElementQuirkInfo : ViewElementInfo
	{
		public AI.Quirks.QuirkInfo QuirkInfo;
	}
	public class CViewElementQuirk : CViewElement
	{
		public QuirkEditorUI EditorUI;
		public UnityEngine.UI.Button EditButton;
		public UnityEngine.UI.InputField PriorityIF;
		public AI.Quirks.IQuirk Quirk;
		public List<AI.Quirks.IQuirkTrigger> Triggers;

		public override void _OnAwake()
		{
			base._OnAwake();
			EditButton.onClick.AddListener(OnEditButton);
		}

		public override void ElementInit(ViewElementInfo info, CView view)
		{
			base.ElementInit(info, view);
			var quirkInfo = (info as ViewElementQuirkInfo).QuirkInfo;

			Quirk = quirkInfo.Quirk;
			PriorityIF.text = quirkInfo.Priority.ToString();
			Triggers = quirkInfo.Triggers;
		}

		void OnEditButton()
		{
			EditorUI.gameObject.SetActive(true);
			EditorUI.Init(Quirk, Triggers, OnEditButtonEnd);
		}
		void OnEditButtonEnd()
		{
			EditorUI.gameObject.SetActive(false);
			Triggers = EditorUI.GetTriggers();
		}
	}
}