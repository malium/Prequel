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
	public static class Materials
	{
		public static Material[] Mat;

		public static Material GetMaterial(Def.Materials material)
		{
			return Mat[(int)material];
		}
		public static void Prepare()
		{
			Mat = new Material[Def.MaterialCount];
			for(int i = 0; i < AssetLoader.Materials.Length; ++i)
			{
				var mat = AssetLoader.Materials[i];
				var matName = mat.name.ToLower();
				switch(matName)
				{
					case "shadedmaterial":
						Mat[(int)Def.Materials.Default] = mat;
						break;
					case "cutoutmaterial":
						Mat[(int)Def.Materials.Cutout] = mat;
						break;
					case "fadematerial":
						Mat[(int)Def.Materials.Fade] = mat;
						break;
					case "transparentmaterial":
						Mat[(int)Def.Materials.Transparent] = mat;
						break;
					case "spritematerial":
						Mat[(int)Def.Materials.Sprite] = mat;
						break;
					//case "spriteunlit":
					//	Mat[(int)Def.Materials.SpriteUnlit] = mat;
					//	break;
					case "spritelit":
						Mat[(int)Def.Materials.SpriteLit] = mat;
						break;
					case "spritelitds":
						Mat[(int)Def.Materials.SpriteLitDS] = mat;
						break;
					case "coloredmat":
						Mat[(int)Def.Materials.ColoredDefault] = mat;
						break;
					case "coloredtransparentmat":
						Mat[(int)Def.Materials.ColoredTransparent] = mat;
						break;
					case "coloredcutoutmat":
						Mat[(int)Def.Materials.ColoredCutout] = mat;
						break;
					case "skymat":
						Mat[(int)Def.Materials.Background] = mat;
						break;
				}
			}
		}
	}
}
