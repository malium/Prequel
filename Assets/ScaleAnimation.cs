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
using UnityEngine.Events;

namespace Assets
{
	public class ScaleAnimation : MonoBehaviour
	{
		[SerializeField] ScaleAnimationInfo m_Info;
		Timer m_Timer;
		public UnityEvent OnEnd;

		public ScaleAnimation()
		{
			m_Timer = new Timer();
			m_Timer.OnTimerTrigger += OnTimerFinish;
		}
		private void OnTimerFinish()
		{
			var lastKeyFrame = m_Info.KeyFrames[m_Info.KeyFrames.Length - 1];
			m_Info.Target.localScale = lastKeyFrame.Value;
			OnEnd?.Invoke();
		}
		void TickAnimation()
		{
			var pct = m_Timer.GetRemainingPct();
			//Debug.Log($"PCT {pct}");
			for(int i = 0; i < m_Info.KeyFrames.Length - 1; ++i)
			{
				var keyFrame = m_Info.KeyFrames[i];
				var nextKeyFrame = m_Info.KeyFrames[i + 1];

				if (keyFrame.PCT > pct || nextKeyFrame.PCT < pct)
					continue;

				var diff = Mathf.Abs(keyFrame.PCT - nextKeyFrame.PCT);
				var start = pct - keyFrame.PCT;
				var lerp = start / diff;
				//if (lerp < 0f)
				//	Debug.Log("HI");
				//Debug.Log($"Lerp {lerp} Start {keyFrame.Value} End {nextKeyFrame.Value}");
				m_Info.Target.transform.localScale = Vector3.Lerp(keyFrame.Value, nextKeyFrame.Value, lerp);
				return;
			}
			if (m_Info.KeyFrames.Length == 1)
				m_Info.Target.transform.localScale = m_Info.KeyFrames[0].Value;
		}
		public void Set(ScaleAnimationInfo info)
		{
			m_Info = info;
			enabled = true;
			m_Timer.Reset(m_Info.Duration);
		}
		private void Start()
		{
			if(m_Timer.IsFinished() || m_Timer.IsPaused())
				m_Timer.Reset(m_Info.Duration);
		}
		private void Update()
		{
			TickAnimation();
			m_Timer.Update();
		}
	}
	[Serializable]
	public class AnimKeyFrameV3
	{
		public Vector3 Value;
		public float PCT;
	}
	[Serializable]
	public class ScaleAnimationInfo
	{
		public Transform Target;
		public AnimKeyFrameV3[] KeyFrames;
		public float Duration;
	}
}