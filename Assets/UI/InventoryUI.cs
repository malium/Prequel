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
	public class InventoryUI : MonoBehaviour
	{
		public UnityEngine.UI.Button CrossButton;
		public UnityEngine.UI.Button AddItemButton;

		public UnityEngine.UI.Button[] StateButtons;
		public UnityEngine.UI.Image[] StateSelectionImages;

		public UnityEngine.UI.Button FilterButton;
		public UnityEngine.UI.Button AscDscButton;

		public Sprite FilterAlphabeticImage;
		public Sprite FilterCategoryImage;

		public TMPro.TMP_InputField SearchIF;

		public CViewSorted ItemView;
		public CQuickItems QuickItems;
		bool m_SearchIFSelected;

		public event OnCloseCB OnClose;

		static SelectorUI Selector;

		static InventoryUI Instance;
		public static InventoryUI GetInstance()
		{
			if (Instance == null)
			{
				Instance = Resources.Load<InventoryUI>("UI/InventoryUI");
				if (Instance == null)
					throw new Exception("Couldn't load InventoryUI");
			}
			return Instance;
		}

		enum InventoryState
		{
			Equipment,
			Consumable,
			Resources,
			Quest,

			COUNT
		}
		const int InventoryStateCount = (int)InventoryState.COUNT;
		enum SortState
		{
			Alphabetic,
			Category,
			Search,
		}

		SortState m_SortState;
		bool m_SortAsc;
		InventoryState m_State;
		AI.CSpellCaster m_SpellCaster;
		UnityEngine.Events.UnityAction[] OnStateButton;

		List<ViewElementInfoB> m_AllItems;
		List<ViewElementInfoB> m_ShowingItems;


		private void Awake()
		{
			OnStateButton = new UnityEngine.Events.UnityAction[InventoryStateCount]
			{
				() => ChangeInventoryState(InventoryState.Equipment),
				() => ChangeInventoryState(InventoryState.Consumable),
				() => ChangeInventoryState(InventoryState.Resources),
				() => ChangeInventoryState(InventoryState.Quest)
			};
			for (int i = 0; i < StateButtons.Length; ++i)
				StateButtons[i].onClick.AddListener(OnStateButton[i]);

			m_SearchIFSelected = false;
			SearchIF.onValueChanged.AddListener(OnSearchBar);
			SearchIF.onSelect.AddListener((string _) => { m_SearchIFSelected = true; /*Debug.Log("Sel");*/ } );
			SearchIF.onDeselect.AddListener((string _) => { m_SearchIFSelected = false; /*Debug.Log("Del");*/ } );
			SearchIF.resetOnDeActivation = false;
			m_State = InventoryState.COUNT;

			CrossButton.onClick.AddListener(() => OnClose?.Invoke());
			AddItemButton.onClick.AddListener(OnAddItem);
			FilterButton.onClick.AddListener(OnFilterButton);
			AscDscButton.onClick.AddListener(OnAscDscButton);
		}
		void LoadSelectorUI()
		{
			if (Selector != null)
				return;

			Selector = Instantiate(SelectorUI.GetInstance());

			var canvas = CameraManager.Mgr.Canvas;
			Selector.transform.SetParent(canvas.transform);

			var rt = Selector.gameObject.GetComponent<RectTransform>();
			rt.sizeDelta = Vector2.zero;
			rt.anchoredPosition = Vector2.zero;

			Selector.OnSelection += OnItemsSelected;
			Selector.OnClose += OnSelectorClose;
		}
		void LoadItems()
		{
			var slotCount = m_SpellCaster.GetItems().Count;

			if (m_AllItems == null)
				m_AllItems = new List<ViewElementInfoB>(slotCount);
			if (m_ShowingItems == null)
				m_ShowingItems = new List<ViewElementInfoB>(slotCount);

			m_AllItems.Clear();
			m_ShowingItems.Clear();

			if (m_AllItems.Capacity < slotCount)
				m_AllItems.Capacity = slotCount;

			if (m_ShowingItems.Capacity < slotCount)
				m_ShowingItems.Capacity = slotCount;

			for (int i = 0; i < m_SpellCaster.GetItems().Count; ++i)
			{
				var slot = m_SpellCaster.GetItems()[i];
				m_AllItems.Add(new ViewElementBItemInventoryInfo()
				{
					Item = slot.Item,
					Count = slot.Count,
					Inventory = this,
					IsNew = false,
					IsFavourite = false,
					CategoryName = "",
					Name = slot.Item.GetName().ToLower() + '_' + slot.Item.GetID().ToString()
				});
			}
		}
		public void Init(AI.CSpellCaster spellCaster)
		{
			m_SpellCaster = spellCaster;

			LoadItems();

			ItemView.Init(CViewElementBItemInventory.GetItemInventoryInstance());

			m_SortState = SortState.Alphabetic;
			m_SortAsc = true;
			ChangeInventoryState(InventoryState.Equipment);

			QuickItems.Init();
		}
		List<ViewElementBSelectableInfo> GetItemList()
		{
			var list = new List<ViewElementBSelectableInfo>(AI.Items.ItemLoader.ItemIDDict.Count);
			for(int i = 0; i < AI.Items.ItemLoader.ItemIDDict.Count; ++i)
			{
				var pair = AI.Items.ItemLoader.ItemIDDict.ElementAt(i);
				if (pair.Value < 0 || pair.Value >= AI.Items.ItemLoader.Items.Count)
					continue;

				var defItem = AI.Items.ItemLoader.Items[pair.Value];
				if (defItem == null)
					continue;

				Sprite itemImage;
				if (AI.Items.ItemLoader.ItemSpriteDict.TryGetValue(defItem.GetImageName(), out int imageIdx))
					itemImage = AI.Items.ItemLoader.ItemSprites[imageIdx];
				else
					itemImage = AI.Items.ItemLoader.InvalidItem;

				list.Add(new ViewElementBItemSelectableInfo()
				{
					Item = defItem.Duplicate(),
					Name = defItem.GetName() + '_' + pair.Key.ToString(),
					Image = itemImage,
					Selected = false,
					Selector = Selector
				});
			}
			return list;
		}
		void ChangeInventoryState(InventoryState state)
		{
			if (m_State == state || state == InventoryState.COUNT)
				return;

			for (int i = 0; i < StateSelectionImages.Length; ++i)
				StateSelectionImages[i].gameObject.SetActive(false);
			for (int i = 0; i < StateButtons.Length; ++i)
				StateButtons[i].interactable = true;

			StateButtons[(int)state].interactable = false;
			StateSelectionImages[(int)state].gameObject.SetActive(true);
						
			m_State = state;
			
			if (SearchIF.text.Length > 0)
				SearchIF.text = "";
			else
				OnSearchBar("");
		}
		void ChangeSorting(SortState state, bool sortAsc)
		{
			m_SortState = state;
			m_SortAsc = sortAsc;
			if(m_SortState != SortState.Search)
				OnSearchBar(SearchIF.text);

			if (m_SortAsc)
				AscDscButton.transform.localScale = Vector3.one;
			else
				AscDscButton.transform.localScale = new Vector3(1f, -1f, 1f);

			switch (m_SortState)
			{
				case SortState.Alphabetic:
					FilterButton.image.sprite = FilterAlphabeticImage;
					FilterButton.interactable = true;
					break;
				case SortState.Category:
					FilterButton.image.sprite = FilterCategoryImage;
					FilterButton.interactable = true;
					break;
				case SortState.Search:
					FilterButton.interactable = false;
					break;
			}
		}
		IEnumerator NextFrame(Action fn)
		{
			yield return null;

			fn();
		}
		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				StartCoroutine(NextFrame(() => OnClose?.Invoke()));
				return;
			}
			// Key down -> focus search and send text
			if(!m_SearchIFSelected && Input.anyKeyDown && Input.inputString.Length > 0)
			{
				SearchIF.ActivateInputField();
				SearchIF.text += Input.inputString;
				SearchIF.caretPosition += Input.inputString.Length;
			}
			if(Input.GetKeyDown(KeyCode.Tab))
			{
				var state = (int)m_State;
				state += 1;
				if (state >= InventoryStateCount)
					state = 0;
				ChangeInventoryState((InventoryState)state);
			}
		}
		void OnAddItem()
		{
			LoadSelectorUI();

			enabled = false;
			Selector.gameObject.SetActive(true);

			Selector.Init("Add x100 Items", CViewElementBItemSelectable.GetItemInstance(), GetItemList(), true);
		}
		void OnItemsSelected(List<ViewElementBSelectableInfo> items)
		{
			for(int i = 0; i < items.Count; ++i)
			{
				var itemVE = items[i] as ViewElementBItemSelectableInfo;
				var item = itemVE.Item;
				m_SpellCaster.AddItemToInventory(item.GetID(), 100);

			}
			LoadItems();
			OnSearchBar(SearchIF.text);
		}
		void OnSelectorClose()
		{
			enabled = true;
			Selector.gameObject.SetActive(false);
		}
		void OnFilterButton()
		{
			if (m_SortState == SortState.Alphabetic)
				ChangeSorting(SortState.Category, m_SortAsc);
			else if (m_SortState == SortState.Category)
				ChangeSorting(SortState.Alphabetic, m_SortAsc);
		}
		void OnAscDscButton()
		{
			ChangeSorting(m_SortState, !m_SortAsc);
		}
		public AI.CSpellCaster GetSpellCaster() => m_SpellCaster;
		bool ItemCategoryIsValid(Def.ItemCategory category)
		{
			switch (category)
			{
				case Def.ItemCategory.Consumable:
					return m_State == InventoryState.Consumable;
				case Def.ItemCategory.Resource:
					return m_State == InventoryState.Resources;
				case Def.ItemCategory.Equipment:
					return m_State == InventoryState.Equipment;
				case Def.ItemCategory.Quest:
					return m_State == InventoryState.Quest;
			}
			return false;
		}
		bool SortAlphabetic(CViewElementB left , CViewElementB right)
		{
			var l = left as CViewElementBItemInventory;
			var r = right as CViewElementBItemInventory;
			var comparison = string.Compare(l.GetName(), r.GetName(), comparisonType: StringComparison.OrdinalIgnoreCase);
			if (m_SortAsc)
				return comparison > 0;
			return comparison < 0;
		}
		void OnSearchBar(string text)
		{
			if (text.Length > 0)
				ChangeSorting(SortState.Search, m_SortAsc);
			else if (m_SortState == SortState.Search)
				ChangeSorting(SortState.Alphabetic, m_SortAsc);


			ItemView.Clear();
			m_ShowingItems.Clear();

			for(int i = 0; i < m_AllItems.Count; ++i)
			{
				var info = m_AllItems[i] as ViewElementBItemInventoryInfo;
				if (!ItemCategoryIsValid(info.Item.GetItemCategory()))
					continue;
				m_ShowingItems.Add(info);
			}

			if(text.Length > 0)
			{
				var searchLower = text.ToLower();
				for(int i = 0; i < m_ShowingItems.Count;)
				{
					var info = m_ShowingItems[i] as ViewElementBItemInventoryInfo;
					var name = info.Name;

					bool containsAllChars = true;
					for(int j = 0; j < searchLower.Length; ++j)
					{
						var idx = name.IndexOf(searchLower[j]);
						if(idx < 0)
						{
							containsAllChars = false;
							break;
						}
						if (idx == 0)
							name = name.Substring(1);
						else if (idx == (name.Length - 1))
							name = name.Substring(0, name.Length - 1);
						else
							name = name.Substring(0, idx) + name.Substring(idx + 1);
					}
					if (!containsAllChars)
						m_ShowingItems.RemoveAt(i);
					else
						++i;
				}
			}

			if (m_SortState != SortState.Category)
				ItemView.SetSortFn(SortAlphabetic);
			else
				ItemView.SetSortFn(null);

			ItemView.AddElements(m_ShowingItems);
		}
		public delegate void OnCloseCB();
	}
}
