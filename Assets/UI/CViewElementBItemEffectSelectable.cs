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
	public class CViewElementBItemEffectSelectable : CViewElementBSelectable
	{
		public TMPro.TMP_Text Name;

		static CViewElementBItemEffectSelectable InstanceItemEffect;
		public static CViewElementBItemEffectSelectable GetItemEffectInstance()
		{
			if (InstanceItemEffect == null)
			{
				InstanceItemEffect = Resources.Load<CViewElementBItemEffectSelectable>("UI/ViewElementItemEffectSelectable");
				if (InstanceItemEffect == null)
					throw new Exception("Couldn't load ViewElementItemEffectSelectable");
			}
			return InstanceItemEffect;
		}

		public override void ElementInit(CViewB view, ViewElementInfoB info)
		{
			base.ElementInit(view, info);

			var ve = info as ViewElementBItemEffectSelectableInfo;

			if (Name != null)
				Name.text = ve.Name;
		}
		public override void _Awake()
		{
			base._Awake();
		}
	}
	public class ViewElementBItemEffectSelectableInfo : ViewElementBSelectableInfo
	{

	}
}