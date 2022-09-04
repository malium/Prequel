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
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Assets.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class CDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		public UnityEvent DragBegin;
		public UnityEvent<CDropRegion> DragEnd;


		CDropRegion m_PrevRegion;
		public void OnBeginDrag(PointerEventData eventData)
		{
			DragBegin?.Invoke();

			transform.position = new Vector3(eventData.position.x, eventData.position.y, transform.position.z);
			m_PrevRegion = null;
		}
		public void OnDrag(PointerEventData eventData)
		{
			transform.position = new Vector3(eventData.position.x, eventData.position.y, transform.position.z);

			var region = DragDropManager.GetClosestRegion(this);
			
			if (m_PrevRegion != region && m_PrevRegion != null)
				m_PrevRegion.DraggableNotOnTop();

			if (region != null)
				region.DraggableIsOnTop(this);

			m_PrevRegion = region;
		}
		public void OnEndDrag(PointerEventData eventData)
		{
			DragEnd?.Invoke(m_PrevRegion);

			if (m_PrevRegion != null)
				m_PrevRegion.DraggableIsDropped(this);

			m_PrevRegion = null;
		}
		
	}
}
