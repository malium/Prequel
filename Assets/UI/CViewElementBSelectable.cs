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
	public class ViewElementBSelectableInfo : ViewElementInfoB
	{
		public SelectorUI Selector;
		public string Name;
		public bool Selected;
	}
	public class CViewElementBSelectable : CViewElementB
	{
		public UnityEngine.UI.Button SelectButton;
		public UnityEngine.UI.Toggle SelectToggle;

		protected SelectorUI m_Selector;
		protected string m_Name;
		bool m_ValueLock;
		public override void ElementInit(CViewB view, ViewElementInfoB info)
		{
			base.ElementInit(view, info);

			var selectableInfo = info as ViewElementBSelectableInfo;

			m_Selector = selectableInfo.Selector;
			m_Name = selectableInfo.Name;

			if (m_Selector.IsMultiselection())
			{
				if (SelectButton != null)
				{
					SelectButton.gameObject.SetActive(false);
				}

				if (SelectToggle != null)
				{
					SelectToggle.gameObject.SetActive(true);
					SelectToggle.isOn = selectableInfo.Selected;
				}
			}
			else
			{
				if (SelectToggle != null)
				{
					SelectToggle.gameObject.SetActive(false);
				}

				if (SelectButton != null)
				{
					SelectButton.gameObject.SetActive(true);
				}
			}
		}
		public string GetName() => m_Name;
		public override void _Awake()
		{
			base._Awake();
			if (SelectButton != null)
				SelectButton.onClick.AddListener(OnSelectedBttn);

			if (SelectToggle != null)
				SelectToggle.onValueChanged.AddListener(OnSelectedTggl);
		}
		void OnSelectedBttn()
		{
			m_Selector.SelectItem(m_Name);
		}
		void OnSelectedTggl(bool sel)
		{
			if (m_ValueLock)
				return;

			if (sel)
				m_Selector.SelectItem(m_Name);
			else
				m_Selector.UnselectItem(m_Name);
		}
	}
}
