/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI
{
	public class ItemEditorUI : MonoBehaviour
	{
		enum Selection
		{
			Image,
			Effect
		}
		public UnityEngine.UI.Button CrossButton;

		public UnityEngine.UI.Button DeleteItemBttn;

		public UnityEngine.UI.Button ImageButton;
		public UnityEngine.UI.Image Image;

		public TMPro.TMP_InputField NameIF;
		public TMPro.TMP_InputField IDIF;
		public TMPro.TMP_Dropdown CategoryDropdown;
		public TMPro.TMP_Dropdown SubcategoryDropdown;
		public TMPro.TMP_InputField DescriptionIF;
		public TMPro.TMP_InputField ShowIF;
		public UnityEngine.UI.Button AddEffectButton;
		public CViewB EffectView;
		static SelectorUI m_SelectorUI;

		public UnityEngine.UI.Image MessageBck;
		public TMPro.TMP_Text MessageText;

		AI.Items.Item m_Item;
		bool m_ValueLock;
		Selection m_Selection;
		static List<ViewElementBSelectableInfo> ItemSprites;
		static List<ViewElementBSelectableInfo> Effects;
		string m_OriginalName;
		int m_OriginalID;

		static ItemEditorUI Instance;
		public static ItemEditorUI GetInstance()
		{
			if (Instance == null)
			{
				Instance = Resources.Load<ItemEditorUI>("UI/ItemEditorUI");
				if (Instance == null)
					throw new Exception("Couldn't load ItemEditorUI");
			}
			return Instance;
		}

		public event OnCloseCB OnClose; 
		void LoadItemsSprites()
		{
			if (ItemSprites != null)
				return;
			ItemSprites = new List<ViewElementBSelectableInfo>(AI.Items.ItemLoader.ItemSprites.Length);
			for(int i = 0; i < AI.Items.ItemLoader.ItemSprites.Length; ++i)
			{
				var sprite = AI.Items.ItemLoader.ItemSprites[i];
				ItemSprites.Add(new ViewElementBImageNameSelectableInfo()
				{
					Selector = m_SelectorUI,
					Image = sprite,
					Name = sprite.name
				});
			}
		}
		void LoadItemEffects()
		{
			if (Effects != null)
				return;
			Effects = new List<ViewElementBSelectableInfo>(AI.Items.ItemLoader.ItemEffects.Count);
			for(int i = 0; i < AI.Items.ItemLoader.ItemEffects.Count; ++i)
			{
				Effects.Add(new ViewElementBItemEffectSelectableInfo()
				{
					Selector = m_SelectorUI,
					Name = AI.Items.ItemLoader.ItemEffects[i].GetItemEffectType().ToString()
				});
			}
		}
		private void Awake()
		{
			CrossButton.onClick.AddListener(OnCloseButton);
			ImageButton.onClick.AddListener(OnImageButton);
			NameIF.onEndEdit.AddListener(OnNameChange);
			IDIF.onEndEdit.AddListener(OnIDChange);
			DescriptionIF.onValueChanged.AddListener(OnDescriptionTextChange);
			DescriptionIF.onEndEdit.AddListener(OnDescriptionTextFinish);
			AddEffectButton.onClick.AddListener(OnAddEffect);
			DeleteItemBttn.onClick.AddListener(OnDeleteItemButton);

			var categories = new List<TMPro.TMP_Dropdown.OptionData>(Def.ItemCategoryCount);
			for(int i = 0; i < Def.ItemCategoryCount; ++i)
			{
				categories.Add(new TMPro.TMP_Dropdown.OptionData(((Def.ItemCategory)i).ToString()));
			}
			CategoryDropdown.ClearOptions();
			CategoryDropdown.AddOptions(categories);
			CategoryDropdown.value = 0;
			CategoryDropdown.RefreshShownValue();

			CategoryDropdown.onValueChanged.AddListener(OnCategoryChanged);

			EffectView.OnElementRemovePrev += OnEffectRemove;
			m_ValueLock = false;
		}
		public void Init(AI.Items.Item item)
		{
			m_Item = item;
			if (AI.Items.ItemLoader.ItemSpriteDict.TryGetValue(m_Item.GetImageName(), out int spriteIdx))
				Image.sprite = AI.Items.ItemLoader.ItemSprites[spriteIdx];
			else
				Image.sprite = AI.Items.ItemLoader.InvalidItem;

			m_OriginalID = m_Item.GetID();
			m_OriginalName = m_Item.GetName();

			DeleteItemBttn.interactable = m_OriginalID >= 0;

			NameIF.text = m_Item.GetName();
			IDIF.text = m_Item.GetID().ToString();
			DescriptionIF.text = m_Item.GetDescription();
			CategoryDropdown.value = (int)m_Item.GetItemCategory();
			CategoryDropdown.RefreshShownValue();

			EffectView.Init(CViewElementBItemEffect.GetInstance());

			for(int i = 0; i < m_Item.GetItemEffects().Count; ++i)
			{
				EffectView.AddElement(new ViewElementBItemEffectInfo()
				{
					ItemEffect = m_Item.GetItemEffects()[i]
				});
			}
		}
		bool VerifyAndSaveInformation()
		{
			string errorText = "";
			bool error = false;
			if((AI.Items.ItemLoader.ItemIDDict.ContainsKey(m_Item.GetID()) && m_Item.GetID() != m_OriginalID) || m_Item.GetID() < 0 || m_Item.GetID() == int.MaxValue)
			{
				errorText += "The ID is invalid\n";
				error = true;
			}
			if(m_Item.GetName() == "")
			{
				errorText += "The name is empty\n";
				error = true;
			}
			if(error)
			{
				var mb = MessageBoxUI.Create();
				mb.Init(Def.MessageBoxType.YesNo, "Error", errorText +
					"\n(YES)Exit and ignore the changes\n(NO)Return to item edit.", null, null,
					() => //Yes
					{
						//if(AI.Items.ItemLoader.ItemIDDict.TryGetValue(m_OriginalID, out int idx))
						//{
						//	AI.Items.ItemLoader.ItemIDDict.Remove(m_OriginalID);
						//	AI.Items.ItemLoader.Items[idx] = null;
						//}
						m_Item = null;
						GameUtils.DeleteGameobject(mb.gameObject);
						OnClose();
					},
					() => // No
					{
						GameUtils.DeleteGameobject(mb.gameObject);
					});
				return false;
			}
			// no error so lets save
			// Name or ID is not the same as before, try to remove the old file
			if(m_OriginalName != m_Item.GetName() || m_OriginalID != m_Item.GetID())
			{
				var fi = new FileInfo(AI.Items.ItemLoader.ItemPath + '/' + m_OriginalName + '_' + m_OriginalID.ToString() + ".xml");
				if (fi.Exists)
					fi.Delete();

				if (m_OriginalID != m_Item.GetID())
				{
					if(AI.Items.ItemLoader.ItemIDDict.TryGetValue(m_OriginalID, out int idx))
					{
						AI.Items.ItemLoader.ItemIDDict.Remove(m_OriginalID);
						AI.Items.ItemLoader.ItemIDDict.Add(m_Item.GetID(), idx);
					}
					else
					{
						idx = AI.Items.ItemLoader.Items.FindIndex((AI.Items.Item test) => test == m_Item);
						if(idx < 0 || idx >= AI.Items.ItemLoader.Items.Count) // This item is new
						{
							idx = AI.Items.ItemLoader.Items.Count;
							AI.Items.ItemLoader.Items.Add(m_Item);
						}
						AI.Items.ItemLoader.ItemIDDict.Add(m_Item.GetID(), idx);
					}
				}
			}
			bool isActive = false;
			for(int i = 0; i < m_Item.GetItemEffects().Count; ++i)
			{
				var effect = m_Item.GetItemEffects()[i];
				if(effect.IsActivable())
				{
					isActive = true;
					break;
				}
			}
			m_Item.SetIsActive(isActive);
			AI.Items.ItemLoader.SaveItem(m_Item);
			return true;
		}
		void OnCloseButton()
		{
			if(VerifyAndSaveInformation())
				OnClose();
		}
		void OnNameChange(string text)
		{
			m_Item.SetName(text);
		}
		void OnIDChange(string text)
		{
			if (m_ValueLock)
				return;

			var id = m_Item.GetID();

			if(!int.TryParse(text, out int nid))
			{
				StopCoroutine("DisplayMessage");
				m_ValueLock = true;
				IDIF.text = id.ToString();
				m_ValueLock = false;
				StartCoroutine(DisplayMessage("<color=\"red\"><align=\"center\">Error</color>\n\nInvalid Item ID</align>"));
				return;
			}

			if(nid < 0 || nid == int.MaxValue)
			{
				StopCoroutine("DisplayMessage");
				m_ValueLock = true;
				IDIF.text = id.ToString();
				m_ValueLock = false;
				StartCoroutine(DisplayMessage("<color=\"red\"><align=\"center\">Error</color>\n\nInvalid Item ID</align>"));
				return;
			}

			if(AI.Items.ItemLoader.ItemIDDict.ContainsKey(nid))
			{
				StopCoroutine("DisplayMessage");
				m_ValueLock = true;
				IDIF.text = id.ToString();
				m_ValueLock = false;
				StartCoroutine(DisplayMessage("<color=\"red\"><align=\"center\">Error</color>\n\nID already in-use</align>"));
			}

			m_ValueLock = true;
			IDIF.text = nid.ToString();
			m_ValueLock = false;
			m_Item.SetID(nid);
		}
		IEnumerator DisplayMessage(string msg)
		{
			MessageText.text = msg;
			MessageBck.gameObject.SetActive(true);
			var bckColor = MessageBck.color;
			var textColor = MessageText.color;

			float time = 0f;
			while(time < 1f)
			{
				bckColor.a = time;
				textColor.a = time;
				MessageBck.color = bckColor;
				MessageText.color = textColor;
				time += Time.deltaTime * 4f;
				yield return null;
			}
			bckColor.a = 1f;
			textColor.a = 1f;
			time = 0f;
			while(time < 1f)
			{
				time += Time.deltaTime;
				yield return null;
			}
			while (time > 0f)
			{
				bckColor.a = time;
				textColor.a = time;
				MessageBck.color = bckColor;
				MessageText.color = textColor;
				time -= Time.deltaTime * 4f;
				yield return null;
			}
			bckColor.a = 0f;
			textColor.a = 0f;
			MessageBck.color = bckColor;
			MessageText.color = textColor;
		}
		void OnDescriptionTextChange(string text)
		{
			ShowIF.text = text;
			ShowIF.verticalScrollbar.value = DescriptionIF.verticalScrollbar.value;
		}
		void OnDescriptionTextFinish(string text)
		{
			m_Item.SetDescription(text);
		}
		void OnCategoryChanged(int category)
		{
			m_Item.SetCategory((Def.ItemCategory)category);
		}
		void LoadSelectorUI()
		{
			if (m_SelectorUI != null)
				return;

			m_SelectorUI = Instantiate(SelectorUI.GetInstance());

			var canvas = CameraManager.Mgr.Canvas;
			m_SelectorUI.transform.SetParent(canvas.transform);

			var rt = m_SelectorUI.gameObject.GetComponent<RectTransform>();
			rt.sizeDelta = Vector2.zero;
			rt.anchoredPosition = Vector2.zero;

			m_SelectorUI.OnClose += OnSelectorClose;
			m_SelectorUI.OnSelection += OnSelectionEnd;
		}
		void OnImageButton()
		{
			LoadSelectorUI();
			LoadItemsSprites();

			m_Selection = Selection.Image;
			m_SelectorUI.gameObject.SetActive(true);
			m_SelectorUI.Init("Select item image", CViewElementBImageNameSelectable.GetNameImageInstance(), ItemSprites, false);
			gameObject.SetActive(false);
		}
		private void OnSelectionEnd(List<ViewElementBSelectableInfo> selection)
		{
			if (selection.Count == 0)
				return;

			switch (m_Selection)
			{
				case Selection.Image:
					{
						var selItem = selection[0] as ViewElementBImageNameSelectableInfo;
						var name = selItem.Name;
						m_Item.SetImageName(name);
						Image.sprite = selItem.Image;
					}
					break;
				case Selection.Effect:
					{
						var selItem = selection[0] as ViewElementBItemEffectSelectableInfo;
						var name = selItem.Name;

						if(AI.Items.ItemLoader.ItemEffectNameDict.TryGetValue(name, out int ieffectIdx))
						{
							var itemEffect = AI.Items.ItemLoader.GetItemEffect((Def.ItemEffectType)ieffectIdx);
							m_Item.GetItemEffects().Add(itemEffect);
							EffectView.AddElement(new ViewElementBItemEffectInfo()
							{
								ItemEffect = itemEffect
							});
						}
					}
					break;
				default:
					Debug.LogWarning("Not all enum handled: " + m_Selection.ToString());
					break;
			}
		}
		private void OnSelectorClose()
		{
			gameObject.SetActive(true);
			m_SelectorUI.gameObject.SetActive(false);
		}
		void OnAddEffect()
		{
			LoadSelectorUI();
			LoadItemEffects();

			m_Selection = Selection.Effect;
			m_SelectorUI.gameObject.SetActive(true);
			m_SelectorUI.Init("Select Item effect", CViewElementBItemEffectSelectable.GetItemEffectInstance(), Effects, false);
			gameObject.SetActive(false);
		}
		void OnEffectRemove(CViewElementB element)
		{
			var ve = element as CViewElementBItemEffect;
			var idx = m_Item.GetItemEffects().FindIndex((AI.Items.IItemEffect test) => test == ve.GetItemEffect());
			if (idx < 0 || idx >= m_Item.GetItemEffects().Count)
				return;
			m_Item.GetItemEffects().RemoveAt(idx);
		}
		void OnDeleteItemButton()
		{
			var mb = MessageBoxUI.Create();
			mb.Init(Def.MessageBoxType.YesNo, "Warning",
				"Are you sure you want to delete this Item?",
				null, null, () => // YES
				{
					var fi = new FileInfo(AI.Items.ItemLoader.ItemPath + '/' + m_OriginalName + '_' + m_OriginalID.ToString() + ".xml");
					if (fi.Exists)
						fi.Delete();

					if (AI.Items.ItemLoader.ItemIDDict.TryGetValue(m_OriginalID, out int idx))
					{
						AI.Items.ItemLoader.ItemIDDict.Remove(m_OriginalID);
						AI.Items.ItemLoader.Items[idx] = null;
					}

					m_Item = null;
					GameUtils.DeleteGameobject(mb.gameObject);
					OnClose();
				}, () => // No
				{
					GameUtils.DeleteGameobject(mb.gameObject);
				});
		}
		public AI.Items.Item GetItem() => m_Item;
		public delegate void OnCloseCB();
	}
}
