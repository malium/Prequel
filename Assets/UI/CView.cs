/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI
{
	public class CView : MonoBehaviour
	{
		public CViewElement DefaultElement;
		public Vector2 DefaultElementSize;
		public int RowElementCount;
		public RectTransform ViewContent;
		List<CViewElement> m_Elements;

		Action<CViewElement> m_OnElementRemove;

		private void Awake()
		{
			m_Elements = new List<CViewElement>();
			if(ViewContent == null)
				ViewContent = gameObject.GetComponent<RectTransform>();
		}
		public void SetOnElementRemove(Action<CViewElement> onElementRemove) => m_OnElementRemove = onElementRemove;
		void SetElements()
		{
			if (RowElementCount < 1)
				RowElementCount = 1;
			int rowIdx = 0;
			int colIdx = 0;

			int rows = Mathf.CeilToInt(m_Elements.Count / (float)RowElementCount);

			ViewContent.sizeDelta = new Vector2(ViewContent.sizeDelta.x, rows * DefaultElementSize.y);
			for(int i = 0; i < m_Elements.Count; ++i)
			{
				var elem = m_Elements[i];
				if(colIdx == RowElementCount)
				{
					++rowIdx;
					colIdx = 0;
					//ViewContent.sizeDelta = new Vector2(ViewContent.sizeDelta.x,
					//	(1 + rowIdx) * DefaultElementSize.y);
				}
				elem.Transform.anchoredPosition = new Vector2(colIdx * DefaultElementSize.x,
					-rowIdx * DefaultElementSize.y);
				elem.transform.localScale = Vector3.one;
				++colIdx;
			}
		}
		public CViewElement AddElement(ViewElementInfo info)
		{
			var elem = Instantiate(DefaultElement);
			elem.gameObject.SetActive(true);
			elem.transform.SetParent(DefaultElement.transform.parent);
			elem.ElementInit(info, this);
			m_Elements.Add(elem);
			SetElements();
			return elem;
		}
		public void AddElement(List<ViewElementInfo> infos)
		{
			for(int i = 0; i < infos.Count; ++i)
			{
				var elem = Instantiate(DefaultElement);
				elem.gameObject.SetActive(true);
				elem.transform.SetParent(DefaultElement.transform.parent);
				elem.ElementInit(infos[i], this);
				m_Elements.Add(elem);
			}
			SetElements();
		}
		public void _RemoveElement(CViewElement obj)
		{
			bool found = false;
			string objName = "";
			if (obj.NameText != null)
				objName = obj.NameText.text;
			if (obj.TMPNameText != null)
				objName = obj.TMPNameText.text;
			for (int i = 0; i < m_Elements.Count; ++i)
			{
				var elem = m_Elements[i];
				if (elem == obj)
				{
					found = true;
					m_Elements.RemoveAt(i);
					
					break;
				}
			}
			if (!found)
				return;

			m_OnElementRemove?.Invoke(obj);
			GameUtils.DeleteGameobject(obj.gameObject);
			SetElements();
		}
		//public void RemoveElement(string elementName)
		//{
		//	bool found = false;
		//	for(int i = 0; i < m_Elements.Count; ++i)
		//	{
		//		var elem = m_Elements[i];
		//		if (elem.NameText.text == elementName)
		//		{
		//			found = true;
		//			m_Elements.RemoveAt(i);
		//			GameUtils.DeleteGameobject(elem.gameObject);
		//			break;
		//		}
		//	}
		//	if (!found)
		//		return;

		//	SetElements();

		//	m_OnElementRemove?.Invoke(elementName);
		//}
		public void Clear()
		{
			for (int i = 0; i < m_Elements.Count; ++i)
				GameUtils.DeleteGameobject(m_Elements[i].gameObject);
			m_Elements.Clear();
		}
		public virtual void Enable(bool enable)
		{
			for (int i = 0; i < m_Elements.Count; ++i)
				m_Elements[i].enabled = enable;
		}
		public List<CViewElement> GetElements() => m_Elements;
		private void OnEnable()
		{
			Enable(true);
		}
		private void OnDisable()
		{
			Enable(false);
		}
	}
}