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
	[RequireComponent(typeof(Collider))]
	class ColliderEventTriggerer : MonoBehaviour
	{
		Collider m_Collider;

		public UnityEvent<Collider, Collider> TriggerEnter;
		public UnityEvent<Collider, Collider> TriggerStay;
		public UnityEvent<Collider, Collider> TriggerExit;

		private void Awake()
		{
			m_Collider = gameObject.GetComponent<Collider>();
		}
		private void OnTriggerEnter(Collider other)
		{
			Debug.Log("Enter " + other.name);
			TriggerEnter?.Invoke(m_Collider, other);
		}
		private void OnTriggerStay(Collider other)
		{
			Debug.Log("Stay " + other.name);
			TriggerStay?.Invoke(m_Collider, other);
		}
		private void OnTriggerExit(Collider other)
		{
			Debug.Log("Exit " + other.name);
			TriggerExit?.Invoke(m_Collider, other);
		}
	}
}
