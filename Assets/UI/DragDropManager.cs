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
	public static class DragDropManager
	{
		static List<CDropRegion> m_Regions;
		public static void RegisterRegion(CDropRegion region)
		{
			if (m_Regions == null)
				m_Regions = new List<CDropRegion>(1);

			if (m_Regions.Contains(region))
				return;
			m_Regions.Add(region);
		}
		public static void UnregisterRegion(CDropRegion region)
		{
			if (m_Regions == null || !m_Regions.Contains(region))
				return;
			m_Regions.Remove(region);
		}
		public static CDropRegion GetClosestRegion(CDraggable draggable)
		{
			if(m_Regions == null || m_Regions.Count == 0)
				return null;

			var draggablePos = new Vector2(draggable.transform.position.x, draggable.transform.position.y);
			//Debug.Log("DraggablePos " + draggablePos.ToString());
			CDropRegion closest = null;
			float distance = float.MaxValue;

			for(int i = 0; i < m_Regions.Count; ++i)
			{
				var region = m_Regions[i];
				var regionRT = region.GetRT();
				var regionPos = new Vector2(region.transform.position.x, region.transform.position.y);
				var size = regionRT.sizeDelta;
				if (regionRT.rect.x < 0)
					regionPos.Set(regionPos.x - size.x, regionPos.y);
				if (regionRT.rect.y < 0)
					regionPos.Set(regionPos.x, regionPos.y - size.y);
				var rect = new Rect(regionPos, size);
				if (!rect.Contains(draggablePos))
					continue; // Draggable outside

				var center = regionPos + regionRT.sizeDelta * 0.5f;

				var dist = Vector2.Distance(draggablePos, center);
				if(dist < distance)
				{
					closest = region;
					distance = dist;
				}
			}

			return closest;
		}
	}
}
