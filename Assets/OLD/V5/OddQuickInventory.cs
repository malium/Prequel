/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Assets
//{
//	public class OddQuickInventory
//	{
//		const float SelectChangeWaitTime = 0.2f;

//		[SerializeField]
//		List<InventoryItem> m_Items;
//		public List<InventoryItem> Items
//		{
//			get
//			{
//				return m_Items;
//			}
//		}
//		GameObject UIGameObject;
//		Image CurrentItemImage;
//		Image NextItemImage;
//		Image PrevItemImage;

//		[SerializeField]
//		int m_CurSelItem;
//		public int CurItem
//		{
//			get
//			{
//				return m_CurSelItem;
//			}
//			set
//			{
//				m_CurSelItem = value;
//				UpdateItemSprites();
//			}
//		}

//		float LastSelectedChangedTime;

//		GameObject GetItemChild(string parent)
//		{
//			GameObject item = null;

//			for (int i = 0; i < UIGameObject.transform.childCount; ++i)
//			{
//				var child = UIGameObject.transform.GetChild(i);
//				if (child.name.ToLower() == parent)
//				{
//					Transform childMask = null;
//					for (int j = 0; j < child.childCount; ++j)
//					{
//						var cChild = child.GetChild(j);
//						if (cChild.name.ToLower() == "mask")
//						{
//							childMask = cChild;
//							break;
//						}
//					}
//					if (childMask == null)
//						throw new Exception("Couldn't find the Mask child from ");

//					Transform childItem = null;
//					for (int j = 0; j < childMask.childCount; ++j)
//					{
//						var cChild = childMask.GetChild(j);
//						if (cChild.name.ToLower() == "item")
//						{
//							childItem = cChild;
//							break;
//						}
//					}
//					item = childItem.gameObject;
//					break;
//				}
//			}
//			return item;
//		}

//		public bool enabled
//		{
//			get
//			{
//				return UIGameObject.activeSelf;
//			}
//			set
//			{
//				UIGameObject.SetActive(value);                
//			}
//		}

//		public void Start()
//		{
//			m_Items = new List<InventoryItem>(3);
//			m_CurSelItem = 0;
//			var acUIGO = AssetContainer.Mgr.UIGameObjects;
//			UIGameObject = null;
//			for(int i = 0; i < acUIGO.Length; ++i)
//			{
//				if(acUIGO[i].name.ToLower() == "quick_inventory")
//				{
//					UIGameObject = acUIGO[i];
//					break;
//				}
//			}
//			if (UIGameObject == null)
//				throw new Exception("Couldn't find the Quick inventory UI GameObject.");

//			UIGameObject.SetActive(true);

//			var prevItemGO = GetItemChild("previous_item");
//			var nextItemGO = GetItemChild("next_item");
//			var currItemGO = GetItemChild("current_item");
//			PrevItemImage = prevItemGO.GetComponent<Image>();
//			NextItemImage = nextItemGO.GetComponent<Image>();
//			CurrentItemImage = currItemGO.GetComponent<Image>();

