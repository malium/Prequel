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
	public class CQuickItemElement : MonoBehaviour
	{
		[SerializeReference] UnityEngine.UI.Image Image;
		[SerializeReference] TMPro.TMP_Text Count;
		[SerializeReference] Def.QuickItemSlot Slot;
		[SerializeReference] CQuickItems QuickItems;
		[SerializeReference] UnityEngine.UI.Image DropEffect;
		CDropRegion m_DropRegion;

		private void Awake()
		{
			m_DropRegion = gameObject.GetComponent<CDropRegion>();
			if(m_DropRegion != null)
			{
				//m_DropRegion.OnDrop.AddListener(OnDroppedItem);
				m_DropRegion.OnDraggableAbove.AddListener(OnDraggableOnTop);
				m_DropRegion.OnDraggableNotAbove.AddListener(OnDraggableNoLongerOnTop);
			}
		}
		void OnDraggableNoLongerOnTop()
		{
			//Debug.Log("Item no longer above " + Slot.ToString());
			DropEffect.gameObject.SetActive(false);
		}
		void OnDraggableOnTop(CDraggable draggable)
		{
			//Debug.Log("Item above " + Slot.ToString());
			DropEffect.gameObject.SetActive(true);
		}
		//void OnDroppedItem(CDraggable draggable)
		//{
		//	Debug.Log("Dropped item into " + Slot.ToString());
		//}
		public void Set(CQuickItems quickItems, Sprite itemImage, int count)
		{
			QuickItems = quickItems;

			if(Image != null)
			{
				if (itemImage != null)
				{
					Image.gameObject.SetActive(true);
					Image.sprite = itemImage;
				}
				else
				{
					Image.gameObject.SetActive(false);
				}
			}
			if(Count != null)
			{
				if (count > 0)
				{
					Count.gameObject.SetActive(true);
					if (count < 100)
						Count.text = count.ToString();
					else
						Count.text = "+99";
				}
				else
				{
					Count.gameObject.SetActive(false);
				}
			}
		}
		public Def.QuickItemSlot GetSlot() => Slot;
		public CQuickItems GetQuickItems() => QuickItems;
	}
}
