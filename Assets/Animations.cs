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
	public class FramedAnimation : MonoBehaviour
	{
		Timer m_Timer;

		float m_WaitTime = float.MaxValue;

		public void ChangeFPS(float fps)
		{
			if (fps > 0f)
				m_WaitTime = 1f / fps;
			else
				m_WaitTime = float.MaxValue;

			m_Timer.Reset(m_WaitTime);
		}
		public float GetFPS()
		{
			if (m_WaitTime <= 0f)
				return 0f;
			return 1f / m_WaitTime;
		}
		public Timer GetTimer() => m_Timer;

		private void Awake()
		{
			m_Timer = new Timer();
		}

		public void Set(float fps, Timer.TimerTriggerCB onTrigger, bool pauseAffected = true)
		{
			m_Timer.SetAffectPause(pauseAffected);
			ChangeFPS(fps);
			m_Timer.OnTimerTrigger += onTrigger;
		}
		public void Restart()
		{
			m_Timer.Reset(m_WaitTime);
		}
		private void Update()
		{
			m_Timer.Update();
		}
	}

	public class SquashStrechAnimation : MonoBehaviour
	{
		public enum RoundsType
		{
			Props,
			Monsters
		}
		Transform m_Transform;
		Timer m_Timer;
		Vector3 m_DefaultScale;
		bool m_Bidimiensional;
		float m_Strength;
		bool m_DestroyAtEnd;
		Action m_OnEnd;

		Vector2[][] m_Rounds;

		static readonly Vector2[][] RoundsMonster = new Vector2[][]
		{
			new Vector2[2]
			{
				Vector2.one,
				new Vector2(1.1f, 0.9f)
			},
			new Vector2[2]
			{
				new Vector2(1.1f, 0.9f),
				Vector2.one,
			},
			new Vector2[2]
			{
				Vector2.one,
				new Vector2(0.9f, 1.2f)
			},
			new Vector2[2]
			{
				new Vector2(0.9f, 1.2f),
				Vector2.one,
			},
			new Vector2[2]
			{
				Vector2.one,
				new Vector2(1.05f, 0.95f)
			},
			new Vector2[2]
			{
				new Vector2(1.05f, 0.95f),
				Vector2.one,
			},
			new Vector2[2]
			{
				Vector2.one,
				Vector2.one
			},
		};
		static readonly Vector2[][] RoundsProp = new Vector2[7][]
		{
			new Vector2[2]
			{
				Vector2.one,
				new Vector2(0.8f, 1.2f)
			},
			new Vector2[2]
			{
				new Vector2(0.8f, 1.2f),
				Vector2.one
			},
			new Vector2[2]
			{
				Vector2.one,
				new Vector2(0.9f, 1.1f)
			},
			new Vector2[2]
			{
				new Vector2(0.9f, 1.1f),
				Vector2.one
			},
			new Vector2[2]
			{
				Vector2.one,
				new Vector2(0.95f, 1.05f)
			},
			new Vector2[2]
			{
				new Vector2(0.95f, 1.05f),
				Vector2.one
			},
			new Vector2[2]
			{
				Vector2.one,
				Vector2.one
			},
		};

		void SetScale(Vector2 scale)
		{
			m_Transform.localScale = m_Bidimiensional ?
				new Vector3(scale.x, scale.y, 1f) :
				new Vector3(scale.x, scale.y, scale.x);
		}
		void OnAnimation()
		{
			var pct = m_Timer.GetRemainingPct();// (m_StartTime + m_Duration) - Time.time;
			float t;
			var scale = Vector2.one;
			float timeScale = 1f / (m_Rounds.Length - 1);
			var defaultScale = new Vector2(m_DefaultScale.x, m_DefaultScale.y);

			bool done = false;
			for(int i = 0; i < (m_Rounds.Length - 1); ++i)
			{
				if(pct < (i + 1) * timeScale)
				{
					t = pct - (i * timeScale);
					t /= timeScale;
					scale = (1f - t) * (defaultScale * m_Rounds[i][0]) + t * (defaultScale * m_Rounds[i][1]);
					done = true;
					break;
				}
			}
			if(!done)
			{
				return;
			}

			SetScale(scale);
		}
		void OnAnimationEnd()
		{
			SetScale(m_DefaultScale);
			if(m_DestroyAtEnd)
				GameObject.Destroy(this);
			m_OnEnd();
			enabled = false;
		}
		public SquashStrechAnimation()
		{
			m_Timer = new Timer(true, false);
			m_Timer.OnTimerTrigger += OnAnimationEnd;
		}
		public void Set(Transform trfm, Vector3 defaultScale, float duration, bool bidimensional, RoundsType roundsType, bool destroyAtEnd = true, float strength = 1f, Action onEnd = null)
		{
			if (onEnd == null)
				onEnd = () => { };

			m_OnEnd = onEnd;
			m_Transform = trfm;
			m_Strength = strength;
			m_Timer.Reset(duration);
			m_DefaultScale = defaultScale;
			m_Bidimiensional = bidimensional;
			m_DestroyAtEnd = destroyAtEnd;
			switch (roundsType)
			{
				case RoundsType.Props:
					m_Rounds = RoundsProp;
					break;
				case RoundsType.Monsters:
					m_Rounds = RoundsMonster;
					break;
				default:
					m_Rounds = RoundsProp;
					Debug.LogWarning("Unhandled SquashStrech animation with round type " + roundsType.ToString());
					break;
			}
		}
		private void Update()
		{
			OnAnimation();
			m_Timer.Update();
		}
	}
	public class FadeoutAnimation : MonoBehaviour
	{
		Func<Color> m_GetColorFn;
		Action<Color> m_SetColorFn;
		Timer m_Timer;
		bool m_DestroyAtEnd;
		//float m_StartTime;
		//float m_EndTime;
		Action m_OnEnd;

		public FadeoutAnimation()
		{
			m_Timer = new Timer(true, false);
			m_Timer.OnTimerTrigger += OnAnimationEnd;
		}
		void OnAnimation()
		{
			var curTime = Time.time;
			var color = m_GetColorFn();

			color.a = 1f - m_Timer.GetRemainingPct();
			m_SetColorFn(color);
		}
		void OnAnimationEnd()
		{
			var color = m_GetColorFn();
			color.a = 0f;
			m_SetColorFn(color);
			if(m_DestroyAtEnd)
				GameObject.Destroy(this);
			m_OnEnd();
			enabled = false;
		}
		public void Set(Func<Color> getColorFn, Action<Color> setColorFn, float duration, bool destroyAtEnd = true, Action onEnd = null)
		{
			if (onEnd == null)
				onEnd = () => { };
			m_OnEnd = onEnd;
			m_Timer.Reset(duration);
			//m_StartTime = Time.time;
			//m_EndTime = Time.time + duration;
			m_DestroyAtEnd = destroyAtEnd;
			m_GetColorFn = getColorFn;
			m_SetColorFn = setColorFn;
		}
		private void Update()
		{
			OnAnimation();
			m_Timer.Update();
		}
	}
}