//			int lastInventoryPlace = 0;
//			//{
//			//    InventoryItem swordItem;
//			//    swordItem.Type = InvItemType.WEAPON;
//			//    swordItem.InventoryPlace = lastInventoryPlace++;
//			//    swordItem.ID = 0;
//			//    swordItem.ItemTexture = null;
//			//    for(int i = 0; i < AssetContainer.Mgr.InventoryItems.Length; ++i)
//			//    {
//			//        var item = AssetContainer.Mgr.InventoryItems[i];
//			//        if(item.name.ToLower() == "sword")
//			//        {
//			//            swordItem.ItemTexture = Sprite.Create(item, new Rect(0.0f, 0.0f, item.width, item.height), new Vector2(item.width * 0.5f, item.height * 0.5f));
//			//            break;
//			//        }
//			//    }
//			//    if (swordItem.ItemTexture == null)
//			//        throw new Exception("Couldn't find the item 'Sword' for the QuickInventory");
//			//    m_Items.Add(swordItem);
//			//}
//			{
//				InventoryItem bombItem;
//				bombItem.Type = InvItemType.BOMB;
//				bombItem.InventoryPlace = lastInventoryPlace++;
//				bombItem.ID = 0;
//				bombItem.ItemTexture = null;
//				for (int i = 0; i < AssetContainer.Mgr.InventoryItems.Length; ++i)
//				{
//					var item = AssetContainer.Mgr.InventoryItems[i];
//					if (item.name.ToLower() == "bombclassic")
//					{
//						bombItem.ItemTexture = Sprite.Create(item, new Rect(0.0f, 0.0f, item.width, item.height), new Vector2(item.width * 0.5f, item.height * 0.5f));
//						break;
//					}
//				}
//				if (bombItem.ItemTexture == null)
//					throw new Exception("Couldn't find the item 'BombClassic' for the QuickInventory");
//				m_Items.Add(bombItem);
//			}
//			{
//				InventoryItem woodPlankItem;
//				woodPlankItem.Type = InvItemType.BLOCK;
//				woodPlankItem.InventoryPlace = lastInventoryPlace++;
//				//woodPlankItem.ID = 13;
//				woodPlankItem.ID = BlockMaterial.FamilyDict["Wood plank"];
//				var tex = BlockMaterial.MaterialFamilies[woodPlankItem.ID].NormalMaterials[0].TopPart.Mat.GetTexture(Def.MaterialTextureID);
//				//var tex = BlockMaterial.BlockMaterials[BlockMaterial.MaterialTypes[13].Def.Materials[(int)BlockType.NORMAL][0].TopMat].BlockMaterial.GetTexture(Def.MaterialTextureID);
//				woodPlankItem.ItemTexture = Sprite.Create((Texture2D)tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(tex.width * 0.5f, tex.height * 0.5f));
//				m_Items.Add(woodPlankItem);
//			}
//			{
//				InventoryItem metal;
//				metal.Type = InvItemType.BLOCK;
//				metal.InventoryPlace = lastInventoryPlace++;
//				metal.ID = BlockMaterial.FamilyDict["Smooth metal"];
//				var tex = BlockMaterial.MaterialFamilies[metal.ID].NormalMaterials[0].TopPart.Mat.GetTexture(Def.MaterialTextureID);
//				//var tex = BlockMaterial.BlockMaterials[BlockMaterial.MaterialTypes[47].Def.Materials[(int)BlockType.NORMAL][0].TopMat].BlockMaterial.GetTexture(Def.MaterialTextureID);
//				metal.ItemTexture = Sprite.Create((Texture2D)tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(tex.width * 0.5f, tex.height * 0.5f));
//				m_Items.Add(metal);
//			}
//			{
//				InventoryItem bridgeItem;
//				bridgeItem.Type = InvItemType.BRIDGE;
//				bridgeItem.InventoryPlace = lastInventoryPlace++;
//				bridgeItem.ID = 0;
//				bridgeItem.ItemTexture = null;
//				for (int i = 0; i < AssetContainer.Mgr.InventoryItems.Length; ++i)
//				{
//					var item = AssetContainer.Mgr.InventoryItems[i];
//					if (item.name.ToLower() == "bridgeclassic")
//					{
//						bridgeItem.ItemTexture = Sprite.Create(item, new Rect(0.0f, 0.0f, item.width, item.height), new Vector2(item.width * 0.5f, item.height * 0.5f));
//						break;
//					}
//				}
//				if (bridgeItem.ItemTexture == null)
//					throw new Exception("Couldn't find the item 'BridgeClassic' for the QuickInventory");
//				m_Items.Add(bridgeItem);
//			}

//			CurItem = 0;
//			//PrevItemImage.sprite = m_Items[m_Items.Count - 1].ItemTexture;
//			//CurrentItemImage.sprite = m_Items[0].ItemTexture;
//			//NextItemImage.sprite = m_Items[1].ItemTexture;
//		}

