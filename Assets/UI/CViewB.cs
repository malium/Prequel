/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI
{
	[RequireComponent(typeof(UnityEngine.UI.ScrollRect))]
	public class CViewB : MonoBehaviour
	{
		[SerializeReference] CViewElementB m_DefaultElement;
		[SerializeField] protected List<CViewElementB> m_Elements;
		RectTransform m_ContentRT;
		RectTransform m_ViewRT;
		[SerializeField] bool m_Vertical;
		Indexing m_IndexingFn;

		public event OnElementRemoveCB OnElementRemovePrev;
		public event OnElementRemoveCB OnElementRemovePost;

		void SetContentSize()
		{
			float maxX = 0f;
			float maxY = 0f;
			for(int i = 0; i < m_Elements.Count; ++i)
			{
				var elemRT = m_Elements[i].GetRT();
				var elemPos = elemRT.anchoredPosition;
				var elemHeight = Mathf.Abs(elemRT.sizeDelta.y);
				var elemWidth = Mathf.Abs(elemRT.sizeDelta.x);

				var endPos = new Vector2(Mathf.Abs(elemPos.x) + elemWidth, Mathf.Abs(elemPos.y) + elemHeight);
				if (maxX < endPos.x)
					maxX = endPos.x;
				if (maxY < endPos.y)
					maxY = endPos.y;
			}
			if (m_Vertical)
				maxX = 0;
			else
				maxY = 0;

			m_ContentRT.sizeDelta = new Vector2(maxX, maxY);
		}
		void VerticalIndexing(ref float currentWidth, ref float currentHeight, ref float maxRowHeight, ref float maxColumnWidth, float elemWidth, float elemHeight, ref Vector2 elemPos)
		{
			if(m_ContentRT.rect.width < (currentWidth + elemWidth)) // nextRow
			{
				currentWidth = 0f;
				currentHeight += maxRowHeight;
				maxRowHeight = 0f;
				elemPos.Set(currentWidth, currentHeight);
				maxRowHeight = Mathf.Max(maxRowHeight, elemHeight);
				currentWidth += elemWidth;
			}
			else // currentRow
			{
				elemPos.Set(currentWidth, currentHeight);
				currentWidth += elemWidth;
				maxRowHeight = Mathf.Max(maxRowHeight, elemHeight);
			}
		}
		void HorizontalIndexing(ref float currentWidth, ref float currentHeight, ref float maxRowHeight, ref float maxColumnWidth, float elemWidth, float elemHeight, ref Vector2 elemPos)
		{
			if (m_ContentRT.rect.width < (currentWidth + elemWidth)) // nextColumn
			{
				currentHeight = 0f;
				currentWidth += maxColumnWidth;
				maxColumnWidth = 0f;
				elemPos.Set(currentWidth, currentHeight);
				maxColumnWidth = Mathf.Max(maxColumnWidth, elemWidth);
				currentHeight += elemHeight;
			}
			else // currentColumn
			{
				elemPos.Set(currentWidth, currentHeight);
				currentHeight += elemHeight;
				maxColumnWidth = Mathf.Max(maxColumnWidth, elemWidth);
			}
		}
		delegate void Indexing(ref float currentWidth, ref float currentHeight, ref float maxRowHeight, ref float maxColumnWidth, float elemWidth, float elemHeight, ref Vector2 elemPos);
		protected void SetElements()
		{
			float currentWidth = 0f, currentHeight = 0f, maxRowHeight = 0f, maxColumWidth = 0f;
			var elemPos = new Vector2();
			for (int i = 0; i < m_Elements.Count; ++i)
			{
				var elem = m_Elements[i];
				var elemHeight = Mathf.Abs(elem.GetRT().sizeDelta.y);
				var elemWidth = Mathf.Abs(elem.GetRT().sizeDelta.x);

				m_IndexingFn(ref currentWidth, ref currentHeight, ref maxRowHeight, ref maxColumWidth, elemWidth, elemHeight, ref elemPos);
				elem.GetRT().anchoredPosition = new Vector2(elemPos.x, -elemPos.y);
				elem.transform.localScale = Vector3.one;
			}
			SetContentSize();
		}
		private void Awake()
		{
			m_Elements = new List<CViewElementB>();

			m_ViewRT = gameObject.GetComponent<RectTransform>();

			var sr = gameObject.GetComponent<UnityEngine.UI.ScrollRect>();
			if (sr.vertical)
			{
				m_IndexingFn = VerticalIndexing;
				m_Vertical = true;
			}
			else if (sr.horizontal)
			{
				m_IndexingFn = HorizontalIndexing;
				m_Vertical = false;
			}
			else // fallback
			{
				m_IndexingFn = VerticalIndexing;
				m_Vertical = true;
			}

			var viewport = GameUtils.FindChild(gameObject, "Viewport");

			if (viewport == null)
				throw new Exception("Couldn't find the Viewport GameObject inside the ScrollRect " + gameObject.name);

			var content = GameUtils.FindChild(viewport, "Content");
			if (content == null)
				throw new Exception("Couldn't find the Content GameObject inside the ScrollRect " + gameObject.name);

			m_ContentRT = content.GetComponent<RectTransform>();

			if (m_ContentRT == null)
				throw new Exception("Content GameObject does not have a RectTransform component, something is wrong.");
		}
		public void Init(CViewElementB defaultElement)
		{
			Clear();

			m_DefaultElement = defaultElement;

			if (m_DefaultElement == null)
				throw new Exception("View without a default element!");

			var defaultRT = m_DefaultElement.gameObject.GetComponent<RectTransform>();
			if (defaultRT == null)
				throw new Exception("CView DefaultElement does not have RectTransform component, something is wrong.");

			//m_ElementWidth = defaultRT.rect.width;
			//m_ElementHeight = defaultRT.rect.height;

			//float contentWidth = m_ContentRT.rect.width;
			//float contentHeight = m_ContentRT.rect.height;

			//GameObject scrollBar;
			//if (m_Vertical)
			//	scrollBar = GameUtils.FindChild(gameObject, "Scrollbar Vertical");
			//else
			//	scrollBar = GameUtils.FindChild(gameObject, "Scrollbar Horizontal");

			//float scrollbarOffset = 0f;
			//if (scrollBar)
			//{
			//	var sbRT = scrollBar.GetComponent<RectTransform>();
			//	if (sbRT == null)
			//		throw new Exception("Found a ScrollBar without a RectTransform component");

			//	if (m_Vertical)
			//		scrollbarOffset = sbRT.rect.width;
			//	else
			//		scrollbarOffset = sbRT.rect.height;
			//}

			//if (m_Vertical)
			//{
			//	//contentWidth -= scrollbarOffset;
			//	if (m_ElementWidth >= contentWidth)
			//		m_MaxElementsInRow = 1;
			//	else
			//		m_MaxElementsInRow = Mathf.FloorToInt(contentWidth / m_ElementWidth);
			//}
			//else
			//{
			//	contentHeight -= scrollbarOffset;
			//	if (m_ElementHeight >= contentHeight)
			//		m_MaxElementsInColumn = 1;
			//	else
			//		m_MaxElementsInColumn = Mathf.FloorToInt(contentHeight / m_ElementHeight);
			//}

			SetContentSize();
		}
		private void OnEnable()
		{
			_Enable(true);
		}
		private void OnDisable()
		{
			_Enable(false);
		}
		public List<CViewElementB> GetElements() => m_Elements;
		public void ScrollToBeginnig()
		{
			if (m_Vertical)
			{
				m_ContentRT.anchoredPosition = new Vector2(m_ContentRT.anchoredPosition.x, 0f);
			}
			else
			{
				m_ContentRT.anchoredPosition = new Vector2(0f, m_ContentRT.anchoredPosition.y);
			}
		}
		public void ScrollTo(float pct)
		{
			if (m_Vertical)
			{
				float scroll = m_ContentRT.sizeDelta.y - m_ViewRT.sizeDelta.y;
				scroll = Mathf.Max(0f, scroll);
				m_ContentRT.anchoredPosition = new Vector2(m_ContentRT.anchoredPosition.x, scroll * pct);
			}
			else
			{
				float scroll = m_ContentRT.sizeDelta.x - m_ViewRT.sizeDelta.x;
				scroll = Mathf.Max(0f, scroll);
				m_ContentRT.anchoredPosition = new Vector2(-scroll * pct, m_ContentRT.anchoredPosition.y);
			}
		}
		public void ScrollToEnding()
		{
			if (m_Vertical)
			{
				float scroll = m_ContentRT.sizeDelta.y - m_ViewRT.sizeDelta.y;
				scroll = Mathf.Max(0f, scroll);
				m_ContentRT.anchoredPosition = new Vector2(m_ContentRT.anchoredPosition.x, scroll);
			}
			else
			{
				float scroll = m_ContentRT.sizeDelta.x - m_ViewRT.sizeDelta.x;
				scroll = Mathf.Max(0f, scroll);
				m_ContentRT.anchoredPosition = new Vector2(scroll, m_ContentRT.anchoredPosition.y);
			}
		}
		public void Clear()
		{
			for (int i = 0; i < m_Elements.Count; ++i)
				GameObject.Destroy(m_Elements[i].gameObject);
			m_Elements.Clear();
			SetContentSize();
		}
		void _Enable(bool enable)
		{
			for (int i = 0; i < m_Elements.Count; ++i)
				m_Elements[i].enabled = enable;
		}
		IEnumerator NextFrame(Action fn)
		{
			yield return null;

			fn();
		}
		public virtual void AddElement(ViewElementInfoB info)
		{
			if (m_DefaultElement == null)
				throw new Exception("Trying to add an element to a CView but was not initialized.");
			var elem = Instantiate(m_DefaultElement);
			elem.gameObject.SetActive(true);
			elem.transform.SetParent(m_ContentRT.transform);
			elem.ElementInit(this, info);
			m_Elements.Add(elem);
			StartCoroutine(NextFrame(SetElements));
		}
		public virtual void AddElements(List<ViewElementInfoB> infos)
		{
			if (m_DefaultElement == null)
				throw new Exception("Trying to add an element to a CView but was not initialized.");

			if (infos == null)
				return;
			for (int i = 0; i < infos.Count; ++i)
			{
				var elem = Instantiate(m_DefaultElement);
				elem.gameObject.SetActive(true);
				elem.transform.SetParent(m_ContentRT.transform);
				elem.ElementInit(this, infos[i]);
				m_Elements.Add(elem);
			}
			StartCoroutine(NextFrame(SetElements));// avoid reorder each iteration, reorder at end
			//SetElements(); 
		}
		public virtual void RemoveElement(CViewElementB element)
		{
			var index = m_Elements.IndexOf(element);
			if (index < 0)
			{
				Debug.LogWarning("Trying to remove a non-registered CViewElement.");
				GameObject.Destroy(element.gameObject);
				return;
			}

			OnElementRemovePrev?.Invoke(element);
			m_Elements.RemoveAt(index);
			OnElementRemovePost?.Invoke(element);
			GameObject.Destroy(element.gameObject);
			SetElements();
		}

		public delegate void OnElementRemoveCB(CViewElementB element);
	}
}
