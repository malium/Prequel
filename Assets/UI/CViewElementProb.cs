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
	public class ViewElementProbInfo : ViewElementInfo
	{
		public float Probability;
	}
	public class CViewElementProb : CViewElement
	{
		public CTMPSlider ProbabilitySlider;
		public UnityEngine.UI.Toggle LockToggle;
		bool m_ValueLock = false;
		Action<string, float> m_OnProbChange;

		public bool IsLocked()
		{
			if (LockToggle == null)
				return false;
			return LockToggle.isOn;
		}
		public void SetOnProbChange(Action<string, float> onProbChange)
		{
			m_OnProbChange = onProbChange;
			if (m_OnProbChange == null)
				m_OnProbChange = (string _1, float _2) => { };
		}
		public override void _OnAwake()
		{
			base._OnAwake();
			if (LockToggle != null)
				LockToggle.onValueChanged.AddListener(OnLockToggle);
		}
		public override void ElementInit(ViewElementInfo info, CView view)
		{
			base.ElementInit(info, view);
			SetOnProbChange(null);
			ProbabilitySlider.SetCallback(OnProbChange);
			var probInfo = info as ViewElementProbInfo;

			SetProbValue(probInfo.Probability);
		}
		public void SetProbValue(float value)
		{
			m_ValueLock = true;
			ProbabilitySlider.SetValue(value);
			m_ValueLock = false;
		}
		void OnProbChange(float value)
		{
			if (m_ValueLock)
				return;
			m_OnProbChange(TMPNameText.text, value);
		}
		void OnLockToggle(bool value)
		{
			if (ProbabilitySlider == null)
				return;
			if (ProbabilitySlider.Slider != null)
				ProbabilitySlider.Slider.interactable = !value;
			if (ProbabilitySlider.Input != null)
				ProbabilitySlider.Input.interactable = !value;
		}
	}
}