//		public void Stop()
//		{
//			UIGameObject.SetActive(false);
//			m_Items.Clear();
//			m_Items = null;
//			CurrentItemImage = null;
//			PrevItemImage = null;
//			NextItemImage = null;
//			m_CurSelItem = 0;
//		}

//		void UpdateItemSprites()
//		{
//			if (m_CurSelItem < 0)
//				m_CurSelItem = m_Items.Count - 1;
//			else if (m_CurSelItem >= m_Items.Count)
//				m_CurSelItem = 0;

//			var prevItemID = m_CurSelItem - 1;
//			var nextItemID = m_CurSelItem + 1;
//			if (prevItemID < 0)
//				prevItemID = m_Items.Count - 1;
//			if (nextItemID >= m_Items.Count)
//				nextItemID = 0;

//			PrevItemImage.sprite = m_Items[prevItemID].ItemTexture;
//			CurrentItemImage.sprite = m_Items[m_CurSelItem].ItemTexture;
//			NextItemImage.sprite = m_Items[nextItemID].ItemTexture;
//		}

//		public void Update()
//		{
//			if (!enabled)
//				return;
//			if(LastSelectedChangedTime < Time.time)
//			{
//				var scroll = Input.mouseScrollDelta.y;
//				if (scroll != 0.0f)
//				{
//					if (scroll < 0.0f)
//					{
//						--m_CurSelItem;
//					}
//					else if (scroll > 0.0f)
//					{
//						++m_CurSelItem;
//					}

//					UpdateItemSprites();

//					LastSelectedChangedTime = Time.time + SelectChangeWaitTime;
//				}
//			}
//		}
		
//		//public void OnGUI()
//		//{
//		//    var rect = Manager.Mgr.m_Canvas.pixelRect;
//		//    const float hOffset = 5.0f;
//		//    const float vOffset = 10.0f;
//		//    const float imageSize = 32.0f;
//		//    const float curImageSize = 40.0f;

//		//    if (m_Items.Count == 0)
//		//        return;

//		//    var xPos = rect.width - (imageSize + hOffset);
//		//    var curXPos = rect.width - (curImageSize + hOffset);
//		//    var curYPos = rect.height * 0.5f - curImageSize * 0.5f;
//		//    var prevYPos = curYPos - (vOffset + imageSize);
//		//    var nextYPos = curYPos + curImageSize + vOffset;

//		//    var prevItemID = m_CurSelItem - 1;
//		//    var nextItemID = m_CurSelItem + 1;

//		//    if (prevItemID < 0)
//		//        prevItemID = m_Items.Count - 1;
//		//    if (nextItemID >= m_Items.Count)
//		//        nextItemID = 0;

//		//    var curItem = m_Items[m_CurSelItem];
//		//    var prevItem = m_Items[prevItemID];
//		//    var nextItem = m_Items[nextItemID];

//		//    //// draw prevItem
//		//    //GUI.DrawTexture(new Rect(xPos, prevYPos, imageSize, imageSize), ItemBackground);
//		//    //GUI.DrawTexture(new Rect(xPos, prevYPos, imageSize, imageSize), prevItem.ItemTexture);
//		//    //GUI.DrawTexture(new Rect(xPos, prevYPos, imageSize, imageSize), UnselectedForeground);

//		//    //// draw curItem
//		//    //GUI.DrawTexture(new Rect(curXPos, curYPos, curImageSize, curImageSize), ItemBackground);
//		//    //GUI.DrawTexture(new Rect(curXPos, curYPos, curImageSize, curImageSize), curItem.ItemTexture);
//		//    //GUI.DrawTexture(new Rect(curXPos, curYPos, curImageSize, curImageSize), SelectedForeground);

//		//    //// draw nextItem
//		//    //GUI.DrawTexture(new Rect(xPos, nextYPos, imageSize, imageSize), ItemBackground);
//		//    //GUI.DrawTexture(new Rect(xPos, nextYPos, imageSize, imageSize), nextItem.ItemTexture);
//		//    //GUI.DrawTexture(new Rect(xPos, nextYPos, imageSize, imageSize), UnselectedForeground);
//		//}
//	}
//}
