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
	public class CTriggerEditorUI : MonoBehaviour
	{
		static Action DefaultOnClose = () => { };
		public UnityEngine.UI.Button CrossButton;
		public CView ConfigView;

		public AI.Quirks.IQuirkTrigger Trigger;
		Action m_OnClose;

		void Awake()
		{
			CrossButton.onClick.AddListener(OnClose);
		}
		public void Init(AI.Quirks.IQuirkTrigger trigger, Action onClose = null)
		{
			m_OnClose = onClose;
			if (m_OnClose == null)
				m_OnClose = DefaultOnClose;
			Trigger = trigger;
			for(int i = 0; i < Trigger.GetConfig().Count; ++i)
			{
				var conf = Trigger.GetConfig()[i];
				ConfigView.AddElement(new ViewElementConfigInfo()
				{
					Text = conf.GetConfigName(),
					Config = conf,
					Image = null
				});
			}
		}
		void OnClose()
		{
			ConfigView.Clear();
			m_OnClose();
		}
	}
}
