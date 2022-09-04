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
	public class SelectorUI : MonoBehaviour
	{
		public CViewB ItemView;
		public UnityEngine.UI.Button CrossButton;
		public TMPro.TMP_Text Title;
		public UnityEngine.UI.Button SelectAllButton;
		public TMPro.TMP_InputField SearchIF;

		bool m_Multiselection;
		List<ViewElementInfoB> m_TempViewElements;
		List<ViewElementBSelectableInfo> m_Selected;
		List<ViewElementBSelectableInfo> m_Elements;
		List<ViewElementBSelectableInfo> m_ShowingElements;

		static SelectorUI Instance;
		public static SelectorUI GetInstance()
		{
			if(Instance == null)
			{
				Instance = Resources.Load<SelectorUI>("UI/SelectorUI");
				if (Instance == null)
					throw new Exception("Couldn't load SelectorUI");
			}
			return Instance;
		}
		private void Awake()
		{
			m_Multiselection = false;
			SearchIF.onValueChanged.AddListener(OnSearch);
			SelectAllButton.onClick.AddListener(OnSelectAll);
			m_TempViewElements = new List<ViewElementInfoB>();
			m_Selected = new List<ViewElementBSelectableInfo>();
			m_Elements = new List<ViewElementBSelectableInfo>();
			m_ShowingElements = new List<ViewElementBSelectableInfo>();

			CrossButton.onClick.AddListener(OnCloseButton);
		}
		public void Init(string title, CViewElementBSelectable defaultItem, List<ViewElementBSelectableInfo> elements, bool multiselection)
		{
			if (Title != null)
				Title.text = title;

			ItemView.Init(defaultItem);
			m_Multiselection = multiselection;
			
			m_Elements.AddRange(elements);

			if (SearchIF.text.Length > 0)
				SearchIF.text = "";
			else
				OnSearch("");

			SelectAllButton.interactable = m_Multiselection;
		}
		public void UnselectItem(string name)
		{
			var idx = m_Selected.FindIndex((ViewElementBSelectableInfo sel) => sel.Name == name);
			if (idx >= 0 && idx < m_Selected.Count)
			{
				var elem = m_Selected[idx];
				elem.Selected = false;
				m_Selected.RemoveAt(idx);
			}
		}
		public void SelectItem(string name)
		{
			var idx = m_Selected.FindIndex((ViewElementBSelectableInfo sel) => sel.Name == name);
			bool added = false;
			if (idx < 0 || idx >= m_Selected.Count) // Wasn't selected, add to selected
			{
				idx = m_ShowingElements.FindIndex((ViewElementBSelectableInfo sel) => sel.Name == name);
				if(idx >= 0 && idx < m_ShowingElements.Count)
				{
					var elem = m_ShowingElements[idx];
					elem.Selected = true;
					m_Selected.Add(elem);
					added = true;
				}
			}
			if (!added)
				return;

			if (!m_Multiselection)
			{
				OnSelection(m_Selected);
				OnCloseButton();
			}
		}
		void OnCloseButton()
		{
			if (m_Multiselection)
				OnSelection(m_Selected);

			OnClose();
			ItemView.Clear();
			m_Elements.Clear();
			m_ShowingElements.Clear();
			m_Selected.Clear();
		}
		IEnumerator NextFrame(Action fn)
		{
			yield return null;

			fn();
		}
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				StartCoroutine(NextFrame(OnCloseButton));
		}
		public bool IsMultiselection() => m_Multiselection;
		public List<ViewElementBSelectableInfo> GetSelected() => m_Selected;
		void OnSearch(string value)
		{
			m_ShowingElements.Clear();
			ItemView.Clear();
			if (value.Length == 0)
			{
				m_ShowingElements.AddRange(m_Elements);
			}
			else
			{
				var searchLower = value.ToLower();
				for (int i = 0; i < m_Elements.Count; ++i)
				{
					var name = m_Elements[i].Name;
					bool containsAllChars = true;
					var lowerName = name.ToLower();
					for (int j = 0; j < searchLower.Length; ++j)
					{
						var idx = lowerName.IndexOf(searchLower[j]);
						if (idx == -1)
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
					if (containsAllChars)
						m_ShowingElements.Add(m_Elements[i]);
				}
			}

			m_TempViewElements.Clear();
			if (m_TempViewElements.Capacity < m_ShowingElements.Count)
				m_TempViewElements.Capacity = m_ShowingElements.Count;
			for (int i = 0; i < m_ShowingElements.Count; ++i)
				m_TempViewElements.Add(m_ShowingElements[i]);
			
			ItemView.AddElements(m_TempViewElements);
		}
		void OnSelectAll()
		{
			for(int i = 0; i < m_ShowingElements.Count; ++i)
			{
				var elem = m_ShowingElements[i];
				
				// Was already added
				var idx = m_Selected.FindIndex((ViewElementBSelectableInfo ve) => ve.Name == elem.Name);
				if (idx >= 0)
					continue; // Already added

				// Is in the view
				idx = ItemView.GetElements().FindIndex((CViewElementB ve) => (ve as CViewElementBItemSelectable).GetName() == elem.Name);
				if (idx < 0)
					continue; // Couldn't find it

				// Activate the toggle
				var selectable = ItemView.GetElements()[idx] as CViewElementBItemSelectable;
				selectable.SelectToggle.isOn = true; // should trigger SelectItem
			}
		}

		public event OnSelectionCB OnSelection;
		public event OnCloseCB OnClose;

		public delegate void OnCloseCB();
		public delegate void OnSelectionCB(List<ViewElementBSelectableInfo> selection);
	}
}
