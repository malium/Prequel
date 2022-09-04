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

namespace Assets.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class CDropRegion : MonoBehaviour
	{
		RectTransform m_RT;

		public UnityEvent<CDraggable> OnDrop;
		public UnityEvent<CDraggable> OnDraggableAbove;
		public UnityEvent OnDraggableNotAbove;
		bool m_WasDraggableOnTop;
		
		private void Awake()
		{
			m_RT = gameObject.GetComponent<RectTransform>();
			m_WasDraggableOnTop = false;
		}
		public void DraggableIsDropped(CDraggable draggable)
		{
			OnDrop?.Invoke(draggable);

			DraggableNotOnTop();
		}
		public void DraggableIsOnTop(CDraggable draggable)
		{
			if (!m_WasDraggableOnTop)
			{
				OnDraggableAbove?.Invoke(draggable);
				m_WasDraggableOnTop = true;
			}
		}
		public void DraggableNotOnTop()
		{
			if (m_WasDraggableOnTop)
				OnDraggableNotAbove?.Invoke();

			m_WasDraggableOnTop = false;
		}
		private void OnEnable()
		{
			DragDropManager.RegisterRegion(this);
		}
		private void OnDisable()
		{
			DragDropManager.UnregisterRegion(this);
		}
		public RectTransform GetRT() => m_RT;
	}
}
