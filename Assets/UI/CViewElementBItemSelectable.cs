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
	public class CViewElementBItemSelectable : CViewElementBSelectable
	{
		public UnityEngine.UI.Image Image;
		public TMPro.TMP_Text Name;
		AI.Items.Item m_Item;

		static CViewElementBItemSelectable InstanceItem;
		public static CViewElementBItemSelectable GetItemInstance()
		{
			if (InstanceItem == null)
			{
				InstanceItem = Resources.Load<CViewElementBItemSelectable>("UI/ViewElementItemSelectable");
				if (InstanceItem == null)
					throw new Exception("Couldn't load ViewElementItemSelectable");
			}
			return InstanceItem;
		}

		public AI.Items.Item GetItem() => m_Item;
		public override void ElementInit(CViewB view, ViewElementInfoB info)
		{
			base.ElementInit(view, info);

			var ve = info as ViewElementBItemSelectableInfo;
			if (Image != null)
				Image.sprite = ve.Image;

			m_Item = ve.Item;

			if (Name != null)
			{
				if (m_Item == null)
					Name.text = "New Item";
				else
					Name.text = m_Item.GetName();
			}
		}
		public override void _Awake()
		{
			base._Awake();
		}
	}
	public class ViewElementBItemSelectableInfo : ViewElementBSelectableInfo
	{
		public Sprite Image;
		public AI.Items.Item Item;
	}
}
