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
	public class ItemEffectEditorUI : MonoBehaviour
	{
		public UnityEngine.UI.Button CrossBttn;

		public TMPro.TMP_Text ItemEffectTypeName;
		public UnityEngine.UI.Toggle IsActivableTggl;
		public TMPro.TMP_InputField EditIF;
		public TMPro.TMP_InputField ShowIF;
		public CViewB ConfigView;

		public event OnCloseCB OnClose;

		AI.Items.IItemEffect m_ItemEffect;

		static ItemEffectEditorUI Instance;
		public static ItemEffectEditorUI GetInstance()
		{
			if (Instance == null)
			{
				Instance = Resources.Load<ItemEffectEditorUI>("UI/ItemEffectEditorUI");
				if (Instance == null)
					throw new Exception("Couldn't load ItemEffectEditorUI");
			}
			return Instance;
		}

		private void Awake()
		{
			CrossBttn.onClick.AddListener(OnCrossButton);
			EditIF.onValueChanged.AddListener(OnDescriptionTextChange);
			EditIF.onEndEdit.AddListener(OnDescriptionTextFinish);
		}
		void OnDescriptionTextChange(string text)
		{
			ShowIF.text = text;
			ShowIF.verticalScrollbar.value = EditIF.verticalScrollbar.value;
		}
		void OnDescriptionTextFinish(string text)
		{
			m_ItemEffect.SetDisplayString(text);
		}
		public void Init(AI.Items.IItemEffect itemEffect)
		{
			m_ItemEffect = itemEffect;
			ConfigView.Init(CViewElementBConfig.GetConfigInstance());
			for(int i = 0; i < m_ItemEffect.GetConfig().Count; ++i)
			{
				ConfigView.AddElement(new ViewElementBConfigInfo()
				{
					Config = m_ItemEffect.GetConfig()[i]
				});
			}
			ItemEffectTypeName.text = m_ItemEffect.GetItemEffectType().ToString();
			IsActivableTggl.isOn = m_ItemEffect.IsActivable();
			EditIF.text = m_ItemEffect.GetDisplayString();
		}
		void OnCrossButton()
		{
			OnClose?.Invoke();
		}
		public AI.Items.IItemEffect GetItemEffect() => m_ItemEffect;
		public delegate void OnCloseCB();
	}
}
