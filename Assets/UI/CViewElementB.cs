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
	public class ViewElementInfoB
	{
		// Empty
	}
	[RequireComponent(typeof(UnityEngine.RectTransform))]
	public class CViewElementB : MonoBehaviour
	{
		protected RectTransform m_RT;
		protected CViewB m_View;
		public UnityEngine.UI.Button CrossButton; // Not mandatory

		public RectTransform GetRT() => m_RT;

		private void Awake()
		{
			_Awake();
		}
		public virtual void _Awake()
		{
			m_RT = gameObject.GetComponent<RectTransform>();
			if (CrossButton != null)
			{
				CrossButton.onClick.AddListener(() =>
				{
					if (m_View != null)
						m_View.RemoveElement(this);
				});
			}
		}
		public virtual void ElementInit(CViewB view, ViewElementInfoB info)
		{
			m_View = view;
		}
	}
}
