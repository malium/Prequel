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
	public class MessageBoxUI : MonoBehaviour
	{
		static readonly Action DefaultAction = () => { };
		static MessageBoxUI Instance;

		public TMPro.TMP_Text Title;
		public TMPro.TMP_Text Message;
		public UnityEngine.UI.Button CrossButton;
		public UnityEngine.UI.Button OKButton;
		public UnityEngine.UI.Button YesButton;
		public UnityEngine.UI.Button NoButton;
		RectTransform m_RT;

		Def.MessageBoxType m_Type;
		Action m_OnCross;
		Action m_OnOK;
		Action m_OnYes;
		Action m_OnNo;
		Action SetAction(Action nAction) => nAction == null ? DefaultAction : nAction;
		MessageBoxUI()
		{
			m_OnCross = SetAction(null);
			m_OnOK = SetAction(null);
			m_OnYes = SetAction(null);
			m_OnNo = SetAction(null);
		}
		private void Awake()
		{
			CrossButton.onClick.AddListener(OnCross);
			OKButton.onClick.AddListener(OnOk);
			YesButton.onClick.AddListener(OnYes);
			NoButton.onClick.AddListener(OnNo);
			m_RT = gameObject.GetComponent<RectTransform>();
		}
		public Def.MessageBoxType GetMessageBoxType() => m_Type;
		public void Init(Def.MessageBoxType type, string title, string message, Action onCross = null,
			Action onOk = null, Action onYes = null, Action onNo = null)
		{
			m_RT.offsetMax = Vector2.zero;
			m_RT.offsetMin = Vector2.zero;
			Title.text = title;
			Message.text = message;
			m_Type = type;

			switch (m_Type)
			{
				case Def.MessageBoxType.OnlyCross:
					CrossButton.gameObject.SetActive(true);
					OKButton.gameObject.SetActive(false);
					YesButton.gameObject.SetActive(false);
					NoButton.gameObject.SetActive(false);
					break;
				case Def.MessageBoxType.OnlyOk:
					CrossButton.gameObject.SetActive(false);
					OKButton.gameObject.SetActive(true);
					YesButton.gameObject.SetActive(false);
					NoButton.gameObject.SetActive(false);
					break;
				case Def.MessageBoxType.CrossNOk:
					CrossButton.gameObject.SetActive(true);
					OKButton.gameObject.SetActive(true);
					YesButton.gameObject.SetActive(false);
					NoButton.gameObject.SetActive(false);
					break;
				case Def.MessageBoxType.YesNo:
					CrossButton.gameObject.SetActive(false);
					OKButton.gameObject.SetActive(false);
					YesButton.gameObject.SetActive(true);
					NoButton.gameObject.SetActive(true);
					break;
				case Def.MessageBoxType.YesNoNCross:
					CrossButton.gameObject.SetActive(true);
					OKButton.gameObject.SetActive(false);
					YesButton.gameObject.SetActive(true);
					NoButton.gameObject.SetActive(true);
					break;
			}
			m_OnCross = SetAction(onCross);
			m_OnOK = SetAction(onOk);
			m_OnYes = SetAction(onYes);
			m_OnNo = SetAction(onNo);
		}
		public static MessageBoxUI Create()
		{
			var obj = Instantiate(Instance);
			obj.transform.SetParent(Instance.transform.parent);
			obj.gameObject.SetActive(true);
			return obj;
		}
		public static void LoadInit()
		{
			var mb = GameUtils.FindChild(CameraManager.Mgr.Canvas.gameObject, "MessageBox");
			if(mb == null)
			{
				Debug.LogError("Couldn't find MessageBox as a Canvas child.");
				return;
			}
			Instance = mb.GetComponent<MessageBoxUI>();
			if(Instance == null)
			{
				Debug.LogWarning("MessageBox GameObject do not have MessageBoxUI component!");
			}
		}
		void OnCross()
		{
			m_OnCross();
		}
		void OnOk()
		{
			m_OnOK();
		}
		void OnNo()
		{
			m_OnNo();
		}
		void OnYes()
		{
			m_OnYes();
		}
	}
}
