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

namespace Assets
{
	[Serializable]
	public class Timer
	{
		[SerializeField] float m_TotalTime;
		[SerializeField] float m_Remaining;
		[SerializeField] bool m_AffectsPause;
		[SerializeField] bool m_AutoReset;
		[SerializeField] bool m_Enabled;

		public event TimerTriggerCB OnTimerTrigger;
		public Timer(bool pauseAffected = true, bool autoReset = false)
		{
			m_Remaining = m_TotalTime = 0f;
			m_AffectsPause = pauseAffected;
			m_AutoReset = autoReset;
			m_Enabled = true;
		}
		public Timer(float seconds, bool pauseAffected = true, bool autoReset = false)
		{
			Reset(seconds);
			m_AffectsPause = pauseAffected;
			m_AutoReset = autoReset;
			m_Enabled = true;
		}
		public void Reset()
		{
			m_Remaining = m_TotalTime;
		}
		public void Reset(float time)
		{
			m_Remaining = m_TotalTime = time;
		}
		public void Stop()
		{
			m_Remaining = 0f;
		}
		public float GetRemainingTime() => m_Remaining;
		public float GetTotalTime() => m_TotalTime;
		public float GetRemainingPct()
		{
			if (m_TotalTime == 0f)
				return 0f; // Avoid division by zero
			return (m_TotalTime - m_Remaining) / m_TotalTime;
		}
		public void SetAffectPause(bool affectsPause) => m_AffectsPause = affectsPause;
		public bool IsPauseAffected() => m_AffectsPause;
		public void SetAutoReset(bool autoReset) => m_AutoReset = autoReset;
		public bool IsAutoReset() => m_AutoReset;
		public void SetEnabled(bool enable) => m_Enabled = enable;
		public bool IsEnabled() => m_Enabled;
		public bool IsFinished() => m_Remaining <= 0f;
		//public void SetTotalTime(float time) => m_TotalTime = time;
		public bool IsPaused() => m_AffectsPause && Manager.Mgr.IsPaused;
		public void Update()
		{
			if(!IsEnabled() || IsPaused() || IsFinished())
				return;

			m_Remaining -= Time.deltaTime;

			UpdateRemaining();
		}
		public void FixedUpdate()
		{
			if (!IsEnabled() || IsPaused() || IsFinished())
				return;

			m_Remaining -= Time.fixedDeltaTime;

			UpdateRemaining();
		}
		void UpdateRemaining()
		{
			if (m_Remaining <= 0f)
			{
				if (!m_AutoReset)
					m_Remaining = 0f;
				OnTimerTrigger?.Invoke();
				if (m_AutoReset)
				{
					float r = m_Remaining * -1f;
					int times = Mathf.FloorToInt(r / m_TotalTime);
					for (int i = 0; i < times; ++i)
						OnTimerTrigger?.Invoke();
					r -= times * m_TotalTime;

					Reset(m_TotalTime);
					m_Remaining -= r;
				}
			}
		}
		public void GetFullTime(out int hours, out int minutes, out int seconds, out int milliseconds)
		{
			hours = minutes = seconds = milliseconds = 0;
			if (m_Remaining <= 0f)
				return;

			int sec = Mathf.FloorToInt(m_Remaining);
			milliseconds = Mathf.FloorToInt((m_Remaining - seconds) * 1000f);
			hours = Mathf.FloorToInt(sec / 3600f);
			sec -= hours * 3600;
			minutes = Mathf.FloorToInt(sec / 60f);
			sec -= minutes * 60;
			seconds = sec;
		}
		public delegate void TimerTriggerCB();
	}
}
