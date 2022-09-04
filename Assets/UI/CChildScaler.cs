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
	public class CChildScaler : MonoBehaviour
	{
		RectTransform m_RT;
		public bool _Update;
		public bool X = true;
		public bool Y = true;
		private void OnValidate()
		{
			_Update = false;

			UpdateTransform();
		}
		public void UpdateTransform()
		{
			if (m_RT == null)
				m_RT = gameObject.GetComponent<RectTransform>();

			var minPos = new Vector2(float.MaxValue, float.MaxValue);
			var maxPos = new Vector2(float.MinValue, float.MinValue);
			if(!X)
			{
				minPos.Set(transform.position.x, minPos.y);
				maxPos.Set(transform.position.x + m_RT.sizeDelta.x, maxPos.y);
			}
			if(!Y)
			{
				minPos.Set(minPos.x, transform.position.y);
				maxPos.Set(maxPos.y, transform.position.y + m_RT.sizeDelta.y);
			}
			for (int i = 0; i < transform.childCount; ++i)
			{
				var childTrfm = transform.GetChild(i);
				if (!childTrfm.gameObject.activeSelf)
					continue;

				var child = childTrfm.gameObject.GetComponent<RectTransform>();
				if (child == null)
					continue;

				if (X && childTrfm.position.x < minPos.x)
					minPos.Set(childTrfm.position.x, minPos.y);

				if (Y && childTrfm.position.y < minPos.y)
					minPos.Set(minPos.x, childTrfm.position.y);

				var fPos = new Vector2(childTrfm.position.x + child.sizeDelta.x, childTrfm.position.y + child.sizeDelta.y);

				if (X && fPos.x > maxPos.x)
					maxPos.Set(fPos.x, maxPos.y);

				if (Y && fPos.y > maxPos.y)
					maxPos.Set(maxPos.x, fPos.y);
			}

			if (minPos.x == float.MaxValue || minPos.y == float.MaxValue)
				minPos = new Vector2(transform.position.x, transform.position.y);
			if (maxPos.x == float.MinValue || maxPos.y == float.MinValue)
				maxPos = minPos + m_RT.sizeDelta;

			transform.position = new Vector3(minPos.x, minPos.y, transform.position.z);
			m_RT.sizeDelta = maxPos - minPos;
		}
	}
}
