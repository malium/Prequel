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
	public enum FogPlacement
	{
		TOP_LEFT,
		TOP_CENTER,
		TOP_RIGHT,
		CENTER_LEFT,
		CENTER_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_CENTER,
		BOTTOM_RIGHT,
		COUNT
	}
	public class FogManager : MonoBehaviour
	{
		public float Radius;
		public float CutRadius;
		float m_CurrentRadius;
		[SerializeField]
		FogComponent[] m_Fogs;

		//OddScript Odd;

		private void OnEnable()
		{
			//if (Odd == null)
			//	return;
			for (int i = 0; i < m_Fogs.Length; ++i)
			{
				m_Fogs[i].gameObject.SetActive(true);
				m_Fogs[i].enabled = true;
			}
		}
		private void OnDisable()
		{
			//if (Odd == null)
			//	return;
			for (int i = 0; i < m_Fogs.Length; ++i)
			{
				m_Fogs[i].enabled = false;
				m_Fogs[i].gameObject.SetActive(false);
			}
		}

		private void Start()
		{
			//Odd = Manager.Mgr.OddGO.GetComponent<OddScript>();
			// Create child fogs
			m_Fogs = new FogComponent[8]
			{
				new GameObject("Fog_TL").AddComponent<FogComponent>(),
				new GameObject("Fog_TC").AddComponent<FogComponent>(),
				new GameObject("Fog_TR").AddComponent<FogComponent>(),
				new GameObject("Fog_CL").AddComponent<FogComponent>(),
				new GameObject("Fog_CR").AddComponent<FogComponent>(),
				new GameObject("Fog_BL").AddComponent<FogComponent>(),
				new GameObject("Fog_BC").AddComponent<FogComponent>(),
				new GameObject("Fog_BR").AddComponent<FogComponent>()
			};
			Radius = 26.0f;
			m_CurrentRadius = Radius;
			CutRadius = 25.0f;
			for (int i = 0; i < m_Fogs.Length; ++i)
			{
				m_Fogs[i].transform.SetParent(transform);
				m_Fogs[i].SetFog(this, (FogPlacement)i);
				var posOffset = m_Fogs[i].Direction * Radius;
				var pos = transform.position + new Vector3(posOffset.x, 0.0f, posOffset.y);
				var offset = pos - transform.position;
				m_Fogs[i].transform.Translate(offset, Space.World);
			}
		}

		public void Update()
		{
			var pos = transform.position;
			//var oddPos = Odd.Position;
			//oddPos.y += 2.0f;
			//var diff = oddPos - pos;
			if (m_CurrentRadius != Radius)
			{
				for (int i = 0; i < m_Fogs.Length; ++i)
				{
					//var posOffsetOld = m_Fogs[i].Direction * m_CurrentRadius;
					var posNew = m_Fogs[i].Direction * Radius; // + new Vector2(transform.position.x, transform.position.z);
					m_Fogs[i].transform.localPosition = new Vector3(posNew.x, 0.0f, posNew.y);
					//var offset = posOffsetNew - new Vector2(m_Fogs[i].transform.position.x, m_Fogs[i].transform.position.z);
					//m_Fogs[i].transform.Translate(new Vector3(offset.x, 0.0f, offset.y), Space.World);
				}
				m_CurrentRadius = Radius;
			}
			//transform.Translate(diff, Space.World);
			transform.Rotate(Vector3.up, Time.deltaTime * 15.0f, Space.World);
		}
	}
}
