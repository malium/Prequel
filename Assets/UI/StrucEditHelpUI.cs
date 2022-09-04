/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace Assets.UI
{
	public class StrucEditHelpUI : MonoBehaviour
	{
		Action m_OnExit;
		public void Init(Action onExit)
		{
			m_OnExit = onExit;
		}
		void Update()
		{
			if (!Input.GetKey(KeyCode.F1))
				m_OnExit();
		}
	}
}