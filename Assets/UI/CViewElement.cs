/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace Assets.UI
{
	public class ViewElementInfo
	{
		public Sprite Image;
		public string Text;
	}
	public class CViewElement : MonoBehaviour
	{
		public UnityEngine.UI.Button CrossButton;
		public UnityEngine.UI.Image ElementImage;
		public UnityEngine.UI.Text NameText;
		public TMPro.TMP_Text TMPNameText;
		public RectTransform Transform;

		protected CView m_View;
		
		public virtual void _OnAwake()
		{
			if(CrossButton != null)
				CrossButton.onClick.AddListener(OnRemove);
			Transform = gameObject.GetComponent<RectTransform>();
		}
		private void Awake()
		{
			_OnAwake();
		}
		public virtual void ElementInit(ViewElementInfo info, CView view)
		{
			if(ElementImage != null)
				ElementImage.sprite = info.Image;
			if(NameText != null)
				NameText.text = info.Text;
			if (TMPNameText != null)
				TMPNameText.text = info.Text;
			m_View = view;
		}
		public void OnRemove()
		{
			m_View._RemoveElement(this);
		}
	}
}