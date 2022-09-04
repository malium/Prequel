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
	public class CLockArrow : MonoBehaviour
	{
		const float YOffset = 0.5f;
		AI.CLivingEntity m_LivingEntity;
		[SerializeField] UnityEngine.UI.Image m_BackImage;
		[SerializeField] ScaleAnimation m_Animation;
		[SerializeField] ScaleAnimationInfo m_EnterAnimation;
		[SerializeField] ScaleAnimationInfo m_ExitAnimation;

		public void Set(AI.CLivingEntity target)
		{
			m_LivingEntity = target;
			m_Animation.enabled = true;
			m_Animation.Set(m_EnterAnimation);
		}
		public void Unset()
		{
			m_LivingEntity = null;
			m_Animation.enabled = true;
			m_Animation.Set(m_ExitAnimation);
		}
		private void Awake()
		{
			var mgr = Manager.Mgr;
			if (mgr != null && mgr.UIAnchor != null)
				transform.SetParent(mgr.UIAnchor.transform);
		}
		public void OnAnimationEnd()
		{
			if (m_LivingEntity != null)
				m_Animation.enabled = false;
			else
				gameObject.SetActive(false);
		}
		private void Start()
		{
			transform.localScale = Vector3.one;
		}
		private void LateUpdate()
		{
			if (m_LivingEntity == null)
				return;

			var cam = CameraManager.Mgr;
			var wPos = m_LivingEntity.transform.position;
			wPos += new Vector3(0f, m_LivingEntity.GetHeight() + YOffset, 0f);
			var sPoint = RectTransformUtility.WorldToScreenPoint(cam.Camera, wPos);
			transform.position = new Vector3(sPoint.x, sPoint.y, 0f);
		}
	}
}
