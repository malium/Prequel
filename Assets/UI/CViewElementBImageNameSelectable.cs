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
	public class CViewElementBImageNameSelectable : CViewElementBSelectable
	{
		public UnityEngine.UI.Image Image;
		public TMPro.TMP_Text Name;

		static CViewElementBImageNameSelectable InstanceNameImage;
		public static CViewElementBImageNameSelectable GetNameImageInstance()
		{
			if (InstanceNameImage == null)
			{
				InstanceNameImage = Resources.Load<CViewElementBImageNameSelectable>("UI/ViewElementNameImageSelectable");
				if (InstanceNameImage == null)
					throw new Exception("Couldn't load ViewElementNameImageSelectable");
			}
			return InstanceNameImage;
		}

		public override void ElementInit(CViewB view, ViewElementInfoB info)
		{
			base.ElementInit(view, info);

			var ve = info as ViewElementBImageNameSelectableInfo;
			if (Image != null)
				Image.sprite = ve.Image;

			if (Name != null)
				Name.text = ve.Name;
		}
		public override void _Awake()
		{
			base._Awake();
		}
	}
	public class ViewElementBImageNameSelectableInfo : ViewElementBSelectableInfo
	{
		public Sprite Image;
	}
}
