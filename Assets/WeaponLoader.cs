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

namespace Assets
{
	public static class WeaponLoader
	{
		public static List<OddWeaponInfo> WeaponInfos;
		public static Dictionary<string, int> Dict;
		public static void Prepare()
		{
			WeaponInfos = new List<OddWeaponInfo>(AssetLoader.OddWeaponInfos);
			Dict = new Dictionary<string, int>(WeaponInfos.Count);

			for(int i = 0; i < WeaponInfos.Count; ++i)
			{
				var weaponInfo = WeaponInfos[i];
				if(!Dict.TryAdd(weaponInfo.WeaponName, i))
				{
					Debug.LogWarning($"Duplicated weapon {weaponInfo.WeaponName}"); }
			}
		}
	}
}
