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
	public class CViewElementBItemInventory : CViewElementB
	{
		[SerializeField] RectTransform Frame;
		[SerializeField] UnityEngine.UI.Image ItemImage;
		[SerializeField] UnityEngine.UI.Image FavouriteImage;
		[SerializeField] TMPro.TMP_Text CountText;
		[SerializeField] UnityEngine.UI.Image IsNewImage;
		[SerializeField] CTooltipTrigger ItemTooltip;

		[SerializeField] RectTransform Spacer;
		[SerializeField] TMPro.TMP_Text CategoryText;
		
		[SerializeField] CChildScaler Scaler;
		[SerializeField] CDraggable Draggable;

		AI.Items.Item m_Item;
		InventoryUI Inventory;
		RectTransform m_ContentRT;
		string m_Name;

		static CViewElementBItemInventory InstanceItemInventory;
		public static CViewElementBItemInventory GetItemInventoryInstance()
		{
			if (InstanceItemInventory == null)
			{
				InstanceItemInventory = Resources.Load<CViewElementBItemInventory>("UI/VEInventory");
				if (InstanceItemInventory == null)
					throw new Exception("Couldn't load VEInventory");
			}
			return InstanceItemInventory;
		}

		public override void _Awake()
		{
			base._Awake();

			//Scaler = gameObject.GetComponent<CChildScaler>();
			//Draggable = gameObject.GetComponent<CDraggable>();
			Draggable.DragEnd.AddListener(DragEnd);
			Draggable.DragBegin.AddListener(DragBegin);
		}
		public override void ElementInit(CViewB view, ViewElementInfoB info)
		{
			base.ElementInit(view, info);

			var itemInventory = info as ViewElementBItemInventoryInfo;
			m_Item = itemInventory.Item;
			Inventory = itemInventory.Inventory;
			m_Name = itemInventory.Name;

			m_ContentRT = transform.parent.gameObject.GetComponent<RectTransform>();

			if (itemInventory.Item != null) // It's an Item
			{
				Spacer.gameObject.SetActive(false);
				Frame.gameObject.SetActive(true);
				Sprite itemImage;
				if (!AI.Items.ItemLoader.ItemSpriteDict.TryGetValue(m_Item.GetImageName(), out int itemImageIdx))
					itemImage = AI.Items.ItemLoader.InvalidItem;
				else
					itemImage = AI.Items.ItemLoader.ItemSprites[itemImageIdx];

				ItemImage.sprite = itemImage;
				FavouriteImage.gameObject.SetActive(itemInventory.IsFavourite);
				IsNewImage.gameObject.SetActive(itemInventory.IsNew);
				if (itemInventory.Count <= 99)
					CountText.text = itemInventory.Count.ToString();
				else
					CountText.text = "+99";
				Draggable.enabled = true;
			}
			else // It's a category
			{
				Spacer.gameObject.SetActive(true);
				Frame.gameObject.SetActive(false);
				CategoryText.text = itemInventory.CategoryName;

				Spacer.sizeDelta = new Vector2(m_ContentRT.rect.width, Spacer.sizeDelta.y);
				Draggable.enabled = false;
			}
			Scaler.UpdateTransform();
		}
		public AI.Items.Item GetItem() => m_Item;
		public bool IsItem() => m_Item != null;
		public bool IsCategory() => m_Item == null;
		void DragBegin()
		{
			var canvas = CameraManager.Mgr.Canvas;
			Frame.transform.SetParent(canvas.transform);

			ItemTooltip.OnPointerExit(null);
			ItemTooltip.enabled = false;
		}
		void DragEnd(CDropRegion dropRegion)
		{
			Frame.transform.SetParent(transform);
			Frame.anchoredPosition = Vector2.zero;
			ItemTooltip.enabled = true;

			if (dropRegion != null && dropRegion.TryGetComponent(out CQuickItemElement quickItem))
			{
				var slot = quickItem.GetSlot();
				var sc = Inventory.GetSpellCaster();
				sc.GetQuickItems()[(int)slot] = m_Item.GetID();
				quickItem.GetQuickItems().Init();
			}
		}
		public string GetName() => m_Name;
		private void OnDisable()
		{
			if (ItemTooltip != null)
				ItemTooltip.GetTooltip().gameObject.SetActive(false);
		}
	}
	public class ViewElementBItemInventoryInfo : ViewElementInfoB
	{
		public string CategoryName;
		public AI.Items.Item Item;
		public int Count;
		public bool IsFavourite;
		public bool IsNew;
		public InventoryUI Inventory;
		public string Name;
	}
}
