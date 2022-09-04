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
	public static class SpellWeapons
	{
		public static Sprite[] List;
		public static Dictionary<string, int> Dict;

		public static void Prepare()
		{
			List = AssetLoader.SpellWeapons;
			Dict = new Dictionary<string, int>(List.Length);
			for(int i = 0; i < List.Length; ++i)
			{
				var elem = List[i];
				var elemName = elem.name;
				var begin = elemName.Substring(0, 3).ToLower();
				if(begin != "sw_")
				{
					Debug.LogWarning("Wrong SpellWeapon with name: " + elemName);
					continue;
				}
				var name = elemName.Substring(3);
				if(Dict.ContainsKey(name))
				{
					Debug.LogWarning("SpellWeapon '" + name + "' is repeated.");
					continue;
				}
				var cpy = Sprite.Instantiate(elem);
				cpy.name = name;
				List[i] = cpy;
				Dict.Add(name, i);
			}
		}
	}
}
