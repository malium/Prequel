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
	public static class EditorSprites
	{
		public static List<Sprite> Sprites;
		public static Dictionary<string, int> Dict;

		public static void Prepare()
		{
			Sprites = new List<Sprite>(AssetLoader.EditorSprites);
			Dict = new Dictionary<string, int>(Sprites.Count);
			for(int i = 0; i < Sprites.Count; ++i)
			{
				Dict.Add(Sprites[i].name, i);
			}
		}
		public static Sprite GetSprite(string name)
		{
			if (Dict == null || Sprites == null)
				throw new Exception("EditorSprites is not ready, trying to get a Sprite before loading?");
			if (Dict.ContainsKey(name))
			{
				return Sprites[Dict[name]];
			}
			if (Sprites.Count == 0)
				throw new Exception("There are no EditorSprites, something went really wrong.");

			return Sprites[UnityEngine.Random.Range(0, Sprites.Count)];
		}
	}
}
