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
	public class CVFXUI : MonoBehaviour
	{
		[SerializeField] UnityEngine.UI.Image m_Image;
		FramedAnimation m_Animation;

		[SerializeField] Sprite[] m_Frames;
		[SerializeField] float m_FPS;
		[SerializeField] int m_CurrentFrame;
		[SerializeField] Def.VFXEnd m_OnEnd;

		public UnityEvent<CVFXUI> OnEnd;

		private void Awake()
		{
			m_Animation = gameObject.AddComponent<FramedAnimation>();
		}
		private void Start()
		{
			m_CurrentFrame = 0;
			m_Animation.Set(m_FPS, OnAnimation, false);
			m_Animation.GetTimer().SetAutoReset(true);
			m_Image.sprite = m_Frames[m_CurrentFrame];
		}
		void OnAnimation()
		{
			if (m_CurrentFrame >= m_Frames.Length)
			{
				switch (m_OnEnd)
				{
					case Def.VFXEnd.Stop:
						m_Animation.GetTimer().SetEnabled(false);
						OnEnd?.Invoke(this);
						enabled = false;
						return;
					case Def.VFXEnd.SelfDestroy:
						OnEnd?.Invoke(this);
						GameUtils.DeleteGameobject(gameObject);
						return;
					case Def.VFXEnd.Repeat:
						m_CurrentFrame = 0;
						break;
				}
			}
			m_Image.sprite = m_Frames[m_CurrentFrame++];
		}
		public void Restart()
		{
			m_CurrentFrame = 0;
			m_Image.sprite = m_Frames[m_CurrentFrame];
			m_Animation.Restart();
			m_Animation.GetTimer().SetEnabled(true);
			enabled = true;
		}
		private void OnEnable()
		{
			if(m_Image != null)
				m_Image.enabled = true;
		}
		private void OnDisable()
		{
			if (m_Image != null)
				m_Image.enabled = false;
		}
	}
}
