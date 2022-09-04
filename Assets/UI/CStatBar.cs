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
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Prequel/StatusBar")]
	[DisallowMultipleComponent]
	public class CStatBar : MonoBehaviour
	{
		const float Delay = 0.5f;
		public UnityEngine.UI.Extensions.Gradient2 Background;
		public UnityEngine.UI.Extensions.Gradient2 Effect;
		public UnityEngine.UI.Extensions.Gradient2 Value;
		public TMPro.TMP_Text ValueText;
		public float EffectSpeed = 1f;
		[Range(0, 100)]
		public float Progress;
		RectTransform m_Transform;
		RectTransform m_BackgroundRT;
		RectTransform m_EffectRT;
		RectTransform m_ValueRT;
		float m_DamageTime;

		UnityEngine.UI.Image BackgroundImage;
		UnityEngine.UI.Image EffectImage;
		UnityEngine.UI.Image ValueImage;
		public void SetAlpha(float alpha)
		{
			if(alpha == 1f)
			{
				if (Background != null)
					Background.gameObject.SetActive(true);
			}
			else
			{
				if(Background != null && Background.isActiveAndEnabled)
				{
					Background.gameObject.SetActive(false);
				}
			}

			void SetImage(UnityEngine.UI.Image image)
			{
				if (image == null)
					return;
				var color = image.color;
				color.a = alpha;
				image.color = color;
			}
			SetImage(BackgroundImage);
			SetImage(EffectImage);
			SetImage(ValueImage);
		}
		void GetRTs()
		{
			if (m_Transform == null)
				m_Transform = gameObject.GetComponent<RectTransform>();
			if (m_BackgroundRT == null)
				m_BackgroundRT = Background.GetComponent<RectTransform>();
			if (Effect != null && m_EffectRT == null)
				m_EffectRT = Effect.GetComponent<RectTransform>();
			if (Value != null && m_ValueRT == null)
				m_ValueRT = Value.GetComponent<RectTransform>();
		}
		private void OnValidate()
		{
			if (UnityEngine.Application.isPlaying)
				return;
			GetRTs();

			var width = m_BackgroundRT.sizeDelta.x;
			width *= (Progress * 0.01f);
			if (m_EffectRT != null)
				m_EffectRT.sizeDelta = new Vector2(width, m_EffectRT.sizeDelta.y);
			if (m_ValueRT != null)
				m_ValueRT.sizeDelta = new Vector2(width, m_ValueRT.sizeDelta.y);
			UpdateText();
		}
		void UpdateText()
		{
			if (ValueText == null)
				return;
			if (this.Progress > 99f && this.Progress < 100f)
				ValueText.text = "99%";
			else if (this.Progress > 0f && this.Progress < 1f)
				ValueText.text = "1%";
			else
				ValueText.text = ((int)this.Progress).ToString() + '%';
		}
		private void Awake()
		{
			GetRTs();
			if (Background != null)
				BackgroundImage = Background.gameObject.GetComponent<UnityEngine.UI.Image>();
			if (Effect != null)
				EffectImage = Effect.gameObject.GetComponent<UnityEngine.UI.Image>();
			if (Value != null)
				ValueImage = Value.gameObject.GetComponent<UnityEngine.UI.Image>();
		}
		private void Update()
		{
			var wantedWidth = m_BackgroundRT.sizeDelta.x;
			var speed = wantedWidth * EffectSpeed;
			wantedWidth *= (Progress * 0.01f);
			if(wantedWidth < m_ValueRT.sizeDelta.x)
			{
				m_DamageTime = Time.time + Delay;
			}
			m_ValueRT.sizeDelta = new Vector2(wantedWidth, m_ValueRT.sizeDelta.y);
			if (m_EffectRT != null && m_DamageTime < Time.time)
			{
				var effectWidth = m_EffectRT.sizeDelta.x;
				if (wantedWidth < effectWidth)
				{
					effectWidth -= speed * Time.deltaTime;
					if (effectWidth < wantedWidth)
						effectWidth = wantedWidth;
				}
				else
				{
					effectWidth = wantedWidth;
				}
				m_EffectRT.sizeDelta = new Vector2(effectWidth, m_EffectRT.sizeDelta.y);
			}
		}
		private void FixedUpdate()
		{
			UpdateText();
		}
	}
}
