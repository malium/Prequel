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
	public class CImageSelectorUI : MonoBehaviour
	{
		const int ElementWidth = 160;
		const int ElementHeight = 150;
		const int RowElementCount = 5;

		public UnityEngine.UI.Button CrossButton;
		public UnityEngine.UI.Button SelectAllButton;
		public CImageSelectorElement ElementSingle;
		public CImageSelectorElement ElementMulti;
		public UnityEngine.UI.InputField SearchField;
		public RectTransform PanelRT;
		public RectTransform ViewContent;

		public struct ElementInfo
		{
			public Sprite Image;
			public string Name;

			public override bool Equals(object obj)
			{
				return obj is ElementInfo info &&
					info == this;
			}
			public override int GetHashCode()
			{
				int hashCode = 1856990384;
				hashCode = hashCode * -1521134295 + EqualityComparer<Sprite>.Default.GetHashCode(Image);
				hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
				return hashCode;
			}
			public static bool operator==(ElementInfo left, ElementInfo right)
			{
				if (left.Image != right.Image)
					return false;

				if (left.Name != right.Name)
					return false;

				return true;
			}
			public static bool operator!=(ElementInfo left, ElementInfo right)
			{
				return !(left == right);
			}
		}
		struct Element
		{
			public Sprite Image;
			public string Name;
			public CImageSelectorElement Compnt;
		}

		List<Element> m_ShowingElements;
		List<Element> m_Elements;
		bool m_Multiselection;
		Action m_OnSelectionEnd;
		List<string> m_Selected;

		private void Awake()
		{
			CrossButton.onClick.AddListener(OnSelectionEnd);
			SearchField.onValueChanged.AddListener(OnSearchChange);
			SelectAllButton.onClick.AddListener(OnSelectAll);
		}
		public List<string> GetSelected() => m_Selected;
		public void OnSearchChange(string value)
		{
			if(value.Length == 0)
			{
				m_ShowingElements = new List<Element>(m_Elements);
			}
			else
			{
				var searchLower = value.ToLower();
				m_ShowingElements.Clear();
				for(int i = 0; i < m_Elements.Count; ++i)
				{
					var name = m_Elements[i].Name;
					bool containsAllChars = true;
					var lowerName = name.ToLower();
					for(int j = 0; j < searchLower.Length; ++j)
					{
						var idx = lowerName.IndexOf(searchLower[j]);
						if(idx == -1)
						{
							containsAllChars = false;
							break;
						}
						if (idx == 0)
							lowerName = lowerName.Substring(1);
						else if (idx == (lowerName.Length - 1))
							lowerName = lowerName.Substring(0, lowerName.Length - 1);
						else
							lowerName = lowerName.Substring(0, idx) + lowerName.Substring(idx + 1);
					}
					if(containsAllChars)
					{
						m_ShowingElements.Add(new Element()
						{
							Image = m_Elements[i].Image,
							Name = m_Elements[i].Name,
							Compnt = m_Elements[i].Compnt
						});
					}
					m_Elements[i].Compnt.gameObject.SetActive(containsAllChars);
				}
			}

			int rowIdx = 0;
			int columnIdx = 0;

			int vElements = Mathf.CeilToInt(m_ShowingElements.Count / (float)RowElementCount);
			ViewContent.sizeDelta =
						new Vector2(ViewContent.sizeDelta.x, vElements * ElementHeight);
			for (int i = 0; i < m_ShowingElements.Count; ++i)
			{
				var elem = m_ShowingElements[i].Compnt;
				elem.gameObject.SetActive(true);
				elem.Transform.anchoredPosition =
					new Vector2(columnIdx * ElementWidth, -rowIdx * ElementHeight);
				elem.transform.localScale = new Vector3(1f, 1f, 1f);
				++columnIdx;
				if(columnIdx == RowElementCount)
				{
					++rowIdx;
					columnIdx = 0;
				}
			}
		}
		public void OnSelectAll()
		{
			for(int i = 0; i < m_ShowingElements.Count; ++i)
			{
				var elem = m_ShowingElements[i];
				if (m_Selected.Contains(elem.Name))
					continue;
				elem.Compnt.SelectionToggle.isOn = true;
				OnElementSelected(elem.Name);
			}
		}
		public void Init(List<ElementInfo> elements, bool multiselection, Action onSelectionEnd,
			Def.ImageSelectorPosition position = Def.ImageSelectorPosition.Left)
		{
			m_Elements = new List<Element>(elements.Count);
			m_ShowingElements = new List<Element>(elements.Count);
			SearchField.text = "";
			SelectAllButton.interactable = multiselection;
			m_Multiselection = multiselection;
			m_OnSelectionEnd = onSelectionEnd;
			m_Selected = new List<string>();
			var canvas = CameraManager.Mgr.Canvas;
			switch (position)
			{
				case Def.ImageSelectorPosition.Left:
					PanelRT.anchoredPosition = new Vector2(0f, PanelRT.anchoredPosition.y);
					break;
				case Def.ImageSelectorPosition.Center:
					PanelRT.anchoredPosition = new Vector2(Screen.width / 2 - PanelRT.sizeDelta.x * 0.5f * canvas.scaleFactor, PanelRT.anchoredPosition.y);
					break;
			}
			//PanelRT.ForceUpdateRectTransforms();

			CImageSelectorElement elementToCopy = m_Multiselection ? ElementMulti : ElementSingle;

			//int rowIdx = 0;
			//int columnIdx = 0;
			for(int i = 0; i < elements.Count; ++i)
			{
				var curElem = elements[i];
				var elem = Instantiate(elementToCopy);
				elem.transform.SetParent(elementToCopy.transform.parent);
				m_Elements.Add(new Element()
				{
					Image = curElem.Image,
					Name = curElem.Name,
					Compnt = elem
				});
				//m_ShowingElements.Add(new Element()
				//{
				//	Image = curElem.Image,
				//	Name = curElem.Name,
				//	Compnt = elem
				//});
				elem.gameObject.SetActive(false);
				elem.SetElement(curElem);
				//elem.Transform.anchoredPosition = new Vector2(columnIdx * ElementWidth, -rowIdx * ElementHeight);
				//elem.transform.localScale = new Vector3(1f, 1f, 1f);
				//++columnIdx;
				//if(columnIdx == RowElementCount)
				//{
				//	++rowIdx;
				//	columnIdx = 0;
				//	ViewContent.sizeDelta = new Vector2(ViewContent.sizeDelta.x, rowIdx * ElementHeight);
				//}
			}
			OnSearchChange("");
		}
		public void OnElementSelected(string name)
		{
			if (!m_Selected.Contains(name))
				m_Selected.Add(name);
			if (!m_Multiselection)
				OnSelectionEnd();
		}
		public void OnElementDeselected(string name)
		{
			if (m_Selected.Contains(name))
				m_Selected.Remove(name);
		}
		public void OnSelectionEnd()
		{
			for(int i = 0; i < m_Elements.Count; ++i)
			{
				GameUtils.DeleteGameobject(m_Elements[i].Compnt.gameObject);
			}
			m_Elements.Clear();
			m_OnSelectionEnd();
		}
	}
}