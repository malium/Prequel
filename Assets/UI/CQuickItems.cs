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
	public class CQuickItems : MonoBehaviour
	{
		public CQuickItemElement[] Elements;
		public InventoryUI Inventory;

		public void Init()
		{
			var sc = Inventory.GetSpellCaster();
			for(int i = 0; i < Elements.Length; ++i)
			{
				var element = Elements[i];
				var quickItemID = sc.GetQuickItems()[i];
				if(quickItemID < 0)
				{
					element.Set(this, null, 0);
					continue;
				}

				var slot = sc.GetItemSlot(quickItemID);
				Sprite image;
				if (AI.Items.ItemLoader.ItemSpriteDict.TryGetValue(slot.Item.GetImageName(), out int imageIDX))
					image = AI.Items.ItemLoader.ItemSprites[imageIDX];
				else
					image = AI.Items.ItemLoader.InvalidItem;
				element.Set(this, image, slot.Count);
			}
		}
	}
}
