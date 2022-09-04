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
	public class CViewElementBItemEffect : CViewElementB
	{
		public TMPro.TMP_Text Name;
		public UnityEngine.UI.Button ConfigBttn;

		static CViewElementBItemEffect Instance;
		public static CViewElementBItemEffect GetInstance()
		{
			if (Instance == null)
			{
				Instance = Resources.Load<CViewElementBItemEffect>("UI/ViewElementItemEffect");
				if (Instance == null)
					throw new Exception("Couldn't load ViewElementNameImageSelectable");
			}
			return Instance;
		}
		static ItemEffectEditorUI EffectEditorUI;
		void LoadEffectEditorUI()
		{
			if (EffectEditorUI != null)
				return;

			EffectEditorUI = Instantiate(ItemEffectEditorUI.GetInstance());

			var canvas = CameraManager.Mgr.Canvas;
			EffectEditorUI.transform.SetParent(canvas.transform);

			var rt = EffectEditorUI.gameObject.GetComponent<RectTransform>();
			rt.sizeDelta = Vector2.zero;
			rt.anchoredPosition = Vector2.zero;

			EffectEditorUI.OnClose += OnEffectEditorClose;
		}
		AI.Items.IItemEffect m_ItemEffect;
		public AI.Items.IItemEffect GetItemEffect() => m_ItemEffect;
		public override void ElementInit(CViewB view, ViewElementInfoB info)
		{
			base.ElementInit(view, info);

			m_ItemEffect = (info as ViewElementBItemEffectInfo).ItemEffect;
			if (m_ItemEffect != null && Name != null)
				Name.text = m_ItemEffect.GetItemEffectType().ToString();
		}
		public override void _Awake()
		{
			base._Awake();

			if (ConfigBttn != null)
				ConfigBttn.onClick.AddListener(OnConfigButton);
		}
		void OnConfigButton()
		{
			LoadEffectEditorUI();
			
			EffectEditorUI.gameObject.SetActive(true);
			EffectEditorUI.Init(m_ItemEffect);
		}
		void OnEffectEditorClose()
		{
			EffectEditorUI.gameObject.SetActive(false);
		}

	}
	public class ViewElementBItemEffectInfo : ViewElementInfoB
	{
		public AI.Items.IItemEffect ItemEffect;
	}
}
