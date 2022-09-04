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
	public class EntityStatusBar : MonoBehaviour
	{
		public CStatBar HealthBar;
		public CStatBar SoulnessBar;

		AI.CLivingEntity m_Entity;
		public void SetTransparency(float transparency)
		{
			if(HealthBar != null)
				HealthBar.SetAlpha(transparency);
			if(SoulnessBar != null)
				SoulnessBar.SetAlpha(transparency);
		}
		public void Init(AI.CLivingEntity entity)
		{
			m_Entity = entity;

			var manager = Manager.Mgr;
			if (manager.UIAnchor != null)
				transform.SetParent(manager.UIAnchor.transform);

			if (HealthBar != null)
			{
				if (m_Entity.GetMaxHealth() <= 0f)
					HealthBar.gameObject.SetActive(false);
				else
					HealthBar.Progress = (m_Entity.GetCurrentHealth() / m_Entity.GetMaxHealth()) * 100f;
			}
			if(SoulnessBar != null)
			{
				if (m_Entity.GetMaxSoulness() <= 0f)
					SoulnessBar.gameObject.SetActive(false);
				else
					SoulnessBar.Progress = (m_Entity.GetCurrentSoulness() / m_Entity.GetMaxSoulness()) * 100f;
			}
		}
		void FixedUpdate()
		{
			if (HealthBar != null && HealthBar.enabled)
				HealthBar.Progress = (m_Entity.GetCurrentHealth() / m_Entity.GetMaxHealth()) * 100f;

			if (SoulnessBar != null && SoulnessBar.enabled)
				SoulnessBar.Progress = (m_Entity.GetCurrentSoulness() / m_Entity.GetMaxSoulness()) * 100f;
		}
		private void LateUpdate()
		{
			
		}
	}
}
