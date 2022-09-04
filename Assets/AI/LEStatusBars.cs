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

namespace Assets.AI
{
	[RequireComponent(typeof(RectTransform))]
	public class LEStatusBars : MonoBehaviour
	{
		public UI.CStatBar HealthBar;
		public UI.CStatBar SoulnessBar;
		CLivingEntity m_LE;
		public void SetAlpha(float alpha)
		{
			HealthBar.SetAlpha(alpha);
			SoulnessBar.SetAlpha(alpha);
		}
		public void Init(CLivingEntity le)
		{
			m_LE = le;

			var manager = Manager.Mgr;
			if (manager.UIAnchor != null)
				transform.SetParent(manager.UIAnchor.transform);

			HealthBar.Progress = 100f;
			SoulnessBar.Progress = 100f;
		}
		private void LateUpdate()
		{
			HealthBar.Progress = (m_LE.GetCurrentHealth() / m_LE.GetMaxHealth()) * 100f;
			SoulnessBar.Progress = (m_LE.GetCurrentSoulness() / m_LE.GetMaxSoulness()) * 100f;

			var cam = CameraManager.Mgr;
			var wPos = m_LE.transform.position;
			wPos += new Vector3(0f, m_LE.GetHeight(), 0f);
			var sPoint = RectTransformUtility.WorldToScreenPoint(cam.Camera, wPos);
			transform.position = new Vector3(sPoint.x, sPoint.y, 0f);
			//var sPos = cam.Camera.WorldToScreenPoint(wPos);
			//var sPoint = new Vector2(sPos.x, sPos.y);
			//var gPoint = GUIUtility.ScreenToGUIPoint(sPoint);
			//transform.position = new Vector3(gPoint.x, gPoint.y, 0f);

			//var posY = transform.position.y;
			//var camPos = CameraManager.Mgr.transform.position;
			//transform.LookAt(new Vector3(camPos.x, posY + 1f, camPos.z));
			//transform.Rotate(Vector3.up, 180f);
		}
	}
}
