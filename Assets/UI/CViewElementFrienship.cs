/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace Assets.UI
{
	public class FriendshipViewElementInfo : ViewElementInfo
	{
		public bool IsFriend;
		public Action<string, bool> OnFriendshipChange;
	}

	public class CViewElementFrienship : CViewElement
	{
		public UnityEngine.UI.Button FriendButton;
		public UnityEngine.UI.Button EnemyButton;

		Action<string, bool> m_OnFriendChange;

		public override void _OnAwake()
		{
			base._OnAwake();
			FriendButton.onClick.AddListener(OnFriendButton);
			EnemyButton.onClick.AddListener(OnEnemyButton);
			EnemyButton.interactable = false;
		}
		void OnFriendButton()
		{
			FriendButton.interactable = false;
			EnemyButton.interactable = true;
			m_OnFriendChange(NameText.text, true);
		}
		void OnEnemyButton()
		{
			FriendButton.interactable = true;
			EnemyButton.interactable = false;
			m_OnFriendChange(NameText.text, false);
		}
		public override void ElementInit(ViewElementInfo info, CView view)
		{
			var info2 = info as FriendshipViewElementInfo;
			base.ElementInit(info, view);
			m_OnFriendChange = info2.OnFriendshipChange;
			if (info2.IsFriend)
				OnFriendButton();
			else
				OnEnemyButton();
		}
	}
}